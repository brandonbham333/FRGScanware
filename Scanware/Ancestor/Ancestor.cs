using System;
using System.Text;
using System.Collections;
using System.DirectoryServices;
using Scanware.Data;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using Scanware.Models;
using System.Web.Security;


namespace Scanware.Ancestor
{
    public class AncestorAuth
    {
        private String _path;
        private String _filterAttribute;
        public user_info logged_in_user;
        public application_security logged_in_user_application_security;
        public List<function_level_security> logged_in_user_function_level_security;

        public AncestorAuth(String path)
        {
            _path = path;
        }

        public bool IsAuthenticated(String domain, String username, String pwd)
        {
            if (Properties.Settings.Default.SSO == "Y")
            {
                return true;
            }

            String domainAndUsername = domain + @"\" + username;
            DirectoryEntry entry = new DirectoryEntry(_path, domainAndUsername, pwd);

            try
            {//Bind to the native AdsObject to force authentication.
                Object obj = entry.NativeObject;

                DirectorySearcher search = new DirectorySearcher(entry);

                search.Filter = "(SAMAccountName=" + username + ")";
                search.PropertiesToLoad.Add("cn");
                SearchResult result = search.FindOne();

                if (null == result)
                {
                    return false;
                }

                //Update the new path to the user in the directory.
                _path = result.Path;
                _filterAttribute = (String)result.Properties["cn"][0];
            }
            catch (Exception ex)
            {
            //    throw new Exception("Error authenticating user. " + ex.Message);
                throw new Exception("Error authenticating user. " + ex);
            }

            return true;
        }


        public String GetGroups()
        {
            DirectorySearcher search = new DirectorySearcher(_path);
            search.Filter = "(cn=" + _filterAttribute + ")";
            search.PropertiesToLoad.Add("memberOf");
            StringBuilder groupNames = new StringBuilder();

            try
            {
                SearchResult result = search.FindOne();

                int propertyCount = result.Properties["memberOf"].Count;

                String dn;
                int equalsIndex, commaIndex;

                for (int propertyCounter = 0; propertyCounter < propertyCount; propertyCounter++)
                {
                    dn = (String)result.Properties["memberOf"][propertyCounter];

                    equalsIndex = dn.IndexOf("=", 1);
                    commaIndex = dn.IndexOf(",", 1);
                    if (-1 == equalsIndex)
                    {
                        return null;
                    }

                    groupNames.Append(dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1));
                    groupNames.Append("|");

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error obtaining group names. " + ex.Message);
            }
            return groupNames.ToString();
        }
        public string getInfo(string MyInfo)
        {
            char[] chars = MyInfo.ToArray();

            Random r = new Random(259);

            char[] scramChars = MyInfo.ToArray();
            List<int> swaps = new List<int>();
            for (int i = 0; i < scramChars.Length; i++)
            {
                swaps.Add(r.Next(0, scramChars.Length));
            }
            for (int i = scramChars.Length - 1; i >= 0; i--)
            {
                char temp = scramChars[swaps[i]];
                scramChars[swaps[i]] = scramChars[i];
                scramChars[i] = temp;
            }

            string unscrambled = new string(scramChars);
            return unscrambled;
        }
    }
}