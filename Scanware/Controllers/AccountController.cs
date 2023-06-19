using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using Scanware.Filters;
using Scanware.Models;
using Scanware.Ancestor;
using Scanware.Data;
using System.Security;
using System.Diagnostics;
using System.Security.Principal;

namespace Scanware.Controllers
{

    [Authorize]
    //[InitializeSimpleMembership]
    public class AccountController : Controller
    {

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            LoginModel model = new LoginModel();

            model.SSO = Properties.Settings.Default.SSO;
            if (Properties.Settings.Default.SSO == "N")
            {
                try
                {
                    model.ComputerName = Util.DetermineCompName(Request.UserHostName);
                }
                catch { }

                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }
            else
            {

                FormsAuthentication.SignOut();
                Session.Abandon();

                //get ref_system_param row
                ref_sys_param rsp = ref_sys_param.GetRefSysParam();

                String adPath = rsp.ancestor_auth_ldap; //Fully-qualified Domain Name

                AncestorAuth adAuth = new AncestorAuth(adPath);

                application_settings ship_warehouse = application_settings.GetSetting("SHIP_FROM_WAREHOUSE");

                //reset session timeout
                HttpContext.Session.Timeout = 600;

                model.UserName = HttpContext.User.Identity.Name;

                //used to authenticate within debug
                string user_domain = "";
                string[] ud;
                if (Debugger.IsAttached)
                {
                    user_domain = new System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent()).Identity.Name.ToLower();
                }
                else
                {
                    user_domain = User.Identity.Name.ToLower();
                }

                ud = user_domain.Split('\\');


                model.UserName = ud[1];

                try
                {
                    if (true == adAuth.IsAuthenticated(rsp.ancestor_auth_domain, model.UserName, model.Password))
                    {
                        //get user info
                        adAuth.logged_in_user = user_info.GetUserInfo(model.UserName);

                        //get application security record
                        //logged_in_user.application_security = application_security.GetUserApplicationSecurity(model.UserName, Properties.Settings.Default.AppName.ToString(), Properties.Settings.Default.AppVersion);

                        var CurrentAppSecurity = from AppSec in adAuth.logged_in_user.application_security
                                                 where AppSec.app_name == Properties.Settings.Default.AppName && AppSec.valid_versions == Properties.Settings.Default.AppVersion
                                                 select AppSec;

                        if (CurrentAppSecurity.ToList().Count() == 0)
                        {
                            //throw new HttpException(401, "You are not authorized for this application");
                            TempData["result"] = "No Access to App - L3Admin";
                            return View(model);
                        }

                        //set application security for this app
                        foreach (application_security app_sec in CurrentAppSecurity)
                        {
                            adAuth.logged_in_user_application_security = app_sec;
                        }


                        adAuth.logged_in_user_function_level_security = function_level_security.GetUserFunctionLevelSecurity(adAuth.logged_in_user_application_security.user_id, adAuth.logged_in_user_application_security.app_name);

                        //not needed for SSO
                        //String groups = adAuth.GetGroups();

                        //Create the ticket, and add the groups.
                        bool isCookiePersistent = model.RememberMe;

                        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, model.UserName, DateTime.Now, DateTime.Now.AddMinutes(1), isCookiePersistent, String.Empty); //refresh cookie every 60 minutes

                        //Encrypt the ticket.
                        String encryptedTicket = FormsAuthentication.Encrypt(authTicket);

                        //Create a cookie, and then add the encrypted ticket to the cookie as data.
                        HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

                        if (true == isCookiePersistent)
                            authCookie.Expires = authTicket.Expiration;

                        Session["function_level_security"] = adAuth.logged_in_user_function_level_security;
                        Session["application_security"] = adAuth.logged_in_user_application_security;
                        //Add the cookie to the outgoing cookies collection.
                        Response.Cookies.Add(authCookie);

                        try
                        {
                            
                            model.ComputerName = Util.DetermineCompName(Request.UserHostName);
                            application_login.AddApplicationLogin(adAuth.logged_in_user_application_security.user_id, model.ComputerName, "I", null);

                        }
                        catch { }

                        //You can redirect now.
                        //return Redirect(FormsAuthentication.GetRedirectUrl(model.UserName, false));
                        return RedirectToLocal(returnUrl);
                        //return Redirect("/Part");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Windows Authentication did not succeed. Check your user name and password.");
                        TempData["result"] = "Not a valid username " + model.UserName;
                        return View(model);

                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    TempData["result"] = "";
                    TempData["result"] = "App Issue";
                    return View(model);
                }
            }
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            //get ref_system_param row
            ref_sys_param rsp = ref_sys_param.GetRefSysParam();

