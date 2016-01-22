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

namespace OBSMVC.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult OBSLogin()
        {
            return View();
        }
        // POST: Login
        [HttpPost]
        public ActionResult OBSLogin(FormCollection login_info, string ReturnUrl)
        {

            string username = login_info.Get("Username");
            string password = login_info.Get("Password");            
            WebRequest request = WebRequest.Create("http://192.168.43.112/api/v2/user/session?service=LDAPTUSER");
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
                Session.Add("username",username);
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
            return RedirectToAction("OBSLogin", "Login");
        }
    }

}