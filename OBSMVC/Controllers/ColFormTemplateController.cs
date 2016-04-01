using System;
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
        public ActionResult Index(string title_search, string question_search, string t_search, string q_search, FormCollection form)
        {
            string onlyPublished = "N";
            try { onlyPublished = Request.QueryString["chkPublished"]; }
            catch { }
            string onlyLive = "N";
            try { onlyLive = Request.QueryString["chkLive"]; }
            catch { }
            var oBS_COLLECT_FORM_TMPLT = db.OBS_COLLECT_FORM_TMPLT.Include(o => o.DSC_CUSTOMER).Include(o => o.DSC_LC).Include(o => o.OBS_TYPE);
            List<SelectListItem> fullFuncList = setfullFuncList();
            int selectedFunctionId = -1;
            try
            {
                selectedFunctionId = Convert.ToInt32(Request.QueryString["fullFuncList"]);
            }
            catch { }

            List<ObsColFormTemplate> ObsColFormTemplateList = new List<ObsColFormTemplate>(
                
                );
            if (selectedFunctionId > 0)
            {//case where function is selected
                if (String.IsNullOrWhiteSpace(question_search) && String.IsNullOrWhiteSpace(title_search))
                {//scenario where user didn't pass any search strings
                    foreach (var x in oBS_COLLECT_FORM_TMPLT)
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
                            obsForm.isPublished = isPublishedForm(x.obs_cft_pub_dtm);
                            obsForm.isActive = IsActiveForm(x.obs_cft_eff_st_dt, x.obs_cft_eff_end_dt, obsForm.isPublished);
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
                            if (obsForm.getAssignedFunctions(x.obs_cft_id).IndexOf(selectedFunctionId) != -1)
                            {
                                obsForm.OBSformID = x.obs_cft_id;
                                obsForm.FormTitle = string.Format(x.obs_cft_title);
                                obsForm.FormNumber = x.obs_cft_nbr;
                                obsForm.FormVersion = x.obs_cft_ver;
                                obsForm.Customer = x.DSC_CUSTOMER.dsc_cust_name;
                                obsForm.LC = x.DSC_LC.dsc_lc_name;
                                obsForm.OBS_Type = x.OBS_TYPE.obs_type_name;
                                obsForm.AssignedFunctions = obsForm.getAssignedFunctions(x.obs_cft_id);
                                obsForm.isPublished = isPublishedForm(x.obs_cft_pub_dtm);
                                obsForm.isActive = IsActiveForm(x.obs_cft_eff_st_dt, x.obs_cft_eff_end_dt, obsForm.isPublished);
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
                                obsForm.isPublished = isPublishedForm(x.obs_cft_pub_dtm);
                                obsForm.isActive = IsActiveForm(x.obs_cft_eff_st_dt, x.obs_cft_eff_end_dt, obsForm.isPublished);
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
                                obsForm.isPublished = isPublishedForm(x.obs_cft_pub_dtm);
                                obsForm.isActive = IsActiveForm(x.obs_cft_eff_st_dt, x.obs_cft_eff_end_dt, obsForm.isPublished);
                                obsForm.ObservationCount = obsForm.getTimesCompletedCount(x.obs_cft_id);
                                obsForm.LastCompleteDate = obsForm.getLastCompleteDate(x.obs_cft_id);
                                obsForm.FormSubtitle = x.obs_cft_subtitle;
                                ObsColFormTemplateList.Add(obsForm);//title search match

                            }

                        }

                    }//end of foreach

                }
                fullFuncList.Single(x => x.Value == Request.QueryString["fullFuncList"]).Selected = true;
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
                        obsForm.isPublished = isPublishedForm(x.obs_cft_pub_dtm);
                        obsForm.isActive = IsActiveForm(x.obs_cft_eff_st_dt, x.obs_cft_eff_end_dt, obsForm.isPublished);
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
                            obsForm.isPublished = isPublishedForm(x.obs_cft_pub_dtm);
                            obsForm.isActive = IsActiveForm(x.obs_cft_eff_st_dt, x.obs_cft_eff_end_dt, obsForm.isPublished);
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
                            obsForm.isPublished = isPublishedForm(x.obs_cft_pub_dtm);
                            obsForm.isActive = IsActiveForm(x.obs_cft_eff_st_dt, x.obs_cft_eff_end_dt, obsForm.isPublished);
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
                            obsForm.isPublished = isPublishedForm(x.obs_cft_pub_dtm);
                            obsForm.isActive = IsActiveForm(x.obs_cft_eff_st_dt, x.obs_cft_eff_end_dt, obsForm.isPublished);
                            obsForm.QuestionCount = obsForm.getAssignedQuestionCount(x.obs_cft_id);
                            obsForm.ObservationCount = obsForm.getTimesCompletedCount(x.obs_cft_id);
                            obsForm.LastCompleteDate = obsForm.getLastCompleteDate(x.obs_cft_id);
                            obsForm.FormSubtitle = x.obs_cft_subtitle;
                            ObsColFormTemplateList.Add(obsForm);//title search match
                        }

                    }//end of foreach

                }

            }           
            if (onlyLive== "on")
            {
                ObsColFormTemplateList.RemoveAll(x => x.isActive != true);
            }
            else
            {
                if (onlyPublished == "on")
                {
                    ObsColFormTemplateList.RemoveAll(x => x.isPublished != true);
                }
            }
            ViewBag.fullFuncList = fullFuncList;
            return View(ObsColFormTemplateList.ToList());
        }

        [HttpGet]
        public ActionResult SectionNameLookup(string term)
        {
            // replace multiple spaces with one
            //Regex regex = new Regex("[ ]{2,}", RegexOptions.None);
            //term = Regex.Replace(term, " {2,}", " ");
            term = Regex.Replace(term, @"\s+", " ", RegexOptions.Multiline);
            
            // ******* PREVENT SECURITY HOLE ***********************************************
            // Remove all special characters to avoid javascript or SQL injection attacks
            // allow only space, any unicode letter and digit, underscore and dash
            term = Regex.Replace(term, "[^a-zA-Z0-9_.]+", "", RegexOptions.None);            
            // *****************************************************************************

            //var data = from s in db.OBS_FORM_SECTION select new { label = s.obs_form_section_name, value = s.obs_form_section_name};
            var data = db.OBS_FORM_SECTION.Where(x => x.obs_form_section_name.Contains(term)).Select(item => item.obs_form_section_name).ToArray();
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        // GET: ColFormTemplate/Details/5
        public ActionResult Details(int id)
        {
            //if (id == null) { return HttpNotFound(); }

            oCollectionForm selectedColForm = new oCollectionForm(id);
            if (selectedColForm == null) { return HttpNotFound(); }

            return View(selectedColForm);
        }


        // GET: ColFormTemplate/AddEdit
        [HttpGet]
        public ActionResult AddEditForm(int? id)
        {
            int cftid = id ?? 0;
            oCollectionForm selectedColForm = new oCollectionForm(cftid);
            if (cftid > 0)
            {
                ViewBag.exception = "You are in EDIT mode!!!";
            }
            else
            {
                selectedColForm.cft_eff_st_dt = DateTime.Now;
                selectedColForm.cft_eff_end_dt = Convert.ToDateTime("12/31/2060");
            }

            ViewBag.dsc_cust_id = new SelectList(db.DSC_CUSTOMER.Where(x => x.dsc_cust_id >= 0), "dsc_cust_id", "dsc_cust_name");
            ViewBag.dsc_lc_id = new SelectList(db.DSC_LC.Where(x => x.dsc_lc_id >= 0), "dsc_lc_id", "dsc_lc_name");
            ViewBag.obs_type_id = new SelectList(db.OBS_TYPE.Where(x => x.obs_type_id >= 0), "obs_type_id", "obs_type_name");
            return View(selectedColForm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEditForm(oCollectionForm colForm, FormCollection formData)
        {
            string data_from_form = formData["formQuestions"];
            string is_published = formData["isPublished"];
            int cft_id = -1;
            bool hasQuestions = !String.IsNullOrEmpty(data_from_form);

            if (hasQuestions)
            {
                cft_id = saveForm(colForm, data_from_form, is_published);
            }

            return RedirectToAction("Details", new { id = cft_id });
            //return RedirectToAction("Index");

        }


        //======================================================================================================================
        // GET: ColFormTemplate/Create
        //[HttpGet]
        //public ActionResult CreateForm(int? id)
        //{
        //    int cftid = id ?? 0;
        //    oCollectionForm selectedColForm = new oCollectionForm(cftid);
        //    if (cftid > 0)
        //    { 
        //       ViewBag.exception = "You are in EDIT mode!!!"; 
        //    }
        //    else
        //    {
        //        selectedColForm.cft_eff_st_dt = DateTime.Now;
        //        selectedColForm.cft_eff_end_dt = Convert.ToDateTime("12/31/2060");
        //    }

        //    ViewBag.dsc_cust_id = new SelectList(db.DSC_CUSTOMER.Where(x=>x.dsc_cust_id>=0), "dsc_cust_id", "dsc_cust_name");
        //    ViewBag.dsc_lc_id = new SelectList(db.DSC_LC.Where(x=>x.dsc_lc_id>=0), "dsc_lc_id", "dsc_lc_name");
        //    ViewBag.obs_type_id = new SelectList(db.OBS_TYPE.Where(x=>x.obs_type_id>=0), "obs_type_id", "obs_type_name");
        //    return View(selectedColForm);
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CreateForm(OBS_COLLECT_FORM_TMPLT oBS_COLLECT_FORM_TMPLT, FormCollection formData)
        //{
        //    string data_from_form = formData["formQuestions"];
        //    string is_published = formData["isPublished"];
        //    int cft_id = -1;
        //    bool hasQuestions = !String.IsNullOrEmpty(data_from_form);
        //    if(hasQuestions)
        //    {
        //        cft_id = saveForm(oBS_COLLECT_FORM_TMPLT, data_from_form, is_published, cft_id);
        //    }

        //    return RedirectToAction("Details", new { id =cft_id});
        //    //return RedirectToAction("Index");

        //}

        // GET: ColFormTemplate/Create
        public ActionResult Create()
        {
            ViewBag.dsc_cust_id = new SelectList(db.DSC_CUSTOMER.Where(x => x.dsc_cust_id >= 0), "dsc_cust_id", "dsc_cust_name");
            ViewBag.dsc_lc_id = new SelectList(db.DSC_LC.Where(x => x.dsc_lc_id >= 0), "dsc_lc_id", "dsc_lc_name");
            ViewBag.obs_type_id = new SelectList(db.OBS_TYPE.Where(x => x.obs_type_id >= 0), "obs_type_id", "obs_type_name");
            return View();
        }



        // POST: ColFormTemplate/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OBS_COLLECT_FORM_TMPLT oBS_COLLECT_FORM_TMPLT, FormCollection formData)
        {
            //if (ModelState.IsValid)
            //{
            //    db.OBS_COLLECT_FORM_TMPLT.Add(oBS_COLLECT_FORM_TMPLT);
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}

            //ViewBag.dsc_cust_id = new SelectList(db.DSC_CUSTOMER, "dsc_cust_id", "dsc_cust_name", oBS_COLLECT_FORM_TMPLT.dsc_cust_id);
            //ViewBag.dsc_lc_id = new SelectList(db.DSC_LC, "dsc_lc_id", "dsc_lc_name", oBS_COLLECT_FORM_TMPLT.dsc_lc_id);
            //ViewBag.obs_type_id = new SelectList(db.OBS_TYPE, "obs_type_id", "obs_type_name", oBS_COLLECT_FORM_TMPLT.obs_type_id);
            //return View(oBS_COLLECT_FORM_TMPLT);
            return RedirectToAction("Index");
        }


        //GET: List of Quesions
        [HttpGet]
        public PartialViewResult getQuestionsList(string full_text_search,  int? page, int? pageSize)
        {
            //full text can be question text and medatada(value and category) 
            //System.Threading.Thread.Sleep(2000);
            List<AvailableQuestions> availableQuestions = new List<AvailableQuestions>();
            List<AvailableQuestions> questions_for_display = new List<AvailableQuestions>();
            if (String.IsNullOrWhiteSpace(full_text_search))
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
            else
            {//search by question text and metadata

                List<AvailableQuestions> temp_questions_list = new List<AvailableQuestions>();
                List<AvailableQuestions> temp_md_list = new List<AvailableQuestions>();

                //first lets search for question text
                List<OBS_QUESTION> list_of_questions = db.OBS_QUESTION.Where(item => item.obs_question_eff_st_dt <= DateTime.Now && item.obs_question_eff_end_dt > DateTime.Now && item.obs_question_full_text.ToLower().Contains(full_text_search.ToLower())).ToList();             
                if (list_of_questions.Count > 0)
                {
                    foreach (OBS_QUESTION q in list_of_questions)
                    {
                        AvailableQuestions quest = new AvailableQuestions();
                        quest.obs_question_id = q.obs_question_id;
                        quest.obs_question_full_text = q.obs_question_full_text;
                        quest.assigned_metadata = quest.getAssignedMetadata(q.obs_question_id);
                        temp_questions_list.Add(quest);

                    }
                }
                //now lets find all matching metadata
                List<OBS_QUESTION> list_of_md_questions = db.OBS_QUESTION.Where(item => item.obs_question_eff_st_dt <= DateTime.Now && item.obs_question_eff_end_dt > DateTime.Now).ToList();

                foreach (OBS_QUESTION q in list_of_md_questions)
                {
                    AvailableQuestions md_quest = new AvailableQuestions();
                    if (md_quest.getAssignedMetadata(q.obs_question_id).Count > 0)
                    {
                        md_quest.assigned_metadata = md_quest.getAssignedMetadata(q.obs_question_id);
                        foreach (string s in md_quest.assigned_metadata)
                        {
                            if (s.ToLower().Contains(full_text_search.ToLower())&&(temp_questions_list.Where(x=>x.obs_question_id==q.obs_question_id).Count()==0))
                            {
                                md_quest.obs_question_id = q.obs_question_id;
                                md_quest.obs_question_full_text = q.obs_question_full_text;
                                temp_md_list.Add(md_quest);
                                break;
                            }
                            else { continue; }

                        }
                    }
                }
                availableQuestions = temp_questions_list.Union(temp_md_list).ToList();
                
            }//end of else

            

            questions_for_display = availableQuestions.OrderBy(x => x.obs_question_id).Skip(((page ?? 1) - 1) * (pageSize ?? 10)).Take(pageSize ?? 10).ToList();

            return PartialView("_getQuestionsList", questions_for_display);
        }

        [HttpGet]
        public PartialViewResult getQuestionInfoTest(int question_id, int qCounter)
        {
            //return "You selected Question Id: " + question_id + "[Counter: " + qCounter + "] via Ajax call.";
            return PartialView("_getQuestionInfoTest");
        }

        [HttpGet]
        public PartialViewResult getQuestionInfo(int question_id, int qCounter)
        {
            QuestionInfo questionInfo = new QuestionInfo();
            questionInfo.question_id = question_id;
            questionInfo.uniqueCounter = qCounter;
            questionInfo.full_text = db.OBS_QUESTION.Single(item => item.obs_question_id == question_id).obs_question_full_text;
            List<OBS_QUEST_ANS_TYPES> QAInstances = db.OBS_QUEST_ANS_TYPES.Where(x => x.obs_question_id == question_id && (x.obs_qat_end_eff_dt == null || x.obs_qat_end_eff_dt > DateTime.Now)).ToList();

            if (QAInstances.Count() == 0)  //There were no records found in the 'OBS_QUEST_ANS_TYPES' Table for this question Id
            {
                questionInfo.hasInstances = false;
            }
            else
            {//there's a record(s) in 'OBS_QUEST_ANS_TYPES'. now we need to loop through all of them, add them to the list and find default answer type

                questionInfo.hasInstances = true;
                foreach (OBS_QUEST_ANS_TYPES qaInstanceTemp in QAInstances)
                {
                    questionInfo.obs_question_answer_types.Add(qaInstanceTemp);
                    if (qaInstanceTemp.obs_qat_default_ans_type_yn == "Y")
                    {   //if we're here, that means we found default default answer type
                        questionInfo.default_qat_id = qaInstanceTemp.obs_qat_id;//we set our object's qat id to the default qat id                                              
                    }
                    //now we need to find the corresponding answer type and assign it to the object
                    OBS_ANS_TYPE temp_answer = db.OBS_ANS_TYPE.Single(item => item.obs_ans_type_id == qaInstanceTemp.obs_ans_type_id);
                    questionInfo.assigned_answer_types.Add(temp_answer);
                    SelectListItem answer_for_dropdown = new SelectListItem() { Text = temp_answer.obs_ans_type_name, Value = qaInstanceTemp.obs_qat_id.ToString() };
                    questionInfo.question_assigned_answer_types.Add(answer_for_dropdown);
                    // lets check if this answer type requires selectable answers
                    if (db.OBS_ANS_TYPE.Single(item => item.obs_ans_type_id == qaInstanceTemp.obs_ans_type_id).obs_ans_type_has_fxd_ans_yn == "Y")
                    {
                        //if true, we need to list all of them and assign them to object's list of selectable answers
                      
                        int count =QAInstances.Count(x => x.obs_ans_type_id == qaInstanceTemp.obs_ans_type_id);
                        if (count > 1)
                        {
                              String sel_ans ="(";
                            foreach (OBS_QUEST_SLCT_ANS temp in db.OBS_QUEST_SLCT_ANS.Where(item => item.obs_qat_id == qaInstanceTemp.obs_qat_id && item.obs_qsa_eff_st_dt <= DateTime.Now && item.obs_qsa_eff_end_dt > DateTime.Now))
                            {
                                questionInfo.selectable_answers.Add(temp);
                                sel_ans = sel_ans == "(" ? sel_ans + temp.obs_qsa_text : sel_ans + "," + temp.obs_qsa_text;
                            }
                            sel_ans = sel_ans + ")";
                            questionInfo.question_assigned_answer_types.Single(x => Convert.ToInt32(x.Value) == qaInstanceTemp.obs_qat_id).Text = temp_answer.obs_ans_type_name + sel_ans;                            
                        }
                        else
                        {
                            foreach (OBS_QUEST_SLCT_ANS temp in db.OBS_QUEST_SLCT_ANS.Where(item => item.obs_qat_id == qaInstanceTemp.obs_qat_id && item.obs_qsa_eff_st_dt <= DateTime.Now && item.obs_qsa_eff_end_dt > DateTime.Now))
                            {
                                questionInfo.selectable_answers.Add(temp);
                            }
                        }                                                                        
                    }

                }
                questionInfo.question_assigned_answer_types =questionInfo.question_assigned_answer_types.OrderBy(item=>item.Text).ToList();
                questionInfo.question_assigned_answer_types.Add(new SelectListItem() { Text = "Add New...", Value = "New" });
            }
            if (questionInfo.default_qat_id > 0)
            {
                questionInfo.question_assigned_answer_types.Single(x => x.Value == questionInfo.default_qat_id.ToString()).Selected = true;
            }
            return PartialView("_getQuestionInfo", questionInfo);
        }


        // =====================================================================================================================
        public String GetSelectableAnswers(string qat_id)
        {
            if (qat_id == "")
            {
                return "";
            }
            else
            {
                int obs_qat_id = Convert.ToInt32(qat_id);
                var list = db.OBS_QUEST_SLCT_ANS.Where(item => item.obs_qat_id == obs_qat_id && item.obs_qsa_eff_st_dt <= DateTime.Now && item.obs_qsa_eff_end_dt > DateTime.Now);
                if (list.Count() > 0)
                {
                    string sel_ans = "";
                    foreach (var x in list)
                    {
                        sel_ans = sel_ans + "<div style=\"display: inline\"><span class=\"badge\">" + x.obs_qsa_text + "</span></div>&nbsp;";
                    }

                    return sel_ans;
                }
                else { return ""; }
            }
        }

        public PartialViewResult addNewSection(string sCounter, CollectionFormSection colFormSection)
        {
            ViewData["sNumber"] = sCounter;
            return PartialView("_addNewSection", colFormSection);
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
            [Display(Name = "Live")]
            public bool isActive { get; set; }
            [Display(Name = "Published")]
            public bool isPublished { get; set; } 
            [Display(Name = "Last Submitted on")]
            [DisplayFormat(DataFormatString = "{0:g}")]
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
                return OBSdb.OBS_COLLECT_FORM_INST.Where(item => item.obs_cft_id == cft_id).Max(x => x.obs_cfi_comp_date).Equals(null) ? null : OBSdb.OBS_COLLECT_FORM_INST.Where(item => item.obs_cft_id == cft_id).Max(x => x.obs_cfi_comp_date);

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
        public static bool IsActiveForm(DateTime? start_date, DateTime end_date, bool isPublished)
        {   // Start Date can be nullable  
            if ((start_date !=null && DateTime.Now >= start_date) && DateTime.Now < end_date && isPublished) { return true; }
            else { return false; }
        }
        public static bool isPublishedForm(DateTime? publishedDate)
        {
            if (publishedDate != null && publishedDate <DateTime.Now) { return true; }   
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
        public List<SelectListItem> setfullFuncList()
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

        private int saveForm(oCollectionForm colForm, string form_questions_from_gui, string isPublished)
        {
            int cft_id = colForm.cft_id;
            if (cft_id <= 0 && db.OBS_COLLECT_FORM_TMPLT.Where(item => item.obs_cft_title == colForm.cft_Title).Count() > 0)
            {//we need to check if title passed from user is unique. if it already exists, we need to return the error message back to the screen
                ViewBag.exception = "ERROR: The Question Id is either invalid or the Form Title Already Exist.";
                return -1;
            }

            ViewBag.exception = "";
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (colForm.cft_id > 0)//this means we are editing an existing form
                    {
                        OBS_COLLECT_FORM_TMPLT template_to_edit = db.OBS_COLLECT_FORM_TMPLT.Find(cft_id);
                        template_to_edit.dsc_cust_id = Convert.ToInt32(colForm.cft_Cust);
                        template_to_edit.obs_type_id = Convert.ToInt32(colForm.cft_obsType);
                        template_to_edit.dsc_lc_id = Convert.ToInt32(colForm.cft_LC);
                        template_to_edit.obs_cft_title = colForm.cft_Title;
                        template_to_edit.obs_cft_subtitle = colForm.cft_SubTitle;
                        template_to_edit.obs_cft_eff_end_dt = (colForm.cft_eff_end_dt == null) || (colForm.cft_eff_end_dt < Convert.ToDateTime("01/01/2000")) ? Convert.ToDateTime("12/31/2060") : colForm.cft_eff_end_dt;
                        template_to_edit.obs_cft_last_saved_dtm = DateTime.Now;
                        template_to_edit.obs_cft_upd_dtm = DateTime.Now;
                        template_to_edit.obs_cft_upd_uid = User.Identity.Name;
                        List<OBS_COL_FORM_QUESTIONS> old_form_questions = db.OBS_COL_FORM_QUESTIONS.Where(x => x.obs_cft_id == cft_id).ToList();
                        db.OBS_COL_FORM_QUESTIONS.RemoveRange(old_form_questions);
                        //foreach (OBS_COL_FORM_QUESTIONS old_form_question in old_form_questions)
                        //{
                        //    db.OBS_COL_FORM_QUESTIONS.Remove(old_form_question)
                        //}
                        db.SaveChanges();
                    }
                    else//this means we're saving new form
                    {
                        //first we need to save OBS_COLLECT_FORM_TMPLT table data
                        OBS_COLLECT_FORM_TMPLT template_to_save = new OBS_COLLECT_FORM_TMPLT();
                        template_to_save.dsc_cust_id = Convert.ToInt32(colForm.cft_Cust);
                        template_to_save.obs_type_id = Convert.ToInt32(colForm.cft_obsType);
                        template_to_save.dsc_lc_id = Convert.ToInt32(colForm.cft_LC);
                        short cft_number = (short)(db.OBS_COLLECT_FORM_TMPLT.Max(x => x.obs_cft_nbr) + 1);
                        template_to_save.obs_cft_nbr = cft_number;
                        template_to_save.obs_cft_ver = 1;
                        template_to_save.obs_cft_title = colForm.cft_Title;
                        template_to_save.obs_cft_subtitle = colForm.cft_SubTitle;
                        template_to_save.obs_cft_eff_end_dt = (colForm.cft_eff_end_dt == null) || (colForm.cft_eff_end_dt < Convert.ToDateTime("01/01/2000")) ? Convert.ToDateTime("12/31/2060") : colForm.cft_eff_end_dt;
                        template_to_save.obs_cft_added_dtm = DateTime.Now;
                        template_to_save.obs_cft_added_uid = User.Identity.Name;
                        template_to_save.obs_cft_last_saved_dtm = DateTime.Now;
                        template_to_save.obs_cft_upd_dtm = DateTime.Now;
                        template_to_save.obs_cft_upd_uid = User.Identity.Name;
                        if (isPublished == "true")
                        {                          
                            if ( colForm.cft_eff_st_dt != null || (colForm.cft_eff_st_dt < Convert.ToDateTime("01/01/2000")))
                            {
                                template_to_save.obs_cft_eff_st_dt = colForm.cft_eff_st_dt;

                            }
                            else
                            {
                                return -1;
                            }
                        }
                        else if (!(colForm.cft_eff_st_dt != null || (colForm.cft_eff_st_dt < Convert.ToDateTime("01/01/2000"))))
                        {
                            template_to_save.obs_cft_eff_st_dt = colForm.cft_eff_st_dt;
                        }
                        db.OBS_COLLECT_FORM_TMPLT.Add(template_to_save);
                        db.SaveChanges();
                        cft_id = template_to_save.obs_cft_id;
                        //now we need to query OBS_COLLECT_FORM_TMPLT table to find CFT ID we just created
                        //int cft_id = db.OBS_COLLECT_FORM_TMPLT.Single(item => item.obs_cft_nbr == cft_number && item.obs_cft_ver == 1).obs_cft_id;                       
                    }//end of else(saving new form header form)

                    //now we need to save all the form questions
                    string[] splitterm = { "," };
                    string[] parsed_questions = form_questions_from_gui.Split(splitterm, StringSplitOptions.RemoveEmptyEntries);
                    short order_counter = 1;
                    foreach (string question in parsed_questions)
                    {
                        //now lets first save split the string we received from the gui
                        // string format should be: order,qat_id,section_text
                        string[] question_items = question.Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                        short order = order_counter;
                        int qat_id = Convert.ToInt32(question_items[0]);
                        int form_section_id = getSectionID(question_items[1]);
                        OBS_COL_FORM_QUESTIONS new_form_question = new OBS_COL_FORM_QUESTIONS();
                        new_form_question.obs_cft_id = cft_id;

                        new_form_question.obs_form_section_id = form_section_id;
                        new_form_question.obs_qat_id = qat_id;
                        new_form_question.obs_col_form_quest_order = order;
                        new_form_question.obs_col_form_quest_wgt = 1;
                        new_form_question.obs_col_form_quest_na_yn = "Y";
                        db.OBS_COL_FORM_QUESTIONS.Add(new_form_question);
                        db.SaveChanges();
                        order_counter++;
                    }//end of foreach

                    transaction.Commit();
                    return cft_id;
                }//end of try
                catch (Exception e)
                {
                    string notUsed = e.Message;
                    transaction.Rollback();
                    return -1;
                }
            }//end of  using (var transaction = db.Database.BeginTransaction())

        }

 //       private int saveForm(OBS_COLLECT_FORM_TMPLT template_from_form, string form_questions_from_gui, string isPublished, int cft_id_from_form)
 //       {
 //           int cft_id = cft_id_from_form;
 //           if (cft_id<0 && db.OBS_COLLECT_FORM_TMPLT.Where(item =>item.obs_cft_title == template_from_form.obs_cft_title).Count() > 0)
 //           {//we need to check if title passed from user is unique. if it already exists, we need to return the error message back to the screen
 //               return -1;
 //           }

 //           ViewBag.exception = "";
 //           using (var transaction = db.Database.BeginTransaction())
 //           {
 //               try
 //               {
 //                   if(cft_id_from_form >0)//this means we're editing existing form
 //                   {
 //                       OBS_COLLECT_FORM_TMPLT template_to_edit = db.OBS_COLLECT_FORM_TMPLT.Find(cft_id);
 //                       template_to_edit.dsc_cust_id = template_from_form.dsc_cust_id;
 //                       template_to_edit.obs_type_id = template_from_form.obs_type_id;
 //                       template_to_edit.dsc_lc_id = template_from_form.dsc_lc_id;                                                                  
 //                       template_to_edit.obs_cft_title = template_from_form.obs_cft_title;
 //                       template_to_edit.obs_cft_subtitle = template_from_form.obs_cft_subtitle;
 //                       template_to_edit.obs_cft_eff_end_dt = (template_from_form.obs_cft_eff_end_dt == null) || (template_from_form.obs_cft_eff_end_dt < Convert.ToDateTime("01/01/2000")) ? Convert.ToDateTime("12/31/2060") : template_from_form.obs_cft_eff_end_dt;                       
 //                       template_to_edit.obs_cft_last_saved_dtm = DateTime.Now;
 //                       template_to_edit.obs_cft_upd_dtm = DateTime.Now;
 //                       template_to_edit.obs_cft_upd_uid = User.Identity.Name;
 //                       List<OBS_COL_FORM_QUESTIONS> old_form_questions = db.OBS_COL_FORM_QUESTIONS.Where(x => x.obs_cft_id == cft_id).ToList();
 //                       db.OBS_COL_FORM_QUESTIONS.RemoveRange(old_form_questions);
 //                       //foreach (OBS_COL_FORM_QUESTIONS old_form_question in old_form_questions)
 //                       //{
 //                       //    db.OBS_COL_FORM_QUESTIONS.Remove(old_form_question)
 //                       //}
 //                       db.SaveChanges();
 //                   }
 //                   else//this means we're saving new form
 //                   {
 //                   //first we need to save OBS_COLLECT_FORM_TMPLT table data
 //                   OBS_COLLECT_FORM_TMPLT template_to_save = new OBS_COLLECT_FORM_TMPLT();
 //                   template_to_save.dsc_cust_id = template_from_form.dsc_cust_id;
 //                   template_to_save.obs_type_id = template_from_form.obs_type_id;
 //                   template_to_save.dsc_lc_id = template_from_form.dsc_lc_id;
 //                   short cft_number = (short)(db.OBS_COLLECT_FORM_TMPLT.Max(x => x.obs_cft_nbr) + 1);
 //                   template_to_save.obs_cft_nbr = cft_number;
 //                   template_to_save.obs_cft_ver = 1;
 //                   template_to_save.obs_cft_title = template_from_form.obs_cft_title;
 //                   template_to_save.obs_cft_subtitle = template_from_form.obs_cft_subtitle;
 //                   template_to_save.obs_cft_eff_end_dt = (template_from_form.obs_cft_eff_end_dt == null) || (template_from_form.obs_cft_eff_end_dt < Convert.ToDateTime("01/01/2000")) ? Convert.ToDateTime("12/31/2060") : template_from_form.obs_cft_eff_end_dt;
 //                   template_to_save.obs_cft_added_dtm = DateTime.Now;
 //                   template_to_save.obs_cft_added_uid = User.Identity.Name;
 //                   template_to_save.obs_cft_last_saved_dtm = DateTime.Now;
 //                   if (isPublished == "true")
 //                   {
 //                       template_to_save.obs_cft_pub_by_uid = User.Identity.Name;
 //                       template_to_save.obs_cft_pub_dtm = DateTime.Now;
 //                       if (template_from_form.obs_cft_eff_end_dt != null)
 //                       {
 //                           template_to_save.obs_cft_eff_st_dt = template_from_form.obs_cft_eff_st_dt;

 //                       }
 //                       else
 //                       {
 //                        return -1;
 //}
 //                   }
 //                   else
 //                   {
 //                       template_to_save.obs_cft_eff_st_dt = template_from_form.obs_cft_eff_st_dt;
 //                   }                
 //                   db.OBS_COLLECT_FORM_TMPLT.Add(template_to_save);
 //                   db.SaveChanges();
 //                       cft_id = template_to_save.obs_cft_id;
 //                       //now we need to query OBS_COLLECT_FORM_TMPLT table to find CFT ID we just created
 //                       //int cft_id = db.OBS_COLLECT_FORM_TMPLT.Single(item => item.obs_cft_nbr == cft_number && item.obs_cft_ver == 1).obs_cft_id;                       
 //                   }//end of else(saving new form header form)

 //                   //now we need to save all the form questions
 //                   string[] splitterm = { "," };
 //                   string[] parsed_questions = form_questions_from_gui.Split(splitterm, StringSplitOptions.RemoveEmptyEntries);
 //                   short order_counter = 1;
 //                   foreach (string question in parsed_questions)
 //                   {
 //                       //now lets first save split the string we received from the gui
 //                       // string format should be: order,qat_id,section_text
 //                       string[] question_items = question.Split(new string[] {"~"}, StringSplitOptions.RemoveEmptyEntries);
 //                       short order = order_counter;
 //                       int qat_id = Convert.ToInt32(question_items[0]);
 //                       int form_section_id = getSectionID(question_items[1]);
 //                       OBS_COL_FORM_QUESTIONS new_form_question = new OBS_COL_FORM_QUESTIONS();
 //                       new_form_question.obs_cft_id = cft_id;
                        
 //                       new_form_question.obs_form_section_id = form_section_id;
 //                       new_form_question.obs_qat_id = qat_id;
 //                       new_form_question.obs_col_form_quest_order = order;
 //                       new_form_question.obs_col_form_quest_wgt = 1;
 //                       new_form_question.obs_col_form_quest_na_yn = "Y";
 //                       db.OBS_COL_FORM_QUESTIONS.Add(new_form_question);
 //                       db.SaveChanges();
 //                       order_counter++;
 //                   }//end of foreach

 //                   transaction.Commit();
 //                   return cft_id;
 //               }//end of try
 //               catch (Exception e)
 //               {
 //                   transaction.Rollback();
 //                   return -1;
 //               }
 //           }//end of  using (var transaction = db.Database.BeginTransaction())

 //       }

        private int getSectionID(string section_name)
        {
            if (db.OBS_FORM_SECTION.Where(item => item.obs_form_section_name == section_name).Count() > 0)
            {
                return db.OBS_FORM_SECTION.Single(item => item.obs_form_section_name == section_name).obs_form_section_id;
            }
            else
            {
                OBS_FORM_SECTION section = new OBS_FORM_SECTION();
                section.obs_form_section_name = section_name;
                db.OBS_FORM_SECTION.Add(section);
                db.SaveChanges();
                return db.OBS_FORM_SECTION.Single(item => item.obs_form_section_name == section_name).obs_form_section_id;
            }
        }


    }
    //\==================== END OF CONTROLLERS CLASS ==================================================/


    // =================================================================================================
    // ============================ HELPER CLASES FOR CONTROLERS  ======================================
    //******* CLASES ***********************************************************************************
    public class oCollectionForm
    {
        private DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY();  //To get Database access inside this Class

        //--- CONSTRUCTOR------------------
        public oCollectionForm() : this(0) { }
        public oCollectionForm(int id)
        {//Create the Collection Form Data (Header Info) from the Id passed as a parameter

            cft_id = id;
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
                         cft_Status = ((A.obs_cft_eff_st_dt < DateTime.Now) && (A.obs_cft_eff_end_dt > DateTime.Now) && (A.obs_cft_pub_dtm != null && A.obs_cft_pub_dtm < DateTime.Now)) ? "LIVE" : "NOT LIVE",
                         cft_isPublished = (A.obs_cft_pub_dtm != null && A.obs_cft_pub_dtm < DateTime.Now) ? "PUBLISHED" : "NOT PUBLISHED",
                         cft_eff_st_dt = A.obs_cft_eff_st_dt,
                         cft_eff_end_dt = A.obs_cft_eff_end_dt
                     }).ToList().FirstOrDefault();
            // Set the properties from query result
            if (q != null)
            {
            cft_Title = q.cft_Title;
            cft_SubTitle = q.cft_SubTitle;
            cft_obsType = q.cft_obsType;
            cft_Cust = q.cft_Cust;
            cft_LC = q.cft_LC;
            cft_Status = q.cft_Status;
            cft_isPublished = q.cft_isPublished;
            cft_Nbr = q.cft_Nbr;
            cft_Version = q.cft_Version;
            colFormSections = new List<CollectionFormSection>();
            retrieveQuestionData();
        }
            else { 
            // Form Id not fouond in the database, leave all values empty
                cft_Title = "";
                cft_SubTitle = "";
                //cft_obsType = q.cft_obsType;
                //cft_Cust = q.cft_Cust;
                //cft_LC = q.cft_LC;
                //cft_Status = q.cft_Status;
                //cft_isPublished = q.cft_isPublished;
                cft_Nbr = 0;
                cft_Version = 0;
                colFormSections = new List<CollectionFormSection>();
            }
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

        public string cft_isPublished { get; set; }
        public int cft_Nbr { get; set; }
        public int cft_Version { get; set; }
        public DateTime cft_eff_st_dt { get; set; }
        public DateTime cft_eff_end_dt { get; set; }
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
                    oQuestion.cfq_fullText = q.OBS_QUEST_ANS_TYPES.OBS_QUESTION.obs_question_full_text.Replace(": (", ":<br/>(");
                    oQuestion.cfq_AT = q.OBS_QUEST_ANS_TYPES.OBS_ANS_TYPE.obs_ans_type_name;
                    oQuestion.cfq_qatId = q.obs_qat_id;
                    oQuestion.cfq_SelectableAnswers = q.OBS_QUEST_ANS_TYPES.OBS_QUEST_SLCT_ANS.Where(item => item.obs_qsa_eff_st_dt <= DateTime.Now && item.obs_qsa_eff_end_dt > DateTime.Now).OrderBy(xx => xx.obs_qsa_order).Select(x => x.obs_qsa_text).ToList();
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
        [Display(Name = "Assigned Meta Data")]
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
    public class QuestionInfo
    {
        public int question_id { set; get; }
        public string full_text { set; get; }
        public bool hasInstances { get; set; }
        public int default_qat_id = -1;

        public List<OBS_ANS_TYPE> assigned_answer_types = new List<OBS_ANS_TYPE>();
        public List<OBS_QUEST_SLCT_ANS> selectable_answers = new List<OBS_QUEST_SLCT_ANS>();
        public List<OBS_QUEST_ANS_TYPES> obs_question_answer_types = new List<OBS_QUEST_ANS_TYPES>();
        public List<SelectListItem> question_assigned_answer_types = new List<SelectListItem>();
        public int uniqueCounter { set; get; }

    }

}
