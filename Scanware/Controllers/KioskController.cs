using Scanware.App_Objects;
using Scanware.Data;
using Scanware.Models;
using Scanware.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.IO;

namespace Scanware.Controllers
{
    [AllowAnonymous]
    public class KioskController : Controller
    {
        [AllowAnonymous]
        private ActionResult KioskErrorHandler(string err)
        {
            KioskModel viewModel = new KioskModel { Error = err };
            ViewData["step"] = 0;

            return View(viewModel);
        }

        private string SetLocaleCode(string UICulture)
        {
            // localization selection
            switch (UICulture)
            {
                case "en-US":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US"); break;
                case "es-MX":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-MX"); break;
                default:
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentUICulture; break;
            }

            return CultureInfo.CurrentUICulture.ToString();
        }
        
        [AllowAnonymous]
        public ActionResult Index(string UICulture, string loadcard_no, string precheck_checkbox, string step_id, string Message, string Error, string state, string city, string vehicle_no, string carrier_cd, string driver_name, string scale_time_in, string customer_pick_up_description, byte[] MySignature = null)
        {
            KioskModel viewModel = new KioskModel();
            viewModel.locale_code = SetLocaleCode(UICulture);

            if (loadcard_no == null || loadcard_no == "") return View(viewModel);

            string[] pieces = loadcard_no.Split('-');

            // verify char_load_id exists && is a truck load
            try
            {
                bool isValidLoad = precheck_checkbox == "on" ?
                    shipment_load.IsValidKioskLoad(int.Parse(pieces[0]), int.Parse(pieces[1])) :
                    shipment_load.IsValidKioskLoad("S" + loadcard_no);

                if (!isValidLoad) throw new InvalidDataException();
            }
            catch (InvalidDataException) { return KioskErrorHandler(StringResources.error_invalid_load_card_number + " (" + loadcard_no + ")"); }
            catch (IndexOutOfRangeException) { return KioskErrorHandler(StringResources.error_invalid_precheck_number + " (" + loadcard_no + ")"); }

            // Prechecked loads skip to step 3 (signature & verification)
            if (precheck_checkbox == "on")
            {
                try
                {
                    vw_sw_pre_check_loads pre_check = vw_sw_pre_check_loads.GetPreCheck(Convert.ToInt32(pieces[0]), Convert.ToInt32(pieces[1]));
                    viewModel.shipment = shipment_load.GetShipmentLoad(Convert.ToInt32(pre_check.load_id));
                    viewModel.pre_check_vehicle_no = pre_check.vehicle_no;
                    viewModel.pre_check_driver_name = pre_check.driver_name;
                    viewModel.current_carrier = carrier.GetCarrier(Convert.ToInt32(viewModel.shipment.carrier_cd)) ?? new carrier() { name = "" };

                    step_id = "2"; // escalates to 3 at switch
                }
                catch (Exception) { return KioskErrorHandler(StringResources.error_precheck_id_not_found + " " + loadcard_no); }
            }
            else
            {
                viewModel.shipment = shipment_load.GetShipmentLoad("S" + loadcard_no);
            }

            //verify load is not for MSP or NPS
            if (viewModel.shipment.ship_to_location_name == "MSP" || viewModel.shipment.ship_to_location_name.ToUpper().Replace(" ", "") == "NEWPROCESS-COLUMBUS")
                return KioskErrorHandler(StringResources.error_local_destination + " " + loadcard_no + " " + StringResources.error_local_destination_supplement + viewModel.shipment.ship_to_location_name);

            ViewData["loadcard_no"] = loadcard_no;
            switch (step_id)
            {
                case "0":
                    // select destination: city & state
                    ViewData["step"] = 1; break;

                case "1":
                    // validate destination for authentication step
                    customer_ship_to destination = customer_ship_to.GetCustomerShipToByShipToID(Convert.ToInt32(viewModel.shipment.ToShipToID));

                    if (destination.state == state && destination.city == city)
                    {
                        viewModel.current_carrier = carrier.GetCarrier(Convert.ToInt32(viewModel.shipment.carrier_cd));
                        viewModel.active_carriers = carrier.GetActiveCarriers();

                        ViewData["step"] = 2;
                    }
                    else
                    {
                        return KioskErrorHandler(StringResources.error_validation_failed);
                    }
                    break;

                case "2":
                    // get vehicle number & validate/update carrier
                    viewModel.pre_check_vehicle_no = viewModel.pre_check_vehicle_no ?? vehicle_no;
                    viewModel.pre_check_driver_name = viewModel.pre_check_driver_name ?? "";
                    viewModel.current_carrier = carrier.GetCarrier(Convert.ToInt32(carrier_cd));
                    viewModel.shipment.customer_pick_up_description = customer_pick_up_description;
                    ViewData["step"] = 3; break;

                case "3":
                    // get driver name & signature
                    viewModel.pre_check_vehicle_no = vehicle_no;
                    viewModel.pre_check_driver_name = driver_name;
                    viewModel.current_carrier = carrier.GetCarrier(Convert.ToInt32(carrier_cd));
                    viewModel.shipment.customer_pick_up_description = customer_pick_up_description;
                    viewModel.shipment_customer_ship_to = customer_ship_to.GetCustomerShipToByShipToID(Convert.ToInt32(viewModel.shipment.ToShipToID));

                    if (MySignature == null)
                        return KioskErrorHandler(StringResources.error_signature_required);

                    try
                    {

                        if(shipment_load_signature.SignaureExists(viewModel.shipment.load_id))
                        {
                            shipment_load_signature.RemoveShipmentLoadSignature(viewModel.shipment.load_id);
                        }

                        shipment_load_signature.InsertShipmentLoadSignature(viewModel.shipment.load_id, MySignature);
                    }
                    catch (Exception ex)
                    {
                        // most likely exception in testing is that a signature already exists for this load_id
                        return KioskErrorHandler(StringResources.error_signature_failed);
                    }

                    ViewData["step"] = 4; break;

                case "4":
                    viewModel.pre_check_vehicle_no = vehicle_no;
                    viewModel.pre_check_driver_name = driver_name;
                    viewModel.current_carrier = carrier.GetCarrier(Convert.ToInt32(carrier_cd));
                    viewModel.shipment.customer_pick_up_description = customer_pick_up_description;
                    viewModel.shipment_customer_ship_to = customer_ship_to.GetCustomerShipToByShipToID(Convert.ToInt32(viewModel.shipment.ToShipToID));
                    viewModel.MySignature = shipment_load_signature.GetShipmentLoadSignature(viewModel.shipment.load_id).signature_image;
                    viewModel.coils_in_shipment = viewModel.shipment.char_load_id.Replace("-901", "") != viewModel.shipment.char_load_id ?          // <--- if subload, find all coils
                        load_dtl.GetLoadDtlForAllSubLoads(viewModel.shipment.char_load_id) :
                        load_dtl.GetLoadDtl(viewModel.shipment.load_id);

                    try
                    {
                        shipment_load.CheckInTruck(viewModel.shipment.load_id, DateTime.Parse(scale_time_in), -1, vehicle_no, driver_name, 0, (int)viewModel.current_carrier.carrier_cd, customer_pick_up_description);
                    }
                    catch (Exception ex)
                    {
                        return KioskErrorHandler(StringResources.error_check_in_failed + " Load ID: " + loadcard_no + " / " + "Char Load ID: " + viewModel.shipment.char_load_id + ". " + ex.ToString().Substring(0, 200));
                    }

                    //ViewData["step"] = 5; break;

                //case "5":
                    List<load_dtl> current_coils = new List<load_dtl>();

                    // if subload, find all coils
                    current_coils = viewModel.shipment.char_load_id.Replace("-901", "") != viewModel.shipment.char_load_id ?
                        load_dtl.GetLoadDtlForAllSubLoads(viewModel.shipment.char_load_id) :
                        load_dtl.GetLoadDtl(viewModel.shipment.load_id);

                    viewModel.coils_in_shipment = current_coils;

                    try
                    {
                        //build load card
                        v_zebra_template_load_card load_card = v_zebra_template_load_card.GetLoadCardTemplate(viewModel.shipment.load_id);

                        string template = load_card.template;

                        foreach (load_dtl coil in current_coils)
                        {
                            //get ship tag template
                            v_zebra_template_coil current_template = v_zebra_template_coil.GetCoilTemplate(coil.production_coil_no, "Ship Tag");

                            template = template + current_template.template;

                            //add audit record
                            Utils.AddAuditRecord_Kiosk(coil.production_coil_no, "Scanware Kiosk - Check In", viewModel.shipment.char_load_id);
                        }

                        // print load card
                        printer default_zebra_printer = printer.GetPrinterByPK(19);             // <----- Columbus Paint Line Zebra
                        Utils.FTPTemplateToZebra(default_zebra_printer, template, "load_card");

                        // print BoL
                        printer default_network_printer = printer.GetPrinterByPK(17);           // <----- Columbus Paint Line BOL
                        Utils.PrintBOL(default_network_printer, (int)viewModel.shipment.customer_id, viewModel.shipment.load_id);
                    }
                    catch (Exception)
                    {
                        application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];
                        shipment_load.CancelTruckCheckIn(viewModel.shipment.load_id, 0);
                        return KioskErrorHandler(StringResources.error_print_failed);
                    }

                    viewModel.Message = StringResources.success_message;
                    break;

                default:
                    ViewData["loadcard_no"] = null;
                    ViewData["step"] = 0; break;
            }

            //viewModel.shipment_loads = shipment_load.GetTrucksOnSite();

            return View(viewModel);
        }

        [AllowAnonymous]
        public JsonResult States()
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            var result = from st in db.states
                         select new location { state_abbr = st.state1, state_long = st.description };

            return Json(result.OrderBy(x => x.state_abbr).ToList(), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult Cities(string state)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            var result = db.to_freight_locations
                .Where(x => x.state == state)
                .OrderBy(x => x.city)
                .Select(x => x.city)
                .ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
