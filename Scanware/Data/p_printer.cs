using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class printer
    {
        public static List<printer> GetAllPrinters()
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.printers.ToList();

        }

        public static List<printer> GetAllZebraPrinters()
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.printers.Where(x=>x.type=="Zebra").ToList();

        }


        public static printer GetPrinterByPath(string path)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.printers.FirstOrDefault(x => x.path == path);
        }

        public static printer GetPrinterByPK(int pk)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.printers.FirstOrDefault(x => x.pk == pk);
        }
    }
}