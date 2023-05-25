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
    
    public partial class warehoused_shipment_load
    {
        public int load_id { get; set; }
        public string carrier_mode { get; set; }
        public Nullable<short> carrier_cd { get; set; }
        public string vehicle_no { get; set; }
        public string load_status { get; set; }
        public Nullable<System.DateTime> shipped_date { get; set; }
        public Nullable<System.DateTime> scheduled_date { get; set; }
        public Nullable<int> total_weight_load { get; set; }
        public Nullable<int> customer_id { get; set; }
        public Nullable<short> from_freight_location_cd { get; set; }
        public Nullable<short> to_freight_location_cd { get; set; }
        public string carrier_edi_sent { get; set; }
        public string char_load_id { get; set; }
        public string ShipmentType { get; set; }
        public string comment { get; set; }
        public string initial_in { get; set; }
        public string initial_out { get; set; }
        public string manual_weight_in { get; set; }
        public string manual_weight_out { get; set; }
        public Nullable<int> scale_weight_in { get; set; }
        public Nullable<int> scale_weight_out { get; set; }
        public Nullable<System.DateTime> scale_time_in { get; set; }
        public Nullable<System.DateTime> scale_time_out { get; set; }
        public string PrepaidOrCollect { get; set; }
        public Nullable<System.DateTime> add_datetime { get; set; }
        public Nullable<int> add_user_id { get; set; }
        public Nullable<System.DateTime> change_datetime { get; set; }
        public Nullable<int> change_user_id { get; set; }
        public Nullable<int> master_load_id { get; set; }
    }
}
