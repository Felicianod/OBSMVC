using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using OBSMVC;


namespace OBSMVC.Models
{
    public class LCViewModel
    {

        [Required]
        [Display(Name = "LC ID")]
        public int dsc_lc_id { get; }
        [Display(Name = "LC Name")]
        public string dsc_lc_name { get; set; }
        [Display(Name = "LC Code")]
        public string dsc_lc_code { get; set; }
        [Display(Name = "LC Timezone")]
        public string dsc_lc_timezone { get; set; }
        [Display(Name = "Is Active LC?")]
        public string active { get; set; }
        public string actionText { get; set; }

        public LCViewModel(int dsc_lc_id, string dsc_lc_name,  string dsc_lc_code, string dsc_lc_timezone, string active, string actionText)
        {
            this.dsc_lc_id = dsc_lc_id;
            this.dsc_lc_name = dsc_lc_name;
            this.dsc_lc_code = dsc_lc_code;
            this.dsc_lc_timezone = dsc_lc_timezone; 
            this.active = active;
            this.actionText = actionText;
        }

    }
}