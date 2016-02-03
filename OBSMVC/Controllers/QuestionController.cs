using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OBSMVC.Models;
using PagedList;
using PagedList.Mvc;

namespace OBSMVC.Controllers
{
    public class QuestionController : Controller
    {
        private DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY();
        //-----------------------------------------------------------------------------------------------------------------
        // GET: Question
        public ActionResult Index( string search, string includeActiveOnly, int? page, int? PageSize)
        {
            ViewBag.CurrentItemsPerPage = PageSize ?? 10;
            if (!String.IsNullOrWhiteSpace(search) && includeActiveOnly == "on")
            {

                List<OBS_QUESTION> l1 = db.OBS_QUESTION.Where(ques => ques.obs_question_full_text.Contains(search) && DateTime.Today >= ques.obs_question_eff_st_dt && DateTime.Today < ques.obs_question_eff_end_dt).ToList();
                List<OBS_QUESTION> l2 = db.OBS_QUESTION.Where(ques => ques.OBS_QUEST_ASSGND_MD.Any(e =>(e.OBS_QUESTION_METADATA.obs_quest_md_cat.Contains(search)|| e.OBS_QUESTION_METADATA.obs_quest_md_value.Contains(search)) && DateTime.Today >= ques.obs_question_eff_st_dt && DateTime.Today < ques.obs_question_eff_end_dt)).ToList();
                var combined = l1.Union(l2);               
                return View(combined.ToPagedList(page ?? 1, PageSize ?? 10));
            }
            else if (!String.IsNullOrWhiteSpace(search) && String.IsNullOrWhiteSpace(includeActiveOnly))
            {
                List<OBS_QUESTION> l1 = db.OBS_QUESTION.Where(ques => ques.obs_question_full_text.Contains(search)).ToList();
                List<OBS_QUESTION> l2 = db.OBS_QUESTION.Where(ques => ques.OBS_QUEST_ASSGND_MD.Any(e => e.OBS_QUESTION_METADATA.obs_quest_md_cat.Contains(search) || e.OBS_QUESTION_METADATA.obs_quest_md_value.Contains(search))).ToList();
                var combined = l1.Union(l2);
                return View(combined.ToPagedList(page ?? 1, PageSize ?? 10));
            }
            else if (String.IsNullOrWhiteSpace(search) && includeActiveOnly == "on")
            {
                return View(db.OBS_QUESTION.Where(ques => DateTime.Today >= ques.obs_question_eff_st_dt && DateTime.Today < ques.obs_question_eff_end_dt).ToList().ToPagedList(page ?? 1, PageSize ?? 10));
            }

            else { return View(db.OBS_QUESTION.Where(ques => DateTime.Today >= ques.obs_question_eff_st_dt && DateTime.Today < ques.obs_question_eff_end_dt).ToList().ToPagedList(page ?? 1, PageSize ?? 10)); }
            
        }
        //-----------------------------------------------------------------------------------------------------------------
        // GET: Question/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            /*OBS_QUESTION oBS_QUESTION = db.OBS_QUESTION.Find(id);
             if (oBS_QUESTION == null)
             {
                 return HttpNotFound();
             }
            return View(oBS_QUESTION);*/

            QuestionMDViewModel obsQMD = new QuestionMDViewModel((int)id);
            if (obsQMD == null)
            {
                return HttpNotFound();
            }
            return View(obsQMD);


        }
        //-----------------------------------------------------------------------------------------------------------------
        [HttpGet]  // GET: Question/Create
        public ActionResult Create()
        {
            return View();
        }
        //-----------------------------------------------------------------------------------------------------------------
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

        //-----------------------------------------------------------------------------------------------------------------
        // GET: Question/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            //OBS_QUESTION oBS_QUESTION = db.OBS_QUESTION.Find(id);
            //if (oBS_QUESTION == null)
            //{
            //    return HttpNotFound();
            //}
            //ViewBag.mdTags = db.OBS_QUESTION_METADATA.ToList();
            
