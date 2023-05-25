using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class inventory_item
    {
        public static inventory_item GetInventoryItemByProductionCoilNumber(string production_coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;
            inventory_item c = new inventory_item();

            try
            {
                c = db.inventory_item.SingleOrDefault(x => x.production_coil_no == production_coil_no);
            }
            catch (Exception ex)
            {
                
            }

            return c;
        }
    }
}