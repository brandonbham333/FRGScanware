using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Scanware.Data;

namespace Scanware.Data
{
    public partial class user_info
    {
        public static user_info GetUserInfo(string user_name)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            user_info logged_in_user = db.user_info.SingleOrDefault(x => x.user_name == user_name);

            return logged_in_user;
        }
    }
}