using Scanware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Scanware.Data;
using Scanware.App_Objects;
using System.Text;
using System.Windows;
using System.Drawing.Printing;
using System.Net;
using System.IO;
using PdfSharp.Pdf.Printing;
using System.Data.SqlClient;
using Scanware.Ancestor;

namespace Scanware.Controllers
{
    [Authorize]
    public class ShippingController : Controller
    {
        public ActionResult Index()
        {
            ShippingModel viewModel = new ShippingModel();

            viewModel.shipment_loads = shipment_load.GetTrucksOnSite();

            viewModel.shipment_loads_rail = shipment_load.GetOpenRailLoads();

            return View(viewModel);
        }

        public ActionResult ViewSchedule()
        {
            ShippingModel viewModel = new ShippingModel();

            viewModel.SchedCoils = shipping_schedule_334.GetScheduledCoils();
            var lastWeek = DateTime.Now.AddDays(-7);
            viewModel.SchedCoils = viewModel.SchedCoils.Where(x => x.schedule_date >= lastWeek).OrderBy(x => x.schedule_date).ThenBy(x => x.location).ToList();
            return View(viewModel);
        }

        public ActionResult LoadDetails(string production_coil_no, string op_tag_no, string char_load_id, string po_order_no, string order_no, string vehicle_no)
        {
            ShippingModel viewModel = new ShippingModel();

            string ls_loc = Properties.Settings.Default.RegLocation;

            viewModel.searched_char_load_id = char_load_id;
            viewModel.searched_production_coil_no = production_coil_no;
            viewModel.searched_tag_no = op_tag_no;
            viewModel.searched_po_order_no = po_order_no;
            viewModel.searched_order_no = order_no;
            viewModel.searched_vehicle_no = vehicle_no;
            viewModel.location = ls_loc;

            viewModel.load_details = new List<v_sw_load_details>();
            bool filtered_prior = false;

            if (production_coil_no != "")
            {
                viewModel.load_details = v_sw_load_details.GetLoadDetailsByProductionCoilNo(production_coil_no);
                filtered_prior = true;
            }

            //get values from db if not filtered prior
            if (op_tag_no != "" && !filtered_prior)
            {
                viewModel.load_details = v_sw_load_details.GetLoadDetailsByTagNo(op_tag_no);
                filtered_prior = true;
            }
            //otherwise filter from already filtered list
            else if (op_tag_no != "" && filtered_prior)
            {
                viewModel.load_details = viewModel.load_details.Where(x => x.tag_no == op_tag_no).ToList();
                filtered_prior = true;

            }

            //get values from db if not filtered prior
            if (char_load_id != "" && !filtered_prior)
            {
                if (char_load_id != null && ls_loc != "C")
                {
                    int load_id = Int32.Parse(char_load_id);

                    viewModel.load_details = v_sw_load_details.GetLoadDetailsByLoadID(load_id);
                }
                else
                {

                    viewModel.load_details = v_sw_load_details.GetLoadDetailsByCharLoadID(char_load_id);
                }

                filtered_prior = true;
            }
            //otherwise filter from already filtered list
            else if (char_load_id != "" && filtered_prior)
            {
                if (char_load_id != null && ls_loc != "C")
                {
                    int load_id = Int32.Parse(char_load_id);

                    viewModel.load_details = viewModel.load_details.Where(x => x.load_id == load_id).ToList();
                }
                else
                {
                    viewModel.load_details = viewModel.load_details.Where(x => x.char_load_id.Contains(char_load_id)).ToList(); //like
                }

                filtered_prior = true;

            }

            //get values from db if not filtered prior
            if (po_order_no != "" && !filtered_prior)
            {
                viewModel.load_details = v_sw_load_details.GetLoadDetailsByPoOrderNo(po_order_no);
                filtered_prior = true;
            }
            //otherwise filter from already filtered list
            else if (po_order_no != "" && filtered_prior)
            {
                viewModel.load_details = viewModel.load_details.Where(x => x.po_order_no == po_order_no).ToList();
                filtered_prior = true;

            }

            //get values from db if not filtered prior
            if (order_no != "" && !filtered_prior)
            {
                viewModel.load_details = v_sw_load_details.GetLoadDetailsByOrderLineItemNo(order_no);
                filtered_prior = true;
            }
            //otherwise filter from already filtered list
            else if (order_no != "" && filtered_prior)
            {
                viewModel.load_details = viewModel.load_details.Where(x => x.order_line_item_no.Contains(order_no)).ToList(); //like
                filtered_prior = true;

            }

            //get values from db if not filtered prior
            if (vehicle_no != "" && !filtered_prior)
            {
                viewModel.load_details = v_sw_load_details.GetLoadDetailsByVehicleNo(vehicle_no);
                filtered_prior = true;
            }
            //otherwise filter from already filtered list
            else if (vehicle_no != "" && filtered_prior)
            {
                viewModel.load_details = viewModel.load_details.Where(x => x.vehicle_no == vehicle_no).ToList();
                filtered_prior = true;

            }

            return View(viewModel);
        }

        public JsonResult AutoCompleteDriver(string prefix)
        {
            List<string> driver_names = shipment_load.GetRecentDrivers(prefix);

            return Json(driver_names);
        }

        public ActionResult EmailPaperwork(int load_id)
        {
            ShippingModel viewModel = new ShippingModel();

            try
            {
                application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];
                usp.usp_sw_email_shipment_paperwork(load_id, current_application_security.user_id);
                viewModel.Message = "Email Queued";
            }
            catch
            {
                viewModel.Error = "Error";
            }

            return PartialView("EmailPaperwork", viewModel);
        }

