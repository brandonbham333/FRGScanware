using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using Scanware.Data;

namespace Scanware.Models
{
    public class PaintModel
    {
        public string Error { get; set; }
        public string Message { get; set; }
        public int? searched_location_cd { get; set; }
        public List<paint_location> paint_locations { get; set; }
        public paint_location current_paint_location { get; set; }
        public sw_paint_receiving  current_paint_receiving { get; set; }
        public paint_inventory current_paint_inventory { get; set; }
        public string current_paint_container { get; set; }
        public paint_barcode current_paint_barcode { get; set; }
        public string location { get; set; }
        public string current_inventory_status { get; set; }

        public int paint_batch_total { get; set; }

        public List<int> paint_batch_range { get; set; }

    }
}