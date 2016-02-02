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

        public QuestionMDViewModel()
        {
        }

        public QuestionMDViewModel( int qId )
        {
            q = db.OBS_QUESTION.Find(qId);

            //var tempMD = from t1 in db.OBS_QUESTION_METADATA
            //             join t2 in db.OBS_QUEST_ASSGND_MD 
            //             on t1.obs_quest_md_id equals t2.obs_quest_md_id into t1Group
            //             from t2 in t1Group.DefaultIfEmpty()
            //             select new
            //             {
            //                 md_id = t1.obs_quest_md_id,
            //                 mdValue = t1.obs_quest_md_value,
            //                 mdCat = t1.obs_quest_md_cat,
            //                 mdSelected = (t2 ==null)?false:true
            //             };
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
                                 mdSelected = (t2 == null) ? false : true
                                 //xmdSelected = (t1Group == null) ? false : true
                             };
                foreach (var mdNew in tempMD)
                {
                    metaData x = new metaData();
                    x.obs_quest_md_id = mdNew.md_id;
                    x.obs_quest_md_value = mdNew.mdValue;
                    x.obs_quest_md_cat = mdNew.mdCat;
                    x.mdSelected = mdNew.mdSelected;
                    qMD.Add(x);
                }
            }
        }
        public QuestionMDViewModel(int qId, bool isDetail)
        {
            q = db.OBS_QUESTION.Find(qId);       
            if (q != null)
            {
                // Add all Metadata List to the QuestionMD Object
                var tempMD = from t1 in db.OBS_QUESTION_METADATA
                             join t2 in db.OBS_QUEST_ASSGND_MD.Where(item => item.obs_question_id == qId)
                             on t1.obs_quest_md_id equals t2.obs_quest_md_id into t1Group
                             from t2 in t1Group.DefaultIfEmpty()
                             select new
                             {
                                 md_id = t1.obs_quest_md_id,
                                 mdValue = t1.obs_quest_md_value,
                                 mdCat = t1.obs_quest_md_cat,
                                 mdSelected = (t2 == null) ? false : true
                                 //xmdSelected = (t1Group == null) ? false : true
                             };
                foreach (var mdNew in tempMD)
                {
                    metaData x = new metaData();
                    x.obs_quest_md_id = mdNew.md_id;
                    x.obs_quest_md_value = mdNew.mdValue;
                    x.obs_quest_md_cat = mdNew.mdCat;
                    x.mdSelected = mdNew.mdSelected;
                    if(mdNew.mdSelected)
                    { qMD.Add(x); }
                    
                }
            }
        }

        public OBS_QUESTION q { get; set; }
        public List<metaData> qMD = new List<metaData>();
    }

    public class metaData
    {
        [Required]
        public int obs_quest_md_id { get; set; }
        [Display(Name="Selected")]
        public bool mdSelected { get; set; }
        [Display(Name = "Metadata Value")]
        public string obs_quest_md_value { get; set; }
        [Display(Name = "Metadata Category")]
        public string obs_quest_md_cat { get; set; }
    }
}