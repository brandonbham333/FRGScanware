using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Scanware.Data;
using Scanware.Models;
using Scanware.App_Objects;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using Scanware.Ancestor;

namespace Scanware.Controllers
{
    [Authorize]
    public partial class CoilController : Controller
    {
        public ActionResult CoilsInColumn(string column)
        {
            CoilModel viewModel = new CoilModel();

            viewModel.AllCoilYardCols = coil_yard_columns.GetCoilYardColumns();


            if (column != "")
            {
                viewModel.CoilYardLocations = coil_yard_locations.GetCoilYardLocations(column);
            }

            return View(viewModel);
        }

        public ActionResult Index(string production_coil_no, string Message, string Error, string alias = "")
        {
            CoilModel viewModel = new CoilModel();

            viewModel.loc = Properties.Settings.Default.RegLocation;

            if (production_coil_no != null && production_coil_no.Length > 0)
            {
                if (production_coil_no[0].ToString().ToLower() == "s")
                {
                    production_coil_no = production_coil_no.Substring(1);
                }
            }

            int user_id = user_info.GetUserInfo(User.Identity.Name).application_security
                .Where(x => x.app_name == "Scanware").First().user_id;
            function_level_security fls = function_level_security.GetUserFunctionLevelSecurity(user_id, "Scanware")
                .Where(x => x.function_name == "SHIPPING-HOT").FirstOrDefault();
            viewModel.has_HOT_fls = fls != null;

            var coilAliases = usp.GetCoilAliases(alias);

            viewModel.searched_coil_number = production_coil_no;
            viewModel.Error = Error;
            viewModel.Message = Message;
            viewModel.ScanwareHoldDefects = claim_reason.GetScanwareHoldDefects();
            ViewBag.CoilAliases = coilAliases;

            if (production_coil_no != "" && production_coil_no != null)
            {

                //check if it is an int assuming cons coil number
                int cons_coil_no;

                if (int.TryParse(production_coil_no, out cons_coil_no))
                {
                    viewModel.current_all_produced_coil =
                        all_produced_coils.GetAllProducedCoilLastProduced(cons_coil_no);
                }


                if (viewModel.current_all_produced_coil == null)
                {
                    //get all produced coil
                    viewModel.current_all_produced_coil =
                        all_produced_coils.GetAllProducedCoilByProductionCoilNumberOrTagNumber(production_coil_no);
                    viewModel.searched_coil_consumed = false;

                    if (viewModel.current_all_produced_coil != null)
                    {
                        //if coil is consumed or cons coil number entered, find non consumed coil and return it
                        if (viewModel.current_all_produced_coil.coil_last_facility_ind == "N")
                        {
                            viewModel.searched_coil_consumed = true;
                            viewModel.current_all_produced_coil =
                                all_produced_coils.GetAllProducedCoilLastProduced(
                                    Convert.ToInt32(viewModel.current_all_produced_coil.cons_coil_no));
                        }
                    }
                }

                if (viewModel.current_all_produced_coil != null)
                {
                    if (viewModel.current_all_produced_coil.produced_dt_stamp > DateTime.Now.AddYears(-2))
                    {
                        viewModel.coil_history =
                            coil_audit_trail.GetCoilHistoryByCons(
                                Convert.ToInt32(viewModel.current_all_produced_coil.cons_coil_no));
                    }
                    else
                    {
                        viewModel.coil_history =
                            coil_audit_trail.GetCoilHistoryByProductionCoilNumber(viewModel.current_all_produced_coil
                                .production_coil_no);
                    }
                }
                else
                {
                    viewModel.Error = "Unable to find " + production_coil_no + " in L3";
                    viewModel.current_all_produced_coil = new all_produced_coils();
                    return View(viewModel);
                }

                //get coil
                viewModel.current_coil =
                    coil.GetCoilByProductionCoilNumber(viewModel.current_all_produced_coil.production_coil_no);

                //if coil is in coil table/on order
                if (viewModel.current_coil != null)
                {
                    try
                    {
                        viewModel.coil_customer_ship_to = customer_ship_to.GetCustomerShipTo(
                            Convert.ToInt32(viewModel.current_coil.customer_order_line_item.customer_id),
                            viewModel.current_coil.ship_to_location_name);
                        if (viewModel.current_coil.coil_status.Length > 2)
                        {
                            viewModel.current_coil_status = new coil_status { description = viewModel.current_coil.coil_status };
                        }
                        else
                        {
                            viewModel.current_coil_status = coil_status.GetCoilStatus(viewModel.current_coil.coil_status);
                        }
                        
                        viewModel.char_load_id = coil.GetCharLoadID(viewModel.current_coil.production_coil_no);
                        viewModel.order_product_type = product_type.GetProductType(
                            Convert.ToInt32(viewModel.current_coil.customer_order_line_item.product_type_cd));
                        viewModel.current_packaging_type = packaging_type.GetPackagingType(
                            Convert.ToInt32(viewModel.current_coil.customer_order_line_item.finish_line_setup
                                .packaging_type_cd));
                    }
                    catch (Exception e)
                    {
                        viewModel.Error = "Could not return coil data!";
                    }

            }
            else
            {
                //not in coil table
                try
                {
                    viewModel.current_coil = new coil();
                    viewModel.current_inventory_item =
                        inventory_item.GetInventoryItemByProductionCoilNumber(viewModel.current_all_produced_coil
                            .production_coil_no);
                    viewModel.current_inventory_reason =
                        inventory_reason.GetInventoryReason(viewModel.current_inventory_item.inventory_reason_cd);
                }
                catch (NullReferenceException)
                {
                    viewModel.Error = "Coil has not been received.";
                }
            }

            viewModel.sister_coils =
                    viewModel.current_all_produced_coil.produced_dt_stamp > DateTime.Now.AddYears(-2)
                        ? all_produced_coils.GetSisterCoils(
                            Convert.ToInt32(viewModel.current_all_produced_coil.cons_coil_no))
                        : new List<all_produced_coils>();

                viewModel.current_coil_yard_location =
                    coil_yard_locations.GetCoilYardLocation(viewModel.current_all_produced_coil.production_coil_no);

                if (viewModel.current_coil_yard_location == null)
                {
                    viewModel.current_coil_yard_location = new coil_yard_locations()
                    {
                        row = "",
                        column = ""
                    };
                }

                viewModel.coil_images =
                    v_apc_images.GetCoilImages(Convert.ToInt32(viewModel.current_all_produced_coil.cons_coil_no));
            }

            return View(viewModel);
        }

