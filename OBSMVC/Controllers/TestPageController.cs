using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OBSMVC.Controllers
{
    public class TestPageController : Controller
    {
        // GET: TestPage
        public ActionResult Test()
        {
            return View();
        }
    }
}