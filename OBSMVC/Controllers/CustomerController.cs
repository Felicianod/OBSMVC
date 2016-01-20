using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OBSMVC.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
        public ActionResult Index()
        {
            return View();
        }
        // GET: Customer
        public ActionResult List()
        {

            var customers = new List<DSC_CUSTOMER>();
            using (DSC_OBS_DEVEntities cust_ent = new DSC_OBS_DEVEntities())
            {
                customers = cust_ent.DSC_CUSTOMER.Where(cust_id => cust_id.dsc_cust_id > 0).ToList();

            }
            DateTime active_date;
            foreach (DSC_CUSTOMER customer in customers)
            {
                try
                {
                    if (String.IsNullOrEmpty(customer.dsc_cust_eff_end_date))
                    {
                        customer.dsc_cust_eff_end_date = "YES";

                    }//end of if
                    else
                    {
                        active_date = DateTime.Parse(customer.dsc_cust_eff_end_date);
                        if (active_date <= DateTime.Today)
                        {
                            customer.dsc_cust_eff_end_date = "NO";
                        }
                        else
                        {
                            customer.dsc_cust_eff_end_date = "YES";
                        }
                    }//end of else
                }//end of try
                catch
                {
                    customer.dsc_cust_eff_end_date = "NO";
                }//end of catch

            }// end of foreach
            return View(customers);

        }
        // POST: /Customer/List
        [HttpPost]
        public ActionResult List(int dsc_cust_id)
        {
            try
            {

                using (DSC_OBS_DEVEntities cust_ent = new DSC_OBS_DEVEntities())
                {
                    var customer = cust_ent.DSC_CUSTOMER.Single(cust_id => cust_id.dsc_cust_id == dsc_cust_id);
                    customer.dsc_cust_eff_end_date = null;
                    cust_ent.SaveChanges();
                    return View(List());
                }
            }
            catch { return View(List()); }


        }
#region Helpers
        public static bool ActivateCust(int dsc_cust_id)
        {
            using (DSC_OBS_DEVEntities cust_ent = new DSC_OBS_DEVEntities())
            {
                var customer = cust_ent.DSC_CUSTOMER.Single(cust_id => cust_id.dsc_cust_id == dsc_cust_id);
                customer.dsc_cust_eff_end_date = null;
                cust_ent.SaveChanges();
                
            }
            return true;
            
        }
        public static bool DeactivateCust(int dsc_cust_id)
        {
            using (DSC_OBS_DEVEntities cust_ent = new DSC_OBS_DEVEntities())
            {
                DateTime today = DateTime.Today;
                var customer = cust_ent.DSC_CUSTOMER.Single(cust_id => cust_id.dsc_cust_id == dsc_cust_id);
                customer.dsc_cust_eff_end_date = today.ToString("d");
                cust_ent.SaveChanges();

            }
             return true;

        }
        #endregion
    }
}