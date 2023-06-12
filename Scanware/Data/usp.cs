using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using Scanware.Models;
using System.Web;

namespace Scanware.Data
{
    public class cs_wh_assess
    {
        public string return_status { get; set; }
        public string return_status_desc { get; set; }
    } 
    public class usp
    {
        public static void usp_sw_ship_load_in_lomas(string char_load_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            db.usp_sw_ship_load_in_lomas(char_load_id);

        }

        public static string usp_paint_receiving(paint_barcode to_receive)
        {
            int return_code = 0;

            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            string loc = Properties.Settings.Default.RegLocation;

            try
            {
                return_code = db.paint_receiving2(to_receive.po_no, to_receive.paint_code, to_receive.batch_no, to_receive.bol, to_receive.batch_gallons, current_application_security.user_id, loc, to_receive.drum_no);
            }
            catch (Exception ex)
            {
                return ex.InnerException.Message.ToString();

            }

            if (return_code == -1)
            {
                return "Receiving Process Failed!";
            }
            return "Successfully Received " + return_code.ToString() + " Containers";
        }

        public static string usp_call_spc_set_load_shipped_v2(string char_load_id)
        {
            int loadID = 0;
            int return_cd = 0;
           // string user_init;

            sdipdbEntities db = ContextHelper.SDIPDBContext;

          /*  application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];
            user_defaults user_initials = user_defaults.GetUserDefaultByName(current_application_security.user_id, "UserInitials");

            if (user_initials != null)
            {
                user_init = user_initials.value;
            }
            else
            {
                user_init = current_application_security.user_id.ToString();
            }*/

            int.TryParse(char_load_id, out loadID);
            try
            {
                return_cd = db.usp_call_spc_set_load_shipped_v2(loadID);
            }
                catch (Exception ex)
                {
                return ex.InnerException.Message.ToString();
            }

            if (return_cd == -1)
            {
                //Confirm that this was a failure

                scanware_shipping_loads load = db.scanware_shipping_loads.SingleOrDefault(x => x.load_id == loadID);
                short? rtn_cd = load.return_cd;

                if (rtn_cd < 0)
                    return "ERROR";
                else
                    return "SUCCESS";
            }

            return "SUCCESS";

        }

        public static string usp_get_loading_instruction(int load_id, string char_load_id, string coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            int order_no = 0, order_li = 0, line_item_coil = 0, customer_id = 0, from_location_cd;
            string coil_status = "", facility_cd = "", ship_to_location = "";
            var loading_instruct = "";

            try
            {                 
                ShippingModel viewModel = new ShippingModel();

                shipment_load current_load = shipment_load.GetShipmentLoad(char_load_id);

                customer_id = Convert.ToInt32(current_load.customer_id);
                from_location_cd = Convert.ToInt32(current_load.from_freight_location_cd);
                coil c = coil.GetCoilByProductionCoilNumber(coil_no);

                if (c != null)
                { 
                    coil_status = c.coil_status;
                    order_no = c.order_no;
                    order_li = c.line_item_no;
                    line_item_coil = c.line_item_coil_no;
                    ship_to_location = c.ship_to_location_name;
                }
                all_produced_coils apc_coil = db.all_produced_coils.Where(x => x.production_coil_no == coil_no).FirstOrDefault();

                if (apc_coil != null)
                {
                    facility_cd = apc_coil.facility_cd;
                }

                if (coil_status == "IE")
                {
                     var sql = "select pp.spec_instruct_loading from coil_planned_routing cpr LEFT JOIN product_processors pp " +
                                " ON cpr.processor_cd = pp.processor_cd LEFT JOIN receiver r ON r.receiver_id = pp.receiver_id " +
                                " where cpr.order_no = @order_no and cpr.line_item_no = @line_item_no " +
                                " and r.display_loading_instruct = 'Y'" +
                                " and cpr.line_item_coil_no = @line_item_coil_no and cpr.processing_order = (select min(processing_order) " +
                                " from coil_planned_routing where order_no = @order_no and line_item_no = @line_item_no " +
                                " and line_item_coil_no = @line_item_coil_no and processing_order > " +
                                " (select max(processing_order) from coil_planned_routing  where order_no = @order_no " +
                                " and line_item_no = @line_item_no and line_item_coil_no = @line_item_coil_no " +
                               " and facility_cd = @facility_cd and processed_ind = 'Y' ))";

                     loading_instruct = db.Database.SqlQuery<string>(sql, new SqlParameter("@order_no", order_no),
                                                                             new SqlParameter("@line_item_no", order_li),
                                                                             new SqlParameter("@line_item_coil_no", line_item_coil),
                                                                             new SqlParameter("@facility_cd", facility_cd)).SingleOrDefault();

                }
                else 
                {
                    var sql2 = "select c.spec_instruct_loading from customer_ship_to_origin c JOIN receiver r " +
                               "ON c.ship_to_location_name = r.ship_to_location_name where c.ship_to_location_name = @ship_to_location " +
                               " and c.customer_id = @customer_id and c.from_freight_location_cd = @from_location_cd AND r.display_loading_instruct = 'Y'";

                    loading_instruct = db.Database.SqlQuery<string>(sql2, new SqlParameter("@ship_to_location", ship_to_location),
                                                                            new SqlParameter("@customer_id", customer_id),
                                                                            new SqlParameter("@from_location_cd", from_location_cd)).SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                return "ERROR";
            }

            return loading_instruct;
    }

        public static string usp_call_spc_set_load_shipped_wh(string char_load_id)
        {
            int loadID = 0;
            int return_cd = 0;

            sdipdbEntities db = ContextHelper.SDIPDBContext;

            int.TryParse(char_load_id, out loadID);
            try
            {
                return_cd = db.call_spc_set_load_shipped_wh(loadID);
            }
            catch (Exception ex)
            {
                return ex.InnerException.Message.ToString();
            }

            if (return_cd == -1)
            {
                //Confirm that this was a failure

                scanware_shipping_loads load = db.scanware_shipping_loads.SingleOrDefault(x => x.load_id == loadID);
                short? rtn_cd = load.return_cd;

                if (rtn_cd < 0)
                    return "ERROR";
                else
                    return "SUCCESS";
            }

            return "SUCCESS";

        }
        
        public static void usp_sw_update_status_after_move(string production_coil_no, int user_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;
            var warehouseCoilStatus = warehoused_inventory.GetCoilStatusByProductionCoilNumber(production_coil_no);
            if (warehouseCoilStatus != "NA")
            {
                if (warehouseCoilStatus == "EC")
                {
                    warehoused_inventory.UpdateCoilStatusByProductionCoilNumber(production_coil_no,
                        "IY", user_id);
                }
                else
                {
                    cs_wh_assess status = new cs_wh_assess();
                    try
                    {
                        status = db.Database.SqlQuery<cs_wh_assess>("EXEC cs_wh_assess @production_coil_no, @user_id",
                            new SqlParameter("@production_coil_no", production_coil_no),
                            new SqlParameter("@user_id", user_id)
                            ).SingleOrDefault();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }


                    if (status.return_status != "")
                    {
                        warehoused_inventory.UpdateCoilStatusByProductionCoilNumber(production_coil_no,
                       status.return_status, user_id);
                    }
                }
            }
            else
            {
                db.usp_sw_update_status_after_move(production_coil_no, user_id);
            }
            
        }

        public static void usp_sw_update_rail_car_in_lomas(string char_load_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            db.usp_sw_update_rail_car_in_lomas(char_load_id);

        }

        public static void usp_add_coil_image(string production_coil_no, byte[] image_data)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            db.usp_add_coil_image(production_coil_no, image_data);
        }

