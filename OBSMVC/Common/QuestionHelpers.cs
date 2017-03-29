using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OBSMVC.Models;

namespace OBSMVC.App_Code
{
    public static class QuestionHelpers
    {
        public static List<OBS_QUESTION> SearchQuestion(string search, string includeActiveOnly, DSC_OBS_DB_ENTITY db)
        {
            if (!String.IsNullOrWhiteSpace(search) && includeActiveOnly == "on")
            {
              
                return db.OBS_QUESTION.Where(ques => ques.obs_question_full_text.Contains(search) && DateTime.Today >= ques.obs_question_eff_st_dt && DateTime.Today < ques.obs_question_eff_end_dt).ToList();
            }
            else if (!String.IsNullOrWhiteSpace(search) && String.IsNullOrWhiteSpace(includeActiveOnly))
            {
                return db.OBS_QUESTION.Where(ques => ques.obs_question_full_text.Contains(search)).ToList();
            }
            else if (String.IsNullOrWhiteSpace(search) && includeActiveOnly == "on")
            {
                return db.OBS_QUESTION.Where(ques => DateTime.Today >= ques.obs_question_eff_st_dt && DateTime.Today < ques.obs_question_eff_end_dt).ToList();
            }

            else { return db.OBS_QUESTION.Where(ques => DateTime.Today >= ques.obs_question_eff_st_dt && DateTime.Today < ques.obs_question_eff_end_dt).ToList(); }

        }
        public static List<String> getDefault_Selected_Ans_list(string answer_type_category)
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