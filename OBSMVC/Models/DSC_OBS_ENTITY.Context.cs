﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DSC_OBS_DB_ENTITY : DbContext
    {
        public DSC_OBS_DB_ENTITY()
            : base("name=DSC_OBS_DB_ENTITY")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<DSC_LC> DSC_LC { get; set; }
        public virtual DbSet<OBS_SUB_TYPE> OBS_SUB_TYPE { get; set; }
        public virtual DbSet<OBS_SUPER_TYPE> OBS_SUPER_TYPE { get; set; }
        public virtual DbSet<OBS_TYPE> OBS_TYPE { get; set; }
        public virtual DbSet<OBS_QUEST_ANS_TYPES> OBS_QUEST_ANS_TYPES { get; set; }
        public virtual DbSet<OBS_QUEST_ASSGND_MD> OBS_QUEST_ASSGND_MD { get; set; }
        public virtual DbSet<OBS_QUESTION_METADATA> OBS_QUESTION_METADATA { get; set; }
        public virtual DbSet<OBS_ANS_TYPE> OBS_ANS_TYPE { get; set; }
        public virtual DbSet<OBS_APP_EVENT_LOG> OBS_APP_EVENT_LOG { get; set; }
        public virtual DbSet<OBS_ATTRIB_DATA_TYPES> OBS_ATTRIB_DATA_TYPES { get; set; }
        public virtual DbSet<OBS_COL_FORM_INST_ATTRIB_VALS> OBS_COL_FORM_INST_ATTRIB_VALS { get; set; }
        public virtual DbSet<OBS_COL_FORM_INST_MM_ATTACH> OBS_COL_FORM_INST_MM_ATTACH { get; set; }
        public virtual DbSet<OBS_COL_FORM_INST_SIGS> OBS_COL_FORM_INST_SIGS { get; set; }
        public virtual DbSet<OBS_COL_FORM_TMPLT_ATTRIBS> OBS_COL_FORM_TMPLT_ATTRIBS { get; set; }
        public virtual DbSet<OBS_COLLECT_FORM_INST> OBS_COLLECT_FORM_INST { get; set; }
        public virtual DbSet<OBS_FORM_ATTRIBS> OBS_FORM_ATTRIBS { get; set; }
        public virtual DbSet<OBS_FORM_SECTION> OBS_FORM_SECTION { get; set; }
        public virtual DbSet<OBS_INST> OBS_INST { get; set; }
        public virtual DbSet<OBS_MULTIMEDIA_TYPE> OBS_MULTIMEDIA_TYPE { get; set; }
        public virtual DbSet<OBS_RVW_FORM_INST> OBS_RVW_FORM_INST { get; set; }
        public virtual DbSet<OBS_TYPE_SUB_TYPES> OBS_TYPE_SUB_TYPES { get; set; }
        public virtual DbSet<OBS_QUEST_SLCT_ANS> OBS_QUEST_SLCT_ANS { get; set; }
        public virtual DbSet<OBS_COL_FORM_QUESTIONS> OBS_COL_FORM_QUESTIONS { get; set; }
        public virtual DbSet<OBS_QUESTION> OBS_QUESTION { get; set; }
        public virtual DbSet<DSC_CUSTOMER> DSC_CUSTOMER { get; set; }
        public virtual DbSet<OBS_COLLECT_FORM_TMPLT> OBS_COLLECT_FORM_TMPLT { get; set; }
        public virtual DbSet<DSC_EMPLOYEE> DSC_EMPLOYEE { get; set; }
        public virtual DbSet<OBS_ROLE> OBS_ROLE { get; set; }
        public virtual DbSet<OBS_USER_AUTH> OBS_USER_AUTH { get; set; }
        public virtual DbSet<OBS_USER_ROLE> OBS_USER_ROLE { get; set; }
        public virtual DbSet<OBS_EMP_ASSGND_LC> OBS_EMP_ASSGND_LC { get; set; }
        public virtual DbSet<OBS_COL_FORM_INST_ANS> OBS_COL_FORM_INST_ANS { get; set; }
        public virtual DbSet<OBS_COL_FORM_INST_QUEST> OBS_COL_FORM_INST_QUEST { get; set; }
        public virtual DbSet<OBS_APPLICATION> OBS_APPLICATION { get; set; }
    }
}
