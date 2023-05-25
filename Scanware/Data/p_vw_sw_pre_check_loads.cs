using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class vw_sw_pre_check_loads
    {
        public static vw_sw_pre_check_loads GetPreCheck(int day_count, int random_pin)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.vw_sw_pre_check_loads.Where(x => x.day_count == day_count && x.random_pin == random_pin).FirstOrDefault();

        }
    }
}