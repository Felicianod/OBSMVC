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
    
    public partial class OBS_SUB_TYPE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OBS_SUB_TYPE()
        {
            this.OBS_TYPE_SUB_TYPES = new HashSet<OBS_TYPE_SUB_TYPES>();
        }
    
        public int obs_sub_type_id { get; set; }
        public string obs_sub_type_name { get; set; }
        public string obs_sub_type_desc { get; set; }
        public string obs_sub_type_group { get; set; }
        public Nullable<System.DateTime> obs_sub_type_eff_end_dt { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OBS_TYPE_SUB_TYPES> OBS_TYPE_SUB_TYPES { get; set; }
    }
}
