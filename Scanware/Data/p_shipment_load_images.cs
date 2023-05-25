using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class shipment_load_images
    {
        public static List<shipment_load_images> GetLoadImages(int load_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            List<shipment_load_images> load_images = new List<shipment_load_images>();

            var shipment_load_images = from sl in db.shipment_load_images
                                       where sl.load_id == load_id  
                                       select sl;

            foreach (shipment_load_images image in shipment_load_images.OrderBy(x => x.add_datetime))
            {
                load_images.Add(image);
            }

            return load_images;
        }


        public static shipment_load_images GetLoadImage(int image_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.shipment_load_images.Where(x => x.image_no == image_no).FirstOrDefault();
        }

        public string add_user_name
        {
            get
            {

                sdipdbEntities db = ContextHelper.SDIPDBContext;

                int add_user_id;

                if (this.add_user_id == null)
                {
                    add_user_id = -1;
                }
                else
                {
                    add_user_id = Convert.ToInt32(this.add_user_id);
                }

                application_security current_application_security = application_security.GetApplicationSecurity(add_user_id);


                if (current_application_security != null)
                {
                    return current_application_security.user_name;
                }
                else
                {
                    return "";
                }

            }
        }

        public byte[] image_data
        {
            get
            {

                sdipdbEntities db = ContextHelper.SDIPDBContext;

                v_shipment_load_images image_bytes = v_shipment_load_images.GetLoadImage(this.image_no);

                return image_bytes.image_data;

                //if (image_bytes != null)
                //{
                //    return image_bytes.image_data;
                //}
                //else
                //{
                //    return image_data;
                //}

            }
        }


    }
}