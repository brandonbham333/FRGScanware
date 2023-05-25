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
    
    public partial class claim_reason
    {
        public short claim_reason_cd { get; set; }
        public string description { get; set; }
        public Nullable<System.DateTime> change_datetime { get; set; }
        public Nullable<int> change_user_id { get; set; }
        public string defect_cd { get; set; }
        public string hot_mill_claim { get; set; }
        public string pickle_claim { get; set; }
        public string galv1_claim { get; set; }
        public string galv2_claim { get; set; }
        public string cold_mill_claim { get; set; }
        public string annealing_claim { get; set; }
        public string temper_mill_claim { get; set; }
        public string metalsite_reason { get; set; }
        public string include_if_metalsite_excess { get; set; }
        public string include_if_metalsite_secondary { get; set; }
        public string sales_approv_req { get; set; }
        public string quality_approv_req { get; set; }
        public string bonus_related { get; set; }
        public string defect_type { get; set; }
        public string galv3_claim { get; set; }
        public string paint_claim { get; set; }
        public string m_c_fault { get; set; }
        public string paint2_claim { get; set; }
        public string galvalume_claim { get; set; }
        public string slitting_claim { get; set; }
        public string off_order_notifier_exclusion { get; set; }
        public string leveling_claim { get; set; }
        public string hold_specifications { get; set; }
        public string defect_owner { get; set; }
        public string scanware_hold { get; set; }
        public Nullable<System.DateTime> trg_change_datetime { get; set; }
        public string hold_status { get; set; }
    }
}
