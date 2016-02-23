using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OBSMVC.Models;
using System.ComponentModel.DataAnnotations;

namespace OBSMVC.Controllers
{
    public class ColFormTemplateController : Controller
    {
        private DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY();

        // GET: ColFormTemplate
        public ActionResult Index()
        {
            var oBS_COLLECT_FORM_TMPLT = db.OBS_COLLECT_FORM_TMPLT.Include(o => o.DSC_CUSTOMER).Include(o => o.DSC_LC).Include(o => o.OBS_TYPE);

            List<ObsColFormTemplate> ObsColFormTemplateList = new List<ObsColFormTemplate>();
            foreach(var x in oBS_COLLECT_FORM_TMPLT)
            {
                ObsColFormTemplate obsForm = new ObsColFormTemplate();
                obsForm.OBSformID = x.obs_cft_id;
                obsForm.FormTitle = x.obs_cft_title;
                obsForm.FormNumber = x.obs_cft_nbr;
                obsForm.FormVersion = x.obs_cft_ver;
                obsForm.Customer = x.DSC_CUSTOMER.dsc_cust_name;
                obsForm.LC = x.DSC_LC.dsc_lc_name;
                obsForm.OBS_Type = x.OBS_TYPE.obs_type_name;
                obsForm.isActive = IsActiveForm(x.obs_cft_eff_st_dt, x.obs_cft_eff_end_dt);
                obsForm.QuestionCount = obsForm.getAssignedQuestionCount(x.obs_cft_id);
                obsForm.ObservationCount = obsForm.getTimesCompletedCount(x.obs_cft_id);
                obsForm.LastCompleteDate = obsForm.getLastCompleteDate(x.obs_cft_id);
                obsForm.FormSubtitle = x.obs_cft_subtitle;

                ObsColFormTemplateList.Add(obsForm);

            }
            return View(ObsColFormTemplateList.ToList());
        }

        // GET: ColFormTemplate/Details/5
        public ActionResult Details(int id)
        {
            if (id == null) { return HttpNotFound(); }

            CollectionForm selectedColForm = new CollectionForm(id);
            if (selectedColForm == null) { return HttpNotFound(); }
            
            return View(selectedColForm);
        }

        // GET: ColFormTemplate/Create
        public ActionResult Create()
        {
            ViewBag.dsc_cust_id = new SelectList(db.DSC_CUSTOMER, "dsc_cust_id", "dsc_cust_name");
            ViewBag.dsc_lc_id = new SelectList(db.DSC_LC, "dsc_lc_id", "dsc_lc_name");
            ViewBag.obs_type_id = new SelectList(db.OBS_TYPE, "obs_type_id", "obs_type_name");
            return View();
        }

        // POST: ColFormTemplate/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "obs_cft_id,obs_type_id,dsc_cust_id,dsc_lc_id,obs_cft_nbr,obs_cft_ver,obs_cft_eff_st_dt,obs_cft_eff_end_dt,obs_cft_title,obs_cft_subtitle")] OBS_COLLECT_FORM_TMPLT oBS_COLLECT_FORM_TMPLT)
        {
            if (ModelState.IsValid)
            {
                db.OBS_COLLECT_FORM_TMPLT.Add(oBS_COLLECT_FORM_TMPLT);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.dsc_cust_id = new SelectList(db.DSC_CUSTOMER, "dsc_cust_id", "dsc_cust_name", oBS_COLLECT_FORM_TMPLT.dsc_cust_id);
            ViewBag.dsc_lc_id = new SelectList(db.DSC_LC, "dsc_lc_id", "dsc_lc_name", oBS_COLLECT_FORM_TMPLT.dsc_lc_id);
            ViewBag.obs_type_id = new SelectList(db.OBS_TYPE, "obs_type_id", "obs_type_name", oBS_COLLECT_FORM_TMPLT.obs_type_id);
            return View(oBS_COLLECT_FORM_TMPLT);
        }

        // GET: ColFormTemplate/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OBS_COLLECT_FORM_TMPLT oBS_COLLECT_FORM_TMPLT = db.OBS_COLLECT_FORM_TMPLT.Find(id);
            if (oBS_COLLECT_FORM_TMPLT == null)
            {
                return HttpNotFound();
            }
            ViewBag.dsc_cust_id = new SelectList(db.DSC_CUSTOMER, "dsc_cust_id", "dsc_cust_name", oBS_COLLECT_FORM_TMPLT.dsc_cust_id);
            ViewBag.dsc_lc_id = new SelectList(db.DSC_LC, "dsc_lc_id", "dsc_lc_name", oBS_COLLECT_FORM_TMPLT.dsc_lc_id);
            ViewBag.obs_type_id = new SelectList(db.OBS_TYPE, "obs_type_id", "obs_type_name", oBS_COLLECT_FORM_TMPLT.obs_type_id);
            return View(oBS_COLLECT_FORM_TMPLT);
        }

