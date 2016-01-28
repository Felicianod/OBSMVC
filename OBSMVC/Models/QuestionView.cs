using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OBSMVC.Models
{
    [MetadataType(typeof(OuestionMetaData))]
    public partial class OBS_QUESTION { }
    public partial class OuestionMetaData
    {
       [Display(Name = "Question Id")]
       [HiddenInput(DisplayValue = false)]
        public int obs_question_id { get; set; }

        [Display(Name = "Question Version")]
        public short obs_question_ver { get; set; }
        [Display(Name = "Full Text")]
        public string obs_question_full_text { get; set; }

        [Display(Name = "Short Text")]
        public string obs_question_short_text { get; set; }

        [Display(Name = "Description")]
        public string obs_question_desc { get; set; }

        [Display(Name = "MultiMedia URL")]
        public string obs_question_mm_url { get; set; }
        [Display(Name = "Effective Start Date")]
        [DisplayFormat(DataFormatString = ("{0:MMM dd, yyyy}"))]
        public DateTime obs_question_eff_st_dt { get; set; }

        [Display(Name = "Effective End Date")]
        [DisplayFormat(DataFormatString = ("{0:MMM dd, yyyy}"))]
        public DateTime obs_question_eff_end_dt { get; set; }

        [Display(Name = "Question Added By")]
        public string obs_question_added_uid { get; set; }

        [Display(Name = "Date Added")]
        [DisplayFormat(DataFormatString = ("{0:MMM dd, yyyy}"))]
        public DateTime obs_question_added_dtm { get; set; }

        [Display(Name = "Question Updated By")]
        public string obs_question_upd_uid { get; set; }

        [Display(Name = "Date Updated")]
        [DisplayFormat(DataFormatString = ("{0:MMM dd, yyyy}"))]
        public DateTime? obs_question_upd_dtm { get; set; }

    }
}