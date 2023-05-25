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


namespace Scanware.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        public ActionResult LocationReconcile(string column, string row, string bay_cd)
        {
            InventoryModel viewModel = new InventoryModel();

            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

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
            user_defaults default_from_freight_location_cd = user_defaults.GetUserDefaultByName(current_application_security.user_id, "FromFreightLocation");

            int from_freight_location_cd = 77;

            if (default_from_freight_location_cd != null)
            {
                from_freight_location_cd = Convert.ToInt32(default_from_freight_location_cd.value);
            }

            if (session_bay_cd != "")
            {
                viewModel.CoilYardCols = coil_yard_columns.GetAllCoilYardColumns(from_freight_location_cd, session_bay_cd);
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

            return View(viewModel);
        }

        public ActionResult CoilsInLocation(string column, string row, string bay_cd, string scanned_location)
        {
            InventoryModel viewModel = new InventoryModel();

            if (scanned_location != "")
            {
                string[] loc = scanned_location.Split('$');
                if (loc.Length > 0)
                {
                    column = loc[1];
                    row = loc[2];
                }
      
            }

            viewModel.coilsInLocation = coil_yard_locations.GetCoilsInLocation(column, row);

            viewModel.current_row = row;
            viewModel.current_column = column;
            viewModel.current_coil_yard_bay = new coil_yard_bays();
            viewModel.current_coil_yard_bay.bay_cd = bay_cd;

            return PartialView("CoilsInLocationPartial", viewModel);
        }

        public ActionResult submitLocation(string row, string column)
        {
            InventoryModel viewModel = new InventoryModel();

            viewModel.coilsInLocation = coil_yard_locations.GetCoilsInLocation(column, row);

            viewModel.current_row = row;
            viewModel.current_column = column;
            viewModel.current_coil_yard_bay = new coil_yard_bays();
            //viewModel.current_coil_yard_bay.bay_cd = bay_cd;

            return PartialView("CoilsInLocationPartial", viewModel);


        }

        public int AllCoilsValidated(string row, string column)
        {
           int auditSuccess = coil_yard_locations.CoilLocationAudit(column, row);

            return auditSuccess;
        }

        public string UpdateUserInLocation(string coilNumber, string row, string column, string isScannerUsed)
        {
            InventoryModel viewModel = new InventoryModel();
            string error = "";

            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

            if (coilNumber != "" && coilNumber != null && row != "")
            {
                viewModel.current_all_produced_coil = all_produced_coils.GetAllProducedCoilByProductionCoilNumberOrTagNumber(coilNumber);

                //check if coil exists
                if (viewModel.current_all_produced_coil == null)
                {
                    error = "Unable to find " + coilNumber + " in L3";

                    return error;
                }

                viewModel.current_coil = coil.GetCoilByProductionCoilNumber(viewModel.current_all_produced_coil.production_coil_no);

                try
                {
                    if (viewModel.current_coil.coil_status == "SC")
                    {
                        error = viewModel.current_all_produced_coil.production_coil_no + " has been shipped but has been moved successfully. Coil will show on mis-shipped report.";
                    }
                }
                catch { } //WILL ERROR IF THEY MOVE A CONSUMED COIL

                if (viewModel.current_all_produced_coil.coil_last_facility_ind == "N")
                {
                    error = viewModel.current_all_produced_coil.production_coil_no + " is consumed but has been moved successfully. Coil will show on consumed coil report.";
                }

                viewModel.current_coil_yard_location = coil_yard_locations.GetCoilYardLocation(viewModel.current_all_produced_coil.production_coil_no);


                ref_sys_param rsp = ref_sys_param.GetRefSysParam();

                //if butler update location if exists
                if (rsp.location != "C")
                {
                    //if exists then update
                    if (viewModel.current_coil_yard_location != null)
                    {
                        viewModel.new_coil_yard_location = coil_yard_locations.UpdateCoilYardLocation(viewModel.current_all_produced_coil.production_coil_no, column, row, current_application_security.user_id, isScannerUsed);
                    }
                    else
                    {
                        viewModel.new_coil_yard_location = coil_yard_locations.InsertCoilYardLocation(viewModel.current_all_produced_coil.production_coil_no, column, row, current_application_security.user_id, isScannerUsed);
                    }

                    Utils.AddAuditRecord2(viewModel.current_all_produced_coil.production_coil_no, "Inventory - Scanware Coil Move", "Moved using Scanware.", isScannerUsed, column, row);
                }
                else //for columbus lomas triggering delete than add
                {

                    //delete if exists to reinsert later
                    if (viewModel.current_coil_yard_location != null)
                    {
                        coil_yard_locations.DeleteCoilYardLocation(viewModel.current_all_produced_coil.production_coil_no);
                    }

                    viewModel.new_coil_yard_location = coil_yard_locations.InsertCoilYardLocation(viewModel.current_all_produced_coil.production_coil_no, column, row, current_application_security.user_id, isScannerUsed);

                    usp.usp_sw_update_status_after_move(viewModel.current_all_produced_coil.production_coil_no, current_application_security.user_id);
                }

            }

            return error;

        }
        public string RemoveCoilFromLocation(string coilNumber, string row, string column, string bay, string is_scanner_used)
        {
            InventoryModel viewModel = new InventoryModel();

            viewModel.coilsInLocation = coil_yard_locations.GetCoilsInLocation(column, row);

            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

                if (coilNumber != "")
                {

                    all_produced_coils current_all_produced_coil = all_produced_coils.GetAllProducedCoilByProductionCoilNumberOrTagNumber(coilNumber);

                    //check if coil exists
                    if (current_all_produced_coil != null)
                    {

                        coil_yard_locations current_coil_yard_location = coil_yard_locations.GetCoilYardLocation(current_all_produced_coil.production_coil_no);

                        if (current_coil_yard_location != null)
                        {
                            coil_yard_locations.DeleteCoilYardLocation(current_all_produced_coil.production_coil_no);

                        //If Warehouse Coil - set to Hold Lost
                        string status = warehoused_inventory.GetCoilStatusByProductionCoilNumber(coilNumber);

                        if(status != "NA" && status != "SC")
                        {
                            warehoused_inventory.UpdateCoilStatusByProductionCoilNumber(coilNumber, "HL", current_application_security.user_id);
                            Utils.AddAuditRecordWarehouse(current_all_produced_coil.production_coil_no, "Inventory - Delete Coil Yard", "Deleted using Scanware.", is_scanner_used);
                        }
                        else
                        {
                            Utils.AddAuditRecord2(current_all_produced_coil.production_coil_no, "Inventory - Delete Coil Yard", "Deleted using Scanware.", is_scanner_used, column, row);
                        }

                        viewModel.Message = viewModel.Message + coilNumber + " - success. ";
                             
                             //remove item from the list
                             CoilsInLocation itemToRemove = viewModel.coilsInLocation.SingleOrDefault(r => r.production_coil_no == coilNumber);

                            if (itemToRemove != null)
                            {
                                viewModel.coilsInLocation.Remove(itemToRemove);
                            }

                        }
                        else
                        {
                            viewModel.Error = viewModel.Error + coilNumber + "- no location.";
                        }


                    }
                    else
                    {
                        viewModel.Error = viewModel.Error + coilNumber + "- not found. ";
                    }

                }

            return viewModel.Error;

        }

    public string AddCoilToLocation(string coilNumber, string row, string column, string bay, string isScannerUsed)
        {
            InventoryModel viewModel = new InventoryModel();

            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

            viewModel.coilsInLocation = coil_yard_locations.GetCoilsInLocation(column, row);

            viewModel.Alert = "SUCCESS";
            if (coilNumber != null && coilNumber.Length > 0)
            {
                if (coilNumber[0].ToString().ToLower() == "s")
                {
                    coilNumber = coilNumber.Substring(1);
                }
            }

            if (coilNumber != "" && coilNumber != null && row != "")
            {
                viewModel.current_all_produced_coil = all_produced_coils.GetAllProducedCoilByProductionCoilNumberOrTagNumber(coilNumber);

                //check if coil exists
                if (viewModel.current_all_produced_coil == null)
                {
                    viewModel.Error = "Unable to find " + coilNumber + " in L3";
                    viewModel.current_all_produced_coil = new all_produced_coils();
                    viewModel.current_all_produced_coil.production_coil_no = coilNumber;
                    return viewModel.Error;
                }

                viewModel.current_coil = coil.GetCoilByProductionCoilNumber(viewModel.current_all_produced_coil.production_coil_no);

                string status = "";

                if (viewModel.current_coil == null)
                {
                    status = warehoused_inventory.GetCoilStatusByProductionCoilNumber(coilNumber);

                }

                try
                {
                    if (viewModel.current_coil != null && viewModel.current_coil.coil_status == "SC")
                    {
                        viewModel.Alert = viewModel.current_all_produced_coil.production_coil_no + " has been shipped but has been moved successfully. Coil will show on mis-shipped report.";
                    }
                    if (viewModel.current_coil == null && status == "SC")
                    {
                        viewModel.Alert = viewModel.current_all_produced_coil.production_coil_no + " has been shipped but has been moved successfully. Coil will show on mis-shipped report.";
                    }
                }
                catch { } //WILL ERROR IF THEY MOVE A CONSUMED COIL

                if (viewModel.current_all_produced_coil.coil_last_facility_ind == "N")
                {
                    viewModel.Alert = viewModel.current_all_produced_coil.production_coil_no + " is consumed but has been moved successfully. Coil will show on consumed coil report.";
                }

                viewModel.current_coil_yard_location = coil_yard_locations.GetCoilYardLocation(viewModel.current_all_produced_coil.production_coil_no);


                ref_sys_param rsp = ref_sys_param.GetRefSysParam();

                //if butler update location if exists
                if (rsp.location != "C")
                {
                    //if exists then update
                    if (viewModel.current_coil_yard_location != null)
                    {
                        viewModel.new_coil_yard_location = coil_yard_locations.UpdateCoilYardLocation(viewModel.current_all_produced_coil.production_coil_no, column, row, current_application_security.user_id, isScannerUsed);
                    }
                    else
                    {
                        viewModel.new_coil_yard_location = coil_yard_locations.InsertCoilYardLocation(viewModel.current_all_produced_coil.production_coil_no, column, row, current_application_security.user_id, isScannerUsed);
                    }
                    CoilsInLocation Newitem = new CoilsInLocation();
                    Newitem.production_coil_no = coilNumber;

                    viewModel.coilsInLocation.Add(Newitem);
                   
                }
                else //for columbus lomas triggering delete than add
                {

                    //delete if exists to reinsert later
                    if (viewModel.current_coil_yard_location != null)
                    {
                        coil_yard_locations.DeleteCoilYardLocation(viewModel.current_all_produced_coil.production_coil_no);
                    }

                    viewModel.new_coil_yard_location = coil_yard_locations.InsertCoilYardLocation(viewModel.current_all_produced_coil.production_coil_no, column, row, current_application_security.user_id, isScannerUsed);

                }

                usp.usp_sw_update_status_after_move(viewModel.current_all_produced_coil.production_coil_no, current_application_security.user_id);

                if (rsp.location != "C")
                {
                    Utils.AddAuditRecord2(viewModel.current_all_produced_coil.production_coil_no, "Inventory - Add Coil to Location", "Moved using Scanware.", isScannerUsed, column, row);
                }

            }

            return viewModel.Alert;
        }
        
    }
}

