//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OBSMVC.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class OBS_COL_FORM_INST_ACT_ANS
    {
        public long obs_cfiaa_id { get; set; }
        public long obs_cfi_id { get; set; }
        public int obs_question_id { get; set; }
        public Nullable<short> obs_cfiaa_question_wgt { get; set; }
        public string obs_cfiaa_ans_value { get; set; }
        public Nullable<short> obs_cfiaa_ans_wgt { get; set; }
        public string obs_cfiaa_comment { get; set; }
        public string obs_cfiaa_comment_mand_yn { get; set; }
        public string obs_cfiaa_na_yn { get; set; }
        public string obs_cfiaa_mult_ans_yn { get; set; }
    
        public virtual OBS_COLLECT_FORM_INST OBS_COLLECT_FORM_INST { get; set; }
        public virtual OBS_QUESTION OBS_QUESTION { get; set; }
    }
}