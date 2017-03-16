using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OBSMVC.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            Exception ex = new HttpUnhandledException();
            return View("Error", "NE - " + ex);
        }
        // GET: Error
        public ActionResult Index(string ErrorMsg)
        {
            Exception ex = new Exception(ErrorMsg);
            return View("Error", ex);
        }

        // GET: Error
        public ActionResult Index(Exception catchedException)
        {
            return View("Error", catchedException);
        }

        // GET: Error
        public ActionResult NotFound(string aspxerrorpath)
        {
            string errorMessage = "Page not found or invalid URL Entry point used.";
            if (!String.IsNullOrEmpty(aspxerrorpath)) { errorMessage += "Error Path: " + aspxerrorpath; }

            //Exception ex = new Exception("Page not found or invalid URL Entry point used." );
            Exception ex = new Exception(errorMessage);

            return View("Error", ex);
        }

        // GET: Error
        public ActionResult BadRequest()
        {
            Exception ex = new Exception("Bad Request. Please verify your submission and try again.");
            return View("Error", ex);
        }

        // GET: Error
        public ActionResult NotAuthorized()
        {
            Exception ex = new UnauthorizedAccessException();
            return View("Error", ex);
        }
    }
}