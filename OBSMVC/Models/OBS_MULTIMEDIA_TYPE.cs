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
    
    public partial class OBS_MULTIMEDIA_TYPE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OBS_MULTIMEDIA_TYPE()
        {
            this.OBS_COL_FORM_INST_MM_ATTACH = new HashSet<OBS_COL_FORM_INST_MM_ATTACH>();
        }
    
        public short obs_mm_type_id { get; set; }
        public string obs_mm_type_cat { get; set; }
        public string obs_mm_type_fmt { get; set; }
        public string obs_mm_mime { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OBS_COL_FORM_INST_MM_ATTACH> OBS_COL_FORM_INST_MM_ATTACH { get; set; }
    }
}
