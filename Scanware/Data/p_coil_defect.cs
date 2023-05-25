using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class coil_defect
    {
        public static void AddManualDefect(string production_coil_no, int? user_id, string defect_cd, string unit_detected, string unit_responsible, string defect_comment, string inv_or_hold,
            int? order_no, int? line_item_no, int? cons_coil_no, string order_inv, string reason_last_change, string app, int seq_no)
        {

            //string type, string mode,


            coil_defect cd = new coil_defect()
            {
                production_coil_no = production_coil_no, 
                defect_cd = defect_cd, 
                unit_detection = unit_detected, 
                defect_comment = defect_comment, 
                change_datetime = DateTime.Now, 
                change_user_id = user_id, 
                observed_only = "Y", 
                inventory_or_hold = inv_or_hold, 
                order_no = order_no, 
                line_item_no = line_item_no, 
                cons_coil_no = cons_coil_no, 
                order_inventory = order_inv,
                add_datetime = DateTime.Now,
                seq_no = seq_no,
                reason_for_last_status_change = reason_last_change,
                unit_responsible = unit_responsible
            };

            sdipdbEntities db = ContextHelper.SDIPDBContext;
            db.coil_defect.Add(cd);
            db.SaveChanges();

        }

    }
}