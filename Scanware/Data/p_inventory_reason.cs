using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class inventory_reason
    {
        public static inventory_reason GetInventoryReason(string inventory_reason_cd)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            inventory_reason c = db.inventory_reason.SingleOrDefault(x => x.inventory_reason_cd == inventory_reason_cd);

            return c;
        }
    }
}