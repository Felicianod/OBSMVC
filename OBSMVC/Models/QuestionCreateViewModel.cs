using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OBSMVC.Models;

namespace OBSMVC.Models
{
    public class QuestionCreateViewModel
    {
        private DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY();
        //= = = = = = = = = = = = = = = CONSTRUCTOR (No parameters Create Empty Object) = = = = = = = = = = = = = = = = = = = = = = = = = = = = 
        public QuestionCreateViewModel() {
            qAssignedMD = new List<metaDataTag>();
            qUnassignedMD = new List<metaDataTag>();
            qMDCategories = new List<string>();
            var mdList = db.OBS_QUESTION_METADATA.ToList();
            foreach (var md in mdList)
            {
                metaDataTag mdTag = new metaDataTag();
                mdTag.md_id = md.obs_quest_md_id;
                mdTag.md_cat= md.obs_quest_md_cat;
                mdTag.md_value = md.obs_quest_md_value;                
                qUnassignedMD.Add(mdTag);
                qMDCategories.Add(md.obs_quest_md_cat);
            }
            qMDCategories = qMDCategories.Distinct().ToList().OrderBy(q => q).ToList();
        }

        //= = = = = = = = = = = = = = = CONSTRUCTOR (Needs a Question Id parameter) = = = = = = = = = = = = = = = = = = = = = = = = = = = = 
        public QuestionCreateViewModel(int qId)
        {
            //Set the distict list of metadata tags available
            qMDCategories = new List<string>();
            qMDCategories = db.OBS_QUESTION_METADATA.Select(x => x.obs_quest_md_cat).Distinct().OrderBy(y => y).ToList();
           
            
            // Retrieve the Question Information from OBS_Question Table
            questn = db.OBS_QUESTION.Find(qId);

            if (questn != null)
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
                    metaDataTag qMD = new metaDataTag();
                    qMD.md_id = mdNew.md_id;
                    qMD.md_value = mdNew.mdValue;
                    qMD.md_cat = mdNew.mdCat;
                    if (mdNew.mdSelected)
                    {
                        qAssignedMD.Add(qMD);
                        preMetaDataIds.Add(mdNew.md_id);
                    }
                    else
                    {
                        qUnassignedMD.Add(qMD);
                    }
                }
            }
        }
        // ----------------------------------- PUBLIC CLASS PROPERTIES ----------------------------------------------
        
        public OBS_QUESTION questn = new OBS_QUESTION();
        public List<metaDataTag> qAssignedMD = new List<metaDataTag>();
        public List<metaDataTag> qUnassignedMD = new List<metaDataTag>();
        public List<string> qMDCategories = new List<string>();
        public List<int> preMetaDataIds = new List<int>();
    }

    public class metaDataTag
    {
        [Required]
        public int md_id { get; set; }
        [Display(Name="Selected")]        
        public string md_value { get; set; }
        [Display(Name = "Metadata Category")]
        public string md_cat { get; set; }
    }
}