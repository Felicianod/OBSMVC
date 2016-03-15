using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OBSMVC.Models;
namespace OBSMVC.Controllers
{
    public class CustomerController : Controller
    {
        //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        // GET: Customer
        public ActionResult Index()
        {
            return View();
        }
        //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        // GET: Customer
        public ActionResult List()
        {
            return View();
        }
        //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        // POST: /Customer/List
        [HttpPost]
        public ActionResult List(int dsc_cust_id)
        {
            try
            {
                using (DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY())
                {
                    var customer = db.DSC_CUSTOMER.Single(cust_id => cust_id.dsc_cust_id == dsc_cust_id);
                    customer.dsc_cust_eff_end_date = null;
                    db.SaveChanges();
                    return View(List());
                }
            }
            catch { return View(List()); }
        }

        //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        #region Helpers
        public static bool ActivateCust(int dsc_cust_id)
        {
            using (DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY())
            {
                var customer = db.DSC_CUSTOMER.Single(cust_id => cust_id.dsc_cust_id == dsc_cust_id);
                customer.dsc_cust_eff_end_date = null;
                db.SaveChanges();
            }
            return true;            
        }
        //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        public static bool DeactivateCust(int dsc_cust_id)
        {
            using (DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY())
            {
                var customer = db.DSC_CUSTOMER.Single(cust_id => cust_id.dsc_cust_id == dsc_cust_id);
                customer.dsc_cust_eff_end_date = DateTime.Today;
                db.SaveChanges();
            }
             return true;
        }
        #endregion
    }
}