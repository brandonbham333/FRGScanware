using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class coil_yard_rows
    {
        public static List<coil_yard_rows> GetRowsByColumn(string column)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            var cyr = from coil_yard_rws in db.coil_yard_rows
                      where coil_yard_rws.column == column
                      select coil_yard_rws;

            List<coil_yard_rows> return_cyr = new List<coil_yard_rows>();

            foreach (coil_yard_rows coil_yard_row in cyr)
            {
                return_cyr.Add(coil_yard_row);
            }

            return return_cyr.OrderByDescending(x => x.row.All(char.IsDigit)).ThenByDescending(x => x.row.Any(char.IsLetter)).ThenBy(x => x.column).ToList();
            

        }

        public static List<coil_yard_rows> GetRowsByColumnAndBay(string column, string bay_cd)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            var cyr = from coil_yard_rws in db.coil_yard_rows
                      where coil_yard_rws.column == column && coil_yard_rws.bay_cd==bay_cd
                      select coil_yard_rws;

            List<coil_yard_rows> return_cyr = new List<coil_yard_rows>();

            foreach (coil_yard_rows coil_yard_row in cyr)
            {
                return_cyr.Add(coil_yard_row);
            }

            return return_cyr.OrderByDescending(x => x.row.All(char.IsDigit)).ThenByDescending(x => x.row.Any(char.IsLetter)).ThenBy(x => x.column).ToList();


        }

        public string GetYardBay(string column, string row)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            if (column == null || row == null)
                return "";

            var cyr = from coil_yard_rws in db.coil_yard_rows
                      where coil_yard_rws.column == column && coil_yard_rws.row == row
                      select coil_yard_rws.bay_cd;
            string ret = cyr.First();

            return ret;

        }

    }
}