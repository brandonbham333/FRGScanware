using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class product_processors
    {
        public static List<product_processors> GetProductProcessorsWithEntryLocation()
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            List<product_processors> inside_product_processors = new List<product_processors>();

            inside_product_processors = db.product_processors.Where(x => x.inside_or_outside =="I" && x.entry_coil_yard_column != null && x.entry_coil_yard_row != null).OrderBy(y => y.processor_cd).ToList();

            //SPECIAL CASE FOR RAIL STAGING
            product_processors rail_staged = new product_processors()
            {
                inside_or_outside="I",
                name="SDI - COL Rail Staging",
                facility_cd="R",
                entry_coil_yard_column="HBS",
                entry_coil_yard_row="RS1"

            };

            inside_product_processors.Add(rail_staged);

            return inside_product_processors;

        }

        public static List<product_processors> GetInsideProductProcessors()
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            List<product_processors> product_processors = new List<product_processors>();

            product_processors = db.product_processors.Where(x => x.inside_or_outside == "I").OrderBy(y => y.processor_cd).ToList();

            return product_processors;

        }

        public static product_processors GetInsideProductProcessorFromFacilityCD(string facility_cd)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.product_processors.Where(x => x.facility_cd == facility_cd && x.inside_or_outside == "I").FirstOrDefault();

        }


    }
}