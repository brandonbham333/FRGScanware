using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class printer_queue
    {
        public static void AddToPrintQueue(byte[] bytes, int printer_pk, string type, string object_string, int copies, int add_user_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            printer_queue to_print = new printer_queue()
            {
                bytes=bytes, 
                printer_pk=printer_pk, 
                date_inserted=DateTime.Now,
                type=type,
                @object = object_string,
                copies = copies,
                add_user_id= add_user_id
            };

            db.printer_queue.Add(to_print);

            db.SaveChanges();
        }
    }
}