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
    
    public partial class OBS_RVW_FORM_INST
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OBS_RVW_FORM_INST()
        {
            this.OBS_INST_EVENT_LOG = new HashSet<OBS_INST_EVENT_LOG>();
        }
    
        public long obs_cfi_id { get; set; }
        public long obs_inst_id { get; set; }
        public int dsc_reviewer_emp_id { get; set; }
    
        public virtual EMPLOYEE DSC_EMPLOYEE { get; set; }
        public virtual OBS_INST OBS_INST { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OBS_INST_EVENT_LOG> OBS_INST_EVENT_LOG { get; set; }
    }
}
