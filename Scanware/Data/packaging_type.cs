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
    
    public partial class packaging_type
    {
        public short packaging_type_cd { get; set; }
        public string description { get; set; }
        public Nullable<System.DateTime> change_datetime { get; set; }
        public Nullable<int> change_user_id { get; set; }
        public string simplified_instructions { get; set; }
        public string active { get; set; }
        public string level_2_code { get; set; }
        public Nullable<byte> circ_bands { get; set; }
        public Nullable<byte> eye_bands { get; set; }
        public string line_package { get; set; }
        public string further_package { get; set; }
        public string eye_to_the_sky { get; set; }
        public Nullable<decimal> band_width { get; set; }
        public Nullable<int> additional_weight { get; set; }
        public string internal_external { get; set; }
    }
}