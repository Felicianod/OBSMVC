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
    public class QuestionController : Controller
    {
        private DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY();

        // GET: Question
        public ActionResult Index()
        {
            return View(db.OBS_QUESTION.ToList());
        }

        // GET: Question/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OBS_QUESTION oBS_QUESTION = db.OBS_QUESTION.Find(id);
            if (oBS_QUESTION == null)
            {
                return HttpNotFound();
            }
            return View(oBS_QUESTION);
        }

        // GET: Question/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Question/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "obs_question_id,obs_question_ver,obs_question_full_text,obs_question_short_text,obs_question_desc,obs_question_mm_url,obs_question_eff_st_dt,obs_question_eff_end_dt")] OBS_QUESTION oBS_QUESTION)
        {
            if (ModelState.IsValid)
            {
                db.OBS_QUESTION.Add(oBS_QUESTION);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(oBS_QUESTION);
        }

        // GET: Question/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OBS_QUESTION oBS_QUESTION = db.OBS_QUESTION.Find(id);
            if (oBS_QUESTION == null)
            {
                return HttpNotFound();
            }
            return View(oBS_QUESTION);
        }

        // POST: Question/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "obs_question_id,obs_question_ver,obs_question_full_text,obs_question_short_text,obs_question_desc,obs_question_mm_url,obs_question_eff_st_dt,obs_question_eff_end_dt")] OBS_QUESTION oBS_QUESTION)
        {
            if (ModelState.IsValid)
            {
                db.Entry(oBS_QUESTION).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(oBS_QUESTION);
        }

        // GET: Question/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OBS_QUESTION oBS_QUESTION = db.OBS_QUESTION.Find(id);
            if (oBS_QUESTION == null)
            {
                return HttpNotFound();
            }
            return View(oBS_QUESTION);
        }

        // POST: Question/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OBS_QUESTION oBS_QUESTION = db.OBS_QUESTION.Find(id);
            db.OBS_QUESTION.Remove(oBS_QUESTION);
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
