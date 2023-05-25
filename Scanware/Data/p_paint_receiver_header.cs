using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Scanware.Data;

namespace Scanware.Data
{
    public partial class paint_receiver_header
    {
        public static string IsDuplicate(paint_barcode to_check)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            DateTime FromDate = DateTime.Now;
            DateTime? ReceivedDate;
            FromDate = Convert.ToDateTime(FromDate).AddDays(-2);

            paint_inventory pi = db.paint_inventory.SingleOrDefault(x => x.drum_no == to_check.drum_no);

            if (pi != null)
            {
                ReceivedDate = pi.received_date;

                return "This might be a duplicate Receipt! Drum No. " + to_check.drum_no + " Already exist and was received on " + pi.received_date;
            }

            paint_receiver_header pr = db.paint_receiver_header.SingleOrDefault(x => x.po_number == to_check.po_no
            && x.batch_number == to_check.batch_no && x.bill_of_lading == to_check.bol
            && x.paint_cd == to_check.paint_code && x.date_in > FromDate);

            if (pr != null)
            {
                return "This might be a duplicate Receipt!";
            }

            return "";
        }
    }
}