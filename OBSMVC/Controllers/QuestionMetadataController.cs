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
    public class QuestionMetadataController : Controller
    {
        private DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY();

        // GET: QuestionMetadata
        public ActionResult Index()
        {
            return View(db.OBS_QUESTION_METADATA.ToList());
        }

        // GET: QuestionMetadata/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OBS_QUESTION_METADATA oBS_QUESTION_METADATA = db.OBS_QUESTION_METADATA.Find(id);
            if (oBS_QUESTION_METADATA == null)
            {
                return HttpNotFound();
            }
            return View(oBS_QUESTION_METADATA);
        }

        // GET: QuestionMetadata/Create
        public ActionResult Create(int qId)
        {
            ViewBag.qId = qId;
            return View();
        }

        // POST: QuestionMetadata/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "obs_quest_md_id,obs_quest_md_value,obs_quest_md_cat, qId")] OBS_QUESTION_METADATA oBS_QUESTION_METADATA)
        {
            if (ModelState.IsValid)
            {
                db.OBS_QUESTION_METADATA.Add(oBS_QUESTION_METADATA);
                db.SaveChanges();

                string returnId = Request.QueryString["qId"];
                return RedirectToAction("Edit", "Question", new{id = returnId});
            }

            return View(oBS_QUESTION_METADATA);
        }

        // GET: QuestionMetadata/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OBS_QUESTION_METADATA oBS_QUESTION_METADATA = db.OBS_QUESTION_METADATA.Find(id);
            if (oBS_QUESTION_METADATA == null)
            {
                return HttpNotFound();
            }
            return View(oBS_QUESTION_METADATA);
        }

        // POST: QuestionMetadata/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "obs_quest_md_id,obs_quest_md_value,obs_quest_md_cat")] OBS_QUESTION_METADATA oBS_QUESTION_METADATA)
        {
            if (ModelState.IsValid)
            {
                db.Entry(oBS_QUESTION_METADATA).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(oBS_QUESTION_METADATA);
        }

        // GET: QuestionMetadata/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OBS_QUESTION_METADATA oBS_QUESTION_METADATA = db.OBS_QUESTION_METADATA.Find(id);
            if (oBS_QUESTION_METADATA == null)
            {
                return HttpNotFound();
            }
            return View(oBS_QUESTION_METADATA);
        }

        // POST: QuestionMetadata/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OBS_QUESTION_METADATA oBS_QUESTION_METADATA = db.OBS_QUESTION_METADATA.Find(id);
            db.OBS_QUESTION_METADATA.Remove(oBS_QUESTION_METADATA);
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
