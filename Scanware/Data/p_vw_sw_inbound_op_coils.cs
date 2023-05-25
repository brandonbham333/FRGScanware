using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class vw_sw_inbound_op_coils
    {
        public static List<vw_sw_inbound_op_coils> GetOPInboundOPCoils()
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.vw_sw_inbound_op_coils.OrderBy(x => x.column).OrderBy(y => y.produced_dt_stamp).ToList();

        }

        public static vw_sw_inbound_op_coils GetOPInboundOPCoil(string production_coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.vw_sw_inbound_op_coils.Where(x=>x.production_coil_no == production_coil_no).FirstOrDefault();

        }


    }
}