        public ActionResult MoveCoil(string column, string row, string production_coil_no, string scanner_used,
            string bay_cd, string scanned_location)
        {
            //, string shipped_coil_no
            CoilModel viewModel = new CoilModel();
            //viewModel.production_coil_no = production_coil_no;
            string update = "YES";
            if (production_coil_no != null && production_coil_no.Length > 0)
            {
                if (production_coil_no[0].ToString().ToLower() == "s")
                {
                    production_coil_no = production_coil_no.Substring(1);
                }
            }


            //check if scanned_location
            if ((column == "" || column == null) && (row == "" || row == null))
            {
                if (scanned_location != "" && scanned_location != null && scanned_location != " " &&
                    scanned_location.Contains("$"))
                {

                    var colRow = GetColumnAndRow(scanned_location);
                    column = colRow.Item1;
                    row = colRow.Item2;
                    viewModel.current_loc = scanned_location;

                }
            }

            ref_sys_param rsp = ref_sys_param.GetRefSysParam();

            viewModel.searched_coil_number = production_coil_no;

            application_security current_application_security =
                (application_security)System.Web.HttpContext.Current.Session["application_security"];

            viewModel.CoilYardBays = coil_yard_bays.GetAllCoilYardBays();

            string session_bay_cd = "";

            if (viewModel.CoilYardBays.Count > 0)
            {
                viewModel.current_coil_yard_bay = new coil_yard_bays();

                //if bay set then set session
                if (bay_cd != null)
                {
                    Session["bay_cd"] = bay_cd;
                }

                if (Session["bay_cd"] != null && Session["bay_cd"].ToString() != "")
                {
                    session_bay_cd = Session["bay_cd"].ToString();
                    viewModel.current_coil_yard_bay = coil_yard_bays.GetCoilYardBay(session_bay_cd);
                }

            }

            //to support jville users using location 20
            user_defaults default_from_freight_location_cd =
                user_defaults.GetUserDefaultByName(current_application_security.user_id, "FromFreightLocation");
          
            int from_freight_location_cd = 77;

            if (default_from_freight_location_cd != null)
            {
                from_freight_location_cd = Convert.ToInt32(default_from_freight_location_cd.value);
            }
            //var isJville = from_freight_location_cd == 20;
            //var current_coils_status = coil.GetCoilStatusByProductionCoilNumber(production_coil_no);
           // var moveCoil = true;
            //if (isJville)
            //{
            //    if (shipped_coil_no != null && production_coil_no != shipped_coil_no)
            //    {
            //        viewModel.Error = "The coil number and Shipped Tag do not match.";
            //        moveCoil = false;
            //        viewModel.add_jville_check = true;
            //    }
            //    if (current_coils_status == "SE" && (shipped_coil_no == null || production_coil_no != shipped_coil_no))
            //    {
            //        viewModel.Error = "Please Scan the Bill of Lading";
            //        moveCoil = false;
            //        viewModel.add_jville_check = true;

            //    }
            //    viewModel.shipped_coil_no = shipped_coil_no;
               

            //}
            
            if (session_bay_cd != "")
            {
                viewModel.CoilYardCols =
                    coil_yard_columns.GetAllCoilYardColumns(from_freight_location_cd, session_bay_cd);
            }
            else
            {
                viewModel.CoilYardCols = coil_yard_columns.GetAllCoilYardColumns(from_freight_location_cd);
            }

            if (column != "" && column != null)
            {
                viewModel.current_column = column;

                if (session_bay_cd != "")
                {
                    viewModel.CoilYardRows = coil_yard_rows.GetRowsByColumnAndBay(column, session_bay_cd);
                }
                else
                {
                    viewModel.CoilYardRows = coil_yard_rows.GetRowsByColumn(column);
                }

            }
            else
            {
                viewModel.CoilYardRows = new List<coil_yard_rows>();
            }

            if (row != "" && row != null)
            {
                viewModel.current_row = row;
            }

            if (production_coil_no != "" && production_coil_no != null && row != "")
            {
                viewModel.current_all_produced_coil =
                    all_produced_coils.GetAllProducedCoilByProductionCoilNumberOrTagNumber(production_coil_no);

                //check if coil exists
                if (viewModel.current_all_produced_coil == null)
                {
                    viewModel.Error = "Unable to find " + production_coil_no + " in L3";
                    viewModel.current_all_produced_coil = new all_produced_coils();
                    viewModel.current_all_produced_coil.production_coil_no = production_coil_no;

                    return View(viewModel);
                }

                //CHECK FOR CONSUMED MOVEMENT SETTING 
                bool consumedMovementSetting = ConsumedMovementSetting();
                if (!consumedMovementSetting && IsCoilConsumed(production_coil_no, viewModel))
                {
                    viewModel.Alert = MoveConsumedCoil(production_coil_no, viewModel, scanner_used);
                    if (viewModel.Alert.Contains(""))
                    {
                        update = "YES";
                    }
                    else
                    {
                        update = "NO";
                    }
                }
                else
                {

                    viewModel.current_coil =
                        coil.GetCoilByProductionCoilNumber(viewModel.current_all_produced_coil.production_coil_no);

                    try
                    {
                        if (viewModel.current_coil.coil_status == "SC")
                        {
                            viewModel.Alert = viewModel.current_all_produced_coil.production_coil_no +
                                              " has been shipped but has been moved successfully. Coil will show on mis-shipped report.";
                        }
                    }
                    catch
                    {
                    } //WILL ERROR IF THEY MOVE A CONSUMED COIL

                    if (IsCoilConsumed(production_coil_no, viewModel))
                    {
                        viewModel.Alert = viewModel.current_all_produced_coil.production_coil_no +
                                          " is consumed but has been moved successfully. Coil will show on consumed coil report.";
                    }

                    viewModel.current_coil_yard_location =
                        coil_yard_locations.GetCoilYardLocation(viewModel.current_all_produced_coil.production_coil_no);

                    //if butler update location if exists
                    if (rsp.location != "C")
                    {
                        update = "NO";
                        //Make sure row and column selected
                        if (column != null && row != null)
                        {
                            update = "YES";
                            //if exists then update
                            if (viewModel.current_coil_yard_location != null)
                            {
                                viewModel.new_coil_yard_location =
                                    coil_yard_locations.UpdateCoilYardLocation(
                                        viewModel.current_all_produced_coil.production_coil_no, column, row,
                                        current_application_security.user_id, scanner_used);
                            }
                            else
                            {
                                viewModel.new_coil_yard_location =
                                    coil_yard_locations.InsertCoilYardLocation(
                                        viewModel.current_all_produced_coil.production_coil_no, column, row,
                                        current_application_security.user_id, scanner_used);
                            }

                        }

                        // else
                        //   {
                        //       viewModel.Error = "No Location selected for Coil " + production_coil_no + ".";
                        //       return PartialView("MoveCoilButler", viewModel);
                        //   }
                    }
                    else //for columbus lomas triggering delete than add
                    {

                        //delete if exists to reinsert later
                        if (viewModel.current_coil_yard_location != null)
                        {
                            //if current location is MSP and we are moving back here, initiate coil validate process and email
                            if (viewModel.current_coil_yard_location.column == "MSP")
                            {
                                vw_sw_inbound_op_coils inbound_op =
                                    vw_sw_inbound_op_coils.GetOPInboundOPCoil(viewModel.current_all_produced_coil
                                        .production_coil_no);

                                if (inbound_op != null)
                                {
                                    //only initiate if shipping final and market like automotive
                                    if ((inbound_op.next_facility_cd == "I" || inbound_op.next_facility_cd == "") &&
                                        inbound_op.end_use_market_name.ToUpper().Contains("AUTOMOTIVE"))
                                    {


                                        viewModel.Alert =
                                            "Automotive Coil Returned from MSP. Weight must be validated!";

                                        sw_op_coil_validate.InsertNewOPCoil(
                                            viewModel.current_all_produced_coil.production_coil_no,
                                            current_application_security.user_id);

                                        SmtpClient SmtpServer = new SmtpClient(rsp.mail_server);
                                        SmtpServer.Timeout = 200000;

                                        MailMessage myMail = new MailMessage();
                                        myMail.IsBodyHtml = true;
                                        myMail.From = new MailAddress("scanware@steeldynamics.com", "scanware");

                                        //add all the email addresses
                                        foreach (scanware_hold_coil_email email in scanware_hold_coil_email
                                            .GetHoldEmailsByFacility("2")) //op coil return validate group
                                        {
                                            myMail.To.Add(email.email_address);

                                        }

                                        myMail.Subject = "Automotive OP Coil Returned - " +
                                                         viewModel.current_all_produced_coil.production_coil_no;
                                        myMail.Body = "New Location: " + column + " - " + row + "<br/>User Name: " +
                                                      current_application_security.user_name;


                                        //send email
                                        SmtpServer.Send(myMail);

                                        //audit record in scanware
                                        Utils.AddAuditRecord(viewModel.current_all_produced_coil.production_coil_no,
                                            "Automotive OP Validate Initiated",
                                            viewModel.current_coil_yard_location.column);

                                    }
                                }

                            }

                            coil_yard_locations.DeleteCoilYardLocation(viewModel.current_all_produced_coil
                                .production_coil_no);
                        }

                        viewModel.new_coil_yard_location = coil_yard_locations.InsertCoilYardLocation(
                            viewModel.current_all_produced_coil.production_coil_no, column, row,
                            current_application_security.user_id, scanner_used);
                    }
                }

                if (update == "YES")
                {
                    usp.usp_sw_update_status_after_move(viewModel.current_all_produced_coil.production_coil_no,
                        current_application_security.user_id);
                }
            }

            return View(viewModel);
        }



