using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Scanware.Data
{
    public partial class application_settings 
    {
        public static application_settings GetSetting(string settingName)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            var setting  = db.application_settings.FirstOrDefault(x => x.app_name == "Scanware" && x.setting_name == settingName);

            return setting;
        }

        public static application_settings GetAppSetting(string settingName)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            var setting = db.application_settings.FirstOrDefault(x => x.app_name == "Scanware" && x.setting_name == settingName);

            return setting;

        }

        public static List<application_settings> GetApplicationSettings()
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;
            var settings = db.application_settings.Where(x => x.app_name == "Scanware").ToList();
            return settings;
        }

    }
}
