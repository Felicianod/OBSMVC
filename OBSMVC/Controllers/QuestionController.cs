﻿using System;
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

        [HttpGet]  // GET: Question/Create
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
                oBS_QUESTION.obs_question_ver = 1;
                oBS_QUESTION.obs_question_added_uid = User.Identity.Name;
                oBS_QUESTION.obs_question_added_dtm = DateTime.Now;

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
        public ActionResult Edit([Bind(Include = "obs_question_id,obs_question_ver,obs_question_full_text,obs_question_short_text,obs_question_desc,obs_question_mm_url,obs_question_eff_st_dt,obs_question_eff_end_dt, obs_question_added_dtm, obs_question_added_uid")] OBS_QUESTION oBS_QUESTION)
        {
            using (DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY())
            {
                var question = db.OBS_QUESTION.Single(x => x.obs_question_id == oBS_QUESTION.obs_question_id);

                if (!oBS_QUESTION.obs_question_full_text.Equals(question.obs_question_full_text))
                {
                    question.obs_question_ver++;               
                }
                question.obs_question_full_text = oBS_QUESTION.obs_question_full_text;
                question.obs_question_short_text = oBS_QUESTION.obs_question_short_text;
                question.obs_question_desc = oBS_QUESTION.obs_question_desc;
                question.obs_question_mm_url = oBS_QUESTION.obs_question_mm_url;
                question.obs_question_eff_st_dt = oBS_QUESTION.obs_question_eff_st_dt;
                question.obs_question_eff_end_dt = oBS_QUESTION.obs_question_eff_end_dt;
                question.obs_question_upd_dtm = DateTime.Now;
                question.obs_question_upd_uid = User.Identity.Name;

                db.SaveChanges();
                ViewBag.ConfMsg = "Success";
                return View(question);
            }
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

        [HttpGet]
        public ActionResult Manage(string searchKeyWords)
        {
            if (!String.IsNullOrEmpty(searchKeyWords))
            {// Default pulls all Question Data in a list.
                ViewBag.searchTerm = "";
                return View(db.OBS_QUESTION.ToList());
            }
            else
            {
                // Filter view by search Keywords
                ViewBag.searchTerm = searchKeyWords;
                var questionList = db.OBS_QUESTION.Where(x => x.obs_question_full_text.Contains(searchKeyWords));
                //if (questionList.ToList().Count > 0)
                //{
                //    return View(questionList.ToList());
                //}
                return View(questionList.ToList());
                //else
                //{
                //    try
                //    {
                //        string[] words = search.Split(' ');
                //        string word0 = words[0];
                //        string word1 = words[1];
                //        return View(employeeList.Where(emp => (emp.dsc_emp_first_name.Contains(word0) && emp.dsc_emp_last_name.Contains(word1)) || (emp.dsc_emp_first_name.Contains(word1) && emp.dsc_emp_last_name.Contains(word0))).ToList().ToPagedList(page ?? 1, PageSize ?? 10));
                //    }
                //    catch
                //    {
                //        return View(employeeList.Where(emp => emp.dsc_emp_last_name.Contains(search) || emp.dsc_emp_first_name.Contains(search) || emp.DSC_LC.dsc_lc_name.Contains(search) || emp.dsc_emp_perm_id.ToString().Contains(search) || emp.dsc_emp_adp_id.Contains(search) || emp.dsc_emp_email_addr.Contains(search)).ToList().ToPagedList(page ?? 1, PageSize ?? 10));
                //    }
                //}
                
                
            }
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
