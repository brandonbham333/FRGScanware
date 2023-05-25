using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class v_sw_print_rail_loads
    {
      
        public static List<v_sw_print_rail_loads> GetRailLoadsToPrint(DateTime scheduled_date)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;
            return db.v_sw_print_rail_loads.Where(x=> x.schedule_date == scheduled_date).OrderBy(y=>y.schedule_date).ThenBy(z=>z.char_load_id).ToList();

        }

        public static List<v_sw_print_rail_loads> GetRailLoadsToPrintWithPriorUnshipped(DateTime scheduled_date)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;
            return db.v_sw_print_rail_loads.Where(x => x.schedule_date == scheduled_date || (x.schedule_date < scheduled_date && x.shipped_date == null)).OrderBy(y => y.schedule_date).ThenBy(z => z.char_load_id).ToList();

        }


    }
}