        public ActionResult LineProduction(string facility_cd, string column, string row, string saddle, string Message,
            string Error)
        {
            CoilModel viewModel = new CoilModel();

            viewModel.Message = Message;
            viewModel.Error = Error;

            viewModel.schedule_product_processors = product_processors.GetInsideProductProcessors();
            viewModel.current_facility_cd = facility_cd;
            viewModel.current_column = column;
            viewModel.current_row = row;
            viewModel.current_saddle = saddle;

            application_security current_application_security =
                (application_security)System.Web.HttpContext.Current.Session["application_security"];

            //to support jville users using location 20
            user_defaults default_from_freight_location_cd =
                user_defaults.GetUserDefaultByName(current_application_security.user_id, "FromFreightLocation");

            int from_freight_location_cd = 77;

            if (default_from_freight_location_cd != null)
            {
                from_freight_location_cd = Convert.ToInt32(default_from_freight_location_cd.value);
            }

            viewModel.CoilYardCols = coil_yard_columns.GetAllCoilYardColumns(from_freight_location_cd);

            if (column != "")
            {
                viewModel.current_column = column;
                viewModel.CoilYardRows = coil_yard_rows.GetRowsByColumn(column);
            }

            if (row != "")
            {
                viewModel.current_row = row;
            }

            if (facility_cd != "" && facility_cd != null)
            {
                viewModel.produced_coils = vw_sw_produced_coils.GetProducedCoilsByFacility(facility_cd);
            }

            return View(viewModel);
        }

