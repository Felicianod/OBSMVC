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

            _listAnswerTypes((int)id);
            return View(obsQMD);

        }

        //-----------------------------------------------------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection postedData, QuestionMDViewModel QuestionMDView,
                                 [Bind(Prefix = "q")] OBS_QUESTION questionHdr )
        {

            QuestionMDView.q = questionHdr;
            //QuestionMDView.q.obs_question_full_text = (string)postedData["q.obs_question_full_text"];
            //QuestionMDView.q.obs_question_id = Convert.ToInt32(postedData["q.obs_question_id"]);
            //QuestionMDView.q.obs_question_short_text = (string)postedData["q.obs_question_short_text"];
            //QuestionMDView.q.obs_question_desc = (string)postedData["q.obs_question_desc"];
            //QuestionMDView.q.obs_question_mm_url = (string)postedData["q.obs_question_mm_url"];
            //QuestionMDView.q.obs_question_eff_end_dt = Convert.ToDateTime(postedData["q.obs_question_eff_end_dt"]);
            //QuestionMDView.q.obs_question_eff_st_dt = Convert.ToDateTime(postedData["q.obs_question_eff_st_dt"]);

            if (!ModelState.IsValid) // Model State is not Valid return Errors
            {
                ViewBag.ConfMsg = "Failed to Update Information!";
                return View(QuestionMDView);
            }

            //-------- Save the Question Information ----
            using (DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY())
            {
                OBS_QUESTION editedQuestion = db.OBS_QUESTION.Single(x => x.obs_question_id == questionHdr.obs_question_id);

                if (!editedQuestion.obs_question_full_text.Equals(questionHdr.obs_question_full_text))
                {
                    editedQuestion.obs_question_ver++;
                }
                editedQuestion.obs_question_full_text = QuestionMDView.q.obs_question_full_text;
                editedQuestion.obs_question_short_text = QuestionMDView.q.obs_question_short_text;
                //question.obs_question_desc = QuestionMDView.q.obs_question_desc;
                //question.obs_question_mm_url = QuestionMDView.q.obs_question_mm_url;
                editedQuestion.obs_question_eff_st_dt = QuestionMDView.q.obs_question_eff_st_dt;
                editedQuestion.obs_question_eff_end_dt = QuestionMDView.q.obs_question_eff_end_dt;
                editedQuestion.obs_question_upd_dtm = DateTime.Now;
                editedQuestion.obs_question_upd_uid = User.Identity.Name;

                //db.Entry(editQuestion).State = EntityState.Modified;
                
                 db.SaveChanges();
            }

            // ------- Save the Question Metadata Changes ----
            string MDlistBefore = postedData["origTags"];
            string MDlistAfter = postedData["qAssignedMD"];
            List<string> originalMDList = new List<string>();
            List<string> newMDList = new List<string>();
            if (MDlistBefore != null) { originalMDList = MDlistBefore.Split(',').ToList(); }
            if (MDlistAfter != null) { newMDList = MDlistAfter.Split(',').ToList(); }
            string[] mdIDsToDelete = originalMDList.Except(newMDList).ToArray();
            string[] mdIDsToAdd = newMDList.Except(originalMDList).ToArray();

            using (DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY())
            {
                //---- Soft Delete all Metadata tags that are no longer used by the question
                foreach (string deleteId in mdIDsToDelete)
                {
                    int tempId = Convert.ToInt32(deleteId);
                    //int joitemp = db.OBS_QUEST_ASSGND_MD.Where(x => x.obs_quest_md_id == Convert.ToInt32(deleteId) && x.obs_question_id == questionHdr.obs_question_id).Select(x => x.obs_qad_id);
                    OBS_QUEST_ASSGND_MD oBS_QUEST_ASSGND_MD = db.OBS_QUEST_ASSGND_MD.FirstOrDefault(x => x.obs_quest_md_id == tempId && x.obs_question_id == questionHdr.obs_question_id);
                    oBS_QUEST_ASSGND_MD.obs_qad_eff_end_dt = DateTime.Today;
                    //db.OBS_QUESTION_METADATA.Remove(oBS_QUESTION_METADATA);  //No hard deletes
                }

                //---- Enable or Add the New metadata (MD) that will be assigned to the question ---
                //-- Process each metadata entry to add for the selected question
                foreach (string mdIdtoAdd in mdIDsToAdd)
                {
                    int tempId = Convert.ToInt32(mdIdtoAdd);
                    //-- Look for an entry in OBS_QUEST_ASSGND_MD usign the Question Id and the metadata it.
                    OBS_QUEST_ASSGND_MD oBS_QUEST_ASSGND_MD = db.OBS_QUEST_ASSGND_MD.FirstOrDefault(x => x.obs_quest_md_id == tempId && x.obs_question_id == questionHdr.obs_question_id);
                    //-- If an entry is found in the junction table for that question, just enable it
                    if (oBS_QUEST_ASSGND_MD != null && tempId > 0)
                    {
                        oBS_QUEST_ASSGND_MD.obs_qad_eff_end_dt = Convert.ToDateTime("12/31/2060");
                    }
                    else
                    { //-- If selected MD does not exist in the junction table for that question, add it.
                        oBS_QUEST_ASSGND_MD = new OBS_QUEST_ASSGND_MD();
                        oBS_QUEST_ASSGND_MD.obs_quest_md_id = tempId;
                        oBS_QUEST_ASSGND_MD.obs_question_id = questionHdr.obs_question_id;
                        oBS_QUEST_ASSGND_MD.obs_qad_eff_st_dt = DateTime.Today;
                        oBS_QUEST_ASSGND_MD.obs_qad_eff_end_dt = Convert.ToDateTime("12/31/2060");
                        db.OBS_QUEST_ASSGND_MD.Add(oBS_QUEST_ASSGND_MD);                    
                    }
                }

                //---- Save All Changes ------
                db.SaveChanges();           
            }

            ViewBag.ConfMsg = "Success! Data Saved Successfully";

            //return View(QuestionMDView);
            return RedirectToAction("Edit", "Question", new { id = questionHdr.obs_question_id });
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


        // ==================================================================================================
        [ChildActionOnly]
        public void _listAnswerTypes(int question_id)
        {
            var all_answer_types = db.OBS_ANS_TYPE.Where(item => item.obs_ans_type_id > 0).ToList();
            var default_answer_type = (from qat in db.OBS_QUEST_ANS_TYPES.Where(x => x.obs_question_id == question_id && x.obs_qat_default_ans_type_yn == "Y")
                                       select new { qat.obs_ans_type_id, qat.obs_qat_id }).ToList();
            int default_answer_id = -1;
            int default_answer_qat_id = -1;
            List<OBS_QUEST_SLCT_ANS> question_selected_ans_type = new List<OBS_QUEST_SLCT_ANS>();

            try
            {
                default_answer_id = default_answer_type.FirstOrDefault().obs_ans_type_id;
                default_answer_qat_id = default_answer_type.FirstOrDefault().obs_qat_id;
            }
            catch (NullReferenceException)
            { }

            List<SelectListItem> list_of_answers = new List<SelectListItem>();
            foreach (var x in all_answer_types)
            {
                SelectListItem answer_type = new SelectListItem();
                answer_type.Value = x.obs_ans_type_id.ToString();
                answer_type.Text = x.obs_ans_type_name;
                if (x.obs_ans_type_id == default_answer_id)
                {
                    answer_type.Selected = true;
                    if (x.obs_ans_type_has_fxd_ans_yn == "Y")
                    {
                        question_selected_ans_type = db.OBS_QUEST_SLCT_ANS.Where(item => item.obs_qat_id == default_answer_qat_id).ToList();
                    }

                }
                else
                {
                    answer_type.Selected = false;
                }
                list_of_answers.Add(answer_type);
            }
            ViewBag.list_of_answers = list_of_answers;
            ViewBag.question_selected_ans_type = question_selected_ans_type;

        }
        // =================================================================================

    }
}
