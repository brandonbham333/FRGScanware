using Scanware.App_Objects;
using Scanware.Data;
using Scanware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Scanware.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult PrintPaintLocation(int? location_cd, string Message, string Error)
        {
            AdminModel viewModel = new AdminModel();

            if (location_cd.HasValue && location_cd != 0)
            {
                viewModel.current_paint_location = paint_location.GetPaintLocation(Convert.ToInt32(location_cd));
            }
            else
            {
                viewModel.current_paint_location = new paint_location();
            }

            viewModel.paint_locations = paint_location.GetActivePaintLocations();

            viewModel.Message = Message;
            viewModel.Error = Error;

            return View(viewModel);
        }

        public ActionResult PrintPaintLocationSubmit(int? location_cd)
        {
            AdminModel viewModel = new AdminModel();

            if (location_cd.HasValue && location_cd != 0)
            {

                application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];
                user_defaults default_zebra_printer = user_defaults.GetUserDefaultByName(current_application_security.user_id, "ScanwareZebraPrinter");

                //confirm default printer is setup
                if (default_zebra_printer != null)
                {
                    viewModel.default_zebra_printer = printer.GetPrinterByPK(Convert.ToInt32(default_zebra_printer.value));
                }
                else 
                {
                    return RedirectToAction("PrintPaintLocation", "Admin", new { location_cd = location_cd, Error = "You do not have a default zebra printer setup. Click your login name in the upper right hand corner and set a default zebra printer." });
                }

                viewModel.current_paint_location = paint_location.GetPaintLocation(Convert.ToInt32(location_cd));

                //print paperwork
                try
                {

                    v_zebra_template_paint_location current_location_template = v_zebra_template_paint_location.GetPaintLocationTemplate(viewModel.current_paint_location.location_cd);
                    Utils.FTPTemplateToZebra(viewModel.default_zebra_printer, current_location_template.template, "paint_location");

                }
                catch (Exception ex)
                {
                    viewModel.Message = "";
                    viewModel.Error = "There was an error while printing location barcodes" + ex.ToString();
                }

                viewModel.Message = viewModel.current_paint_location.loc_description + " successfuly printed on " + viewModel.default_zebra_printer.description;

            }
            else
            {
                viewModel.Error = "No Paint Location Defined";
            }


            return RedirectToAction("PrintPaintLocation", "Admin", new { location_cd = location_cd, Error = viewModel.Error, Message = viewModel.Message });

        }


        public ActionResult HoldEmail(string Message, string Error)
        {
            AdminModel viewModel = new AdminModel();
            
            viewModel.Message = Message;
            viewModel.Error = Error;

            viewModel.HoldEmails = scanware_hold_coil_email.GetAllHoldEmails();
            viewModel.InsideProductProcessors = product_processors.GetInsideProductProcessors();
            
            return View(viewModel);
        }

        public ActionResult AddHoldEmail(string facility_cd, string email_address)
        {
            AdminModel viewModel = new AdminModel();

            if (Ancestor.Util.IsEmailValid(email_address))
            {
                scanware_hold_coil_email.AddHoldEmail(email_address, facility_cd);

                return RedirectToAction("HoldEmail", "Admin", new { Message = email_address + " successfully added to facility " + facility_cd });
            }
            else
            {
                return RedirectToAction("HoldEmail", "Admin", new { Error = email_address + " was not added, please verify the email address is in the correct format." });
            }          

        }

        public ActionResult RemoveHoldEmail(int pk)
        {
            AdminModel viewModel = new AdminModel();

            scanware_hold_coil_email to_delete = scanware_hold_coil_email.GetHoldEmailByPK(pk);

            scanware_hold_coil_email.DeleteHoldEmail(pk);

            return RedirectToAction("HoldEmail", "Admin", new { Message = to_delete.email_address + " was successfully deleted for facility " + to_delete.facility_cd });         

        }

        public ActionResult RailCars(string Message)
        {
            AdminModel viewModel = new AdminModel();
            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];
            application_settings set_max_rail_car_weight = application_settings.GetAppSetting("set_max_rail_car_weight") ?? new application_settings();
            viewModel.set_max_weight = set_max_rail_car_weight.default_value == "Y";
            viewModel.Message = Message;
            viewModel.RailCars = rail_cars.GetAllRailCars();

            return View(viewModel);
        }

        public ActionResult EditRailCar(string vehicle_no)
        {
            AdminModel viewModel = new AdminModel();

            viewModel.RailCar = rail_cars.GetRailCar(vehicle_no);

            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];
            application_settings set_max_rail_car_weight = application_settings.GetAppSetting("set_max_rail_car_weight") ?? new application_settings();
            viewModel.set_max_weight = set_max_rail_car_weight.default_value == "Y";
            return View(viewModel);
        }

        public ActionResult EditRailCarSubmit(string vehicle_no, int empty_weight, string status, string permanent_flg, DateTime weight_in_datetime,int? max_weight_limit = null)
        {

            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

            rail_cars.UpdateRailCarDetails(vehicle_no, empty_weight, status, permanent_flg, weight_in_datetime, current_application_security.user_id,max_weight_limit);

            return RedirectToAction("RailCars", "Admin", new { Message = "Vehicle " + vehicle_no + " successfully updated" });

        }

        public ActionResult AddRailCar(string Message, string Error)
        {
            AdminModel viewModel = new AdminModel();

            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];
            application_settings set_max_rail_car_weight = application_settings.GetAppSetting("set_max_rail_car_weight") ?? new application_settings();
            viewModel.set_max_weight = set_max_rail_car_weight.default_value == "Y";
            viewModel.Message = Message;
            viewModel.Error = Error;

            return View(viewModel);
        }

        public ActionResult AddRailCarSubmit(string vehicle_no, int empty_weight, string status, string permanent_flg, DateTime weight_in_datetime, int? max_weight_limit = null)
        {
            
            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

            rail_cars rc_exists = rail_cars.GetRailCar(vehicle_no);

            if (rc_exists == null)
            {
                rail_cars.AddRailCar(vehicle_no, empty_weight, status, permanent_flg, weight_in_datetime, Convert.ToInt32(current_application_security.user_id),max_weight_limit);
                return RedirectToAction("AddRailCar", "Admin", new { Message = "Vehicle " + vehicle_no + " successfully added" });
            }
            else
            {
                return RedirectToAction("AddRailCar", "Admin", new { Error = "Vehicle " + vehicle_no + " already exists" });
            }


        }


    }
}