        public ActionResult MoveProducedCoilToLocation(string facility_cd, string column, string row, string saddle,
            string[] production_coil_no)
        {
            string log_message = "";
            string Error = "";
            product_processors selected_processor =
                product_processors.GetInsideProductProcessorFromFacilityCD(facility_cd);


            if (production_coil_no == null)
            {
                Error = "No coils selected";
            }
            else
            {
                foreach (string coil in production_coil_no)
                {
                    if (log_message == "")
                    {

                        log_message = "The following coils were moved to " + column + " - " + row + ": " + coil;

                    }
                    else
                    {

                        log_message = log_message + ", " + coil;

                    }

                    coil_yard_locations current_coil_yard_location = coil_yard_locations.GetCoilYardLocation(coil);

                    application_security current_application_security =
                        (application_security)System.Web.HttpContext.Current.Session["application_security"];

                    //delete if exists to reinsert later
                    if (current_coil_yard_location != null)
                    {
                        coil_yard_locations.DeleteCoilYardLocation(coil);
                    }

                    coil_yard_locations new_coil_yard_location =
                        coil_yard_locations.InsertCoilYardLocation(coil, column, row,
                            current_application_security.user_id, "P");


                    usp.usp_sw_update_status_after_move(coil, current_application_security.user_id);

                }
            }

            return RedirectToAction("LineProduction", "Coil",
                new
                {
                    facility_cd = facility_cd,
                    column = column,
                    row = row,
                    saddle = saddle,
                    Message = log_message,
                    Error = Error
                });
        }

        public ActionResult Packaging(string TypeOfPackaging, string facility_cd, string production_coil_no)
        {
            CoilModel viewModel = new CoilModel();
            viewModel.current_facility_cd = facility_cd;
            viewModel.current_type_of_packaging = TypeOfPackaging;

            viewModel.schedule_product_processors = product_processors.GetInsideProductProcessors();

            if (production_coil_no != "" && production_coil_no != null)
            {

                application_security current_application_security =
                    (application_security)System.Web.HttpContext.Current.Session["application_security"];

                int next_action_sequence_number = 1;

                viewModel.current_all_produced_coil =
                    all_produced_coils.GetAllProducedCoilByProductionCoilNumberOrTagNumber(production_coil_no);


                //has it been packaged before?
                coil_audit_trail prior_package =
                    coil_audit_trail.GetLatestRecordByActionDescription(production_coil_no, "Packaged");

                if (prior_package != null)
                {
                    viewModel.Alert = production_coil_no + " was previously packaged " +
                                      prior_package.change_datetime.ToString();

                }


                next_action_sequence_number =
                    coil_audit_trail.GetNextActionSequenceNumber(viewModel.current_all_produced_coil
                        .production_coil_no);
                coil_audit_trail.AddAuditRecord("Packaged", viewModel.current_all_produced_coil.production_coil_no,
                    Convert.ToInt32(viewModel.current_all_produced_coil.cons_coil_no), TypeOfPackaging,
                    current_application_security.user_id, next_action_sequence_number);

                viewModel.Message = production_coil_no + " marked as packaged";

            }

            if (TypeOfPackaging == "line_package")
            {
                viewModel.coils_to_package = vw_sw_packaging.GetCoilsToBeLinePackaged(facility_cd);
            }
            else if (TypeOfPackaging == "further_package")
            {
                viewModel.coils_to_package = vw_sw_packaging.GetCoilsToBeFurtherPackaged(facility_cd);
            }

            return View(viewModel);
        }

