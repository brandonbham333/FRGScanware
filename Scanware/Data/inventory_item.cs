//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class inventory_item
    {
        public string production_coil_no { get; set; }
        public string inventory_reason_cd { get; set; }
        public Nullable<int> last_order_no { get; set; }
        public Nullable<short> last_line_item_no { get; set; }
        public Nullable<System.DateTime> change_datetime { get; set; }
        public Nullable<int> change_user_id { get; set; }
        public Nullable<decimal> price { get; set; }
        public string ms_batch_id { get; set; }
        public string ms_class_of_product { get; set; }
        public string ms_metalsite_reason { get; set; }
        public string ms_seller_defined_reason_code { get; set; }
        public string ms_seller_defined_reason_desc { get; set; }
        public string ms_expiration_date { get; set; }
        public string ms_product_description { get; set; }
        public string ms_processing_description { get; set; }
        public string ms_url { get; set; }
        public string ms_image_name { get; set; }
        public Nullable<decimal> ms_item_price { get; set; }
        public string ms_version { get; set; }
        public string ms_family { get; set; }
        public string ms_product_type { get; set; }
        public string ms_brand_name { get; set; }
        public Nullable<decimal> ms_gauge { get; set; }
        public string ms_weight_unit_of_measure { get; set; }
        public Nullable<decimal> ms_weight { get; set; }
        public Nullable<decimal> ms_width { get; set; }
        public Nullable<decimal> ms_lft_length { get; set; }
        public Nullable<decimal> ms_inside_diameter { get; set; }
        public Nullable<decimal> ms_outside_diameter { get; set; }
        public string ms_heat { get; set; }
        public string ms_grade { get; set; }
        public string ms_quality { get; set; }
        public string ms_astm_number { get; set; }
        public string ms_finish { get; set; }
        public string ms_coating { get; set; }
        public string ms_treatment { get; set; }
        public string ms_temper { get; set; }
        public string ms_pickling { get; set; }
        public string ms_seller_defined_1 { get; set; }
        public string ms_seller_defined_2 { get; set; }
        public string ms_seller_defined_3 { get; set; }
        public string ms_seller_defined_4 { get; set; }
        public string ms_seller_defined_5 { get; set; }
        public string ms_transfer_status { get; set; }
        public Nullable<System.DateTime> ms_post_date { get; set; }
        public string ms_commerce_type { get; set; }
        public string ms_product_num { get; set; }
        public string ms_status_num { get; set; }
        public string ms_status_num_desc { get; set; }
        public Nullable<decimal> sfi_price_cwt { get; set; }
        public Nullable<short> sfi_product_type_cd { get; set; }
        public string sfi_material_specification_cd { get; set; }
        public string product_type { get; set; }
        public Nullable<System.DateTime> available_to_sfi { get; set; }
        public string frg_production_coil_no { get; set; }
        public Nullable<System.DateTime> trg_change_datetime { get; set; }
        public string last_part_no { get; set; }
    
        public virtual all_produced_coils all_produced_coils { get; set; }
    }
}