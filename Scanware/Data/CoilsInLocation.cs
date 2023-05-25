using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public class CoilsInLocation
    {
        public string production_coil_no { get; set; }
        public string status { get; set; }

    }

    public class LoadsAndCoils
    {
        public int load_id { get; set; }
        public int? pickup_no { get; set; }
        public string vehicle_no { get; set; }
        public DateTime scale_time_in { get; set; }
        public List<CoilsInLoad> coils { get; set; }
    }

    public class CoilsInLoad
    {
        public string production_coil_no { get; set; }
        public string coilShipTag { get; set; }
        public string coilOnPaperwork { get; set; }
        public DateTime verified { get; set; }
        public string Message { get; set; }

        public int coilWeight { get; set; }

        public string coilLocation { get; set; }

        public static bool GetLoadDtlCoilsStatus(List<CoilsInLoad> coils)
        {

            bool load_verified = true;

            foreach (var coil in coils)
            {

                if ((coil.coilOnPaperwork == null) || (coil.coilShipTag == null))
                {
                    load_verified = false;
                    return load_verified;
                }
            }

            return load_verified;

        }
    }

    
}