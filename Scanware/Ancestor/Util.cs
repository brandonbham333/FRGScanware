using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Scanware.Data;
using System.IO;
using System.Net.Mail;
using System.Configuration;
using System.Web.Configuration;
using Microsoft.Win32;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Scanware.Ancestor
{
    public class Util
    {
        public static string DetermineCompName(string IP)
        {
            IPAddress myIP = IPAddress.Parse(IP);
            IPHostEntry GetIPHost = Dns.GetHostEntry(myIP);
            List<string> compName = GetIPHost.HostName.ToString().Split('.').ToList();
            return compName.First();
        }

        public static bool IsEmailValid(string emailAddress)
        {

            try
            {
                MailAddress mailAddress = new MailAddress(emailAddress);
            }
            catch (Exception)
            {
                return false;
            }

            return true;

        }

        public static string GetRegLocation2()
        {
            string location = "";
            string userId;
            string userPwd;

            if (Debugger.IsAttached)
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\SDI\Company_Preferences");
                location = key.GetValue("Location").ToString();
                string live_test = key.GetValue("LiveTest").ToString();
                key.Close();

                try
                {
                    AncestorAuth myClass = new AncestorAuth("test");

                    userPwd = myClass.getInfo("_!*Erc@5m*Hc?=@bmUHz2cM8P&hDFB");
                    userId = myClass.getInfo("icresrreFneUGg");
                  
                }
                catch (Exception e)
                {
                    return e.Message;
                }

                string connStringBeg = "metadata = res://*/Data.sdipdb.csdl|res://*/Data.sdipdb.ssdl|res://*/Data.sdipdb.msl;provider=System.Data.SqlClient;provider connection string='data source=";
                string connStringEnd = ";initial catalog=sdipdb;persist security info=True;user id=" + userId + ";password=" + userPwd + ";MultipleActiveResultSets=True;App=SCANWARE'";
                string connString;

                connString = connStringBeg;

                Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
                if (location == "C")
                {
                    if (live_test == "L")
                    {
                        connString += "col-l3";
                    }
                    else
                    {
                        connString += @"vlab-l3\test";
                    }
                    connString += connStringEnd;

                }
                else if (location == "B")
                {
                    if (live_test == "L")
                    {
                        connString += "fb3s-sql-prod.btlr.sdi.local";
                    }
                    else
                    {
                        connString += @"fb3s-sql-test.btlr.sdi.local\test";
                    }
                    connString += connStringEnd;
                }
                else if (location == "S")
                {
                    if (live_test == "L")
                    {
                        connString += "fs3s-sqlprodha.sfrd.sdi.local";
                    }
                    else
                    {
                        connString += @"fs3s-sql-dev.sfrd.sdi.local";
                    }
                    connString += connStringEnd;
                }
                else if (location == "T")
                {
                    if (live_test == "L")
                    {
                        connString += @"GTDB2\sdisql";
                    }
                    else
                    {
                        connString += @"GTDBTest2\sdisql";
                    }
                    connString += connStringEnd;
                }

                //config.ConnectionStrings.ConnectionStrings["sdipdbEntities"].ConnectionString = string.Format(config.ConnectionStrings.ConnectionStrings["sdipdbEntities"].ConnectionString, "sdipdb");
                config.ConnectionStrings.ConnectionStrings["sdipdbEntities"].ConnectionString = connString;
                config.Save();
                ConfigurationManager.RefreshSection("connectionStrings");
            }
            else
            {
                location = Properties.Settings.Default.RegLocation;
            }

            return location;
        }
        public static string GetRegLocation()
        {
            string location = "";

            if (Debugger.IsAttached)
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\SDI\Company_Preferences");
                location = key.GetValue("Location").ToString();
                string live_test = key.GetValue("LiveTest").ToString();
                key.Close();

                string connStringBeg = "metadata = res://*/Data.sdipdb.csdl|res://*/Data.sdipdb.ssdl|res://*/Data.sdipdb.msl;provider=System.Data.SqlClient;provider connection string='data source=";
                string connStringEnd = ";initial catalog=sdipdb;persist security info=True;user id=SdiGenericUser;password=Aug282:08PM;MultipleActiveResultSets=True;App=SCANWARE'";
                string connString;

                connString = connStringBeg;

                Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
                if (location == "C")
                {
                    if (live_test == "L")
                    {
                        connString += "col-l3";
                    }
                    else
                    {
                        connString += @"vlab-l3\test";
                    }
                    connString += connStringEnd;

                }
                else if (location == "B")
                {
                    if (live_test == "L")
                    {
                        connString += "fb3s-sql-prod.btlr.sdi.local";
                    }
                    else
                    {
                        connString += @"fb3s-sql-test.btlr.sdi.local\test";
                    }
                    connString += connStringEnd;
                }
                else if (location == "S")
                {
                    if (live_test == "L")
                    {
                        connString += "fs3s-sqlprodha.sfrd.sdi.local";
                    }
                    else
                    {
                        connString += @"fs3s-sql-dev.sfrd.sdi.local";
                    }
                    connString += connStringEnd;
                }
                else if (location == "T")
                {
                    if (live_test == "L")
                    {
                        connString += @"GTDB2\sdisql";
                    }
                    else
                    {
                        connString += @"GTDBTest2\sdisql";
                    }
                    connString += connStringEnd;
                }
                else if (location == "M")
                {
                    if (live_test == "L")
                    {
                        connString += @"fm3s-gp.mxfrd.sdi.local\L3";
                    }
                    else
                    {
                        connString += @"fm3s-gp.mxfrd.sdi.local\L3Test";
                    }
                    connString += connStringEnd;
                }

                    //config.ConnectionStrings.ConnectionStrings["sdipdbEntities"].ConnectionString = string.Format(config.ConnectionStrings.ConnectionStrings["sdipdbEntities"].ConnectionString, "sdipdb");
                    config.ConnectionStrings.ConnectionStrings["sdipdbEntities"].ConnectionString = connString;
                config.Save();
                ConfigurationManager.RefreshSection("connectionStrings");
            }
            else
            {
                location = Properties.Settings.Default.RegLocation;
            }

            return location;
        }

        public static string GetRegLiveTest()
        {
            string live_test = "";
            if (!Debugger.IsAttached)
            {
                live_test = "L";
            }
            else
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\SDI\Company_Preferences");
                live_test = key.GetValue("LiveTest").ToString();
                key.Close();
            }

            return live_test;
        }

        public static string GetRegLoc()
        {
            string location = "";

            if (!Debugger.IsAttached)
            {
                location = Properties.Settings.Default.RegLocation;
            }
            else
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\SDI\Company_Preferences");
                location = key.GetValue("Location").ToString();
                key.Close();
            }

            return location;
        }
      
    }

}