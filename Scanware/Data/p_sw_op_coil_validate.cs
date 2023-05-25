using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class sw_op_coil_validate
    {
        public static List<sw_op_coil_validate> GetInvalidCoils()
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.sw_op_coil_validate.Where(x => x.is_valid == "N").ToList();
        }


        public static sw_op_coil_validate GetCoilValidate(string production_coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.sw_op_coil_validate.Where(x => x.production_coil_no == production_coil_no).FirstOrDefault();
        }





        public string tag_no
        {
            get
            {

                sdipdbEntities db = ContextHelper.SDIPDBContext;

                all_produced_coils apc = all_produced_coils.GetAllProducedCoil(this.production_coil_no);

                if (apc != null)
                {
                    return apc.tag_no;
                }
                else
                {
                    return "";
                }

            }
        }


        public string coil_yard_location
        {
            get
            {
                return all_produced_coils.GetCoilYardLocation(this.production_coil_no);  

            }
        }


        public static void UpdateMeasurements(string production_coil_no, int coil_weight, float coil_thickness, float coil_width, int change_user_id)
        {

            sdipdbEntities db = ContextHelper.SDIPDBContext;

            sw_op_coil_validate to_update = db.sw_op_coil_validate.Where(x => x.production_coil_no == production_coil_no).FirstOrDefault();

            to_update.coil_weight = coil_weight;
            to_update.coil_thickness = coil_thickness;
            to_update.coil_width = coil_width;
            to_update.is_valid = "Y";
            to_update.measurement_date = DateTime.Now;
            to_update.change_user_id = change_user_id;

            db.SaveChanges();

        }


        public static void InsertNewOPCoil(string production_coil_no, int change_user_id)
        {

            sdipdbEntities db = ContextHelper.SDIPDBContext;

            sw_op_coil_validate to_update = new sw_op_coil_validate();

            to_update.production_coil_no = production_coil_no;
            to_update.is_valid = "N";
            to_update.add_datetime = DateTime.Now;
            to_update.change_user_id = change_user_id;

            db.sw_op_coil_validate.Add(to_update);
            db.SaveChanges();

        }


    }
}