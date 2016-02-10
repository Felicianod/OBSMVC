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
        public ActionResult Edit(int id)
        {
            //if (id == null)
            //{
            //    return RedirectToAction("Index");
            //}
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
        public ActionResult displayAnswerSection(int id)
        {
            // This is a test method that accepts a question Id and the selected Index of the dropdown list 
            // and returns the section for Answers

            OBSQuestion obsQuestion = new OBSQuestion(id);
            //ViewBag.list_of_answers = obsQuestion.fullAnswerTypeDDL;

            return View(obsQuestion);
        }

        [HttpPost]
        public ActionResult displayAnswerSection(FormCollection postedData)
        {
            // Rebuild the Question Object for reuse
            int id = Convert.ToInt32(postedData["question_id"]);
            int newSelIndex = Convert.ToInt16(postedData["AnswerTypesDDL"]);
            OBSQuestion obsQuestion = new OBSQuestion(id);

            //set the ddl to the new index value based on the posted form
            obsQuestion.setAnswerTypeDDL((short)newSelIndex);

            
            // This is a test method that accepts a question Id and the selected Index of the dropdown list 
            // and returns the section for Answers

            //// Retrieve alist of all the observation answer types from the database to populate the dropdown list
            ////var all_answer_types = db.OBS_ANS_TYPE.Where(item => item.obs_ans_type_id > 0).ToList();
            //var default_answer_type = (from qat in db.OBS_QUEST_ANS_TYPES.Where(x => x.obs_question_id == id && x.obs_qat_default_ans_type_yn == "Y")
            //                           select new { 
            //                               qat_Id = qat.obs_qat_id,
            //                               at_Id = qat.obs_ans_type_id                                           
            //                           }).ToList().FirstOrDefault();

            //int at_Id = -1;
            //int qat_Id = -1;
            //try { at_Id = default_answer_type.at_Id; }
            //catch { }
            //try { qat_Id = default_answer_type.qat_Id; }
            //catch { }

            //List<SelectListItem> list_of_answers = new List<SelectListItem>();
            //foreach (var x in db.OBS_ANS_TYPE.Where(item => item.obs_ans_type_id > 0).ToList())
            //{
            //    SelectListItem answer_typeDDL = new SelectListItem();
            //    answer_typeDDL.Value = x.obs_ans_type_id.ToString();
            //    answer_typeDDL.Text = x.obs_ans_type_name;
            //    if (x.obs_ans_type_id == at_Id)
            //    {
            //        answer_typeDDL.Selected = true;
            //        //if (x.obs_ans_type_has_fxd_ans_yn == "Y")
            //        //{
            //        //    question_selected_ans_type = db.OBS_QUEST_SLCT_ANS.Where(item => item.obs_qat_id == default_answer_qat_id).ToList();
            //        //}
            //    }
            //    else
            //    {
            //        answer_typeDDL.Selected = false;
            //    }
            //    list_of_answers.Add(answer_typeDDL);
            //}
            //ViewBag.list_of_answers = list_of_answers;

            return View(obsQuestion);
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
        // =================================================================================================
        // ============================ HELPER METHODS FOR OBS_QUESTION ====================================

        public class OBSQA {
            // Constructor
            public OBSQA() { }
            // --- Properties ----
            public bool isDefaultQA { get; set; } 
            public int answerTypeId { get; set; }
            //public List<OBS_QUEST_SLCT_ANS> selectableAnsList = new List<OBS_QUEST_SLCT_ANS>();
            // --- Methods -------        
        }
        public class SelAnswerType {
            public SelAnswerType()
            {

            }
            public int indexinDDL =-1;
            public int ATid = 0;
            public string ATvalue = String.Empty;
            public string ATcathegory = String.Empty;
            public bool hasSelectableAnswers = false;
            public List<string> selAnsList = new List<string>();
        }
        public class OBSQuestion
        {       
            private DSC_OBS_DB_ENTITY OBSdb = new DSC_OBS_DB_ENTITY();

            // Constructor
            public OBSQuestion(int Id) {
                questionId = Id;
                //fullAnswerTypeList = OBSdb.OBS_ANS_TYPE.ToList();
                indexOfDefaultQA = -1;     //Set Initial Default to "No Default Found or -1"
                OBSQA_List = retrieveQAInstances(); // This method also sets the correct 'indexOfDefaultQA' and the 'hasInstances' properties.                
            }

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - \\
            //- - - - - - - - - - Properties - - - - - - - - - - - - - - - - - - - - |
            // All Properties are set at Constructor Time
            public int questionId { get; set; }
            public bool hasInstances { get; set; }
            public int indexOfDefaultQA { get; set; }
            public List<OBSQA> OBSQA_List = new List<OBSQA>();
            public List<SelectListItem> fullAnswerTypeDDL = new List<SelectListItem>();
            //public List<OBS_ANS_TYPE> fullAnswerTypeList = new List<OBS_ANS_TYPE>();
            //public int userATselId = -1;
            //public string userATcathegory;
            //public List<string> userSelectableAnsList = new List<string>();
            public SelAnswerType selectedAT = new SelAnswerType();

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -\\
            //- - - - - - - - - - - - CLASS METHODS - - - - - - - - - - - - - - - - |
            public void setAnswerTypeDDL(short obs_AT_id = 0)
            { // If the "selected" ATId is not Passed (zero value) then the Dropdown list has no selected Item

                //Loop through all the Answer Types in the Database Table
                foreach (OBS_ANS_TYPE ansTypeEntry in OBSdb.OBS_ANS_TYPE.ToList())
                {                    
                    SelectListItem ddlListItem = new SelectListItem();
                    ddlListItem.Value = ansTypeEntry.obs_ans_type_id.ToString();
                    ddlListItem.Text = ansTypeEntry.obs_ans_type_name;
                    
                    // Check if this Ans Type entry should be displayed as selected
                    // Based on the parameter received
                    if ((obs_AT_id > 0) && (ansTypeEntry.obs_ans_type_id == obs_AT_id))
                    {  //Set the current List Item as Selected
                        ddlListItem.Selected = true;
                        //Set all the "selectedAT" class values
                        selectedAT.indexinDDL = -1;    //NEEEEEEEEED THIS!!!!!!    //int
                        selectedAT.ATid = ansTypeEntry.obs_ans_type_id;               //int
                        selectedAT.ATvalue = ansTypeEntry.obs_ans_type_name;  //string
                        selectedAT.ATcathegory = ansTypeEntry.obs_ans_type_category; //string
                        //Check if the QuestionId/AnsTypeid combination exist in the "OBS_QUEST_ANS_TYPE" Table
                        // Get the Id from the "OBS_QUEST_ANS_TYPE" table
                        int QATinstanceId = (OBSdb.OBS_QUEST_ANS_TYPES.FirstOrDefault(x => x.obs_question_id == questionId || x.obs_ans_type_id == obs_AT_id)).obs_qat_id;
                        if (QATinstanceId > 0) {
                            // If the Id exist, then the Dropdown Selection has selectable answers
                            selectedAT.hasSelectableAnswers = true;
                            // Grab selectable Answer values from OBS_QUEST_SLCT_ANS table
                            selectedAT.selAnsList = OBSdb.OBS_QUEST_SLCT_ANS.Where(X => X.obs_qat_id == QATinstanceId).OrderBy(y => y.obs_qsa_order).Select(z => z.obs_qsa_text).ToList();
                        }
                        else {
                            // The curent Selection does not have selectable answers. 
                            selectedAT.hasSelectableAnswers = false;  //bool
                            // Grab selectable Answer values from harcoded list
                            selectedAT.selAnsList = getDefaultSLCT(ansTypeEntry.obs_ans_type_category);
                        }
                    }

                    // Add the List Item to the Dropdown (SelectItemList) property
                    fullAnswerTypeDDL.Add(ddlListItem);
                }
                //fullAnswerTypeDDL.ElementAt(indexOfDefaultQA).Selected = true;
            }

            private List<OBSQA> retrieveQAInstances()
            {
                List<OBSQA> QAlistFound = new List<OBSQA>();
                
                // Firt get a list of all the QAType Instances for this Question Id (If Any) in the 'OBS_QUEST_ANS_TYPES' Table
                List<OBS_QUEST_ANS_TYPES> QAInstances = OBSdb.OBS_QUEST_ANS_TYPES.Where(x => x.obs_question_id == questionId).ToList();

                if (QAInstances.Count() == 0)  //There were no records found in the 'OBS_QUEST_ANS_TYPES' Table for this question Id
                {
                    hasInstances = false;
                    indexOfDefaultQA = -1;             //There is no default instance (Nothing Found)
                    //fullAnswerTypeList = null;         // There are no QustionAnswer instances Found
                }
                else  //The Selected Question Id has at least on instance in the 'OBS_QUEST_ANS_TYPES' Table
                {
                    hasInstances = true;
                    int index = 0;
                    // loop through each of the instances found and build the "obsQA_List" list property of the "OBSQuestion" Object
                    foreach (OBS_QUEST_ANS_TYPES qaInstanceTemp in QAInstances)
                    {
                        OBSQA myQAinstance = new OBSQA();
                        myQAinstance.answerTypeId = qaInstanceTemp.obs_ans_type_id;
                        myQAinstance.isDefaultQA = qaInstanceTemp.obs_qat_default_ans_type_yn == "Y" ? true : false;
                        // Check if this instance is the default. If so, set the Drop down select list to the correct default selected value
                        if (myQAinstance.isDefaultQA) {
                            // If this is the default QA instance, then this is the selected Answer Type to Use for Display
                            indexOfDefaultQA = index;
                            //Rebuild the dropdown with the new selected Value
                            setAnswerTypeDDL(qaInstanceTemp.obs_ans_type_id);
                        }  // Set the Index of the default type Instance
                        
                        //myQAinstance.selectableAnsList = OBSdb.OBS_QUEST_SLCT_ANS.Where(x => x.obs_qat_id == qaInstanceTemp.obs_qat_id).OrderBy(y => y.obs_qsa_order).ToList();
                        QAlistFound.Add(myQAinstance);
                        index++;
                    }
                    if (indexOfDefaultQA < 0)
                    {   // No default value was found so no DDL List was created
                        //CreateDatabaseIfNotExists a dropdown list with no seletced Value
                        setAnswerTypeDDL(0);
                    }
                }
                return QAlistFound;
            }

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
            public List<string> getDefaultSLCT( string category)
            {

                List<string> q_selected_ans_type = new List<string>();
                switch (category)
                {

                    case "3 Val Range":
                        q_selected_ans_type.Add("Bad");
                        q_selected_ans_type.Add("OK");
                        q_selected_ans_type.Add("Good");
                        break;
                    case "5 Val Range":
                        q_selected_ans_type.Add("Monday");
                        q_selected_ans_type.Add("Tuesday");
                        q_selected_ans_type.Add("Wednesday");
                        q_selected_ans_type.Add("Thursday");
                        q_selected_ans_type.Add("Friday");
                        break;
                    case "MS List":
                        q_selected_ans_type.Add("MS List Item 1");
                        q_selected_ans_type.Add("MS List Item 2");
                        q_selected_ans_type.Add("MS List Item 3");
                        q_selected_ans_type.Add("MS List Item 4");
                        q_selected_ans_type.Add("MS List Item 5");
                        break;
                    case "SS List":
                        q_selected_ans_type.Add("Single Selected List Item 1");
                        q_selected_ans_type.Add("Single Selected List Item 2");
                        q_selected_ans_type.Add("Single Selected List Item 3");
                        q_selected_ans_type.Add("Single Selected List Item 4");
                        break;

                }//end of switch    
                return q_selected_ans_type;        
            }
            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        }

    }
}
