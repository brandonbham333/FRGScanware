using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class v_zebra_template_coil
    {
        public static v_zebra_template_coil GetCoilTemplate(string production_coil_no, int pk)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            v_zebra_template_coil c = db.v_zebra_template_coil.SingleOrDefault(x => x.production_coil_no == production_coil_no && x.pk == pk);

            return c;

        }

        public static v_zebra_template_coil GetCoilTemplate(string production_coil_no, string name)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            v_zebra_template_coil c = db.v_zebra_template_coil.SingleOrDefault(x => x.production_coil_no == production_coil_no && x.name == name);

            return c;

        }
    }
}