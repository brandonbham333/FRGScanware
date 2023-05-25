using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class v_zebra_template_load_card
    {
        public static v_zebra_template_load_card GetLoadCardTemplate(int load_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            v_zebra_template_load_card c = db.v_zebra_template_load_card.FirstOrDefault(x => x.load_id==load_id);

            return c;
        }
    }
}