﻿using System;
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

            var tempMD = from t1 in db.OBS_QUESTION_METADATA
                         join t2 in db.OBS_QUEST_ASSGND_MD
                         on t1.obs_quest_md_id equals t2.obs_quest_md_id
                         select new
                         {
                             md_id = t1.obs_quest_md_id,
                             mdSelected = t2.obs_question_id,
                             mdValue = t1.obs_quest_md_value,
                             mdCat = t1.obs_quest_md_cat
                         };
            foreach (var mdNew in tempMD)
            {
                metaData x = new metaData();
                x.obs_quest_md_id = mdNew.md_id;
                x.obs_quest_md_value = mdNew.mdValue;
                x.obs_quest_md_cat = mdNew.mdCat;
                x.mdSelected =  mdNew.mdSelected == qId?true:false;
                qMD.Add(x);
            }
        }

        public OBS_QUESTION q { get; set; }
        public List<metaData> qMD = new List<metaData>();
    }

    public class metaData
    {
        public int obs_quest_md_id { get; set; }
        public bool mdSelected { get; set; }
        [Display(Name = "Metadata Value")]
        public string obs_quest_md_value { get; set; }
        [Display(Name = "Metadata Category")]
        public string obs_quest_md_cat { get; set; }
    }
}