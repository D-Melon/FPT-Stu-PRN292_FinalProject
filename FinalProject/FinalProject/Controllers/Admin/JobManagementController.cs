﻿using FinalProject.Models;
using Newtonsoft.Json.Linq;
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
            using (FinalProjectEntities db = new FinalProjectEntities())
            {
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
            if (titleJob == null || titleJob == "")
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
            }
            else
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
                        where j.Title like @titleJob
                        order by " + sortColumnName + " " + sortDirection + " OFFSET " + start + " ROWS FETCH NEXT " + length + " ROWS ONLY";
            }
            using (FinalProjectEntities db = new FinalProjectEntities())
            {
                var listJob = (titleJob == null || titleJob == "")
                               ? db.Database.SqlQuery<FullJob>(sqlJob).ToList()
                               : db.Database.SqlQuery<FullJob>(sqlJob, new SqlParameter("@titleJob", "%" + titleJob + "%")).ToList();

                int totalrows = db.Jobs.Count();
                int totalrowsafterfiltering = totalrows;
                return Json(new { recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering, listJob = listJob });
            }
        }

        [HttpPost]
        public ActionResult AddJob()
        {
            try
            {
                string title = Request["titleJob"];
                string shortdes = Request["shortdes"];
                string detail = Request["detail"];
                string salary = Request["salary"];
                string time = Request["time"];
                string emplID = Request["emplID"];
                string skillID = Request["skillID"];
                string locationID = Request["locationID"];

                string mySql = @"insert into Job
                            values(@skillID, @emplID, @title,@shortdes, @detail, @salary, @time, @locationID)";
                using (FinalProjectEntities db = new FinalProjectEntities())
                {
                    Job j = new Job();
                    j.Title = title;
                    j.Short_Des = shortdes;
                    j.Detail = detail;
                    j.Salary = salary;
                    j.Time = DateTime.Parse(time);
                    j.EmpID = Convert.ToInt32(emplID);
                    j.SkillID = Convert.ToInt32(skillID);
                    j.LocationID = Convert.ToInt32(locationID);
                    db.Jobs.Add(j);
                    db.SaveChanges();
                    return Json(new { success = true, title = "Thành công", message = "Thêm tài khoản thành công." });
                }
            }
            catch (Exception e)
            {
                return Json(new { error = true, title = "Lỗi", message = "Có lỗi xảy ra." });
            }
        }

        [HttpGet]
        public ActionResult getJobByID()
        {
            var jobID = Convert.ToInt32(Request["jobID"]);
            var mySql = @"select * from Job where ID = @jobID";
            using (FinalProjectEntities db = new FinalProjectEntities())
            {
                var job = db.Database.SqlQuery<FullJob>(mySql, new SqlParameter("@jobID", jobID)).FirstOrDefault();
                
                if (job != null)
                {
                    return Json(new { success = true ,jobByID = job }, JsonRequestBehavior.AllowGet);
                } else
                {
                    return Json(new { error = true });
                }
            }
        }

        [HttpPost]
        public ActionResult updateJob()
        {
            var string_job = Request["job"];
            var job = JObject.Parse(string_job);
            using (FinalProjectEntities db = new FinalProjectEntities())
            {
                var u_job = db.Jobs.Find(Convert.ToInt32(job.GetValue("jobID")));
                if (u_job != null)
                {
                    u_job.Title = job.GetValue("jobTitle").ToString();
                    u_job.Short_Des = job.GetValue("jobShortDes").ToString();
                    u_job.Detail = job.GetValue("jobDetail").ToString();
                    u_job.Salary = job.GetValue("jobSalary").ToString();
                    if (job.GetValue("jobTime").ToString() != null) {
                        u_job.Time = DateTime.Parse(job.GetValue("jobTime").ToString());
                    }
                    if (job.GetValue("jobEmplID").ToString() != null) {
                        u_job.EmpID = Convert.ToInt32(job.GetValue("jobEmplID").ToString());
                    }

                    if (job.GetValue("jobSkillID").ToString() != null)
                    {
                        u_job.SkillID = Convert.ToInt32(job.GetValue("jobSkillID").ToString());
                    }
                   
                    if(job.GetValue("jobLocationID").ToString() != null)
                    {
                        u_job.LocationID = Convert.ToInt32(job.GetValue("jobLocationID").ToString());
                    }
                    db.SaveChanges();
                    return Json(new { success = true, title = "Thành công", message = "Cập nhật công việc thành công." });
                } else
                {
                    return Json(new { success = false, title = "Lỗi", message = "Cập nhật công việc lỗi." });
                }
            }
        }

        [HttpPost]
        public ActionResult deleteJob()
        {
            int jobID = Convert.ToInt32(Request["jobID"]);
            using (FinalProjectEntities db = new FinalProjectEntities())
            {
                var d_job = db.Jobs.Find(jobID);
                if (d_job != null)
                {
                    db.Jobs.Remove(d_job);
                    db.SaveChanges();
                    return Json(new { success = true, title = "Thành công", message = "Xóa công việc thành công." });
                } else
                {
                    return Json(new { success = false, title = "Lỗi", message = "Xóa công việc lỗi." });
                }
            }
        }
    }
}

public class FullJob : Job
{
    public string SkillName { get; set; }
    public string CompanyName { get; set; }
    public string LocationName { get; set; }
    public string StringDate
    {
        get { return (Time == null ? "" : Time.Value.ToString("dd/MM/yyyy")); }
        set { StringDate = (Time == null ? "" : Time.Value.ToString("dd/MM/yyyy")); }
    }
}