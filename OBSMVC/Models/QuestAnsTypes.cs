using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OBSMVC.Models;
/*
 This class represents OBS_QUEST_ANS_TYPES, OBS_ANS_TYPE and OBS_QUEST_SLCT_ANS tables   
*/
namespace OBSMVC.Models
{
   
    public class QuestAnsTypes
    {

        private DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY();

        public int obs_qat_id { get; set; }
        public int obs_question_id { get; set; }
        public short obs_ans_type_id { get; set; }
        public Nullable<System.DateTime> obs_qat_end_eff_dt { get; set; }
        public string obs_qat_default_ans_type_yn { get; set; }

        List<OBS_QUEST_SLCT_ANS> quest_slct_ans = new List<OBS_QUEST_SLCT_ANS>();
        List<OBS_ANS_TYPE> ans_type = new List<OBS_ANS_TYPE>();


        public QuestAnsTypes(){}//default constructor;


    }
}