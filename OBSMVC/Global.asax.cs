using System;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace OBSMVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            GlobalFilters.Filters.Add(new System.Web.Mvc.AuthorizeAttribute());
            GlobalFilters.Filters.Add(new HandleErrorAttribute());
            ViewEngines.Engines.Add(new RazorViewEngine());
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContextWrapper context = new HttpContextWrapper(this.Context);

            if (context.Request.IsAjaxRequest())
            {
                context.Response.SuppressFormsAuthenticationRedirect = true;
            }
        }
        
        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null || authCookie.Value == "")
                return;

            FormsAuthenticationTicket authTicket;
            try
            {
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch
            {
                return;
            }

            // retrieve roles from UserData
            string[] roles = authTicket.UserData.Split(';');

            if (Context.User != null)
                Context.User = new GenericPrincipal(Context.User.Identity, roles);
            //Valid Roles are: "Admin", "Super User", "Editor", "Viewer"
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            // Get the exception object.
            Exception exc = Server.GetLastError();

            if (exc == null) { exc = new Exception("An Unknown Error occurred."); }

            //System.Diagnostics.Debug.WriteLine(exc);
            string errorMessage = exc.Message.TrimEnd(System.Environment.NewLine.ToCharArray());  //Remove the Carriage returns from the String

            // ----------- Handle the HTTP errors ----------------------------
            if (exc.GetType() == typeof(HttpException))
            {
                // The Complete Error Handling Example generates some errors using URLs with "NoCatch" in them;
                // ignore these here to simulate what would happen if a global.asax handler were not implemented.
                if (exc.Message.Contains("NoCatch") || exc.Message.Contains("maxUrlLength"))
                    return;


                // Process exception...
                if (exc.Message.Contains("ServerExecuteHttpHandlerAsyncWrapper") || !Request.IsAuthenticated)
                {
                    Response.Redirect("/Account/Login");
                }


                //Redirect HTTP errors to HttpError page
                //Server.Transfer("HttpErrorPage.aspx");     //For Standard ASPX Error Handling Page
                //Or do a Response Redirect to a controller View (MVC)
            }


            // For other kinds of errors give the user some information but stay on the default page
            Response.Write("<h2>Global Page Error</h2>\n");
            Response.Write("<p>" + exc.Message + "</p>\n");
            Response.Write(@"Return to the <a href='\Home\Index\'>Default Page</a>\n");

            //// Log the exception and notify system operators
            //ExceptionUtility.LogException(exc, "DefaultPage");
            //ExceptionUtility.NotifySystemOps(exc);

            //// We've handled the error, so clear it from the Server. 
            ////Leaving the server in an error state can cause unintended side effects as the server continues its attempts to handle the error.
            Server.ClearError();

            //// Possible that a partially rendered page has already been written to response buffer before encountering error, so clear it.
            //Response.Clear();

            //// Finally redirect, transfer, or render an error view if Unhandled

            //Session["ErrorMessage"] = errorMessage;            
            //Server.Transfer("~/Error/ErrorMsg");
            //Server.Transfer("~/Account/Login");
            //Response.Redirect("~/Error/ErrorMsg");

        }


    }
}
