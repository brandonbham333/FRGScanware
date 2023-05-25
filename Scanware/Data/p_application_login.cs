using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class application_login
    {
        public static void AddApplicationLogin(int user_id, string computer_name, string i_o, int? spid )
        {

            application_login new_login = new application_login()
            {
                user_id=user_id, 
                computer_name = computer_name, 
                in_out = i_o,
                spid = spid, 
                change_datetime=DateTime.Now

            };

            sdipdbEntities db = ContextHelper.SDIPDBContext;
            db.application_login.Add(new_login);
            db.SaveChanges();

        }
    }
}