using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class vw_sw_outbound_op_coils
    {
        public static List<vw_sw_outbound_op_coils> GetOPOutboundOPCoils(string location_column, string ship_to_location_name)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.vw_sw_outbound_op_coils.OrderBy(x => x.ship_to_location_name).Where(y => y.column == location_column || location_column == "" || location_column == null).Where(z => z.ship_to_location_name == ship_to_location_name || ship_to_location_name== null || ship_to_location_name =="").ToList();
        }

        public static vw_sw_outbound_op_coils GetOutBoundCoil(string production_coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.vw_sw_outbound_op_coils.SingleOrDefault(x => x.production_coil_no == production_coil_no);

        }

        public static List<string> GetDistinctShipToLocationNames()
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.vw_sw_outbound_op_coils.Select(m => m.ship_to_location_name).Distinct().ToList();

        }

        public static List<string> GetDistinctLocationColumns()
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;
            var result =  db.vw_sw_outbound_op_coils.Where(y=>y.column != null).Select(x => x.column).Distinct();

            List<string> columns = new List<string>();

            foreach (var column in result)
            {
                columns.Add(column.ToString());
            }

            return columns;

        }
    }
}