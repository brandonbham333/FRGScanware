using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class warehoused_inventory
    {
        public static string GetCoilStatusByProductionCoilNumber(string production_coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            warehoused_inventory c = db.warehoused_inventory.SingleOrDefault(x => x.production_coil_no == production_coil_no);

            if (c == null)
            {
                return "NA";
            }
            return c.warehouse_status_cd;
        }

        public static void UpdateCoilStatusByProductionCoilNumber(string production_coil_no, string status, int change_user_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            warehoused_inventory new_coil = db.warehoused_inventory.SingleOrDefault(x => x.production_coil_no == production_coil_no);

            new_coil.change_datetime = DateTime.Now;
            new_coil.change_user_id = change_user_id;
            new_coil.warehouse_status_cd = status;

            db.SaveChanges();
        }
    }
}