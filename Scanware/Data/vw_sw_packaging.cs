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
    
    public partial class vw_sw_packaging
    {
        public string production_coil_no { get; set; }
        public string column { get; set; }
        public string row { get; set; }
        public string facility_cd { get; set; }
        public string description { get; set; }
        public string line_package { get; set; }
        public string further_package { get; set; }
        public Nullable<System.DateTime> produced_dt_stamp { get; set; }
        public string carrier_mode { get; set; }
        public string coil_status { get; set; }
        public Nullable<System.DateTime> coil_scanned_dt { get; set; }
    }
}