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
        //------------------------------------- INDEX  [GET] ---------------------------------------------------------------------
        [HttpGet]
        public ActionResult Index(string search, string includeActiveOnly, int? page, int? PageSize)
        {
            if (search != "" || page != null) { ViewBag.firstPageDisplay = "N"; }
            ViewBag.CurrentItemsPerPage = PageSize ?? 10;
            if (!String.IsNullOrEmpty(includeActiveOnly))
            {// If the "includeActiveOnly" parameter is not null, we assume it's "on".  (It can only be "null" or "on")
                ViewBag.searchActiveOnly = "Y";
            }
            else
            {
                ViewBag.searchActiveOnly = "N";
            }

            if (!String.IsNullOrWhiteSpace(search) && includeActiveOnly == "on")
            {
                List<OBS_QUESTION> l1 = db.OBS_QUESTION.Where(ques => ques.obs_question_full_text.Contains(search) && DateTime.Today >= ques.obs_question_eff_st_dt && DateTime.Today < ques.obs_question_eff_end_dt).ToList();
                List<OBS_QUESTION> l2 = db.OBS_QUESTION.Where(ques => ques.OBS_QUEST_ASSGND_MD.Any(e => (e.OBS_QUESTION_METADATA.obs_quest_md_cat.Contains(search) || e.OBS_QUESTION_METADATA.obs_quest_md_value.Contains(search)) && DateTime.Today >= ques.obs_question_eff_st_dt && DateTime.Today < ques.obs_question_eff_end_dt)).ToList();
                var combined = l1.Union(l2);
                return View(combined.ToPagedList(page ?? 1, PageSize ?? 10));
            }
            else if (!String.IsNullOrWhiteSpace(search) && String.IsNullOrWhiteSpace(includeActiveOnly))
            {
                List<OBS_QUESTION> l1 = db.OBS_QUESTION.Where(ques => ques.obs_question_full_text.Contains(search)).ToList();
                List<OBS_QUESTION> l2 = db.OBS_QUESTION.Where(ques => ques.OBS_QUEST_ASSGND_MD.Any(e => (e.OBS_QUESTION_METADATA.obs_quest_md_cat.Contains(search)) || e.OBS_QUESTION_METADATA.obs_quest_md_value.Contains(search))).ToList();
                var combined = l1.Union(l2);
                return View(combined.ToPagedList(page ?? 1, PageSize ?? 10));
            }
            else if (String.IsNullOrWhiteSpace(search) && includeActiveOnly == "on")
            {
                return View(db.OBS_QUESTION.Where(ques => DateTime.Today >= ques.obs_question_eff_st_dt && DateTime.Today < ques.obs_question_eff_end_dt).ToList().ToPagedList(page ?? 1, PageSize ?? 10));
            }

            else { return View(db.OBS_QUESTION.Where(ques => DateTime.Today >= ques.obs_question_eff_st_dt && DateTime.Today < ques.obs_question_eff_end_dt).ToList().ToPagedList(page ?? 1, PageSize ?? 10)); }

        }
        
        //--------------------------------------- DETAILS [GET] ------------------------------------------------------------------
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

        //---------------------------------------- ADDUPDATE  [GET] -----------------------------------------------------------------

        //[HttpGet]
        //public ActionResult QuestionCreate()
        //{
        //    QuestionCreateViewModel obsQMD = new QuestionCreateViewModel();
        //    obsQMD.questn.obs_question_eff_st_dt = DateTime.Today;
        //    obsQMD.questn.obs_question_eff_end_dt = Convert.ToDateTime("2060/12/31");
        //    return View("QuestionAddUpdate", obsQMD);
        //}

        //---------------------------------------- QuestionAddUpd [ GET ] -----------------------------------------------------------------
        [HttpGet]
        [ActionName("QuestionAddUpdate")]
        public ActionResult QuestionAddUpdateEdit(int? id)
        {
            int questionId = id ?? -1;
            QuestionCreateEditViewModel obsQCVM;


            if (questionId < 1)
            {
                obsQCVM = new QuestionCreateEditViewModel();
                obsQCVM.questn.obs_question_eff_st_dt = DateTime.Now;
                obsQCVM.questn.obs_question_eff_end_dt = Convert.ToDateTime("2060/12/31");
            }
            else
            {
                obsQCVM = new QuestionCreateEditViewModel(questionId);
                foreach (qatTags qatTag in obsQCVM.Quest_Assigned_qatTags)
                {
                    if (db.OBS_COL_FORM_QUESTIONS.Where(item => item.obs_qat_id == qatTag.QAT.obs_qat_id).Count() == 0)
                    {
                        qatTag.editable = "true";
                    }
                }                
            }
            
            return View("QuestionAddUpdate", obsQCVM);

        }


        //---------------------------------------- QuestionAddUpd [POST] -----------------------------------------------------------------
        [HttpPost]        
        [ActionName("QuestionAddUpdate")]
        [ValidateAntiForgeryToken]
        public ActionResult QuestionAddUpdatePost(FormCollection postedData, QuestionMDViewModel QuestionMDView,
                                 [Bind(Prefix = "questn")] OBS_QUESTION questionHdr, string ans_type_list)
        {
            string posted_deleted_ids = postedData["obs_qat_Id_delList"];

            if (questionHdr.obs_question_eff_end_dt < Convert.ToDateTime("01/01/2000")) { questionHdr.obs_question_eff_end_dt = Convert.ToDateTime("12/31/2060"); }
            string posted_ans_type_list = postedData["ans_type_list"];//represents newly added selectable ans types
            string posted_existing_ans_type_data = postedData["sel_ans_list"]; //represents existing assigned ans types that need to be modified
            //string ans_type_list = "6~true~yes~no~maybe,10~false~1~2~3~4~5";
            //ans_type_list format: at_id~default~sel_ans,
            //                          6~ true  ~always~sometimes~never,
            //obs_qat_id_delList = qat_id,qat_id,... list of answer types to be deleted
            //-------- Save the Question Information ----
            int defaultQATid = -1;
            try
            {
                defaultQATid = Convert.ToInt32(postedData["defaultQATid"]);
            }
            catch { }
            using (DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        
                        if(defaultQATid>0)
                        {
                            updateDefaultAnswerType(defaultQATid, "Y");
                        }
                        else if (questionHdr.obs_question_id > 0)
                        {             
                            List<OBS_QUEST_ANS_TYPES> temp_OBS_QUEST_ANS_TYPES = db.OBS_QUEST_ANS_TYPES.Where(x => x.obs_question_id == questionHdr.obs_question_id).ToList();
                            foreach(OBS_QUEST_ANS_TYPES x in temp_OBS_QUEST_ANS_TYPES )
                            {
                                x.obs_qat_default_ans_type_yn = "N";
                                db.SaveChanges();
                            }
                        }
                        ///////////////////////////////This section saves changes to existing answer types/////////////////////////////////
                        if (!string.IsNullOrEmpty(posted_existing_ans_type_data))
                        {
                            string[] split_ans_types_by = { "," };
                            string[] posted_single_ans_type_data = posted_existing_ans_type_data.Split(split_ans_types_by, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string str in posted_single_ans_type_data)
                            {
                                List<string> selAnsList_from_form = new List<string>();
                                string[] splitterm = { "~" };
                                string[] ans_type_elements = str.Split(splitterm, StringSplitOptions.RemoveEmptyEntries);
                                int intQATid = Convert.ToInt32(ans_type_elements[0]);
                                short obs_ans_type_id = db.OBS_QUEST_ANS_TYPES.Single(x => x.obs_qat_id == intQATid).obs_ans_type_id;
                                for (int i = 1; i < ans_type_elements.Length; i++)
                                {
                                    selAnsList_from_form.Add(ans_type_elements[i].Trim().ToUpper());
                                }
                                OBS_ANS_TYPE ans_type = db.OBS_ANS_TYPE.Single(item => item.obs_ans_type_id == obs_ans_type_id);
                                List<string> current_sel_ans_list = db.OBS_QUEST_SLCT_ANS.Where(item => item.obs_qat_id == intQATid && item.obs_qsa_eff_st_dt <= DateTime.Now && item.obs_qsa_eff_end_dt > DateTime.Now).Select(x => x.obs_qsa_text).ToList();                           
                                if (isEqualList(current_sel_ans_list, selAnsList_from_form, ans_type.obs_ans_type_category))
                                {//if we're here that means 2 lists are the same and we only need to change the order of selected answers list
                                        short order = 1;
                                        for (int i = 1; i < ans_type_elements.Length; i++)
                                        {
                                            OBS_QUEST_SLCT_ANS oBS_QUEST_SLCT_ANS = db.OBS_QUEST_SLCT_ANS.Single(item => item.obs_qat_id == intQATid && item.obs_qsa_text == ans_type_elements[i] && item.obs_qsa_eff_st_dt <= DateTime.Now && item.obs_qsa_eff_end_dt > DateTime.Now);
                                            oBS_QUEST_SLCT_ANS.obs_qsa_order = order;
                                            db.SaveChanges();
                                            order++;
                                        }
                                }
                                else//if we're here, that means user passed a different list of selected answers and we need to delete the current one and add new
                                {                                  
                                        //List<OBS_QUEST_SLCT_ANS> oBS_QUEST_SLCT_ANS = db.OBS_QUEST_SLCT_ANS.Where(item => item.obs_qat_id == intQATid).ToList();
                                        db.OBS_QUEST_SLCT_ANS.RemoveRange(db.OBS_QUEST_SLCT_ANS.Where(x => x.obs_qat_id == intQATid)); ;//update end effective date to todays date
                                        short order = 1;
                                        for (int i = 1; i < ans_type_elements.Length; i++)//now lets create a new record with updated selected answers
                                        {
                                            OBS_QUEST_SLCT_ANS UPDATED_oBS_QUEST_SLCT_ANS = new OBS_QUEST_SLCT_ANS();
                                            UPDATED_oBS_QUEST_SLCT_ANS.obs_qat_id = intQATid;
                                            UPDATED_oBS_QUEST_SLCT_ANS.obs_qsa_text = ans_type_elements[i].ToUpper();
                                            UPDATED_oBS_QUEST_SLCT_ANS.obs_qsa_order = order;
                                            UPDATED_oBS_QUEST_SLCT_ANS.obs_qsa_wt = order;
                                            UPDATED_oBS_QUEST_SLCT_ANS.obs_qsa_dflt_yn = "N";
                                            UPDATED_oBS_QUEST_SLCT_ANS.obs_qsa_eff_st_dt = DateTime.Now;
                                            UPDATED_oBS_QUEST_SLCT_ANS.obs_qsa_eff_end_dt = Convert.ToDateTime("12/31/2060");
                                            db.OBS_QUEST_SLCT_ANS.Add(UPDATED_oBS_QUEST_SLCT_ANS);
                                            order++;
                                        }// end foreach
                                       db.SaveChanges();                                                                      
                                }
                            }//foreach (string str in posted_single_ans_type_data)
                        }//end of  if (!string.IsNullOrEmpty(posted_existing_ans_type_data))
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        //this section saves new question information or new question answer type
                        if (questionHdr.obs_question_id > 0)
                        {
                            OBS_QUESTION editedQuestion = db.OBS_QUESTION.Single(x => x.obs_question_id == questionHdr.obs_question_id);

                            if (!editedQuestion.obs_question_full_text.Equals(questionHdr.obs_question_full_text))
                            {
                                editedQuestion.obs_question_ver++;
                                
                            }                            
                            editedQuestion.obs_question_full_text = questionHdr.obs_question_full_text;
                            editedQuestion.obs_question_desc = questionHdr.obs_question_desc;
                            editedQuestion.obs_question_eff_st_dt = questionHdr.obs_question_eff_st_dt;
                            editedQuestion.obs_question_eff_end_dt = questionHdr.obs_question_eff_end_dt;
                            editedQuestion.obs_question_upd_dtm = DateTime.Now;
                            editedQuestion.obs_question_upd_uid = User.Identity.Name;
                            db.SaveChanges();
                        }
                        else {

                            if (questionHdr.obs_question_eff_end_dt < Convert.ToDateTime("01/01/1900"))
                            {
                                questionHdr.obs_question_eff_end_dt = Convert.ToDateTime("12/31/2060");
                            }
                            if (questionHdr.obs_question_eff_st_dt < Convert.ToDateTime("01/01/1900"))
                            {
                                questionHdr.obs_question_eff_st_dt = DateTime.Now;
                            }
                            questionHdr.obs_question_ver = 1;
                            questionHdr.obs_question_added_uid = User.Identity.Name;
                            questionHdr.obs_question_added_dtm = DateTime.Now;
                            db.OBS_QUESTION.Add(questionHdr);
                            db.SaveChanges();
                        }

                        if (!String.IsNullOrEmpty(posted_ans_type_list))
                        {
                            string[] splitter = { "," };
                            string[] passed_sel_ans_info = posted_ans_type_list.Split(splitter, StringSplitOptions.RemoveEmptyEntries);//all passed selectable answers data
                            foreach (string s in passed_sel_ans_info)
                            {
                                string[] splitby = { "~" };
                                string[] single_sel_ans_info = s.Split(splitby, StringSplitOptions.RemoveEmptyEntries);//individual answer type data
                                OBS_QUEST_ANS_TYPES new_assigned_ans_type = new OBS_QUEST_ANS_TYPES();
                                new_assigned_ans_type.obs_ans_type_id = Convert.ToInt16(single_sel_ans_info[0]);
                                new_assigned_ans_type.obs_question_id = questionHdr.obs_question_id;
                                new_assigned_ans_type.obs_qat_default_ans_type_yn = single_sel_ans_info[1] == "true" ? "Y" : "N";
                                db.OBS_QUEST_ANS_TYPES.Add(new_assigned_ans_type);
                                db.SaveChanges();//at this point we've saved the OBS_QUEST_ANS_TYPES record.
                                OBS_QUEST_ANS_TYPES passed_quest_ans_record = db.OBS_QUEST_ANS_TYPES.Single(x => x.obs_qat_id == new_assigned_ans_type.obs_qat_id);
                                if (passed_quest_ans_record.obs_qat_default_ans_type_yn != new_assigned_ans_type.obs_qat_default_ans_type_yn && new_assigned_ans_type.obs_qat_default_ans_type_yn == "N")
                                { //if existing record is default and we need to set it to N we need to do it here and we won't have to 
                                  //change any other records
                                    passed_quest_ans_record.obs_qat_default_ans_type_yn = "N";
                                    db.SaveChanges();
                                }
                                else if (passed_quest_ans_record.obs_qat_default_ans_type_yn != new_assigned_ans_type.obs_qat_default_ans_type_yn && new_assigned_ans_type.obs_qat_default_ans_type_yn == "Y")
                                {//  if passed qat_id needs to be default one, we should first set existing default to N and then update passed  to Y   
                                    try
                                    {
                                        int current_default_quest_id = db.OBS_QUEST_ANS_TYPES.Single(x => x.obs_question_id == passed_quest_ans_record.obs_question_id && x.obs_qat_id != passed_quest_ans_record.obs_qat_id && x.obs_qat_default_ans_type_yn == "Y").obs_question_id;
                                        setExistingDefaultToN(current_default_quest_id);
                                    }
                                    catch { }
                                    passed_quest_ans_record.obs_qat_default_ans_type_yn = "Y";
                                    db.SaveChanges();
                                }
                                if (single_sel_ans_info.Count() > 2)//now we need to check if there's selectable answers for this question
                                {
                                    short order = 1;
                                    for (int i = 2; i < single_sel_ans_info.Count(); i++)
                                    {
                                        OBS_QUEST_SLCT_ANS new_sel_ans = new OBS_QUEST_SLCT_ANS();
                                        new_sel_ans.obs_qat_id = new_assigned_ans_type.obs_qat_id;
                                        new_sel_ans.obs_qsa_text = single_sel_ans_info[i];
                                        new_sel_ans.obs_qsa_order = order;
                                        new_sel_ans.obs_qsa_wt = order;
                                        new_sel_ans.obs_qsa_dflt_yn = "N";
                                        new_sel_ans.obs_qsa_eff_st_dt = DateTime.Now;
                                        new_sel_ans.obs_qsa_eff_end_dt = Convert.ToDateTime("12/31/2060");
                                        db.OBS_QUEST_SLCT_ANS.Add(new_sel_ans);
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                        ///////////////////////////////////WE DELETE SEL ANS HERE///////////////////////////////////////////
                        if (!string.IsNullOrEmpty(posted_deleted_ids))
                        {
                            string[] qats_to_delete = posted_deleted_ids.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string qat_to_delete in qats_to_delete)
                            {
                                deleteAssignedSelAns(Convert.ToInt32(qat_to_delete), db);
                            }
                        }
                        ///////////////////////////////////END OF DELETING SEL ANS///////////////////////////////////////////

                        // ------- Save the Question Metadata Changes ----
                        string MDlistBefore = postedData["origTags"];
                        string MDlistAfter = postedData["qAssignedMD"];
                        List<string> originalMDList = new List<string>();
                        List<string> newMDList = new List<string>();
                        if (!String.IsNullOrEmpty(MDlistBefore)) { originalMDList = MDlistBefore.Split(',').ToList(); }
                        if (!String.IsNullOrEmpty(MDlistAfter)) { newMDList = MDlistAfter.Split(',').ToList(); }
                        string[] mdIDsToDelete = originalMDList.Except(newMDList).ToArray();
                        string[] mdIDsToAdd = newMDList.Except(originalMDList).ToArray();

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
                            try
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
                            catch { }
                        }

                        //---- Save All Changes ------
                        db.SaveChanges();

                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        string notused = e.Message;
                        transaction.Rollback();
                    }
                }


            }


            //ViewBag.ConfMsg = "Data Saved Successfully.";

            return RedirectToAction("QuestionAddUpdate", new {id = questionHdr.obs_question_id});

            //return RedirectToAction("Index", "Question");
            //return View("Edit");
        }



        //---------------------------------------- Render Templates Partial View -----------------------------------------------------------------
        [HttpGet]
        //[ChildActionOnly]
        public PartialViewResult selAnsTemplates(string templateCathegory)
        {
            List<string> templateList = new List<string>();
            switch (templateCathegory)
            {
                case "3 Val Range":
                    templateList.Add("1,2,3");
                    templateList.Add("YES,NO,MAYBE");
                    templateList.Add("LOW,MEDIUM,HIGH");
                    templateList.Add("NEVER,SOMETIMES,ALWAYS");
                    templateList.Add("ALWAYS,SOMETIMES,NEVER");
                    break;
                case "5 Val Range":
                    templateList.Add("1,2,3,4,5");
                    templateList.Add("NEVER,RARELY,SOMETIMES,OFTEN,ALWAYS");
                    templateList.Add("STRONGLY DISAGREE,DISAGREE,N/A,AGREE,STRONGLY AGREE");
                    break;
                case "MS List":
                    templateList.Add("NO TEMPLATES AVAILABLE");
                    break;
                case "SS List":
                    templateList.Add("NO TEMPLATES AVAILABLE");
                    break;
                default:
                    templateList.Add("NO TEMPLATES NEEDED");
                    break;
            }//end of switch    

            return PartialView("_selAnsTemplates", templateList);
        }


        //----------------------------------------------------------------

        [HttpGet]
        public PartialViewResult getQuestionAnswerInfo(qatTags qatInfo)
        {
            return PartialView("_getQuestionAnswerInfo", qatInfo);
        }

        [HttpGet]
        public PartialViewResult addQuestionAnswerInfo()
        {
            var selAnsList = db.OBS_ANS_TYPE.Select(x => new SelectListItem
            {
                Value = x.obs_ans_type_id.ToString(),
                Text = x.obs_ans_type_name
            }).OrderBy(y => y.Text);
            ViewBag.fullSelATlist = selAnsList;
            return PartialView("_addQuestionAnswerInfo");
        }
        [HttpGet]
        public PartialViewResult saveNewSelectableAnswer(string input_data)
        {
            AddedSelAnswer added_answer_type = new AddedSelAnswer();
            string[] splitter = { "," };
            string[] data_from_gui = input_data.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
            added_answer_type.ans_type_id = Convert.ToInt16(data_from_gui[0]);
            added_answer_type.isDefault = data_from_gui[1] == "true" ? true : false;
            added_answer_type.ans_type_name = db.OBS_ANS_TYPE.Single(x => x.obs_ans_type_id == added_answer_type.ans_type_id).obs_ans_type_name;
            if (data_from_gui.Count() > 2)//now we need to check if there's selectable answers for this answer type
            {                
                for (int i = 2; i < data_from_gui.Count(); i++)
                {
                    added_answer_type.selAnsList.Add(data_from_gui[i]);
                }
            }
            return PartialView("_saveNewSelectableAnswer", added_answer_type);
        }

        [HttpPost]
        public string saveSelAns(int qat_id, string sel_ans_list)
        {
                           
           return updateSel_Ans_Types(qat_id, sel_ans_list);                    
                          
        }

        [HttpPost]
        public string setDefaultQAT(int qat_id,bool isDefault)
        {
            try
            {
                updateDefaultAnswerType(qat_id, isDefault ? "Y" : "N");
                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }



        //-----------------------------------------------------------------------------------------------------------------
        // GET: QuestionMetadata
        [ChildActionOnly]     [OutputCache(Duration =2000)]
        public ActionResult qMetaDataList()
        {
            return View(db.OBS_QUESTION_METADATA.ToList());
        }
        //------------------------------------------------- [ DELETE  Action: GET]-----------------------------------------------
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
        //--------------------------------------------------[ DELETE  Action: POST ] --------------------------------------------
        // POST: Question/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //---------------------------------------------[ DELETECONFIRMED  Action: GET] -----------------------------------------------
        public ActionResult DeleteConfirmed(int id)
        {
            OBS_QUESTION oBS_QUESTION = db.OBS_QUESTION.Find(id);
            db.OBS_QUESTION.Remove(oBS_QUESTION);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //---------------------------------------- [ displayAnswerSection  Actio: GET ]------------------------------------------
        [HttpGet]
        public ActionResult displayAnswerSection(int id)
        {
            // This is a test method that accepts a question Id and the selected Index of the dropdown list 
            // and returns the section for Answers

            OBSQuestion obsQuestion = new OBSQuestion(id);
            //ViewBag.list_of_answers = obsQuestion.fullAnswerTypeDDL;
            //ViewBag.Message = "This is the [GET] Method";
            return View(obsQuestion);
        }
        //---------------------------------------- [ displayAnswerSection  Actio: POST ]-----------------------------------------
        [HttpPost]
        public ActionResult displayAnswerSection(FormCollection postedData)
        {
            //ViewBag.Message = "This is the [POST] Method";

            // Rebuild the Question Object for reuse
            int id = Convert.ToInt32(postedData["question_id"]);
            
            int newSelIndex = -1;
            try { newSelIndex = Convert.ToInt16(postedData["AnswerTypesDDL"]); }
            catch { }

            OBSQuestion obsQuestion = new OBSQuestion(id);

            //set the ddl to the new index value based on the posted form
            obsQuestion.fullAnswerTypeDDL.Clear();
            obsQuestion.setAnswerTypeDDL((short)newSelIndex);            

            if (postedData["save"].Equals("true"))
            {          
                try
                {
                    obsQuestion.selectedAT.selAnsList = assign_new_selAnsList_to_OBSQuestion(postedData["userSelAnsList"]);
                }
                catch { }
                SaveDefaultAnswerType(obsQuestion);
                obsQuestion = new OBSQuestion(id);
            }
            obsQuestion.templates = obsQuestion.getTemplates(obsQuestion.selectedAT.ATcathegory);
            return View(obsQuestion);
        }
        //---------------------------------------- [ Manage   <NOT USED> ]------------------------------------------------------
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
            }
        }

        //---------------------------------------- [ DISPOSE  Used as Garbage Collector on Delete Action]-----------------------
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
        //******** CLASES *******************************
       
        public class OBSQA {
            // Constructor
            public OBSQA() { }
            // --- Properties ----
            public bool isDefaultQA { get; set; } 
            public int answerTypeId { get; set; }
            //public int o
            //public List<OBS_QUEST_SLCT_ANS> selectableAnsList = new List<OBS_QUEST_SLCT_ANS>();
            // --- Methods -------        
        }
        public class SelAnswerType {
            public SelAnswerType()  { }
            public int indexinDDL =-1;
            public int ATid = 0;
            public string ATvalue = String.Empty;
            public string ATcathegory = String.Empty;
            public bool hasSelectableAnswers = false;
            public bool requiresSelectableAnswers = false;
            public List<string> selAnsList = new List<string>();
        }

        public class OBSQuestion
        {       
            private DSC_OBS_DB_ENTITY OBSdb = new DSC_OBS_DB_ENTITY();

            // Constructor
            public OBSQuestion(int Id ) {
                questionId = Id;
                //fullAnswerTypeList = OBSdb.OBS_ANS_TYPE.ToList();
                indexOfDefaultQA = -1;     //Set Initial Default to "No Default Found or -1"
                OBSQA_List = retrieveQAInstances(); // This method also sets the correct 'indexOfDefaultQA' and the 'hasInstances' properties.
                templates = getTemplates(selectedAT.ATcathegory);
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
            public List<string> templates = new List<string>();

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -\\
            //- - - - - - - - - - - - CLASS METHODS - - - - - - - - - - - - - - - - |
            public void setAnswerTypeDDL(short obs_AT_id = 0)
            { // If the "selected" ATId is not Passed (zero value) then the Dropdown list has no selected Item
                int defaultIndex = 0;
                //Loop through all the Answer Types in the Database Table

                if (obs_AT_id < 1)
                {//Clear all the AT data
                    selectedAT.ATid = 0;
                    selectedAT.indexinDDL = 0;                    
                    selectedAT.ATvalue = String.Empty;  //string
                    selectedAT.ATcathegory = String.Empty; //string
                    selectedAT.hasSelectableAnswers = false;
                    selectedAT.selAnsList.Clear();
                }


                foreach (OBS_ANS_TYPE ansTypeEntry in OBSdb.OBS_ANS_TYPE.ToList())
                {                    
                    SelectListItem ddlListItem = new SelectListItem();
                    ddlListItem.Value = ansTypeEntry.obs_ans_type_id.ToString();
                    ddlListItem.Text = ansTypeEntry.obs_ans_type_name;
                    
                    // Check if this Ans Type entry should be displayed as selected
                    // Based on the parameter received
                    if (((obs_AT_id > 0)) && (ansTypeEntry.obs_ans_type_id == obs_AT_id))
                    {  //Set the current List Item as Selected
                        ddlListItem.Selected = true;
                        //Set all the "selectedAT" class values
                        selectedAT.indexinDDL = defaultIndex; //int
                        selectedAT.ATid = ansTypeEntry.obs_ans_type_id;               //int
                        selectedAT.ATvalue = ansTypeEntry.obs_ans_type_name;  //string
                        selectedAT.ATcathegory = ansTypeEntry.obs_ans_type_category; //string
                        selectedAT.requiresSelectableAnswers = ansTypeEntry.obs_ans_type_has_fxd_ans_yn.Equals("Y") ? true : false ; 
                        //Check if the QuestionId/AnsTypeid combination exist in the "OBS_QUEST_ANS_TYPE" Table
                        // Get the Id from the "OBS_QUEST_ANS_TYPE" table
                        int QATinstanceId = 0;
                        try { QATinstanceId = (OBSdb.OBS_QUEST_ANS_TYPES.FirstOrDefault(x => x.obs_question_id == questionId && x.obs_ans_type_id == obs_AT_id)).obs_qat_id; }
                        catch { }

                        if (QATinstanceId > 0)
                        {
                            // If the Id exist, then the Dropdown Selection has selectable answers
                            selectedAT.hasSelectableAnswers = true;
                            // Grab selectable Answer values from OBS_QUEST_SLCT_ANS table
                            selectedAT.selAnsList = OBSdb.OBS_QUEST_SLCT_ANS.Where(X => X.obs_qat_id == QATinstanceId && X.obs_qsa_eff_st_dt<=DateTime.Now && X.obs_qsa_eff_end_dt>DateTime.Now).OrderBy(y => y.obs_qsa_order).Select(z => z.obs_qsa_text).ToList();
                        }
                        else
                        {
                            // The curent Selection does not have selectable answers. 
                            selectedAT.hasSelectableAnswers = false;  //bool
                            // Grab selectable Answer values from harcoded list
                            selectedAT.selAnsList = getDefaultSLCT(ansTypeEntry.obs_ans_type_category);
                        }
                    }
                    
                    defaultIndex++;
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
                    indexOfDefaultQA = -1;          //There is no default instance (Nothing Found)
                    setAnswerTypeDDL(0);             // Invoke the routine to create the dropdown with no selected value   
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
                    {   // Records were found in the 'OBS_QUEST_ANS_TYPES' Table but none of them was a default entry
                        // Create dropdown list with no seletced Value
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
                        q_selected_ans_type.Add("Enter a Value");
                        q_selected_ans_type.Add("Enter a Value");
                        q_selected_ans_type.Add("Enter a Value");
                        break;
                    case "5 Val Range":
                        q_selected_ans_type.Add("Enter a Value");
                        q_selected_ans_type.Add("Enter a Value");
                        q_selected_ans_type.Add("Enter a Value");
                        q_selected_ans_type.Add("Enter a Value");
                        q_selected_ans_type.Add("Enter a Value");
                        break;
                    case "MS List":
                        q_selected_ans_type.Add("Enter a Value");
                        q_selected_ans_type.Add("Enter a Value");
                        q_selected_ans_type.Add("Enter a Value");
                        q_selected_ans_type.Add("Enter a Value");
                        q_selected_ans_type.Add("Enter a Value");
                        q_selected_ans_type.Add("Enter a Value");
                        q_selected_ans_type.Add("Enter a Value");
                        q_selected_ans_type.Add("Enter a Value");
                        break;
                    case "SS List":
                        q_selected_ans_type.Add("Enter a Value");
                        q_selected_ans_type.Add("Enter a Value");
                        q_selected_ans_type.Add("Enter a Value");
                        q_selected_ans_type.Add("Enter a Value");
                        q_selected_ans_type.Add("Enter a Value");
                        q_selected_ans_type.Add("Enter a Value");
                        q_selected_ans_type.Add("Enter a Value");
                        q_selected_ans_type.Add("Enter a Value");
                        break;

                }//end of switch    
                return q_selected_ans_type;        
            }

            public List<string> getTemplates(string category)
            {
                List<string> templateList = new List<string>();
                switch (category)
                {
                    case "3 Val Range":
                        templateList.Add("1,2,3");
                        templateList.Add("YES,NO,MAYBE");
                        templateList.Add("LOW,MEDIUM,HIGH");
                        templateList.Add("NEVER,SOMETIMES,ALWAYS");
                        templateList.Add("ALWAYS,SOMETIMES,NEVER");
                        break;
                    case "5 Val Range":
                        templateList.Add("1,2,3,4,5");
                        templateList.Add("NEVER,RARELY,SOMETIMES,OFTEN,ALWAYS");
                        templateList.Add("STRONGLY DISAGREE,DISAGREE,N/A,AGREE,STRONGLY AGREE");
                        break;
                    case "MS List":
                        templateList.Add("Forklift,Hat,Steel toe shoes,Gloves");
                        break;
                    case "SS List":
                        templateList.Add("FIRST SHIFT,SECOND SHIFT,THIRD SHIFT");
                        break;
                    default:
                        templateList.Add("NO TEMPLATES NEEDED");
                        break;
                }//end of switch    
                return templateList;
            }
            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        }
        // ******* Helper Methods ***********************
        public void updateDefaultAnswerType(int qat_id, string isDefault)
        {
            OBS_QUEST_ANS_TYPES passed_quest_ans_record = db.OBS_QUEST_ANS_TYPES.Single(x => x.obs_qat_id == qat_id);
            if(passed_quest_ans_record.obs_qat_default_ans_type_yn!=isDefault && isDefault == "N")
            { //if existing record is default and we need to set it to N we need to do it here and we won't have to 
              //change any other records
                passed_quest_ans_record.obs_qat_default_ans_type_yn = "N";
                db.SaveChanges();
            }
            else if(passed_quest_ans_record.obs_qat_default_ans_type_yn != isDefault && isDefault == "Y")
            {//  if passed qat_id needs to be default one, we should first set existing default to N and then update passed  to Y   
                try
                {
                    int current_default_quest_id = db.OBS_QUEST_ANS_TYPES.Single(x => x.obs_question_id == passed_quest_ans_record.obs_question_id && x.obs_qat_id != passed_quest_ans_record.obs_qat_id && x.obs_qat_default_ans_type_yn == "Y").obs_question_id;
                    setExistingDefaultToN(current_default_quest_id);
                }
                catch { }
                passed_quest_ans_record.obs_qat_default_ans_type_yn = "Y";
                db.SaveChanges();
            }
        }
        public string updateSel_Ans_Types(int qat_id, string sel_ans_list)
        {
            List<string> selAnsList_from_form = new List<string>();
            short obs_ans_type_id = db.OBS_QUEST_ANS_TYPES.Single(x => x.obs_qat_id == qat_id).obs_ans_type_id;
            string[] splitterm = { "~" };
            string[] selected_new_sel_ans_types = sel_ans_list.Split(splitterm, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in selected_new_sel_ans_types)
            {
                { selAnsList_from_form.Add(s.Trim().ToUpper()); }

            }
            
            OBS_ANS_TYPE ans_type = db.OBS_ANS_TYPE.Single(item => item.obs_ans_type_id == obs_ans_type_id);

            List<string> current_sel_ans_list = db.OBS_QUEST_SLCT_ANS.Where(item => item.obs_qat_id == qat_id && item.obs_qsa_eff_st_dt <= DateTime.Now && item.obs_qsa_eff_end_dt > DateTime.Now).Select(x => x.obs_qsa_text).ToList();
            if(current_sel_ans_list.Count()!= selAnsList_from_form.Count() && (ans_type.obs_ans_type_category =="3 Val Range"|| ans_type.obs_ans_type_category=="5 Val Range"))
            {
                return "ERROR: Not Enough Selectable Answers for this category!!!!";
            }
            if (isEqualList(current_sel_ans_list, selAnsList_from_form, ans_type.obs_ans_type_category))
            {//if we're here that means 2 lists are the same and we only need to change the order of selected answers list

                try {
                    short order = 1;
                    foreach (string str in selAnsList_from_form)
                    {
                        OBS_QUEST_SLCT_ANS oBS_QUEST_SLCT_ANS = db.OBS_QUEST_SLCT_ANS.Single(item => item.obs_qat_id == qat_id && item.obs_qsa_text == str && item.obs_qsa_eff_st_dt <= DateTime.Now && item.obs_qsa_eff_end_dt > DateTime.Now);
                        oBS_QUEST_SLCT_ANS.obs_qsa_order = order;
                        db.SaveChanges();
                        order++;
                    }
                    return "OK";
                }
                catch(Exception e)
                {
                    return e.Message;
                }

            }
            else//if we're here, that means user passed a different list of selected answers and we need to disable the current one and add new
            {
                try
                {
                    List<OBS_QUEST_SLCT_ANS> oBS_QUEST_SLCT_ANS = db.OBS_QUEST_SLCT_ANS.Where(item => item.obs_qat_id == qat_id).ToList();
                    oBS_QUEST_SLCT_ANS.ForEach(x => x.obs_qsa_eff_end_dt = DateTime.Now);//update end effective date to todays date
                    short order = 1;
                    foreach (string str in selAnsList_from_form)//now lets create a new record with updated selected answers
                    {
                        OBS_QUEST_SLCT_ANS UPDATED_oBS_QUEST_SLCT_ANS = new OBS_QUEST_SLCT_ANS();
                        UPDATED_oBS_QUEST_SLCT_ANS.obs_qat_id = qat_id;
                        UPDATED_oBS_QUEST_SLCT_ANS.obs_qsa_text = str;
                        UPDATED_oBS_QUEST_SLCT_ANS.obs_qsa_order = order;
                        UPDATED_oBS_QUEST_SLCT_ANS.obs_qsa_wt = order;
                        UPDATED_oBS_QUEST_SLCT_ANS.obs_qsa_dflt_yn = "N";
                        UPDATED_oBS_QUEST_SLCT_ANS.obs_qsa_eff_st_dt = DateTime.Now;
                        UPDATED_oBS_QUEST_SLCT_ANS.obs_qsa_eff_end_dt = Convert.ToDateTime("12/31/2060");
                        db.OBS_QUEST_SLCT_ANS.Add(UPDATED_oBS_QUEST_SLCT_ANS);
                        order++;
                    }// end foreach
                    db.SaveChanges();
                    return "OK";
                }
                catch (Exception e)
                {
                    return e.Message;
                }                
            }
        }

        public void SaveDefaultAnswerType(OBSQuestion obsQuestion)
        {
            int selected_ans_type_id = obsQuestion.selectedAT.ATid;
            int question_id = obsQuestion.questionId;
            //string default_sel_ans_types = formData["default_selected_ans_types"];
            //first lets check if user submitted None as a default answer type 
            if (selected_ans_type_id < 1)
            {
                OBS_QUEST_ANS_TYPES oBS_QUEST_ANS_TYPES = new OBS_QUEST_ANS_TYPES();
                try
                {
                    oBS_QUEST_ANS_TYPES = db.OBS_QUEST_ANS_TYPES.Single(item => item.obs_question_id == question_id && item.obs_qat_default_ans_type_yn == "Y");
                    oBS_QUEST_ANS_TYPES.obs_qat_default_ans_type_yn = "N";
                    db.SaveChanges();
                }
                catch { }
            }//end  if (String.IsNullOrEmpty(answer_type_id))
            else //if we're here, that means user submtted  answer type that <>None
            {
                //now we need to check if this question/answer type combination already exist in obs_quest_ans_type table
                if (!obsQuestion.hasInstances) // oBS_QUEST_ANS_TYPES Table does not have Any records
                {
                    //if we're here, that means we need to insert a new record in OBS_QUEST_ANS_TYPE table
                    //first we need to check if this selected answer type requires a record in OBS_QUEST_SLCT_ANS
                    if (isQuest_Slct_Ans_Required(selected_ans_type_id))
                    {   // oBS_QUEST_ANS_TYPES Table does not have Any records and The Seacted Ans Type requires a record in OBS_QUEST_SLCT_ANS
                        //so we are here, that means we need to save both OBS_QUEST_ANS_TYPE and OBS_QUEST_SLCT_ANS

                        //-- First Insert record into 'OBS_QUEST_ANS_TYPES' Table
                        OBS_QUEST_ANS_TYPES oBS_QUEST_ANS_TYPES = new OBS_QUEST_ANS_TYPES();
                        oBS_QUEST_ANS_TYPES.obs_question_id = question_id;
                        oBS_QUEST_ANS_TYPES.obs_ans_type_id = (short)selected_ans_type_id;
                        oBS_QUEST_ANS_TYPES.obs_qat_default_ans_type_yn = "Y";
                        //oBS_QUEST_ANS_TYPES.obs_qat_end_eff_dt = Convert.ToDateTime("12/31/2060");
                        db.OBS_QUEST_ANS_TYPES.Add(oBS_QUEST_ANS_TYPES);
                        db.SaveChanges();

                        //-- Second Insert record into 'OBS_QUEST_SLCT_ANS' Table
                        //short temp_selected_ans_type_id = (short)selected_ans_type_id;
                        int createdQAT_id = db.OBS_QUEST_ANS_TYPES.SingleOrDefault(item => item.obs_ans_type_id == obsQuestion.selectedAT.ATid && item.obs_question_id == question_id).obs_qat_id;

                        short order = 1;
                        foreach (string str in obsQuestion.selectedAT.selAnsList)
                        {
                            OBS_QUEST_SLCT_ANS oBS_QUEST_SLCT_ANS = new OBS_QUEST_SLCT_ANS();
                            oBS_QUEST_SLCT_ANS.obs_qat_id = createdQAT_id;
                            oBS_QUEST_SLCT_ANS.obs_qsa_text = str;
                            oBS_QUEST_SLCT_ANS.obs_qsa_order = order;
                            oBS_QUEST_SLCT_ANS.obs_qsa_wt = order;
                            oBS_QUEST_SLCT_ANS.obs_qsa_dflt_yn = "N";
                            oBS_QUEST_SLCT_ANS.obs_qsa_eff_st_dt = DateTime.Now;
                            oBS_QUEST_SLCT_ANS.obs_qsa_eff_end_dt = Convert.ToDateTime("12/31/2060");
                            db.OBS_QUEST_SLCT_ANS.Add(oBS_QUEST_SLCT_ANS);
                            order++;
                        }
                        db.SaveChanges();

                    }//end of if (isQuest_Slct_Ans_Required(selected_ans_type_id))
                    else
                    {
                        //-- Insert record into 'OBS_QUEST_ANS_TYPES' Table
                        OBS_QUEST_ANS_TYPES oBS_QUEST_ANS_TYPES = new OBS_QUEST_ANS_TYPES();
                        oBS_QUEST_ANS_TYPES.obs_question_id = question_id;
                        oBS_QUEST_ANS_TYPES.obs_ans_type_id = (short)selected_ans_type_id;
                        oBS_QUEST_ANS_TYPES.obs_qat_default_ans_type_yn = "Y";
                        // oBS_QUEST_ANS_TYPES.obs_qat_end_eff_dt = Convert.ToDateTime("12/31/2060");
                        db.OBS_QUEST_ANS_TYPES.Add(oBS_QUEST_ANS_TYPES);
                        db.SaveChanges();
                    }

                }// End of if (!obsQuestion.hasInstances)
                 //if (isNew_Quest_Ans_Type(selected_ans_type_id, question_id))
                else if (obsQuestion.indexOfDefaultQA >= 0 &&(obsQuestion.selectedAT.ATid ==obsQuestion.OBSQA_List[obsQuestion.indexOfDefaultQA].answerTypeId)&& obsQuestion.selectedAT.requiresSelectableAnswers) 
                { //logic to update existing "OBS_QUEST_SLCT_ANS" when submitted answer type is default goes here

                    //first lets find the obs_qat_id from  OBS_QUEST_ANS_TYPES table for this question id and selected answer type id
                    int default_qat_id = db.OBS_QUEST_ANS_TYPES.SingleOrDefault(item => item.obs_ans_type_id == obsQuestion.selectedAT.ATid && item.obs_question_id == question_id).obs_qat_id;
                    List<string> current_default_sel_ans_list = db.OBS_QUEST_SLCT_ANS.Where(item => item.obs_qat_id == default_qat_id && item.obs_qsa_eff_st_dt <= DateTime.Today && item.obs_qsa_eff_end_dt > DateTime.Today).Select(x =>x.obs_qsa_text).ToList();
                    //at this point we have 2 lists of strings(current default selected answers and ones user passed from the form) and we need to compare them
                    if(isEqualList(current_default_sel_ans_list, obsQuestion.selectedAT.selAnsList, obsQuestion.selectedAT.ATcathegory))
                    {//if we're here that means 2 lists are the same and we only need to change the order of selected answers list
                        short order = 1;
                        foreach (string str in obsQuestion.selectedAT.selAnsList)
                        {
                            OBS_QUEST_SLCT_ANS oBS_QUEST_SLCT_ANS = db.OBS_QUEST_SLCT_ANS.Single(item => item.obs_qat_id == default_qat_id && item.obs_qsa_text == str && item.obs_qsa_eff_st_dt <= DateTime.Today && item.obs_qsa_eff_end_dt > DateTime.Today);                           
                            oBS_QUEST_SLCT_ANS.obs_qsa_order = order;
                            oBS_QUEST_SLCT_ANS.obs_qsa_wt = order;
                            db.SaveChanges();                     
                            order++;
                        }

                    }
                    else//if we're here, that means user passed a different list of selected answers and we need to disable the current one and add new
                    {
                        List<OBS_QUEST_SLCT_ANS> oBS_QUEST_SLCT_ANS = db.OBS_QUEST_SLCT_ANS.Where(item => item.obs_qat_id == default_qat_id).ToList();
                        oBS_QUEST_SLCT_ANS.ForEach(x => x.obs_qsa_eff_end_dt = DateTime.Today);//update end effective date to todays date
                        short order = 1;
                        foreach (string str in obsQuestion.selectedAT.selAnsList)//now lets create a new record with updated selected answers
                        {
                            OBS_QUEST_SLCT_ANS UPDATED_oBS_QUEST_SLCT_ANS = new OBS_QUEST_SLCT_ANS();
                            UPDATED_oBS_QUEST_SLCT_ANS.obs_qat_id = default_qat_id;
                            UPDATED_oBS_QUEST_SLCT_ANS.obs_qsa_text = str;
                            UPDATED_oBS_QUEST_SLCT_ANS.obs_qsa_order = order;
                            UPDATED_oBS_QUEST_SLCT_ANS.obs_qsa_wt = order;
                            UPDATED_oBS_QUEST_SLCT_ANS.obs_qsa_dflt_yn = "N";
                            UPDATED_oBS_QUEST_SLCT_ANS.obs_qsa_eff_st_dt = DateTime.Now;
                            UPDATED_oBS_QUEST_SLCT_ANS.obs_qsa_eff_end_dt = Convert.ToDateTime("12/31/2060");
                            db.OBS_QUEST_SLCT_ANS.Add(UPDATED_oBS_QUEST_SLCT_ANS);
                            order++;
                        }// end foreach
                        db.SaveChanges();

                    }


                }// end of  "else if (obsQuestion.indexOfDefaultQA >= 0 &&(obsQuestion.selectedAT.ATid ==obsQuestion.OBSQA_List[obsQuestion.indexOfDefaultQA].answerTypeId)&& obsQuestion.selectedAT.requiresSelectableAnswers) "
                else// this branch should take care of the scenario where this question/answer type exists in the obs_quest_ans_type table
                {
                    //Check if the id a default "Y" record. If so, set it to "N"
                    if (obsQuestion.indexOfDefaultQA >= 0)
                    {
                        setExistingDefaultToN(obsQuestion.questionId); 
                    }

                    //Check if the selected "AT id" and "Question Id" combination exist in the " OBS_QUEST_ANS_TYPES" table
                    if (obsQuestion.OBSQA_List.Where(x => x.answerTypeId == selected_ans_type_id).Count() > 0)
                    {
                        OBS_QUEST_ANS_TYPES oBS_QUEST_ANS_TYPES = new OBS_QUEST_ANS_TYPES();
                        try
                        {
                            oBS_QUEST_ANS_TYPES = db.OBS_QUEST_ANS_TYPES.Single(item => item.obs_question_id == question_id && item.obs_ans_type_id == selected_ans_type_id);
                            oBS_QUEST_ANS_TYPES.obs_qat_default_ans_type_yn = "Y";
                            db.SaveChanges();
                            //after we set new answer type to be default, we need to check if selected answer types require updates as well
                            if (obsQuestion.selectedAT.requiresSelectableAnswers)
                            {
                                int default_qat_id = oBS_QUEST_ANS_TYPES.obs_qat_id;
                                List<string> current_default_sel_ans_list = db.OBS_QUEST_SLCT_ANS.Where(item => item.obs_qat_id == default_qat_id && item.obs_qsa_eff_st_dt <= DateTime.Today && item.obs_qsa_eff_end_dt > DateTime.Today).Select(x => x.obs_qsa_text).ToList();
                                if (isEqualList(current_default_sel_ans_list, obsQuestion.selectedAT.selAnsList, obsQuestion.selectedAT.ATcathegory))
                                {//if we're here that means 2 lists are the same and we only need to change the order of selected answers list
                                    short order = 1;
                                    foreach (string str in obsQuestion.selectedAT.selAnsList)
                                    {
                                        OBS_QUEST_SLCT_ANS oBS_QUEST_SLCT_ANS = db.OBS_QUEST_SLCT_ANS.Single(item => item.obs_qat_id == default_qat_id && item.obs_qsa_text == str && item.obs_qsa_eff_st_dt <= DateTime.Today && item.obs_qsa_eff_end_dt > DateTime.Today);
                                        oBS_QUEST_SLCT_ANS.obs_qsa_order = order;
                                        db.SaveChanges();
                                        order++;
                                    }

                                }
                                else//if we're here, that means user passed a different list of selected answers and we need to disable the current one and add new
                                {
                                    List<OBS_QUEST_SLCT_ANS> oBS_QUEST_SLCT_ANS = db.OBS_QUEST_SLCT_ANS.Where(item => item.obs_qat_id == default_qat_id).ToList();
                                    oBS_QUEST_SLCT_ANS.ForEach(x => x.obs_qsa_eff_end_dt = DateTime.Now);//update end effective date to todays date
                                    short order = 1;
                                    foreach (string str in obsQuestion.selectedAT.selAnsList)//now lets create a new record with updated selected answers
                                    {
                                        OBS_QUEST_SLCT_ANS UPDATED_oBS_QUEST_SLCT_ANS = new OBS_QUEST_SLCT_ANS();
                                        UPDATED_oBS_QUEST_SLCT_ANS.obs_qat_id = default_qat_id;
                                        UPDATED_oBS_QUEST_SLCT_ANS.obs_qsa_text = str;
                                        UPDATED_oBS_QUEST_SLCT_ANS.obs_qsa_order = order;
                                        UPDATED_oBS_QUEST_SLCT_ANS.obs_qsa_wt = order;
                                        UPDATED_oBS_QUEST_SLCT_ANS.obs_qsa_dflt_yn = "N";
                                        UPDATED_oBS_QUEST_SLCT_ANS.obs_qsa_eff_st_dt = DateTime.Now;
                                        UPDATED_oBS_QUEST_SLCT_ANS.obs_qsa_eff_end_dt = Convert.ToDateTime("12/31/2060");
                                        db.OBS_QUEST_SLCT_ANS.Add(UPDATED_oBS_QUEST_SLCT_ANS);
                                        order++;
                                    }// end foreach
                                    db.SaveChanges();
                                }//end of else
                            }//if (obsQuestion.selectedAT.requiresSelectableAnswers)
                        }
                        catch { }
                    }
                    else { 
                    //It doesn't exist. We need to create a new record with "Y" default indicator
                        //First check if the new AT type selection requires selectable answers

                        //-- First Insert record into 'OBS_QUEST_ANS_TYPES' Table
                        OBS_QUEST_ANS_TYPES oBS_QUEST_ANS_TYPES = new OBS_QUEST_ANS_TYPES();
                        oBS_QUEST_ANS_TYPES.obs_question_id = question_id;
                        oBS_QUEST_ANS_TYPES.obs_ans_type_id = (short)selected_ans_type_id;
                        oBS_QUEST_ANS_TYPES.obs_qat_default_ans_type_yn = "Y";
                        //oBS_QUEST_ANS_TYPES.obs_qat_end_eff_dt = Convert.ToDateTime("12/31/2060");
                        db.OBS_QUEST_ANS_TYPES.Add(oBS_QUEST_ANS_TYPES);
                        db.SaveChanges();

                        if (isQuest_Slct_Ans_Required(selected_ans_type_id))
                        {   //If selectable answers are required:

                            //-- Second Insert record into 'OBS_QUEST_SLCT_ANS' Table
                            //short temp_selected_ans_type_id = (short)selected_ans_type_id;
                            int createdQAT_id = db.OBS_QUEST_ANS_TYPES.SingleOrDefault(item => item.obs_ans_type_id == obsQuestion.selectedAT.ATid && item.obs_question_id == question_id).obs_qat_id;

                            short order = 1;
                            foreach (string str in obsQuestion.selectedAT.selAnsList)
                            {
                                OBS_QUEST_SLCT_ANS oBS_QUEST_SLCT_ANS = new OBS_QUEST_SLCT_ANS();
                                oBS_QUEST_SLCT_ANS.obs_qat_id = createdQAT_id;
                                oBS_QUEST_SLCT_ANS.obs_qsa_text = str;
                                oBS_QUEST_SLCT_ANS.obs_qsa_order = order;
                                oBS_QUEST_SLCT_ANS.obs_qsa_wt = order;
                                oBS_QUEST_SLCT_ANS.obs_qsa_dflt_yn = "N";
                                oBS_QUEST_SLCT_ANS.obs_qsa_eff_st_dt = DateTime.Now;
                                oBS_QUEST_SLCT_ANS.obs_qsa_eff_end_dt = Convert.ToDateTime("12/31/2060");
                                db.OBS_QUEST_SLCT_ANS.Add(oBS_QUEST_SLCT_ANS);
                                order++;
                            }
                            db.SaveChanges();

                        }//end of if (isQuest_Slct_Ans_Required(selected_ans_type_id))
                    }                   
                } 

            }

        }

        public bool isQuest_Slct_Ans_Required(int ans_type_id)
        {
            short temp = (short)ans_type_id;
            bool isRequired = (db.OBS_ANS_TYPE.Single(item => item.obs_ans_type_id == temp).obs_ans_type_has_fxd_ans_yn) == "Y" ? true : false;
            return isRequired;
        }
        public void setExistingDefaultToN(int question_id)
        {
            OBS_QUEST_ANS_TYPES oBS_QUEST_ANS_TYPES = new OBS_QUEST_ANS_TYPES();
            try
            {
                oBS_QUEST_ANS_TYPES = db.OBS_QUEST_ANS_TYPES.Single(item => item.obs_question_id == question_id && item.obs_qat_default_ans_type_yn == "Y");
                oBS_QUEST_ANS_TYPES.obs_qat_default_ans_type_yn = "N";
                db.SaveChanges();
            }
            catch { }
        }
        public List<string> assign_new_selAnsList_to_OBSQuestion(string user_input_from_form)
        {
            List<string> selAnsList_from_form = new List<string>();
            string[] splitterm = { "," };
            string[] selected_new_sel_ans_types = user_input_from_form.Split(splitterm, StringSplitOptions.RemoveEmptyEntries);
            foreach(string s in selected_new_sel_ans_types)
            {
                if (s != "Enter a Value") { selAnsList_from_form.Add(s); }

            }
            return selAnsList_from_form;
        }
        /*
        *This isEqualList method compares 2 lists of selected answer types: current default and the one passed from the form
        *If 2 lists are the same, method returns true. if they're different it returns false
        */
        public bool isEqualList(List<string> currentDefault, List<string> passedFromForm, string ans_category)
        {
            
            //this if checks if there are elements that are in the first list but not in the second. if "Except" method returns count>0
            //that means there's something in the currentDefault list that is not in the passedFromForm. in this case we know user selected a new set of selected answers
            if(ans_category == "3 Val Range" || ans_category == "5 Val Range")
            {
                if (currentDefault.Except(passedFromForm).ToList().Count > 0) { return false; }
                else { return true; }
            }
            else//if we're here, that means the answer type category is MS List or SS List
            {
                //first, lets compare the size of both lists. if they are different, we need to return false 
                if (currentDefault.Count == passedFromForm.Count)
                {
                    //sizes are the same, so lets check
                    //if there are elements that are in the first list but not in the second. if "Except" method returns count>0
                    //that means there's something in the currentDefault list that is not in the passedFromForm. 
                    //in this case we know user selected a new set of selected answers
                    if (currentDefault.Except(passedFromForm).ToList().Count > 0) { return false; }
                    else { return true; }
                }
                else { return false; }

            }
            
        }

        /*
        *       This method deletes assigned selected answers and all the selectable answers if needed  
        */
        public void deleteAssignedSelAns(int qat_id, DSC_OBS_DB_ENTITY ObsDB)
        {
            //first need to check if qat_id is already tied to any Form
            if (ObsDB.OBS_COL_FORM_QUESTIONS.Where(x => x.obs_qat_id == qat_id).ToList().Count > 0)
            {
                //if answer type requires selectable answers, lets deactivate them first
                short ans_type_id = ObsDB.OBS_QUEST_ANS_TYPES.Single(y => y.obs_qat_id == qat_id).obs_ans_type_id;
                if (ObsDB.OBS_ANS_TYPE.Single(x => x.obs_ans_type_id == ans_type_id).obs_ans_type_has_fxd_ans_yn == "Y")
                {
                    List<OBS_QUEST_SLCT_ANS> sel_answers = ObsDB.OBS_QUEST_SLCT_ANS.Where(x => x.obs_qat_id == qat_id && x.obs_qsa_eff_st_dt <= DateTime.Now && x.obs_qsa_eff_end_dt > DateTime.Now).ToList();
                    foreach (OBS_QUEST_SLCT_ANS sel_ans in sel_answers) 
                    {
                        sel_ans.obs_qsa_eff_end_dt = DateTime.Now;                       
                    }
                }
                ObsDB.OBS_QUEST_ANS_TYPES.Single(x => x.obs_qat_id == qat_id).obs_qat_end_eff_dt = DateTime.Now;

            }
            else
            {//ok, this qat id doesn't belong to any form. We can hard delete it
                //but first we need to check if there are selectable answers for this answer type and delete them
                short ans_type_id = ObsDB.OBS_QUEST_ANS_TYPES.Single(y => y.obs_qat_id == qat_id).obs_ans_type_id;
                if (ObsDB.OBS_ANS_TYPE.Single(x=>x.obs_ans_type_id== ans_type_id).obs_ans_type_has_fxd_ans_yn=="Y")
                {
                    ObsDB.OBS_QUEST_SLCT_ANS.RemoveRange(ObsDB.OBS_QUEST_SLCT_ANS.Where(x => x.obs_qat_id == qat_id));
                }               
                ObsDB.OBS_QUEST_ANS_TYPES.Remove(ObsDB.OBS_QUEST_ANS_TYPES.Find(qat_id));
            }
            ObsDB.SaveChanges();
        }
    }
}