        public static void usp_add_shipment_load_image(int load_id, int add_user_id, byte[] image_data)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            db.usp_add_shipment_load_image(load_id, image_data, add_user_id);
        }

        public static void usp_add_paint_coil_image(int load_id, int add_user_id, byte[] image_data)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            db.usp_add_shipment_load_image(load_id, image_data, add_user_id);
        }

        //public static void cs_defect_entry(string production_coil_no, int? user_id, string type, string mode, string defect_cd, string unit_detected, string defect_comment, string inv_or_hold, 
        //    int? order_no, int? line_item_no, int? cons_coil_no, string order_inv, string reason_last_change, string app)
        //{
        //    sdipdbEntities db = ContextHelper.SDIPDBContext;

        //    ObjectParameter Output = new ObjectParameter("return_cd", typeof(string));


        //    db.cs_defect_entry(production_coil_no, user_id, type, mode, defect_cd, unit_detected, defect_comment, inv_or_hold, order_no, line_item_no, cons_coil_no, order_inv, reason_last_change, app, Output);

        //}

        public static int spc_up_seq_no(string system_sequence_cd)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            ObjectParameter Output = new ObjectParameter("next_sequence_no_p", typeof(int));

            db.spc_up_seq_no(system_sequence_cd, Output);

            return Convert.ToInt32(Output.Value);

        }

        public static void usp_sw_email_shipment_paperwork(int load_id, int user_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            db.usp_sw_email_shipment_paperwork(load_id, user_id);

        }

        public static int usp_change_coil_status(string production_coil_no, string coil_type, string new_type, string coil_status, string new_coil_status, int? user_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            ObjectParameter return_message = new ObjectParameter("return_message", typeof(string));
            ObjectParameter return_result = new ObjectParameter("return_result", typeof(int));

            db.usp_change_coil_status(production_coil_no, coil_type, new_type, coil_status, new_coil_status, user_id, "", return_message, return_result);

            return Convert.ToInt32(return_result.Value);

        }

        public static void usp_coil_reauthorization(string old_coil_no, int user_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            db.usp_coil_reauthorization(old_coil_no, user_id);

        }

        public static List<usp_scanware_coil_alias_get_Result> GetCoilAliases(string alias)
        {
            if (alias.Length > 0)
            {
                sdipdbEntities db = ContextHelper.SDIPDBContext;

                return db.usp_scanware_coil_alias_get(alias).ToList();
            }

            return null;
        }

        public static string GetNoOrderCoilTag(string production_coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.usp_scanware_NoOrderCoilTag_get(production_coil_no).FirstOrDefault().template;
        }
    }
}