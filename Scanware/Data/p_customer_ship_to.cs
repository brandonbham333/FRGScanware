using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class customer_ship_to
    {

        public static customer_ship_to GetCustomerShipTo(int customer_id, string ship_to_location_name)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            customer_ship_to cst = db.customer_ship_to.SingleOrDefault(x => x.customer_id == customer_id && x.ship_to_location_name == ship_to_location_name);

            return cst;
        }

        public static customer_ship_to GetCustomerShipToByShipToID(int ToShipToID)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            customer_ship_to cst = db.customer_ship_to.SingleOrDefault(x => x.shiptoid == ToShipToID);

            return cst;
        }

        public static customer_ship_to GetCustomerShipToByFromLoc(int ToFreightLocationCode)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            customer_ship_to cst = db.customer_ship_to.SingleOrDefault(x => x.shiptoid == 11);

            return cst;
        }

        public static bool IsCustomerShipToInCAorMX(int ToShipToID)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            var cstRecord = db.customer_ship_to.FirstOrDefault(x => x.shiptoid == ToShipToID);

            if (cstRecord != null &&
                (cstRecord.country.ToUpper() == "MEXICO" || cstRecord.country.ToUpper() == "CANADA"))
            {
                return true;
            }

            return false;
        }
    }
}