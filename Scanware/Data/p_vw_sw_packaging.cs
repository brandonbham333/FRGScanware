using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class vw_sw_packaging
    {
        public static List<vw_sw_packaging> GetCoilsToBeFurtherPackaged(string facility_cd)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            List<vw_sw_packaging> coils_to_package = new List<vw_sw_packaging>();

            coils_to_package = db.vw_sw_packaging.Where(x => x.facility_cd == facility_cd && x.further_package == "Y").OrderBy(y => y.production_coil_no).ToList();

            return coils_to_package;

        }

        public static List<vw_sw_packaging> GetCoilsToBeLinePackaged(string facility_cd)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            List<vw_sw_packaging> coils_to_package = new List<vw_sw_packaging>();

            coils_to_package = db.vw_sw_packaging.Where(x => x.facility_cd == facility_cd && x.line_package == "Y").OrderBy(y => y.production_coil_no).ToList();

            return coils_to_package;

        }
    }
}