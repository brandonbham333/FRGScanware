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
    
    public partial class rail_cars
    {
        public string vehicle_no { get; set; }
        public Nullable<int> empty_weight { get; set; }
        public string status { get; set; }
        public string permanent_flag { get; set; }
        public Nullable<System.DateTime> weight_in_datetime { get; set; }
        public Nullable<System.DateTime> change_datetime { get; set; }
        public Nullable<int> change_user_id { get; set; }
        public Nullable<int> max_weight_limit { get; set; }
    }
}
