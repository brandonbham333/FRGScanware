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
    
    public partial class warehoused_coil_product_data
    {
        public string production_coil_no { get; set; }
        public Nullable<int> coil_weight { get; set; }
        public Nullable<decimal> coil_width { get; set; }
        public Nullable<decimal> coil_gauge { get; set; }
        public Nullable<int> coil_length { get; set; }
        public string vendor_load_id { get; set; }
        public string po_number { get; set; }
        public string part_no { get; set; }
        public Nullable<System.DateTime> received_datetime { get; set; }
        public string notes { get; set; }
        public Nullable<System.DateTime> expected_arrival_date { get; set; }
        public string expected_incoming_transportation_mode { get; set; }
        public Nullable<System.DateTime> add_datetime { get; set; }
        public Nullable<int> add_user_id { get; set; }
        public Nullable<System.DateTime> change_datetime { get; set; }
        public Nullable<int> change_user_id { get; set; }
    }
}