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
    
    public partial class zinc_tracking
    {
        public string ingot_id { get; set; }
        public Nullable<int> weight { get; set; }
        public string status_cd { get; set; }
        public Nullable<System.DateTime> add_datetime { get; set; }
        public Nullable<int> add_user_id { get; set; }
        public Nullable<System.DateTime> change_datetime { get; set; }
        public Nullable<int> change_user_id { get; set; }
        public Nullable<byte> line_consumed { get; set; }
        public Nullable<System.DateTime> consumed_datetime { get; set; }
        public Nullable<int> consumed_user_id { get; set; }
    }
}
