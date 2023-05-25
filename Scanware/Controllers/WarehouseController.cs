using Scanware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Scanware.Data;
using Scanware.App_Objects;
using System.Data.SqlClient;

namespace Scanware.Controllers
{
    public class WarehouseShippingController : Controller
    {
        public ActionResult Index()
        {
            WarehouseModel viewModel = new WarehouseModel();

            viewModel.shipment_loads = shipment_load.GetTrucksOnSite();

            viewModel.shipment_loads_rail = shipment_load.GetOpenRailLoads();

            viewModel.shipment_loads_wh = warehoused_shipment_load.GetTrucksOnSite();

            viewModel.shipment_loads_rail_wh = warehoused_shipment_load.GetOpenRailLoads();

            return View(viewModel);
        }
    
    public ActionResult LoadTruck(string char_load_id, string Message, string Error)
    {
        WarehouseModel viewModel = new WarehouseModel();
        int load_id;

        if (char_load_id != null && char_load_id != "")
        {
            load_id = Int32.Parse(char_load_id);
        }
        else
        {
                load_id = 0;
        }

        List<int> subLoads = new List<int>();

        if (Message == "RESHIP")
        {
            viewModel.retry_shipping = true;
            Message = null;
        }
        else
        {
            viewModel.retry_shipping = false;
        }

        viewModel.ButlerSubLoads = false;

        ref_sys_param rsp = ref_sys_param.GetRefSysParam();

        viewModel.location = rsp.location;
        viewModel.Message = Message;
        viewModel.Error = Error;
        viewModel.searched_char_load_id = char_load_id;

        viewModel.shipment_loads = shipment_load.GetTrucksOnSite();
        viewModel.shipment_loads_wh = warehoused_shipment_load.GetTrucksOnSite();

        viewModel.warehouse_load = false;
        viewModel.warehouse_shipment = null;

        viewModel.shipment = shipment_load.GetShipmentLoad(char_load_id);
        if (viewModel.shipment == null && char_load_id != "" && char_load_id != null)
        {
                //Is warehouse load
                viewModel.warehouse_shipment = warehoused_shipment_load.GetShipmentLoad(load_id);

                if (viewModel.warehouse_shipment == null && char_load_id != "" && char_load_id != null)
                {
                    viewModel.Error = "Unable to find load " + char_load_id;
                    viewModel.load_images = new List<shipment_load_images>();
                }
                else
                {
                    viewModel.warehouse_load = true;
                }
        }
        else if (viewModel.shipment != null && viewModel.shipment.carrier_mode != "T")
        {
                viewModel.Error = "The load you entered does not exist as a truck load.";
                viewModel.shipment = null;
        }

        //Check if this is a subload - Butler
        if (rsp.location != "C" && char_load_id != null && viewModel.warehouse_load == false)
        {
                subLoads = shipment_load.isMasterLoad(char_load_id);

                if (subLoads.Count() > 0)
                {
                    viewModel.ButlerSubLoads = true;
                }
        }

        if (viewModel.shipment != null)
        {
            //if a subload is involved
            if (viewModel.shipment.char_load_id.Replace("-901", "") != viewModel.shipment.char_load_id)
            {
                viewModel.coils_in_shipment = load_dtl.GetLoadDtlForAllSubLoads(viewModel.shipment.char_load_id);

                //do not show -901 to shipping
                viewModel.shipment.char_load_id = viewModel.shipment.char_load_id.Replace("-901", "");
            }
            else
            {
                if (rsp.location != "C" && viewModel.ButlerSubLoads)
                {
                    viewModel.coils_in_shipment = load_dtl.GetLoadDtlSub(subLoads);
                }
                else
                {
                    viewModel.coils_in_shipment = load_dtl.GetLoadDtl(viewModel.shipment.load_id);
                }
            }

            viewModel.coils_in_shipment_weight = Convert.ToInt32(viewModel.coils_in_shipment.Sum(i => i.coil_weight));
            if (viewModel.shipment.load_id > 0)
                {
                    viewModel.load_id = viewModel.shipment.load_id;
                }
        }

        if (viewModel.warehouse_shipment != null)
        {
             viewModel.coils_in_shipment_wh = warehoused_load_dtl.GetLoadDtl(viewModel.warehouse_shipment.load_id);
             viewModel.coils_in_shipment_weight = Convert.ToInt32(viewModel.coils_in_shipment_wh.Sum(i => i.coil_weight));
             if (viewModel.warehouse_shipment.load_id > 0)
             {
                    viewModel.load_id = viewModel.warehouse_shipment.load_id;
             }
        }
            
        try
            {
                List<LoadsAndCoils> newLoad = new List<LoadsAndCoils>();
                //Populate coils in load for all Loads
                foreach (shipment_load load in viewModel.shipment_loads)
                {
                    LoadsAndCoils addLoad = new LoadsAndCoils();
                    addLoad.load_id = load.load_id;
                    addLoad.scale_time_in = Convert.ToDateTime(load.scale_time_in);
                    addLoad.vehicle_no = load.vehicle_no.Trim();
                    addLoad.pickup_no = load.PickUp_no;

                    subLoads = shipment_load.isMasterLoad(addLoad.load_id.ToString());

                    if (subLoads.Count() > 0)
                    {
                        addLoad.coils = load.GetcoilsInLoad(load.load_id, 1);
                    }
                    else
                    {
                        addLoad.coils = load.GetcoilsInLoad(load.load_id, 0);
                    }

                    newLoad.Add(addLoad);
                }

                foreach (warehoused_shipment_load load in viewModel.shipment_loads_wh)
                {
                    LoadsAndCoils addLoad2 = new LoadsAndCoils();
                    addLoad2.load_id = load.load_id;
                    addLoad2.scale_time_in = Convert.ToDateTime(load.scale_time_in);
                    addLoad2.vehicle_no = load.vehicle_no.Trim();

                    addLoad2.coils = load.GetcoilsInLoad(load.load_id);

                    newLoad.Add(addLoad2);
                }

                if (newLoad != null)
                {
                    viewModel.loads = newLoad;
                }

                if (viewModel.coils_in_shipment != null)
                {
                    if (viewModel.coils_in_shipment.Count() > 0)
                    {
                        if (viewModel.ButlerSubLoads)
                        {
                            viewModel.coilsInLoad = load_dtl.GetLoadDtlCoilsSubs(viewModel.shipment.load_id);
                        }
                        else
                        {
                            viewModel.coilsInLoad = load_dtl.GetLoadDtlCoils(viewModel.shipment.load_id);
                        }

                        Session["coilsInLoad"] = viewModel.coilsInLoad;
                    }
                    viewModel.load_images = shipment_load_images.GetLoadImages(viewModel.shipment.load_id);
                }
                else if (viewModel.coils_in_shipment_wh != null)
                {
                        if (viewModel.coils_in_shipment_wh.Count() > 0)
                        {                     
                            viewModel.coilsInLoad = warehoused_load_dtl.GetLoadDtlCoils(viewModel.warehouse_shipment.load_id);
                            Session["coilsInLoad"] = viewModel.coilsInLoad;
                        }

                        viewModel.load_images = shipment_load_images.GetLoadImages(viewModel.warehouse_shipment.load_id);
                }
                        
            }
            catch (Exception ex)
            {
                Error = "There was an error while getting Truck load data: " + char_load_id + ". " + ex.ToString().Substring(0, 200);
                viewModel.loads = new List<LoadsAndCoils>();
                viewModel.Error = Error;
                return PartialView("LoadTruckWarehouse", viewModel);
            }
            return PartialView("LoadTruckWarehouse", viewModel);

    }