            // Populate the new QuestionMD Model from the selected Id and forward it to the View
            QuestionMDViewModel obsQMD = new QuestionMDViewModel((int)id);
            if (obsQMD == null) {
                return HttpNotFound();
            }
            return View(obsQMD);

        }

        //-----------------------------------------------------------------------------------------------------------------
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection postedData, QuestionMDViewModel QuestionMDView,
                                 [Bind(Prefix = "q")] OBS_QUESTION questionHdr)
        {
            questionHdr.obs_question_added_uid = "999";
            QuestionMDView.q = questionHdr;
            //QuestionMDView.q.obs_question_full_text = (string)postedData["q.obs_question_full_text"];
            //QuestionMDView.q.obs_question_id = Convert.ToInt32(postedData["q.obs_question_id"]);
            //QuestionMDView.q.obs_question_short_text = (string)postedData["q.obs_question_short_text"];
            //QuestionMDView.q.obs_question_desc = (string)postedData["q.obs_question_desc"];
            //QuestionMDView.q.obs_question_mm_url = (string)postedData["q.obs_question_mm_url"];
            //QuestionMDView.q.obs_question_eff_end_dt = Convert.ToDateTime(postedData["q.obs_question_eff_end_dt"]);
            //QuestionMDView.q.obs_question_eff_st_dt = Convert.ToDateTime(postedData["q.obs_question_eff_st_dt"]);

            QuestionMDViewModel newQMD = new QuestionMDViewModel(questionHdr.obs_question_id);

            //if (!ModelState.IsValid) // Model State is not Valid return Errors
            if (questionHdr.obs_question_id < 1) // Model State is not Valid return Errors
            {
                newQMD.q = QuestionMDView.q;
                ViewBag.ConfMsg = "Failed to Update Information!";
                return View(newQMD);
            }

            using (DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY())
            {
                //var question = db.OBS_QUESTION.Single(x => x.obs_question_id == oBS_QUESTION.obs_question_id);


                //var question = newQMD.q;

                if (!QuestionMDView.q.obs_question_full_text.Equals(newQMD.q.obs_question_full_text))
                {
                    newQMD.q.obs_question_ver++;
                }
                newQMD.q.obs_question_full_text = QuestionMDView.q.obs_question_full_text;
                newQMD.q.obs_question_short_text = QuestionMDView.q.obs_question_short_text;
                //question.obs_question_desc = QuestionMDView.q.obs_question_desc;
                //question.obs_question_mm_url = QuestionMDView.q.obs_question_mm_url;
                newQMD.q.obs_question_eff_st_dt = QuestionMDView.q.obs_question_eff_st_dt;
                newQMD.q.obs_question_eff_end_dt = QuestionMDView.q.obs_question_eff_end_dt;
                newQMD.q.obs_question_upd_dtm = DateTime.Now;
                newQMD.q.obs_question_upd_uid = User.Identity.Name;

                db.SaveChanges();

                //QuestionMDView.q = newQMD.q;
                ViewBag.ConfMsg = "Success! Data Saved Successfully";

                //return View(QuestionMDView);
                return RedirectToAction("Edit", "Question", new { id = newQMD.q.obs_question_id });
            }
        }


        //-----------------------------------------------------------------------------------------------------------------
        // GET: QuestionMetadata
        [ChildActionOnly]
        [OutputCache(Duration =2000)]
        public ActionResult qMetaDataList()
        {
            return View(db.OBS_QUESTION_METADATA.ToList());
        }
        //-----------------------------------------------------------------------------------------------------------------
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
        //-----------------------------------------------------------------------------------------------------------------
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
        //-----------------------------------------------------------------------------------------------------------------
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

        //-----------------------------------------------------------------------------------------------------------------
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        //-----------------------------------------------------------------------------------------------------------------

    }
}
