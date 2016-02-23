using OBSMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace OBSMVC.Controllers
{
    public class TestQuestionAnswerTypeDefaultController : Controller
    {
        private DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY();
        // GET: QuestionAnswerTypeDefault
        public ActionResult _TestlistAnswerTypes(int question_id)
        {
            var all_answer_types = db.OBS_ANS_TYPE.Where(item =>item.obs_ans_type_id>0).ToList();
            var default_answer_type = (from qat in db.OBS_QUEST_ANS_TYPES.Where(x => x.obs_question_id == question_id && x.obs_qat_default_ans_type_yn == "Y")
                                       select new { qat.obs_ans_type_id,qat.obs_qat_id }).ToList();
            int default_answer_id = -1;
            int default_answer_qat_id = -1;
            List<OBS_QUEST_SLCT_ANS> question_selected_ans_type = new List<OBS_QUEST_SLCT_ANS>();

            try
            {
                default_answer_id = default_answer_type.FirstOrDefault().obs_ans_type_id;
                default_answer_qat_id = default_answer_type.FirstOrDefault().obs_qat_id;
            }
            catch(NullReferenceException)
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
                    if(x.obs_ans_type_has_fxd_ans_yn=="Y")
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
            ViewBag.question_selected_ans_type= question_selected_ans_type;            
            return View(list_of_answers);
        }

        // POST: Default Answer Type
        [HttpPost]
        public ActionResult _TestlistAnswerTypes(FormCollection formData)
        {
           
            string isSave = formData["save"];           

            string passed_selected_ans_types = formData["question_selected_ans_type"];
            if (isSave == "false")
            {
                List<SelectListItem> list_of_answers = new List<SelectListItem>();
                List<OBS_QUEST_SLCT_ANS> question_selected_ans_type = new List<OBS_QUEST_SLCT_ANS>();
                var all_answer_types = db.OBS_ANS_TYPE.Where(item => item.obs_ans_type_id > 0).ToList();
                string answer_type_id = formData["list_of_answers"];
                string question_id = formData["question_id"];
                if (String.IsNullOrEmpty(answer_type_id))
                {
                    foreach (var x in all_answer_types)
                    {
                        SelectListItem answer_type = new SelectListItem();
                        answer_type.Value = x.obs_ans_type_id.ToString();
                        answer_type.Text = x.obs_ans_type_name;
                        answer_type.Selected = false;
                        list_of_answers.Add(answer_type);
                    }
                    ViewBag.list_of_answers = list_of_answers;
                    ViewBag.question_selected_ans_type = question_selected_ans_type;
                    return View(list_of_answers);
                }
                else
                {
                    int selected_ans_type_id = Convert.ToInt32(answer_type_id);
                    int quest_id = Convert.ToInt32(question_id);
                    int selected_obs_qat_id = -1;
                    var obs_qat_id = (from qat in db.OBS_QUEST_ANS_TYPES.Where(x => x.obs_question_id == quest_id && x.obs_ans_type_id == selected_ans_type_id)
                                      select new { qat.obs_qat_id }).ToList();
                    try
                    {
                        selected_obs_qat_id = obs_qat_id.FirstOrDefault().obs_qat_id;

                    }
                    catch (NullReferenceException)
                    { }
                    foreach (var x in all_answer_types)
                    {
                        SelectListItem answer_type = new SelectListItem();
                        answer_type.Value = x.obs_ans_type_id.ToString();
                        answer_type.Text = x.obs_ans_type_name;
                        if (x.obs_ans_type_id == selected_ans_type_id)
                        {
                            answer_type.Selected = true;
                            if (x.obs_ans_type_has_fxd_ans_yn == "Y" && selected_obs_qat_id != -1)
                            {
                                question_selected_ans_type = db.OBS_QUEST_SLCT_ANS.Where(item => item.obs_qat_id == selected_obs_qat_id).ToList();
                                ViewBag.question_selected_ans_type = question_selected_ans_type;
                            }
                            else if (x.obs_ans_type_has_fxd_ans_yn == "Y")
                            {
                                List<string> q_selected_ans_type = new List<string>();
                                switch (x.obs_ans_type_category)
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

                                //QuestionHelpers.getDefault_Selected_Ans_list(x.obs_ans_type_category);
                                ViewBag.q_selected_ans_type = q_selected_ans_type;
                            }//end of else if(x.obs_ans_type_has_fxd_ans_yn == "Y")

                        }//end of if (x.obs_ans_type_id == selected_ans_type_id)
                        else
                        {
                            answer_type.Selected = false;
                        }
                        list_of_answers.Add(answer_type);
                    }//end of foreach

                    ViewBag.list_of_answers = list_of_answers;
                    return View(list_of_answers);
                }
            } //end of   if (isSave == "false")
            else
            {//HERE, LETS SAVE THE DB CHANGES
                SaveDefaultAnswerType(formData);
                return RedirectToAction("_TestlistAnswerTypes");
            }        
           // ViewBag.list_of_answers = list_of_answers;
           // return RedirectToAction("_TestlistAnswerTypes(59)");
        }

      
        public void SaveDefaultAnswerType(FormCollection formData)
        {
            string answer_type_id = formData["list_of_answers"];
            int question_id = 59;         
            //string default_sel_ans_types = formData["default_selected_ans_types"];
            //first lets check if user submitted None as a default answer type 
            if (String.IsNullOrEmpty(answer_type_id))
            {
              
                OBS_QUEST_ANS_TYPES oBS_QUEST_ANS_TYPES = new OBS_QUEST_ANS_TYPES();
                try {
                    oBS_QUEST_ANS_TYPES = db.OBS_QUEST_ANS_TYPES.Single(item => item.obs_question_id == question_id && item.obs_qat_default_ans_type_yn == "Y");
                    oBS_QUEST_ANS_TYPES.obs_qat_default_ans_type_yn = "N";
                    db.SaveChanges();
                }
                catch { }
            }//end  if (String.IsNullOrEmpty(answer_type_id))
            else //if we're here, that means user submtted  answer type that <>None
            {
                int selected_ans_type_id = Convert.ToInt32(formData["list_of_answers"]);
                //now we need to check if this question/answer type combination already exist in obs_quest_ans_type table
                if (isNew_Quest_Ans_Type(selected_ans_type_id, question_id))
                {
                    //if we're here, that means we need to insert a new record in OBS_QUEST_ANS_TYPE table
                    //first we need to check if this selected answer type requires a record in OBS_QUEST_SLCT_ANS
                    if (isQuest_Slct_Ans_Required(selected_ans_type_id)) {
                        //so we are here, that means we need to save both OBS_QUEST_ANS_TYPE and OBS_QUEST_SLCT_ANS

                       using (var transaction = db.Database.BeginTransaction())
                        {//need to create a transaction variable to rollback the changes for both tables if something goes wrong
                            try {
                                setExistingDefaultToN(question_id);
                                string default_sel_ans_types = formData["default_selected_ans_types"];                              
                                string[] splitterm = { "," };
                                string[] selected_new_sel_ans_types = default_sel_ans_types.Split(splitterm, StringSplitOptions.RemoveEmptyEntries);                                                               
                                OBS_QUEST_ANS_TYPES oBS_QUEST_ANS_TYPES = new OBS_QUEST_ANS_TYPES();
                                oBS_QUEST_ANS_TYPES.obs_question_id = question_id;
                                oBS_QUEST_ANS_TYPES.obs_ans_type_id = (short)selected_ans_type_id;
                                oBS_QUEST_ANS_TYPES.obs_qat_default_ans_type_yn = "Y";
                                oBS_QUEST_ANS_TYPES.obs_qat_end_eff_dt = Convert.ToDateTime("12/31/2060");
                                db.OBS_QUEST_ANS_TYPES.Add(oBS_QUEST_ANS_TYPES);
                                db.SaveChanges();
                                short temp_selected_ans_type_id = (short)selected_ans_type_id;
                                int qat_id = db.OBS_QUEST_ANS_TYPES.SingleOrDefault(item => item.obs_ans_type_id == temp_selected_ans_type_id && item.obs_question_id == question_id && item.obs_qat_end_eff_dt > DateTime.Today).obs_qat_id;                               
                                short order = 1;
                                foreach (string str in selected_new_sel_ans_types)
                                {
                                    OBS_QUEST_SLCT_ANS oBS_QUEST_SLCT_ANS = new OBS_QUEST_SLCT_ANS();
                                    oBS_QUEST_SLCT_ANS.obs_qat_id = qat_id;
                                    oBS_QUEST_SLCT_ANS.obs_qsa_text = str;
                                    oBS_QUEST_SLCT_ANS.obs_qsa_order = order;
                                    oBS_QUEST_SLCT_ANS.obs_qsa_order = order;
                                    oBS_QUEST_SLCT_ANS.obs_qsa_dflt_yn = "N";
                                    oBS_QUEST_SLCT_ANS.obs_qsa_eff_st_dt = DateTime.Today;
                                    oBS_QUEST_SLCT_ANS.obs_qsa_eff_end_dt = Convert.ToDateTime("12/31/2060");
                                    db.OBS_QUEST_SLCT_ANS.Add(oBS_QUEST_SLCT_ANS);                                   
                                    order++;
                                }
                               db.SaveChanges();
                              }
                            catch(Exception e)
                            {
                                string error = e.Message;
                                transaction.Rollback();
                                ViewBag.ResultMessage = "Error occured, records rolledback.";
                            }
                           
                        }//end of using (var transaction = db.Database.BeginTransaction())

                    }//end of if (isQuest_Slct_Ans_Required(selected_ans_type_id))
                    else {
                        setExistingDefaultToN(question_id);
                        OBS_QUEST_ANS_TYPES oBS_QUEST_ANS_TYPES_U = new OBS_QUEST_ANS_TYPES();
                        oBS_QUEST_ANS_TYPES_U.obs_question_id = question_id;
                        oBS_QUEST_ANS_TYPES_U.obs_ans_type_id = (short)selected_ans_type_id;
                        oBS_QUEST_ANS_TYPES_U.obs_qat_default_ans_type_yn = "Y";
                        oBS_QUEST_ANS_TYPES_U.obs_qat_end_eff_dt = Convert.ToDateTime("12/31/2060");
                        save(oBS_QUEST_ANS_TYPES_U);
                    }

                }//if (isNew_Quest_Ans_Type(selected_ans_type_id, question_id))
                else // this branch should take care of the scenario where this question/answer type exists in the obs_quest_ans_type table
                {
                    OBS_QUEST_ANS_TYPES oBS_QUEST_ANS_TYPES = new OBS_QUEST_ANS_TYPES();
                    try
                    {    //set default flag the existing default answer type id to N
                        // we need try/catch block in case there's no existing default answer type 
                        oBS_QUEST_ANS_TYPES = db.OBS_QUEST_ANS_TYPES.Single(item => item.obs_question_id == question_id && item.obs_qat_default_ans_type_yn == "Y");
                        oBS_QUEST_ANS_TYPES.obs_qat_default_ans_type_yn = "N";
                        db.SaveChanges();
                    }
                    catch { }
                    //now let's set the selected answer type to be default one 
                    oBS_QUEST_ANS_TYPES = db.OBS_QUEST_ANS_TYPES.Single(item => item.obs_ans_type_id == selected_ans_type_id && item.obs_question_id == question_id);
                    oBS_QUEST_ANS_TYPES.obs_qat_default_ans_type_yn = "Y";
                    db.SaveChanges();
                }               
            }
           
        }
        public bool isQuest_Slct_Ans_Required(int ans_type_id)
        {
            short temp = (short)ans_type_id;
            bool isRequired = (db.OBS_ANS_TYPE.Single(item => item.obs_ans_type_id == temp).obs_ans_type_has_fxd_ans_yn)=="Y" ? true : false;
            return isRequired;
        }
        /*
        *This method checks if there's existing record in OBS_QUEST_ANS_TYPES table for passed answer type id
        *if returns true: This is a new question id/answer type id combination
        *if returns false: The record with this question id/answer type id combination already exists in the table
         */
        public bool isNew_Quest_Ans_Type(int ans_type_id, int question_id)
        {
            short temp = (short)ans_type_id;
           
                int number_of_records = db.OBS_QUEST_ANS_TYPES.Where(item => item.obs_ans_type_id == temp && item.obs_question_id == question_id).ToList().Count;
                bool isNew_Quest_Ans_Type = number_of_records == 0 ? true : false;
                return isNew_Quest_Ans_Type;           
            
        }
        public void setExistingDefaultToN( int question_id)
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
        public void save(OBS_QUEST_ANS_TYPES oBS_QUEST_ANS_TYPES_U)
        {
            using (DSC_OBS_DB_ENTITY db1 = new DSC_OBS_DB_ENTITY())
            {
                db1.OBS_QUEST_ANS_TYPES.Add(oBS_QUEST_ANS_TYPES_U);
                db1.SaveChanges();
            }
        }



        public  List<String> getDefault_Selected_Ans_list(string answer_type_category)
        {
            List<string> q_selected_ans_type = new List<string>();
            switch (answer_type_category)
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
                    q_selected_ans_type.Add("Single Selected List Item");
                    break;
            }
            return q_selected_ans_type;
        }
    }
}