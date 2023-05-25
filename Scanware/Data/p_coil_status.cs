using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class coil_status
    {
        public static coil_status GetCoilStatus(string coil_status)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            coil_status c = db.coil_status.SingleOrDefault(x => x.coil_status1 == coil_status);
            if (c == null)
            {
                var warehouse_status = coil.GetWarehouseCoilStatus(coil_status);
                c = new coil_status
                {
                    description = warehouse_status,
                };
            }

            return c;
        }
    }
}