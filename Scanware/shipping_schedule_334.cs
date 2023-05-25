using Scanware.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scanware
{
    public class shipping_schedule_334 
    {
        public vw_shipping_schedule_334 schedule {get; set;}
        public static List<vw_shipping_schedule_334> GetScheduledCoils()
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;
            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];
            user_defaults default_bays = user_defaults.GetUserDefaultByName(current_application_security.user_id, "CoilYardBays");
            string[] selected_bays = { " " };
            List<vw_shipping_schedule_334> coils = new List<vw_shipping_schedule_334>();

            //Default values from the DB
            if (default_bays != null)
            {//Apply bay code filtering
                selected_bays = default_bays.value.Split(',');

                var returnCoils = from v in db.vw_shipping_schedule_334
                                  where selected_bays.Contains(v.bay_cd)
                                  orderby v.bay_cd, v.schedule_date
                                  select v;

                foreach (vw_shipping_schedule_334 schedule in returnCoils)
                {
                    coils.Add(schedule);
                }
            }
            else
            {//No Bay code filtering
                var returnCoils = from v in db.vw_shipping_schedule_334
                                  orderby v.bay_cd, v.location
                                  select v;

                foreach (vw_shipping_schedule_334 schedule in returnCoils)
                {
                    coils.Add(schedule);
                }
            }

            return coils;

        }
    }
}
