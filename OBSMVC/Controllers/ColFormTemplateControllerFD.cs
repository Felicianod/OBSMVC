using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OBSMVC.Models;

namespace OBSMVC.Controllers
{
    public class ColFormTemplateControllerFD : Controller
    {
        private DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY();


        [HttpGet]
        public ActionResult showColFormTemplate(int id)
        {
            CollectionForm selectedColForm = new CollectionForm(id);
            return View(selectedColForm);            
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