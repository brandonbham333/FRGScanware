using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class scanware_shipping_loads
    {
        public static void remove(int load_id)
        { 
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            scanware_shipping_loads existing_row = db.scanware_shipping_loads.SingleOrDefault(x => x.load_id == load_id);

            if (existing_row != null)
            { 
                db.scanware_shipping_loads.Remove(existing_row);    
                db.SaveChanges();
            }
        }
    }
}
   
