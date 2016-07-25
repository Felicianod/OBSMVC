using System;
using System.Configuration;
using System.Web.Mvc;

namespace OBSMVC.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            string environment = "";
            //Verify whether we are running in DEV environemnt or not
            if (!(ConfigurationManager.AppSettings["ServerType"] == null))
            {
                environment = ConfigurationManager.AppSettings["ServerType"].ToString();
                if (environment.Equals("Development") || environment.Equals("QA")) { return RedirectToAction("AppSelection", "Home"); }
            }

            if (!Request.IsAuthenticated || Session["first_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            return RedirectToAction("Home", "Home");
        }

        [AllowAnonymous]
        public ActionResult AppSelection()
        {
            return View();
        }

        public ActionResult Home()
        {
            //if (!Request.IsAuthenticated) { return RedirectToAction("Login", "Login"); }

            return View();
        }

    }




}