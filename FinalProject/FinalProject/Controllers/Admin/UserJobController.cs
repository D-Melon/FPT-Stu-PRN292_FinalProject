using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProject.Controllers.Admin
{
    public class UserJobController : Controller
    {
        // GET: UserJob
        public ActionResult Manage()
        {
            ViewBag.user = (User)Session["account"];
            
            return View("~/Views/Admin/UserJob/ListUserJob.cshtml");
        }

        [HttpPost]
        public ActionResult ListUserJob()
        {
            FinalProjectEntities db = new FinalProjectEntities();
            List<UserJob_Chitiet> listData = new List<UserJob_Chitiet>();
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            var sql = @"SELECT User_Job.ID, [User].Username, [User].Name, [User].Email, Job.Title, Employer.Company_Name
                      FROM     User_Job INNER JOIN
                      Job ON User_Job.JobID = Job.ID INNER JOIN
                      Employer ON Job.EmpID = Employer.ID INNER JOIN
                      [User] ON User_Job.JskID = [User].ID";
            listData = db.Database.SqlQuery<UserJob_Chitiet>(sql, JsonRequestBehavior.AllowGet).ToList();

            int totalrows = db.User_Job.Count();
            int totalrowsafterfiltering = totalrows;
            return Json(new { listData = listData, recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);
        }

        public class UserJob_Chitiet : User_Job
        {
            public string Username { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Title { get; set; }
            public string Company_Name { get; set; }
        }
    }
}