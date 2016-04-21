using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OBSMVC.Models;

namespace OBSMVC.Controllers
{
    public class LCController : Controller
    {
        private DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY();

        // GET: LC
        public ActionResult Index()
        {
            var lcs = db.DSC_LC.Where(lc_id => lc_id.dsc_lc_id > 0).ToList();
            var lcview = new List<LCViewModel>();

            Nullable<DateTime> active_date;
            string isActive;
            foreach (DSC_LC lc in lcs)
            {
                try
                {
                    if (lc.dsc_lc_eff_end_date==null)
                    {
                        isActive = "YES";


                    }//end of if
                    else
                    {
                        active_date = lc.dsc_lc_eff_end_date;
                        if (active_date <= DateTime.Today)
                        {
                            isActive = "NO";
                        }
                        else
                        {
                            isActive = "YES";
                        }
                    }//end of else
                }//end of try
                catch
                {
                    isActive = "NO";
                }//end of catch
                lcview.Add(new LCViewModel(lc.dsc_lc_id,lc.dsc_lc_name, lc.dsc_lc_code, lc.dsc_lc_timezone, isActive, isActive == "YES" ? "Deactivate" : "Activate"));
            }//end of foreach

                return View(lcview.OrderBy(x=>x.dsc_lc_name));
        }

        [HttpGet]
        public ActionResult Activate(int id, string actionText)
        {
            if (actionText == "Activate")
            {                
                var lc = db.DSC_LC.Single(lc_id => lc_id.dsc_lc_id == id);
                lc.dsc_lc_eff_end_date = null;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            else if (actionText == "Deactivate")
            {
                 DateTime today = DateTime.Today;
                 var lc = db.DSC_LC.Single(lc_id => lc_id.dsc_lc_id == id);
                 lc.dsc_lc_eff_end_date = today;
                 db.SaveChanges();
               
                return RedirectToAction("Index");
            }
            else { return HttpNotFound(); }
           
        }

        // GET: LC/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSC_LC dSC_LC = db.DSC_LC.Find(id);
            if (dSC_LC == null)
            {
                return HttpNotFound();
            }
            return View(dSC_LC);
        }

        // POST: LC/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "dsc_lc_id,dsc_lc_name,dsc_lc_code,dsc_lc_timezone,dsc_lc_eff_end_date")] DSC_LC dSC_LC)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dSC_LC).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dSC_LC);
        }

        // GET: LC/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSC_LC dSC_LC = db.DSC_LC.Find(id);
            if (dSC_LC == null)
            {
                return HttpNotFound();
            }
            return View(dSC_LC);
        }

        // POST: LC/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DSC_LC dSC_LC = db.DSC_LC.Find(id);
            db.DSC_LC.Remove(dSC_LC);
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
