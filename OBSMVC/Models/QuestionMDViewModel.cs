using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OBSMVC.Models;

namespace OBSMVC.Models
{
    public class QuestionMDViewModel
    {
        private DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY();
        //= = = = = = = = = = = = = =  (DEFAULT CONSTRUCTOR)  = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = 
        public QuestionMDViewModel() {    }
        //= = = = = = = = = = = = = = = CONSTRUCTOR FULL METADATA LIST = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = 
        public QuestionMDViewModel( int qId )
        {
            q = db.OBS_QUESTION.Find(qId);
            if (q != null)
            {
                // Add all Metadata List to the QuestionMD Object
                var tempMD = from t1 in db.OBS_QUESTION_METADATA
                             join t2 in db.OBS_QUEST_ASSGND_MD.Where(item => item.obs_question_id == qId && DateTime.Today >= item.obs_qad_eff_st_dt && DateTime.Today < item.obs_qad_eff_end_dt)
                             on t1.obs_quest_md_id equals t2.obs_quest_md_id into t1Group
                             from t2 in t1Group.DefaultIfEmpty()
                             select new
                             {
                                 md_id = t1.obs_quest_md_id,
                                 mdValue = t1.obs_quest_md_value,
                                 mdCat = t1.obs_quest_md_cat,
                                 mdSelected =( (t2 != null) || (DateTime.Today >= t2.obs_qad_eff_st_dt && DateTime.Today < t2.obs_qad_eff_end_dt )? true : false)                                 
                                 //xmdSelected = (t1Group == null) ? false : true
                             };
                foreach (var mdNew in tempMD)
                {
                    metaData x = new metaData();
                    x.obs_quest_md_id = mdNew.md_id;
                    x.obs_quest_md_value = mdNew.mdValue;
                    x.obs_quest_md_cat = mdNew.mdCat;
                    if (mdNew.mdSelected)
                    {
                        qAssignedMD.Add(x);
                        preMetaDataIds.Add(mdNew.md_id);
                    }
                    else
                    {
                        qUnassignedMD.Add(x);
                    }
                }
                
                var temp_cft = (from q in db.OBS_QUEST_ANS_TYPES
                                join f in db.OBS_COL_FORM_QUESTIONS
                                on q.obs_qat_id equals f.obs_qat_id
                                where q.obs_question_id == qId && (q.obs_qat_end_eff_dt==null || q.obs_qat_end_eff_dt>DateTime.Now)
                                select f.obs_cft_id).Distinct();
                foreach(var cft in temp_cft)
                {
                    OBS_COLLECT_FORM_TMPLT x = new OBS_COLLECT_FORM_TMPLT();
                    x = db.OBS_COLLECT_FORM_TMPLT.Single(item => item.obs_cft_id == cft);
                    forms.Add(x);
                } 
            }
        }


        // ----------------------------------- PUBLIC CLASS PROPERTIES ----------------------------------------------
        
        public OBS_QUESTION q = new OBS_QUESTION();
        public List<metaData> qAssignedMD = new List<metaData>();
        public List<metaData> qUnassignedMD = new List<metaData>();
        public List<int> preMetaDataIds = new List<int>();
        public List<OBS_COLLECT_FORM_TMPLT> forms = new List<OBS_COLLECT_FORM_TMPLT>();
    }

    public class metaData
    {
        [Required]
        public int obs_quest_md_id { get; set; }
        [Display(Name="Selected")]        
        public string obs_quest_md_value { get; set; }
        [Display(Name = "Metadata Category")]
        public string obs_quest_md_cat { get; set; }
    }
}