        public ActionResult PrintLabel(string printer_path, string production_coil_no, int? template_cd)
        {
            string ls_loc = Properties.Settings.Default.RegLocation;

            CoilModel viewModel = new CoilModel();
            viewModel.current_all_produced_coil = all_produced_coils.GetAllProducedCoil(production_coil_no);

            viewModel.available_printers = printer.GetAllZebraPrinters();
            viewModel.zebra_templates = zebra_template.GetAllCoilTemplates();

            viewModel.searched_coil_number = production_coil_no;



            viewModel.selected_template = zebra_template.GetTemplate(Convert.ToInt32(template_cd));

            if (printer_path != "" && printer_path != null && production_coil_no != "" && production_coil_no != null)
            {
                viewModel.current_all_produced_coil =
                    all_produced_coils.GetAllProducedCoilByProductionCoilNumberOrTagNumber(production_coil_no);


                if (viewModel.current_all_produced_coil == null)
                {
                    viewModel.Error = production_coil_no + " can not be found in L3, unable to print.";
                }
                else
                {

                    //if sinton and coil is consumed, print new coil tag.
                    if (application_settings.GetSetting("PrintConsumedTags") != null && application_settings.GetSetting("PrintConsumedTags").default_value == "N" &&
                        IsCoilConsumed(production_coil_no, viewModel))
                    {
                        //get coil number of processed coil (coil last indiicator == "Y")
                        int cons_coil = Int32.Parse(production_coil_no.Substring(3, 6));
                        viewModel.current_all_produced_coil =
                            all_produced_coils.GetAllProducedCoilLastProduced(cons_coil);
                        int count = all_produced_coils.GetCountOfDaughterCoils(cons_coil);

                        string new_coil_number = viewModel.current_all_produced_coil.production_coil_no;
                        viewModel.Error =
                            $"{production_coil_no} has been consumed!{Environment.NewLine}Printing coil tag for {new_coil_number}.";
                        if (count > 1)
                        {
                            var sisters = all_produced_coils.GetSisterCoils(cons_coil);
                            viewModel.Error += $"{Environment.NewLine} There are {count} daughter coils from this coil";
                            sisters.ForEach(x => viewModel.Error += ", " + x.production_coil_no);
                        }

                        production_coil_no = new_coil_number;
                    }

                    viewModel.selected_printer_path = printer_path;

                    printer current_printer = printer.GetPrinterByPath(printer_path);
                    //get template
                    v_zebra_template_coil current_template =
                        v_zebra_template_coil.GetCoilTemplate(production_coil_no, Convert.ToInt32(template_cd));

                    // Check if is Ship Tag No Order
                    var isTagNoOrder = viewModel.selected_template.name == "Ship Tag No Order";
                    if (isTagNoOrder)
                    {
                        current_template.template = usp.GetNoOrderCoilTag(production_coil_no);
                    }

                    //get template if coil number exists
                    if (current_template == null)
                    {
                        viewModel.Error = "Unable to print label for " + production_coil_no +
                                          ", check coil number and try again";
                        return View(viewModel);
                    }

                    try
                    {
                        //If Techs use TCP to zebra
                        if (ls_loc == "T")
                        {
                            if (current_printer.description.Contains("300") &&
                                !current_template.name.Contains("300"))
                            {
                                current_template.template = ZPLUtils.ScaleZPL(current_template.template, null);
                            }

                            Utils.TCPTemplateToZebra(current_printer, current_template.template);

                        }
                        else
                            Utils.FTPTemplateToZebra(current_printer, current_template.template, "barcode");

                        //create audit log record
                        int next_action_sequence_number = 1;

                        application_security current_application_security =
                            (application_security)System.Web.HttpContext.Current.Session["application_security"];

                        if (viewModel.current_all_produced_coil != null)
                        {
                            next_action_sequence_number =
                                coil_audit_trail.GetNextActionSequenceNumber(viewModel.current_all_produced_coil
                                    .production_coil_no);
                            coil_audit_trail.AddAuditRecord("New Label Printed",
                                viewModel.current_all_produced_coil.production_coil_no,
                                Convert.ToInt32(viewModel.current_all_produced_coil.cons_coil_no),
                                current_printer.description, current_application_security.user_id,
                                next_action_sequence_number);
                        }

                        viewModel.Message = "Label for " + production_coil_no + " printed at " +
                                            current_printer.description;
                    }
                    catch (DbUpdateException)
                    {
                        // This is an error with entity framework:
                        // Unable to update the EntitySet 'v_zebra_template_coil' because it has a DefiningQuery and no <UpdateFunction>
                        // element exists in the <ModificationFunctionMapping> element to support the current operation.
                        viewModel.Message = "Label for " + production_coil_no + " printed at " +
                                            current_printer.description;
                    }
                    catch
                    {
                        viewModel.Error = "Error sending label to printer, check printer status.";
                    }
                }
            }

            return View(viewModel);
        }

