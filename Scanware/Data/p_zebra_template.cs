using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class zebra_template
    {
        public static zebra_template GetTemplate(int template_cd)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            zebra_template c = db.zebra_template.SingleOrDefault(x => x.pk == template_cd);

            return c;

        }

        public static List<zebra_template> GetAllTemplates()
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;
            return db.zebra_template.ToList();

        }

        public static List<zebra_template> GetAllCoilTemplates()
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.zebra_template.Where(x => x.template_type == "Coil").ToList();



        }


    }

}