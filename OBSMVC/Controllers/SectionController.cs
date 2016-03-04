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
    public class SectionController : Controller
    {
        private DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY();

        // GET: Section
        public ActionResult Index()
        {
            return View(db.OBS_FORM_SECTION.ToList());
        }

        // GET: Section/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OBS_FORM_SECTION oBS_FORM_SECTION = db.OBS_FORM_SECTION.Find(id);
            if (oBS_FORM_SECTION == null)
            {
                return HttpNotFound();
            }
            return View(oBS_FORM_SECTION);
        }

        // GET: Section/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Section/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "obs_form_section_id,obs_form_section_name")] OBS_FORM_SECTION oBS_FORM_SECTION)
        {
            if (ModelState.IsValid)
            {
                db.OBS_FORM_SECTION.Add(oBS_FORM_SECTION);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(oBS_FORM_SECTION);
        }

        // GET: Section/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OBS_FORM_SECTION oBS_FORM_SECTION = db.OBS_FORM_SECTION.Find(id);
            if (oBS_FORM_SECTION == null)
            {
                return HttpNotFound();
            }
            return View(oBS_FORM_SECTION);
        }

        // POST: Section/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "obs_form_section_id,obs_form_section_name")] OBS_FORM_SECTION oBS_FORM_SECTION)
        {
            if (ModelState.IsValid)
            {
                db.Entry(oBS_FORM_SECTION).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(oBS_FORM_SECTION);
        }

        // GET: Section/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OBS_FORM_SECTION oBS_FORM_SECTION = db.OBS_FORM_SECTION.Find(id);
            if (oBS_FORM_SECTION == null)
            {
                return HttpNotFound();
            }
            return View(oBS_FORM_SECTION);
        }

        // POST: Section/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OBS_FORM_SECTION oBS_FORM_SECTION = db.OBS_FORM_SECTION.Find(id);
            db.OBS_FORM_SECTION.Remove(oBS_FORM_SECTION);
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
