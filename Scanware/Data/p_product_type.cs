using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class product_type
    {
        public static product_type GetProductType(int product_type_cd)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            product_type c = db.product_type.SingleOrDefault(x => x.product_type_cd == product_type_cd);

            return c;
        }
    }
}