        public ActionResult CheckInPreCheck(string pre_check_id)
        {
            try
            {
                string[] pieces = pre_check_id.Split('-');

                vw_sw_pre_check_loads pre_check = vw_sw_pre_check_loads.GetPreCheck(Convert.ToInt32(pieces[0]), Convert.ToInt32(pieces[1]));
                shipment_load sl = shipment_load.GetShipmentLoad(Convert.ToInt32(pre_check.load_id));

                return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = sl.char_load_id.Replace("S", ""), vehicle_no = pre_check.vehicle_no, driver_name = pre_check.driver_name });
            }
            catch
            {
                return RedirectToAction("CheckInOutTruck", "Shipping", new { Error = "Unable to find Pre-Check ID " + pre_check_id });
            }
        }

        public ActionResult CheckInOutTruck(string char_load_id, string Message, string Error, string vehicle_no, string driver_name)
        {
            ShippingModel viewModel = new ShippingModel();

            viewModel.Message = Message;
            viewModel.Error = Error;

            viewModel.pre_check_vehicle_no = vehicle_no;
            viewModel.pre_check_driver_name = driver_name;

            if (char_load_id != null && char_load_id != "")
            {
                string ls_loc = Properties.Settings.Default.RegLocation;

                viewModel.location = ls_loc;
                viewModel.searched_char_load_id = char_load_id.ToString();

                if (ls_loc == "C")
                {
                    char_load_id = "S" + char_load_id.ToString();
                }

                viewModel.shipment = shipment_load.GetShipmentLoad(char_load_id);

                //Retrieve user location
                application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];
                user_defaults default_from_freight_location = user_defaults.GetUserDefaultByName(current_application_security.user_id, "FromFreightLocation");

                //verifiy load exists
                if (viewModel.shipment == null)
                {
                    viewModel.Error = "Unable to find " + char_load_id;
                    viewModel.shipment = null;
                    return View(viewModel);
                }

                //For Techs - alert if user location differs from ship to location (prevent loading alternate site loads)
                if (viewModel.shipment != null && ls_loc == "T")
                {
                    var slFromLoc = viewModel.shipment.from_freight_location_cd.HasValue
                        ? viewModel.shipment.from_freight_location_cd.Value
                        : 0;

                    if (slFromLoc.ToString() != default_from_freight_location.value)
                    {
                        viewModel.Error = "The load you are attempting to Check-in has a different ship from location than your user's current location.";
                        viewModel.shipment = null;
                        return View(viewModel);
                    }
                }

                //verify this is a truck load
                if (viewModel.shipment != null && viewModel.shipment.carrier_mode != "T")
                {
                    viewModel.Error = "This screen can only be used on truck shipments, " + char_load_id + "'s carrier mode is " + viewModel.shipment.carrier_mode;
                    viewModel.shipment = null;
                    return View(viewModel);
                }

                //verify this not msp/nps
                if (viewModel.shipment != null && (viewModel.shipment.ship_to_location_name == "MSP" || viewModel.shipment.ship_to_location_name == "NEW PROCESS-COLUMBUS"))
                {
                    viewModel.Error = "This screen can not be used for MSP or NEW PROCESS-COLUMBUS Shipments. " + char_load_id + " is destined for " + viewModel.shipment.ship_to_location_name;
                    viewModel.shipment = null;
                    return View(viewModel);
                }

                //if a subload is involved
                if (viewModel.shipment.char_load_id.Replace("-901", "") != viewModel.shipment.char_load_id)
                {
                    viewModel.coils_in_shipment = load_dtl.GetLoadDtlForAllSubLoads(viewModel.shipment.char_load_id);
                }
                else
                {
                    viewModel.coils_in_shipment = load_dtl.GetLoadDtl(viewModel.shipment.load_id);
                }

                if (ls_loc == "C" || ls_loc == "T")
                {
                    viewModel.shipment_customer_ship_to = customer_ship_to.GetCustomerShipToByShipToID(Convert.ToInt32(viewModel.shipment.ToShipToID));
                }
                else
                {
                    viewModel.shipment_customer_ship_to = customer_ship_to.GetCustomerShipTo(Convert.ToInt32(viewModel.shipment.customer_id), coil.GetShipToLocationName(viewModel.coils_in_shipment.FirstOrDefault().production_coil_no));

                    if (viewModel.shipment_customer_ship_to == null)
                    {
                        sdipdbEntities db = ContextHelper.SDIPDBContext;
                        int cust = Convert.ToInt32(viewModel.shipment.customer_id);

                        string customer_name = db.Database.SqlQuery<string>($"SELECT name FROM customer WHERE customer_id = {cust}").SingleOrDefault();

                        viewModel.Error = "Error getting the Ship to location for Customer:" + customer_name + " and Location: " + viewModel.shipment.ship_to_location_name;
                        return View(viewModel);
                    }
                }


                viewModel.current_carrier = carrier.GetCarrier(Convert.ToInt32(viewModel.shipment.carrier_cd)) ?? new carrier() { name = "" };
                viewModel.active_carriers = carrier.GetActiveCarriers();
                viewModel.coils_in_shipment_weight = Convert.ToInt32(viewModel.coils_in_shipment.Sum(i => i.coil_weight));

                //if any of the coils are not scanned the all coils are not verified
                if (viewModel.coils_in_shipment.Where(x => x.coil_scanned_dt == null).Count() > 0)
                {
                    viewModel.all_coils_verified = false;
                }
                else
                {
                    viewModel.all_coils_verified = true;
                }

                if (ls_loc != "B")
                {
                    shipment_load_signature current_signature = shipment_load_signature.GetShipmentLoadSignature(viewModel.shipment.load_id);

                    if (current_signature != null)
                    {
                        viewModel.MySignature = current_signature.signature_image;
                    }
                }

                if (ls_loc == "T")
                    viewModel.scale_weight_in = 1;
            }

            viewModel.shipment_loads = shipment_load.GetTrucksOnSite();

            return View(viewModel);
        }

        public ActionResult CheckInTruck(int load_id, DateTime scale_time_in, int scale_weight_in, string vehicle_no, string driver_name, int carrier_cd, string customer_pick_up_description, int? pickup_no)
        {
            string Message = "";
            string Error = "";

            string ls_loc;

            ls_loc = Properties.Settings.Default.RegLocation;

            ShippingModel viewModel = new ShippingModel();
            viewModel.shipment = shipment_load.GetShipmentLoad(load_id);
            viewModel.coils_in_shipment = load_dtl.GetLoadDtl(viewModel.shipment.load_id);
            viewModel.location = ls_loc;

            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];
            user_defaults default_zebra_printer = user_defaults.GetUserDefaultByName(current_application_security.user_id, "ScanwareZebraPrinter");
            user_defaults default_user_initials = user_defaults.GetUserDefaultByName(current_application_security.user_id, "UserInitials");

            //confirm default printer is setup
            if (default_zebra_printer != null)
            {
                viewModel.default_zebra_printer = printer.GetPrinterByPK(Convert.ToInt32(default_zebra_printer.value));
            }

            if (viewModel.default_zebra_printer == null)
            {
                Error = "You do not have a default zebra printer setup. Click your login name in the upper right hand corner and set a default zebra printer.";
                return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = viewModel.shipment.char_load_id.Replace("S", ""), Message = Message, Error = Error });
            }
            if (ls_loc == "T")
            {
                if (default_user_initials == null)
                {
                    Error = "You have not set your default user intials.  Click your login name in the upper right hand corner to set them in user defaults.";
                    return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = viewModel.shipment.char_load_id.Replace("S", ""), Message = Message, Error = Error });
                }

                user_defaults default_network_printer = user_defaults.GetUserDefaultByName(current_application_security.user_id, "ScanwareNetworkPrinter");

                //confirm default printer is setup
                if (default_network_printer != null)
                {
                    viewModel.default_network_printer = printer.GetPrinterByPK(Convert.ToInt32(default_network_printer.value));
                }

                if (viewModel.default_network_printer == null)
                {
                    Error = "You do not have a default network printer setup. Click your login name in the upper right hand corner and set a default network printer.";
                    return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = viewModel.shipment.char_load_id.Replace("S", ""), Message = Message, Error = Error });
                }
            }

            //ship load in system
            try
            {
                shipment_load.CheckInTruck(load_id, scale_time_in, scale_weight_in, vehicle_no, driver_name, current_application_security.user_id, carrier_cd, customer_pick_up_description, pickup_no);
                Message = "Load ID: " + Convert.ToString(load_id) + " / " + "Char Load ID: " + viewModel.shipment.char_load_id + " was successfully checked in.";
            }
            catch (Exception ex)
            {
                Error = "There was an error while checking in Load ID: " + Convert.ToString(load_id) + " / " + "Char Load ID: " + viewModel.shipment.char_load_id + ". " + ex.ToString().Substring(0, 200);
            }

            if (ls_loc == "C" || ls_loc == "T")
            {
                //print in gate paperwork
                try
                {
                    //print load card
                    v_zebra_template_load_card load_card = v_zebra_template_load_card.GetLoadCardTemplate(viewModel.shipment.load_id);

                    string template = load_card.template;

                    if (ls_loc == "T")
                    {
                        if (ZPLUtils.HighDPI(viewModel.default_zebra_printer))
                            template = ZPLUtils.ScaleZPL(template, null);

                        if (pickup_no != null)
                            template += ZPLUtils.CraneTagZPL(pickup_no.Value,
                                ZPLUtils.HighDPI(viewModel.default_zebra_printer));

                        //Print Pick List SSRS Report for a given Truck Sequence
                        Utils.PrintPickList(viewModel.default_network_printer,
                            viewModel.shipment.load_id,
                            pickup_no.Value.ToString());
                    }

                    foreach (load_dtl coil in viewModel.coils_in_shipment)
                    {
                        if (ls_loc == "C")
                        {
                            //get ship tag template
                            v_zebra_template_coil current_template = v_zebra_template_coil.GetCoilTemplate(coil.production_coil_no, "Ship Tag");

                            template = template + current_template.template;
                        }
                    }

                    //If Techs - use TCP printing method
                    if (ls_loc == "T")
                    {
                        Utils.TCPTemplateToZebra(viewModel.default_zebra_printer, template);
                    }
                    else
                        Utils.FTPTemplateToZebra(viewModel.default_zebra_printer, template, "load_card");

                    List<load_dtl> current_coils = new List<load_dtl>();

                    //if a subload is involved
                    if (viewModel.shipment.char_load_id.Replace("-901", "") != viewModel.shipment.char_load_id)
                    {
                        current_coils = load_dtl.GetLoadDtlForAllSubLoads(viewModel.shipment.char_load_id);
                    }
                    else
                    {
                        current_coils = load_dtl.GetLoadDtl(viewModel.shipment.load_id);
                    }

                    //add audit record, move coils to rail car location
                    foreach (load_dtl coil in current_coils)
                    {
                        Utils.AddAuditRecord(coil.production_coil_no, "Truck Scaled In", viewModel.shipment.char_load_id);
                    }

                }
                catch (Exception)
                {
                    shipment_load.CancelTruckCheckIn(load_id, current_application_security.user_id);
                    Message = "";
                    Error = "There was an error while printing ship tag / load cards";
                }
            }

            return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = viewModel.shipment.char_load_id.Replace("S", ""), Message = Message, Error = Error });
        }

        public ActionResult ReprintInGatePaperwork(int load_id)
        {
            string Message = "";
            string Error = "";

            string ls_loc = Properties.Settings.Default.RegLocation;

            ShippingModel viewModel = new ShippingModel();
            viewModel.shipment = shipment_load.GetShipmentLoad(load_id);
            viewModel.coils_in_shipment = load_dtl.GetLoadDtl(viewModel.shipment.load_id);

            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];
            user_defaults default_zebra_printer = user_defaults.GetUserDefaultByName(current_application_security.user_id, "ScanwareZebraPrinter");

            //confirm default printer is setup
            if (default_zebra_printer != null)
            {
                viewModel.default_zebra_printer = printer.GetPrinterByPK(Convert.ToInt32(default_zebra_printer.value));
            }

            if (viewModel.default_zebra_printer == null)
            {
                Error = "You do not have a default zebra printer setup. Click your login name in the upper right hand corner and set a default zebra printer.";
                return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = viewModel.shipment.char_load_id.Replace("S", ""), Message = Message, Error = Error });
            }
            if (ls_loc == "T")
            {
                user_defaults default_network_printer = user_defaults.GetUserDefaultByName(current_application_security.user_id, "ScanwareNetworkPrinter");

                //confirm default printer is setup
                if (default_network_printer != null)
                {
                    viewModel.default_network_printer = printer.GetPrinterByPK(Convert.ToInt32(default_network_printer.value));
                }

                if (viewModel.default_network_printer == null)
                {
                    Error = "You do not have a default network printer setup. Click your login name in the upper right hand corner and set a default network printer.";
                    return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = viewModel.shipment.char_load_id.Replace("S", ""), Message = Message, Error = Error });
                }
            }

            //print in gate paperwork
            try
            {
                //print load card
                v_zebra_template_load_card load_card = v_zebra_template_load_card.GetLoadCardTemplate(viewModel.shipment.load_id);

                string template = load_card.template;

                if (ls_loc == "T")
                {
                    if (ZPLUtils.HighDPI(viewModel.default_zebra_printer))
                        template = ZPLUtils.ScaleZPL(template, null);

                    if (viewModel.shipment.PickUp_no != null)
                        template += ZPLUtils.CraneTagZPL(viewModel.shipment.PickUp_no.Value,
                            ZPLUtils.HighDPI(viewModel.default_zebra_printer));

                    //Print Pick List SSRS Report for a given Truck Sequence
                    Utils.PrintPickList(viewModel.default_network_printer,
                        viewModel.shipment.load_id,
                        viewModel.shipment.PickUp_no.Value.ToString());
                }

                if (ls_loc == "C")
                {
                    foreach (load_dtl coil in viewModel.coils_in_shipment)
                    {
                        //get ship tag template
                        v_zebra_template_coil current_template = v_zebra_template_coil.GetCoilTemplate(coil.production_coil_no, "Ship Tag");

                        template = template + current_template.template;
                    }
                }

                //If Techs - use TCP printing method
                if (ls_loc == "T")
                    Utils.TCPTemplateToZebra(viewModel.default_zebra_printer, template);
                else
                    Utils.FTPTemplateToZebra(viewModel.default_zebra_printer, template, "load_card");
            }
            catch (Exception)
            {
                Message = "";
                Error = "There was an error while printing ship tag / load cards";
            }

            return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = viewModel.shipment.char_load_id.Replace("S", ""), Message = Message, Error = Error });
        }

        public ActionResult CancelTruckCheckIn(int load_id)
        {
            string Message = "";
            string Error = "";

            ShippingModel viewModel = new ShippingModel();
            viewModel.shipment = shipment_load.GetShipmentLoad(load_id);

            try
            {
                application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

                shipment_load.CancelTruckCheckIn(load_id, current_application_security.user_id);


                Message = "Load ID: " + Convert.ToString(load_id) + " / " + "Char Load ID: " + viewModel.shipment.char_load_id + " check in was cancelled.";
            }
            catch (Exception ex)
            {
                Error = "There was an error while checking in Load ID: " + Convert.ToString(load_id) + " / " + "Char Load ID: " + viewModel.shipment.char_load_id + ". " + ex.ToString().Substring(0, 200);
            }

            return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = viewModel.shipment.char_load_id.Replace("S", ""), Message = Message, Error = Error });
        }

        public ActionResult SetScaleWeightAndGetSignature(int load_id, int scale_weight_out)
        {
            string Message = "";
            string Error = "";

            string location = Properties.Settings.Default.RegLocation;

            ShippingModel viewModel = new ShippingModel();
            viewModel.shipment = shipment_load.GetShipmentLoad(load_id);
            viewModel.coils_in_shipment = load_dtl.GetLoadDtl(viewModel.shipment.load_id);
            viewModel.coils_in_shipment_weight = Convert.ToInt32(viewModel.coils_in_shipment.Sum(i => i.coil_weight));
            viewModel.coils_in_shipment_packaging_weight = viewModel.coils_in_shipment.Sum(i => i.additional_weight);

            if (location == "B")
            {
                return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = viewModel.shipment.char_load_id.Replace("S", ""), Message = Message, Error = Error });
            }

            //rule is 1% of coil weight for tolerance
            //if scale weight is too high
            if (viewModel.shipment.scale_weight_in + viewModel.coils_in_shipment_weight > scale_weight_out + (viewModel.coils_in_shipment_weight * .01))
            {
                Error = "Scale weight out is less than the 1% coil weight tolerance. Expecting a final weight of " + (viewModel.shipment.scale_weight_in + viewModel.coils_in_shipment_weight).ToString() + " +/- 1% of total coil weight of " + viewModel.coils_in_shipment_weight.ToString() + ".";

                return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = viewModel.shipment.char_load_id.Replace("S", ""), Message = Message, Error = Error });
            }

            //if scale weight is too high
            if (viewModel.shipment.scale_weight_in + viewModel.coils_in_shipment_weight < scale_weight_out - (viewModel.coils_in_shipment_weight * .01) - viewModel.coils_in_shipment_packaging_weight)
            {
                Error = "Scale weight out exceeds the 1% coil weight tolerance. Expecting a maximum scale weight out of " + viewModel.shipment.scale_weight_in.ToString() + "(Scale Weight In) + " + viewModel.coils_in_shipment_weight.ToString() + "(Coils in Shipment Weight) +/- " + (viewModel.coils_in_shipment_weight * .01) + "(1% of Coils in Shipment Weight)";

                if (viewModel.coils_in_shipment_packaging_weight > 0)
                {
                    Error = Error + " + Additional Packaging Weight of " + viewModel.coils_in_shipment_packaging_weight.ToString();
                }

                int max_weight_out = Convert.ToInt32((viewModel.shipment.scale_weight_in + viewModel.coils_in_shipment_weight) + (Convert.ToInt32(viewModel.coils_in_shipment_weight * .01)) + viewModel.coils_in_shipment_packaging_weight);

                Error = Error + " = Maximum Scale Weight Out of " + max_weight_out.ToString() + " Pounds";

                return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = viewModel.shipment.char_load_id.Replace("S", ""), Message = Message, Error = Error });
            }

            try
            {
                application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

                shipment_load.SetScaleWeightOut(load_id, scale_weight_out, current_application_security.user_id);
                //Message = "Load ID: " + Convert.ToString(load_id) + " / " + "Char Load ID: " + viewModel.shipment.char_load_id + " was successfully checked out.";

                return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = viewModel.shipment.char_load_id.Replace("S", ""), Message = Message, Error = Error });
            }
            catch (Exception ex)
            {
                Error = "There was an error while checking out Load ID: " + Convert.ToString(load_id) + " / " + "Char Load ID: " + viewModel.shipment.char_load_id + ". " + ex.ToString().Substring(0, 200);
                return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = viewModel.shipment.char_load_id.Replace("S", ""), Message = Message, Error = Error });
            }
        }

        public ActionResult CheckOutTruck(int load_id)
        {
            string Message = "";
            string Error = "";

            ShippingModel viewModel = new ShippingModel();
            viewModel.shipment = shipment_load.GetShipmentLoad(load_id);

            string ls_loc = Properties.Settings.Default.RegLocation;

            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

            if (current_application_security == null)
            {
                Error = "Session Lost, please log out and back in. Also let Michael Clardy know.";
                return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = viewModel.shipment.char_load_id.Replace("S", ""), Message = Message, Error = Error });
            }

            user_defaults default_network_printer = user_defaults.GetUserDefaultByName(current_application_security.user_id, "ScanwareNetworkPrinter");

            //confirm default printer is setup
            if (default_network_printer != null)
            {
                viewModel.default_network_printer = printer.GetPrinterByPK(Convert.ToInt32(default_network_printer.value));
            }


            if (viewModel.default_network_printer == null)
            {
                Error = "You do not have a default network printer setup. Click your login name in the upper right hand corner and set a default network printer.";
                return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = viewModel.shipment.char_load_id.Replace("S", ""), Message = Message, Error = Error });
            }

            try
            {

                shipment_load.CheckOutTruck(load_id, current_application_security.user_id);
                Message = "Load ID: " + Convert.ToString(load_id) + " / " + "Char Load ID: " + viewModel.shipment.char_load_id + " was successfully checked out.";

                //Expand BOL for multi-customer loads
                var custIDs = load_dtl.GetCustomersIDsInLoad(viewModel.shipment.load_id);

                foreach (var custId in custIDs)
                {
                    //Print Paperwork
                    Utils.PrintBOL(viewModel.default_network_printer,
                        custId,
                        viewModel.shipment.load_id,
                        ls_loc == "T" ? 3 : 2,
                        true);
                }

                //Print Proforma/USMCA when appropriate
                if (viewModel.shipment.ToShipToID != null && ls_loc == "T" && customer_ship_to.IsCustomerShipToInCAorMX(viewModel.shipment.ToShipToID.Value))
                {
                    Utils.PrintProformaUSMCADocs(viewModel.default_network_printer, viewModel.shipment.customer_id.Value, viewModel.shipment.load_id);
                }

                List<load_dtl> current_coils = new List<load_dtl>();

                //if a subload is involved
                if (viewModel.shipment.char_load_id.Replace("-901", "") != viewModel.shipment.char_load_id)
                {
                    current_coils = load_dtl.GetLoadDtlForAllSubLoads(viewModel.shipment.char_load_id);
                }
                else
                {
                    current_coils = load_dtl.GetLoadDtl(viewModel.shipment.load_id);
                }

                //add audit record, move coils to rail car location
                foreach (load_dtl coil in current_coils)
                {
                    Utils.AddAuditRecord(coil.production_coil_no, "Truck Scaled Out", viewModel.shipment.char_load_id);
                }

                if (ls_loc == "T")
                {
                    string result = usp.usp_call_spc_set_load_shipped_v2(viewModel.shipment.char_load_id);

                    if (result != "SUCCESS")
                    {
                        viewModel.Error = result + ". Please contact Level 3!";
                        return RedirectToAction("LoadTruck", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = viewModel.shipment.char_load_id });
                    }

                    string error = shipment_load.UpdateScaleTimeOutButler(viewModel.shipment.load_id, current_application_security.user_id);

                    if (error != "SUCCESS")
                    {
                        viewModel.Error = error + ". Please contact Level 3!";
                        return RedirectToAction("LoadTruck", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = viewModel.shipment.char_load_id });
                    }
                }

                return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = viewModel.shipment.char_load_id.Replace("S", ""), Message = Message });

            }
            catch (Exception ex)
            {
                Error = "There was an error while checking out Load ID: " + Convert.ToString(load_id) + " / " + "Char Load ID: " + viewModel.shipment.char_load_id + ". " + ex.ToString().Substring(0, 200);
                return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = viewModel.shipment.char_load_id.Replace("S", ""), Message = Message, Error = Error });
            }
        }

        public ActionResult ShipKioskTruckLoad(string char_load_id)
        {

            string Message = "";
            string Error = "";

            ShippingModel viewModel = new ShippingModel();
            viewModel.shipment = shipment_load.GetShipmentLoad(char_load_id);

            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

            shipment_load.SetScaleWeightOut(viewModel.shipment.load_id, -1, current_application_security.user_id);

            try
            {

                shipment_load.CheckOutTruck(viewModel.shipment.load_id, current_application_security.user_id);
                Message = "Load ID: " + Convert.ToString(viewModel.shipment.load_id) + " / " + "Char Load ID: " + viewModel.shipment.char_load_id + " was successfully checked out.";

                List<load_dtl> current_coils = new List<load_dtl>();

                //if a subload is involved
                if (viewModel.shipment.char_load_id.Replace("-901", "") != viewModel.shipment.char_load_id)
                {
                    current_coils = load_dtl.GetLoadDtlForAllSubLoads(viewModel.shipment.char_load_id);
                }
                else
                {
                    current_coils = load_dtl.GetLoadDtl(viewModel.shipment.load_id);
                }

                //add audit record, move coils to rail car location
                foreach (load_dtl coil in current_coils)
                {
                    Utils.AddAuditRecord(coil.production_coil_no, "Kiosk Load Checked Out", viewModel.shipment.char_load_id);
                }

            }
            catch (Exception ex)
            {
                Error = ex.ToString().Substring(0, 200);

            }


            return RedirectToAction("LoadTruck", "Shipping", new { Message = Message, Error = Error, char_load_id = char_load_id });
        }

        public ActionResult GetDriverSignature(int load_id)
        {
            ShippingModel viewModel = new ShippingModel();

            viewModel.shipment = shipment_load.GetShipmentLoad(load_id);
            viewModel.coils_in_shipment = load_dtl.GetLoadDtl(viewModel.shipment.load_id);
            //viewModel.shipment_customer_ship_to = customer_ship_to.GetCustomerShipTo(Convert.ToInt32(viewModel.shipment.customer_id), coil.GetShipToLocationName(viewModel.coils_in_shipment.FirstOrDefault().production_coil_no));
            viewModel.shipment_customer_ship_to = customer_ship_to.GetCustomerShipToByShipToID(Convert.ToInt32(viewModel.shipment.ToShipToID));
            viewModel.current_carrier = carrier.GetCarrier(Convert.ToInt32(viewModel.shipment.carrier_cd));
            viewModel.coils_in_shipment_weight = Convert.ToInt32(viewModel.coils_in_shipment.Sum(i => i.coil_weight));

            return View(viewModel);
        }

        public ActionResult SaveDriverSignature(int load_id, byte[] MySignature)
        {
            ShippingModel viewModel = new ShippingModel();

            string Message = "";
            string Error = "";

            string location = Properties.Settings.Default.RegLocation;

            if (location == "B")
            {
                return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = viewModel.shipment.char_load_id.Replace("S", ""), Message = Message, Error = Error });
            }

            viewModel.shipment = shipment_load.GetShipmentLoad(load_id);

            if (MySignature == null)
            {
                Error = "Signature can not be blank, please try again.";
                return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = viewModel.shipment.char_load_id.Replace("S", ""), Message = Message, Error = Error });
            }

            try
            {
                shipment_load_signature.InsertShipmentLoadSignature(viewModel.shipment.load_id, MySignature);
                Message = "Signature saved for Load ID: " + Convert.ToString(load_id) + " / " + "Char Load ID: " + viewModel.shipment.char_load_id + ". Verify details below and check out to print paperwork.";

                //return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = viewModel.shipment.char_load_id, Message = Message, Error = Error });
                //automatically check out after saving signature
                return RedirectToAction("CheckOutTruck", "Shipping", new { load_id = viewModel.shipment.load_id });

            }
            catch (Exception ex)
            {

                Error = "There was an error while checking out Load ID: " + Convert.ToString(load_id) + " / " + "Char Load ID: " + viewModel.shipment.char_load_id + ". " + ex.ToString().Substring(0, 200);
                return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = viewModel.shipment.char_load_id.Replace("S", ""), Message = Message, Error = Error });
            }

        }

        public ActionResult ResetScaleWeightAndSignature(int load_id)
        {
            ShippingModel viewModel = new ShippingModel();

            string Message = "";
            string Error = "";

            viewModel.shipment = shipment_load.GetShipmentLoad(load_id);

            string location = Properties.Settings.Default.RegLocation;

            if (location == "B")
            {
                return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = viewModel.shipment.char_load_id.Replace("S", ""), Message = Message, Error = Error });
            }

            try
            {
                application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

                shipment_load_signature.RemoveShipmentLoadSignature(viewModel.shipment.load_id);
                shipment_load.SetScaleWeightOut(load_id, null, current_application_security.user_id);

                Message = "Signature removed and scale weight out reset for Load ID: " + Convert.ToString(load_id) + " / " + "Char Load ID: " + viewModel.shipment.char_load_id + ".";

                return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = viewModel.shipment.char_load_id.Replace("S", ""), Message = Message, Error = Error });

            }
            catch (Exception ex)
            {



                Error = "There was an error while resetting load data: " + Convert.ToString(load_id) + " / " + "Char Load ID: " + viewModel.shipment.char_load_id + ". " + ex.ToString().Substring(0, 200);
                return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = viewModel.shipment.char_load_id.Replace("S", ""), Message = Message, Error = Error });

            }
        }

        public ActionResult OPReturnValidate(string Message, string Error)
        {
            ShippingModel viewModel = new ShippingModel();

            viewModel.Message = Message;
            viewModel.Error = Error;
            viewModel.op_coil_validate = sw_op_coil_validate.GetInvalidCoils();

            return View(viewModel);
        }

        public ActionResult OPReturnValidateCoil(string tag_no)
        {
            ShippingModel viewModel = new ShippingModel();

            viewModel.current_all_produced_coil = all_produced_coils.GetAllProducedCoilByTagNumber(tag_no);
            viewModel.op_coil = sw_op_coil_validate.GetCoilValidate(viewModel.current_all_produced_coil.production_coil_no);

            return View(viewModel);
        }

        public ActionResult OPReturnValidateDimensions(string production_coil_no, int coil_weight, float coil_width, float coil_thickness)
        {
            ShippingModel viewModel = new ShippingModel();

            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

            viewModel.current_all_produced_coil = all_produced_coils.GetAllProducedCoil(production_coil_no);

            sw_op_coil_validate.UpdateMeasurements(production_coil_no, coil_weight, coil_thickness, coil_width, current_application_security.user_id);

            string Message = viewModel.current_all_produced_coil.tag_no + " / " + viewModel.current_all_produced_coil.production_coil_no + " was successfully validated";

            return RedirectToAction("OPReturnValidate", "Shipping", new { Message = Message });
        }

        public ActionResult ResetScaleWeight(int load_id)
        {
            ShippingModel viewModel = new ShippingModel();

            string Message = "";
            string Error = "";

            viewModel.shipment = shipment_load.GetShipmentLoad(load_id);

            try
            {
                application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

                shipment_load.SetScaleWeightOut(load_id, null, current_application_security.user_id);

                Message = "Scale weight out reset for Load ID: " + Convert.ToString(load_id) + " / " + "Char Load ID: " + viewModel.shipment.char_load_id + ".";

                return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = viewModel.shipment.char_load_id.Replace("S", ""), Message = Message, Error = Error });
            }
            catch (Exception ex)
            {
                Error = "There was an error while resetting load data: " + Convert.ToString(load_id) + " / " + "Char Load ID: " + viewModel.shipment.char_load_id + ". " + ex.ToString().Substring(0, 200);
                return RedirectToAction("CheckInOutTruck", "Shipping", new { char_load_id = viewModel.shipment.char_load_id.Replace("S", ""), Message = Message, Error = Error });
            }
        }

        public ActionResult ShipOPCoils(string Message, string Error, string location_column, string ship_to_location_name)
        {
            ShippingModel viewModel = new ShippingModel();

            viewModel.Message = Message;
            viewModel.Error = Error;

            //if set from ShipOPCoils then set session
            if (location_column != null)
            {
                Session["ship_op_location_column"] = location_column;
            }

            string session_location_column = "";

            if (Session["ship_op_location_column"] != null)
            {
                session_location_column = Session["ship_op_location_column"].ToString();
            }

            //if set from ShipOPCoils then set session
            if (ship_to_location_name != null)
            {
                Session["ship_op_ship_to_location_name"] = ship_to_location_name;
            }

            string session_ship_to_location_name = "";

            if (Session["ship_op_ship_to_location_name"] != null)
            {
                session_ship_to_location_name = Session["ship_op_ship_to_location_name"].ToString();
            }

            viewModel.outbound_op_coils = vw_sw_outbound_op_coils.GetOPOutboundOPCoils(session_location_column, session_ship_to_location_name);
            viewModel.inbound_op_coils = vw_sw_inbound_op_coils.GetOPInboundOPCoils();
            viewModel.distinct_location_columns = vw_sw_outbound_op_coils.GetDistinctLocationColumns();
            viewModel.distinct_ship_to_location_names = vw_sw_outbound_op_coils.GetDistinctShipToLocationNames();
            viewModel.searched_location_column = session_location_column;
            viewModel.searched_ship_to_location_name = session_ship_to_location_name;

            return View(viewModel);
        }

        public ActionResult ShipOutboundCoils(string[] production_coil_nos)
        {
            string Message = "";
            string Error = "";
            int count = 0;

            //check to make sure coils were checked
            if (production_coil_nos == null || production_coil_nos.Count() == 0)
            {
                Error = "No coils selected to ship";
            }
            else
            {
                foreach (string production_coil_no in production_coil_nos)
                {
                    //increment count for message generation
                    count++;

                    //move in coil yard location
                    coil_yard_locations current_coil_yard_location = coil_yard_locations.GetCoilYardLocation(production_coil_no);

                    application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

                    //delete if exists to reinsert later
                    if (current_coil_yard_location != null)
                    {
                        coil_yard_locations.DeleteCoilYardLocation(production_coil_no);
                    }

                    //determine which column to add coil to
                    vw_sw_outbound_op_coils current_coil = vw_sw_outbound_op_coils.GetOutBoundCoil(production_coil_no);

                    string column = "";

                    //configure Column based on Ship To Location Name vw_sw_outbound_op_coils standardizes ship to location names to either MSP or NEW PROCESS-COLUMBUS
                    if (current_coil.ship_to_location_name == "MSP")
                    {
                        column = "MSP";
                    }
                    else if (current_coil.ship_to_location_name == "NEW PROCESS-COLUMBUS")
                    {
                        column = "NPS";
                    }

                    coil_yard_locations new_coil_yard_location = coil_yard_locations.InsertCoilYardLocation(production_coil_no, column, "FLR", current_application_security.user_id, "O");

                    //ship in lomas if not repair coil
                    if (current_coil.char_load_id != "Repair")
                    {
                        //set scale time out to be used for audit report
                        shipment_load.UpdateScaleTimeOut(Convert.ToInt32(current_coil.load_id), current_application_security.user_id);

                        usp.usp_sw_ship_load_in_lomas(current_coil.char_load_id);
                    }

                    //append coil number
                    Message = Message + current_coil.production_coil_no;

                    //if last time through finish up message
                    if (count == production_coil_nos.Count())
                    {
                        if (count == 1)
                        {
                            Message = Message + " has been shipped.";
                        }
                        else
                        {
                            Message = Message + " have been shipped.";
                        }
                    }
                    //append ,
                    else
                    {
                        Message = Message + ", ";
                    }
                }
            }

            return RedirectToAction("ShipOPCoils", "Shipping", new { Message = Message, Error = Error });
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult LoadTruck(string char_load_id, string Message, string Error)
        {
            ShippingModel viewModel = new ShippingModel();

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

            if (rsp.location == "C")
            {
                //append S to match database if user doesn't type in S
                if (char_load_id != null && char_load_id.Substring(0, 1) != "S")
                {
                    char_load_id = "S" + char_load_id;
                }
            }
            viewModel.location = rsp.location;
            viewModel.Message = Message;
            viewModel.Error = Error;
            viewModel.searched_char_load_id = char_load_id;
            viewModel.shipment_loads = shipment_load.GetTrucksOnSite();
            viewModel.shipment = shipment_load.GetShipmentLoad(char_load_id);
            if (viewModel.shipment == null && char_load_id != "" && char_load_id != null)
            {
                viewModel.Error = "Unable to find load " + char_load_id;
                viewModel.load_images = new List<shipment_load_images>();
            }
            else if (viewModel.shipment != null && viewModel.shipment.carrier_mode != "T")
            {
                viewModel.Error = "The load you entered does not exist as a truck load.";
                viewModel.shipment = null;
            }
            List<int> subLoads = new List<int>();

            //Check if this is a subload - Butler
            if (rsp.location != "C" && char_load_id != null && char_load_id != "")
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
            }



            if (rsp.location != "C")
            {
                try
                {

                    if (char_load_id != null && viewModel.shipment != null && viewModel.shipment.load_id > 0 && rsp.location == "B") 
                    {
                        string coil;

                        load_dtl coils = viewModel.coils_in_shipment.FirstOrDefault();
                        coil = coils.production_coil_no;

                        if (viewModel.shipment.from_freight_location_cd == 77)
                        {
                            string err = usp.usp_get_loading_instruction(viewModel.shipment.load_id, char_load_id, coil);

                            if (err == "ERROR")
                            {
                                viewModel.Error = "Failed to retrieve shipping loading instructions for Load " + char_load_id;
                                viewModel.shipping_loading_instructions = "ERROR";
                            }
                            else
                            {
                                viewModel.shipping_loading_instructions = err;
                            }
                        }
                    }
                    //get app settings for Overide coils verification by line
                    application_settings ovd_b = application_settings.GetSetting("B_COILS_OVD");
                    application_settings ovd_l = application_settings.GetSetting("L_COILS_OVD");
                    application_settings COIL_SCAN_FLAG = application_settings.GetSetting("COIL_SCAN_FLAG");

                    
                    viewModel.ovd_b_coils = ovd_b.default_value;
                    viewModel.ovd_l_coils = ovd_l.default_value;
                    viewModel.coil_scan_flag = COIL_SCAN_FLAG.default_value;

                    Session["coilsInLoad"] = null;
                    Session["LoadVerified"] = null;

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
                }
                catch (Exception ex)
                {
                    Error = "There was an error while getting Truck load data: " + char_load_id + ". " + ex.ToString().Substring(0, 200);
                    viewModel.loads = new List<LoadsAndCoils>();
                    viewModel.Error = Error;
                    return PartialView("LoadTruckButler", viewModel);
                }
              //  ViewBag.Vehicle = "LoadTruck";
                return PartialView("LoadTruckButler", viewModel);
            }
            else
            {
                return View(viewModel);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VerifiedCoilsPartial(string production_coil_no, int load_id, string inputName, string scanner_used)
        {
            string error = "";
            ref_sys_param rsp = ref_sys_param.GetRefSysParam();

            ShippingModel viewModel = new ShippingModel();

            if (scanner_used == null)
                scanner_used = "N";

            if (production_coil_no != "" && production_coil_no != null)
            {
                all_produced_coils apc = all_produced_coils.GetAllProducedCoilByProductionCoilNumberOrTagNumber(production_coil_no);

                //check if coil exists
                if (apc == null)
                {
                    error = "Unable to find " + production_coil_no + " in L3";
                }
            }

            List<CoilsInLoad> fromDB = load_dtl.GetLoadDtlCoils(load_id);

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

                        if (rsp.location != "M")
                        {
                            load_dtl.UpdateCoilScannedDt(production_coil_no, DateTime.Now, current_application_security.user_id);
                        }
                        else
                        {
                            load_dtl.UpdateCoilScannedDt_mx(production_coil_no, DateTime.Now, current_application_security.user_id, load_id);
                        }
                        Utils.AddAuditRecord(production_coil_no, "Verified", load_id.ToString(), scanner_used);
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

        [AcceptVerbs(HttpVerbs.Post)]
        // Set all Coils Verified
        public string AllCoilsVerified(int load_id)
        {
            ShippingModel viewModel = new ShippingModel();
            string char_load_id;
            string error = "SUCCESS";
            ref_sys_param rsp = ref_sys_param.GetRefSysParam();

            try
            {
                char_load_id = load_id.ToString();

                viewModel.shipment = shipment_load.GetShipmentLoad(char_load_id);

                if (viewModel.shipment == null)
                {
                    throw new Exception(char_load_id + " is not found.");
                }

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

                if (viewModel.ButlerSubLoads)
                {
                    viewModel.coilsInLoad = load_dtl.GetLoadDtlCoilsSubs(load_id);
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
                //Make sure this load is not in scanware_loads_ship_after_12 - If it is verify that user has SHIP_AFTER_12 Permission to Ship 
                string is_after_12_load = scanware_loads_ship_after_12.is_load_ship_after_12(load_id);

                Session["ShipLoad"] = 0;
                Session["SHIP_AFTER_12"] = "N";

                if (is_after_12_load != null && is_after_12_load == "Y")
                {
                    function_level_security logged_in_user = new function_level_security(System.Web.HttpContext.Current.Session["function_level_security"] as List<function_level_security>);
                    // viewModel.is_after_12_load = "Y";
                    Session["SHIP_AFTER_12"] = "Y";

                    //Allowed to ship loads that are marked to ship after 12AM on Month-End (Rail Loads are marked in Shipping Management)
                    if (logged_in_user.HasFunctionLevelSecurity("SHIP_AFTER_12"))
                    {
                        Session["ShipLoad"] = load_id.ToString();
                    }
                    else
                    {
                        Session["ShipLoad"] = -1;
                    }
                }
                else
                {
                    Session["ShipLoad"] = load_id.ToString();
                }
            }

            return error;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VerifyCoil(string ship_tag, string production_coil_no, string char_load_id, bool? unverify, int? scannerUsed)
        {
            ShippingModel viewModel = new ShippingModel();

            try
            {
                application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

                viewModel.shipment = shipment_load.GetShipmentLoad(char_load_id);

                if (viewModel.shipment == null)
                {
                    throw new Exception(char_load_id + " is not found.");
                }

                //adding logic for new hb coil number for 2015 and 2016, creating jira issue to address a final fix
                //if the ship tag is for a hot band coil or RS (reship) hot band coil, auto fill in the coil number
                //if (ship_tag.StartsWith("S15B") || ship_tag.StartsWith("S16B") || ship_tag.StartsWith("S17B") || ship_tag.StartsWith("SRS17B") || ship_tag.StartsWith("SRS15B") || ship_tag.StartsWith("SRS16B"))
                if ((ship_tag.Substring(0, 1) == "S" && ship_tag.Substring(3, 1) == "B") || (ship_tag.Substring(0, 3) == "SRS" && ship_tag.Substring(5, 1) == "B"))
                {
                    production_coil_no = ship_tag.Substring(1);
                }

                ref_sys_param rsp = ref_sys_param.GetRefSysParam();

                if (rsp.location != "C")
                {
                    if (production_coil_no.Substring(0, 1) == "S")
                    {
                        production_coil_no = production_coil_no.Substring(1);
                    }
                }

                if (rsp.location == "T")
                {
                    user_defaults default_user_initials = user_defaults.GetUserDefaultByName(current_application_security.user_id, "UserInitials");

                    if (default_user_initials == null)
                    {
                        viewModel.Error = "You have not set your default user intials.  Click your login name in the upper right hand corner to set them in user defaults.";
                        return RedirectToAction("LoadTruck", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
                    }
                }

                all_produced_coils current_all_produced_coil = null;

                if (production_coil_no != "")
                {
                    current_all_produced_coil = all_produced_coils.GetAllProducedCoilByProductionCoilNumberOrTagNumber(production_coil_no);
                }
                else
                {
                    current_all_produced_coil = all_produced_coils.GetAllProducedCoilByTagNumber(ship_tag.Substring(1));
                }

                //if scanning op coil, let it through
                if (current_all_produced_coil.tag_no != null && current_all_produced_coil.tag_no != "" && current_all_produced_coil != null)
                {
                    production_coil_no = ship_tag.Substring(1);
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
            //no action on failure, just pass error back to user
            catch (Exception ex)
            {
                viewModel.Error = ex.ToString();
            }

            if (viewModel.shipment.carrier_mode == "T")
            {
                return RedirectToAction("LoadTruck", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
            }
            else //(carrier_mode == "R")
            {
                return RedirectToAction("LoadRail", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
            }
        }

        public ActionResult LoadRail(string char_load_id, string Message, string Error, string rail_car_brand, string vehicle_no = null, byte? rail_car_no = null, bool set_max_weight = false)
        {
            ShippingModel viewModel = new ShippingModel();
            viewModel.set_max_weight = set_max_weight;
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
            viewModel.rail_car_brand = rail_car_brand;

            ref_sys_param rsp = ref_sys_param.GetRefSysParam();

            viewModel.location = rsp.location;

            if (char_load_id != null && char_load_id != "")
            {
                viewModel.shipment = shipment_load.GetShipmentLoad(char_load_id);



                //happens when bad char_load_id passed in
                if (viewModel.shipment == null)
                {
                    viewModel.Error = "Unable to find rail load " + char_load_id;
                    viewModel.shipment_loads = shipment_load.GetOpenRailLoads();
                    viewModel.load_images = new List<shipment_load_images>();
                }
                else if (viewModel.shipment != null && viewModel.shipment.carrier_mode != "R")
                {
                    viewModel.Error = "The load you entered does not exist as a rail load.";
                    viewModel.shipment = null;
                }
                else
                {
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

                    viewModel.load_images = shipment_load_images.GetLoadImages(viewModel.shipment.load_id);

                    //if vehicle no not set then query db for all rail cars
                    if (viewModel.shipment.vehicle_no == null)
                    {
                        //viewModel.rail_cars = rail_cars.GetAllRailCars().Where(x => x.status == "A").OrderBy(x => x.vehicle_no).ToList();
                        viewModel.rail_car_brands = Scanware.Data.rail_car_brand.GetAllRailCarBrands();
                        //var carrier_cd = shipment_load.GetLoadCarrier(char_load_id);
                        //var x = carrier.GetCarrier()
                    }
                }
            }

            viewModel.shipment_loads_rail = shipment_load.GetOpenRailLoads();

            if (rsp.location != "C")
            {
                viewModel.shipment_loads_rail_after_12am = new List<shipment_load>();

                //get app settings for Overide coils verification by line
                application_settings ovd_b = application_settings.GetAppSetting("B_COILS_OVD");
                application_settings ovd_l = application_settings.GetAppSetting("L_COILS_OVD");
                application_settings COIL_SCAN_FLAG = application_settings.GetSetting("COIL_SCAN_FLAG");

                viewModel.ovd_b_coils = ovd_b.default_value;
                viewModel.ovd_l_coils = ovd_l.default_value;
                viewModel.coil_scan_flag = COIL_SCAN_FLAG.default_value;

                Session["coilsInLoad"] = null;
                Session["LoadVerified"] = null;
   
                //Populate coils in load for all Loads
                List<LoadsAndCoils> newLoad = new List<LoadsAndCoils>();
                foreach (shipment_load load in viewModel.shipment_loads_rail)
                {
                    //Check if after_12am load - If not remove from shipment_loads_rail_after_12am
                    string is_after_12_load = scanware_loads_ship_after_12.is_load_ship_after_12(load.load_id);

                    //Remove from shipment_loads_rail_after_12am - Supports the display of a 12am load indicator on the View
                    if (is_after_12_load != null && is_after_12_load == "Y")
                    {
                        viewModel.shipment_loads_rail_after_12am.Add(load);
                    }

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
                if (viewModel.shipment != null)
                {
                    viewModel.shipment.vehicle_no = viewModel.shipment.vehicle_no ?? vehicle_no;
                    viewModel.shipment.rail_car_number = viewModel.shipment.rail_car_number ?? rail_car_no;
                }

                return PartialView("LoadRailButler", viewModel);
            }
            else
            {
                return View(viewModel);
            }
        }

        public ActionResult StartRailLoad(string char_load_id, string rail_car_brand, string vehicle_no, byte? rail_car_no = null, int max_weight_limit = 0)
        {
            ShippingModel viewModel = new ShippingModel();

            shipment_load current_load = shipment_load.GetShipmentLoad(char_load_id);

            ref_sys_param rsp = ref_sys_param.GetRefSysParam();

            rail_cars current_rail_car;
            var max_weight_set = true;

            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];
            application_settings set_max_rail_car_weight = application_settings.GetAppSetting("set_max_rail_car_weight") ?? new application_settings();

            if (set_max_rail_car_weight.default_value == "Y")
            {

                max_weight_set = rail_cars.RailCarHasMaxWeight(rail_car_brand + vehicle_no);
            }

            var passed_in_vehicle_no = vehicle_no;
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
            else if (!max_weight_set && max_weight_limit == 0)
            {
                viewModel.set_max_weight = true;
                viewModel.Message = "Please Set the Max Weight for this car";
                return RedirectToAction("LoadRail", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id, rail_car_brand = rail_car_brand, vehicle_no = passed_in_vehicle_no, rail_car_no = rail_car_no, set_max_weight = viewModel.set_max_weight });
            }
            else
            {


                List<shipment_load> loads = new List<shipment_load>();
                if (!max_weight_set)
                {
                    rail_cars.SaveRailCarMaxWeight(vehicle_no, max_weight_limit, current_application_security.user_id);
                }

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
                    if (rsp.location == "C")
                    {
                        shipment_load.UpdateVehicleNoAndLoadingUser(sl.load_id, vehicle_no, current_application_security.user_id);
                        viewModel.Message = "Rail load " + char_load_id + " started with rail car " + vehicle_no;
                    }
                    else
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
                    }

                    List<load_dtl> current_coils = load_dtl.GetLoadDtl(sl.load_id);

                    //add audit record
                    foreach (load_dtl coil in current_coils)
                    {
                        Utils.AddAuditRecord(coil.production_coil_no, "Rail Load Started", sl.char_load_id + " - " + vehicle_no);
                    }
                }
            }

            return RedirectToAction("LoadRail", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
        }

        public bool VerifyCarrierBrand(string char_load_id, string rail_car_brand)
        {
            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];
            ref_sys_param rsp = ref_sys_param.GetRefSysParam();
            int from_freight_location_cd = 77;
            user_defaults default_from_freight_location_cd =
           user_defaults.GetUserDefaultByName(current_application_security.user_id, "FromFreightLocation");
            if (default_from_freight_location_cd != null)
            {
                from_freight_location_cd = Convert.ToInt32(default_from_freight_location_cd.value);
            }

            var carrier_cd = shipment_load.GetLoadCarrier(char_load_id);
            if (rsp.location == "B" && from_freight_location_cd == 77 && rail_car_brand != null)
            {
                return Scanware.Data.rail_car_brand.VerifyCarrierRailCar(carrier_cd, rail_car_brand);
            }
            return true;
        }
        public ActionResult ShipRailLoad(string char_load_id, int? ScannerUsed)
        {
            ShippingModel viewModel = new ShippingModel();

            application_settings REQUIRE_RAIL_IMAGE = application_settings.GetAppSetting("REQUIRE_RAIL_IMAGE");

            if (REQUIRE_RAIL_IMAGE == null)
            {
                viewModel.require_rail_image = "N";
            }
            else
            {
                viewModel.require_rail_image = REQUIRE_RAIL_IMAGE.default_value;
            }

            shipment_load current_load = shipment_load.GetShipmentLoad(char_load_id);

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
                    int load_id = Int32.Parse(char_load_id);

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

                    //Added 11/16 #315192
                    //Make sure this load is not in scanware_loads_ship_after_12 - If it is verify that user has SHIP_AFTER_12 Permission to Ship 
                    string is_after_12_load = scanware_loads_ship_after_12.is_load_ship_after_12(load_id);

                    Session["ShipLoad"] = 0;
                    Session["SHIP_AFTER_12"] = "N";

                    if (is_after_12_load != null && is_after_12_load == "Y")
                    {
                        function_level_security logged_in_user = new function_level_security(System.Web.HttpContext.Current.Session["function_level_security"] as List<function_level_security>);
                        // viewModel.is_after_12_load = "Y";
                        Session["SHIP_AFTER_12"] = "Y";

                        //Ship loads that are marked to ship after 12AM on Month-End (Rail Loads are marked in Shipping Management)
                        if (!logged_in_user.HasFunctionLevelSecurity("SHIP_AFTER_12"))
                        {
                            Session["ShipLoad"] = -1;

                            viewModel.Error = "You are not authorized to ship load " + char_load_id + ". It is marked as 12AM load.";
                            return RedirectToAction("LoadRail", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
                        }

                    }
                    else
                    {
                        Session["ShipLoad"] = load_id.ToString();
                    }
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

                    //Require a load image prior to shipping
                    if (viewModel.require_rail_image == "Y")
                    {
                        function_level_security logged_in_user2 = new function_level_security(System.Web.HttpContext.Current.Session["function_level_security"] as List<function_level_security>);

                        if (!logged_in_user2.HasFunctionLevelSecurity("OVD_RAIL_IMAGE"))
                        {
                            viewModel.load_images = shipment_load_images.GetLoadImages(current_load.load_id);

                            if (viewModel.load_images.Count() == 0)
                            {
                                viewModel.Error = char_load_id + " Rail load is missing Images! Please upload at least one Image prior to Shipping!";
                                return RedirectToAction("LoadRail", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
                            }
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
                                return RedirectToAction("LoadRail", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
                            }

                            shipment_load.UpdateScaleTimeOutButler(load.load_id, current_application_security.user_id);

                            //If sub-load - get weight out value from Master Load
                            if (subLoads.Count() > 0)
                            {
                                int load_id = Int32.Parse(char_load_id);

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

                            if (rsp.location == "S" && current_load.ShipmentType == "P")
                                Utils.AddAuditRecord(coil.production_coil_no, "OP Rail Load Shipped", current_load.char_load_id, scanner_user);
                            else
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
                    return RedirectToAction("LoadRail", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error });
                }
                else if (!all_coils_verified)
                {
                    viewModel.Error = "Not all coils are verified on " + char_load_id;
                    return RedirectToAction("LoadRail", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
                }
                else
                {
                    viewModel.Error = "Coils already marked as shipped on " + char_load_id;
                    return RedirectToAction("LoadRail", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
                }
            }
            else
            {
                viewModel.Error = "Unable to find " + char_load_id;
                return RedirectToAction("LoadRail", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
            }
        }

        public ActionResult ShipTruckLoad(string char_load_id, int? ScannerUsed)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            ref_sys_param rsp = ref_sys_param.GetRefSysParam();

            ShippingModel viewModel = new ShippingModel();

            viewModel.retry_shipping = false;

            viewModel.shipping_loading_instructions = null;

            shipment_load current_load = shipment_load.GetShipmentLoad(char_load_id);

            //Verify that a load was checked-in
            if ((current_load.scale_weight_in == null || current_load.scale_weight_in == 0) || (current_load.initial_in == null) || (current_load.vehicle_no == null))
            {
                viewModel.Error = "Missing Truck Check-In Information " + char_load_id;
                viewModel.all_coils_verified = false;
                return RedirectToAction("LoadTruck", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
            }

            if (current_load != null)
            {
                List<int> subLoads = new List<int>();
                int load_id = Int32.Parse(char_load_id);
                List<shipment_load> current_loads = new List<shipment_load>();
                List<load_dtl> current_coils = new List<load_dtl>();

                //Check if this is a subload - Butler
                if (rsp.location != "C" && char_load_id != null)
                {
                    subLoads = shipment_load.isMasterLoad(char_load_id);

                    if (subLoads.Count() > 0)
                    {
                        current_loads = db.shipment_load.Where(x => x.master_load_id == load_id).ToList();
                    }
                    else
                    {
                        current_loads = db.shipment_load.Where(x => x.load_id == load_id).ToList();
                    }
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
                            return RedirectToAction("CheckInOutTruck", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
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
                                return RedirectToAction("LoadTruck", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
                            }

                            string error = shipment_load.UpdateScaleTimeOutButler(load.load_id, current_application_security.user_id);

                            if (error != "SUCCESS")
                            {
                                viewModel.Error = error + ". Please contact Level 3!";
                                return RedirectToAction("LoadTruck", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
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

                        if(rsp.location == "S" && current_load.ShipmentType == "P")
                            Utils.AddAuditRecord(coil.production_coil_no, "OP Truck Load Shipped", current_load.char_load_id, scanner_user);
                        else
                            Utils.AddAuditRecord(coil.production_coil_no, "Truck Load Shipped", current_load.char_load_id, scanner_user);
                        
                        
                    }

                    viewModel.Message = char_load_id + " has been shipped in the system";
                    return RedirectToAction("LoadTruck", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error });
                }
                else
                {
                    viewModel.Error = "Not all coils are verified on " + char_load_id;
                    return RedirectToAction("LoadTruck", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
                }
            }
            else
            {
                viewModel.Error = "Unable to find " + char_load_id;
                return RedirectToAction("LoadTruck", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = char_load_id });
            }
        }

        public ActionResult CancelRailLoad(string char_load_id)
        {
            ShippingModel viewModel = new ShippingModel();

            shipment_load current_shipment = shipment_load.GetShipmentLoad(char_load_id);

            List<shipment_load> loads = new List<shipment_load>();

            //if sub loads involved
            if (current_shipment.char_load_id.Replace("-901", "") != current_shipment.char_load_id)
            {
                loads = shipment_load.GetShipmentLoadSubLoads(current_shipment.char_load_id);
            }
            else
            {
                //just add the current load
                loads.Add(current_shipment);
            }

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

            return RedirectToAction("LoadRail", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error });
        }

        public ActionResult PrintRailLoads(DateTime? scheduled_date, string Message, string Error, string char_load_id, string ship_to_location_name, string rail_route, string rail_car_type, string print_status)
        {
            ShippingModel viewModel = new ShippingModel();
            viewModel.Message = Message;
            viewModel.Error = Error;
            viewModel.searched_scheduled_date = scheduled_date ?? DateTime.Now.Date;
            viewModel.searched_rail_car_type = rail_car_type;
            viewModel.searched_char_load_id = char_load_id;
            viewModel.searched_rail_route = rail_route;
            viewModel.searched_print_status = print_status;
            viewModel.searched_ship_to_location_name = ship_to_location_name;

            if (viewModel.searched_scheduled_date == DateTime.Now.Date)
            {
                viewModel.print_rail_loads = v_sw_print_rail_loads.GetRailLoadsToPrintWithPriorUnshipped(viewModel.searched_scheduled_date);
            }
            else
            {
                viewModel.print_rail_loads = v_sw_print_rail_loads.GetRailLoadsToPrint(viewModel.searched_scheduled_date);
            }

            viewModel.ship_to_location_names = viewModel.print_rail_loads.OrderBy(x => x.ship_to_location_name_long).Select(m => m.ship_to_location_name_long).Distinct();
            viewModel.char_load_ids = viewModel.print_rail_loads.OrderBy(x => x.char_load_id).Select(m => m.char_load_id).Distinct();
            viewModel.rail_routes = viewModel.print_rail_loads.OrderBy(x => x.rail_route).Select(m => m.rail_route).Distinct();
            viewModel.rail_car_types = viewModel.print_rail_loads.OrderBy(x => x.rail_car_type).Select(m => m.rail_car_type).Distinct();

            if (char_load_id != "" && char_load_id != null)
            {
                viewModel.print_rail_loads = viewModel.print_rail_loads.Where(x => x.char_load_id == char_load_id).ToList();
            }

            if (rail_car_type != "" && rail_car_type != null)
            {
                viewModel.print_rail_loads = viewModel.print_rail_loads.Where(x => x.rail_car_type == rail_car_type).ToList();
            }

            if (rail_route != "" && rail_route != null)
            {
                viewModel.print_rail_loads = viewModel.print_rail_loads.Where(x => x.rail_route == rail_route).ToList();
            }

            if (ship_to_location_name != "" && ship_to_location_name != null)
            {
                viewModel.print_rail_loads = viewModel.print_rail_loads.Where(x => x.ship_to_location_name_long == ship_to_location_name).ToList();
            }

            if (print_status == "Printed")
            {
                viewModel.print_rail_loads = viewModel.print_rail_loads.Where(x => x.print_datetime != null).ToList();
            }

            if (print_status == "Not Printed")
            {
                viewModel.print_rail_loads = viewModel.print_rail_loads.Where(x => x.print_datetime == null).ToList();
            }

            return View(viewModel);
        }


        public ActionResult PrintRailLoadsSubmit(string[] print_load_ids, string[] stage_load_ids, string print_mode, DateTime schedule_date, string char_load_id, string ship_to_location_name, string rail_route, string rail_car_type, string print_status)
        {
            ShippingModel viewModel = new ShippingModel();

            string ls_loc = Properties.Settings.Default.RegLocation;
            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

            if (stage_load_ids == null)
            {
                stage_load_ids = new string[] { "" };
            }

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            //process staging
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            foreach (string load_id in stage_load_ids)
            {
                //do not process if null from above
                if (load_id != "")
                {
                    shipment_load sl = shipment_load.GetShipmentLoad(Convert.ToInt32(load_id));

                    if (sl.stage_rail == null || sl.stage_rail == "N")
                    {
                        shipment_load.UpdateStageRail(sl.load_id, "Y", current_application_security.user_id);

                        if (viewModel.Message == "" || viewModel.Message == null)
                        {
                            viewModel.Message = "The Following Loads have been staged:";
                        }

                        viewModel.Message = viewModel.Message + " " + sl.char_load_id + ",";

                        List<load_dtl> coils_in_shipment = load_dtl.GetLoadDtl(sl.load_id);

                        //log staged event
                        foreach (load_dtl coil in coils_in_shipment)
                        {
                            Utils.AddAuditRecord(coil.production_coil_no, "Load Staged", sl.char_load_id);
                        }
                    }
                }
            }

            List<int> StagedRailLoads = new List<int>();

            if (schedule_date.Date == DateTime.Now.Date)
            {
                StagedRailLoads = shipment_load.GetStagedRailLoadsWithPriorUnshipped(schedule_date, char_load_id, ship_to_location_name, rail_route, rail_car_type, print_status);
            }
            else
            {
                StagedRailLoads = shipment_load.GetStagedRailLoads(schedule_date, char_load_id, ship_to_location_name, rail_route, rail_car_type, print_status);
            }

            int first_time_through = 0;

            //blank out the ones that were un checked
            foreach (int sl in StagedRailLoads)
            {
                int count = stage_load_ids.Where(x => x == sl.ToString()).Count();

                if (count == 0)
                {
                    if (first_time_through == 0)
                    {
                        viewModel.Message = viewModel.Message + "The Following Loads have been un-staged: ";
                        first_time_through = 1;
                    }

                    shipment_load.UpdateStageRail(sl, "N", current_application_security.user_id);

                    shipment_load unstaged_shipment_load = shipment_load.GetShipmentLoad(sl);

                    List<load_dtl> coils_in_shipment = load_dtl.GetLoadDtl(unstaged_shipment_load.load_id);

                    //log staged event
                    foreach (load_dtl coil in coils_in_shipment)
                    {
                        Utils.AddAuditRecord(coil.production_coil_no, "Load Un-Staged", unstaged_shipment_load.char_load_id);
                    }

                    viewModel.Message = viewModel.Message + " " + unstaged_shipment_load.char_load_id + ",";
                }
            }
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            //end process staging
            //////////////////////////////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////////////////////////////
            //process printing
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            user_defaults default_zebra_printer = user_defaults.GetUserDefaultByName(current_application_security.user_id, "ScanwareZebraPrinter");

            //confirm default printer is setup
            if (default_zebra_printer != null)
            {
                viewModel.default_zebra_printer = printer.GetPrinterByPK(Convert.ToInt32(default_zebra_printer.value));
            }

            //check for default printer
            if (viewModel.default_zebra_printer == null)
            {
                return RedirectToAction("PrintRailLoads", "Shipping", new { Error = "You do not have a default zebra printer setup. Click your login name in the upper right hand corner and set a default zebra printer." });
            }

            user_defaults default_network_printer = user_defaults.GetUserDefaultByName(current_application_security.user_id, "ScanwareNetworkPrinter");

            //confirm default printer is setup
            if (default_network_printer != null)
            {
                viewModel.default_network_printer = printer.GetPrinterByPK(Convert.ToInt32(default_network_printer.value));
            }

            //check for default printer
            if (viewModel.default_network_printer == null)
            {
                return RedirectToAction("PrintRailLoads", "Shipping", new { Error = "You do not have a default network printer setup. Click your login name in the upper right hand corner and set a default network printer." });
            }

            first_time_through = 0;

            if (print_load_ids != null)
            {
                string template = "";

                foreach (string load_id in print_load_ids)
                {
                    shipment_load sl = shipment_load.GetShipmentLoad(Convert.ToInt32(load_id));
                    List<load_dtl> coils_in_shipment = load_dtl.GetLoadDtl(sl.load_id);

                    //If both or load card print load card
                    if (print_mode == "Both" || print_mode == "Load Card")
                    {
                        //print load card
                        v_zebra_template_load_card load_card = v_zebra_template_load_card.GetLoadCardTemplate(sl.load_id);

                        template = template + load_card.template;


                        //Do not reprint shipping tags for the techs
                        if (ls_loc != "T")
                        {
                            foreach (load_dtl coil in coils_in_shipment)
                            {
                                //get ship tag template
                                v_zebra_template_coil current_template = v_zebra_template_coil.GetCoilTemplate(coil.production_coil_no, "Ship Tag");

                                template = template + current_template.template;
                            }
                        }
                    }

                    //if both or BOL print BOL
                    if (print_mode == "Both" || print_mode == "BOL")
                    {
                        if (ls_loc == "T")
                        {
                            if (ZPLUtils.HighDPI(viewModel.default_zebra_printer))
                                template = ZPLUtils.ScaleZPL(template, null);

                            //TODO: Populate & print crane tag for rail cars

                            //if (pickup_no != null)
                            //    template += ZPLUtils.CraneTagZPL(pickup_no.Value,
                            //        ZPLUtils.HighDPI(viewModel.default_zebra_printer));

                            //Print Pick List SSRS Report for a given Truck Sequence


                            Utils.PrintPickList(viewModel.default_network_printer,
                                sl.load_id,
                                "XXX");
                        }


                        //print paper work
                        //Expand BOL for multi-customer loads
                        var custIDs = load_dtl.GetCustomersIDsInLoad(sl.load_id);

                        foreach (var custId in custIDs)
                        {
                            //Print Paperwork
                            Utils.PrintBOL(viewModel.default_network_printer,
                                custId,
                                sl.load_id,
                                ls_loc == "T" ? 3 : 2,
                                true);
                        }
                    }

                    shipment_load.UpdatePrintDateTime(sl.load_id, current_application_security.user_id);

                    if (first_time_through == 0)
                    {
                        first_time_through = 1;

                        viewModel.Message = viewModel.Message + "The Following Loads have been printed: ";
                    }

                    viewModel.Message = viewModel.Message + " " + sl.char_load_id + ",";

                    //add print audit record
                    foreach (load_dtl coil in coils_in_shipment)
                    {
                        Utils.AddAuditRecord(coil.production_coil_no, "Load Printed", sl.char_load_id + " - " + print_mode);
                    }
                }

                //If both or load card print load card
                if (print_mode == "Both" || print_mode == "Load Card")
                {

                    if (ls_loc == "T")
                    {
                        Utils.TCPTemplateToZebra(viewModel.default_zebra_printer, template);
                    }
                    else
                    {
                        Utils.FTPTemplateToZebra(viewModel.default_zebra_printer, template, "load_cards");
                    }
                }

                //update messaging
                if (print_mode != "Both")
                {
                    viewModel.Message = viewModel.Message + " - " + print_mode + " only!";
                }
            }

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            //end process printing
            //////////////////////////////////////////////////////////////////////////////////////////////////////

            return RedirectToAction("PrintRailLoads", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error, scheduled_date = schedule_date, char_load_id = char_load_id, ship_to_location_name = ship_to_location_name, rail_route = rail_route, rail_car_type = rail_car_type, print_status = print_status });
        }

        [HttpPost]
        public ActionResult UploadFiles(int load_id, HttpPostedFileBase[] upload_files)
        {
            ShippingModel viewModel = new ShippingModel();

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
                return RedirectToAction("LoadRail", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = sl.char_load_id });
            }
            else //if (sl.carrier_mode == "T")
            {
                return RedirectToAction("LoadTruck", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = sl.char_load_id });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadShipmentLoadImage(int load_id, HttpPostedFileBase upload_file)
        {
            ShippingModel viewModel = new ShippingModel();

            shipment_load sl = shipment_load.GetShipmentLoad(load_id);

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

                    viewModel.Message = "Image uploaded for " + sl.load_id.ToString() + "/" + sl.char_load_id;
                }
                catch (Exception ex)
                {
                    viewModel.Error = ex.ToString();
                }
            }

            if (sl.carrier_mode == "R")
            {
                return RedirectToAction("LoadRail", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = sl.char_load_id });
            }
            else //if (sl.carrier_mode == "T")
            {
                return RedirectToAction("LoadTruck", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = sl.char_load_id });
            }
        }

        public ActionResult UploadShipmentLoadImages(int load_id, HttpPostedFileBase[] upload_files)
        {
            ShippingModel viewModel = new ShippingModel();

            shipment_load sl = shipment_load.GetShipmentLoad(load_id);

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
                        viewModel.Message = upload_files.Count().ToString() + " Images uploaded for " + sl.load_id.ToString() + "/" + sl.char_load_id;

                    }
                }
                catch (Exception ex)
                {
                    viewModel.Error = ex.ToString();
                }
            }

            if (sl.carrier_mode == "R")
            {
                return RedirectToAction("LoadRail", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = sl.char_load_id });
            }
            else //if (sl.carrier_mode == "T")
            {
                return RedirectToAction("LoadTruck", "Shipping", new { Message = viewModel.Message, Error = viewModel.Error, char_load_id = sl.char_load_id });
            }
        }

        public ActionResult ViewShipmentLoadImage(int image_no)
        {
            ShippingModel viewModel = new ShippingModel();

            viewModel.load_image = shipment_load_images.GetLoadImage(image_no);
            viewModel.load_image_bytes = v_shipment_load_images.GetLoadImage(image_no);
            viewModel.shipment = shipment_load.GetShipmentLoad(viewModel.load_image.load_id);

            return View(viewModel);
        }

        /* BEGIN COIL REAUTHORIZATION BLOCK */

        //public ActionResult CoilReauthorization()
        //{
        //    ShippingModel viewModel = new ShippingModel();

        //    return View(viewModel);
        //}
        public ActionResult CoilReauthorization(string old_coil_no)
        {
            ShippingModel viewModel = new ShippingModel();
            if (old_coil_no != "" && old_coil_no != null)
            {
                application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

                //make sure coil is valid
                if (all_produced_coils.GetAPCCoilReauthorization(old_coil_no).Count != 1)
                {
                    ViewBag.alert = old_coil_no + " is not a valid coil number in L3.";
                    ViewBag.alert_class = "panel panel-red";
                    return View(viewModel);
                }
                //make sure coil is shipped
                if (coil.GetCoilByProductionCoilNumber(old_coil_no).coil_status != "SC")
                {
                    ViewBag.alert = old_coil_no + " is not showing shipped final in L3.";
                    ViewBag.alert_class = "panel panel-red";
                    return View(viewModel);
                }
                //make sure coil is not already re-authorized
                if (coil_reauthorization.GetCoilReAuthorization(old_coil_no).Count() >= 1)
                {
                    ViewBag.alert = old_coil_no + " has already been re-authorized.";
                    ViewBag.alert_class = "panel panel-red";
                    return View(viewModel);
                }

                //print functionality
                user_defaults default_network_printer = user_defaults.GetUserDefaultByName(current_application_security.user_id, "ScanwareNetworkPrinter");

                //confirm default printer is setup
                if (default_network_printer != null)
                {
                    viewModel.default_network_printer = printer.GetPrinterByPK(Convert.ToInt32(default_network_printer.value));
                }

                //check for default printer
                if (viewModel.default_network_printer == null)
                {
                    //return RedirectToAction("PrintRailLoads", "Shipping", new { Error = "You do not have a default network printer setup. Click your login name in the upper right hand corner and set a default network printer." });
                    ViewBag.alert = "You do not have a default network printer setup. Click your login name in the upper right hand corner and set a default network printer.";
                    ViewBag.alert_class = "panel panel-red";
                    return View(viewModel);
                }

                try
                {
                    usp.usp_coil_reauthorization(old_coil_no, current_application_security.user_id);

                    var reauth = coil_reauthorization.GetCoilReAuthorization(old_coil_no);
                    var new_coil_no = reauth.FirstOrDefault().new_coil_no;
                    var apc = all_produced_coils.GetAPCCoilReauthorization(new_coil_no);
                    var ra_id = reauth.FirstOrDefault().ra_id;

                    Utils.PrintReathorization(viewModel.default_network_printer, ra_id);

                    if (reauth.Count() == 1)
                    {
                        ViewBag.alert = "Re-Authorization # " + ra_id + " has been submitted for " + old_coil_no + ". New Coil #: " + new_coil_no + ".  The Re-Authorization paperwork has been sent to print.";
                        ViewBag.alert_class = "panel panel-green";
                    }
                    else
                    {
                        ViewBag.alert = "Coil was NOT successfully inserted back in to L3 inventory.  Please contact L3.";
                        ViewBag.alert_class = "panel panel-red";
                    }

                }
                catch (Exception)
                {
                    ViewBag.alert = "The Re-Authorization failed.  Please contact IT.";
                    ViewBag.alert_class = "panel panel-red";
                }
            }

            return View(viewModel);
        }

        /* END COIL REAUTHORIZATION BLOCK */
    }
}
