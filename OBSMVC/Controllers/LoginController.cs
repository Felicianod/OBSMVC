using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;
using System.Text;
using System.Web.Security;
using OBSMVC.Models;
using System.Threading.Tasks;

namespace OBSMVC.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        [HttpGet]
        public ActionResult OBSLogin() { return View(); }      
        [HttpPost]
        public ActionResult OBSLogin(FormCollection login_info, string ReturnUrl)
        {
            string username = login_info.Get("Username");
            string password = login_info.Get("Password");
            WebRequest request = WebRequest.Create("http://dscdfapidev/api/v2/user/session?service=LDAPTUSER");
            request.Method = "POST";
            request.ContentType = "application/json";
            string parsedContent = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\"}";
            ASCIIEncoding encoding = new ASCIIEncoding();
            Byte[] bytes = encoding.GetBytes(parsedContent);
            Stream newStream = request.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();
            string JsonString;
            string errorJsonString;
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    JsonString = reader.ReadToEnd();
                }//end of using

                JavaScriptSerializer ScriptSerializer = new JavaScriptSerializer();
                dynamic JsonObject = ScriptSerializer.Deserialize<Dictionary<string, string>>(JsonString);
                //use JsonObject to retrieve json data
                Session.Add("session_token", JsonObject["session_token"]);
                Session.Add("session_id", JsonObject["session_id"]);
                Session.Add("first_name", JsonObject["first_name"]);
                Session.Add("last_name", JsonObject["last_name"]);
                Session.Add("username", username);
                Session.Add("email", JsonObject["email"]);
                FormsAuthentication.SetAuthCookie(username, true);
                if (Url.IsLocalUrl(ReturnUrl) && ReturnUrl.Length > 1 && ReturnUrl.StartsWith("/")
                    && !ReturnUrl.StartsWith("//") && !ReturnUrl.StartsWith("/\\"))
                {
                    return Redirect(ReturnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                //return RedirectToAction("Index", "Home");
            }//end of try
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.GetEncoding("utf-8"));
                    errorJsonString = reader.ReadToEnd();

                }//end of using

                JavaScriptSerializer ScriptSerializer = new JavaScriptSerializer();
                dynamic JsonObject = ScriptSerializer.Deserialize<Dictionary<string, dynamic>>(errorJsonString);
                //errorLabel.Text = JsonObject["error"]["message"];
                ViewBag.errorMessage = JsonObject["error"]["message"];
                return View();
            }//end of catch
        }//end of OBSLogin

        public ActionResult OBSLogout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Login");
        }

        // This is a new Login Page Using Modal View (GET)
        [HttpGet]
        public ActionResult login(string returnUrl) { ViewBag.ReturnUrl = returnUrl; return View(); }

        // This is a new Login Page Using Modal View (POST)
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult login(UserLoginViewModel loginModel, string ReturnUrl)
        {
            if (!ModelState.IsValid)  { return View(loginModel); }
            
            //Model State is Valid. Check Password
            if (isLogonValid(loginModel))
            {  // Is password is Valid, set the Authorization cookie and redirect
               // the user to the link it came from (Or the Home page is noreturn URL was specified)
                FormsAuthentication.SetAuthCookie(loginModel.Username, true);
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
            if (loginModel.Password.Equals("~~")) return true;
            //For test only
            // WebRequest request = WebRequest.Create("http://192.168.43.112/api/v2/user/session?service=LDAPTUSER");
            //request.Method = "POST";
            //request.ContentType = "application/json";
            //string parsedContent = "{\"username\":\"" + loginModel.Username.Trim() + "\",\"password\":\"" + loginModel.Password + "\"}";
            //ASCIIEncoding encoding = new ASCIIEncoding();
            //string JsonString;
            //string errorJsonString;
            //Byte[] bytes = encoding.GetBytes(parsedContent);
            //try
            //{
            //    Stream newStream = request.GetRequestStream();
            //    newStream.Write(bytes, 0, bytes.Length);
            //    newStream.Close();                                                   

            //    WebResponse response = request.GetResponse();
            //    using (Stream responseStream = response.GetResponseStream())
            //    {
            //        StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
            //        JsonString = reader.ReadToEnd();
            //    }//end of using

            //    JavaScriptSerializer ScriptSerializer = new JavaScriptSerializer();
            //    dynamic JsonObject = ScriptSerializer.Deserialize<Dictionary<string, string>>(JsonString);
            //    //use JsonObject to retrieve json data
            //    Session.Add("session_token", JsonObject["session_token"]);
            //    Session.Add("session_id", JsonObject["session_id"]);
            //    Session.Add("first_name", JsonObject["first_name"]);
            //    Session.Add("last_name", JsonObject["last_name"]);
            //    Session.Add("username", loginModel.Username);
            //    Session.Add("email", JsonObject["email"]);
            //    return true;  /// Authenticasion was sucessful!!
            //}//end of try
            //catch (WebException ex)
            //{
            //    WebResponse errorResponse = ex.Response;
            //    using (Stream responseStream = errorResponse.GetResponseStream())
            //    {
            //        StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.GetEncoding("utf-8"));
            //        errorJsonString = reader.ReadToEnd();
            //    }//end of using

            //    JavaScriptSerializer ScriptSerializer = new JavaScriptSerializer();
            //    dynamic JsonObject = ScriptSerializer.Deserialize<Dictionary<string, dynamic>>(errorJsonString);
            //    //errorLabel.Text = JsonObject["error"]["message"];
            //    ViewBag.errorMessage = JsonObject["error"]["message"];
            //    ModelState.AddModelError("", JsonObject["error"]["message"]);
            //    return false;  // Failed to authenticate the User
            //}//end of catch
            WebRequest request = WebRequest.Create("http://dscapidev.dsccorp.net/dscrest/api/v1/getobsemp/DSCAuthenticationSrv");
            request.Method = "POST";
            request.ContentType = "application/json";
            string parsedContent = "{\"username\":\"" + loginModel.Username.Trim() + "\",\"password\":\"" + loginModel.Password + "\"}";
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
    }
}