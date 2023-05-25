using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public class ContextHelper
    {
        public static sdipdbEntities SDIPDBContext
        {
            get
            {
                string ocKey = "ocm_" + HttpContext.Current.GetHashCode().ToString("x");

                if (!HttpContext.Current.Items.Contains(ocKey))
                {
                    HttpContext.Current.Items.Add(ocKey, new sdipdbEntities());
                }
                return HttpContext.Current.Items[ocKey] as sdipdbEntities;
            }
        }
    }
}