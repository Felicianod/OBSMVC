using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OBSMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            
            try
            {
                if (!String.IsNullOrEmpty(Session["session_id"].ToString()))
                {
                    return View();
                }

            }
            catch
            {
            }
           return RedirectToAction("OBSLogin", "Login");
           
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}