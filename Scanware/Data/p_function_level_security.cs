using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class function_level_security
    {
        public List<function_level_security> function_level_security_list;

        public function_level_security()
        {
            function_level_security_list = null;
        }

        public function_level_security(List<function_level_security> _function_level_security)
        {
            function_level_security_list = _function_level_security;
        }

        public static List<function_level_security> GetUserFunctionLevelSecurity(int user_id, string app_name)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            List<function_level_security> user_function_level_security = new List<function_level_security>();

            var user_fls = from fls in db.function_level_security
                                  where fls.user_id == user_id && fls.app_name == app_name
                                  select fls;
            
            foreach(function_level_security fls in user_fls)
            {
                user_function_level_security.Add(fls);
            }

            return user_function_level_security;
        }

        public bool HasFunctionLevelSecurity(string function_name)
        {
                
            bool IsInRole = false;

            //if roles exist
            if(function_level_security_list != null)
            {

                foreach (function_level_security fls in function_level_security_list)
                {
                    if (fls.function_name == function_name || fls.function_name =="ADMIN")
                    {
                        { 
                            IsInRole = true;
                        }
                    }
                }

            }

            return IsInRole;

        }
    }
}