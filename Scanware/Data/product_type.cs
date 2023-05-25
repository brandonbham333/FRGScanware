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
    
    public partial class product_type
    {
        public short product_type_cd { get; set; }
        public string description { get; set; }
        public Nullable<short> standard_lead_time { get; set; }
        public Nullable<System.DateTime> add_datetime { get; set; }
        public Nullable<int> add_user_id { get; set; }
        public Nullable<System.DateTime> change_datetime { get; set; }
        public Nullable<int> change_user_id { get; set; }
        public string gl_account { get; set; }
        public string pes_cd { get; set; }
        public string long_description { get; set; }
        public Nullable<short> coating_type_cd { get; set; }
        public Nullable<short> surface_quality_cd { get; set; }
        public string process_cd { get; set; }
        public string routing { get; set; }
        public string short_description { get; set; }
        public string side_trimming_mode { get; set; }
        public string physical_inventory_code { get; set; }
        public Nullable<decimal> cost_per_ton { get; set; }
        public Nullable<short> counterpart { get; set; }
        public string publish_lead_time { get; set; }
        public string lead_time { get; set; }
        public Nullable<short> field_reject_product_type_cd { get; set; }
        public string backlog_grouping { get; set; }
        public Nullable<decimal> default_buildup { get; set; }
        public string tolling { get; set; }
        public string active { get; set; }
        public string email_address { get; set; }
        public string backlog_grouping_non_prime { get; set; }
        public string substrate { get; set; }
        public string stock { get; set; }
        public string web_description { get; set; }
        public Nullable<int> sort_order { get; set; }
        public Nullable<byte> booking_plan_id { get; set; }
        public string capacity_display { get; set; }
        public string claims_group { get; set; }
        public Nullable<byte> cap_pt_group_id { get; set; }
        public Nullable<int> planning_web_sort { get; set; }
        public Nullable<int> resource_id { get; set; }
        public Nullable<int> pw_default_leadtime { get; set; }
        public Nullable<System.DateTime> trg_change_datetime { get; set; }
        public Nullable<int> adjustment_product_class_cd { get; set; }
        public Nullable<int> frg_backlog_group_cd { get; set; }
        public string Capacity_Group { get; set; }
        public string spanish_description { get; set; }
        public string customs_description { get; set; }
        public string customs_package_type { get; set; }
    }
}
