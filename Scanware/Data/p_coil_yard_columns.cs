using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class coil_yard_columns
    {

        public static List<coil_yard_columns> GetAllCoilYardColumns(int from_freight_location_cd)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            var cyc = from coil_yard_cols in db.coil_yard_columns
                      where coil_yard_cols.freight_location_cd == from_freight_location_cd
                      orderby coil_yard_cols.column
                      select coil_yard_cols;

            return cyc.ToList();


        }

        public static List<coil_yard_columns> GetAllCoilYardColumns(int from_freight_location_cd, string bay_cd)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            var cyc = from coil_yard_cols in db.coil_yard_columns
                      join cyr in db.coil_yard_rows on coil_yard_cols.column equals cyr.column
                      where coil_yard_cols.freight_location_cd == from_freight_location_cd && cyr.bay_cd == bay_cd
                      orderby coil_yard_cols.column
                      select coil_yard_cols;

            return cyc.Distinct().ToList();


        }

        public static List<coil_yard_columns> GetCoilYardColumns()
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.coil_yard_columns.OrderBy(y=>y.description).ToList();


        }

       
    }
}