using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class coil_reauthorization
    {
        public static List<coil_reauthorization> GetCoilReAuthorization(string coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.coil_reauthorization.Where(x => x.production_coil_no == coil_no).ToList();
        }
    }
}