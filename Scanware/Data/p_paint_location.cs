using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class paint_location
    {
        public static List<paint_location> GetActivePaintLocations()
        {

            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.paint_location.Where(x=>x.active=="Y").OrderBy(y=>y.sort_order).ToList();

        }

        public static List<paint_location> GetActivePaintLocationsButler()
        {

            sdipdbEntities db = ContextHelper.SDIPDBContext;
            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

            //to sort by j'ville or Butler locations
            user_defaults default_from_freight_location_cd = user_defaults.GetUserDefaultByName(current_application_security.user_id, "FromFreightLocation");

            int from_freight_location_cd = 77;

            if (default_from_freight_location_cd != null)
            {
                from_freight_location_cd = Convert.ToInt32(default_from_freight_location_cd.value);
            }

            if (from_freight_location_cd == 77)
            { 
                return db.paint_location.Where(x => x.active == "Y" && x.paint_line_location == "B").OrderBy(y => y.sort_order).ToList();
            }
            else
            {
                return db.paint_location.Where(x => x.active == "Y" && x.paint_line_location == "J").OrderBy(y => y.sort_order).ToList();
            }
            
            
        }

        public static paint_location GetPaintLocation(int location_cd)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.paint_location.Where(x => x.location_cd == location_cd).FirstOrDefault();

        }
    }
}