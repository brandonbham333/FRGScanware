using Scanware.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Models
{
    public class AdminModel
    {
        public List<rail_cars> RailCars { get; set; }
        public rail_cars RailCar { get; set; }
        public string Error { get; set; }
        public string Message { get; set; }
        public List<scanware_hold_coil_email> HoldEmails { get; set; }
        public List<product_processors> InsideProductProcessors { get; set; }

        public List<paint_location> paint_locations { get; set; }
        public paint_location current_paint_location { get; set; }
        public printer default_zebra_printer { get; set; }
        public bool set_max_weight { get; set; }
    }
}