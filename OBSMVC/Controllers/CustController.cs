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
using PagedList;
using PagedList.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBSMVC.Controllers
{
   
    public class CustController : Controller
    {
        private DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY();

        //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        // GET: Cust
        [HttpGet]
        public ActionResult Index(int? page, int? PageSize, string sortBy)
        {
            var customers = new List<DSC_CUSTOMER>();
            var viewCustomers = new List<CustViewModel>();
            ViewBag.CurrentItemsPerPage = PageSize ?? 10;
            ViewBag.SortNameParameter = String.IsNullOrEmpty(sortBy) ? "Name desc" : "Name";
            ViewBag.SortParentParameter = sortBy == "Parent" ? "Parent desc" : "Parent"; 

            using (DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY())
            {
                customers = db.DSC_CUSTOMER.Where(cust_id => cust_id.dsc_cust_id > 0).ToList();
            }
            //DateTime active_date;
            foreach (DSC_CUSTOMER customer in customers)
            {
                string activeAction = "";
                try
                {
                    if (customer.dsc_cust_eff_end_date == null)
                    {
                        activeAction = "YES";

                    }//end of if
                    else
                    {
                        if (customer.dsc_cust_eff_end_date <= DateTime.Today)
                        {
                            activeAction = "NO";
                        }
                        else
                        {
                            activeAction = "YES";
                        }
                    }//end of else
                }//end of try
                catch
                {
                    activeAction = "NO";
                }//end of catch

                viewCustomers.Add(new CustViewModel(customer.dsc_cust_id, customer.dsc_cust_name, customer.dsc_cust_parent_name, activeAction, activeAction == "YES" ? "Deactivate" : "Activate"));
            }// end of foreach
            switch (sortBy)
            {
                case "Name desc":
                    return View(viewCustomers.OrderByDescending(x=>x.dsc_cust_name).ToPagedList(page ?? 1, PageSize ?? 10));
                case "Parent desc":
                    return View(viewCustomers.OrderByDescending(x => x.dsc_cust_parent_name).ToPagedList(page ?? 1, PageSize ?? 10));
                case"Name":
                    return View(viewCustomers.OrderBy(x => x.dsc_cust_name).ToPagedList(page ?? 1, PageSize ?? 10));
                case "Parent":
                    return View(viewCustomers.OrderBy(x => x.dsc_cust_parent_name).ToPagedList(page ?? 1, PageSize ?? 10));
                default: return View(viewCustomers.ToPagedList(page ?? 1, PageSize ?? 10));


            }

            
            
        }
        //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
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
                    var customer = db.DSC_CUSTOMER.Single(cust_id => cust_id.dsc_cust_id == id);
                    customer.dsc_cust_eff_end_date = DateTime.Today;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        
        //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
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

        //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
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

        //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
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

        //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
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
