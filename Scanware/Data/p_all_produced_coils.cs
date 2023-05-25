using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class all_produced_coils
    {
        public static all_produced_coils GetAllProducedCoilByProductionCoilNumberOrTagNumber(string production_coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            all_produced_coils apc = db.all_produced_coils.SingleOrDefault(x => x.production_coil_no == production_coil_no || x.tag_no == production_coil_no);
            return apc;
        }

        public static all_produced_coils GetAllProducedCoilByTagNumber(string tag_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            all_produced_coils apc = db.all_produced_coils.SingleOrDefault(x => x.tag_no == tag_no);

            return apc;
        }

        public static all_produced_coils GetAllProducedCoil(string production_coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            all_produced_coils apc = db.all_produced_coils.SingleOrDefault(x => x.production_coil_no == production_coil_no);

            return apc;
        }

        public static all_produced_coils GetAllProducedCoilLastProduced(int cons_coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            DateTime TwoYearsBack = DateTime.Now.AddYears(-2);

            all_produced_coils apc = db.all_produced_coils.FirstOrDefault(x => x.cons_coil_no == cons_coil_no && x.coil_last_facility_ind=="Y" && x.produced_dt_stamp > TwoYearsBack);

            return apc;
        }

        public static int GetCountOfDaughterCoils(int cons_coil_no)
        {
            int count = 0;
            DateTime TwoYearsBack = DateTime.Now.AddYears(-2);

            sdipdbEntities db = ContextHelper.SDIPDBContext;

            count = db.all_produced_coils.Count(x => x.cons_coil_no == cons_coil_no && x.coil_last_facility_ind == "Y" && x.produced_dt_stamp > TwoYearsBack);
                
            return count;
        }

        public static List<all_produced_coils> GetSisterCoils(int cons_coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            DateTime TwoYearsBack = DateTime.Now.AddYears(-2);

            List<all_produced_coils> apc = db.all_produced_coils.Where(x => x.cons_coil_no == cons_coil_no && x.coil_last_facility_ind == "Y" && x.produced_dt_stamp > TwoYearsBack).ToList();

            return apc;
        }

        public static string GetCoilYardLocation(string production_coil_no)
        {
            //move in coil yard location
            coil_yard_locations current_coil_yard_location = coil_yard_locations.GetCoilYardLocation(production_coil_no);

            if (current_coil_yard_location != null)
            {
                return current_coil_yard_location.column + " " + current_coil_yard_location.row;
            }
            else
            {
                return null;
            }
        }

        public static List<all_produced_coils> GetAPCCoilReauthorization(string production_coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.all_produced_coils.Where(x => x.production_coil_no == production_coil_no).ToList();
        }
    }
}