        // POST: ColFormTemplate/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "obs_cft_id,obs_type_id,dsc_cust_id,dsc_lc_id,obs_cft_nbr,obs_cft_ver,obs_cft_eff_st_dt,obs_cft_eff_end_dt,obs_cft_title,obs_cft_subtitle")] OBS_COLLECT_FORM_TMPLT oBS_COLLECT_FORM_TMPLT)
        {
            if (ModelState.IsValid)
            {
                db.Entry(oBS_COLLECT_FORM_TMPLT).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.dsc_cust_id = new SelectList(db.DSC_CUSTOMER, "dsc_cust_id", "dsc_cust_name", oBS_COLLECT_FORM_TMPLT.dsc_cust_id);
            ViewBag.dsc_lc_id = new SelectList(db.DSC_LC, "dsc_lc_id", "dsc_lc_name", oBS_COLLECT_FORM_TMPLT.dsc_lc_id);
            ViewBag.obs_type_id = new SelectList(db.OBS_TYPE, "obs_type_id", "obs_type_name", oBS_COLLECT_FORM_TMPLT.obs_type_id);
            return View(oBS_COLLECT_FORM_TMPLT);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public class ObsColFormTemplate
        {
            private DSC_OBS_DB_ENTITY OBSdb = new DSC_OBS_DB_ENTITY();

            //Constructor


            //------------------------------------------PROPERTIES---------------------------------------------------//

            [Display(Name = "Form ID")]
            public int OBSformID { get; set; }

            [Display(Name = "Form Title")]
            public String FormTitle { get; set; }

            [Display(Name = "Form Number")]
            public short FormNumber { get; set; }

            [Display(Name = "Form Version")]
            public short FormVersion { get; set; }

            [Display(Name = "Customer")]
            public string Customer { get; set; }

            [Display(Name = "LC")]
            public string LC { get; set; }

            [Display(Name = "Observation Type")]
            public string OBS_Type { get; set; }

            [Display(Name = "Number of Questions")]
            public int QuestionCount { get; set; }
            [Display(Name = "Number of Observations")]
            public int ObservationCount { get; set; }
            [Display(Name = "Active")]
            public bool isActive { get; set; }

            [Display(Name = "Last Submitted on")]
            [DisplayFormat(DataFormatString ="{0:g}")]
            public DateTime? LastCompleteDate { get; set; }

            public string FormSubtitle { get; set; }

            public int getAssignedQuestionCount(int cft_id)
            {
                int quest_count = OBSdb.OBS_COL_FORM_QUESTIONS.Where(item => item.obs_cft_id == cft_id).Count();
                return quest_count;
            }
            public int getTimesCompletedCount(int cft_id)
            {
                int times_compleded = OBSdb.OBS_COLLECT_FORM_INST.Where(item => item.obs_cft_id == cft_id && !item.obs_cfi_comp_date.Equals(null)).Count();
                return times_compleded;
            }
           public DateTime? getLastCompleteDate(int cft_id)
            {
                return  OBSdb.OBS_COLLECT_FORM_INST.Where(item => item.obs_cft_id == cft_id).Max(x => x.obs_cfi_comp_date).Equals(null) ? null : OBSdb.OBS_COLLECT_FORM_INST.Where(item => item.obs_cft_id == cft_id).Max(x => x.obs_cfi_comp_date);
                
            }
            /*public List<int> searchByQuestion(string search)
            {
                List<int> = from q in OBSdb.OBS_QUESTION join qa in OBSdb.OBS_QUEST_ANS_TYPES 
                            on q.obs_question_id equals qa.obs_question_id join fq in OBSdb.OBS_COL_FORM_QUESTIONS
                            on qa.obs_qat_id equals fq.obs_quest_ans_types_id
            }*/
        }
        //---------------------------------------------HELPERS----------------------------------------//
        public static bool IsActiveForm(DateTime start_date, DateTime end_date)
        {
            if (DateTime.Today >= start_date && DateTime.Today < end_date) { return true; }
            else { return false; }
        }
    }
    //\==================== END OF CONTROLLERS CLASS ==================================================/


    // =================================================================================================
    // ============================ HELPER CLASES FOR CONTROLERS  ======================================
    //******* CLASES ***********************************************************************************
    public class CollectionForm
    {
        private DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY();  //To give Database access to this Class

        //--- CONSTRUCTOR------------------
        public CollectionForm(int id)
        {//Ctreate the Collection Form Data (Header Info) from the Id passed as a parameter

            cft_id = id;
            //cft_Title = "TEST";
            //cft_SubTitle = "TEST";
            //cft_obsType = "TEST";
            //cft_Cust = "TEST";
            //cft_LC = "TEST";
            //cft_Status = "TEST";
            //cft_Nbr = 1;
            //cft_Version = 2;

            var q = (from A in db.OBS_COLLECT_FORM_TMPLT
                     join B in db.OBS_TYPE                          //First Table Left join
                         on A.obs_type_id equals B.obs_type_id
                         into tl_b
                     where A.obs_cft_id == cft_id
                     from B in tl_b.DefaultIfEmpty()
                     join C in db.DSC_CUSTOMER                      //Second Table Left join
                     on A.dsc_cust_id equals C.dsc_cust_id into tl_c
                     from C in tl_c.DefaultIfEmpty()
                     join D in db.DSC_LC                            //Second Table Left join
                     on A.dsc_lc_id equals D.dsc_lc_id into tl_d
                     from D in tl_d.DefaultIfEmpty()
                     select new
                     {
                         cft_id = A.obs_cft_id,
                         cft_Nbr = A.obs_cft_nbr,
                         cft_Version = A.obs_cft_ver,
                         cft_Title = A.obs_cft_title,
                         cft_SubTitle = A.obs_cft_subtitle,
                         cft_obsType = B.obs_type_name,
                         cft_Cust = C.dsc_cust_name,
                         cft_LC = D.dsc_lc_name,
                         cft_Status = ((A.obs_cft_eff_st_dt < DateTime.Now) && (A.obs_cft_eff_end_dt > DateTime.Now)) ? "ACTIVE" : "INACTIVE"
                     }).ToList().FirstOrDefault();

            cft_Title = q.cft_Title;
            cft_SubTitle = q.cft_SubTitle;
            cft_obsType = q.cft_obsType;
            cft_Cust = q.cft_Cust;
            cft_LC = q.cft_LC;
            cft_Status = q.cft_Status;
            cft_Nbr = q.cft_Nbr;
            cft_Version = q.cft_Version;
        }
        //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - \\
        //- - - - - - - - - - Properties - - - - - - - - - - - - - - - - - - - - |
        // All Properties are set at Constructor Time
        public int cft_id { get; set; }
        public string cft_Title { get; set; }
        public string cft_SubTitle { get; set; }
        public string cft_obsType { get; set; }
        public string cft_Cust { get; set; }
        public string cft_LC { get; set; }
        public string cft_Status { get; set; }
        public int cft_Nbr { get; set; }
        public int cft_Version { get; set; }
        public List<CollectionFormQuestion> colFormQuestionList { get; set; }
        //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -\\
        //- - - - - - - - - - - - CLASS METHODS - - - - - - - - - - - - - - - - |
        //.... TODO ....
        //------------------
    }

    public class CollectionFormQuestion
    {
        private DSC_OBS_DB_ENTITY OBSdb = new DSC_OBS_DB_ENTITY();  //To give Database access to this Class

        //--- CONSTRUCTOR------------------
        public CollectionFormQuestion()
        {
        }
        //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - \\
        //- - - - - - - - - - Properties - - - - - - - - - - - - - - - - - - - - |
        // All Properties are set at Constructor Time
        public int cft_id { get; set; }
        public string Name { get; set; }
        //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -\\
        //- - - - - - - - - - - - CLASS METHODS - - - - - - - - - - - - - - - - |
        //.... TODO ....
        //------------------
    }
}
