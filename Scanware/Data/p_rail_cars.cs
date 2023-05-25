using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Scanware.Data
{
    public partial class rail_cars
    {
        public static List<rail_cars> GetAllRailCars()
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.rail_cars.OrderBy(x => x.vehicle_no).ToList();
        }

        public static rail_cars GetRailCar(string vehicle_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;
            vehicle_no = Regex.Replace(vehicle_no, @"\s+", "");
            rail_cars r = db.rail_cars.FirstOrDefault(x => x.vehicle_no == vehicle_no);

            if (r == null)
            {
                return null;
            }
            return r;
        }

        public static void UpdateRailCarDetails(string vehicle_no, int empty_weight, string status, string permanent_flg, DateTime weight_in_datetime, int change_user_id, int? max_weight_limit = null)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;
            vehicle_no = Regex.Replace(vehicle_no, @"\s+", "");
            rail_cars to_update = db.rail_cars.FirstOrDefault(x => x.vehicle_no == vehicle_no);
            to_update.empty_weight = empty_weight;
            to_update.status = status;
            to_update.permanent_flag = permanent_flg;
            to_update.weight_in_datetime = weight_in_datetime;
            to_update.change_datetime = DateTime.Now;
            to_update.change_user_id = change_user_id;
            to_update.max_weight_limit = max_weight_limit;

            db.SaveChanges();

        }

        public static void AddRailCar(string vehicle_no, int empty_weight, string status, string permanent_flg, DateTime weight_in_datetime, int change_user_id, int? max_weight_limit = null)
        {

            rail_cars to_update = new rail_cars();
            //remove spaces
            vehicle_no = Regex.Replace(vehicle_no, @"\s+", "");

            to_update.vehicle_no = vehicle_no;
            to_update.empty_weight = empty_weight;
            to_update.status = status;
            to_update.permanent_flag = permanent_flg;
            to_update.weight_in_datetime = weight_in_datetime;
            to_update.change_datetime = DateTime.Now;
            to_update.change_user_id = change_user_id;
            to_update.max_weight_limit = max_weight_limit;

            sdipdbEntities db = ContextHelper.SDIPDBContext;
            db.rail_cars.Add(to_update);
            db.SaveChanges();

        }

        public static bool RailCarHasMaxWeight(string vehicle_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;
            vehicle_no = Regex.Replace(vehicle_no, @"\s+", "");
            var hasWeight = db.rail_cars.Any(c => c.vehicle_no == vehicle_no && c.max_weight_limit != null);
            return hasWeight;
        }

        public static bool WitinRailCarWeightLimit(string vehicle_no, int? total_rail_car_weight, application_security current_application_security)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            application_settings set_max_rail_car_weight = application_settings.GetAppSetting("set_max_rail_car_weight") ?? new application_settings();

            var witinWeight = total_rail_car_weight <= 286000;
            if (set_max_rail_car_weight.default_value == "Y")
            {
                vehicle_no = Regex.Replace(vehicle_no, @"\s+", "");
                witinWeight = db.rail_cars.Any(c => c.vehicle_no == vehicle_no && total_rail_car_weight <= c.max_weight_limit);
            }

            return witinWeight;
        }

        public static void SaveRailCarMaxWeight(string vehicle_no, int max_weight_limit, int? user_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;
            vehicle_no = Regex.Replace(vehicle_no, @"\s+", "");
            var rc = db.rail_cars.FirstOrDefault(c => c.vehicle_no == vehicle_no);
            if (rc != null)
            {
                rc.max_weight_limit = max_weight_limit;
                rc.change_datetime = DateTime.Now;
                rc.change_user_id = user_id;
                db.SaveChanges();
            }
        }

    }
}