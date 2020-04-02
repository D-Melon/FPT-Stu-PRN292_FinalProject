using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProject.Controllers.Admin
{
    public class JobManagementController : Controller
    {
        // GET: JobManager
        [HttpGet]
        public ActionResult Manage()
        {
            ViewBag.user = (User)Session["account"];
            //get list skill
            string sqlSkill = @"select * from Skill";
            //get list employeer
            string sqlEmployeer = @"select * from Employer";
            //get list location
            string sqlLocation = @"select * from Location";
            using (FinalProjectEntities db = new FinalProjectEntities()) {
                var listSkill = db.Database.SqlQuery<Skill>(sqlSkill).ToList();
                ViewBag.listSkill = listSkill;
                var listEmployer = db.Database.SqlQuery<Employer>(sqlEmployeer).ToList();
                ViewBag.listEmployer = listEmployer;
                var listLocation = db.Database.SqlQuery<Location>(sqlLocation).ToList();
                ViewBag.listLocation = listLocation;
            } 
            return View("~/Views/Admin/Job/ListJob.cshtml");
        }

        [HttpPost]
        public ActionResult ListJob()
        {
            //params of datatable
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            //get list job
            string titleJob = Request["titleJob"];
            string sqlJob = "";
            if (sqlJob == null || sqlJob == "")
            {
                sqlJob = @"select 
                        j.ID,
                        j.Title,
                        j.Short_Des,
                        j.Detail,
                        j.Salary,
                        j.Time,
                        sk.Name as 'SkillName',
                        e.Company_Name as 'CompanyName',
                        l.Name as 'LocationName' 
                        from Job j 
                        join Skill sk on j.SkillID = sk.ID
                        join Employer e on e.ID = j.EmpID 
                        join Location l on e.ID_Location = l.ID
                        order by " + sortColumnName + " " + sortDirection + " OFFSET " + start + " ROWS FETCH NEXT " + length + " ROWS ONLY";
            } else
            {
                sqlJob = @"select 
                        j.ID,
                        j.Title,
                        j.Short_Des,
                        j.Detail,
                        j.Salary,
                        j.Time,
                        sk.Name as 'SkillName',
                        e.Company_Name as 'CompanyName',
                        l.Name as 'LocationName' 
                        from Job j 
                        join Skill sk on j.SkillID = sk.ID
                        join Employer e on e.ID = j.EmpID 
                        join Location l on e.ID_Location = l.ID
                        where j.Title = @titleJob
                        order by " + sortColumnName + " " + sortDirection + " OFFSET " + start + " ROWS FETCH NEXT " + length + " ROWS ONLY";
            }
            using (FinalProjectEntities db = new FinalProjectEntities())
            {
                var listJob = (sqlJob == null || sqlJob == "")
                               ? db.Database.SqlQuery<FullJob>(sqlJob).ToList()
                               : db.Database.SqlQuery<FullJob>(sqlJob, new SqlParameter("@titleJob", titleJob)).ToList();

                int totalrows = listJob.Count();
                int totalrowsafterfiltering = totalrows;

                return Json(new { recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering, listJob = listJob });
            }
        }
    }
}

public class FullJob : Job
{
    public string SkillName { get; set; }
    public string CompanyName { get; set; }
    public string LocationName { get; set; }
    public string StringDate { 
                                get { return (Time == null ? "" : Time.Value.ToString("dd/MM/yyyy")); }
                                set { StringDate = (Time == null ? "" : Time.Value.ToString("dd/MM/yyyy")); } 
                             }
}