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
    
    public partial class user_info
    {
        public user_info()
        {
            this.application_security = new HashSet<application_security>();
        }
    
        public string user_name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email_address { get; set; }
        public string active { get; set; }
        public System.DateTime change_datetime { get; set; }
        public int change_user_id { get; set; }
        public Nullable<short> user_type_cd { get; set; }
        public Nullable<System.DateTime> trg_change_datetime { get; set; }
        public string coil_status_security_template { get; set; }
    
        public virtual ICollection<application_security> application_security { get; set; }
    }
}
