using System.Web;
using System.Web.Mvc;
using Scanware.Data;
using Scanware.Models;
using System;
using System.Globalization;

namespace Scanware.Controllers
{
    public class ZincController : Controller
    {

        public ActionResult IngotStatus(string ingot_id, string Message, string Error)
        {
            ZincModel viewModel = new ZincModel();

            viewModel.Error = Error;
            viewModel.Message = Message;
           

            if (ingot_id != null && ingot_id != "")
            {
                viewModel.current_ingot = zinc_tracking.GetIngot(ingot_id);
                viewModel.searched_ingot_id = ingot_id;

                if(viewModel.current_ingot == null)
                { 
                    viewModel.Error = "Unable to find " + ingot_id + " in L3";
                    viewModel.current_ingot = new zinc_tracking();
                    return View(viewModel);
                }

                if(viewModel.current_ingot.add_user_id != null)
                { 
                     viewModel.current_ingot_add_user = application_security.GetApplicationSecurity(viewModel.current_ingot.add_user_id ?? default(int)).user_name;
                }

                if (viewModel.current_ingot.consumed_user_id != null)
                {
                    viewModel.current_ingot_consume_user = application_security.GetApplicationSecurity(viewModel.current_ingot.consumed_user_id ?? default(int)).user_name;
                }
                

            }

            return View(viewModel);
        }

        public ActionResult ZincInventory(string ingot_id, string Message, string Error)
        {
            ZincModel viewModel = new ZincModel();

            viewModel.Error = Error;
            viewModel.Message = Message;

            viewModel.zincInventory = zinc_tracking.GetInventory();

            return View(viewModel);
        }

        public string UpdateUserInLocation(string IngotNumber, string isScannerUsed)
        {
            ZincModel viewModel = new ZincModel();
            string error = "";

            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

            if (IngotNumber != "" && IngotNumber != null)
            {
                viewModel.current_ingot = zinc_tracking.GetIngot(IngotNumber);

                //check if Ingot exists
                if (viewModel.current_ingot == null)
                {
                    error = "unable to find " + IngotNumber + " in l3";

                    return error;
                }

               viewModel.current_ingot.UpdateUser(viewModel.current_ingot);
            }

            return "";

        }
        public ActionResult ConsumeIngot(string ingot_id, string Message, string Error, string line)
        {
            ZincModel viewModel = new ZincModel();

            viewModel.Error = Error;
            viewModel.Message = Message;

            viewModel.consumed_ingot_id = "";

            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

            // Default Zinc Line
            user_defaults default_zinc = user_defaults.GetUserDefaultByName(current_application_security.user_id, "defaultZincLine");

            if (default_zinc != null)
            {
                viewModel.default_zinc_line = default_zinc.value;
            }

            if (ingot_id != null && ingot_id != "")
            {
                viewModel.current_ingot = zinc_tracking.GetIngot(ingot_id);

                if (viewModel.current_ingot == null)
                {
                    viewModel.Error = "Unable to find " + ingot_id + " in L3";
                    viewModel.current_ingot = new zinc_tracking();
                    return View(viewModel);
                }

                // Verify that Ingot wasn't added in the past 4 days and that status is Inventory - Cannot consume otherwise
                if (viewModel.current_ingot.status_cd != "I") 
                {
                    viewModel.Error = "Cannot Consume this Ingot - Status InValid";
                    viewModel.current_ingot = new zinc_tracking();
                    return View(viewModel);
                }

                //If no line selected
                if (line == null || line == "")
                {
                    viewModel.Error = "Cannot Consume this Ingot - No Line selected";
                    viewModel.current_ingot = new zinc_tracking();
                    return View(viewModel);
                }

                DateTime nowDT = DateTime.Now;
                DateTime PrevDT = nowDT.AddDays(-4);

                if (viewModel.current_ingot.add_datetime > PrevDT)
                {
                    viewModel.Error = "Cannot Consume this Ingot - It was added less than 4 days ago";
                    viewModel.current_ingot = new zinc_tracking();
                    return View(viewModel);
                }
               
                zinc_tracking.ConsumeIngot(ingot_id, "C", line, current_application_security.user_id, DateTime.Now);
                viewModel.consumed_ingot_id = ingot_id;
            }

            return View(viewModel);
        }
        public string AddIngot(string IngotNumber, string isScannerUsed)
        {
            ZincModel viewModel = new ZincModel();

            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];
            string return_msg = null;

            if (IngotNumber != "" && IngotNumber != null)
            {
                viewModel.current_ingot = zinc_tracking.GetIngot(IngotNumber);

                //check if Ingot exists
                if (viewModel.current_ingot == null)
                {
                    return_msg = "unable to find " + IngotNumber + " in l3";

                    return return_msg;
                }
                return_msg = "SUCCESS";
                viewModel.current_ingot.AddToInventory(viewModel.current_ingot);
            }
            return return_msg;
        }
        public string RemoveIngot(string IngotNumber, string is_scanner_used)
        {
            string error;
            ZincModel viewModel = new ZincModel();

            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

            if (IngotNumber != "" && IngotNumber != null)
            {
                viewModel.current_ingot = zinc_tracking.GetIngot(IngotNumber);

                //check if Ingot exists
                if (viewModel.current_ingot == null)
                {
                    error = "unable to find " + IngotNumber + " in l3";

                    return error;
                }

            }
            //Mark Ingot as Consumed to remove from Inventory
            zinc_tracking.ConsumeIngot(IngotNumber, "C", "1", current_application_security.user_id, DateTime.Now);

            return "SUCCESS";
        }

    }
}
