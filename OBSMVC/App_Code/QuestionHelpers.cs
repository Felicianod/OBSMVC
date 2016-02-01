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

        
    }
    
}