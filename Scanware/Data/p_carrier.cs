using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class carrier
    {
        public static carrier GetCarrier(int carrier_cd)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            carrier c = db.carriers.SingleOrDefault(x => x.carrier_cd == carrier_cd);

            return c;
        }

        public static List<carrier> GetActiveCarriers()
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.carriers.Where(x=>x.active=="Y" && x.carrier_mode=="T").OrderBy(x=>x.name).ToList();

            
        }
    }
}