using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class application_security
    {
        public static List<application_security> GetUserApplicationSecurity(string user_name, string app_name, string valid_versions)
        {

            sdipdbEntities db = ContextHelper.SDIPDBContext;

            //return db.application_security.SingleOrDefault(x => x.app_name == app_name && x.user_name == user_name && x.valid_versions == valid_versions);

            var ApplicationSecurity = from AppSec in db.application_security
                                      where AppSec.user_name == user_name && AppSec.app_name == app_name && AppSec.valid_versions.Contains(valid_versions)
                                      select AppSec;
            
            return ApplicationSecurity.ToList();

        }

        public static application_security GetApplicationSecurity(int user_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.application_security.Where(x => x.user_id == user_id).FirstOrDefault();


        }


        public static application_security GetApplicationSecurityRecord(string user_name, string app_name)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.application_security.Where(x => x.user_name == user_name && x.app_name == app_name).FirstOrDefault();
        }

        public static application_security SetLastRunTime(string user_name, string app_name)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            application_security appSecurity =  db.application_security.Where(x => x.user_name == user_name && x.app_name == app_name).FirstOrDefault();

            appSecurity.last_runtime = DateTime.Now;

            db.SaveChanges();

            return appSecurity;
        }
    }
}