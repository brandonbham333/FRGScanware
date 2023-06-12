using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class shipment_load
    {
        public string change_user_name
        {
            get
            {

                sdipdbEntities db = ContextHelper.SDIPDBContext;

                int change_user_id;

                if (this.change_user_id == null)
                {
                    change_user_id = -1;
                }
                else
                {
                    change_user_id = Convert.ToInt32(this.change_user_id);
                }

                application_security current_application_security = application_security.GetApplicationSecurity(change_user_id);


                if (current_application_security != null)
                {
                    return current_application_security.user_name;
                }
                else
                {
                    return "";
                }

            }
        }

        public string ship_to_location_name
        {
            get
            {

                sdipdbEntities db = ContextHelper.SDIPDBContext;

                customer_ship_to cst = db.customer_ship_to.Where(x => x.shiptoid == this.ToShipToID).FirstOrDefault();

                if (cst != null)
                {
                    return cst.ship_to_location_name;
                }
                else
                {
                    return "";
                }

            }
        }

        public string load_dtl_locations
        {
            get
            {

                sdipdbEntities db = ContextHelper.SDIPDBContext;

                string locations = "";

                int counter = 0;

                var shipment_loads_locations = (from ld in db.load_dtl
                                                join cyl in db.coil_yard_locations on ld.production_coil_no equals cyl.production_coil_no
                                                where ld.load_id == this.load_id
                                                select cyl).Distinct();

                foreach (coil_yard_locations cyl in shipment_loads_locations)
                {
                    if (counter != 0)
                    {
                        locations = locations + ", ";
                    }

                    counter = 1;

                    locations = locations + cyl.column + " - " + cyl.row;
                }

                return locations;



            }

        }

        public List<string> GetLoadYardBays(int load_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;
            List<load_dtl> coils = load_dtl.GetLoadDtl(load_id);
            List<string> bays = new List<string>();
            int next_seq;
            string bay;

            foreach (var coil in coils)
            {
                string coil_check = coil.production_coil_no;
                next_seq = coil_audit_trail.GetNextActionSequenceNumber(coil_check);
                next_seq--;

                List<vw_sw_coil_history> listcoils = coil_audit_trail.GetCoilHistoryByProductionCoilNumber(coil_check);
                var list2 = listcoils.FirstOrDefault(x => x.action_seq_no == next_seq) ?? new vw_sw_coil_history { coil_yard_column = "", coil_yard_row = "" };

                string loc_col = list2.coil_yard_column;
                string loc_row = list2.coil_yard_row;

                coil_yard_rows d = new coil_yard_rows();

                bay = d.GetYardBay(loc_col, loc_row);
                bays.Add(bay);
            }

            return bays;
        }

        public static List<shipment_load> GetTrucksOnSite()
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            string[] selected_bays = { }, load_bays;

            List<shipment_load> trucks_on_site = new List<shipment_load>();

            ref_sys_param rsp = ref_sys_param.GetRefSysParam();
            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];
            user_defaults default_from_freight_location_cd = user_defaults.GetUserDefaultByName(current_application_security.user_id, "FromFreightLocation");

            if (rsp.location != "C" && rsp.location != "T")
            {
                try
                {
                    application_settings app = application_settings.GetAppSetting("Ship_From_Scanware");

                    user_defaults default_bays = user_defaults.GetUserDefaultByName(current_application_security.user_id, "CoilYardBays");

                    //Default values from the DB
                    if (default_bays != null)
                    {
                        selected_bays = default_bays.value.Split(',');
                    }

                    //to sort by j'ville or Butler Trucks

                    int from_freight_location_cd = 77;
                    bool found;

                    if (default_from_freight_location_cd != null)
                    {
                        from_freight_location_cd = Convert.ToInt32(default_from_freight_location_cd.value);
                    }

                    if (from_freight_location_cd == 77)
                    {

                        List<shipment_load> shipment_loads2 = new List<shipment_load>();

                        if (app.default_value == "N")
                        {
                            var nested_loads = db.load_dtl_bay.Where(ld => ld.hold != "Y").Select(ld => ld.load_id);

                            shipment_loads2 = db.shipment_load.Where(sl => sl.carrier_mode == "T" && sl.scale_time_in != null && sl.load_id == sl.master_load_id
                           && sl.from_freight_location_cd == from_freight_location_cd && nested_loads.Contains(sl.load_id)).OrderBy(x => x.scale_time_in).ToList() ?? new List<shipment_load>();
                        }
                        else
                        {
                            shipment_loads2 = db.shipment_load.Where(sl => sl.carrier_mode == "T" && sl.shipped_date == null && sl.load_status == "PI" && sl.load_id == sl.master_load_id
                               && sl.from_freight_location_cd == from_freight_location_cd).OrderBy(x => x.scale_time_in).ToList() ?? new List<shipment_load>();
                        }

                        foreach (shipment_load shipment in shipment_loads2)
                        {
                            found = true;

                            //Filter loads by Coil Yard Bay - only display per User Defaults
                            if (default_bays != null)
                            {
                                load_bays = shipment.GetLoadYardBays(shipment.load_id).ToArray();
                                foreach (string x in load_bays)
                                {
                                    if (selected_bays.Contains(x))
                                    {
                                        found = true;
                                        break;
                                    }
                                    else
                                    {
                                        found = false;
                                    }

                                }
                            }

                            if (found)
                            {
                                trucks_on_site.Add(shipment);
                            }
                        }
                    }
                    else
                    {
                        var shipment_loads3 = from sl in db.shipment_load
                                              where sl.carrier_mode == "T" && sl.scale_time_in != null && sl.scale_time_out == null
                                              select sl;
                        foreach (shipment_load shipment in shipment_loads3.OrderBy(x => x.scale_time_in))
                        {
                            trucks_on_site.Add(shipment);
                        }
                    }
                }
                catch { return trucks_on_site; }
            }
            else if (rsp.location == "T")
            {
                if (default_from_freight_location_cd != null)
                {
                    short? user_freight_location = Convert.ToInt16(default_from_freight_location_cd.value);

                    trucks_on_site.AddRange(db.shipment_load.Where(
                        load => load.from_freight_location_cd == user_freight_location.Value &&
                                load.carrier_mode == "T" &&
                                load.scale_time_in != null &&
                                load.scale_time_out == null).OrderBy(x => x.scale_time_in).ToList());
                }
            }
            else
            {
                var shipment_loads = from sl in db.shipment_load
                                     where sl.carrier_mode == "T" && sl.scale_time_in != null && sl.scale_time_out == null
                                     select sl;

                foreach (shipment_load shipment in shipment_loads.OrderBy(x => x.scale_time_in))
                {
                    trucks_on_site.Add(shipment);
                }
            }

            return trucks_on_site;
        }

        public static List<shipment_load> GetOpenRailLoads()
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            List<shipment_load> open_rail_loads = new List<shipment_load>();

            ref_sys_param rsp = ref_sys_param.GetRefSysParam();

            if (rsp.location == "C")
            {

                var shipment_loads = from sl in db.shipment_load
                                     where (sl.carrier_mode == "R" || sl.carrier_mode == "B") && sl.shipped_date == null && sl.vehicle_no != null && sl.load_status != "SH" && (sl.char_load_id.Contains("-901") || !sl.char_load_id.Contains("-9"))
                                     select sl;

                foreach (shipment_load shipment in shipment_loads.OrderBy(x => x.loading_start_datetime))
                {
                    open_rail_loads.Add(shipment);
                }
            }
            else
            {
                application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];
                user_defaults default_rail_user = user_defaults.GetUserDefaultByName(current_application_security.user_id, "isRailUser");

                if (default_rail_user == null)
                {
                    default_rail_user = new user_defaults();
                }
                //Temp fix until shipping decide if we need this setting
                default_rail_user.value = "YES";

                //to sort by j'ville or Butler Trucks
                user_defaults default_from_freight_location_cd = user_defaults.GetUserDefaultByName(current_application_security.user_id, "FromFreightLocation");

                int from_freight_location_cd = 77;

                if (default_from_freight_location_cd != null)
                {
                    from_freight_location_cd = Convert.ToInt32(default_from_freight_location_cd.value);
                }

                //Display Rail loads if Rail User
                if (default_rail_user.value == "YES")
                {
                    var shipment_loads = from sl in db.shipment_load
                                         where (sl.carrier_mode == "R" || sl.carrier_mode == "B") && (sl.shipped_date == null && sl.load_status != "SH" && sl.load_id == sl.master_load_id)
                                         && sl.from_freight_location_cd == from_freight_location_cd
                                         select sl;

                    foreach (shipment_load shipment in shipment_loads.OrderByDescending(x => x.vehicle_no).ThenBy(x => x.load_id))
                    {
                        open_rail_loads.Add(shipment);
                    }
                }
            }

            return open_rail_loads;
        }

        public static List<int> GetStagedRailLoads(DateTime schedule_date, string char_load_id, string ship_to_location_name, string rail_route, string rail_car_type, string print_status)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            var shipment_loads = (from sl in db.v_sw_print_rail_loads
                                  where sl.stage_rail == "Y" && ((sl.schedule_date == schedule_date) && (sl.char_load_id == char_load_id || char_load_id == "") && (sl.ship_to_location_name_long == ship_to_location_name || ship_to_location_name == "") && (sl.rail_route == rail_route || rail_route == "") && (sl.rail_car_type == rail_car_type || rail_car_type == "") && ((sl.print_datetime != null && print_status == "Printed") || (sl.print_datetime == null && print_status == "Not Printed") || print_status == ""))
                                  select sl.load_id).Distinct();

            List<int> LoadIDs = new List<int>();

            foreach (int shipment in shipment_loads)
            {
                LoadIDs.Add(shipment);
            }

            return LoadIDs;
        }

        public static List<int> GetStagedRailLoadsWithPriorUnshipped(DateTime schedule_date, string char_load_id, string ship_to_location_name, string rail_route, string rail_car_type, string print_status)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            var shipment_loads = (from sl in db.v_sw_print_rail_loads
                                  where sl.stage_rail == "Y" && ((sl.schedule_date == schedule_date || (sl.schedule_date < schedule_date && sl.shipped_date == null)) && (sl.char_load_id == char_load_id || char_load_id == "") && (sl.ship_to_location_name_long == ship_to_location_name || ship_to_location_name == "") && (sl.rail_route == rail_route || rail_route == "") && (sl.rail_car_type == rail_car_type || rail_car_type == "") && ((sl.print_datetime != null && print_status == "Printed") || (sl.print_datetime == null && print_status == "Not Printed") || print_status == ""))
                                  select sl.load_id).Distinct();

            List<int> LoadIDs = new List<int>();

            foreach (int shipment in shipment_loads)
            {
                LoadIDs.Add(shipment);
            }

            return LoadIDs;
        }

        public static shipment_load GetShipmentLoad(string char_load_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.shipment_load.Where(x => x.char_load_id == char_load_id || x.char_load_id == char_load_id + "-901").FirstOrDefault();
        }

        public static List<int> isMasterLoad(string char_load_id)
        {
            List<int> loadsToReturn = new List<int> { };

            if (char_load_id == null || char_load_id == "")
            {
                loadsToReturn.Add(0);
                return loadsToReturn;
            }

            int load_id;
            load_id = System.Convert.ToInt32(char_load_id);


            sdipdbEntities db = ContextHelper.SDIPDBContext;

            var loads = db.shipment_load.Where(x => x.master_load_id == load_id && x.char_load_id != char_load_id).ToList();

            if (loads.Count() > 0)
            {
                foreach (shipment_load x in loads)
                {

                    loadsToReturn.Add(x.load_id);
                }
            }

            return loadsToReturn;
        }

        public static shipment_load GetShipmentSubLoad(int master_load_id, string production_coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            var query = from sl in db.shipment_load
                        join ld in db.load_dtl on sl.load_id equals ld.load_id
                        where sl.master_load_id == master_load_id && ld.production_coil_no == production_coil_no
                        select sl;

            return query.FirstOrDefault();
        }

        public static shipment_load GetShipmentLoad(string char_load_id, string production_coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            var query = from sl in db.shipment_load
                        join ld in db.load_dtl on sl.load_id equals ld.load_id
                        where sl.char_load_id.Contains(char_load_id.Substring(1, char_load_id.IndexOf("-"))) && ld.production_coil_no == production_coil_no
                        select sl;

            return query.FirstOrDefault();
        }

        public static shipment_load GetShipmentLoad(int load_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.shipment_load.Where(x => x.load_id == load_id).FirstOrDefault();
        }

        public static List<shipment_load> GetShipmentLoadSubLoads(string char_load_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.shipment_load.Where(x => x.char_load_id.Contains(char_load_id.Substring(1, char_load_id.IndexOf("-")))).ToList();
        }

        public static void UpdateVehicleNoAndLoadingUser(int load_id, string vehicle_no, int change_user_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            shipment_load current_load = db.shipment_load.Where(x => x.load_id == load_id).FirstOrDefault();

            current_load.vehicle_no = vehicle_no;
            current_load.change_user_id = change_user_id;
            current_load.change_datetime = DateTime.Now;
            current_load.loading_start_datetime = DateTime.Now;
            current_load.loading_start_user = change_user_id;

            db.SaveChanges();
        }

        public static string UpdateNewRailLoad(int load_id, string vehicle_no, application_security current_application_security, byte? rail_car_no = null)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            rail_cars current_railcar = db.rail_cars.Where(x => x.vehicle_no == vehicle_no).FirstOrDefault();

            user_defaults user_initials = user_defaults.GetUserDefaultByName(current_application_security.user_id, "UserInitials");
            ref_sys_param rsp = ref_sys_param.GetRefSysParam();

            short? carrier_cd = 0;

            string message = "";

            if (user_initials == null)
            {
                user_initials = new user_defaults { };

                if (rsp.location == "B")
                {
                    user_initials.value = "N/A";
                }
                else
                { 
                    user_initials.value = "";
                }
            }

            //Is subLoads involved
            List<int> subLoads = new List<int>();

            //Check if this is a subload - Butler
            subLoads = shipment_load.isMasterLoad(load_id.ToString());

            List<shipment_load> current_loads;

            if (subLoads.Count() > 0)
            {
                current_loads = db.shipment_load.Where(x => x.master_load_id == load_id).ToList();
            }
            else
            {
                current_loads = db.shipment_load.Where(x => x.load_id == load_id).ToList();
            }

            carrier_cd = current_loads.FirstOrDefault().carrier_cd;

            //Does Rail load exceed Max allowed weight(286,000 Lbs)? Check only for Actual shipments and not Rail Shuttle
            if (carrier_cd != 258)
            {
                int? total_weight = (current_loads.Where(x => x.load_id == load_id).FirstOrDefault().total_weight_load + current_railcar.empty_weight);

                if (!rail_cars.WitinRailCarWeightLimit(vehicle_no, total_weight, current_application_security))
                {
                    message = "WARNING";
                }

                //else if (total_weight > 286000)
                //{ 
                //    message = "WARNING";
                //}
            }

            foreach (shipment_load current_load in current_loads)
            {
                if (current_load.load_status == "SH")
                {
                    message = "LOAD SHIPPED";
                    continue;
                }

                current_load.vehicle_no = vehicle_no;
                current_load.change_user_id = current_application_security.user_id;
                current_load.change_datetime = DateTime.Now;
                current_load.loading_start_datetime = DateTime.Now;
                current_load.loading_start_user = current_application_security.user_id;
                current_load.scale_weight_in = current_railcar.empty_weight;
                current_load.load_status = "PI";
                current_load.scale_time_in = DateTime.Now;
                current_load.initial_in = user_initials.value;
                current_load.rail_car_number = rail_car_no;
            }
            db.SaveChanges();

            if (message != "WARNING" && message != "LOAD SHIPPED")
            {
                message = "SUCCESS";
            }
            return message;
        }

        public static void UpdateStageRail(int load_id, string stage_rail, int change_user_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            shipment_load current_load = db.shipment_load.Where(x => x.load_id == load_id).FirstOrDefault();

            current_load.stage_rail = stage_rail;
            current_load.change_user_id = change_user_id;
            current_load.change_datetime = DateTime.Now;

            db.SaveChanges();
        }

        public static void UpdatePrintDateTime(int load_id, int change_user_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            shipment_load current_load = db.shipment_load.Where(x => x.load_id == load_id).FirstOrDefault();

            current_load.print_datetime = DateTime.Now;
            current_load.change_user_id = change_user_id;
            current_load.change_datetime = DateTime.Now;

            db.SaveChanges();
        }

        public static void UpdateLoadStatusAndLoadingUser(int load_id, string load_status, int change_user_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            shipment_load current_load = db.shipment_load.Where(x => x.load_id == load_id).FirstOrDefault();

            current_load.load_status = load_status;
            current_load.change_user_id = change_user_id;
            current_load.change_datetime = DateTime.Now;
            current_load.loading_end_datetime = DateTime.Now;
            current_load.loading_end_user = change_user_id;

            db.SaveChanges();
        }

        public static void UpdateScaleTimeOut(int load_id, int change_user_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            shipment_load current_load = db.shipment_load.Where(x => x.load_id == load_id).FirstOrDefault();

            current_load.change_user_id = change_user_id;
            current_load.change_datetime = DateTime.Now;
            current_load.scale_time_out = DateTime.Now;

            db.SaveChanges();
        }

        public static string UpdateScaleTimeOutButler(int load_id, int change_user_id)
        {
            try
            {
                sdipdbEntities db = ContextHelper.SDIPDBContext;

                shipment_load current_load = db.shipment_load.Where(x => x.load_id == load_id).FirstOrDefault();

                user_defaults user_initials = user_defaults.GetUserDefaultByName(change_user_id, "UserInitials");

                if (user_initials == null)
                {
                    user_initials = new user_defaults();
                    user_initials.value = "N/A";
                }

                current_load.change_user_id = change_user_id;
                current_load.change_datetime = DateTime.Now;
                current_load.scale_time_out = DateTime.Now;
                current_load.initial_out = user_initials.value;

                db.SaveChanges();
                return "SUCCESS";
            }
            catch (Exception ex)
            {
                //throw new Exception("Error Update ScaleTimeOut at Butler!" + ex.Message);
                return "Error Update ScaleTimeOut at Butler!" + ex.Message;
            }
        }

        public static void CancelRailLoad(int load_id, int change_user_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            shipment_load current_load = db.shipment_load.Where(x => x.load_id == load_id).FirstOrDefault();

            ref_sys_param rsp = ref_sys_param.GetRefSysParam();

            current_load.vehicle_no = null;
            current_load.change_user_id = change_user_id;
            current_load.change_datetime = DateTime.Now;
            current_load.loading_start_datetime = null;
            current_load.loading_start_user = null;
            current_load.rail_car_number = null;

            if (rsp.location != "C")
            {
                current_load.load_status = "SC";
            }

            db.SaveChanges();
        }

        public static void CheckInTruck(int load_id, DateTime scale_time_in, int scale_weight_in, string vehicle_no, string driver_name, int change_user_id, int carrier_cd, string customer_pick_up_description, int? pickup_no = null)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            ref_sys_param rsp = ref_sys_param.GetRefSysParam();

            shipment_load current_load = db.shipment_load.Where(x => x.load_id == load_id).FirstOrDefault();

            current_load.scale_time_in = scale_time_in;
            current_load.scale_weight_in = scale_weight_in;
            current_load.vehicle_no = vehicle_no;
            current_load.driver_name = driver_name;
            current_load.change_datetime = DateTime.Now;
            current_load.change_user_id = change_user_id;
            current_load.carrier_cd = Convert.ToInt16(carrier_cd);

            var userInitials = user_defaults.GetUserDefaultByName(change_user_id, "UserInitials");
            if (userInitials != null)
                current_load.initial_in = userInitials.value;

            if (rsp.location == "C")
            {
                current_load.FromShipToID = 20625; //temporary while we are syncing loads from lomas. if checked in we know it's from Columbus
            }
            else if (rsp.location == "T")
            {
                //TODO: Set Techs FromShipToID
            }

            current_load.customer_pick_up_description = customer_pick_up_description;
            current_load.PickUp_no = pickup_no;
            current_load.loading_start_datetime = DateTime.Now;
            current_load.loading_start_user = change_user_id;

            db.SaveChanges();
        }

        public static bool IsValidKioskLoad(string char_load_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.shipment_load
                .Where(x => x.char_load_id == char_load_id || x.char_load_id == char_load_id + "-901")
                .Where(x => x.carrier_mode == "T")
                .Count() > 0;
        }

        public static bool IsValidKioskLoad(int prefix, int suffix)
        {
            try
            {
                vw_sw_pre_check_loads search = vw_sw_pre_check_loads.GetPreCheck(prefix, suffix);
                shipment_load load = GetShipmentLoad((int)search.load_id);

                return load != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void CancelTruckCheckIn(int load_id, int change_user_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            shipment_load current_load = db.shipment_load.Where(x => x.load_id == load_id).FirstOrDefault();
            current_load.scale_time_in = null;
            current_load.scale_weight_in = null;
            current_load.vehicle_no = null;
            current_load.driver_name = null;
            current_load.change_datetime = DateTime.Now;
            current_load.change_user_id = change_user_id;
            current_load.loading_start_user = null;
            current_load.loading_start_datetime = null;

            db.SaveChanges();
        }

        public static void SetScaleWeightOut(int load_id, int? scale_weight_out, int change_user_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            shipment_load current_load = db.shipment_load.Where(x => x.load_id == load_id).FirstOrDefault();
            current_load.scale_weight_out = scale_weight_out;
            current_load.change_datetime = DateTime.Now;
            current_load.change_user_id = change_user_id;

            db.SaveChanges();
        }

        public static void CheckOutTruck(int load_id, int change_user_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            ref_sys_param rsp = ref_sys_param.GetRefSysParam();

            shipment_load current_load = db.shipment_load.Where(x => x.load_id == load_id).FirstOrDefault();
            current_load.scale_time_out = DateTime.Now;
            current_load.change_datetime = DateTime.Now;
            current_load.change_user_id = change_user_id;
            current_load.loading_end_user = change_user_id;
            current_load.loading_end_datetime = DateTime.Now;

            //For Techs check-out non-lomas loads
            if (rsp.location == "T")
            {
                current_load.ShipmentType = "F";
            }

            db.SaveChanges();
        }

        public static List<string> GetRecentDrivers(string prefix)
        {

            sdipdbEntities db = ContextHelper.SDIPDBContext;

            DateTime days_ago = DateTime.Now.AddDays(-90);

            var recent_drivers = (from shipments in db.shipment_load.Where(x => x.scale_time_out > days_ago && x.driver_name.Contains(prefix))//.Where(x => x.driver_name.Contains(prefix))
                                  select shipments.driver_name).Distinct();

            List<string> recent_driver_list = new List<string>();

            foreach (string d in recent_drivers)
            {
                recent_driver_list.Add(d.ToString());
            }

            return recent_driver_list;

        }
        public static int GetLoadCarrier(string char_load_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;
            int carrier_cd = -1;
            var load_detail = db.shipment_load.FirstOrDefault(c => c.char_load_id == char_load_id);
            if (load_detail != null)
            {
                if (load_detail.carrier_cd != null)
                {
                    carrier_cd = (int)load_detail.carrier_cd;
                }
                var x = load_detail.carrier.name;
            }
            return carrier_cd;
        }

        public List<CoilsInLoad> GetcoilsInLoad(int load_id, int is_master)
        {
            List<CoilsInLoad> coils;

            if (is_master == 1)
            {
                coils = load_dtl.GetLoadDtlCoilsSubs(load_id);
            }
            else
            {
                coils = load_dtl.GetLoadDtlCoils(load_id);
            }

            return coils;
        }
    }

    // Only used with /Kiosk/States
    public partial class location
    {
        public string state_abbr;
        public string state_long;
    }
}
