using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OBSMVC.Models
{
    public class QuestionMDViewModel
    {

        [Display(Name = "Metadata ID")]
        [HiddenInput(DisplayValue = false)]

        public int obs_quest_md_id { get; set; }
        [Display(Name = "Metadata Value")]
        public string obs_quest_md_value { get; set; }
        [Display(Name = "Metadata Category")]
        public string obs_quest_md_cat { get; set; }

    }
}