            String adPath = rsp.ancestor_auth_ldap; //Fully-qualified Domain Name

            AncestorAuth adAuth = new AncestorAuth(adPath);

            //reset session timeout
            HttpContext.Session.Timeout = 1440;
            int li_version = 0;

            try
            {
                
                if (model.Password == null || model.Password == "")
                {
                    throw new Exception("Password cannot be blank.");
                }

                if (true == adAuth.IsAuthenticated(rsp.ancestor_auth_domain, model.UserName, model.Password))
                {
                    //get user info
                    adAuth.logged_in_user = user_info.GetUserInfo(model.UserName);

                    //get application security record 
                    application_security current_application_security = application_security.GetApplicationSecurityRecord(adAuth.logged_in_user.user_name, Properties.Settings.Default.AppName);

                    //Added Invalid version Specific Error message
                    if (current_application_security != null && !current_application_security.valid_versions.Contains(Properties.Settings.Default.AppVersion))
                    {
                        li_version = -1;
                        throw new HttpException(401, "Access Denied! This is Version " + Properties.Settings.Default.AppVersion + " .You are authorized for Version " + current_application_security.valid_versions);
                    }
                   
                    var CurrentAppSecurity = from AppSec in adAuth.logged_in_user.application_security
                                             where AppSec.app_name == Properties.Settings.Default.AppName && AppSec.valid_versions.Contains(Properties.Settings.Default.AppVersion)
                                             select AppSec;

                    if (CurrentAppSecurity.ToList().Count() == 0)
                    {

                        throw new HttpException(401, "You are not authorized for this application");
                    }

                    //set application security for this app
                    foreach (application_security app_sec in CurrentAppSecurity)
                    {
                        adAuth.logged_in_user_application_security = app_sec;
                        application_security.SetLastRunTime(app_sec.user_name, app_sec.app_name);

                    }

                    
                    adAuth.logged_in_user_function_level_security = function_level_security.GetUserFunctionLevelSecurity(adAuth.logged_in_user_application_security.user_id, adAuth.logged_in_user_application_security.app_name);

              //      String groups = adAuth.GetGroups();

                    //Create the ticket, and add the groups.
                    bool isCookiePersistent = model.RememberMe;

                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, model.UserName, DateTime.Now, DateTime.Now.AddMinutes(600), isCookiePersistent,String.Empty);//groups);

                    //Encrypt the ticket.
                    String encryptedTicket = FormsAuthentication.Encrypt(authTicket);

                    //Create a cookie, and then add the encrypted ticket to the cookie as data.
                    HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

                    if (true == isCookiePersistent)
                        authCookie.Expires = authTicket.Expiration;

                    Session["function_level_security"] = adAuth.logged_in_user_function_level_security;
                    Session["application_security"] = adAuth.logged_in_user_application_security;
                    //Add the cookie to the outgoing cookies collection.
                    Response.Cookies.Add(authCookie);

                    try
                    {

                        application_login.AddApplicationLogin(adAuth.logged_in_user_application_security.user_id, model.ComputerName, "I", null);

                    }
                    catch { }

                    //You can redirect now.
                    //return Redirect(FormsAuthentication.GetRedirectUrl(model.UserName, false));
                    return RedirectToLocal(returnUrl);
                    //return Redirect("/Part");
                }
                else
                {
                    ModelState.AddModelError("", "Windows Authentication did not succeed. Check your user name and password.");
                    model.ErrorMessage = "Windows Authentication did not succeed. Check your user name and password.";
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                if (li_version == -1)
                {
                    model.ErrorMessage = ex.Message.ToString()+ ". Call help desk for assistance.";
                }
                else
                { 
                    model.ErrorMessage ="Be sure to use your windows username/password, not the old pin. Call help desk for assistance.";
                }
            }

            return View(model);

        }

        //
        // POST: /Account/LogOff

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            return RedirectToAction("Index", "Home");
        }



        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
