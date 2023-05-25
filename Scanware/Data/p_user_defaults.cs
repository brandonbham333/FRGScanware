using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class user_defaults
    {
        public static user_defaults GetUserDefaultByName(int user_id, string name)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            user_defaults user_default = db.user_defaults.SingleOrDefault(x => x.user_id == user_id && x.name==name);

            return user_default;
        }

        public static void UpdateDefaultByName(int user_id, string name, string value)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            user_defaults user_default = db.user_defaults.SingleOrDefault(x => x.user_id == user_id && x.name == name);

            user_default.value = value;
            
            db.SaveChanges();

        }

        public static void InsertDefault(int user_id, string name, string value)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            user_defaults user_default = new user_defaults()
            {
                user_id=user_id, 
                name=name,
                value=value
            };

            db.user_defaults.Add(user_default);

            db.SaveChanges();

        }
        public static void DeleteDefault(int user_id, string name, string value)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            user_defaults user_default = db.user_defaults.SingleOrDefault(x => x.user_id == user_id && x.name == name);

            if (user_default != null)
            {
                db.user_defaults.Remove(user_default);
                db.SaveChanges();
            }
        }
    }
}
