﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OBSMVC.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace OBSMVC.Controllers
{
    public class ColFormTemplateController : Controller
    {
        private DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY();

        // GET: ColFormTemplate
        [HttpGet]
        public ActionResult Index(string title_search, string question_search, string t_search, string q_search,FormCollection form)
        {
            var oBS_COLLECT_FORM_TMPLT = db.OBS_COLLECT_FORM_TMPLT.Include(o => o.DSC_CUSTOMER).Include(o => o.DSC_LC).Include(o => o.OBS_TYPE);
            List<SelectListItem> fullFuncList = setfullFuncList();
            int selectedFunctionId = -1;
            try
            {
                selectedFunctionId = Convert.ToInt32(Request.QueryString["fullFuncList"]);              
            }
            catch { }

            List<ObsColFormTemplate> ObsColFormTemplateList = new List<ObsColFormTemplate>();
            if(selectedFunctionId >0)
            {//case where function is selected
                if (String.IsNullOrWhiteSpace(question_search) && String.IsNullOrWhiteSpace(title_search))
                {//scenario where user didn't pass any search strings
                    foreach (var x in oBS_COLLECT_FORM_TMPLT)
                    {
                        ObsColFormTemplate obsForm = new ObsColFormTemplate();
                        if (obsForm.getAssignedFunctions(x.obs_cft_id).IndexOf(selectedFunctionId)!=-1)
                        {
                            obsForm.OBSformID = x.obs_cft_id;
                            obsForm.FormTitle = x.obs_cft_title;
                            obsForm.FormNumber = x.obs_cft_nbr;
                            obsForm.FormVersion = x.obs_cft_ver;
                            obsForm.Customer = x.DSC_CUSTOMER.dsc_cust_name;
                            obsForm.LC = x.DSC_LC.dsc_lc_name;
                            obsForm.OBS_Type = x.OBS_TYPE.obs_type_name;
                            obsForm.AssignedFunctions = obsForm.getAssignedFunctions(x.obs_cft_id);
                            obsForm.isActive = IsActiveForm(x.obs_cft_eff_st_dt, x.obs_cft_eff_end_dt);
                            obsForm.QuestionCount = obsForm.getAssignedQuestionCount(x.obs_cft_id);
                            obsForm.ObservationCount = obsForm.getTimesCompletedCount(x.obs_cft_id);
                            obsForm.LastCompleteDate = obsForm.getLastCompleteDate(x.obs_cft_id);
                            obsForm.FormSubtitle = x.obs_cft_subtitle;
                            ObsColFormTemplateList.Add(obsForm);
                        }
                        
                    }//end of foreach

                }// end of  if (String.IsNullOrWhiteSpace(question_search) && String.IsNullOrWhiteSpace(title_search))
                else if (String.IsNullOrWhiteSpace(question_search) && !String.IsNullOrWhiteSpace(title_search))
                {// search by title
                    foreach (var x in oBS_COLLECT_FORM_TMPLT)
                    {
                        if (matchesSearchCriteria(title_search, x.obs_cft_title + " " + x.OBS_TYPE.obs_type_name + " " + x.obs_cft_subtitle, t_search))
                        {
                            ObsColFormTemplate obsForm = new ObsColFormTemplate();
                            if(obsForm.getAssignedFunctions(x.obs_cft_id).IndexOf(selectedFunctionId) != -1)
                            {
                                obsForm.OBSformID = x.obs_cft_id;
                                obsForm.FormTitle = string.Format(x.obs_cft_title);
                                obsForm.FormNumber = x.obs_cft_nbr;
                                obsForm.FormVersion = x.obs_cft_ver;
                                obsForm.Customer = x.DSC_CUSTOMER.dsc_cust_name;
                                obsForm.LC = x.DSC_LC.dsc_lc_name;
                                obsForm.OBS_Type = x.OBS_TYPE.obs_type_name;
                                obsForm.AssignedFunctions = obsForm.getAssignedFunctions(x.obs_cft_id);
                                obsForm.isActive = IsActiveForm(x.obs_cft_eff_st_dt, x.obs_cft_eff_end_dt);
                                obsForm.QuestionCount = obsForm.getAssignedQuestionCount(x.obs_cft_id);
                                obsForm.ObservationCount = obsForm.getTimesCompletedCount(x.obs_cft_id);
                                obsForm.LastCompleteDate = obsForm.getLastCompleteDate(x.obs_cft_id);
                                obsForm.FormSubtitle = x.obs_cft_subtitle;
                                ObsColFormTemplateList.Add(obsForm);
                            }
                            
                        }

                    }//end of foreach
                } // end of  else if (String.IsNullOrWhiteSpace(question_search) && !String.IsNullOrWhiteSpace(title_search))
                else if (!String.IsNullOrWhiteSpace(question_search) && String.IsNullOrWhiteSpace(title_search))
                {//search by question
                    List<int> cft_with_matching_questions = searchQuestionsWithMatchingSearchCriteria(question_search, q_search);
                    foreach (var x in oBS_COLLECT_FORM_TMPLT)
                    {
                        if (cft_with_matching_questions.Count() > 0 && cft_with_matching_questions.IndexOf(x.obs_cft_id) != -1)
                        {
                            ObsColFormTemplate obsForm = new ObsColFormTemplate();
                            if (obsForm.getAssignedFunctions(x.obs_cft_id).IndexOf(selectedFunctionId) != -1)
                            {
                                obsForm.OBSformID = x.obs_cft_id;
                                obsForm.FormTitle = x.obs_cft_title;
                                obsForm.FormNumber = x.obs_cft_nbr;
                                obsForm.FormVersion = x.obs_cft_ver;
                                obsForm.Customer = x.DSC_CUSTOMER.dsc_cust_name;
                                obsForm.LC = x.DSC_LC.dsc_lc_name;
                                obsForm.OBS_Type = x.OBS_TYPE.obs_type_name;
                                obsForm.AssignedFunctions = obsForm.getAssignedFunctions(x.obs_cft_id);
                                obsForm.isActive = IsActiveForm(x.obs_cft_eff_st_dt, x.obs_cft_eff_end_dt);
                                obsForm.QuestionCount = obsForm.getAssignedQuestionCount(x.obs_cft_id);
                                obsForm.ObservationCount = obsForm.getTimesCompletedCount(x.obs_cft_id);
                                obsForm.LastCompleteDate = obsForm.getLastCompleteDate(x.obs_cft_id);
                                obsForm.FormSubtitle = x.obs_cft_subtitle;
                                ObsColFormTemplateList.Add(obsForm);
                            }
                              

                        }
                    }
                }// end of  else if (!String.IsNullOrWhiteSpace(question_search) && String.IsNullOrWhiteSpace(title_search))
                else if (!String.IsNullOrWhiteSpace(question_search) && !String.IsNullOrWhiteSpace(title_search))
                {// search by both title and question
                    List<int> cft_with_matching_questions = searchQuestionsWithMatchingSearchCriteria(question_search, q_search);
                    foreach (var x in oBS_COLLECT_FORM_TMPLT)
                    {
                        if ((matchesSearchCriteria(title_search, x.obs_cft_title + " " + x.OBS_TYPE.obs_type_name + " " + x.obs_cft_subtitle, t_search)) && ((cft_with_matching_questions.Count() > 0 && cft_with_matching_questions.IndexOf(x.obs_cft_id) != -1)))
                        {
                            ObsColFormTemplate obsForm = new ObsColFormTemplate();
                            if (obsForm.getAssignedFunctions(x.obs_cft_id).IndexOf(selectedFunctionId) != -1)
                            {
                                obsForm.OBSformID = x.obs_cft_id;
                                obsForm.FormTitle = x.obs_cft_title;
                                obsForm.FormNumber = x.obs_cft_nbr;
                                obsForm.FormVersion = x.obs_cft_ver;
                                obsForm.Customer = x.DSC_CUSTOMER.dsc_cust_name;
                                obsForm.LC = x.DSC_LC.dsc_lc_name;
                                obsForm.OBS_Type = x.OBS_TYPE.obs_type_name;
                                obsForm.AssignedFunctions = obsForm.getAssignedFunctions(x.obs_cft_id);
                                obsForm.isActive = IsActiveForm(x.obs_cft_eff_st_dt, x.obs_cft_eff_end_dt);
                                obsForm.QuestionCount = obsForm.getAssignedQuestionCount(x.obs_cft_id);
                                obsForm.ObservationCount = obsForm.getTimesCompletedCount(x.obs_cft_id);
                                obsForm.LastCompleteDate = obsForm.getLastCompleteDate(x.obs_cft_id);
                                obsForm.FormSubtitle = x.obs_cft_subtitle;
                                ObsColFormTemplateList.Add(obsForm);//title search match

                            }
                              
                        }

                    }//end of foreach

                }
                fullFuncList.Single(x =>x.Value== Request.QueryString["fullFuncList"]).Selected = true;
            }// end of if(selectedFunctionId >0)
            else
            {//case where function is NOT passed

                if (String.IsNullOrWhiteSpace(question_search) && String.IsNullOrWhiteSpace(title_search))
                {//scenario where user didn't pass any search strings
                    foreach (var x in oBS_COLLECT_FORM_TMPLT)
                    {
                        ObsColFormTemplate obsForm = new ObsColFormTemplate();

                        obsForm.OBSformID = x.obs_cft_id;
                        obsForm.FormTitle = x.obs_cft_title;
                        obsForm.FormNumber = x.obs_cft_nbr;
                        obsForm.FormVersion = x.obs_cft_ver;
                        obsForm.Customer = x.DSC_CUSTOMER.dsc_cust_name;
                        obsForm.LC = x.DSC_LC.dsc_lc_name;
                        obsForm.OBS_Type = x.OBS_TYPE.obs_type_name;
                        obsForm.AssignedFunctions = obsForm.getAssignedFunctions(x.obs_cft_id);
                        obsForm.isActive = IsActiveForm(x.obs_cft_eff_st_dt, x.obs_cft_eff_end_dt);
                        obsForm.QuestionCount = obsForm.getAssignedQuestionCount(x.obs_cft_id);
                        obsForm.ObservationCount = obsForm.getTimesCompletedCount(x.obs_cft_id);
                        obsForm.LastCompleteDate = obsForm.getLastCompleteDate(x.obs_cft_id);
                        obsForm.FormSubtitle = x.obs_cft_subtitle;
                        ObsColFormTemplateList.Add(obsForm);
                    }//end of foreach

                }// end of  if (String.IsNullOrWhiteSpace(question_search) && String.IsNullOrWhiteSpace(title_search))
                else if (String.IsNullOrWhiteSpace(question_search) && !String.IsNullOrWhiteSpace(title_search))
                {// search by title
                    foreach (var x in oBS_COLLECT_FORM_TMPLT)
                    {
                        if (matchesSearchCriteria(title_search, x.obs_cft_title + " " + x.OBS_TYPE.obs_type_name + " " + x.obs_cft_subtitle, t_search))
                        {
                            ObsColFormTemplate obsForm = new ObsColFormTemplate();

                            obsForm.OBSformID = x.obs_cft_id;
                            obsForm.FormTitle = x.obs_cft_title;
                            obsForm.FormNumber = x.obs_cft_nbr;
                            obsForm.FormVersion = x.obs_cft_ver;
                            obsForm.Customer = x.DSC_CUSTOMER.dsc_cust_name;
                            obsForm.LC = x.DSC_LC.dsc_lc_name;
                            obsForm.OBS_Type = x.OBS_TYPE.obs_type_name;
                            obsForm.AssignedFunctions = obsForm.getAssignedFunctions(x.obs_cft_id);
                            obsForm.isActive = IsActiveForm(x.obs_cft_eff_st_dt, x.obs_cft_eff_end_dt);
                            obsForm.QuestionCount = obsForm.getAssignedQuestionCount(x.obs_cft_id);
                            obsForm.ObservationCount = obsForm.getTimesCompletedCount(x.obs_cft_id);
                            obsForm.LastCompleteDate = obsForm.getLastCompleteDate(x.obs_cft_id);
                            obsForm.FormSubtitle = x.obs_cft_subtitle;
                            ObsColFormTemplateList.Add(obsForm);
                        }

                    }//end of foreach
                } // end of  else if (String.IsNullOrWhiteSpace(question_search) && !String.IsNullOrWhiteSpace(title_search))
                else if (!String.IsNullOrWhiteSpace(question_search) && String.IsNullOrWhiteSpace(title_search))
                {//search by question
                    List<int> cft_with_matching_questions = searchQuestionsWithMatchingSearchCriteria(question_search, q_search);
                    foreach (var x in oBS_COLLECT_FORM_TMPLT)
                    {
                        if (cft_with_matching_questions.Count() > 0 && cft_with_matching_questions.IndexOf(x.obs_cft_id) != -1)
                        {
                            ObsColFormTemplate obsForm = new ObsColFormTemplate();

                            obsForm.OBSformID = x.obs_cft_id;
                            obsForm.FormTitle = x.obs_cft_title;
                            obsForm.FormNumber = x.obs_cft_nbr;
                            obsForm.FormVersion = x.obs_cft_ver;
                            obsForm.Customer = x.DSC_CUSTOMER.dsc_cust_name;
                            obsForm.LC = x.DSC_LC.dsc_lc_name;
                            obsForm.OBS_Type = x.OBS_TYPE.obs_type_name;
                            obsForm.AssignedFunctions = obsForm.getAssignedFunctions(x.obs_cft_id);
                            obsForm.isActive = IsActiveForm(x.obs_cft_eff_st_dt, x.obs_cft_eff_end_dt);
                            obsForm.QuestionCount = obsForm.getAssignedQuestionCount(x.obs_cft_id);
                            obsForm.ObservationCount = obsForm.getTimesCompletedCount(x.obs_cft_id);
                            obsForm.LastCompleteDate = obsForm.getLastCompleteDate(x.obs_cft_id);
                            obsForm.FormSubtitle = x.obs_cft_subtitle;
                            ObsColFormTemplateList.Add(obsForm);

                        }
                    }
                }// end of  else if (!String.IsNullOrWhiteSpace(question_search) && String.IsNullOrWhiteSpace(title_search))
                else if (!String.IsNullOrWhiteSpace(question_search) && !String.IsNullOrWhiteSpace(title_search))
                {// search by both title and question
                    List<int> cft_with_matching_questions = searchQuestionsWithMatchingSearchCriteria(question_search, q_search);
                    foreach (var x in oBS_COLLECT_FORM_TMPLT)
                    {
                        if ((matchesSearchCriteria(title_search, x.obs_cft_title + " " + x.OBS_TYPE.obs_type_name + " " + x.obs_cft_subtitle, t_search)) && ((cft_with_matching_questions.Count() > 0 && cft_with_matching_questions.IndexOf(x.obs_cft_id) != -1)))
                        {
                            ObsColFormTemplate obsForm = new ObsColFormTemplate();

                            obsForm.OBSformID = x.obs_cft_id;
                            obsForm.FormTitle = x.obs_cft_title;
                            obsForm.FormNumber = x.obs_cft_nbr;
                            obsForm.FormVersion = x.obs_cft_ver;
                            obsForm.Customer = x.DSC_CUSTOMER.dsc_cust_name;
                            obsForm.LC = x.DSC_LC.dsc_lc_name;
                            obsForm.OBS_Type = x.OBS_TYPE.obs_type_name;
                            obsForm.AssignedFunctions = obsForm.getAssignedFunctions(x.obs_cft_id);
                            obsForm.isActive = IsActiveForm(x.obs_cft_eff_st_dt, x.obs_cft_eff_end_dt);
                            obsForm.QuestionCount = obsForm.getAssignedQuestionCount(x.obs_cft_id);
                            obsForm.ObservationCount = obsForm.getTimesCompletedCount(x.obs_cft_id);
                            obsForm.LastCompleteDate = obsForm.getLastCompleteDate(x.obs_cft_id);
                            obsForm.FormSubtitle = x.obs_cft_subtitle;
                            ObsColFormTemplateList.Add(obsForm);//title search match
                        }

                    }//end of foreach

                }

            }

            ViewBag.fullFuncList = fullFuncList;
            return View(ObsColFormTemplateList.ToList());
        }

        // GET: ColFormTemplate/Details/5
        public ActionResult Details(int id)
        {
            //if (id == null) { return HttpNotFound(); }

            CollectionForm selectedColForm = new CollectionForm(id);
            if (selectedColForm == null) { return HttpNotFound(); }
            
            return View(selectedColForm);
        }

        // GET: ColFormTemplate/Create
        public ActionResult Create()
        {
            ViewBag.dsc_cust_id = new SelectList(db.DSC_CUSTOMER, "dsc_cust_id", "dsc_cust_name");
            ViewBag.dsc_lc_id = new SelectList(db.DSC_LC, "dsc_lc_id", "dsc_lc_name");
            ViewBag.obs_type_id = new SelectList(db.OBS_TYPE, "obs_type_id", "obs_type_name");
            return View();
        }
        
        //GET: List of Quesions
        [HttpGet]
        public PartialViewResult getQuestions(string full_text_search, string metadata_search, int? page, int? pageSize)
        {
            //System.Threading.Thread.Sleep(2000);
            List<AvailableQuestions> availableQuestions = new List<AvailableQuestions>();
            if (String.IsNullOrWhiteSpace(full_text_search) && String.IsNullOrWhiteSpace(metadata_search))
            {//no search parameters passed
               
                List<OBS_QUESTION> list_of_questions = db.OBS_QUESTION.Where(item => item.obs_question_eff_st_dt <= DateTime.Now && item.obs_question_eff_end_dt > DateTime.Now).OrderBy(x => x.obs_question_id).ToList();
                foreach (OBS_QUESTION q in list_of_questions)
                {
                    AvailableQuestions quest = new AvailableQuestions();
                    quest.obs_question_id = q.obs_question_id;
                    quest.obs_question_full_text = q.obs_question_full_text;
                    quest.assigned_metadata = quest.getAssignedMetadata(q.obs_question_id);
                    availableQuestions.Add(quest);

                }
            }
            else if (!String.IsNullOrWhiteSpace(full_text_search) && String.IsNullOrWhiteSpace(metadata_search))
            {//search by question text
               
                List<OBS_QUESTION> list_of_questions = db.OBS_QUESTION.Where(item => item.obs_question_eff_st_dt <= DateTime.Now && item.obs_question_eff_end_dt > DateTime.Now && item.obs_question_full_text.ToLower().Contains(full_text_search.ToLower())).ToList();
                if(list_of_questions.Count>0)
                {
                    foreach (OBS_QUESTION q in list_of_questions)
                    {
                        AvailableQuestions quest = new AvailableQuestions();
                        quest.obs_question_id = q.obs_question_id;
                        quest.obs_question_full_text = q.obs_question_full_text;
                        quest.assigned_metadata = quest.getAssignedMetadata(q.obs_question_id);
                        availableQuestions.Add(quest);

                    }
                }                
            }
            else if (String.IsNullOrWhiteSpace(full_text_search) && !String.IsNullOrWhiteSpace(metadata_search))
            {//search by metadata

                List<OBS_QUESTION> list_of_questions = db.OBS_QUESTION.Where(item => item.obs_question_eff_st_dt <= DateTime.Now && item.obs_question_eff_end_dt > DateTime.Now ).ToList();

                foreach (OBS_QUESTION q in list_of_questions)
                {
                    AvailableQuestions quest = new AvailableQuestions();
                    if (quest.getAssignedMetadata(q.obs_question_id).Count > 0)
                    {
                        quest.assigned_metadata = quest.getAssignedMetadata(q.obs_question_id);
                        foreach (string s in quest.assigned_metadata)
                        {
                            if(s.ToLower().Contains(metadata_search.ToLower()))
                            {
                        quest.obs_question_id = q.obs_question_id;
                        quest.obs_question_full_text = q.obs_question_full_text;
                                //quest.assigned_metadata = quest.getAssignedMetadata(q.obs_question_id);
                        availableQuestions.Add(quest);
                    }
                            else { continue; }
                           
                }               
                        
                       
            }
                }               
            }
            else if (!String.IsNullOrWhiteSpace(full_text_search) && !String.IsNullOrWhiteSpace(metadata_search))
            {//search by metadata and full text

                List<OBS_QUESTION> list_of_questions = db.OBS_QUESTION.Where(item => item.obs_question_eff_st_dt <= DateTime.Now && item.obs_question_eff_end_dt > DateTime.Now && item.obs_question_full_text.ToLower().Contains(full_text_search.ToLower())).ToList();

                if (list_of_questions.Count > 0)
                {
                    foreach (OBS_QUESTION q in list_of_questions)
                    {
                        AvailableQuestions quest = new AvailableQuestions();
                        if (quest.getAssignedMetadata(q.obs_question_id).Count > 0)
                        {
                            quest.assigned_metadata = quest.getAssignedMetadata(q.obs_question_id);
                            foreach(string s in quest.assigned_metadata)
                            {
                                if (s.ToLower().Contains(metadata_search.ToLower()))
                                {
                            quest.obs_question_id = q.obs_question_id;
                            quest.obs_question_full_text = q.obs_question_full_text;
                            availableQuestions.Add(quest);
                        }
                                else { continue; }
                            }
                            
                        }
                    }
                }
                   
            }
           
            List<AvailableQuestions> questions_for_display = availableQuestions.OrderBy(x => x.obs_question_id).Skip(((page ?? 1) - 1) * (pageSize ?? 10)).Take(pageSize ?? 10).ToList();      
            return PartialView("_getQuestions", questions_for_display);            
        }

        // POST: ColFormTemplate/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "obs_cft_id,obs_type_id,dsc_cust_id,dsc_lc_id,obs_cft_nbr,obs_cft_ver,obs_cft_eff_st_dt,obs_cft_eff_end_dt,obs_cft_title,obs_cft_subtitle")] OBS_COLLECT_FORM_TMPLT oBS_COLLECT_FORM_TMPLT)
        {
            if (ModelState.IsValid)
            {
                db.OBS_COLLECT_FORM_TMPLT.Add(oBS_COLLECT_FORM_TMPLT);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.dsc_cust_id = new SelectList(db.DSC_CUSTOMER, "dsc_cust_id", "dsc_cust_name", oBS_COLLECT_FORM_TMPLT.dsc_cust_id);
            ViewBag.dsc_lc_id = new SelectList(db.DSC_LC, "dsc_lc_id", "dsc_lc_name", oBS_COLLECT_FORM_TMPLT.dsc_lc_id);
            ViewBag.obs_type_id = new SelectList(db.OBS_TYPE, "obs_type_id", "obs_type_name", oBS_COLLECT_FORM_TMPLT.obs_type_id);
            return View(oBS_COLLECT_FORM_TMPLT);
        }

        // GET: ColFormTemplate/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OBS_COLLECT_FORM_TMPLT oBS_COLLECT_FORM_TMPLT = db.OBS_COLLECT_FORM_TMPLT.Find(id);
            if (oBS_COLLECT_FORM_TMPLT == null)
            {
                return HttpNotFound();
            }
            ViewBag.dsc_cust_id = new SelectList(db.DSC_CUSTOMER, "dsc_cust_id", "dsc_cust_name", oBS_COLLECT_FORM_TMPLT.dsc_cust_id);
            ViewBag.dsc_lc_id = new SelectList(db.DSC_LC, "dsc_lc_id", "dsc_lc_name", oBS_COLLECT_FORM_TMPLT.dsc_lc_id);
            ViewBag.obs_type_id = new SelectList(db.OBS_TYPE, "obs_type_id", "obs_type_name", oBS_COLLECT_FORM_TMPLT.obs_type_id);
            return View(oBS_COLLECT_FORM_TMPLT);
        }

        // POST: ColFormTemplate/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "obs_cft_id,obs_type_id,dsc_cust_id,dsc_lc_id,obs_cft_nbr,obs_cft_ver,obs_cft_eff_st_dt,obs_cft_eff_end_dt,obs_cft_title,obs_cft_subtitle")] OBS_COLLECT_FORM_TMPLT oBS_COLLECT_FORM_TMPLT)
        {
            if (ModelState.IsValid)
            {
                db.Entry(oBS_COLLECT_FORM_TMPLT).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.dsc_cust_id = new SelectList(db.DSC_CUSTOMER, "dsc_cust_id", "dsc_cust_name", oBS_COLLECT_FORM_TMPLT.dsc_cust_id);
            ViewBag.dsc_lc_id = new SelectList(db.DSC_LC, "dsc_lc_id", "dsc_lc_name", oBS_COLLECT_FORM_TMPLT.dsc_lc_id);
            ViewBag.obs_type_id = new SelectList(db.OBS_TYPE, "obs_type_id", "obs_type_name", oBS_COLLECT_FORM_TMPLT.obs_type_id);
            return View(oBS_COLLECT_FORM_TMPLT);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public class ObsColFormTemplate
        {
            private DSC_OBS_DB_ENTITY OBSdb = new DSC_OBS_DB_ENTITY();

            //Constructor


            //------------------------------------------PROPERTIES---------------------------------------------------//

            [Display(Name = "Form ID")]
            public int OBSformID { get; set; }

            [Display(Name = "Form Title")]
            public String FormTitle { get; set; }

            [Display(Name = "Form Number")]
            public short FormNumber { get; set; }

            [Display(Name = "Form Version")]
            public short FormVersion { get; set; }

            [Display(Name = "Customer")]
            public string Customer { get; set; }

            [Display(Name = "LC")]
            public string LC { get; set; }

            [Display(Name = "Observation Type")]
            public string OBS_Type { get; set; }

            [Display(Name = "Number of Questions")]
            public int QuestionCount { get; set; }
            [Display(Name = "Number of Observations")]
            public int ObservationCount { get; set; }
            [Display(Name = "Active")]
            public bool isActive { get; set; }

            [Display(Name = "Last Submitted on")]
            [DisplayFormat(DataFormatString ="{0:g}")]
            public DateTime? LastCompleteDate { get; set; }

            public string FormSubtitle { get; set; }
            public List<int> AssignedFunctions = new List<int>();

           
            public int getAssignedQuestionCount(int cft_id)
            {
                int quest_count = OBSdb.OBS_COL_FORM_QUESTIONS.Where(item => item.obs_cft_id == cft_id).Count();
                return quest_count;
            }
            public int getTimesCompletedCount(int cft_id)
            {
                int times_compleded = OBSdb.OBS_COLLECT_FORM_INST.Where(item => item.obs_cft_id == cft_id && !item.obs_cfi_comp_date.Equals(null)).Count();
                return times_compleded;
            }
           public DateTime? getLastCompleteDate(int cft_id)
            {
                return  OBSdb.OBS_COLLECT_FORM_INST.Where(item => item.obs_cft_id == cft_id).Max(x => x.obs_cfi_comp_date).Equals(null) ? null : OBSdb.OBS_COLLECT_FORM_INST.Where(item => item.obs_cft_id == cft_id).Max(x => x.obs_cfi_comp_date);
                
            }
           public List<int> getAssignedFunctions(int cft_if)
            {
                List<int> AssignedFunctions = (from fc in OBSdb.OBS_COLLECT_FORM_TMPLT
                                                  join t in OBSdb.OBS_TYPE on fc.obs_type_id equals t.obs_type_id
                                                  join ots in OBSdb.OBS_TYPE_SUB_TYPES on t.obs_type_id equals ots.obs_type_id
                                                  join os in OBSdb.OBS_SUB_TYPE on ots.obs_sub_type_id equals os.obs_sub_type_id
                                                  where fc.obs_cft_id == cft_if && os.obs_sub_type_group == "FUNCTION"
                                                  select os.obs_sub_type_id).ToList();
                return AssignedFunctions;
            }
          



        }
        
        //---------------------------------------------HELPERS----------------------------------------//
        public List<int> searchQuestionsWithMatchingSearchCriteria(string search_for_string, string search_criteria)
        {
             DSC_OBS_DB_ENTITY OBSdb = new DSC_OBS_DB_ENTITY();
        List<int> cft_ids_with_matching_qiestions = new List<int>();
           
            List<OBS_QUESTION> list_of_questions = (from q in OBSdb.OBS_QUESTION
                                                    join qa in OBSdb.OBS_QUEST_ANS_TYPES
                                                    on q.obs_question_id equals qa.obs_question_id
                                                    join fq in OBSdb.OBS_COL_FORM_QUESTIONS
                                                     on qa.obs_qat_id equals fq.obs_qat_id

                                                    select q).Distinct().ToList();
            string[] splitterm = { " " };
            string[] splitted_search_string = search_for_string.Split(splitterm, StringSplitOptions.RemoveEmptyEntries);
            switch (search_criteria)
            {
                case "Any":
                    foreach (OBS_QUESTION quest in list_of_questions)
                    {                       
                        if (matchesSearchCriteria(search_for_string, quest.obs_question_full_text, search_criteria))
                        {

                              List<int> temp  = (from q in OBSdb.OBS_QUESTION
                                                               join qa in OBSdb.OBS_QUEST_ANS_TYPES
                                                               on q.obs_question_id equals qa.obs_question_id
                                                               join fq in OBSdb.OBS_COL_FORM_QUESTIONS
                                                                on qa.obs_qat_id equals fq.obs_qat_id
                                                               where q.obs_question_id == quest.obs_question_id
                                                               select fq.obs_cft_id).Distinct().ToList();
                            cft_ids_with_matching_qiestions = temp.Union(cft_ids_with_matching_qiestions).ToList();
                        }
                        
                    }
                    break;

                case "All":
                    foreach (OBS_QUESTION quest in list_of_questions)
                    {
                        if (matchesSearchCriteria(search_for_string, quest.obs_question_full_text, search_criteria))
                        {
                            List<int> temp = (from q in OBSdb.OBS_QUESTION
                                                               join qa in OBSdb.OBS_QUEST_ANS_TYPES
                                                               on q.obs_question_id equals qa.obs_question_id
                                                               join fq in OBSdb.OBS_COL_FORM_QUESTIONS
                                                                on qa.obs_qat_id equals fq.obs_qat_id
                                                               where q.obs_question_id == quest.obs_question_id
                                                               select fq.obs_cft_id).Distinct().ToList();
                            cft_ids_with_matching_qiestions = temp.Union(cft_ids_with_matching_qiestions).ToList();
                        }
                    }
                    break;

                case "Exact":
                    cft_ids_with_matching_qiestions = (from q in OBSdb.OBS_QUESTION
                                                       join qa in OBSdb.OBS_QUEST_ANS_TYPES
                                                       on q.obs_question_id equals qa.obs_question_id
                                                       join fq in OBSdb.OBS_COL_FORM_QUESTIONS
                                                        on qa.obs_qat_id equals fq.obs_qat_id
                                                       where q.obs_question_full_text.Contains(search_for_string)
                                                       select fq.obs_cft_id).Distinct().ToList();
                    return cft_ids_with_matching_qiestions;

            }


            return cft_ids_with_matching_qiestions;
        }
        public static bool IsActiveForm(DateTime start_date, DateTime end_date)
        {
            if (DateTime.Today >= start_date && DateTime.Today < end_date) { return true; }
            else { return false; }
        }
        
        public static bool matchesSearchCriteria(string search_for_string, string search_in, string search_criteria)
        {
            string[] splitterm = { " " };
            string[] splitted_search_string = search_for_string.Split(splitterm, StringSplitOptions.RemoveEmptyEntries);
            switch (search_criteria)
            {
                case "Any":
                    foreach (string s in splitted_search_string)
                    {
                        if (search_in.ToLower().Contains(s.ToLower())) { return true; }
                        else { continue; }
                    }
                    return false;
                
                case "All":
                    foreach (string s in splitted_search_string)
                    {
                        if (search_in.ToLower().Contains(s.ToLower())) { continue; }
                        else { return false; }
                    }
                    return true;
                  
                case "Exact":
                    if (search_in.ToLower().Contains(search_for_string.ToLower())) { return true; }
                    else return false;

                default:
                   return false;
            }
            
        }
        public  List<SelectListItem> setfullFuncList()
        {
            List<OBS_SUB_TYPE> oBS_SUB_TYPE = db.OBS_SUB_TYPE.Where(x => x.obs_sub_type_group == "FUNCTION").ToList();
         
            List<SelectListItem> fullFuncList = new List<SelectListItem>();
            foreach (OBS_SUB_TYPE temp in oBS_SUB_TYPE)
            {
                SelectListItem fullFuncListItem = new SelectListItem();
                fullFuncListItem.Value = temp.obs_sub_type_id.ToString();
                fullFuncListItem.Text = temp.obs_sub_type_name;
                fullFuncList.Add(fullFuncListItem);
            }
            return fullFuncList;
        }
    }
    //\==================== END OF CONTROLLERS CLASS ==================================================/


    // =================================================================================================
    // ============================ HELPER CLASES FOR CONTROLERS  ======================================
    //******* CLASES ***********************************************************************************
    public class CollectionForm
    {
        private DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY();  //To get Database access inside this Class

        //--- CONSTRUCTOR------------------
        public CollectionForm(int id)
        {//Ctreate the Collection Form Data (Header Info) from the Id passed as a parameter

            cft_id = id;
            // Query the Data from the Database
            var q = (from A in db.OBS_COLLECT_FORM_TMPLT
                     join B in db.OBS_TYPE                          //First Table Left join
                         on A.obs_type_id equals B.obs_type_id
                         into tl_b
                     where A.obs_cft_id == cft_id
                     from B in tl_b.DefaultIfEmpty()
                     join C in db.DSC_CUSTOMER                      //Second Table Left join
                     on A.dsc_cust_id equals C.dsc_cust_id into tl_c
                     from C in tl_c.DefaultIfEmpty()
                     join D in db.DSC_LC                            //Second Table Left join
                     on A.dsc_lc_id equals D.dsc_lc_id into tl_d
                     from D in tl_d.DefaultIfEmpty()
                     select new
                     {
                         cft_id = A.obs_cft_id,
                         cft_Nbr = A.obs_cft_nbr,
                         cft_Version = A.obs_cft_ver,
                         cft_Title = A.obs_cft_title,
                         cft_SubTitle = A.obs_cft_subtitle,
                         cft_obsType = B.obs_type_name,
                         cft_Cust = C.dsc_cust_name,
                         cft_LC = D.dsc_lc_name,
                         cft_Status = ((A.obs_cft_eff_st_dt < DateTime.Now) && (A.obs_cft_eff_end_dt > DateTime.Now)) ? "ACTIVE" : "INACTIVE"
                     }).ToList().FirstOrDefault();
            // Set the properties from query result
            cft_Title = q.cft_Title;
            cft_SubTitle = q.cft_SubTitle;
            cft_obsType = q.cft_obsType;
            cft_Cust = q.cft_Cust;
            cft_LC = q.cft_LC;
            cft_Status = q.cft_Status;
            cft_Nbr = q.cft_Nbr;
            cft_Version = q.cft_Version;
            colFormSections = new List<CollectionFormSection>();
            retrieveQuestionData();
        }

        //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - \\
        //- - - - - - - - - - Properties - - - - - - - - - - - - - - - - - - - - |
        // All Properties are set at Constructor Time
        public int cft_id { get; set; }
        public string cft_Title { get; set; }
        public string cft_SubTitle { get; set; }
        public string cft_obsType { get; set; }
        public string cft_Cust { get; set; }
        public string cft_LC { get; set; }
        public string cft_Status { get; set; }
        public int cft_Nbr { get; set; }
        public int cft_Version { get; set; }
        public List<CollectionFormSection> colFormSections { get; set; }        
        //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -\\
        //- - - - - - - - - - - - CLASS METHODS - - - - - - - - - - - - - - - - |
        private void retrieveQuestionData()
        {
            DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY();  //To give Database access to this Class
            // Pull all the questions information for the selected Form in the correct order
            var formQuestions = db.OBS_COL_FORM_QUESTIONS.Include(o => o.OBS_FORM_SECTION).Where(x => x.obs_cft_id == cft_id).ToList().OrderBy(y => y.obs_col_form_quest_order);
            if (formQuestions.Count() > 0)
            { //Pupulate the Form Sections and Questions List only if there is question data for the selcted Form
                int sectionCounter = 0;
                string oldSectionName = "undefined";
                string newSectionName = String.Empty;
                int questionCounter = 1;
                CollectionFormSection oSection = new CollectionFormSection();
                foreach (OBS_COL_FORM_QUESTIONS q in formQuestions)
                {
                    newSectionName = q.OBS_FORM_SECTION.obs_form_section_name;
                    //If the section name has changed, then we are in a new section
                    if (!newSectionName.Equals(oldSectionName))   // --- If this is a new Section ---
                    {
                        sectionCounter++;
                      // If this is not the first section. Add the old Section to the List
                        if (!oldSectionName.Equals("undefined")) { colFormSections.Add(oSection); }
                      //Create a new Section from scratch
                        oSection = new CollectionFormSection();
                        oSection.sectionNumber = sectionCounter;
                        oSection.sectionName = q.OBS_FORM_SECTION.obs_form_section_name;
                        oSection.sectionViewId = "viewSection" + oSection.sectionNumber.ToString();
                    }

                    //Create New question, Populate the Info and Add it to the current Form section
                    CollectionFormQuestion oQuestion = new CollectionFormQuestion();
                    oQuestion.cfq_id = q.obs_col_form_quest_id;
                    oQuestion.cfq_questId = q.OBS_QUEST_ANS_TYPES.obs_question_id;
                    oQuestion.cfq_order = q.obs_col_form_quest_order;
                    oQuestion.cfq_seqInForm = questionCounter.ToString("00");
                    oQuestion.cfq_fullText = q.OBS_QUEST_ANS_TYPES.OBS_QUESTION.obs_question_full_text.Replace(": (",":<br/>(");
                    oQuestion.cfq_AT = q.OBS_QUEST_ANS_TYPES.OBS_ANS_TYPE.obs_ans_type_name;
                    oQuestion.cfq_qatId = q.obs_qat_id;
                    oQuestion.cfq_SelectableAnswers = q.OBS_QUEST_ANS_TYPES.OBS_QUEST_SLCT_ANS.OrderBy(xx => xx.obs_qsa_order).Select(x => x.obs_qsa_text).ToList();
                    // .... Populate the rest of the oQuestion properties
                    oSection.colFormQuestionList.Add(oQuestion);
                    
                    // reset the name of the old  section indicator
                    oldSectionName = newSectionName;
                    questionCounter++;
                } // End of For-each question loop
                //Finally add the last populated section to the section List
                colFormSections.Add(oSection);            
            }
        }
        //------------------
    }

    public class CollectionFormSection
    {
        public int sectionNumber = 0;
       public string sectionName = String.Empty;
       public string sectionViewId = String.Empty;
       public List<CollectionFormQuestion> colFormQuestionList { get; set; }
       // --- Constructor --------
       public CollectionFormSection() { colFormQuestionList = new List<CollectionFormQuestion>(); }
    }
    public class CollectionFormQuestion
    {
        //--- CONSTRUCTOR------------------
        public CollectionFormQuestion() { }
        //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - \\
        //- - - - - - - - - - Properties - - - - - - - - - - - - - - - - - - - - |
        // All Properties are set at Constructor Time
        public long cfq_id { get; set; }
        public long cfq_qatId { get; set; }
        public string cfq_seqInForm { get; set; }
        public int cfq_order { get; set; }
        public int cfq_questId { get; set; }
        public string cfq_fullText { get; set; }
        public string cfq_AT { get; set; }
        public List<string> cfq_SelectableAnswers { get; set; }
        public List<string> cfq_AnsHTML { get; set; }      //HTML Code passes to the view to render the answer info
        
        //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -\\
        //- - - - - - - - - - - - CLASS METHODS - - - - - - - - - - - - - - - - |
        //.... TODO ....
        //------------------
    }

    public class AvailableQuestions
    {
        private DSC_OBS_DB_ENTITY OBSdb = new DSC_OBS_DB_ENTITY();
        [Required]
        [Display(Name = "ID")]
        public int obs_question_id { set; get; }
        [Required]
        [Display(Name = "Full Text")]
        public string obs_question_full_text { set; get; }
        [Display(Name="Assigned Meta Data")]
        public List<string> assigned_metadata;

        public List<string> getAssignedMetadata(int qid)
        {

            List<string> metadata = (from q in OBSdb.OBS_QUESTION
                                     join qam in OBSdb.OBS_QUEST_ASSGND_MD on
                                          q.obs_question_id equals qam.obs_question_id
                                     join md in OBSdb.OBS_QUESTION_METADATA on
                                          qam.obs_quest_md_id equals md.obs_quest_md_id
                                     where qam.obs_qad_eff_st_dt <= DateTime.Now && qam.obs_qad_eff_end_dt > DateTime.Now && q.obs_question_id == qid
                                     select md.obs_quest_md_value + " [" + md.obs_quest_md_cat + "]").Distinct().ToList();
            return metadata;
        }


    }


}
