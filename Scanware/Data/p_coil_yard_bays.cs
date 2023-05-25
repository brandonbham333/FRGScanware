using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class coil_yard_bays
    {
        public static List<coil_yard_bays> GetAllCoilYardBays()
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.coil_yard_bays.ToList();

        }

        public static coil_yard_bays GetCoilYardBay(string bay_cd)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.coil_yard_bays.Where(x => x.bay_cd == bay_cd).FirstOrDefault();

        }
    }
}