        public ActionResult Schedule(string facility_cd, string Message, string schedule_number)
        {
            CoilModel viewModel = new CoilModel();
            viewModel.Message = Message;
            viewModel.current_facility_cd = facility_cd;

            viewModel.schedule_product_processors = product_processors.GetProductProcessorsWithEntryLocation();
            viewModel.scheduled_coils = vw_sw_scheduled_coils.GetScheduledCoilsByFacility(facility_cd);

            //if schedule list is one with a schedule number, create filter
            if (viewModel.scheduled_coils.Where(x => x.schedule_number != null).Count() > 0)
            {
                viewModel.schedules = viewModel.scheduled_coils.Select(m => m.schedule_number).Distinct().ToList();
            }

            if (schedule_number != null && schedule_number != "")
            {
                viewModel.selected_schedule = schedule_number;
                viewModel.scheduled_coils =
                    viewModel.scheduled_coils.Where(x => x.schedule_number == schedule_number).ToList();
            }

            return View(viewModel);
        }

        public ActionResult MoveCoilToLineEntry(string[] production_coil_no, string facility_cd, string schedule_number)
        {
            string log_message = "";
            product_processors selected_processor = new product_processors();

            if (facility_cd == "R")
            {
                selected_processor = new product_processors()
                {
                    inside_or_outside = "I",
                    name = "SDI - COL Rail Staging",
                    facility_cd = "R",
                    entry_coil_yard_column = "HBS",
                    entry_coil_yard_row = "RS1"

                };
            }
            else
            {
                selected_processor = product_processors.GetInsideProductProcessorFromFacilityCD(facility_cd);
            }

            foreach (string coil in production_coil_no)
            {
                if (log_message == "")
                {

                    log_message = "The following coils were moved to " + selected_processor.entry_coil_yard_column +
                                  " - " + selected_processor.entry_coil_yard_row + ": " + coil;

                }
                else
                {

                    log_message = log_message + ", " + coil;

                }

                coil_yard_locations current_coil_yard_location = coil_yard_locations.GetCoilYardLocation(coil);

                application_security current_application_security =
                    (application_security)System.Web.HttpContext.Current.Session["application_security"];

                //delete if exists to reinsert later
                if (current_coil_yard_location != null)
                {
                    coil_yard_locations.DeleteCoilYardLocation(coil);
                }

                coil_yard_locations new_coil_yard_location = coil_yard_locations.InsertCoilYardLocation(coil,
                    selected_processor.entry_coil_yard_column, selected_processor.entry_coil_yard_row,
                    current_application_security.user_id, "C");

            }

            return RedirectToAction("Schedule", "Coil",
                new { facility_cd = facility_cd, Message = log_message, schedule_number = schedule_number });
        }

