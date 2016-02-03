using OBSMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OBSMVC.Controllers
{
    public class QuestionAnswerTypeDefaultController : Controller
    {
        private DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY();
        // GET: QuestionAnswerTypeDefault
        public ActionResult _listAnswerTypes(int question_id)
        {

            // var all_answer_types = (from ans in db.OBS_ANS_TYPE select new { obs_ans_type_id=ans.obs_ans_type_id, obs_ans_type_name =ans.obs_ans_type_name,isDefault="N"}).ToList();
            var all_answer_types = db.OBS_ANS_TYPE.Where(item =>item.obs_ans_type_id>0).ToList();
            var default_answer_type = (from qat in db.OBS_QUEST_ANS_TYPES.Where(x => x.obs_question_id == question_id && x.obs_qat_default_ans_type_yn == "Y")
                                       select new { qat.obs_ans_type_id }).ToList();
            int default_answer_id = -1;
            try
            {
                default_answer_id = default_answer_type.FirstOrDefault().obs_ans_type_id;
            }
            catch(NullReferenceException)
            { }

            List<SelectListItem> list_of_answers = new List<SelectListItem>();
            foreach (var x in all_answer_types)
            {
                SelectListItem answer_type = new SelectListItem();
                answer_type.Value = x.obs_ans_type_id.ToString();
                answer_type.Text = x.obs_ans_type_name;
                
                answer_type.Selected = x.obs_ans_type_id == default_answer_id ? true : false;
               list_of_answers.Add(answer_type);
            }
            ViewBag.list_of_answers = list_of_answers;
            return View(list_of_answers);
        }
    }
}