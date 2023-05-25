using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using Scanware.Data;
using System.Linq;

namespace Scanware.Models
{
    public class InventoryModel
    {
        public string Error { get; set; }
        public string Alert { get; set; }
        public string Message { get; set; }

        public List<coil_yard_columns> CoilYardCols { get; set; }
        public List<coil_yard_rows> CoilYardRows { get; set; }
        public List<coil_yard_bays> CoilYardBays { get; set; }

        public string current_column { get; set; }
        public string current_row { get; set; }
        public coil_yard_locations current_coil_yard_location { get; set; }
        public coil_yard_locations new_coil_yard_location { get; set; }
        
        public coil_yard_bays current_coil_yard_bay { get; set; }
        public coil current_coil { get; set; }
        public List<CoilsInLocation> coilsInLocation { get; set; }
        public all_produced_coils current_all_produced_coil { get; set; }


    }
}