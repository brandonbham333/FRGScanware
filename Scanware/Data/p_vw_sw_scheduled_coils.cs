using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class vw_sw_scheduled_coils
    {
        public static List<vw_sw_scheduled_coils> GetScheduledCoilsByFacility(string facility_cd)
        {

            sdipdbEntities db = ContextHelper.SDIPDBContext;

            List<vw_sw_scheduled_coils> scheduled_coils = new List<vw_sw_scheduled_coils>();

            scheduled_coils = db.vw_sw_scheduled_coils.Where(x => x.facility_cd == facility_cd).OrderBy(y => y.prod_seq).ToList();
            
            return scheduled_coils;

        }
    }
}