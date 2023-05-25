using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class shipment_load_signature
    {
        public static void InsertShipmentLoadSignature(int load_id, byte[] signature_image )
        {

            shipment_load_signature sls = new shipment_load_signature()
            {
                load_id = load_id,
                signature_image = signature_image,
                add_datetime = DateTime.Now
            };

            sdipdbEntities db = ContextHelper.SDIPDBContext;
            db.shipment_load_signature.Add(sls);
            db.SaveChanges();

        }

        public static shipment_load_signature GetShipmentLoadSignature(int load_id)
        {

            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.shipment_load_signature.FirstOrDefault(x => x.load_id == load_id);
            
        }


        public static bool SignaureExists(int load_id)
        {

            sdipdbEntities db = ContextHelper.SDIPDBContext;

            int ct  = db.shipment_load_signature.Where(x => x.load_id == load_id).Count();

            if (ct > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }



        public static void RemoveShipmentLoadSignature(int load_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            var signature = new shipment_load_signature { load_id = load_id };
            db.shipment_load_signature.Attach(signature);
            db.shipment_load_signature.Remove(signature);
            db.SaveChanges();


        }


    }
}