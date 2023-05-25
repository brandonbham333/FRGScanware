using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class v_zebra_template_paint_location
    {
        public static v_zebra_template_paint_location GetPaintLocationTemplate(int location_cd)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            v_zebra_template_paint_location c = db.v_zebra_template_paint_location.SingleOrDefault(x => x.location_cd == location_cd);

            return c;

        }
    }
}