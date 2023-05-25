using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class from_freight_locations
    {
        public static List<from_freight_locations> GetFromFreightLocatins()
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.from_freight_locations.ToList();


        }

        public static from_freight_locations GetLocationByCD(int from_freight_location_cd)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.from_freight_locations.FirstOrDefault(x => x.from_freight_location_cd == from_freight_location_cd);
        }

    }
}