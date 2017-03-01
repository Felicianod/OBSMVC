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
            var raisedException = Server.GetLastError();
            string errorMessage = raisedException.Message;
            // Process exception...

            //// We've handled the error, so clear it from the Server. 
            ////Leaving the server in an error state can cause unintended side effects as the server continues its attempts to handle the error.
            //Server.ClearError();

            //// Possible that a partially rendered page has already been written to response buffer before encountering error, so clear it.
            //Response.Clear();

            //// Finally redirect, transfer, or render a error view
            //Response.Redirect("~/Error/Index?ErroMsg=" + errorMessage);
        }


    }
}
