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
    
    public partial class load_dtl
    {
        public int load_id { get; set; }
        public int order_no { get; set; }
        public short line_item_no { get; set; }
        public short line_item_coil_no { get; set; }
        public Nullable<System.DateTime> add_datetime { get; set; }
        public Nullable<int> add_user_id { get; set; }
        public Nullable<System.DateTime> change_datetime { get; set; }
        public Nullable<int> change_user_id { get; set; }
        public string invoiced { get; set; }
        public string production_coil_no { get; set; }
        public Nullable<int> coil_weight { get; set; }
        public Nullable<System.DateTime> coil_scanned_dt { get; set; }
        public Nullable<System.DateTime> doc_scanned_dt { get; set; }
        public bool shipment_recieved { get; set; }
        public long load_dtl_pk { get; set; }
        public Nullable<int> cert_nbr { get; set; }
        public System.DateTime BeginDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public Nullable<System.DateTime> trg_change_datetime { get; set; }
        public Nullable<int> cert_no { get; set; }
        public string frg_production_coil_no { get; set; }
        public string PEDIMENTO { get; set; }
    }
}
