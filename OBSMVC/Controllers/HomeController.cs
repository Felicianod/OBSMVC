using System.Configuration;
using System.Web.Mvc;

namespace OBSMVC.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            //Check whether this server needs to redirect the default "Index" entry point to the App Menu Selection Page or not            
            try
            {
                if (ConfigurationManager.AppSettings["ServerType"].ToString().Equals("Development")) { return RedirectToAction("AppSelection", "Home"); }
            }
            catch
            {
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Login");
            }

            //try
            //{
            //    if (!String.IsNullOrEmpty(Session["session_id"].ToString()))   { return View();}
            //}
            //catch{}
            //return RedirectToAction("OBSLogin", "Login");


            
            return View();
        }

        [AllowAnonymous]
        public ActionResult AppSelection()
        {
            return View();
        }

    }




}