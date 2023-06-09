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
    
    public partial class customer_ship_to
    {
        public int customer_id { get; set; }
        public string ship_to_location_name { get; set; }
        public string street_1 { get; set; }
        public string street_2 { get; set; }
        public string po_box { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip_code { get; set; }
        public string country { get; set; }
        public string contact_name { get; set; }
        public string contact_title { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string carrier_mode { get; set; }
        public Nullable<short> delivering_carrier_cd { get; set; }
        public string delivering_carrier_mode { get; set; }
        public string spec_instruct_receiving { get; set; }
        public string spec_instruct_mark { get; set; }
        public string spec_instruct_packing { get; set; }
        public string spec_instruct_loading { get; set; }
        public string spec_instruct_shipping { get; set; }
        public string spec_instruct_unloading { get; set; }
        public Nullable<float> shipping_time_required { get; set; }
        public Nullable<System.DateTime> add_datetime { get; set; }
        public Nullable<int> add_user_id { get; set; }
        public Nullable<System.DateTime> change_datetime { get; set; }
        public Nullable<int> change_user_id { get; set; }
        public Nullable<int> to_freight_location_cd { get; set; }
        public string profile { get; set; }
        public string equipment { get; set; }
        public string active { get; set; }
        public Nullable<int> shipping_contact_id { get; set; }
        public string edi_destination { get; set; }
        public string duns_no { get; set; }
        public string contract { get; set; }
        public string endorsement { get; set; }
        public string freight_payer_name { get; set; }
        public string freight_payer_street_1 { get; set; }
        public string freight_payer_street_2 { get; set; }
        public string freight_payer_city { get; set; }
        public string freight_payer_state { get; set; }
        public string freight_payer_zip_code { get; set; }
        public string freight_payer_po_box { get; set; }
        public string alternate_freight_billing { get; set; }
        public string freight_payer_country { get; set; }
        public string rule_11 { get; set; }
        public Nullable<int> receiver_id { get; set; }
        public int shiptoid { get; set; }
        public Nullable<short> sales_rep_cd { get; set; }
        public string tmp_finaluofm { get; set; }
        public Nullable<int> loading_code_id { get; set; }
        public string bol_email { get; set; }
        public string proforma_email { get; set; }
        public string ship_to_location_name_long { get; set; }
        public Nullable<int> rail_to_freight_location_cd { get; set; }
        public string asn_pt { get; set; }
        public string asn_sc { get; set; }
        public string hfrd_address_code { get; set; }
        public string hfrd_customer_group { get; set; }
        public string hfrd_key { get; set; }
        public string hfrd_ship_method { get; set; }
        public string hfrd_SYS_USER_DEFINED_1 { get; set; }
        public Nullable<int> tech_custSeq { get; set; }
        public string op_instr_duns_no { get; set; }
        public string hfrd_OraUpd { get; set; }
        public string copy_856 { get; set; }
        public string rfc_tax_id { get; set; }
        public System.DateTime trg_change_datetime { get; set; }
        public string asn_customer { get; set; }
        public string asn_destination { get; set; }
    }
}
