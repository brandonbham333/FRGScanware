using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class rail_car_brand
    {
        
        public static List<rail_car_brand> GetAllRailCarBrands()
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.rail_car_brand.OrderBy(x => x.brand).ToList();
        }

        public static bool VerifyCarrierRailCar(int carrier_cd, string rail_car_brand)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;
            bool valid = true;
            if (carrier_cd != 258)
            {
                try
                {
                    var railCar = db.rail_car_brand.FirstOrDefault(c => c.brand == rail_car_brand && c.carrier_cds != null);

                    if (railCar != null)
                    {
                        var carrier_cds_split = railCar.carrier_cds.Split(',').ToList();
                        valid = carrier_cds_split.Any(c => c == carrier_cd.ToString());
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            

            return valid;
        }
    }
}