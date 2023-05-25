using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class paint_inventory
    {
        public static paint_inventory GetPaintInventoryByBarcode(string vendor_container_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.paint_inventory.Where(x => x.vendor_container_id == vendor_container_id).OrderByDescending(m => m.seq_no).FirstOrDefault();

        }
       
        public static paint_inventory GetPaintInventoryByBarcodeButler(paint_barcode barcode_string)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

           paint_inventory container = db.paint_inventory.Where(x => x.purchase_order_no == barcode_string.po_no && x.batch_no == barcode_string.batch_no
            && x.paint_cd == barcode_string.paint_code && x.drum_no == barcode_string.drum_no).OrderByDescending(m => m.seq_no).FirstOrDefault();

            if(container != null)
            {
                return container;
            }
            else
            {   

                //Check if All Barcode information match except Drum # - if using Temp Drum value - replace with actual
                paint_inventory container2 = db.paint_inventory.Where(x => x.purchase_order_no == barcode_string.po_no && x.batch_no == barcode_string.batch_no
                && x.paint_cd == barcode_string.paint_code && x.drum_no.Contains("-")).OrderByDescending(m => m.seq_no).FirstOrDefault();

                if (container2 != null)
                {
                    return container2;
                }
                else
                {
                    return new paint_inventory();
                }
            }
            
        }

        public static void InsertPaintInventory(paint_inventory pi)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            db.paint_inventory.Add(pi);

            db.SaveChanges();

        }
    }
}