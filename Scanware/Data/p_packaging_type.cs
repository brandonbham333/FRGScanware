using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class packaging_type
    {
        public static packaging_type GetPackagingType(int packaging_type_cd)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            packaging_type c = db.packaging_type.SingleOrDefault(x => x.packaging_type_cd == packaging_type_cd);

            return c;
        }
    }
}