using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class sw_paint_log
    {
        public static void InsertPaintLog(string object_name, string barcode_number, int add_user_id, string message){

            sw_paint_log ins_rec = new sw_paint_log()
            {
                object_name = object_name,
                add_user_id = add_user_id,
                add_datetime = DateTime.Now,
                barcode_number=barcode_number,
                message=message
               
            };

            sdipdbEntities db = ContextHelper.SDIPDBContext;
            db.sw_paint_log.Add(ins_rec);
            db.SaveChanges();

        }
        
    }
}