using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class vw_sw_produced_coils
    {
        public static List<vw_sw_produced_coils> GetProducedCoilsByFacility(string facility_cd)
        {

            sdipdbEntities db = ContextHelper.SDIPDBContext;

            List<vw_sw_produced_coils> produced_coils = new List<vw_sw_produced_coils>();

            produced_coils = db.vw_sw_produced_coils.Where(x => x.facility_cd == facility_cd).OrderBy(y => y.produced_dt_stamp).ToList();

            return produced_coils;

        }
    }
}