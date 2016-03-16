using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OBSMVC.Models;

namespace OBSMVC.Models
{
    public class QuestionCreateEditViewModel
    {
        private DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY();
        //= = = = = = = = = = = = = = = CONSTRUCTOR (No parameters Create Empty Object) = = = = = = = = = = = = = = = = = = = = = = = = = = = = 
        public QuestionCreateEditViewModel() {
            qAssignedMD = new List<metaDataTags>();
            qUnassignedMD = new List<metaDataTags>();
            qMDCategories = new List<string>();
            var mdList = db.OBS_QUESTION_METADATA.ToList();
            foreach (var md in mdList)
            {
                metaDataTags mdTag = new metaDataTags();
                mdTag.md_id = md.obs_quest_md_id;
                mdTag.md_cat= md.obs_quest_md_cat;
                mdTag.md_value = md.obs_quest_md_value;                
                qUnassignedMD.Add(mdTag);
                qMDCategories.Add(md.obs_quest_md_cat);
            }
            qMDCategories = qMDCategories.Distinct().ToList().OrderBy(q => q).ToList();
        }

        //= = = = = = = = = = = = = = = CONSTRUCTOR (Needs a Question Id parameter) = = = = = = = = = = = = = = = = = = = = = = = = = = = = 
        public QuestionCreateEditViewModel(int qId)
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
                    metaDataTags qMD = new metaDataTags();
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
                List<OBS_QUEST_ANS_TYPES> QAInstances = db.OBS_QUEST_ANS_TYPES.Where(x => x.obs_question_id == questn.obs_question_id && (x.obs_qat_end_eff_dt == null || x.obs_qat_end_eff_dt > DateTime.Now)).ToList();
                if (QAInstances.Count() > 0)  //There were no records found in the 'OBS_QUEST_ANS_TYPES' Table for this question Id
                {
                 
                    
                    foreach (OBS_QUEST_ANS_TYPES qaInstanceTemp in QAInstances)
                    {
                        qatTags temp_qat = new qatTags();
                        temp_qat.QAT = qaInstanceTemp;                      
                        OBS_ANS_TYPE temp_answer = db.OBS_ANS_TYPE.Single(item => item.obs_ans_type_id == qaInstanceTemp.obs_ans_type_id);
                        temp_qat.answer_type_name = temp_answer.obs_ans_type_name;
                        temp_qat.answer_type_category = temp_answer.obs_ans_type_category;
                        temp_qat.selectable_ans_required = temp_answer.obs_ans_type_has_fxd_ans_yn;
                        if(temp_answer.obs_ans_type_has_fxd_ans_yn=="Y")
                        {
                            //if true, we need to list all of them and assign them to object's list of selectable answers
                            List<OBS_QUEST_SLCT_ANS> temp_select_ans = db.OBS_QUEST_SLCT_ANS.Where(item => item.obs_qat_id == qaInstanceTemp.obs_qat_id && item.obs_qsa_eff_st_dt <= DateTime.Now && item.obs_qsa_eff_end_dt > DateTime.Now).ToList();
                            temp_qat.selAns = temp_select_ans;
                        }
                        Quest_Assigned_qatTags.Add(temp_qat);
                    }
                    

                }
            }//end of  if (questn != null)
        }
        // ----------------------------------- PUBLIC CLASS PROPERTIES ----------------------------------------------
        
        public OBS_QUESTION questn = new OBS_QUESTION();
        public List<metaDataTags> qAssignedMD = new List<metaDataTags>();
        public List<metaDataTags> qUnassignedMD = new List<metaDataTags>();
        public List<string> qMDCategories = new List<string>();
        public List<int> preMetaDataIds = new List<int>();
        public List<qatTags> Quest_Assigned_qatTags = new List<qatTags>();
     
    }

    public class metaDataTags
    {
        [Required]
        public int md_id { get; set; }
        [Display(Name="Selected")]        
        public string md_value { get; set; }
        [Display(Name = "Metadata Category")]
        public string md_cat { get; set; }
    }
    public class qatTags
    {

        public OBS_QUEST_ANS_TYPES QAT = new OBS_QUEST_ANS_TYPES();
        public List<OBS_QUEST_SLCT_ANS> selAns = new List<OBS_QUEST_SLCT_ANS>();
        [Display(Name = "Answer Type")]
        public string answer_type_name { set; get; }
        [Display(Name = "Answer Type Category")]
        public string answer_type_category { set; get; }
        public string selectable_ans_required { set; get; }
       
        
    }
}