    public ActionResult LoadRail(string char_load_id, string Message, string Error)
    {
            WarehouseModel viewModel = new WarehouseModel();

            function_level_security logged_in_user = new function_level_security(System.Web.HttpContext.Current.Session["function_level_security"] as List<function_level_security>);

            if (Message == "RESHIP")
            {
                viewModel.retry_shipping = true;
                Message = null;
            }
            else
            {
                viewModel.retry_shipping = false;
            }

            viewModel.Error = Error;
            viewModel.Message = Message;
            viewModel.searched_char_load_id = char_load_id;

            ref_sys_param rsp = ref_sys_param.GetRefSysParam();

            viewModel.location = rsp.location;

            int load_id;

            if (char_load_id != null && char_load_id != "")
            {
                load_id = Int32.Parse(char_load_id);
                viewModel.shipment = shipment_load.GetShipmentLoad(char_load_id);

                //happens when bad char_load_id passed in
                if (viewModel.shipment == null)
                {
                    //Is warehouse load
                    viewModel.warehouse_shipment = warehoused_shipment_load.GetShipmentLoad(load_id);

                    if (viewModel.warehouse_shipment == null)
                    {
                        viewModel.Error = "Unable to find rail load " + char_load_id;
                        viewModel.load_images = new List<shipment_load_images>();
                    }
                    else
                    {
                        viewModel.warehouse_load = true;
                    }
                }
                else if (viewModel.shipment != null && viewModel.shipment.carrier_mode != "R")
                {
                    viewModel.Error = "The load you entered does not exist as a rail load.";
                    viewModel.shipment = null;
                }

                if (viewModel.shipment != null)
                {

                    viewModel.coils_in_shipment = load_dtl.GetLoadDtl(viewModel.shipment.load_id);

                    viewModel.load_images = shipment_load_images.GetLoadImages(viewModel.shipment.load_id);

                    //if vehicle no not set then query db for all rail cars
                    if (viewModel.shipment.vehicle_no == null)
                    {
                        viewModel.rail_car_brands = rail_car_brand.GetAllRailCarBrands();
                    }

                    if (viewModel.shipment.load_id > 0)
                    { 
                        viewModel.load_id = load_id;
                    }
                }
                else if (viewModel.warehouse_shipment != null)
                {
                    viewModel.coils_in_shipment_wh = warehoused_load_dtl.GetLoadDtl(viewModel.warehouse_shipment.load_id);
                    viewModel.coils_in_shipment_weight = Convert.ToInt32(viewModel.coils_in_shipment_wh.Sum(i => i.coil_weight));
                    if (viewModel.warehouse_shipment.load_id > 0)
                    {
                        viewModel.load_id = viewModel.warehouse_shipment.load_id;
                    }

                    //if vehicle no not set then query db for all rail cars
                    if (viewModel.warehouse_shipment.vehicle_no == null)
                    {
                        viewModel.rail_car_brands = rail_car_brand.GetAllRailCarBrands();
                    }

                }
            }

            viewModel.shipment_loads_rail = shipment_load.GetOpenRailLoads();
            viewModel.shipment_loads_rail_wh = warehoused_shipment_load.GetOpenRailLoads();

            //Populate coils in load for all Loads
            List<LoadsAndCoils> newLoad = new List<LoadsAndCoils>();
            foreach (shipment_load load in viewModel.shipment_loads_rail)
            {
                    LoadsAndCoils addLoad = new LoadsAndCoils();
                    addLoad.load_id = load.load_id;
                    addLoad.scale_time_in = Convert.ToDateTime(load.scale_time_in);
                    if (load.vehicle_no == null)
                    {
                        addLoad.vehicle_no = "";
                    }
                    else
                    {
                        addLoad.vehicle_no = load.vehicle_no.Trim();
                    }

                    if (viewModel.ButlerSubLoads)
                    {
                        addLoad.coils = load.GetcoilsInLoad(load.load_id, 1);
                    }
                    else
                    {
                        addLoad.coils = load.GetcoilsInLoad(load.load_id, 0);
                    }
                    if (load.rail_car_number != null)
                    {
                        load.vehicle_no = load.vehicle_no + "-" + load.rail_car_number;
                    }
                    newLoad.Add(addLoad);
            }

            foreach (warehoused_shipment_load load in viewModel.shipment_loads_rail_wh)
            {

                LoadsAndCoils addLoad = new LoadsAndCoils();
                addLoad.load_id = load.load_id;
                addLoad.scale_time_in = Convert.ToDateTime(load.scale_time_in);
                if (load.vehicle_no == null)
                {
                    addLoad.vehicle_no = "";
                }
                else
                {
                    addLoad.vehicle_no = load.vehicle_no.Trim();
                }

                addLoad.coils = load.GetcoilsInLoad(load.load_id);

                //if (load.rail_car_number != null)
                //{
                //    load.vehicle_no = load.vehicle_no + "-" + load.rail_car_number;
                //}
                newLoad.Add(addLoad);
            }

            if (newLoad != null)
                {
                    viewModel.loads = newLoad;
                }

                if (viewModel.coils_in_shipment != null)
                {
                    if (viewModel.coils_in_shipment.Count() > 0)
                    {
                        if (viewModel.ButlerSubLoads)
                        {
                            viewModel.coilsInLoad = load_dtl.GetLoadDtlCoilsSubs(viewModel.shipment.load_id);
                        }
                        else
                        {
                            viewModel.coilsInLoad = load_dtl.GetLoadDtlCoils(viewModel.shipment.load_id);
                        }

                        Session["coilsInLoad"] = viewModel.coilsInLoad;
                    }
                }
                else if (viewModel.coils_in_shipment_wh != null)
                {
                    if (viewModel.coils_in_shipment_wh.Count() > 0)
                    {
                        viewModel.coilsInLoad = warehoused_load_dtl.GetLoadDtlCoils(viewModel.warehouse_shipment.load_id);
                        Session["coilsInLoad"] = viewModel.coilsInLoad;
                    }

                    viewModel.load_images = shipment_load_images.GetLoadImages(viewModel.warehouse_shipment.load_id);
                }


            return PartialView("LoadRailWarehouse", viewModel);
        }
    public ActionResult StartRailLoad(string char_load_id, string rail_car_brand, string vehicle_no, byte? rail_car_no = null)
        {
            WarehouseModel viewModel = new WarehouseModel();

            shipment_load current_load = shipment_load.GetShipmentLoad(char_load_id);
            int load_id;

            if (char_load_id != null && char_load_id != "")
            {
                load_id = Int32.Parse(char_load_id);
            }
            else
            {
                throw new Exception(char_load_id + " is not found.");
            }

            //Is Warehouse Rail Load
            if (current_load == null)
            {
                //Is warehouse load
                viewModel.warehouse_shipment = warehoused_shipment_load.GetShipmentLoad(load_id);

                if (viewModel.warehouse_shipment == null)
                {
                    throw new Exception(char_load_id + " is not found.");
                }
                else
                {
                    //Call Ship truck load Warehouse
                    return RedirectToAction("StartRailLoadWarehouse", "WarehouseShipping", new {char_load_id = char_load_id, rail_car_brand = rail_car_brand, vehicle_no = vehicle_no, rail_car_no = rail_car_no});
                }
            }

            ref_sys_param rsp = ref_sys_param.GetRefSysParam();

            rail_cars current_rail_car;

            if (vehicle_no == null || vehicle_no.Length < 1)
            {
                current_rail_car = null;
            }
            else
            {
                //concatenated brand to vehicle no
                vehicle_no = rail_car_brand + vehicle_no;

                current_rail_car = rail_cars.GetRailCar(vehicle_no);
            }

            //make sure load exists
            if (current_load == null || (current_load.carrier_mode != "R" && current_load.carrier_mode != "B"))
            {
                viewModel.Error = "Unable to find rail load " + char_load_id;
            }
            else if (current_rail_car == null)
            {
                viewModel.Error = "Unable to find rail car " + vehicle_no;
            }
            else
            {
                application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

                List<shipment_load> loads = new List<shipment_load>();

                //if sub loads involved
                if (current_load.char_load_id.Replace("-901", "") != current_load.char_load_id)
                {
                    loads = shipment_load.GetShipmentLoadSubLoads(current_load.char_load_id);
                }
                else
                {
                    //just add the current load
                    loads.Add(current_load);
                }

                foreach (shipment_load sl in loads)
                {                  
                        string message = shipment_load.UpdateNewRailLoad(sl.load_id, vehicle_no, current_application_security, rail_car_no);
                        if (message == "SUCCESS")
                        {
                            viewModel.Message = "Rail load " + char_load_id + " with rail car " + vehicle_no + " is now checked-in.";
                        }
                        else if (message == "LOAD SHIPPED")
                        {
                            viewModel.Error = "Rail load " + char_load_id + " has already shipped!";
                        }
                        else
                        {
                            viewModel.Message = "Rail load " + char_load_id + " with rail car " + vehicle_no + " is now checked-in.";
                            viewModel.Error = "This vehicle's gross weight exceeds the maximum weight allowed (286,000 LBS)!";
                        }

                    List<load_dtl> current_coils = load_dtl.GetLoadDtl(sl.load_id);

                    //add audit record
                    foreach (load_dtl coil in current_coils)
                    {
                        Utils.AddAuditRecord(coil.production_coil_no, "Rail Load Started", sl.char_load_id + " - " + vehicle_no);
                    }
                }
            }

            return RedirectToAction("LoadRail", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
        }

    public ActionResult StartRailLoadWarehouse(string char_load_id, string rail_car_brand, string vehicle_no, byte? rail_car_no = null)
    {
            WarehouseModel viewModel = new WarehouseModel();

            int load_id;

            if (char_load_id != null && char_load_id != "")
            {
                load_id = Int32.Parse(char_load_id);
            }
            else
            {
                throw new Exception(char_load_id + " is not found.");
            }

            warehoused_shipment_load current_load = warehoused_shipment_load.GetShipmentLoad(load_id);

            ref_sys_param rsp = ref_sys_param.GetRefSysParam();

            rail_cars current_rail_car;

            if (vehicle_no == null || vehicle_no.Length < 1)
            {
                current_rail_car = null;
            }
            else
            {
                //concatenated brand to vehicle no
                vehicle_no = rail_car_brand + vehicle_no;

                current_rail_car = rail_cars.GetRailCar(vehicle_no);
            }

            //make sure load exists
            if (current_load == null || (current_load.carrier_mode != "R" && current_load.carrier_mode != "B"))
            {
                viewModel.Error = "Unable to find rail load " + char_load_id;
            }
            else if (current_rail_car == null)
            {
                viewModel.Error = "Unable to find rail car " + vehicle_no;
            }
            else
            {
                application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

                List<warehoused_shipment_load> loads = new List<warehoused_shipment_load>();

                //just add the current load
                loads.Add(current_load);
                
                foreach (warehoused_shipment_load sl in loads)
                {
                    string message = warehoused_shipment_load.UpdateNewRailLoad(sl.load_id, vehicle_no, current_application_security, rail_car_no);

                    if (message == "SUCCESS")
                    {
                        viewModel.Message = "Rail load " + char_load_id + " with rail car " + vehicle_no + " is now checked-in.";
                    }
                    else if (message == "LOAD SHIPPED")
                    {
                        viewModel.Error = "Rail load " + char_load_id + " has already shipped!";
                    }
                    else
                    {
                        viewModel.Message = "Rail load " + char_load_id + " with rail car " + vehicle_no + " is now checked-in.";
                        viewModel.Error = "This vehicle's gross weight exceeds the maximum weight allowed (286,000 LBS)!";
                    }

                    List<warehoused_load_dtl> current_coils = warehoused_load_dtl.GetLoadDtl(sl.load_id);

                    //add audit record
                    foreach (warehoused_load_dtl coil in current_coils)
                    {
                        Utils.AddAuditRecordWarehouse(coil.production_coil_no, "Rail Load Started", sl.char_load_id + " - " + vehicle_no);
                    }
                }
            }

            return RedirectToAction("LoadRail", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
        }
        public ActionResult ViewShipmentLoadImage(int image_no)
    {
            WarehouseModel viewModel = new WarehouseModel();

            viewModel.load_image = shipment_load_images.GetLoadImage(image_no);
            viewModel.load_image_bytes = v_shipment_load_images.GetLoadImage(image_no);
            viewModel.shipment = shipment_load.GetShipmentLoad(viewModel.load_image.load_id);
            
            if (viewModel.shipment == null)
            {
                viewModel.warehouse_shipment = warehoused_shipment_load.GetShipmentLoad(viewModel.load_image.load_id);
            }

            return View(viewModel);
    }

        [HttpPost]
        public ActionResult UploadFiles(int load_id, HttpPostedFileBase[] upload_files)
        {
            WarehouseModel viewModel = new WarehouseModel();

            shipment_load sl = shipment_load.GetShipmentLoad(load_id);

            if (upload_files == null)
            {
                viewModel.Error = "Files are empty, use the back button and click the browse button to select an image";
            }
            else
            {
                //iterating through multiple file collection   
                foreach (HttpPostedFileBase upload_file in upload_files)
                {
                    try
                    {
                        byte[] documentBytes = new byte[upload_file.InputStream.Length];

                        upload_file.InputStream.Read(documentBytes, 0, documentBytes.Length);

                        application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

                        usp.usp_add_shipment_load_image(load_id, current_application_security.user_id, documentBytes);

                        viewModel.Message = "Image uploaded for " + sl.load_id.ToString() + "/" + sl.char_load_id;
                    }
                    catch (Exception ex)
                    {
                        viewModel.Error = ex.ToString();
                    }
                }
            }

            if (sl.carrier_mode == "R")
            {
                return RedirectToAction("LoadRail", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = sl.char_load_id });
            }
            else //if (sl.carrier_mode == "T")
            {
                return RedirectToAction("LoadTruck", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = sl.char_load_id });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadShipmentLoadImage(int load_id, HttpPostedFileBase upload_file)
        {
            WarehouseModel viewModel = new WarehouseModel();

            shipment_load sl = shipment_load.GetShipmentLoad(load_id);
            warehoused_shipment_load sl2 = new warehoused_shipment_load();

            string char_load_id = load_id.ToString();

            string carrier_mode = "";

            if (sl == null)
            {
                sl2 = warehoused_shipment_load.GetShipmentLoad(load_id);
            }

            if (upload_file == null)
            {
                viewModel.Error = "File is empty, use the back button and click the browse button to select an image";
            }
            else
            {
                try
                {
                    byte[] documentBytes = new byte[upload_file.InputStream.Length];

                    upload_file.InputStream.Read(documentBytes, 0, documentBytes.Length);

                    application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

                    usp.usp_add_shipment_load_image(load_id, current_application_security.user_id, documentBytes);

                    viewModel.Message = "Image uploaded for " + load_id.ToString();
                }
                catch (Exception ex)
                {
                    viewModel.Error = ex.ToString();
                }
            }

            if(sl != null)
            {
                carrier_mode = sl.carrier_mode;
            }
            else
            {
                carrier_mode = sl2.carrier_mode;
            }

            if (carrier_mode == "R")
            {
                return RedirectToAction("LoadRail", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
            }
            else //if (sl.carrier_mode == "T")
            {
                return RedirectToAction("LoadTruck", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadShipmentLoadImages(int load_id, HttpPostedFileBase[] upload_files)
        {
            WarehouseModel viewModel = new WarehouseModel();

            shipment_load sl = shipment_load.GetShipmentLoad(load_id);

            warehoused_shipment_load sl2 = new warehoused_shipment_load();

            string char_load_id = load_id.ToString();

            string carrier_mode = "";

            if (sl == null)
            {
                sl2 = warehoused_shipment_load.GetShipmentLoad(load_id);
            }

            if (upload_files == null)
            {
                viewModel.Error = "File is empty, use the back button and click the browse button to select an image";
            }
            else
            {
                try
                {
                    application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

                    foreach (HttpPostedFileBase file in upload_files)
                    {
                        //Checking file is available to save.  
                        if (file != null)
                        {
                            byte[] documentBytes = new byte[file.InputStream.Length];
                            file.InputStream.Read(documentBytes, 0, documentBytes.Length);
                            usp.usp_add_shipment_load_image(load_id, current_application_security.user_id, documentBytes);
                        }
                        viewModel.Message = upload_files.Count().ToString() + " Images uploaded for " + load_id.ToString();

                    }
                }
                catch (Exception ex)
                {
                    viewModel.Error = ex.ToString();
                }
            }

            if (sl != null)
            {
                carrier_mode = sl.carrier_mode;
            }
            else
            {
                carrier_mode = sl2.carrier_mode;
            }

            if (carrier_mode == "R")
            {
                return RedirectToAction("LoadRail", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
            }
            else //if (sl.carrier_mode == "T")
            {
                return RedirectToAction("LoadTruck", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
            }
        }

        [HttpPost]
        public ActionResult VerifiedCoilsPartial(string production_coil_no, int load_id, string inputName, string scanner_used)
        {
            string error = "";

            WarehouseModel viewModel = new WarehouseModel();

            if (scanner_used == null)
                scanner_used = "N";

            if (production_coil_no != "" && production_coil_no != null)
            {
                viewModel.shipment = shipment_load.GetShipmentLoad(load_id);

                //check if coil exists
                if (viewModel.shipment == null)
                {
                    //Is warehouse load
                    viewModel.warehouse_shipment = warehoused_shipment_load.GetShipmentLoad(load_id);

                    if (viewModel.warehouse_shipment == null && load_id != 0)
                    {
                        viewModel.Error = "unable to find " + production_coil_no + "!";
                    }
                    else
                    {
                        viewModel.warehouse_load = true;
                    }
                }
            }

            List<CoilsInLoad> fromDB = new List<CoilsInLoad>();

            if (viewModel.warehouse_load == false)
            {
                fromDB = load_dtl.GetLoadDtlCoils(load_id);
            }
            else
            {
                fromDB = warehoused_load_dtl.GetLoadDtlCoils(load_id);
            }

            List<CoilsInLoad> model = Session["coilsInLoad"] as List<CoilsInLoad>;

            foreach (var coil in model)
            {
                var coilDB = fromDB.Find(x => x.production_coil_no == coil.production_coil_no);

                if (coilDB != null && coilDB.verified > Convert.ToDateTime("1/1/2019 12:00 AM"))
                {
                    coil.coilOnPaperwork = production_coil_no;
                    coil.coilShipTag = production_coil_no;
                }
                else if (coil.production_coil_no.Trim() == production_coil_no)
                {
                    coil.Message = error;
                    if (error == "")
                    {
                        switch (inputName)
                        {
                            case ("shippingTag"):
                                coil.coilOnPaperwork = production_coil_no;
                                break;
                            case ("coilTag"):
                                coil.coilShipTag = production_coil_no;
                                break;
                            default:
                                break;
                        }
                    }

                    if (coil.coilOnPaperwork == production_coil_no && coil.coilShipTag == production_coil_no)
                    {
                        application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

                        if (viewModel.warehouse_load == false)
                        {
//                            load_dtl.UpdateCoilScannedDt(production_coil_no, DateTime.Now, current_application_security.user_id);
                            load_dtl.UpdateCoilScannedDt_mx(production_coil_no, DateTime.Now, current_application_security.user_id, load_id);

                        }
                        else
                        {
                            warehoused_load_dtl.UpdateCoilScannedDt(production_coil_no, DateTime.Now, current_application_security.user_id);
                        }

                        if (viewModel.warehouse_load == true)
                        { 
                            Utils.AddAuditRecordWarehouse(production_coil_no, "Verified", load_id.ToString(), scanner_used);
                        }
                        else
                        {
                            Utils.AddAuditRecord(production_coil_no, "Verified", load_id.ToString(), scanner_used);
                        }
                    }
                }
                else
                {
                    viewModel.Error = "Coil No " + production_coil_no + "Is not part of this Load!";
                }
            }
            //Add a loop to check 'Verified' column for each Coil - If Verified: set both properties for that coil (in case 2 loaders work on the same load in 2 seperate sessions)

            Session["coilsInLoad"] = model;

            viewModel.LoadVerified = CoilsInLoad.GetLoadDtlCoilsStatus(model);

            if (viewModel.LoadVerified)
            {
                Session["LoadVerified"] = load_id;
            }

            return PartialView("VerifiedCoilsPartial", model);
        }
        public ActionResult VerifyCoil(string ship_tag, string production_coil_no, string char_load_id, bool? unverify, int? scannerUsed)
        {
            WarehouseModel viewModel = new WarehouseModel();

            int load_id;

            if (char_load_id != null && char_load_id != "")
            {
                load_id = Int32.Parse(char_load_id);
            }
            else
            {
                load_id = 0;
            }

            try
            {
                application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

                viewModel.shipment = shipment_load.GetShipmentLoad(char_load_id);

                if (viewModel.shipment == null)
                {
                    //Is warehouse load
                    viewModel.warehouse_shipment = warehoused_shipment_load.GetShipmentLoad(load_id);

                    if (viewModel.warehouse_shipment != null && load_id != 0)
                    {
                        viewModel.warehouse_load = true;
                    }
                    else
                    { 
                        throw new Exception(char_load_id + " is not found.");
                    }
                }

                ref_sys_param rsp = ref_sys_param.GetRefSysParam();

                if (rsp.location != "C")
                {
                    if (production_coil_no.Substring(0, 1) == "S")
                    {
                        production_coil_no = production_coil_no.Substring(1);
                    }
                }
                
                all_produced_coils current_all_produced_coil = null;

                //SDI Coils
                if (viewModel.warehouse_load == false)
                {
                    if (production_coil_no != "")
                    {
                        current_all_produced_coil = all_produced_coils.GetAllProducedCoilByProductionCoilNumberOrTagNumber(production_coil_no);
                    }
                    else
                    {
                        current_all_produced_coil = all_produced_coils.GetAllProducedCoilByTagNumber(ship_tag.Substring(1));
                    }

                    //get coil record
                    load_dtl verify_coil = load_dtl.GetLoadDtl(current_all_produced_coil.production_coil_no);

                    //get shipment record tied to coil
                    shipment_load coil_shipment = new shipment_load();

                    List<int> subLoads = new List<int>();

                    //Check if this is a subload - Butler
                    if (rsp.location != "C" && char_load_id != null)
                    {
                        subLoads = shipment_load.isMasterLoad(char_load_id);

                        if (subLoads.Count() > 0)
                        {
                            viewModel.ButlerSubLoads = true;
                        }
                    }

                    //if sub loads involved
                    if (viewModel.shipment.char_load_id.Replace("-901", "") != viewModel.shipment.char_load_id)
                    {
                        coil_shipment = shipment_load.GetShipmentLoad(viewModel.shipment.char_load_id, current_all_produced_coil.production_coil_no);
                    }
                    else
                    {
                        //If A Butler subLoad
                        if (rsp.location != "C" && viewModel.ButlerSubLoads)
                        {
                            coil_shipment = shipment_load.GetShipmentSubLoad(Convert.ToInt32(viewModel.shipment.char_load_id), current_all_produced_coil.production_coil_no);
                        }
                        else
                        {
                            coil_shipment = viewModel.shipment;
                        }
                    }

                    if (coil_shipment == null)
                    {
                        throw new Exception(production_coil_no + " is not found on load.");
                    }

                    //confirm shipping is on the correct load screen when verifying
                    if (coil_shipment.load_id != verify_coil.load_id)
                    {
                        throw new Exception(production_coil_no + " is not found on load " + coil_shipment.char_load_id + ".");
                    }

                    if (!unverify.HasValue || unverify.Value == false)
                    {
                        //has coil been previously verified
                        if (verify_coil.coil_scanned_dt != null)
                        {
                            throw new Exception(production_coil_no + " is already verified");
                        }

                        //do tags match
                        if (!ship_tag.Substring(1).Trim().ToUpper().Equals(production_coil_no.Trim().ToUpper()))
                        {
                            throw new Exception(production_coil_no + " (coil) and " + ship_tag + " (ship tag) are not compatible");
                        }

                        load_dtl.UpdateCoilScannedDt(current_all_produced_coil.production_coil_no, DateTime.Now, current_application_security.user_id);

                        if (rsp.location != "C")
                        {
                            string scanner_used;

                            if (scannerUsed == 1)
                                scanner_used = "Y";
                            else
                                scanner_used = "N";

                            Utils.AddAuditRecord(verify_coil.production_coil_no, "Unverified", coil_shipment.char_load_id, scanner_used);
                        }
                        else
                        {
                            //comment used in messaging to lomas, do not change
                            Utils.AddAuditRecord(verify_coil.production_coil_no, "Verified", coil_shipment.char_load_id);
                        }
                        viewModel.Message = production_coil_no + " has been verified on load " + char_load_id;

                    }
                    else
                    {
                        if (verify_coil.coil_scanned_dt == null)
                        {
                            throw new Exception(production_coil_no + " is not verified, uncheck unverified option and try again");
                        }

                        //is coil on load
                        if (verify_coil.load_id != coil_shipment.load_id)
                        {
                            throw new Exception(production_coil_no + " is not found on load " + coil_shipment.char_load_id);
                        }

                        load_dtl.UpdateCoilScannedDt(current_all_produced_coil.production_coil_no, null, current_application_security.user_id);

                        if (rsp.location != "C")
                        {
                            string scanner_used;

                            if (scannerUsed == 1)
                                scanner_used = "Y";
                            else
                                scanner_used = "N";

                            Utils.AddAuditRecord(verify_coil.production_coil_no, "Unverified", coil_shipment.char_load_id, scanner_used);
                        }
                        else
                        {
                            Utils.AddAuditRecord(verify_coil.production_coil_no, "Unverified", coil_shipment.char_load_id);
                        }

                        viewModel.Message = production_coil_no + " has been unverified on load " + char_load_id;
                    }

                }
                //Warehouse Coils
                else
                {
                    //get coil record
                    warehoused_load_dtl verify_coil = warehoused_load_dtl.GetLoadDtl(production_coil_no);

                    //get shipment record tied to coil
                    warehoused_shipment_load coil_shipment = new warehoused_shipment_load();

                    if (coil_shipment == null)
                    {
                        throw new Exception(production_coil_no + " is not found on load.");
                    }

                    //confirm shipping is on the correct load screen when verifying
                    if (coil_shipment.load_id != verify_coil.load_id)
                    {
                        throw new Exception(production_coil_no + " is not found on load " + coil_shipment.char_load_id + ".");
                    }

                    if (!unverify.HasValue || unverify.Value == false)
                    {
                        //has coil been previously verified
                        if (verify_coil.coil_scanned_dt != null)
                        {
                            throw new Exception(production_coil_no + " is already verified");
                        }

                        //do tags match
                        if (!ship_tag.Substring(1).Trim().ToUpper().Equals(production_coil_no.Trim().ToUpper()))
                        {
                            throw new Exception(production_coil_no + " (coil) and " + ship_tag + " (ship tag) are not compatible");
                        }

                        load_dtl.UpdateCoilScannedDt(current_all_produced_coil.production_coil_no, DateTime.Now, current_application_security.user_id);

                        if (rsp.location != "C")
                        {
                            string scanner_used;

                            if (scannerUsed == 1)
                                scanner_used = "Y";
                            else
                                scanner_used = "N";

                            Utils.AddAuditRecord(verify_coil.production_coil_no, "Unverified", coil_shipment.char_load_id, scanner_used);
                        }
                        else
                        {
                            //comment used in messaging to lomas, do not change
                            Utils.AddAuditRecord(verify_coil.production_coil_no, "Verified", coil_shipment.char_load_id);
                        }
                        viewModel.Message = production_coil_no + " has been verified on load " + char_load_id;

                    }
                    else
                    {
                        if (verify_coil.coil_scanned_dt == null)
                        {
                            throw new Exception(production_coil_no + " is not verified, uncheck unverified option and try again");
                        }

                        //is coil on load
                        if (verify_coil.load_id != coil_shipment.load_id)
                        {
                            throw new Exception(production_coil_no + " is not found on load " + coil_shipment.char_load_id);
                        }

                        load_dtl.UpdateCoilScannedDt(current_all_produced_coil.production_coil_no, null, current_application_security.user_id);

                        if (rsp.location != "C")
                        {
                            string scanner_used;

                            if (scannerUsed == 1)
                                scanner_used = "Y";
                            else
                                scanner_used = "N";

                            Utils.AddAuditRecord(verify_coil.production_coil_no, "Unverified", coil_shipment.char_load_id, scanner_used);
                        }
                        else
                        {
                            Utils.AddAuditRecord(verify_coil.production_coil_no, "Unverified", coil_shipment.char_load_id);
                        }

                        viewModel.Message = production_coil_no + " has been unverified on load " + char_load_id;
                    }
                }

            }
            //no action on failure, just pass error back to user
            catch (Exception ex)
            {
                viewModel.Error = ex.ToString();
            }

            if (viewModel.shipment.carrier_mode == "T")
            {
                return RedirectToAction("LoadTruck", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
            }
            else //(carrier_mode == "R")
            {
                return RedirectToAction("LoadRail", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        // Set all Coils Verified
        public string AllCoilsVerified(int load_id)
        {
            WarehouseModel viewModel = new WarehouseModel();
            string char_load_id;
            string error = "SUCCESS";
            ref_sys_param rsp = ref_sys_param.GetRefSysParam();

            try
            {
                char_load_id = load_id.ToString();

                viewModel.shipment = shipment_load.GetShipmentLoad(char_load_id);

                if (viewModel.shipment == null)
                {
                    //Is warehouse load
                    viewModel.warehouse_shipment = warehoused_shipment_load.GetShipmentLoad(load_id);

                    if (viewModel.warehouse_shipment == null)
                    {
                        throw new Exception(char_load_id + " is not found.");
                    }
                    else
                    {
                        viewModel.warehouse_load = true;
                    } 
                }

                if (viewModel.warehouse_load == true)
                {
                    viewModel.coilsInLoad = warehoused_load_dtl.GetLoadDtlCoils(load_id);
                }
                else
                { 
                    viewModel.coilsInLoad = load_dtl.GetLoadDtlCoils(load_id);
                }

                application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

                foreach (var verify_coil in viewModel.coilsInLoad)
                {
                    if (verify_coil.verified < DateTime.Parse("1/1/2019"))
                    {
                        return "Not all Coils have been verified on this load!";
                    };
                }
            }
            //no action on failure, just pass error back to user
            catch (Exception ex)
            {
                error = ex.ToString();
            }

            if (error == "SUCCESS")
            {
                    Session["ShipLoad"] = load_id.ToString();               
            }

            return error;
        }

        public ActionResult ShipTruckLoad(string char_load_id, int? ScannerUsed)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            ref_sys_param rsp = ref_sys_param.GetRefSysParam();
            int load_id = Int32.Parse(char_load_id);

            WarehouseModel viewModel = new WarehouseModel();

            viewModel.retry_shipping = false;

            shipment_load current_load = shipment_load.GetShipmentLoad(char_load_id);

            if (current_load == null)
            {
                //Is warehouse load
                viewModel.warehouse_shipment = warehoused_shipment_load.GetShipmentLoad(load_id);

                if (viewModel.warehouse_shipment == null)
                {
                    throw new Exception(char_load_id + " is not found.");
                }
                else
                {
                    //Call Ship truck load Warehouse
                    return RedirectToAction("ShipTruckLoadWarehouse", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
                }
            }

            //Verify that a load was checked-in
            if ((current_load.scale_weight_in == null || current_load.scale_weight_in == 0) || (current_load.initial_in == null) || (current_load.vehicle_no == null))
            {
                viewModel.Error = "Missing Truck Check-In Information " + char_load_id;
                viewModel.all_coils_verified = false;
                return RedirectToAction("LoadTruck", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
            }

            if (current_load != null)
            {
                List<int> subLoads = new List<int>();

                List<shipment_load> current_loads = new List<shipment_load>();
                List<load_dtl> current_coils = new List<load_dtl>();

                //Check if this is a subload - Butler
                //if (rsp.location != "C" && char_load_id != null)
                //{
                //    subLoads = shipment_load.isMasterLoad(char_load_id);

                //    if (subLoads.Count() > 0)
                //    {
                //        current_loads = db.shipment_load.Where(x => x.master_load_id == load_id).ToList();
                //    }
                //    else
                //    {
                //        current_loads = db.shipment_load.Where(x => x.load_id == load_id).ToList();
                //    }
                //}

                //if a subload is involved
                if (current_load.char_load_id.Replace("-901", "") != current_load.char_load_id)
                {
                    current_coils = load_dtl.GetLoadDtlForAllSubLoads(current_load.char_load_id);
                }
                else
                {
                    if (rsp.location == "C")
                    {
                        current_coils = load_dtl.GetLoadDtl(current_load.load_id);
                    }
                    else
                    {
                        //If sub Loads involved Butler
                        if (subLoads.Count() > 0)
                        {
                            current_coils = load_dtl.GetLoadDtlSub(subLoads);
                        }
                        else
                        {
                            current_coils = load_dtl.GetLoadDtl(current_load.load_id);
                        }
                    }
                }

                bool all_coils_verified = true;

                foreach (load_dtl coil in current_coils)
                {
                    if (coil.coil_scanned_dt == null)
                    {
                        all_coils_verified = false;
                    }
                }

                if (all_coils_verified)
                {
                    application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

                    List<shipment_load> loads = new List<shipment_load>();

                    //if sub loads involved - Columbus
                    if (current_load.char_load_id.Replace("-901", "") != current_load.char_load_id)
                    {
                        loads = shipment_load.GetShipmentLoadSubLoads(current_load.char_load_id);
                    }
                    else
                    {
                        if ((rsp.location != "C") && (subLoads.Count() > 0))
                        {
                            loads = current_loads;
                        }
                        else
                        {
                            //just add the current load
                            loads.Add(current_load);
                        }
                    }

                    foreach (shipment_load load in loads)
                    {
                        shipment_load.UpdateLoadStatusAndLoadingUser(load.load_id, "SH", current_application_security.user_id);

                        if (rsp.location == "T")
                        {
                            return RedirectToAction("CheckInOutTruck", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
                        }
                        else if (rsp.location != "C")
                        {
                            string result = usp.usp_call_spc_set_load_shipped_v2(load.char_load_id);

                            if (result != "SUCCESS")
                            {
                                application_settings re_ship = application_settings.GetAppSetting("RESHIP");

                                if (re_ship.default_value == "Y")
                                {
                                    //Reset Loads status back to 'Pickup', clean scanware_shipping_loads entries and Retry Shipping Procedure once

                                    //Wait 5 seconds and retry to ship
                                    System.Threading.Thread.Sleep(5000);

                                    foreach (shipment_load load2 in loads)
                                    {
                                        shipment_load.UpdateLoadStatusAndLoadingUser(load2.load_id, "PI", current_application_security.user_id);
                                        scanware_shipping_loads.remove(load2.load_id);
                                    }

                                    viewModel.Message = "RESHIP";

                                    //Send alert to shipping supervisors
                                    int msg_delivery_id;
                                    string to_address2;
                                    DateTime now = DateTime.Now;

                                    var system_email = rsp.default_from_user;

                                    try
                                    {
                                        List<string> to_address = db.Database.SqlQuery<string>("SELECT email_address FROM conversation_email WHERE email_type = 'BD'").ToList();
                                        to_address2 = string.Join(",", to_address.ToArray());

                                        msg_delivery_id = db.Database.SqlQuery<int>("EXEC call_spc_up_seq_no 'msg_delivery_id'").SingleOrDefault();

                                        var sql = "INSERT INTO dbo.msg_delivery_queue" +
                                              "(delivery_id, delivery_method_id, from_name, from_address, to_name, to_address, subject, message, add_datetime," +
                                               "add_user_id, send_datetime, send_status_id, from_appname, retry_count, append, notification_sent, customer_id, amyuni_processed)" +
                                           "VALUES" +
                                               "(@msg_delivery_id, 1, 'Scanware Process', '" + system_email + "', @to_address, @to_address" +
                                               ", 'Truck load shipping process problem'" +
                                               ", 'An Error occured while trying to Ship Load #" + load.char_load_id + ". Please verify that all information got updated and the Coils shipped!'" +
                                               ", @now, @user_id, null, 0, 'SCANWARE', 0, 'N', 'N', 0, 'N')";

                                        db.Database.SqlQuery<List<string>>(sql, new SqlParameter("@msg_delivery_id", msg_delivery_id),
                                                                       new SqlParameter("@to_address", to_address2),
                                                                       new SqlParameter("@now", now),
                                                                       new SqlParameter("@user_id", current_application_security.user_id)).SingleOrDefault();
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception(char_load_id + " Shipping Procedure Failed to Ship Load. Please contact Level 3!" + ex.ToString());
                                    }
                                }
                                viewModel.Error = char_load_id + " Shipping Procedure Failed to Ship Load. " + result + ". Please contact Level 3!";
                                return RedirectToAction("LoadTruck", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
                            }

                            string error = shipment_load.UpdateScaleTimeOutButler(load.load_id, current_application_security.user_id);

                            if (error != "SUCCESS")
                            {
                                viewModel.Error = error + ". Please contact Level 3!";
                                return RedirectToAction("LoadTruck", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
                            }

                            //If sub-load - get weight out value from Master Load
                            if (subLoads.Count() > 0)
                            {
                                shipment_load master_load = db.shipment_load.Where(x => x.load_id == load_id).FirstOrDefault();
                                int? master_weight_out = master_load.scale_weight_in + master_load.total_weight_load;

                                if ((master_weight_out != null) && (master_weight_out > 0))
                                {
                                    shipment_load.SetScaleWeightOut(load.load_id, master_weight_out, current_application_security.user_id);
                                }
                            }
                        }
                    }

                    //add audit record
                    foreach (load_dtl coil in current_coils)
                    {
                        string scanner_user;

                        if (ScannerUsed == 1)
                            scanner_user = "Y";
                        else
                            scanner_user = "N";

                        Utils.AddAuditRecord(coil.production_coil_no, "Truck Load Shipped", current_load.char_load_id, scanner_user);
                    }

                    viewModel.Message = char_load_id + " has been shipped in the system";
                    return RedirectToAction("LoadTruck", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error });
                }
                else
                {
                    viewModel.Error = "Not all coils are verified on " + char_load_id;
                    return RedirectToAction("LoadTruck", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
                }
            }
            else
            {
                viewModel.Error = "Unable to find " + char_load_id;
                return RedirectToAction("LoadTruck", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
            }
        }

        public ActionResult ShipTruckLoadWarehouse(string char_load_id, int? ScannerUsed)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            ref_sys_param rsp = ref_sys_param.GetRefSysParam();
            int load_id = Int32.Parse(char_load_id);

            WarehouseModel viewModel = new WarehouseModel();

            viewModel.retry_shipping = false;

            warehoused_shipment_load current_load = warehoused_shipment_load.GetShipmentLoad(load_id);

            if (current_load == null)
            {
                throw new Exception(char_load_id + " is not found.");            
            }

            //Verify that a load was checked-in
            if ((current_load.scale_weight_in == null || current_load.scale_weight_in == 0) || (current_load.initial_in == null) || (current_load.vehicle_no == null))
            {
                viewModel.Error = "Missing Truck Check-In Information " + char_load_id;
                viewModel.all_coils_verified = false;
                return RedirectToAction("LoadTruck", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
            }

            if (current_load != null)
            {

                List<warehoused_shipment_load> current_loads = new List<warehoused_shipment_load>();
                List<warehoused_load_dtl> current_coils = new List<warehoused_load_dtl>();

                current_loads = db.warehoused_shipment_load.Where(x => x.load_id == load_id).ToList();

                current_coils = warehoused_load_dtl.GetLoadDtl(current_load.load_id);

                bool all_coils_verified = true;

                foreach(warehoused_load_dtl coil in current_coils)
                {
                    if (coil.coil_scanned_dt == null)
                    {
                        all_coils_verified = false;
                    }
                }

                if (all_coils_verified)
                {
                    application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

                    List<warehoused_shipment_load> loads = new List<warehoused_shipment_load>();

                    //just add the current load
                    loads.Add(current_load);

                    foreach (warehoused_shipment_load load in loads)
                    {
                        warehoused_shipment_load.UpdateLoadStatusAndLoadingUser(load.load_id, "SH", current_application_security.user_id);

                        string result = usp.usp_call_spc_set_load_shipped_wh(load.char_load_id);

                        if (result != "SUCCESS")
                            {
                                application_settings re_ship = application_settings.GetAppSetting("RESHIP");

                                if (re_ship.default_value == "Y")
                                {
                                    //Reset Loads status back to 'Pickup', clean scanware_shipping_loads entries and Retry Shipping Procedure once

                                    //Wait 5 seconds and retry to ship
                                    System.Threading.Thread.Sleep(5000);

                                    foreach (warehoused_shipment_load load2 in loads)
                                    {
                                        warehoused_shipment_load.UpdateLoadStatusAndLoadingUser(load2.load_id, "PI", current_application_security.user_id);
                                        scanware_shipping_loads.remove(load2.load_id);
                                    }

                                    viewModel.Message = "RESHIP";

                                    //Send alert to shipping supervisors
                                    int msg_delivery_id;
                                    string to_address2;
                                    DateTime now = DateTime.Now;

                                    var system_email = rsp.default_from_user;

                                    try
                                    {
                                        List<string> to_address = db.Database.SqlQuery<string>("SELECT email_address FROM conversation_email WHERE email_type = 'BD'").ToList();
                                        to_address2 = string.Join(",", to_address.ToArray());

                                        msg_delivery_id = db.Database.SqlQuery<int>("EXEC call_spc_up_seq_no 'msg_delivery_id'").SingleOrDefault();

                                        var sql = "INSERT INTO dbo.msg_delivery_queue" +
                                              "(delivery_id, delivery_method_id, from_name, from_address, to_name, to_address, subject, message, add_datetime," +
                                               "add_user_id, send_datetime, send_status_id, from_appname, retry_count, append, notification_sent, customer_id, amyuni_processed)" +
                                           "VALUES" +
                                               "(@msg_delivery_id, 1, 'Scanware Process', '" + system_email + "', @to_address, @to_address" +
                                               ", 'Truck load shipping process problem'" +
                                               ", 'An Error occured while trying to Ship Load #" + load.char_load_id + ". Please verify that all information got updated and the Coils shipped!'" +
                                               ", @now, @user_id, null, 0, 'SCANWARE', 0, 'N', 'N', 0, 'N')";

                                        db.Database.SqlQuery<List<string>>(sql, new SqlParameter("@msg_delivery_id", msg_delivery_id),
                                                                       new SqlParameter("@to_address", to_address2),
                                                                       new SqlParameter("@now", now),
                                                                       new SqlParameter("@user_id", current_application_security.user_id)).SingleOrDefault();
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception(char_load_id + " Warehouse Shipping Procedure Failed to Ship Load. Please contact Level 3!" + ex.ToString());
                                    }
                                }
                                viewModel.Error = char_load_id + " Warehouse Shipping Procedure Failed to Ship Load. " + result + ". Please contact Level 3!";
                                return RedirectToAction("LoadTruck", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
                            }

                            string error = warehoused_shipment_load.UpdateScaleTimeOutWarehouse(load.load_id, current_application_security.user_id);

                            if (error != "SUCCESS")
                            {
                                viewModel.Error = error + ". Please contact Level 3!";
                                return RedirectToAction("LoadTruck", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
                            }

                        }

                    //add audit record
                    foreach (warehoused_load_dtl coil in current_coils)
                    {
                        string scanner_user;

                        if (ScannerUsed == 1)
                            scanner_user = "Y";
                        else
                            scanner_user = "N";

                        Utils.AddAuditRecordWarehouse(coil.production_coil_no, "Truck Load Shipped", current_load.char_load_id, scanner_user);
                    }

                    viewModel.Message = char_load_id + " has been shipped in the system";
                    return RedirectToAction("LoadTruck", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error });
                }
                else
                {
                    viewModel.Error = "Not all coils are verified on " + char_load_id;
                    return RedirectToAction("LoadTruck", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
                }
            }
            else
            {
                viewModel.Error = "Unable to find " + char_load_id;
                return RedirectToAction("LoadTruck", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
            }
        }

        public ActionResult CancelRailLoad(string char_load_id)
        {
            WarehouseModel viewModel = new WarehouseModel();

            shipment_load current_shipment = shipment_load.GetShipmentLoad(char_load_id);

            int load_id = Int32.Parse(char_load_id);

            if (current_shipment == null)
            {
                //Is warehouse load
                viewModel.warehouse_shipment = warehoused_shipment_load.GetShipmentLoad(load_id);

                if (viewModel.warehouse_shipment == null)
                {
                    throw new Exception(char_load_id + " is not found.");
                }
                else
                {
                    //Call Cancel rail load Warehouse
                    return RedirectToAction("CancelRailLoadWarehouse", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
                }
            }
            List<shipment_load> loads = new List<shipment_load>();

            //just add the current load
            loads.Add(current_shipment);

            foreach (shipment_load sl in loads)
            {
                List<load_dtl> current_coils = load_dtl.GetLoadDtl(sl.load_id);

                foreach (load_dtl coil in current_coils)
                {
                    load_dtl.CancelScannedDate(coil.production_coil_no, 0);
                    Utils.AddAuditRecord(coil.production_coil_no, "Rail Load Cancelled", sl.char_load_id + " - " + sl.vehicle_no);
                }

                shipment_load.CancelRailLoad(sl.load_id, 0);
            }

            viewModel.Message = "Rail load " + char_load_id + " has been cancelled";

            return RedirectToAction("LoadRail", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error });
        }

        public ActionResult CancelRailLoadWarehouse(string char_load_id)
        {
            WarehouseModel viewModel = new WarehouseModel();

            int load_id = Int32.Parse(char_load_id);

            warehoused_shipment_load current_shipment = warehoused_shipment_load.GetShipmentLoad(load_id);

            List<warehoused_shipment_load> loads = new List<warehoused_shipment_load>();

            //just add the current load
            loads.Add(current_shipment);

            foreach (warehoused_shipment_load sl in loads)
            {
                List<warehoused_load_dtl> current_coils = warehoused_load_dtl.GetLoadDtl(sl.load_id);

                foreach (warehoused_load_dtl coil in current_coils)
                {
                    warehoused_load_dtl.CancelScannedDate(coil.production_coil_no, 0);
                    Utils.AddAuditRecordWarehouse(coil.production_coil_no, "Rail Load Cancelled", sl.char_load_id + " - " + sl.vehicle_no);
                }

                warehoused_shipment_load.CancelRailLoad(sl.load_id, 0);
            }

            viewModel.Message = "Rail load " + char_load_id + " has been cancelled";

            return RedirectToAction("LoadRail", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error });
        }
        public ActionResult ShipRailLoad(string char_load_id, int? ScannerUsed)
        {
            WarehouseModel viewModel = new WarehouseModel();
            int load_id = Int32.Parse(char_load_id);
            shipment_load current_load = shipment_load.GetShipmentLoad(char_load_id);

            if (current_load == null)
            {
                //Is warehouse load
                viewModel.warehouse_shipment = warehoused_shipment_load.GetShipmentLoad(load_id);

                if (viewModel.warehouse_shipment == null)
                {
                    throw new Exception(char_load_id + " is not found.");
                }
                else
                {
                    //Call Ship Rail load Warehouse
                    return RedirectToAction("ShipRailLoadWarehouse", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
                }
            }

            if (current_load != null)
            {
                bool all_coils_verified = true;
                List<load_dtl> current_coils = new List<load_dtl>();
                ref_sys_param rsp = ref_sys_param.GetRefSysParam();
                List<int> subLoads = new List<int>();
                sdipdbEntities db = ContextHelper.SDIPDBContext;
                List<shipment_load> current_loads = new List<shipment_load>();

                if (rsp.location != "C")
                {

                    //Check if this is a subload - Butler
                    subLoads = shipment_load.isMasterLoad(char_load_id);

                    if (subLoads.Count() > 0)
                    {
                        current_loads = db.shipment_load.Where(x => x.master_load_id == load_id).ToList();
                    }
                    else
                    {
                        if (rsp.location != "C")
                        {
                            current_loads = db.shipment_load.Where(x => x.load_id == load_id).ToList();
                        }
                    }

                    Session["ShipLoad"] = load_id.ToString();
                }

                //if a subload is involved
                if (current_load.char_load_id.Replace("-901", "") != current_load.char_load_id)
                {
                    current_coils = load_dtl.GetLoadDtlForAllSubLoads(current_load.char_load_id);
                }
                else
                {
                    if (rsp.location == "C")
                    {
                        current_coils = load_dtl.GetLoadDtl(current_load.load_id);
                    }
                    else
                    {
                        //If sub Loads involved Butler
                        if (subLoads.Count() > 0)
                        {
                            current_coils = load_dtl.GetLoadDtlSub(subLoads);
                        }
                        else
                        {
                            current_coils = load_dtl.GetLoadDtl(current_load.load_id);
                        }
                    }
                }

                bool coils_shipped = false;
                foreach (load_dtl load_item in current_coils)
                {
                    // check that coil has NOT been previously marked "Rail Load Shipped"
                    coils_shipped = coils_shipped || coil_audit_trail.GetCoilHistoryByProductionCoilNumber(load_item.production_coil_no).Select(x => x.action_description.ToString()).Contains("Rail Load Shipped");

                    if (load_item.coil_scanned_dt == null)
                    {
                        all_coils_verified = false;
                    }
                }

                if (all_coils_verified && !coils_shipped)
                {
                    application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

                    List<shipment_load> loads = new List<shipment_load>();

                    //if sub loads involved
                    if (current_load.char_load_id.Replace("-901", "") != current_load.char_load_id)
                    {
                        loads = shipment_load.GetShipmentLoadSubLoads(current_load.char_load_id);
                    }
                    else
                    {
                        if ((rsp.location != "C") && (subLoads.Count() > 0))
                        {
                            loads = current_loads;
                        }
                        else
                        {
                            //just add the current load
                            loads.Add(current_load);
                        }
                    }

                    foreach (shipment_load load in loads)
                    {
                        shipment_load.UpdateLoadStatusAndLoadingUser(load.load_id, "SH", current_application_security.user_id);

                        if (rsp.location != "C")
                        {
                            string result = usp.usp_call_spc_set_load_shipped_v2(load.char_load_id);

                            if (result != "SUCCESS")
                            {
                                application_settings re_ship = application_settings.GetAppSetting("RESHIP");

                                if (re_ship.default_value == "Y")
                                {
                                    //Reset Loads status back to 'Pickup', clean scanware_shipping_loads entries and Retry Shipping Procedure once

                                    //Wait 5 seconds and retry to ship
                                    System.Threading.Thread.Sleep(5000);

                                    foreach (shipment_load load2 in loads)
                                    {
                                        shipment_load.UpdateLoadStatusAndLoadingUser(load2.load_id, "PI", current_application_security.user_id);
                                        scanware_shipping_loads.remove(load2.load_id);
                                    }

                                    viewModel.Message = "RESHIP";
                                }

                                //Send alert to shipping supervisors
                                int msg_delivery_id;
                                string to_address2;
                                DateTime now = DateTime.Now;

                                var system_email = rsp.default_from_user;
                                try
                                {
                                    List<string> to_address = db.Database.SqlQuery<string>("SELECT email_address FROM conversation_email WHERE email_type = 'BD'").ToList();
                                    to_address2 = string.Join(",", to_address.ToArray());

                                    msg_delivery_id = db.Database.SqlQuery<int>("EXEC call_spc_up_seq_no 'msg_delivery_id'").SingleOrDefault();

                                    var sql = "INSERT INTO dbo.msg_delivery_queue" +
                                          "(delivery_id, delivery_method_id, from_name, from_address, to_name, to_address, subject, message, add_datetime," +
                                           "add_user_id, send_datetime, send_status_id, from_appname, retry_count, append, notification_sent, customer_id, amyuni_processed)" +
                                       "VALUES" +
                                           "(@msg_delivery_id, 1, 'Scanware Process', '" + system_email + "', @to_address, @to_address" +
                                           ", 'Rail load shipping process problem'" +
                                           ", 'An Error occured while trying to Ship Load #" + load.char_load_id + ". Please verify that all information got updated and the Coils shipped!'" +
                                           ", @now, @user_id, null, 0, 'SCANWARE', 0, 'N', 'N', 0, 'N')";

                                    db.Database.SqlQuery<List<string>>(sql, new SqlParameter("@msg_delivery_id", msg_delivery_id),
                                                                   new SqlParameter("@to_address", to_address2),
                                                                   new SqlParameter("@now", now),
                                                                   new SqlParameter("@user_id", current_application_security.user_id)).SingleOrDefault();
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(char_load_id + " Shipping Procedure Failed to Ship Load. Please contact Level 3!" + ex.ToString());
                                }

                                viewModel.Error = char_load_id + " Shipping Procedure Failed to Ship Load. " + result + ". Please contact Level 3!";
                                return RedirectToAction("LoadRail", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
                            }

                            shipment_load.UpdateScaleTimeOutButler(load.load_id, current_application_security.user_id);

                            //If sub-load - get weight out value from Master Load
                            if (subLoads.Count() > 0)
                            {

                                shipment_load master_load = db.shipment_load.Where(x => x.load_id == load_id).FirstOrDefault();
                                int? master_weight_out = master_load.scale_weight_in + master_load.total_weight_load;

                                if ((master_weight_out != null) && (master_weight_out > 0))
                                {
                                    shipment_load.SetScaleWeightOut(load.load_id, master_weight_out, current_application_security.user_id);
                                }
                            }
                        }
                        else
                        {
                            //set scale time out for auditing reporting
                            shipment_load.UpdateScaleTimeOut(load.load_id, current_application_security.user_id);
                            usp.usp_sw_update_rail_car_in_lomas(load.char_load_id);
                        }

                    }

                    string scanner_user;

                    //add audit record, move coils to rail car location
                    foreach (load_dtl coil in current_coils)
                    {
                        if (rsp.location != "C")
                        {
                            if (ScannerUsed == 1)
                                scanner_user = "Y";
                            else
                                scanner_user = "N";

                            Utils.AddAuditRecord(coil.production_coil_no, "Rail Load Shipped", current_load.char_load_id, scanner_user);
                        }
                        else
                        {
                            Utils.AddAuditRecord(coil.production_coil_no, "Rail Load Shipped", current_load.char_load_id);
                        }

                        //get location of coil
                        coil_yard_locations current_coil_yard_location = coil_yard_locations.GetCoilYardLocation(coil.production_coil_no);
                        coil_yard_locations new_coil_yard_location = new coil_yard_locations();

                        //if Columbus insert in to rail location
                        if (rsp.location == "C")
                        {
                            //delete if exists to reinsert later
                            if (current_coil_yard_location != null)
                            {
                                coil_yard_locations.DeleteCoilYardLocation(coil.production_coil_no);
                            }
                            new_coil_yard_location = coil_yard_locations.InsertCoilYardLocation(coil.production_coil_no, "RC", "RC", current_application_security.user_id, "Y");
                        }
                    }

                    viewModel.Message = char_load_id + " has been shipped in the system";
                    return RedirectToAction("LoadRail", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error });
                }
                else if (!all_coils_verified)
                {
                    viewModel.Error = "Not all coils are verified on " + char_load_id;
                    return RedirectToAction("LoadRail", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
                }
                else
                {
                    viewModel.Error = "Coils already marked as shipped on " + char_load_id;
                    return RedirectToAction("LoadRail", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
                }
            }
            else
            {
                viewModel.Error = "Unable to find " + char_load_id;
                return RedirectToAction("LoadRail", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
            }
        }

        public ActionResult ShipRailLoadWarehouse(string char_load_id, int? ScannerUsed)
        {
            WarehouseModel viewModel = new WarehouseModel();
            int load_id = Int32.Parse(char_load_id);
            warehoused_shipment_load current_load = warehoused_shipment_load.GetShipmentLoad(load_id);

            if (current_load != null)
            {
                bool all_coils_verified = true;
                List<warehoused_load_dtl> current_coils = new List<warehoused_load_dtl>();
                ref_sys_param rsp = ref_sys_param.GetRefSysParam();
                List<int> subLoads = new List<int>();
                sdipdbEntities db = ContextHelper.SDIPDBContext;
                List<warehoused_shipment_load> current_loads = new List<warehoused_shipment_load>();

                current_loads = db.warehoused_shipment_load.Where(x => x.load_id == load_id).ToList();

                Session["ShipLoad"] = 0;

                Session["ShipLoad"] = load_id.ToString();

                current_coils = warehoused_load_dtl.GetLoadDtl(current_load.load_id);                

                bool coils_shipped = false;

                foreach (warehoused_load_dtl load_item in current_coils)
                {
                    // check that coil has NOT been previously marked "Rail Load Shipped"
                    coils_shipped = coils_shipped || coil_audit_trail.GetCoilHistoryByProductionCoilNumber(load_item.production_coil_no).Select(x => x.action_description.ToString()).Contains("Rail Load Shipped");

                    if (load_item.coil_scanned_dt == null)
                    {
                        all_coils_verified = false;
                    }
                }

                if (all_coils_verified && !coils_shipped)
                {
                    application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

                    List<warehoused_shipment_load> loads = new List<warehoused_shipment_load>();

                    loads = current_loads;

                    foreach (warehoused_shipment_load load in loads)
                    {
                        warehoused_shipment_load.UpdateLoadStatusAndLoadingUser(load.load_id, "SH", current_application_security.user_id);

                            string result = usp.usp_call_spc_set_load_shipped_wh(load.char_load_id);

                            if (result != "SUCCESS")
                            {
                                application_settings re_ship = application_settings.GetAppSetting("RESHIP");

                                if (re_ship.default_value == "Y")
                                {
                                    //Reset Loads status back to 'Pickup', clean scanware_shipping_loads entries and Retry Shipping Procedure once

                                    //Wait 5 seconds and retry to ship
                                    System.Threading.Thread.Sleep(5000);

                                    foreach (warehoused_shipment_load load2 in loads)
                                    {
                                        warehoused_shipment_load.UpdateLoadStatusAndLoadingUser(load2.load_id, "PI", current_application_security.user_id);
                                        scanware_shipping_loads.remove(load2.load_id);
                                    }

                                    viewModel.Message = "RESHIP";
                                }

                                //Send alert to shipping supervisors
                                int msg_delivery_id;
                                string to_address2;
                                DateTime now = DateTime.Now;

                                var system_email = rsp.default_from_user;
                                try
                                {
                                    List<string> to_address = db.Database.SqlQuery<string>("SELECT email_address FROM conversation_email WHERE email_type = 'BD'").ToList();
                                    to_address2 = string.Join(",", to_address.ToArray());

                                    msg_delivery_id = db.Database.SqlQuery<int>("EXEC call_spc_up_seq_no 'msg_delivery_id'").SingleOrDefault();

                                    var sql = "INSERT INTO dbo.msg_delivery_queue" +
                                          "(delivery_id, delivery_method_id, from_name, from_address, to_name, to_address, subject, message, add_datetime," +
                                           "add_user_id, send_datetime, send_status_id, from_appname, retry_count, append, notification_sent, customer_id, amyuni_processed)" +
                                       "VALUES" +
                                           "(@msg_delivery_id, 1, 'Scanware Process', '" + system_email + "', @to_address, @to_address" +
                                           ", 'Rail load shipping process problem'" +
                                           ", 'An Error occured while trying to Ship Load #" + load.char_load_id + ". Please verify that all information got updated and the Coils shipped!'" +
                                           ", @now, @user_id, null, 0, 'SCANWARE', 0, 'N', 'N', 0, 'N')";

                                    db.Database.SqlQuery<List<string>>(sql, new SqlParameter("@msg_delivery_id", msg_delivery_id),
                                                                   new SqlParameter("@to_address", to_address2),
                                                                   new SqlParameter("@now", now),
                                                                   new SqlParameter("@user_id", current_application_security.user_id)).SingleOrDefault();
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(char_load_id + " Shipping Procedure Failed to Ship Load. Please contact Level 3!" + ex.ToString());
                                }

                                viewModel.Error = char_load_id + " Shipping Procedure Failed to Ship Load. " + result + ". Please contact Level 3!";
                                return RedirectToAction("LoadRail", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
                            }

                             warehoused_shipment_load.UpdateScaleTimeOutButler(load.load_id, current_application_security.user_id);

                    }

                    string scanner_user;

                    //add audit record, move coils to rail car location
                    foreach (warehoused_load_dtl coil in current_coils)
                    {
                        if (rsp.location != "C")
                        {
                            if (ScannerUsed == 1)
                                scanner_user = "Y";
                            else
                                scanner_user = "N";

                            Utils.AddAuditRecordWarehouse(coil.production_coil_no, "Rail Load Shipped", current_load.char_load_id, scanner_user);
                        }
                        else
                        {
                            Utils.AddAuditRecordWarehouse(coil.production_coil_no, "Rail Load Shipped", current_load.char_load_id);
                        }

                        //get location of coil
                        coil_yard_locations current_coil_yard_location = coil_yard_locations.GetCoilYardLocation(coil.production_coil_no);
                        coil_yard_locations new_coil_yard_location = new coil_yard_locations();

                        //if Columbus insert in to rail location
                        if (rsp.location == "C")
                        {
                            //delete if exists to reinsert later
                            if (current_coil_yard_location != null)
                            {
                                coil_yard_locations.DeleteCoilYardLocation(coil.production_coil_no);
                            }
                            new_coil_yard_location = coil_yard_locations.InsertCoilYardLocation(coil.production_coil_no, "RC", "RC", current_application_security.user_id, "Y");
                        }
                    }

                    viewModel.Message = char_load_id + " has been shipped in the system";
                    return RedirectToAction("LoadRail", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error });
                }
                else if (!all_coils_verified)
                {
                    viewModel.Error = "Not all coils are verified on " + char_load_id;
                    return RedirectToAction("LoadRail", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
                }
                else
                {
                    viewModel.Error = "Coils already marked as shipped on " + char_load_id;
                    return RedirectToAction("LoadRail", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
                }
            }
            else
            {
                viewModel.Error = "Unable to find " + char_load_id;
                return RedirectToAction("LoadRail", "WarehouseShipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
            }
        }
    }
}
