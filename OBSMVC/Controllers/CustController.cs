using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OBSMVC;
using OBSMVC.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBSMVC.Controllers
{
   
    public class CustController : Controller
    {
        private DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY();

        // GET: Cust
        [HttpGet]
        public ActionResult Index()
        {
            var customers = new List<DSC_CUSTOMER>();
            var viewCustomers = new List<CustViewModel>();

            using (DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY())
            {
                customers = db.DSC_CUSTOMER.Where(cust_id => cust_id.dsc_cust_id > 0).ToList();

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

                viewCustomers.Add(new CustViewModel(customer.dsc_cust_id, customer.dsc_cust_name,customer.dsc_cust_parent_name,customer.dsc_cust_eff_end_date,customer.dsc_cust_eff_end_date=="YES"?"Deactivate":"Activate"));
            }// end of foreach

            return View(viewCustomers);
            
        }
  
        [HttpGet]
        public ActionResult Activate(int id, string actionText)
        {
            if (actionText == "Activate")
            {
                using (DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY())
                {
                    var customer = db.DSC_CUSTOMER.Single(cust_id => cust_id.dsc_cust_id == id);
                    customer.dsc_cust_eff_end_date = null;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            else if (actionText == "Deactivate")
            {
                using (DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY())
                {
                    DateTime today = DateTime.Today;
                    var customer = db.DSC_CUSTOMER.Single(cust_id => cust_id.dsc_cust_id == id);
                    customer.dsc_cust_eff_end_date = today.ToString("d");
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // POST: Cust/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "dsc_cust_id,dsc_cust_name,dsc_cust_parent_name,dsc_cust_eff_end_date")] DSC_CUSTOMER dSC_CUSTOMER)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dSC_CUSTOMER).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dSC_CUSTOMER);
        }

        // GET: Cust/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSC_CUSTOMER dSC_CUSTOMER = db.DSC_CUSTOMER.Find(id);
            if (dSC_CUSTOMER == null)
            {
                return HttpNotFound();
            }
            return View(dSC_CUSTOMER);
        }

        // POST: Cust/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DSC_CUSTOMER dSC_CUSTOMER = db.DSC_CUSTOMER.Find(id);
            db.DSC_CUSTOMER.Remove(dSC_CUSTOMER);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
