using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Scanware.Data;
using Scanware.Models;
using Scanware.Ancestor;
using Microsoft.Win32;
using System.Diagnostics;

namespace Scanware.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            return Redirect("/Coil/Index");
            //Response.Redirect("/Home/About");
            //return View();
        }

        [AllowAnonymous]
        public ActionResult Reports()
        {
            //HomeModel model = new HomeModel();
            //ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            //Response.Redirect("/Home/About");
            return View();
        }

        public ActionResult About()
        {
            HomeModel model = new HomeModel();

            model.user_info = user_info.GetUserInfo(User.Identity.Name);
            model.IPAddress = Request.UserHostName;
            try
            {
                model.ComputerName = Util.DetermineCompName(model.IPAddress);
            }
            catch { }

            model.Application = Properties.Settings.Default.AppName;
            model.ApplicationVersion = Properties.Settings.Default.AppVersion;
            model.CurrentApplication = application.GetApplication(Properties.Settings.Default.AppName);

            if (Debugger.IsAttached)
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\SDI\Company_Preferences");

                model.Location = key.GetValue("Location").ToString();
                model.Database = key.GetValue("LiveTest").ToString();
            }
            else
            {
                model.Location = Properties.Settings.Default.RegLocation;
                model.Database = "L";
            }

            return View(model);
        }

        public ActionResult SetUserOptions(string zebra, string network, string from_freight_location, string[] selected_bays, int? updated, string initials, bool? israiluser, string defaultZincLine)
        {

            HomeModel model = new HomeModel();

            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

            model.zebra_printers = printer.GetAllPrinters().Where(x=>x.type=="Zebra").OrderBy(y => y.description).ToList();
            model.network_printers = printer.GetAllPrinters().Where(x => x.type == "Network").OrderBy(y => y.description).ToList();
            model.from_freight_locations = from_freight_locations.GetFromFreightLocatins();
            model.CoilYardBays = coil_yard_bays.GetAllCoilYardBays();

            user_defaults user_initials = user_defaults.GetUserDefaultByName(current_application_security.user_id, "UserInitials");

            if (user_initials != null)
            {
                model.user_initials = user_initials.value;
            }

            user_defaults isRailUser = user_defaults.GetUserDefaultByName(current_application_security.user_id, "isRailUser");

            if (isRailUser == null || isRailUser.value == "NO" )
            {
                model.isRailUser = false; 
            }
            else if (isRailUser.value == "YES")
            {
                model.isRailUser = true;
            }

            //User Default Bay Codes
            string session_bay_cd = "";

            user_defaults default_bays = user_defaults.GetUserDefaultByName(current_application_security.user_id, "CoilYardBays");
            string bays_selected = "";

            //Default values from the DB
            if (default_bays != null)
            {
                model.selected_bays = default_bays.value.Split(',');
            }
            //Pressed on the 'Select Bay'
            if (selected_bays != null && selected_bays[0] == "")
                {
                    selected_bays = null;
                }
            
            //Values selected by user
            if (selected_bays != null && selected_bays.Count()>0)
            {
                int cnt = selected_bays.Count();

                for (int x = 0; x < cnt; x++)
                {
                    session_bay_cd += selected_bays[x];
                    if (x < (cnt-1))
                    {
                        session_bay_cd += ",";
                    }
                }

                model.selected_bays = selected_bays;

                coil_yard_bays bay;
                int i = 1;

                foreach (string bay_cd in model.selected_bays)
                {
                    bay = coil_yard_bays.GetCoilYardBay(bay_cd);

                    if (bay != null)
                    {
                        bays_selected += bay.bay_desc;
                        i++;
                    }

                    if (model.selected_bays.Length >= i)
                    {
                        bays_selected += ",";
                    }

                }
            }
            else if(updated == 1 && selected_bays == null)
            {
                model.selected_bays = null;
                user_defaults.DeleteDefault(current_application_security.user_id, "CoilYardBays", session_bay_cd);
            }

            if (model.selected_bays != null && model.selected_bays.Count() > 0 && updated == 1)
            {
 
                if (default_bays == null)
                {
                    //insert new user default record
                    user_defaults.InsertDefault(current_application_security.user_id, "CoilYardBays", session_bay_cd);
                }
                else
                {
                    //update new user default record
                    if (selected_bays != null)
                    {
                        user_defaults.UpdateDefaultByName(current_application_security.user_id, "CoilYardBays", session_bay_cd);
                    }
                    else
                    {
                        user_defaults.UpdateDefaultByName(current_application_security.user_id, "CoilYardBays", default_bays.value);
                    }
                   
                }
            }

            //Zebra printer logic
            //get current default if exists
            user_defaults default_printer = user_defaults.GetUserDefaultByName(current_application_security.user_id, "ScanwareZebraPrinter");

            if (zebra != null && zebra != "")
            {
                if (default_printer == null)
                {
                    //insert new user default record
                    user_defaults.InsertDefault(current_application_security.user_id, "ScanwareZebraPrinter", zebra);
                }
                else
                {
                    //update new user default record
                    user_defaults.UpdateDefaultByName(current_application_security.user_id, "ScanwareZebraPrinter", zebra.ToString());
                }

                default_printer = user_defaults.GetUserDefaultByName(current_application_security.user_id, "ScanwareZebraPrinter");

            }

            //if default printer set
            if (default_printer != null)
            {
                model.selected_zebra_printer = printer.GetPrinterByPK(Convert.ToInt32(default_printer.value));

                if (zebra != null && zebra != "")
                {
                    model.Message = "Default Zebra Printer set to " + model.selected_zebra_printer.description + ". ";
                }

            }

            //Network printer logic
            //get current default if exists
            default_printer = null;

            default_printer = user_defaults.GetUserDefaultByName(current_application_security.user_id, "ScanwareNetworkPrinter");

            if (network != null && network !="")
            {
                if (default_printer == null)
                {
                    //insert new user default record
                    user_defaults.InsertDefault(current_application_security.user_id, "ScanwareNetworkPrinter", network);
                }
                else
                {
                    //update new user default record
                    user_defaults.UpdateDefaultByName(current_application_security.user_id, "ScanwareNetworkPrinter", network.ToString());
                }

                default_printer = user_defaults.GetUserDefaultByName(current_application_security.user_id, "ScanwareNetworkPrinter");

            }

            //if default printer set
            if (default_printer != null)
            {
                model.selected_network_printer = printer.GetPrinterByPK(Convert.ToInt32(default_printer.value));

                if (network != null && network!="")
                {
                    model.Message = model.Message + "Default Network Printer set to " + model.selected_network_printer.description + ".";
                }

            }

            //From Freight Location Logic
            //get current default if exists
            user_defaults default_from_freight_location = user_defaults.GetUserDefaultByName(current_application_security.user_id, "FromFreightLocation");

            if (from_freight_location != null && from_freight_location != "")
            {
                if (default_from_freight_location == null)
                {
                    //insert new user default record
                    user_defaults.InsertDefault(current_application_security.user_id, "FromFreightLocation", from_freight_location.ToString());
                }
                else
                {
                    //update new user default record
                    user_defaults.UpdateDefaultByName(current_application_security.user_id, "FromFreightLocation", from_freight_location.ToString());
                }

                default_from_freight_location = user_defaults.GetUserDefaultByName(current_application_security.user_id, "FromFreightLocation");

            }

            //if default from loc
            if (default_from_freight_location != null)
            {
                model.selected_from_freight_locations = from_freight_locations.GetLocationByCD(Convert.ToInt32(default_from_freight_location.value));

                if (from_freight_location != null && from_freight_location != "")
                {
                    model.Message = model.Message + "From Freight Location set to " + model.selected_from_freight_locations.from_frt_description + ". ";
                }

            }

            user_defaults default_bays_check = user_defaults.GetUserDefaultByName(current_application_security.user_id, "CoilYardBays");
            if (default_bays_check != null && default_bays_check.value != "" && bays_selected.Length > 0 && selected_bays!= null)
            {

                model.Message = model.Message + " Default Coil Yard Bay set to " + bays_selected + ". ";
            }

            //if default initials set
            if (user_initials != null && initials != null)
            {
                if (initials == "" && updated == 1)
                {
                    //delete user default record for initials
                    model.user_initials = null;
                    user_defaults.DeleteDefault(current_application_security.user_id, "UserInitials", initials);
                }
                else
                {
                    //update new user default record
                    user_defaults.UpdateDefaultByName(current_application_security.user_id, "UserInitials", initials);

                    model.Message = model.Message + "User Initials set to " + initials;
                    model.user_initials = initials;
                }
            }
            else if (user_initials == null && initials != null && initials.Length > 0 )
            {
                //insert new user default record
                user_defaults.InsertDefault(current_application_security.user_id, "UserInitials", initials);
                model.Message = model.Message + "User Initials set to " + initials;
                model.user_initials = initials;
            }
            else if (initials == "" && updated == 1)
            {
                //delete user default record for initials
                model.user_initials = null;
                user_defaults.DeleteDefault(current_application_security.user_id, "UserInitials", initials);
            }

            if ((israiluser == null || israiluser == false) && (updated == 1))
            {
                model.isRailUser = false;

                if (isRailUser == null)
                {
                    user_defaults.InsertDefault(current_application_security.user_id, "isRailUser", "NO");
                }
                else
                {
                    user_defaults.UpdateDefaultByName(current_application_security.user_id, "isRailUser", "NO");
                }

                model.Message = model.Message + " Default Rail User set to 'NO'.";
            }
            else if (israiluser == true && updated == 1)
            {
                model.isRailUser = true;

                if (isRailUser == null)
                {
                    user_defaults.InsertDefault(current_application_security.user_id, "isRailUser", "YES");
                }
                else
                { 
                    user_defaults.UpdateDefaultByName(current_application_security.user_id, "isRailUser", "YES");
                }
                model.Message = model.Message + " Default Rail User set to 'YES'.";
            }
            // Default Zinc Line
            user_defaults default_zinc = user_defaults.GetUserDefaultByName(current_application_security.user_id, "defaultZincLine");

            if (defaultZincLine != null && defaultZincLine != "" && updated == 1)
            {
                if (default_zinc == null)
                {
                    //insert new user default record
                    user_defaults.InsertDefault(current_application_security.user_id, "defaultZincLine", defaultZincLine);
                }
                else
                {
                    //update new user default record
                    user_defaults.UpdateDefaultByName(current_application_security.user_id, "defaultZincLine", defaultZincLine);
                }
                model.Message = model.Message + " Default Zinc Consume line set to " + defaultZincLine;
            }

            if (defaultZincLine != null)
            { 
                model.default_zinc_line = defaultZincLine;
            }
            else if (default_zinc != null)
            {
                model.default_zinc_line = default_zinc.value;
            }

            return View(model);

        }

        public void ChangeLocation(string location, string live_test)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\SDI\Company_Preferences", true);
            key.SetValue("Location", location, RegistryValueKind.String);
            key.SetValue("LiveTest", live_test, RegistryValueKind.String);
            key.Close();

            //Util.GetRegLocation2(); - When ready to use FrgGenericUser
            Util.GetRegLocation();

            Response.Redirect("/Home/About");
        }

    }
}
