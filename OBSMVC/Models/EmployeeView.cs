using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OBSMVC.Models
{
    [MetadataType(typeof(EmployeeMetaData))]
    public partial class DSC_EMPLOYEE {}

    public class EmployeeMetaData
    {
        [Display(Name = "Employee Id")]
        [HiddenInput(DisplayValue =false)]
        public int dsc_emp_id { get; set; }
        [Required] [Display(Name = "Employee LC")]
        public int? dsc_assigned_lc_id { get; set; }
        [Display(Name = "JDE ID")]
        public string dsc_emp_perm_id { get; set; }
        [Display(Name = "Clock Nbr")]
        public int? dsc_emp_wms_clock_nbr { get; set; }
        [Required] [Display(Name = "First Name")]  //[ReadOnly(true)]
        public string dsc_emp_first_name { get; set; }
        [Required] [Display(Name = "Last Name")]   //[ReadOnly(true)]
        public string dsc_emp_last_name { get; set; }
        [Display(Name = "Email Address")]
        public string dsc_emp_email_addr { get; set; }
        [Display(Name = "Title")]
        public string dsc_emp_title { get; set; }
        [Required]  [Display(Name = "ADP ID")]
        public string dsc_emp_adp_id { get; set; }
        [Display(Name = "Hire Date")]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? dsc_emp_hire_dt { get; set; }
        [Display(Name = "Work Start Date")]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime dsc_emp_init_work_dt { get; set; }
        [Display(Name = "Termination Date")]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public string dsc_emp_term_dt { get; set; }
        [Display(Name = "Observable")]
        public string dsc_emp_can_be_obs_yn { get; set; }
        [Display(Name = "Temporary")]
        public string dsc_emp_temp_yn { get; set; }
        [Display(Name = "Hourly")]
        public string dsc_emp_hourly_yn { get; set; }
        [Display(Name = "Emp Added By")]
        public string dsc_emp_added_id { get; set; }
        [Display(Name = "Emp Added Date-Time")]
        [DisplayFormat(DataFormatString = ("{0:MMM dd, yyyy}"))]
        public DateTime dsc_emp_added_dtm { get; set; }
        [Display(Name = "Emp Updated By")]
        public string dsc_emp_upd_uid { get; set; }
        [Display(Name = "Emp Updated Date-Time")]
        [DisplayFormat(DataFormatString = ("{0:MMM dd, yyyy}"))]
        public DateTime? dsc_emp_upd_dtm { get; set; }
    }
}
