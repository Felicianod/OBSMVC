﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OBSMVC.Models;
using PagedList;
using PagedList.Mvc;


namespace OBSMVC.Controllers
{   [Authorize]
    public class EmployeeController : Controller
    {
        private DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY();

        // GET: Employee
        public ActionResult Index( string search, int? page)
        {
            if (!String.IsNullOrWhiteSpace(search))
            {
                var eMPLOYEEs = db.EMPLOYEEs.Include(e => e.DSC_LC);
                if (eMPLOYEEs.Where(emp => emp.dsc_emp_last_name.Contains(search) || emp.dsc_emp_first_name.Contains(search) || emp.DSC_LC.dsc_lc_name.Contains(search) || emp.dsc_emp_perm_id.ToString().Contains(search) || emp.dsc_emp_adp_id.Contains(search) || emp.dsc_emp_email_addr.Contains(search)).ToList().Count != 0)
                {
                    return View(eMPLOYEEs.Where(emp => emp.dsc_emp_last_name.Contains(search) || emp.dsc_emp_first_name.Contains(search) || emp.DSC_LC.dsc_lc_name.Contains(search) || emp.dsc_emp_perm_id.ToString().Contains(search) || emp.dsc_emp_adp_id.Contains(search) || emp.dsc_emp_email_addr.Contains(search)).ToList().ToPagedList(page ?? 1, 10));
                }
                else
                {
                    try {
                        string[] words = search.Split(' ');
                        string word0 = words[0];
                        string word1 = words[1];
                        return View(eMPLOYEEs.Where(emp => (emp.dsc_emp_first_name.Contains(word0) && emp.dsc_emp_last_name.Contains(word1)) || (emp.dsc_emp_first_name.Contains(word1) && emp.dsc_emp_last_name.Contains(word0))).ToList().ToPagedList(page ?? 1, 10));
                    }
                    catch
                    {
                        return View(eMPLOYEEs.Where(emp => emp.dsc_emp_last_name.Contains(search) || emp.dsc_emp_first_name.Contains(search) || emp.DSC_LC.dsc_lc_name.Contains(search) || emp.dsc_emp_perm_id.ToString().Contains(search) || emp.dsc_emp_adp_id.Contains(search) || emp.dsc_emp_email_addr.Contains(search)).ToList().ToPagedList(page ?? 1, 10));
                    }
                }
            }
            else
            {
                var eMPLOYEEs = db.EMPLOYEEs.Include(e => e.DSC_LC);
               return View(eMPLOYEEs.ToList().ToPagedList(page ?? 1, 10));
               
            }
        }

        // GET: Employee/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EMPLOYEE eMPLOYEE = db.EMPLOYEEs.Find(id);
            if (eMPLOYEE == null)
            {
                return HttpNotFound();
            }
            return View(eMPLOYEE);
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            ViewBag.dsc_assigned_lc_id = new SelectList(db.DSC_LC, "dsc_lc_id", "dsc_lc_name");
            return View();
        }

        // POST: Employee/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "dsc_emp_id,dsc_assigned_lc_id,dsc_emp_perm_id,dsc_emp_wms_clock_nbr,dsc_emp_first_name,dsc_emp_last_name,dsc_emp_email_addr,dsc_emp_title,dsc_emp_adp_id,dsc_emp_hire_dt,dsc_emp_init_work_dt,dsc_emp_term_dt,dsc_emp_can_be_obs_yn,dsc_emp_temp_yn,dsc_emp_hourly_yn,dsc_emp_added_id,dsc_emp_added_dtm,dsc_emp_upd_uid,dsc_emp_upd_dtm")] EMPLOYEE eMPLOYEE)
        {
            if (ModelState.IsValid)
            {
                db.EMPLOYEEs.Add(eMPLOYEE);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.dsc_assigned_lc_id = new SelectList(db.DSC_LC, "dsc_lc_id", "dsc_lc_name", eMPLOYEE.dsc_assigned_lc_id);
            return View(eMPLOYEE);
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EMPLOYEE eMPLOYEE = db.EMPLOYEEs.Find(id);
            if (eMPLOYEE == null)
            {
                return HttpNotFound();
            }
            ViewBag.dsc_assigned_lc_id = new SelectList(db.DSC_LC, "dsc_lc_id", "dsc_lc_name", eMPLOYEE.dsc_assigned_lc_id);
            return View(eMPLOYEE);
        }

        // POST: Employee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "dsc_emp_id,dsc_assigned_lc_id,dsc_emp_perm_id,dsc_emp_wms_clock_nbr,dsc_emp_first_name,dsc_emp_last_name,dsc_emp_email_addr,dsc_emp_title,dsc_emp_adp_id,dsc_emp_hire_dt,dsc_emp_init_work_dt,dsc_emp_term_dt,dsc_emp_can_be_obs_yn,dsc_emp_temp_yn,dsc_emp_hourly_yn,dsc_emp_added_id,dsc_emp_added_dtm,dsc_emp_upd_uid,dsc_emp_upd_dtm")] EMPLOYEE eMPLOYEE)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eMPLOYEE).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.dsc_assigned_lc_id = new SelectList(db.DSC_LC, "dsc_lc_id", "dsc_lc_name", eMPLOYEE.dsc_assigned_lc_id);
            return View(eMPLOYEE);
        }

        // GET: Employee/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EMPLOYEE eMPLOYEE = db.EMPLOYEEs.Find(id);
            if (eMPLOYEE == null)
            {
                return HttpNotFound();
            }
            return View(eMPLOYEE);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EMPLOYEE eMPLOYEE = db.EMPLOYEEs.Find(id);
            db.EMPLOYEEs.Remove(eMPLOYEE);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
