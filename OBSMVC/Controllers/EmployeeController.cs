﻿using OBSMVC.Models;
using PagedList;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace OBSMVC.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY();

        // GET: Employee
        public ActionResult Index(string search, int? page, int? PageSize)
        {
            ViewBag.CurrentItemsPerPage = PageSize ?? 10;

            if (!String.IsNullOrWhiteSpace(search))
            {
                var employeeList = db.DSC_EMPLOYEE.Include(e => e.DSC_LC);
                if (employeeList.Where(emp => emp.dsc_emp_last_name.Contains(search) || emp.dsc_emp_first_name.Contains(search) || emp.DSC_LC.dsc_lc_name.Contains(search) || emp.dsc_emp_perm_id.ToString().Contains(search) || emp.dsc_emp_adp_id.Contains(search) || emp.dsc_emp_email_addr.Contains(search)).ToList().Count != 0)
                {
                    return View(employeeList.Where(emp => emp.dsc_emp_last_name.Contains(search) || emp.dsc_emp_first_name.Contains(search) || emp.DSC_LC.dsc_lc_name.Contains(search) || emp.dsc_emp_perm_id.ToString().Contains(search) || emp.dsc_emp_adp_id.Contains(search) || emp.dsc_emp_email_addr.Contains(search)).OrderBy(x => x.dsc_emp_last_name).ThenBy(y => y.dsc_emp_first_name).ToList().ToPagedList(page ?? 1, PageSize ?? 10));
                }
                else
                {
                    try
                    {
                        string[] words = search.Split(' ');
                        string word0 = words[0];
                        string word1 = words[1];
                        return View(employeeList.Where(emp => (emp.dsc_emp_first_name.Contains(word0) && emp.dsc_emp_last_name.Contains(word1)) || (emp.dsc_emp_first_name.Contains(word1) && emp.dsc_emp_last_name.Contains(word0))).OrderBy(x => x.dsc_emp_last_name).ThenBy(y => y.dsc_emp_first_name).ToList().ToPagedList(page ?? 1, PageSize ?? 10));
                    }
                    catch
                    {
                        return View(employeeList.Where(emp => emp.dsc_emp_last_name.Contains(search) || emp.dsc_emp_first_name.Contains(search) || emp.DSC_LC.dsc_lc_name.Contains(search) || emp.dsc_emp_perm_id.ToString().Contains(search) || emp.dsc_emp_adp_id.Contains(search) || emp.dsc_emp_email_addr.Contains(search)).OrderBy(x => x.dsc_emp_last_name).ThenBy(y => y.dsc_emp_first_name).ToList().ToPagedList(page ?? 1, PageSize ?? 10));
                    }
                }
            }
            else
            {
                var employeeList = db.DSC_EMPLOYEE.Include(e => e.DSC_LC).OrderBy(x => x.dsc_emp_last_name).ThenBy(y => y.dsc_emp_first_name);
                return View(employeeList.ToList().ToPagedList(page ?? 1, PageSize ?? 10));
            }
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

        // GET: Employee/Edit/5
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSC_EMPLOYEE employeeToUpdate = db.DSC_EMPLOYEE.Find(id);
            if (employeeToUpdate == null)
            {
                throw new Exception("Selected Employee Id does not Exist. Review your input");
                //return HttpNotFound();
            }
            if (employeeToUpdate.dsc_emp_hire_dt == null)
            {
                employeeToUpdate.dsc_emp_hire_dt = employeeToUpdate.dsc_emp_init_work_dt;
            }
            ViewBag.dsc_assigned_lc_id = new SelectList(db.DSC_LC.Where(x => x.dsc_lc_id > 0 && x.dsc_lc_eff_end_date.Equals(null)), "dsc_lc_id", "dsc_lc_name", employeeToUpdate.dsc_assigned_lc_id);
            return View(employeeToUpdate);
        }

        // POST: Employee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "dsc_emp_id,dsc_assigned_lc_id,dsc_emp_perm_id,dsc_emp_wms_clock_nbr,dsc_emp_first_name,dsc_emp_last_name,dsc_emp_email_addr,dsc_emp_title,dsc_emp_adp_id,dsc_emp_hire_dt,dsc_emp_init_work_dt,dsc_emp_term_dt,dsc_emp_can_be_obs_yn,dsc_emp_temp_yn,dsc_emp_hourly_yn,dsc_emp_added_id,dsc_emp_added_dtm,dsc_emp_upd_uid,dsc_emp_upd_dtm,asgnd_lc_list")] DSC_EMPLOYEE formEmployee)
        {
            using (DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY())
            {
                //var employee = db.EMPLOYEEs.Single(x => x.dsc_emp_id == formEmployee.dsc_emp_id);
                DSC_EMPLOYEE employee = formEmployee;
                //if (!ModelState.IsValid) {
                //    return View(formEmployee);
                //}
                try
                {
                    List<string> asgndLCList = new List<string>();
                    if (!String.IsNullOrEmpty(formEmployee.asgnd_lc_list)) { asgndLCList = formEmployee.asgnd_lc_list.Split(',').ToList(); }

                    employee = db.DSC_EMPLOYEE.Find(formEmployee.dsc_emp_id);
                    employee.dsc_emp_title = formEmployee.dsc_emp_title;
                    employee.dsc_emp_perm_id = formEmployee.dsc_emp_perm_id;
                    employee.dsc_assigned_lc_id = formEmployee.dsc_assigned_lc_id;
                    employee.dsc_emp_can_be_obs_yn = formEmployee.dsc_emp_can_be_obs_yn == "on" ? "Y" : "N";
                    employee.dsc_emp_hourly_yn = formEmployee.dsc_emp_hourly_yn == "on" ? "Y" : "N";
                    employee.dsc_emp_temp_yn = formEmployee.dsc_emp_temp_yn == "on" ? "Y" : "N";
                    //employee.dsc_emp_hire_dt = formEmployee.dsc_emp_hire_dt;
                    employee.dsc_emp_term_dt = formEmployee.dsc_emp_term_dt;
                    employee.dsc_emp_email_addr = formEmployee.dsc_emp_email_addr;
                    employee.dsc_emp_upd_dtm = DateTime.Now;
                    employee.dsc_emp_upd_uid = User.Identity.Name;
                    formEmployee = employee;

                    updateUserLCListWithoutCommit(formEmployee.dsc_emp_id, asgndLCList.Select(int.Parse).ToList());

                    db.SaveChanges();
                    ViewBag.dsc_assigned_lc_id = new SelectList(db.DSC_LC.Where(x => x.dsc_lc_id > 0 && x.dsc_lc_eff_end_date.Equals(null)).ToList(), "dsc_lc_id", "dsc_lc_name", formEmployee.dsc_assigned_lc_id);
                    ViewBag.ConfMsg = "Employee Information Saved Successfully.";
                }
                catch (Exception ex)
                {
                    ViewBag.ConfMsg = "ERROR: " + ex.Message;
                }
                if (employee.dsc_emp_hire_dt == null)
                {
                    employee.dsc_emp_hire_dt = employee.dsc_emp_init_work_dt;
                }
                return View(employee);
            }
            //==========================================================
            //EMPLOYEE employeeFromFB = new EMPLOYEE();
            //employeeFromFB = db.EMPLOYEEs.Find(formEmployee.dsc_emp_id);

            //employeeFromFB.dsc_emp_perm_id = formEmployee.dsc_emp_perm_id;
            //employeeFromFB.dsc_emp_title = formEmployee.dsc_emp_title;
            //employeeFromFB.dsc_assigned_lc_id = formEmployee.dsc_assigned_lc_id;
            //employeeFromFB.dsc_emp_hire_dt = formEmployee.dsc_emp_hire_dt;
            //employeeFromFB.dsc_emp_term_dt = formEmployee.dsc_emp_term_dt;
            //employeeFromFB.dsc_emp_can_be_obs_yn = formEmployee.dsc_emp_can_be_obs_yn;
            //employeeFromFB.dsc_emp_hourly_yn = formEmployee.dsc_emp_hourly_yn;
            //employeeFromFB.dsc_emp_temp_yn = formEmployee.dsc_emp_temp_yn;
            //employeeFromFB.dsc_emp_upd_dtm = DateTime.Today;
            //employeeFromFB.dsc_emp_upd_uid = User.Identity.Name;

            //try
            //{
            //    db.Entry(employeeFromFB).State = EntityState.Modified;
            //    db.SaveChanges();
            //    ViewBag.ConfMsg = "Success";
            //}
            //catch { }
            //    //return RedirectToAction("Index");


            ////if (ModelState.IsValid)
            ////{
            ////    db.Entry(employeeFromFB).State = EntityState.Modified;
            ////    db.SaveChanges();
            ////    ViewBag.ConfMsg = "Success";
            ////    //return RedirectToAction("Index");
            ////}

            //ViewBag.dsc_assigned_lc_id = new SelectList(db.DSC_LC, "dsc_lc_id", "dsc_lc_name", formEmployee.dsc_assigned_lc_id);
            //return View(employeeFromFB);

            // ===========================================================================================
            ////using (DSC_OBS_DB_ENTITY db = new DSC_OBS_DB_ENTITY())
            ////{

            ////    var employee = db.EMPLOYEEs.Single(x => x.dsc_emp_id == formEmployee.dsc_emp_id);
            ////    employee.dsc_emp_title = formEmployee.dsc_emp_title;
            ////    db.SaveChanges();
            ////    formEmployee = employee;
            ////    ViewBag.
            ////    ViewBag.dsc_assigned_lc_id = new SelectList(db.DSC_LC.Where(x => x.dsc_lc_id>0).ToList(), "dsc_lc_id", "dsc_lc_name", formEmployee.dsc_assigned_lc_id);
            ////    return View(employee);
            ////}

            ////if (ModelState.IsValid) { }
            //db.ObjectStateManager.ChangeObjectState(dbEmployee, EntityState.Modified);
            //db.SaveChanges();
            //return View(dbEmployee);
        }

        // GET: Employee/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSC_EMPLOYEE eMPLOYEE = db.DSC_EMPLOYEE.Find(id);
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
            DSC_EMPLOYEE eMPLOYEE = db.DSC_EMPLOYEE.Find(id);
            db.DSC_EMPLOYEE.Remove(eMPLOYEE);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Employee/_EmpBldgAssign
        [HttpGet]
        public ActionResult _EmpBldgAssignment(int? app_user_id)
        {
            int empId = app_user_id ?? 0;
            //DSC_EMPLOYEE eMPLOYEE = db.DSC_EMPLOYEE.Find(id);
            //db.DSC_EMPLOYEE.Remove(eMPLOYEE);
            //db.SaveChanges();

            ////--- Using LINQ Lambda Syntax ---
            //List<string> AssignedBuildings = new List<string>();
            //using (DSC_OBS_DB_ENTITY dbs = new DSC_OBS_DB_ENTITY())
            //{
            //    AssignedBuildings = db.OBS_EMP_ASSGND_LC
            //        .Join(db.DSC_LC, e => e.dsc_lc_id, w => w.dsc_lc_id, (e, w) => new { e, w})
            //        .Where( e => e.e.dsc_emp_id == empId)
            //        .OrderBy(x => x.w.dsc_lc_name)
            //        .Select(lc => lc.w.dsc_lc_name)
            //        .ToList();
            //}

            ////--- Using LINQ declarative query syntax ---
            //List<string> AssignedBuildings = (
            //    from a in db.OBS_EMP_ASSGND_LC
            //    join c in db.DSC_LC  on a.dsc_lc_id equals c.dsc_lc_id
            //    where a.dsc_emp_id == empId
            //    select c.dsc_lc_name)
            //    .OrderBy(x => x).ToList();

            empLCAsgnViewModel empLCAsgnViewModel = new empLCAsgnViewModel();

            empLCAsgnViewModel.userLCList = getUserLCList(app_user_id).OrderBy(x => x.dsc_lc_name).ToList();
            empLCAsgnViewModel.unassignedLCList = getAllLCList().Except(empLCAsgnViewModel.userLCList).OrderBy(x => x.dsc_lc_name).ToList();

            return PartialView(empLCAsgnViewModel);

            //return PartialView(AssignedBuildings);
        }

        // POST: Employee/_EmpBldgAssign
        [HttpPost][ActionName("_EmpBldgAssignment")]
        public ActionResult _EmpBldgAssignmentPost(int? app_user_id)
        {
            int empId = app_user_id ?? 0;

            empLCAsgnViewModel empLCAsgnViewModel = new empLCAsgnViewModel();

            empLCAsgnViewModel.userLCList = getUserLCList(app_user_id).OrderBy(x => x.dsc_lc_name).ToList();
            empLCAsgnViewModel.unassignedLCList = getAllLCList().Except(empLCAsgnViewModel.userLCList).OrderBy(x => x.dsc_lc_name).ToList();

            return PartialView(empLCAsgnViewModel);
        }

        // Return a list of all buildings
        private List<DSC_LC> getAllLCList()
        {
            List<DSC_LC> lcList = new List<DSC_LC>();

            //Query Style 1 - SQL notation
            var query1 =
                from a in db.DSC_LC
                where a.dsc_lc_id >= 1
                select a;

            lcList = query1.ToList();

            return lcList;
        }

        // Return a list of buildings associated with a particular app user id
        private List<DSC_LC> getUserLCList(int? appUserId)
        {
            List<DSC_LC> lcList = new List<DSC_LC>();
            if (appUserId == null || appUserId == 0)
            {

            }
            else
            {
                ////Query Style 1 - SQL notation
                //var query1 =
                //    from a in db.DSC_MTRC_LC_BLDG
                //    join b in db.RZ_BLDG_AUTHORIZATION on a.dsc_mtrc_lc_bldg_id equals b.dsc_mtrc_lc_bldg_id
                //    join c in db.DSC_APP_USER on b.app_user_id equals c.app_user_id
                //    where c.app_user_id == appUserId
                //    select a;

                //Query Style 2 - Dot notation
                var query2 =
                    from child in db.OBS_EMP_ASSGND_LC
                    where child.DSC_EMPLOYEE.dsc_emp_id == appUserId
                    select child.DSC_LC;

                lcList = query2.ToList();
            }

            return lcList;
        }

        // Updates the assigned building list for a particular app_user_id
        private string updateUserLCListWithoutCommit(int app_user_id, List<int> asgndLCList)
        {
            string notused = "Success";

            try
            {
                if (app_user_id == 0)
                {
                    notused = "User Id = 0";
                    //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    List<int> userLCList = new List<int>();
                    List<int> newUserLCList = asgndLCList;

                    userLCList = getUserLCList(app_user_id).Select(x => Convert.ToInt32(x.dsc_lc_id)).ToList();

                    //Begin Add Transaction
                    try
                    {
                        //Add Rows
                        foreach (int bldgId in newUserLCList.Except(userLCList))
                        {
                            var addRow = new OBS_EMP_ASSGND_LC
                            {
                                dsc_emp_id = app_user_id,
                                dsc_lc_id = Convert.ToInt16(bldgId)
                            };

                            //if (ModelState.IsValid)
                            //{
                                db.OBS_EMP_ASSGND_LC.Add(addRow);
                            //}
                        }

                        //Delete Rows
                        foreach (int lcId in userLCList.Except(newUserLCList))
                        {
                            var removeRow = db.OBS_EMP_ASSGND_LC.Where(x => x.dsc_emp_id == app_user_id &&
                                                                x.dsc_lc_id == lcId).First();

                            //if (ModelState.IsValid)
                            //{
                                db.OBS_EMP_ASSGND_LC.Remove(removeRow);
                            //}
                        }

                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        notused = e.Message;
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                notused = e.Message;
                throw;
            }

            return notused;
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
