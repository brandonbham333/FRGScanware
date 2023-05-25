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

    public class HomeModel
    {
        public user_info user_info { get; set; }
        public string IPAddress { get; set; }
        public string ComputerName { get; set; }
        public string Application { get; set; }
        public string ApplicationVersion { get; set; }
        public application CurrentApplication { get; set; }

        public string Error { get; set; }
        public string Message { get; set; }
        public List<printer> zebra_printers { get; set; }
        public List<printer> network_printers { get; set; }
        public printer selected_zebra_printer { get; set; }
        public printer selected_network_printer { get; set; }

        public List<from_freight_locations> from_freight_locations { get; set; }
        public from_freight_locations selected_from_freight_locations { get; set; }

        public string user_initials { get; set; }

        public string[] selected_bays { get; set; }

        public List<coil_yard_bays> CoilYardBays { get; set; }

        public string Location { get; set; }
        public string Database { get; set; }

        public bool isRailUser { get; set; }

        public string default_zinc_line { get; set; }
        public bool isSelectedBay(string bay)
        {
            if (selected_bays != null)
            {
                return selected_bays.Contains(bay);
            }
            else
                return false;
        }

    }

}
