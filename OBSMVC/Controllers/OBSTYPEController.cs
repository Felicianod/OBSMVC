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
    public class OBSTYPEController : Controller
    {
        private DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY();

        // GET: OBSTYPE
        public ActionResult Index()
        {
            var oBS_TYPE = db.OBS_TYPE.Include(o => o.OBS_SUPER_TYPE);
            return View(oBS_TYPE.ToList());
        }

        //// GET: OBSTYPE/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    OBS_TYPE oBS_TYPE = db.OBS_TYPE.Find(id);
        //    if (oBS_TYPE == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(oBS_TYPE);
        //}

        // GET: OBSTYPE/Create
        public ActionResult Create()
        {
            ViewBag.obs_super_type_id = new SelectList(db.OBS_SUPER_TYPE, "obs_super_type_id", "obs_super_type_name");
            return View();
        }

        // POST: OBSTYPE/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "obs_type_id,obs_super_type_id,obs_type_name,obs_type_desc,obs_type_eff_st_dt,obs_type_eff_end_dt")] OBS_TYPE oBS_TYPE)
        {
            if (ModelState.IsValid)
            {
                db.OBS_TYPE.Add(oBS_TYPE);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.obs_super_type_id = new SelectList(db.OBS_SUPER_TYPE, "obs_super_type_id", "obs_super_type_name", oBS_TYPE.obs_super_type_id);
            return View(oBS_TYPE);
        }

        // GET: OBSTYPE/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OBS_TYPE oBS_TYPE = db.OBS_TYPE.Find(id);
            if (oBS_TYPE == null)
            {
                return HttpNotFound();
            }
            ViewBag.obs_super_type_id = new SelectList(db.OBS_SUPER_TYPE, "obs_super_type_id", "obs_super_type_name", oBS_TYPE.obs_super_type_id);
            return View(oBS_TYPE);
        }

        // POST: OBSTYPE/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "obs_type_id,obs_super_type_id,obs_type_name,obs_type_desc,obs_type_eff_st_dt,obs_type_eff_end_dt")] OBS_TYPE oBS_TYPE)
        {
            if (ModelState.IsValid)
            {
                db.Entry(oBS_TYPE).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.obs_super_type_id = new SelectList(db.OBS_SUPER_TYPE, "obs_super_type_id", "obs_super_type_name", oBS_TYPE.obs_super_type_id);
            return View(oBS_TYPE);
        }

        // GET: OBSTYPE/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OBS_TYPE oBS_TYPE = db.OBS_TYPE.Find(id);
            if (oBS_TYPE == null)
            {
                return HttpNotFound();
            }
            return View(oBS_TYPE);
        }

        // POST: OBSTYPE/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OBS_TYPE oBS_TYPE = db.OBS_TYPE.Find(id);
            db.OBS_TYPE.Remove(oBS_TYPE);
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
