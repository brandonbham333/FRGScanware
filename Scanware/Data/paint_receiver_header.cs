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
    
    public partial class paint_receiver_header
    {
        public int receiver_no { get; set; }
        public string validated { get; set; }
        public Nullable<int> received_qty { get; set; }
        public Nullable<System.DateTime> date_in { get; set; }
        public string batch_number { get; set; }
        public string location_code { get; set; }
        public string po_number { get; set; }
        public Nullable<int> po_line_item { get; set; }
        public string paint_cd { get; set; }
        public string account_code { get; set; }
        public Nullable<int> change_user_id { get; set; }
        public Nullable<System.DateTime> change_datetime { get; set; }
        public Nullable<int> req_no { get; set; }
        public string bill_of_lading { get; set; }
        public string paid { get; set; }
        public Nullable<System.DateTime> paid_datetime { get; set; }
    }
}
