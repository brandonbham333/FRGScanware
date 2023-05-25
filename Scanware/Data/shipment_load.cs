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
    
    public partial class shipment_load
    {
        public int load_id { get; set; }
        public string carrier_mode { get; set; }
        public Nullable<short> carrier_cd { get; set; }
        public string vehicle_no { get; set; }
        public Nullable<System.DateTime> scale_time_in { get; set; }
        public Nullable<System.DateTime> scale_time_out { get; set; }
        public Nullable<int> scale_weight_in { get; set; }
        public Nullable<int> scale_weight_out { get; set; }
        public string load_status { get; set; }
        public Nullable<System.DateTime> shipped_date { get; set; }
        public string door_id { get; set; }
        public Nullable<System.DateTime> schedule_date { get; set; }
        public Nullable<System.DateTime> schedule_time { get; set; }
        public Nullable<int> total_weight_load { get; set; }
        public Nullable<int> customer_id { get; set; }
        public Nullable<short> from_freight_location_cd { get; set; }
        public Nullable<short> to_freight_location_cd { get; set; }
        public Nullable<System.DateTime> add_datetime { get; set; }
        public Nullable<int> add_user_id { get; set; }
        public Nullable<System.DateTime> change_datetime { get; set; }
        public Nullable<int> change_user_id { get; set; }
        public string permit_number { get; set; }
        public string driver_name { get; set; }
        public string est_weight_flag { get; set; }
        public string initial_in { get; set; }
        public string initial_out { get; set; }
        public string invoiced { get; set; }
        public string shipping_freight_hold { get; set; }
        public Nullable<decimal> shipping_freight_cost { get; set; }
        public Nullable<int> master_load_id { get; set; }
        public Nullable<decimal> shipping_freight_rate { get; set; }
        public string comment { get; set; }
        public string carrier_edi_sent { get; set; }
        public string manual_weight_in { get; set; }
        public string manual_weight_out { get; set; }
        public Nullable<System.DateTime> schedule_datetime { get; set; }
        public string prepaid_collect_flag { get; set; }
        public string edi_sent { get; set; }
        public Nullable<System.DateTime> edi_datetime { get; set; }
        public string release_no { get; set; }
        public string carrier_ppd_collect_flag { get; set; }
        public string customer_truck_name { get; set; }
        public string ShipmentType { get; set; }
        public Nullable<int> FromShipToID { get; set; }
        public Nullable<int> ToShipToID { get; set; }
        public string char_load_id { get; set; }
        public string processor_bol { get; set; }
        public string rail_car_type { get; set; }
        public string rail_route { get; set; }
        public Nullable<System.DateTime> print_datetime { get; set; }
        public string stage_rail { get; set; }
        public Nullable<int> rail_route_gross_weight { get; set; }
        public string customer_pick_up_description { get; set; }
        public Nullable<System.DateTime> loading_start_datetime { get; set; }
        public Nullable<System.DateTime> loading_end_datetime { get; set; }
        public Nullable<int> loading_start_user { get; set; }
        public Nullable<int> loading_end_user { get; set; }
        public System.DateTime BeginDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public Nullable<System.DateTime> trg_change_datetime { get; set; }
        public string embargo_permit_number { get; set; }
        public string embargo_number { get; set; }
        public Nullable<byte> ic_division { get; set; }
        public Nullable<int> ic_load_id { get; set; }
        public Nullable<int> PickUp_no { get; set; }
        public string PEDIMENTO { get; set; }
        public Nullable<System.DateTime> PEDIMENTO_DT { get; set; }
        public Nullable<byte> rail_car_number { get; set; }
    
        public virtual carrier carrier { get; set; }
    }
}
