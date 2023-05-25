using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Scanware.Data;

namespace Scanware.Data
{
    public partial class ref_sys_param
    {
        public static ref_sys_param GetRefSysParam()
       {
            sdipdbEntities db = ContextHelper.SDIPDBContext;
  
            return db.ref_sys_param.SingleOrDefault(x => x.pk == 1);
        }
    }
}