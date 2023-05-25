using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class v_apc_images
    {
        public static v_apc_images GetCoilImage(int image_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.v_apc_images.Where(x => x.image_no == image_no).FirstOrDefault();
        }

        public static List<v_apc_images> GetCoilImages(int cons_coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            string cons_string = cons_coil_no.ToString();

            return db.v_apc_images.Where(x => x.production_coil_no.Contains(cons_string)).OrderBy(m => m.image_no).ToList();
        }
    }
}