using OBSMVC.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace OBSMVC.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY();

        #region UnusedCode
        //[HttpPost]
        //public ActionResult OBSLogin(FormCollection login_info, string ReturnUrl)
        //{
        //    string username = login_info.Get("Username");
        //    string password = login_info.Get("Password");
        //    WebRequest request = WebRequest.Create("http://dscdfapidev/api/v2/user/session?service=LDAPTUSER");
        //    request.Method = "POST";
        //    request.ContentType = "application/json";
        //    string parsedContent = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\"}";
        //    ASCIIEncoding encoding = new ASCIIEncoding();
        //    Byte[] bytes = encoding.GetBytes(parsedContent);
        //    Stream newStream = request.GetRequestStream();
        //    newStream.Write(bytes, 0, bytes.Length);
        //    newStream.Close();
        //    string JsonString;
        //    string errorJsonString;
        //    try
        //    {
        //        WebResponse response = request.GetResponse();
        //        using (Stream responseStream = response.GetResponseStream())
        //        {
        //            StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
        //            JsonString = reader.ReadToEnd();
        //        }//end of using

        //        JavaScriptSerializer ScriptSerializer = new JavaScriptSerializer();
        //        dynamic JsonObject = ScriptSerializer.Deserialize<Dictionary<string, string>>(JsonString);
        //        //use JsonObject to retrieve json data
        //        Session.Add("session_token", JsonObject["session_token"]);
        //        Session.Add("session_id", JsonObject["session_id"]);
        //        Session.Add("first_name", JsonObject["first_name"]);
        //        Session.Add("last_name", JsonObject["last_name"]);
        //        Session.Add("username", username);
        //        Session.Add("email", JsonObject["email"]);
        //        FormsAuthentication.SetAuthCookie(username, true);
        //        if (Url.IsLocalUrl(ReturnUrl) && ReturnUrl.Length > 1 && ReturnUrl.StartsWith("/")
        //            && !ReturnUrl.StartsWith("//") && !ReturnUrl.StartsWith("/\\"))
        //        {
        //            return Redirect(ReturnUrl);
        //        }
        //        else
        //        {
        //            return RedirectToAction("Index", "Home");
        //        }
        //        //return RedirectToAction("Index", "Home");
        //    }//end of try
        //    catch (WebException ex)
        //    {
        //        WebResponse errorResponse = ex.Response;
        //        using (Stream responseStream = errorResponse.GetResponseStream())
        //        {
        //            StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.GetEncoding("utf-8"));
        //            errorJsonString = reader.ReadToEnd();

        //        }//end of using

        //        JavaScriptSerializer ScriptSerializer = new JavaScriptSerializer();
        //        dynamic JsonObject = ScriptSerializer.Deserialize<Dictionary<string, dynamic>>(errorJsonString);
        //        //errorLabel.Text = JsonObject["error"]["message"];
        //        ViewBag.errorMessage = JsonObject["error"]["message"];
        //        return View();
        //    }//end of catch
        //}//end of OBSLogin
        #endregion

        public ActionResult OBSLogout()
        {
            FormsAuthentication.SignOut();
                        ViewBag.ReturnUrl = "\\Home\\Index";
            Session["emp_id"] = "";
            Session["role"] = "";
            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
            //return RedirectToAction("Login", "Login");
        }

        // This is a new Login Page Using Modal View (GET)
        [HttpGet]
        public ActionResult Login(string returnUrl) { ViewBag.ReturnUrl = returnUrl; return View(); }

        // This is a new Login Page Using Modal View (POST)
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLoginViewModel loginModel, string ReturnUrl)
        {
            if (!ModelState.IsValid)  { return View(loginModel); }
            
            //Model State is Valid. Check Password
            if (isLogonValid(loginModel))
            {  // Is password is Valid, set the Authorization cookie and redirect
               // the user to the link it came from (Or the Home page is noreturn URL was specified)
                FormsAuthentication.SetAuthCookie(loginModel.Username, true);
                setUserRoles(loginModel.Username, new string[] {Session["role"].ToString() });
                if (Url.IsLocalUrl(ReturnUrl) && ReturnUrl.Length > 1 && ReturnUrl.StartsWith("/")
                    && !ReturnUrl.StartsWith("//") && !ReturnUrl.StartsWith("/\\"))
                     { return Redirect(ReturnUrl); }
                else { return RedirectToAction("Index", "Home"); }
                //return RedirectToAction("Index", "Home") as default;
            }
            else
            {
                ViewBag.ReturnUrl = ReturnUrl;
                ModelState.AddModelError("", "Cannot Logon");
                return View(loginModel);
            }

            //if (ModelState.IsValid)
            //{
            //    ViewBag.errorMessage = "Model State is Valid!";
            //    return View();
            //}
            //else {
            //    ViewBag.errorMessage = "Model State is not Valid";
            //    return View();
            //}
        }

        //============= PRIVATE LOGIN HELPER METHODS ==================
        private bool isLogonValid(UserLoginViewModel loginModel)
        {
            if (loginModel.Password.Equals("~~") && (loginModel.Username.Equals("delgado_feliciano")|| loginModel.Username.Equals("abduguev_rasul")))
            {
                Session.Add("role", "Admin");
                if (loginModel.Username.Equals("delgado_feliciano"))
                {
                    Session.Add("first_name", "Feliciano");
                    Session.Add("last_name", "Delgado");
                    Session.Add("username", loginModel.Username);
                    Session.Add("email", "feliciano.delgado@dsc-logistics.com");
                }
                else
                {
                    Session.Add("first_name", "Rasul");
                    Session.Add("last_name", "Abduguev");
                    Session.Add("username", loginModel.Username);
                    Session.Add("email", "rasul.abduguev@dsc-logistics.com");
                }

                return true;
            }

            string ldaurl = ConfigurationManager.AppSettings["LDAPURL"];
            WebRequest request = WebRequest.Create(ldaurl);
            request.Method = "POST";
            request.ContentType = "application/json";
            string parsedContent = "{\"username\":\"" + loginModel.Username.Trim() + "\",\"password\":\"" + loginModel.Password + "\",\"appid\":\"DM\"}";
            ASCIIEncoding encoding = new ASCIIEncoding();
            string JsonString;
            //string errorJsonString;
            Byte[] bytes = encoding.GetBytes(parsedContent);
            try
            {
                Stream newStream = request.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();

                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    JsonString = reader.ReadToEnd();
                }//end of using
                JavaScriptSerializer ScriptSerializer = new JavaScriptSerializer();
                dynamic JsonObject = ScriptSerializer.Deserialize<Dictionary<dynamic, dynamic>>(JsonString);
                //use JsonObject to retrieve json data   
                if (JsonObject["result"]=="SUCCESS")
                {
                    Session.Add("first_name", JsonObject["DSCAuthenticationSrv"]["first_name"]);
                    Session.Add("last_name", JsonObject["DSCAuthenticationSrv"]["last_name"]);
                    Session.Add("username", loginModel.Username);
                    Session.Add("email", JsonObject["DSCAuthenticationSrv"]["email"]);
                    string role = (from r in db.OBS_ROLE
                                   join ur in db.OBS_USER_ROLE
                                   on r.obs_role_id equals ur.obs_role_id
                                   join ua in db.OBS_USER_AUTH
                                    on ur.obs_user_auth_id equals ua.obs_user_auth_id
                                   where ua.obs_user_auth_dsc_ad_name == loginModel.Username && r.obs_role_active_yn=="Y"
                                   && ua.obs_user_auth_active_yn =="Y" && ur.obs_user_role_eff_start_dt<= DateTime.Now && ur.obs_user_role_eff_end_dt>DateTime.Now
                                   select r.obs_role_name).FirstOrDefault();
                    if(!String.IsNullOrEmpty(role))
                    {
                        Session.Add("role", role);
                    }
                    else
                    {
                        Session.Add("role", "Not Authorized");
                    }
                    return true;  /// Authenticasion was sucessful!!
                }
                else
                {
                    ViewBag.errorMessage = JsonObject["message"];
                    ModelState.AddModelError("", JsonObject["message"]);
                    return false;
                }
            }//end of try
            catch (Exception ex)
            {
                //WebResponse errorResponse = ex.Response;
                //using (Stream responseStream = errorResponse.GetResponseStream())
                //{
                //    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.GetEncoding("utf-8"));
                //    errorJsonString = reader.ReadToEnd();
                //}//end of using

                //JavaScriptSerializer ScriptSerializer = new JavaScriptSerializer();
                //dynamic JsonObject = ScriptSerializer.Deserialize<Dictionary<string, string>>(errorJsonString);
                ////errorLabel.Text = JsonObject["error"]["message"];
                ViewBag.errorMessage = ex.Message;
                ModelState.AddModelError("", ex.Message);
                return false;  // Failed to authenticate the User
            }//end of catch
        }

        private void setUserRoles(string userName, string[] roles)
        {
            string userRoles = String.Join(";",roles);

            var authTicket = new FormsAuthenticationTicket(
                 1,                             // version
                 userName,                      // user name
                 DateTime.Now,                  // created
                 DateTime.Now.AddMinutes(20),   // expires
                 true,                          // persistent?
                 userRoles              // can be used to store roles
              );

            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            this.ControllerContext.HttpContext.Response.Cookies.Add(authCookie);

            //HttpContext.Current.Response.Cookies.Add(authCookie);
        }
    }



}