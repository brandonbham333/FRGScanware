using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Scanware.Data;

namespace Scanware.Data
{
    public partial class application
    {
        public static application GetApplication(string app){

            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.applications.SingleOrDefault(x => x.application1 == app);


        }
         
    }
}