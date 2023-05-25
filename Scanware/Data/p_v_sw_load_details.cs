using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class v_sw_load_details
    {
        public static List<v_sw_load_details> GetLoadDetailsByProductionCoilNo(string production_coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.v_sw_load_details.Where(x => x.production_coil_no == production_coil_no).ToList();
        }

        public static List<v_sw_load_details> GetLoadDetailsByTagNo(string tag_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.v_sw_load_details.Where(x => x.tag_no == tag_no).ToList();
        }

        public static List<v_sw_load_details> GetLoadDetailsByCharLoadID(string char_load_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.v_sw_load_details.Where(x => x.char_load_id.Contains(char_load_id)).ToList(); //like
        }

        public static List<v_sw_load_details> GetLoadDetailsByLoadID(int load_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.v_sw_load_details.Where(x => x.load_id == load_id).ToList(); 
        }
        public static List<v_sw_load_details> GetLoadDetailsByPoOrderNo(string po_order_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.v_sw_load_details.Where(x => x.po_order_no == po_order_no).ToList();
        }

        public static List<v_sw_load_details> GetLoadDetailsByOrderLineItemNo(string order_line_item_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.v_sw_load_details.Where(x => x.order_line_item_no.Contains(order_line_item_no)).ToList(); //like
        }

        public static List<v_sw_load_details> GetLoadDetailsByVehicleNo(string vehicle_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.v_sw_load_details.Where(x => x.vehicle_no == vehicle_no).ToList();
        }

    }
}