        public ActionResult AddComment(string production_coil_no, string comment)
        {
            CoilModel viewModel = new CoilModel();

            viewModel.current_all_produced_coil =
                all_produced_coils.GetAllProducedCoilByProductionCoilNumberOrTagNumber(production_coil_no);

            if (comment != "")
            {

                Utils.AddAuditRecord(viewModel.current_all_produced_coil.production_coil_no, "Comment", comment);
            }
            else
            {
                viewModel.Error = "Comment can not be blank";
            }

            return RedirectToAction("Index", "Coil",
                new { production_coil_no = production_coil_no, Error = viewModel.Error });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadCoilImage(string production_coil_no, HttpPostedFileBase upload_file)
        {
            CoilModel viewModel = new CoilModel();

            if (upload_file == null)
            {
                viewModel.Error = "File is empty, use the back button and click the browse button to select an image";
            }
            else
            {
                try
                {
                    viewModel.current_all_produced_coil =
                        all_produced_coils.GetAllProducedCoilByProductionCoilNumberOrTagNumber(production_coil_no);

                    byte[] documentBytes = new byte[upload_file.InputStream.Length];

                    upload_file.InputStream.Read(documentBytes, 0, documentBytes.Length);

                    usp.usp_add_coil_image(production_coil_no, documentBytes);

                    Utils.AddAuditRecord(viewModel.current_all_produced_coil.production_coil_no, "Image Uploaded",
                        null);

                    viewModel.Message = "Image uploaded for " + viewModel.current_all_produced_coil.production_coil_no;

                }
                catch (Exception ex)
                {
                    viewModel.Error = ex.ToString();
                }
            }

            return RedirectToAction("Index", "Coil",
                new { production_coil_no = production_coil_no, Message = viewModel.Message, Error = viewModel.Error });
        }

        public ActionResult HoldCoil(string production_coil_no, string comment, int claim_reason_cd)
        {
            CoilModel viewModel = new CoilModel();

            claim_reason selected_claim_reason = claim_reason.GetClaimReason(claim_reason_cd);

            viewModel.current_all_produced_coil =
                all_produced_coils.GetAllProducedCoilByProductionCoilNumberOrTagNumber(production_coil_no);

            //make sure record exists
            if (viewModel.current_all_produced_coil != null)
            {

                if (viewModel.current_all_produced_coil.coil_last_facility_ind == "Y")
                {

                    viewModel.current_coil =
                        coil.GetCoilByProductionCoilNumber(viewModel.current_all_produced_coil.production_coil_no);

                    //add defect for coil
                    if (viewModel.current_coil != null)
                    {
                        Utils.AddCoilDefect(viewModel.current_all_produced_coil.production_coil_no,
                            selected_claim_reason.defect_cd, viewModel.current_all_produced_coil.facility_cd,
                            viewModel.current_all_produced_coil.facility_cd, comment,
                            viewModel.current_coil.coil_status, viewModel.current_coil.order_no,
                            viewModel.current_coil.line_item_no, viewModel.current_all_produced_coil.cons_coil_no, "O",
                            "Y");

                        string new_coil_status = "HS";

                        if (selected_claim_reason.defect_cd == "140") //Lost Coil
                        {
                            new_coil_status = "HL"; //Hold - Lost Coil
                            coil_yard_locations.DeleteCoilYardLocation(viewModel.current_all_produced_coil
                                .production_coil_no); // free up location
                        }

                        if (viewModel.current_coil.coil_status != new_coil_status)
                        {
                            int result = Utils.ChangeCoilStatus(viewModel.current_all_produced_coil.production_coil_no,
                                "O", "O", viewModel.current_coil.coil_status, new_coil_status);

                            if (result == 1)
                            {
                                viewModel.Message = viewModel.current_all_produced_coil.production_coil_no +
                                                    " status updated to " + new_coil_status + ". " +
                                                    selected_claim_reason.description + " defect added.";

                            }
                            else
                            {
                                viewModel.Error =
                                    "Unable to update coil status, please refresh the page and try again.";
                            }
                        }

                    }
                    //add defect for inventory item
                    else
                    {
                        viewModel.current_inventory_item =
                            inventory_item.GetInventoryItemByProductionCoilNumber(viewModel.current_all_produced_coil
                                .production_coil_no);

                        Utils.AddCoilDefect(viewModel.current_all_produced_coil.production_coil_no,
                            selected_claim_reason.defect_cd, viewModel.current_all_produced_coil.facility_cd,
                            viewModel.current_all_produced_coil.facility_cd, comment,
                            viewModel.current_inventory_item.inventory_reason_cd, null, null,
                            viewModel.current_all_produced_coil.cons_coil_no, "I", "N");
                        viewModel.Message = "Defect added for inventory coil " +
                                            viewModel.current_inventory_item.production_coil_no;

                    }

                    application_security current_application_security =
                        (application_security)System.Web.HttpContext.Current.Session["application_security"];

                    //send alert email
                    ref_sys_param rsp = ref_sys_param.GetRefSysParam();

                    SmtpClient SmtpServer = new SmtpClient(rsp.mail_server);
                    SmtpServer.Timeout = 200000;

                    MailMessage myMail = new MailMessage();
                    myMail.IsBodyHtml = true;
                    myMail.From = new MailAddress("scanware@steeldynamics.com", "scanware");

                    //add all the email addresses
                    foreach (scanware_hold_coil_email email in scanware_hold_coil_email
                        .GetHoldEmailsByFacilityPlusDefault(viewModel.current_all_produced_coil.facility_cd))
                    {
                        myMail.To.Add(email.email_address);

                    }

                    myMail.Subject = "Coil Held in Scanware - " +
                                     viewModel.current_all_produced_coil.production_coil_no;
                    myMail.Body = "Coil Number: " + viewModel.current_all_produced_coil.production_coil_no +
                                  "<br/>Defect Description: " + selected_claim_reason.description + "<br/>Comment: " +
                                  comment + "<br/>User Name: " + current_application_security.user_name;

                    //send email
                    SmtpServer.Send(myMail);


                    //audit record in scanware
                    Utils.AddAuditRecord(viewModel.current_all_produced_coil.production_coil_no, "Coil Held",
                        selected_claim_reason.description + " - " + comment);


                } //end last facility check
                else
                {
                    viewModel.Error = production_coil_no + " has been consumed. Please hold child material.";
                }

            }
            else
            {
                viewModel.Error = "Unable to find " + production_coil_no + ". Notify IT";
            }


            return RedirectToAction("Index", "Coil",
                new { production_coil_no = production_coil_no, Message = viewModel.Message, Error = viewModel.Error });
        }

        public ActionResult ViewCoilImage(int image_no)
        {
            CoilModel viewModel = new CoilModel();

            viewModel.coil_image = v_apc_images.GetCoilImage(image_no);
            viewModel.current_all_produced_coil =
                all_produced_coils.GetAllProducedCoilByProductionCoilNumberOrTagNumber(viewModel.coil_image
                    .production_coil_no);

            return View(viewModel);
        }

        public ActionResult DeleteCoilYard(string Message, string Error)
        {

            CoilModel viewModel = new CoilModel();
            viewModel.Message = Message;
            viewModel.Error = Error;
            return View(viewModel);

        }

        [HttpGet]
        public ActionResult ViewGalvSchedule(byte galv_line)
        {
            CoilModel viewModel = new CoilModel();

            List<sdi_galv_sched_sp_model> viewModelGalv = new List<sdi_galv_sched_sp_model>();

            //byte galv_line = 1;
            try
            {
                sdipdbEntities db = ContextHelper.SDIPDBContext;

                viewModelGalv = db.Database.SqlQuery<sdi_galv_sched_sp_model>(
                    "EXEC sdi_galv_sched @galvanizing_line", new SqlParameter("galvanizing_line", galv_line)).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }

            viewModel.GalvSched = viewModelGalv;
            return View(viewModel);
        }

        //Reversing Mill Schedule (427)
        public ActionResult ViewRevMillSchedule()
        {
            CoilModel viewModel = new CoilModel();

            List<sdi_revmill_sched_sp_model> viewModelRevMill = new List<sdi_revmill_sched_sp_model>();

            try
            {
                sdipdbEntities db = ContextHelper.SDIPDBContext;

                viewModelRevMill = db.Database.SqlQuery<sdi_revmill_sched_sp_model>("EXEC sdi_cold_rolled_sched")
                    .ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }

            viewModel.sdi_revmill_sched_Results = viewModelRevMill;
            return View(viewModel);
        }

        //Temper Mill Schedule (426)
        public ActionResult ViewTempMillSchedule()
        {
            CoilModel viewModel = new CoilModel();

            List<sdi_tempmill_sched_sp_model> viewModelTempMill = new List<sdi_tempmill_sched_sp_model>();

            try
            {
                sdipdbEntities db = ContextHelper.SDIPDBContext;

                viewModelTempMill = db.Database.SqlQuery<sdi_tempmill_sched_sp_model>("EXEC sdi_temper_mill_sched")
                    .ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }

            viewModel.sdi_tempmill_sched_Results = viewModelTempMill;
            return View(viewModel);
        }

        public ActionResult DeleteCoilYardSubmit(string production_coil_nos, string comment)
        {
            CoilModel viewModel = new CoilModel();
            viewModel.Message = "";
            viewModel.Error = "";

            application_security current_application_security =
                (application_security)System.Web.HttpContext.Current.Session["application_security"];

            function_level_security logged_in_user = new function_level_security(
                System.Web.HttpContext.Current.Session["function_level_security"] as List<function_level_security>);

            if (!logged_in_user.HasFunctionLevelSecurity("COILYARDDELETE"))
            {
                return RedirectToAction("DeleteCoilYard", "Coil",
                    new { Error = "You do not have the proper permissions to access this page." });
            }

            string[] coilNumbers = null;
            coilNumbers = production_coil_nos.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            foreach (string production_coil_no in coilNumbers)
            {

                if (production_coil_no != "")
                {

                    all_produced_coils current_all_produced_coil =
                        all_produced_coils.GetAllProducedCoilByProductionCoilNumberOrTagNumber(production_coil_no);

                    //check if coil exists
                    if (current_all_produced_coil != null)
                    {

                        coil_yard_locations current_coil_yard_location =
                            coil_yard_locations.GetCoilYardLocation(current_all_produced_coil.production_coil_no);

                        if (current_coil_yard_location != null)
                        {
                            coil_yard_locations.DeleteCoilYardLocation(current_all_produced_coil.production_coil_no);
                            Utils.AddAuditRecord(current_all_produced_coil.production_coil_no, "Delete Coil Yard",
                                comment);
                            viewModel.Message = viewModel.Message + production_coil_no + " - success. ";
                        }
                        else
                        {
                            viewModel.Error = viewModel.Error + production_coil_no + "- no location.";
                        }


                    }
                    else
                    {
                        viewModel.Error = viewModel.Error + production_coil_no + "- not found. ";
                    }

                }


            }

            return View(viewModel);
        }

        public ActionResult ToggleHotStatus(string production_coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            int user_id = user_info.GetUserInfo(User.Identity.Name).application_security.Where(x => x.app_name == "Scanware").First().user_id;
            function_level_security fls = function_level_security.GetUserFunctionLevelSecurity(user_id, "Scanware").Where(x => x.function_name == "SHIPPING-HOT").FirstOrDefault();

            if (fls != null)
            {
                all_produced_coils current_coil = db.all_produced_coils.SingleOrDefault(x => x.production_coil_no == production_coil_no);

                if (current_coil.is_hot == null)
                {
                    current_coil.is_hot = 1;
                }
                else
                {
                    current_coil.is_hot = null;
                }
                db.SaveChanges();
            }

            return Redirect("/Shipping/ShipOPCoils");
        }

    }
}
