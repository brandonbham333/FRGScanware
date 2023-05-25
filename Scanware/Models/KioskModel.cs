using Scanware.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Scanware.Models
{
    public class KioskModel : ShippingModel
    {
        public string locale_code { get; set; }
        public IEnumerable<location> ship_to_locations { get; set; }
        public int loadcard_no { get; set; }
        public int PickUp_no { get; set; }
    }
}
