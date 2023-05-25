using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class v_shipment_load_images
    {
        public static v_shipment_load_images GetLoadImage(int image_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.v_shipment_load_images.Where(x => x.image_no == image_no).FirstOrDefault();
        }
    }
}