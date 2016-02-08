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
            List<SelectListItem> list_of_answers = new List<SelectListItem>();
            List<OBS_QUEST_SLCT_ANS> question_selected_ans_type = new List<OBS_QUEST_SLCT_ANS>();
            var all_answer_types = db.OBS_ANS_TYPE.Where(item => item.obs_ans_type_id > 0).ToList();
            string answer_type_id = formData["list_of_answers"];
            string question_id = formData["question_id"];
            string isSave = formData["save"];

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
                var obs_qat_id = (from qat in db.OBS_QUEST_ANS_TYPES.Where(x => x.obs_question_id == quest_id && x.obs_ans_type_id== selected_ans_type_id)
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
                        if (x.obs_ans_type_has_fxd_ans_yn == "Y"&& selected_obs_qat_id!=-1)
                        {
                            question_selected_ans_type = db.OBS_QUEST_SLCT_ANS.Where(item => item.obs_qat_id == selected_obs_qat_id).ToList();
                            ViewBag.question_selected_ans_type = question_selected_ans_type;
                        }
                        else if(x.obs_ans_type_has_fxd_ans_yn == "Y")
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
                    
        }

        // POST: Save default Answer Type.
        [HttpPost]
        public ActionResult _SaveDefaultAnswerType(FormCollection formData)
        {
            string answer_type_id = formData["list_of_answers"];
            int question_id = Convert.ToInt32(formData["question_id"]);
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
            }//end if
            else 
            {
                int selected_ans_type_id = Convert.ToInt32(formData["list_of_answers"]);


            }
            return null;
        }
        public bool isQuest_Slct_Ans_Required(int ans_type_id)
        {
            short temp = (short)ans_type_id;
            bool isRequired = db.OBS_ANS_TYPE.Where(item => item.obs_ans_type_id == temp).Select(x => x.obs_ans_type_has_fxd_ans_yn).Equals("Y") ? true : false;
            return isRequired;
        }
        /*public bool isNew_Quest_Ans_Type(int ans_type_id, int question_id)
        {
            short temp = (short)ans_type_id;
           // bool isRequired = db.OBS_QUEST_ANS_TYPES.
            return isRequired;
        }*/
    }
}