﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Scanware.Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    
    public partial class sdipdbEntities : DbContext
    {
        public sdipdbEntities()
            : base("name=sdipdbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<ref_sys_param> ref_sys_param { get; set; }
        public DbSet<application_security> application_security { get; set; }
        public DbSet<function_level_description> function_level_description { get; set; }
        public DbSet<function_level_security> function_level_security { get; set; }
        public DbSet<user_info> user_info { get; set; }
        public DbSet<application> applications { get; set; }
        public DbSet<coil> coils { get; set; }
        public DbSet<customer_order> customer_order { get; set; }
        public DbSet<customer_order_line_item> customer_order_line_item { get; set; }
        public DbSet<hb_setup> hb_setup { get; set; }
        public DbSet<customer> customers { get; set; }
        public DbSet<product_type> product_type { get; set; }
        public DbSet<all_produced_coils> all_produced_coils { get; set; }
        public DbSet<customer_ship_to> customer_ship_to { get; set; }
        public DbSet<coil_yard_columns> coil_yard_columns { get; set; }
        public DbSet<coil_yard_locations> coil_yard_locations { get; set; }
        public DbSet<coil_yard_rows> coil_yard_rows { get; set; }
        public DbSet<coil_status> coil_status { get; set; }
        public DbSet<inventory_item> inventory_item { get; set; }
        public DbSet<inventory_reason> inventory_reason { get; set; }
        public DbSet<load_dtl> load_dtl { get; set; }
        public DbSet<shipment_load> shipment_load { get; set; }
        public DbSet<packaging_type> packaging_type { get; set; }
        public DbSet<vw_sw_coil_history> vw_sw_coil_history { get; set; }
        public DbSet<printer> printers { get; set; }
        public DbSet<product_processors> product_processors { get; set; }
        public DbSet<vw_sw_scheduled_coils> vw_sw_scheduled_coils { get; set; }
        public DbSet<coil_audit_trail> coil_audit_trail { get; set; }
        public DbSet<vw_sw_packaging> vw_sw_packaging { get; set; }
        public DbSet<rail_cars> rail_cars { get; set; }
        public DbSet<vw_sw_inbound_op_coils> vw_sw_inbound_op_coils { get; set; }
        public DbSet<vw_sw_outbound_op_coils> vw_sw_outbound_op_coils { get; set; }
        public DbSet<claim_reason> claim_reason { get; set; }
        public DbSet<coil_defect> coil_defect { get; set; }
        public DbSet<paint_location> paint_location { get; set; }
        public DbSet<sw_paint_receiving> sw_paint_receiving { get; set; }
        public DbSet<sw_paint_log> sw_paint_log { get; set; }
        public DbSet<carrier> carriers { get; set; }
        public DbSet<shipment_load_signature> shipment_load_signature { get; set; }
        public DbSet<zebra_template> zebra_template { get; set; }
        public DbSet<v_zebra_template_coil> v_zebra_template_coil { get; set; }
        public DbSet<user_defaults> user_defaults { get; set; }
        public DbSet<printer_queue> printer_queue { get; set; }
        public DbSet<v_zebra_template_load_card> v_zebra_template_load_card { get; set; }
        public DbSet<scanware_hold_coil_email> scanware_hold_coil_email { get; set; }
        public DbSet<v_sw_print_rail_loads> v_sw_print_rail_loads { get; set; }
        public DbSet<shipment_load_images> shipment_load_images { get; set; }
        public DbSet<v_shipment_load_images> v_shipment_load_images { get; set; }
        public DbSet<v_apc_images> v_apc_images { get; set; }
        public DbSet<paint_type> paint_type { get; set; }
        public DbSet<v_zebra_template_paint_location> v_zebra_template_paint_location { get; set; }
        public DbSet<ba_coil_product_data> ba_coil_product_data { get; set; }
        public DbSet<application_login> application_login { get; set; }
        public DbSet<rail_car_brand> rail_car_brand { get; set; }
        public DbSet<from_freight_locations> from_freight_locations { get; set; }
        public DbSet<coil_yard_bays> coil_yard_bays { get; set; }
        public DbSet<load_dtl_bay> load_dtl_bay { get; set; }
        public DbSet<scanware_shipping_loads> scanware_shipping_loads { get; set; }
        public DbSet<vw_shipping_schedule_334> vw_shipping_schedule_334 { get; set; }
        public DbSet<v_sw_load_details> v_sw_load_details { get; set; }
        public DbSet<vw_sw_pre_check_loads> vw_sw_pre_check_loads { get; set; }
        public DbSet<sw_op_coil_validate> sw_op_coil_validate { get; set; }
        public DbSet<coil_reauthorization> coil_reauthorization { get; set; }
        public DbSet<scanware_loads_ship_after_12> scanware_loads_ship_after_12 { get; set; }
        public DbSet<zinc_tracking> zinc_tracking { get; set; }
        public DbSet<paint_coils_images> paint_coils_images { get; set; }
        public DbSet<v_paint_coils_images> v_paint_coils_images { get; set; }
        public DbSet<paint_receiver_header> paint_receiver_header { get; set; }
        public DbSet<paint_inventory> paint_inventory { get; set; }
        public DbSet<vw_sw_produced_coils> vw_sw_produced_coils { get; set; }
        public DbSet<application_settings> application_settings { get; set; }
        public DbSet<to_freight_locations> to_freight_locations { get; set; }
        public DbSet<state> states { get; set; }
        public DbSet<finish_line_setup> finish_line_setup { get; set; }
        public DbSet<warehoused_load_dtl> warehoused_load_dtl { get; set; }
        public DbSet<warehoused_shipment_load> warehoused_shipment_load { get; set; }
        public DbSet<warehoused_coil_product_data> warehoused_coil_product_data { get; set; }
        public DbSet<warehoused_inventory> warehoused_inventory { get; set; }
    
        public virtual int usp_sw_ship_load_in_lomas(string char_load_id)
        {
            var char_load_idParameter = char_load_id != null ?
                new ObjectParameter("char_load_id", char_load_id) :
                new ObjectParameter("char_load_id", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_sw_ship_load_in_lomas", char_load_idParameter);
        }
    
        public virtual int usp_sw_update_rail_car_in_lomas(string char_load_id)
        {
            var char_load_idParameter = char_load_id != null ?
                new ObjectParameter("char_load_id", char_load_id) :
                new ObjectParameter("char_load_id", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_sw_update_rail_car_in_lomas", char_load_idParameter);
        }
    
        public virtual ObjectResult<cs_change_coil_status_Result> cs_change_coil_status(string a_prod_coil, string a_coil_type, string a_new_type, string a_coil_status, string a_new_status, Nullable<int> a_user_id, string a_update_PES)
        {
            var a_prod_coilParameter = a_prod_coil != null ?
                new ObjectParameter("a_prod_coil", a_prod_coil) :
                new ObjectParameter("a_prod_coil", typeof(string));
    
            var a_coil_typeParameter = a_coil_type != null ?
                new ObjectParameter("a_coil_type", a_coil_type) :
                new ObjectParameter("a_coil_type", typeof(string));
    
            var a_new_typeParameter = a_new_type != null ?
                new ObjectParameter("a_new_type", a_new_type) :
                new ObjectParameter("a_new_type", typeof(string));
    
            var a_coil_statusParameter = a_coil_status != null ?
                new ObjectParameter("a_coil_status", a_coil_status) :
                new ObjectParameter("a_coil_status", typeof(string));
    
            var a_new_statusParameter = a_new_status != null ?
                new ObjectParameter("a_new_status", a_new_status) :
                new ObjectParameter("a_new_status", typeof(string));
    
            var a_user_idParameter = a_user_id.HasValue ?
                new ObjectParameter("a_user_id", a_user_id) :
                new ObjectParameter("a_user_id", typeof(int));
    
            var a_update_PESParameter = a_update_PES != null ?
                new ObjectParameter("a_update_PES", a_update_PES) :
                new ObjectParameter("a_update_PES", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<cs_change_coil_status_Result>("cs_change_coil_status", a_prod_coilParameter, a_coil_typeParameter, a_new_typeParameter, a_coil_statusParameter, a_new_statusParameter, a_user_idParameter, a_update_PESParameter);
        }
    
        public virtual int cs_defect_entry(string a_production_coil, Nullable<int> a_user, string a_type, string a_mode, string a_defect_cd, string a_unit_detect, string a_defect_comment, string a_inv_or_hold, Nullable<int> a_order, Nullable<int> a_line_item, Nullable<int> a_cons_coil, string a_order_inv, string a_reason_last_change, string a_app, ObjectParameter return_cd)
        {
            var a_production_coilParameter = a_production_coil != null ?
                new ObjectParameter("a_production_coil", a_production_coil) :
                new ObjectParameter("a_production_coil", typeof(string));
    
            var a_userParameter = a_user.HasValue ?
                new ObjectParameter("a_user", a_user) :
                new ObjectParameter("a_user", typeof(int));
    
            var a_typeParameter = a_type != null ?
                new ObjectParameter("a_type", a_type) :
                new ObjectParameter("a_type", typeof(string));
    
            var a_modeParameter = a_mode != null ?
                new ObjectParameter("a_mode", a_mode) :
                new ObjectParameter("a_mode", typeof(string));
    
            var a_defect_cdParameter = a_defect_cd != null ?
                new ObjectParameter("a_defect_cd", a_defect_cd) :
                new ObjectParameter("a_defect_cd", typeof(string));
    
            var a_unit_detectParameter = a_unit_detect != null ?
                new ObjectParameter("a_unit_detect", a_unit_detect) :
                new ObjectParameter("a_unit_detect", typeof(string));
    
            var a_defect_commentParameter = a_defect_comment != null ?
                new ObjectParameter("a_defect_comment", a_defect_comment) :
                new ObjectParameter("a_defect_comment", typeof(string));
    
            var a_inv_or_holdParameter = a_inv_or_hold != null ?
                new ObjectParameter("a_inv_or_hold", a_inv_or_hold) :
                new ObjectParameter("a_inv_or_hold", typeof(string));
    
            var a_orderParameter = a_order.HasValue ?
                new ObjectParameter("a_order", a_order) :
                new ObjectParameter("a_order", typeof(int));
    
            var a_line_itemParameter = a_line_item.HasValue ?
                new ObjectParameter("a_line_item", a_line_item) :
                new ObjectParameter("a_line_item", typeof(int));
    
            var a_cons_coilParameter = a_cons_coil.HasValue ?
                new ObjectParameter("a_cons_coil", a_cons_coil) :
                new ObjectParameter("a_cons_coil", typeof(int));
    
            var a_order_invParameter = a_order_inv != null ?
                new ObjectParameter("a_order_inv", a_order_inv) :
                new ObjectParameter("a_order_inv", typeof(string));
    
            var a_reason_last_changeParameter = a_reason_last_change != null ?
                new ObjectParameter("a_reason_last_change", a_reason_last_change) :
                new ObjectParameter("a_reason_last_change", typeof(string));
    
            var a_appParameter = a_app != null ?
                new ObjectParameter("a_app", a_app) :
                new ObjectParameter("a_app", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("cs_defect_entry", a_production_coilParameter, a_userParameter, a_typeParameter, a_modeParameter, a_defect_cdParameter, a_unit_detectParameter, a_defect_commentParameter, a_inv_or_holdParameter, a_orderParameter, a_line_itemParameter, a_cons_coilParameter, a_order_invParameter, a_reason_last_changeParameter, a_appParameter, return_cd);
        }
    
        public virtual int usp_add_coil_image(string production_coil_no, byte[] image_data)
        {
            var production_coil_noParameter = production_coil_no != null ?
                new ObjectParameter("production_coil_no", production_coil_no) :
                new ObjectParameter("production_coil_no", typeof(string));
    
            var image_dataParameter = image_data != null ?
                new ObjectParameter("image_data", image_data) :
                new ObjectParameter("image_data", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_add_coil_image", production_coil_noParameter, image_dataParameter);
        }
    
        public virtual int spc_up_seq_no(string system_sequence_cd_p, ObjectParameter next_sequence_no_p)
        {
            var system_sequence_cd_pParameter = system_sequence_cd_p != null ?
                new ObjectParameter("system_sequence_cd_p", system_sequence_cd_p) :
                new ObjectParameter("system_sequence_cd_p", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spc_up_seq_no", system_sequence_cd_pParameter, next_sequence_no_p);
        }
    
        public virtual int usp_change_coil_status(string production_coil_no, string coil_type, string new_type, string coil_status, string new_status, Nullable<int> user_id, string update_pes, ObjectParameter return_message, ObjectParameter return_result)
        {
            var production_coil_noParameter = production_coil_no != null ?
                new ObjectParameter("production_coil_no", production_coil_no) :
                new ObjectParameter("production_coil_no", typeof(string));
    
            var coil_typeParameter = coil_type != null ?
                new ObjectParameter("coil_type", coil_type) :
                new ObjectParameter("coil_type", typeof(string));
    
            var new_typeParameter = new_type != null ?
                new ObjectParameter("new_type", new_type) :
                new ObjectParameter("new_type", typeof(string));
    
            var coil_statusParameter = coil_status != null ?
                new ObjectParameter("coil_status", coil_status) :
                new ObjectParameter("coil_status", typeof(string));
    
            var new_statusParameter = new_status != null ?
                new ObjectParameter("new_status", new_status) :
                new ObjectParameter("new_status", typeof(string));
    
            var user_idParameter = user_id.HasValue ?
                new ObjectParameter("user_id", user_id) :
                new ObjectParameter("user_id", typeof(int));
    
            var update_pesParameter = update_pes != null ?
                new ObjectParameter("update_pes", update_pes) :
                new ObjectParameter("update_pes", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_change_coil_status", production_coil_noParameter, coil_typeParameter, new_typeParameter, coil_statusParameter, new_statusParameter, user_idParameter, update_pesParameter, return_message, return_result);
        }
    
        public virtual int usp_add_shipment_load_image(Nullable<int> load_id, byte[] image_data, Nullable<int> add_user_id)
        {
            var load_idParameter = load_id.HasValue ?
                new ObjectParameter("load_id", load_id) :
                new ObjectParameter("load_id", typeof(int));
    
            var image_dataParameter = image_data != null ?
                new ObjectParameter("image_data", image_data) :
                new ObjectParameter("image_data", typeof(byte[]));
    
            var add_user_idParameter = add_user_id.HasValue ?
                new ObjectParameter("add_user_id", add_user_id) :
                new ObjectParameter("add_user_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_add_shipment_load_image", load_idParameter, image_dataParameter, add_user_idParameter);
        }
    
        public virtual int usp_sw_update_status_after_move(string production_coil_no, Nullable<int> user_id)
        {
            var production_coil_noParameter = production_coil_no != null ?
                new ObjectParameter("production_coil_no", production_coil_no) :
                new ObjectParameter("production_coil_no", typeof(string));
    
            var user_idParameter = user_id.HasValue ?
                new ObjectParameter("user_id", user_id) :
                new ObjectParameter("user_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_sw_update_status_after_move", production_coil_noParameter, user_idParameter);
        }
    
        public virtual int usp_call_spc_set_load_shipped_v2(Nullable<int> load_id)
        {
            var load_idParameter = load_id.HasValue ?
                new ObjectParameter("load_id", load_id) :
                new ObjectParameter("load_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_call_spc_set_load_shipped_v2", load_idParameter);
        }
    
        public virtual ObjectResult<sdi_galv_sched_Result> sdi_galv_sched(Nullable<byte> galvanizing_line)
        {
            var galvanizing_lineParameter = galvanizing_line.HasValue ?
                new ObjectParameter("galvanizing_line", galvanizing_line) :
                new ObjectParameter("galvanizing_line", typeof(byte));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sdi_galv_sched_Result>("sdi_galv_sched", galvanizing_lineParameter);
        }
    
        public virtual int usp_sw_email_shipment_paperwork(Nullable<int> load_id, Nullable<int> user_id)
        {
            var load_idParameter = load_id.HasValue ?
                new ObjectParameter("load_id", load_id) :
                new ObjectParameter("load_id", typeof(int));
    
            var user_idParameter = user_id.HasValue ?
                new ObjectParameter("user_id", user_id) :
                new ObjectParameter("user_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_sw_email_shipment_paperwork", load_idParameter, user_idParameter);
        }
    
        public virtual int usp_coil_reauthorization(string old_coil_no, Nullable<int> user_id)
        {
            var old_coil_noParameter = old_coil_no != null ?
                new ObjectParameter("old_coil_no", old_coil_no) :
                new ObjectParameter("old_coil_no", typeof(string));
    
            var user_idParameter = user_id.HasValue ?
                new ObjectParameter("user_id", user_id) :
                new ObjectParameter("user_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_coil_reauthorization", old_coil_noParameter, user_idParameter);
        }
    
        public virtual int usp_add_paint_coils_image(string production_coil_no, byte[] image_data, Nullable<int> add_user_id)
        {
            var production_coil_noParameter = production_coil_no != null ?
                new ObjectParameter("production_coil_no", production_coil_no) :
                new ObjectParameter("production_coil_no", typeof(string));
    
            var image_dataParameter = image_data != null ?
                new ObjectParameter("image_data", image_data) :
                new ObjectParameter("image_data", typeof(byte[]));
    
            var add_user_idParameter = add_user_id.HasValue ?
                new ObjectParameter("add_user_id", add_user_id) :
                new ObjectParameter("add_user_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_add_paint_coils_image", production_coil_noParameter, image_dataParameter, add_user_idParameter);
        }
    
        public virtual int paint_receiving(string po_number, string paint_cd, string batch_no, string bol, Nullable<int> gallons, Nullable<int> location_cd, Nullable<int> user_id, string tag_number)
        {
            var po_numberParameter = po_number != null ?
                new ObjectParameter("po_number", po_number) :
                new ObjectParameter("po_number", typeof(string));
    
            var paint_cdParameter = paint_cd != null ?
                new ObjectParameter("paint_cd", paint_cd) :
                new ObjectParameter("paint_cd", typeof(string));
    
            var batch_noParameter = batch_no != null ?
                new ObjectParameter("batch_no", batch_no) :
                new ObjectParameter("batch_no", typeof(string));
    
            var bolParameter = bol != null ?
                new ObjectParameter("bol", bol) :
                new ObjectParameter("bol", typeof(string));
    
            var gallonsParameter = gallons.HasValue ?
                new ObjectParameter("gallons", gallons) :
                new ObjectParameter("gallons", typeof(int));
    
            var location_cdParameter = location_cd.HasValue ?
                new ObjectParameter("location_cd", location_cd) :
                new ObjectParameter("location_cd", typeof(int));
    
            var user_idParameter = user_id.HasValue ?
                new ObjectParameter("user_id", user_id) :
                new ObjectParameter("user_id", typeof(int));
    
            var tag_numberParameter = tag_number != null ?
                new ObjectParameter("tag_number", tag_number) :
                new ObjectParameter("tag_number", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("paint_receiving", po_numberParameter, paint_cdParameter, batch_noParameter, bolParameter, gallonsParameter, location_cdParameter, user_idParameter, tag_numberParameter);
        }
    
        public virtual int paint_receiving2(string po_number, string paint_cd, string batch_no, string bol, Nullable<int> gallons, Nullable<int> user_id, string db_location, string drum_no)
        {
            var po_numberParameter = po_number != null ?
                new ObjectParameter("po_number", po_number) :
                new ObjectParameter("po_number", typeof(string));
    
            var paint_cdParameter = paint_cd != null ?
                new ObjectParameter("paint_cd", paint_cd) :
                new ObjectParameter("paint_cd", typeof(string));
    
            var batch_noParameter = batch_no != null ?
                new ObjectParameter("batch_no", batch_no) :
                new ObjectParameter("batch_no", typeof(string));
    
            var bolParameter = bol != null ?
                new ObjectParameter("bol", bol) :
                new ObjectParameter("bol", typeof(string));
    
            var gallonsParameter = gallons.HasValue ?
                new ObjectParameter("gallons", gallons) :
                new ObjectParameter("gallons", typeof(int));
    
            var user_idParameter = user_id.HasValue ?
                new ObjectParameter("user_id", user_id) :
                new ObjectParameter("user_id", typeof(int));
    
            var db_locationParameter = db_location != null ?
                new ObjectParameter("db_location", db_location) :
                new ObjectParameter("db_location", typeof(string));
    
            var drum_noParameter = drum_no != null ?
                new ObjectParameter("drum_no", drum_no) :
                new ObjectParameter("drum_no", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("paint_receiving2", po_numberParameter, paint_cdParameter, batch_noParameter, bolParameter, gallonsParameter, user_idParameter, db_locationParameter, drum_noParameter);
        }
    
        public virtual ObjectResult<usp_scanware_coil_alias_get_Result> usp_scanware_coil_alias_get(string alias)
        {
            var aliasParameter = alias != null ?
                new ObjectParameter("alias", alias) :
                new ObjectParameter("alias", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_scanware_coil_alias_get_Result>("usp_scanware_coil_alias_get", aliasParameter);
        }
    
        public virtual ObjectResult<usp_scanware_NoOrderCoilTag_get_Result> usp_scanware_NoOrderCoilTag_get(string coil_id)
        {
            var coil_idParameter = coil_id != null ?
                new ObjectParameter("coil_id", coil_id) :
                new ObjectParameter("coil_id", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_scanware_NoOrderCoilTag_get_Result>("usp_scanware_NoOrderCoilTag_get", coil_idParameter);
        }
    
        public virtual int call_spc_set_load_shipped_wh(Nullable<int> load_id)
        {
            var load_idParameter = load_id.HasValue ?
                new ObjectParameter("load_id", load_id) :
                new ObjectParameter("load_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("call_spc_set_load_shipped_wh", load_idParameter);
        }
    
        public virtual int shipping_master(Nullable<int> load_id, string given_coil_no, string calling_program, Nullable<int> customer_id, Nullable<bool> messaging_table_output)
        {
            var load_idParameter = load_id.HasValue ?
                new ObjectParameter("load_id", load_id) :
                new ObjectParameter("load_id", typeof(int));
    
            var given_coil_noParameter = given_coil_no != null ?
                new ObjectParameter("given_coil_no", given_coil_no) :
                new ObjectParameter("given_coil_no", typeof(string));
    
            var calling_programParameter = calling_program != null ?
                new ObjectParameter("calling_program", calling_program) :
                new ObjectParameter("calling_program", typeof(string));
    
            var customer_idParameter = customer_id.HasValue ?
                new ObjectParameter("customer_id", customer_id) :
                new ObjectParameter("customer_id", typeof(int));
    
            var messaging_table_outputParameter = messaging_table_output.HasValue ?
                new ObjectParameter("messaging_table_output", messaging_table_output) :
                new ObjectParameter("messaging_table_output", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("shipping_master", load_idParameter, given_coil_noParameter, calling_programParameter, customer_idParameter, messaging_table_outputParameter);
        }
    }
}