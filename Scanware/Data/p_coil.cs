using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class coil
    {
        public static coil GetCoilByProductionCoilNumber(string production_coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            coil return_coil = db.coils.SingleOrDefault(x => x.production_coil_no == production_coil_no);
            if (return_coil == null)
            {
                var wcpd = db.warehoused_coil_product_data.SingleOrDefault(x => x.production_coil_no == production_coil_no);
                var wi = db.warehoused_inventory.SingleOrDefault(x => x.production_coil_no == production_coil_no);
                if (wcpd != null && wi != null)
                {
                    var status = GetWarehouseCoilStatus(wi.warehouse_status_cd);
                    return_coil = new coil
                    {
                        production_coil_no = wcpd.production_coil_no,
                        ship_to_location_name = wi.ship_to_location_name,
                        order_no = 0,
                        line_item_no = 0,
                        coil_status = status,
                        customer_order_line_item = new customer_order_line_item
                        {
                            customer = db.customers.SingleOrDefault(c => c.customer_id == wi.customer_id),
                            customer_id = wi.customer_id,
                            product_type_cd = 0,
                            finish_line_setup = new finish_line_setup
                            {
                                packaging_type_cd = 0
                            },

                        },

                    };
                }
            }

            return return_coil;
        }
        public static string GetWarehouseCoilStatus(string status_cd)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;
            var sql = "select status_description from warehoused_status where warehouse_status_cd = @warehouse_status_cd";
            var warehouse_status = db.Database.SqlQuery<string>(sql, new SqlParameter("@warehouse_status_cd", status_cd)).SingleOrDefault();
            return warehouse_status;
        }

        public static string GetCoilStatusByProductionCoilNumber(string production_coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            coil c = db.coils.SingleOrDefault(x => x.production_coil_no == production_coil_no);
            string status = null;
            if (c == null)
            {
                var wi = db.warehoused_inventory.SingleOrDefault(x => x.production_coil_no == production_coil_no);
                if (wi != null)
                {
                    status = wi.warehouse_status_cd;
                }
               
            }
            else
            {
                status = c.coil_status;
            }
            return status;
        }
        public static string GetShipToLocationName(string production_coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            coil c = db.coils.SingleOrDefault(x => x.production_coil_no == production_coil_no);

            return c.ship_to_location_name;
        }

        public static string GetCharLoadID(string production_coil_no)
        {

            string char_load_id = "";


            sdipdbEntities db = ContextHelper.SDIPDBContext;

            var query = from c in db.coils
                        join ld in db.load_dtl on c.production_coil_no equals ld.production_coil_no
                        join sl in db.shipment_load on ld.load_id equals sl.load_id
                        where c.production_coil_no == production_coil_no
                        select sl.char_load_id;

            //foreach (string load_id in query)
            //{
            //    char_load_id = load_id;
            //}

            char_load_id = query.FirstOrDefault();

            return char_load_id;


        }

        public static string GetProductTypeDescription(string production_coil_no)
        {
            string product_type;

            sdipdbEntities db = ContextHelper.SDIPDBContext;

            var query = from c in db.coils
                        join coli in db.customer_order_line_item on new { c.order_no, c.line_item_no } equals new { coli.order_no, coli.line_item_no }
                        join pt in db.product_type on coli.product_type_cd equals pt.product_type_cd
                        where c.production_coil_no == production_coil_no
                        select pt.description;

            product_type = query.FirstOrDefault();

            return product_type;

        }

    }
}