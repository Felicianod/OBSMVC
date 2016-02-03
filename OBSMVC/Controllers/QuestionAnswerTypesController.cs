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
    public class QuestionAnswerTypesController : Controller
    {
        private DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY();

        // GET: QuestionAnswerTypes
        public ActionResult Index()
        {
            var oBS_QUEST_ANS_TYPES = db.OBS_QUEST_ANS_TYPES.Include(o => o.OBS_QUESTION).Include(o => o.OBS_ANS_TYPE);        
            return View(oBS_QUEST_ANS_TYPES.ToList());
        }

        // GET: QuestionAnswerTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OBS_QUEST_ANS_TYPES oBS_QUEST_ANS_TYPES = db.OBS_QUEST_ANS_TYPES.Find(id);
            if (oBS_QUEST_ANS_TYPES == null)
            {
                return HttpNotFound();
            }
            return View(oBS_QUEST_ANS_TYPES);
        }

        // GET: QuestionAnswerTypes/Create
        public ActionResult Create()
        {
            ViewBag.obs_question_id = new SelectList(db.OBS_QUESTION, "obs_question_id", "obs_question_full_text");
            ViewBag.obs_ans_type_id = new SelectList(db.OBS_ANS_TYPE, "obs_ans_type_id", "obs_ans_type_name");
            return View();
        }

        // POST: QuestionAnswerTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "obs_qat_id,obs_question_id,obs_ans_type_id,obs_qat_end_eff_dt,obs_qat_default_ans_type_yn")] OBS_QUEST_ANS_TYPES oBS_QUEST_ANS_TYPES)
        {
            if (ModelState.IsValid)
            {
                db.OBS_QUEST_ANS_TYPES.Add(oBS_QUEST_ANS_TYPES);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.obs_question_id = new SelectList(db.OBS_QUESTION, "obs_question_id", "obs_question_full_text", oBS_QUEST_ANS_TYPES.obs_question_id);
            ViewBag.obs_ans_type_id = new SelectList(db.OBS_ANS_TYPE, "obs_ans_type_id", "obs_ans_type_name", oBS_QUEST_ANS_TYPES.obs_ans_type_id);
            return View(oBS_QUEST_ANS_TYPES);
        }

        // GET: QuestionAnswerTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OBS_QUEST_ANS_TYPES oBS_QUEST_ANS_TYPES = db.OBS_QUEST_ANS_TYPES.Find(id);
            if (oBS_QUEST_ANS_TYPES == null)
            {
                return HttpNotFound();
            }
            ViewBag.obs_question_id = new SelectList(db.OBS_QUESTION, "obs_question_id", "obs_question_full_text", oBS_QUEST_ANS_TYPES.obs_question_id);
            ViewBag.obs_ans_type_id = new SelectList(db.OBS_ANS_TYPE, "obs_ans_type_id", "obs_ans_type_name", oBS_QUEST_ANS_TYPES.obs_ans_type_id);
            return View(oBS_QUEST_ANS_TYPES);
        }

        // POST: QuestionAnswerTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "obs_qat_id,obs_question_id,obs_ans_type_id,obs_qat_end_eff_dt,obs_qat_default_ans_type_yn")] OBS_QUEST_ANS_TYPES oBS_QUEST_ANS_TYPES)
        {
            if (ModelState.IsValid)
            {
                db.Entry(oBS_QUEST_ANS_TYPES).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.obs_question_id = new SelectList(db.OBS_QUESTION, "obs_question_id", "obs_question_full_text", oBS_QUEST_ANS_TYPES.obs_question_id);
            ViewBag.obs_ans_type_id = new SelectList(db.OBS_ANS_TYPE, "obs_ans_type_id", "obs_ans_type_name", oBS_QUEST_ANS_TYPES.obs_ans_type_id);
            return View(oBS_QUEST_ANS_TYPES);
        }

        // GET: QuestionAnswerTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OBS_QUEST_ANS_TYPES oBS_QUEST_ANS_TYPES = db.OBS_QUEST_ANS_TYPES.Find(id);
            if (oBS_QUEST_ANS_TYPES == null)
            {
                return HttpNotFound();
            }
            return View(oBS_QUEST_ANS_TYPES);
        }

        // POST: QuestionAnswerTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OBS_QUEST_ANS_TYPES oBS_QUEST_ANS_TYPES = db.OBS_QUEST_ANS_TYPES.Find(id);
            db.OBS_QUEST_ANS_TYPES.Remove(oBS_QUEST_ANS_TYPES);
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
