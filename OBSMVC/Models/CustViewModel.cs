using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OBSMVC.Models
{
    
    public class CustViewModel
    {
        
       

        [Required]
        [Display(Name = "Customer ID")]
      
        public int dsc_cust_id { get; }
        [Display(Name = "Customer Name")]
        public string dsc_cust_name { get; set; }
        [Display(Name = "Customer Parent Name")]
        public string dsc_cust_parent_name { get; set; }
        [Display(Name = "Is Active Customer")]
        public string active { get; set; }

       public string actionText { get; set; }

        public CustViewModel(int dsc_cust_id, string dsc_cust_name, string dsc_cust_parent_name, string active, string actionText)
        {
            this.dsc_cust_id = dsc_cust_id;
            this.dsc_cust_name = dsc_cust_name;
            this.dsc_cust_parent_name = dsc_cust_parent_name;
            this.active = active;
            this.actionText = actionText;
        }



    }
}