using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OBSMVC.Models
{
    [MetadataType(typeof(OuestionMetadataMetaData))]
    public partial class OBS_QUESTION_METADATA { }
    public class OuestionMetadataMetaData
    {
        [Display(Name = "Metadata ID")]
        [HiddenInput(DisplayValue = false)]
       
        public int obs_quest_md_id { get; set; }
        [Display(Name = "Metadata Value")]
        public string obs_quest_md_value { get; set; }
        [Display(Name = "Metadata Cateory")]
        public string obs_quest_md_cat { get; set; }
    }

}