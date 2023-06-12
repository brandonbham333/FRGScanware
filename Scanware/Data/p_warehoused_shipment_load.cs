using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class warehoused_shipment_load
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

        public static void CancelRailLoad(int load_id, int change_user_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            warehoused_shipment_load current_load = db.warehoused_shipment_load.Where(x => x.load_id == load_id).FirstOrDefault();

            ref_sys_param rsp = ref_sys_param.GetRefSysParam();

            current_load.vehicle_no = null;
            current_load.change_user_id = change_user_id;
            current_load.change_datetime = DateTime.Now;

            if (rsp.location != "C")
            {
                current_load.load_status = "SC";
            }

            db.SaveChanges();
        }
        public static List<warehoused_shipment_load> GetTrucksOnSite()
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            string[] selected_bays = { }, load_bays;

            List<warehoused_shipment_load> trucks_on_site = new List<warehoused_shipment_load>();

            ref_sys_param rsp = ref_sys_param.GetRefSysParam();
            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];
            user_defaults default_from_freight_location_cd = user_defaults.GetUserDefaultByName(current_application_security.user_id, "FromFreightLocation");

                try
                {
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

                        List<warehoused_shipment_load> shipment_loads2 = new List<warehoused_shipment_load>();

                        shipment_loads2 = db.warehoused_shipment_load.Where(sl => sl.carrier_mode == "T" && sl.shipped_date == null && sl.load_status == "PI" 
                               && sl.from_freight_location_cd == from_freight_location_cd).OrderBy(x => x.scale_time_in).ToList() ?? new List<warehoused_shipment_load>();


                        foreach (warehoused_shipment_load shipment in shipment_loads2)
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
                        var shipment_loads3 = from sl in db.warehoused_shipment_load
                                              where sl.carrier_mode == "T" && sl.scale_time_in != null && sl.scale_time_out == null
                                              select sl;
                        foreach (warehoused_shipment_load shipment in shipment_loads3.OrderBy(x => x.scale_time_in))
                        {
                            trucks_on_site.Add(shipment);
                        }
                    }
                }
                catch { return trucks_on_site; }
        

            return trucks_on_site;
        }

        public static List<warehoused_shipment_load> GetOpenRailLoads()
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            List<warehoused_shipment_load> open_rail_loads = new List<warehoused_shipment_load>();

            ref_sys_param rsp = ref_sys_param.GetRefSysParam();

                application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

                //to sort by j'ville or Butler Trucks
                user_defaults default_from_freight_location_cd = user_defaults.GetUserDefaultByName(current_application_security.user_id, "FromFreightLocation");

                int from_freight_location_cd = 77;

                if (default_from_freight_location_cd != null)
                {
                    from_freight_location_cd = Convert.ToInt32(default_from_freight_location_cd.value);
                }

                    var shipment_loads = from sl in db.warehoused_shipment_load
                                         where (sl.carrier_mode == "R" || sl.carrier_mode == "B") && (sl.shipped_date == null && sl.load_status != "SH")
                                         && sl.from_freight_location_cd == from_freight_location_cd
                                         select sl;

                    foreach (warehoused_shipment_load shipment in shipment_loads.OrderByDescending(x => x.vehicle_no).ThenBy(x => x.load_id))
                    {
                        open_rail_loads.Add(shipment);
                    }


            return open_rail_loads;
        }

        public List<string> GetLoadYardBays(int load_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;
            List<warehoused_load_dtl> coils = warehoused_load_dtl.GetLoadDtl(load_id);
            List<string> bays = new List<string>();

            string bay;

            foreach (var coil in coils)
            {
                string coil_check = coil.production_coil_no;
                coil_yard_locations new_loc = coil_yard_locations.GetCoilYardLocation(coil_check);

                string loc_col = new_loc.column;
                string loc_row = new_loc.row;

                coil_yard_rows d = new coil_yard_rows();

                bay = d.GetYardBay(loc_col, loc_row);
                bays.Add(bay);
            }

            return bays;
        }

        public static warehoused_shipment_load GetShipmentLoad(int load_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.warehoused_shipment_load.Where(x => x.load_id == load_id).FirstOrDefault();
        }

        public List<CoilsInLoad> GetcoilsInLoad(int load_id)
        {
            List<CoilsInLoad> coils;

            coils = warehoused_load_dtl.GetLoadDtlCoils(load_id);

            return coils;
        }

          public static string UpdateNewRailLoad(int load_id, string vehicle_no, application_security current_application_security, byte? rail_car_no = null)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            rail_cars current_railcar = db.rail_cars.Where(x => x.vehicle_no == vehicle_no).FirstOrDefault();

            user_defaults user_initials = user_defaults.GetUserDefaultByName(current_application_security.user_id, "UserInitials");

            short? carrier_cd = 0;

            string message = "";

            if (user_initials == null)
            {
                user_initials = new user_defaults { };
                user_initials.value = "";
            }

            List<warehoused_shipment_load> current_loads;

            current_loads = db.warehoused_shipment_load.Where(x => x.load_id == load_id).ToList();

            carrier_cd = current_loads.FirstOrDefault().carrier_cd;

            //Does Rail load exceed Max allowed weight(286,000 Lbs)? Check only for Actual shipments and not Rail Shuttle
            if (carrier_cd != 258)
            {
                int? total_weight = (current_loads.Where(x => x.load_id == load_id).FirstOrDefault().total_weight_load + current_railcar.empty_weight);

                if (total_weight > 286000)
                {
                    message = "WARNING";
                }
            }

            foreach (warehoused_shipment_load current_load in current_loads)
            {
                if (current_load.load_status == "SH")
                {
                    message = "LOAD SHIPPED";
                    continue;
                }

                current_load.vehicle_no = vehicle_no;
                current_load.change_user_id = current_application_security.user_id;
                current_load.change_datetime = DateTime.Now;
                current_load.scale_weight_in = current_railcar.empty_weight;
                current_load.load_status = "PI";
                current_load.scale_time_in = DateTime.Now;
                current_load.initial_in = user_initials.value;
                //current_load.rail_car_number = rail_car_no;
            }
            db.SaveChanges();

            if (message != "WARNING" && message != "LOAD SHIPPED")
            {
                message = "SUCCESS";
            }
            return message;
        }

        public static void UpdateLoadStatusAndLoadingUser(int load_id, string load_status, int change_user_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            warehoused_shipment_load current_load = db.warehoused_shipment_load.Where(x => x.load_id == load_id).FirstOrDefault();

            current_load.load_status = load_status;
            current_load.change_user_id = change_user_id;
            current_load.change_datetime = DateTime.Now;

            db.SaveChanges();
        }

        public static string UpdateScaleTimeOutWarehouse(int load_id, int change_user_id)
        {
            try
            {
                sdipdbEntities db = ContextHelper.SDIPDBContext;

                warehoused_shipment_load current_load = db.warehoused_shipment_load.Where(x => x.load_id == load_id).FirstOrDefault();

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
                return "Error Update ScaleTimeOut!" + ex.Message;
            }
        }

        public static string UpdateScaleTimeOutButler(int load_id, int change_user_id)
        {
            try
            {
                sdipdbEntities db = ContextHelper.SDIPDBContext;

                warehoused_shipment_load current_load = db.warehoused_shipment_load.Where(x => x.load_id == load_id).FirstOrDefault();

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
                return "Error Update ScaleTimeOut! " + ex.Message;
            }
        }
    }
}