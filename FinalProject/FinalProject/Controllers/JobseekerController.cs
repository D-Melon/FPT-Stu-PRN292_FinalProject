using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProject.Controllers
{
    public class JobseekerController : Controller
    {
        // GET: Jobseeker
        public ActionResult Index()
        {
            FinalProjectEntities db = new FinalProjectEntities();
            ViewData["Notification"] = Session["Notification"] as string;
            String company = Request["company"];
            if (company == null) company = "";
            String cur_page = Request["page"];
            if (cur_page == null) cur_page = "1";

            int cur = Convert.ToInt32(cur_page);
            int company_in_pages = 5;
            int total_company = db.Employers.Where(x => x.Company_Name.Contains(company)).Count();

            int total_pages = total_company % company_in_pages == 0 ? total_company / company_in_pages : total_company / company_in_pages + 1;

            int from = cur * company_in_pages - (company_in_pages - 1);
            int to = from + company_in_pages - 1;
            List<Company_Full> companies = db.Database.SqlQuery<Company_Full>(@"select * from (
                    	                                                        SELECT ROW_NUMBER() over (order by Employer.ID) as number ,Employer.ID, Employer.Company_Name, Employer.Company_Web, Employer.Name, Employer.Title, Employer.Email, Employer.Phone, Location.Name AS LocationName
                                                                                FROM Employer INNER JOIN
                                                                                Location ON Employer.ID_Location = Location.ID
                                                                                where Employer.Company_Name like '%"+company+"%') as Data where Number between " + from+" and "+to).ToList();
            User u = Session["account"] as User;
            ViewBag.name = u.Name;
            ViewBag.company = company;
            ViewBag.companies = companies;
            ViewBag.cur = cur;
            ViewBag.pages = total_pages;
            return View();
        }

        public ActionResult ListJob()
        {
            FinalProjectEntities db = new FinalProjectEntities();
            String key = Request["key"];
            if (key == null) key = "";
            List<Job_Full> jobs = db.Database.SqlQuery<Job_Full>(@"SELECT Job.ID,Job.SkillID,Job.EmpID,Job.Title,Job.Short_Des,Job.Detail,Job.Salary,Job.Time,Job.LocationID, Employer.Company_Name, Location.Name as LocationName, Skill.Name AS SkillName
                  FROM Job INNER JOIN
                  Employer ON Job.EmpID = Employer.ID INNER JOIN
                  Location ON Job.LocationID = Location.ID  INNER JOIN
                  Skill ON Job.SkillID = Skill.ID
                  Where Employer.Company_Name like '%"+key+"%' or Location.Name like'%"+key+"%' or Job.Title like '%"+key+ "%' or Skill.Name like '%"+key+"%'").ToList<Job_Full>();
            int number_job = jobs.Count();
            String cur_page = Request["page"];
            if (cur_page == null) cur_page = "1";
            int jobs_in_pages = 5;
            int cur = Convert.ToInt32(cur_page);
            int total_pages = number_job % jobs_in_pages == 0 ? number_job / jobs_in_pages : number_job / jobs_in_pages + 1;
            int from = cur * jobs_in_pages - (jobs_in_pages - 1);
            int to = from + jobs_in_pages - 1;
            List<Job_Full> job_paging = db.Database.SqlQuery<Job_Full>(@"select * from (
                  SELECT ROW_NUMBER() over (order by Employer.ID) as number, Job.ID,Job.SkillID,Job.EmpID,Job.Title,Job.Short_Des,Job.Detail,Job.Salary,Job.Time,Job.LocationID, Employer.Company_Name, Location.Name as LocationName, Skill.Name AS SkillName
                  FROM Job INNER JOIN
                  Employer ON Job.EmpID = Employer.ID INNER JOIN
                  Location ON Job.LocationID = Location.ID  INNER JOIN
                  Skill ON Job.SkillID = Skill.ID
                  Where Employer.Company_Name like '%" + key + "%' or Location.Name like'%" + key + "%' or Job.Title like '%" + key + "%' or Skill.Name like '%" + key + "%') as Data where Number between " + from + " and " + to).ToList();
            
            User u = Session["account"] as User;
            ViewBag.name = u.Name;
            ViewBag.key = key;
            ViewBag.jobs = job_paging;
            ViewBag.cur = cur;
            ViewBag.pages = total_pages;
            return View();
        }

        public ActionResult DetailJob()
        {
            
            FinalProjectEntities db = new FinalProjectEntities();
            String id = Request["id"];
            if (id == null) id = "1";
            Job_Full job = db.Database.SqlQuery<Job_Full>(@"SELECT  Job.ID,Job.SkillID,Job.EmpID,Job.Title,Job.Short_Des,Job.Detail,Job.Salary,Job.Time,Job.LocationID, Employer.Company_Name, Location.Name as LocationName, Skill.Name AS SkillName
                    FROM Job INNER JOIN
                  Employer ON Job.EmpID = Employer.ID INNER JOIN
                  Location ON Job.LocationID = Location.ID  INNER JOIN
                  Skill ON Job.SkillID = Skill.ID
                  Where Job.ID = "+Convert.ToInt32(id)+"").FirstOrDefault();

            User u = Session["account"] as User;
            ViewBag.name = u.Name;
            ViewBag.job = job;
            ViewBag.key = id;
            return View();
        }

        [HttpGet]
        public ActionResult Apply()
        {
            String job_id = Request["jobid"];
            int jobid = Convert.ToInt32(job_id);
            User u = Session["account"] as User;
            ViewBag.name = u.Name;
            ViewBag.jobid = jobid;
            return View();
        }

        [HttpPost]
        public ActionResult ApplyDone()
        {
            FinalProjectEntities db = new FinalProjectEntities();
            String cv = Request["real"];
            if(cv == null || cv.Equals(""))
            {
                String job_id = Request["jobid"];
                int jobid = Convert.ToInt32(job_id);
                User u = Session["account"] as User;
                ViewBag.jobid = jobid;
                ViewBag.name = u.Name;
                ViewData["Notification"] = "Bạn phải upload CV file";
                return View("~/Views/JobSeeker/Apply.cshtml");
            }
            else
            {
                String job_id = Request["jobid"];
                int jobid = Convert.ToInt32(job_id);
                User u = Session["account"] as User;
                int jskid = u.ID;
                User_Job uj = new User_Job();
                uj.CV = cv;
                uj.JobID = jobid;
                uj.JskID = jskid;

                db.User_Job.Add(uj);
                db.SaveChanges();
                ViewBag.name = u.Name;
                ViewBag.jobid = jobid;
                return Redirect("/OnlineJobSearching/Jobseeker/Index");
            }
        }
        public ActionResult Info()
        {
            User u = Session["account"] as User;
            ViewBag.user = u;
            ViewBag.name = u.Name;
            return View();
        }

        public ActionResult ChangePass()
        {
            User u = Session["account"] as User;
            ViewBag.user = u;
            ViewBag.name = u.Name;
            return View();
        }

        [HttpPost]
        public ActionResult Update()
        {
            User u = Session["account"] as User;
            String name = Request["username"];
            String email = Request["email"];
            FinalProjectEntities db = new FinalProjectEntities();
            User user = db.Users.Find(u.ID);
            user.Name = name;
            user.Email = email;
            db.SaveChanges();
            u.Name = name;
            u.Email = email;
            ViewData["Notification"] = "Cập nhật thành công";
            ViewBag.user = u;
            ViewBag.name = u.Name;
            return View("~/Views/JobSeeker/Info.cshtml");
        }
        [HttpPost]
        public ActionResult ChangePassDone()
        {
            User u = Session["account"] as User;
            String pass = Request["password"];
            String newpass = Request["newpass"];
            String repass = Request["renew"];
            FinalProjectEntities db = new FinalProjectEntities();
            if (!u.Password.Equals(pass))
            {
                ViewBag.user = u;
                ViewBag.name = u.Name;
                ViewData["Notification"] = "Mật khẩu không đúng";
                return View("~/Views/JobSeeker/ChangePass.cshtml");
            }
            else if (!newpass.Equals(repass))
            {
                ViewBag.user = u;
                ViewBag.name = u.Name;
                ViewData["Notification"] = "Mật khẩu không khớp";
                return View("~/Views/JobSeeker/ChangePass.cshtml");
            }
            else
            {
                User user = db.Users.Find(u.ID);
                user.Password = newpass;
                db.SaveChanges();
                ViewData["Notification"] = "Cập nhật thành công";
                ViewBag.user = u;
                ViewBag.name = u.Name;
                return View("~/Views/JobSeeker/Info.cshtml");
            }
        }
        public class Company_Full : Employer
        {
            public string LocationName { get; set; }
        }

        public class Job_Full : Job
        {
            public string LocationName { get; set; }
            public string SkillName { get; set; }
            public string Company_Name { get; set; }
            public string StringDate { get { return (Time == null ? "" : Time.Value.ToString("dd-MM-yyyy")); } }
        }
    }
}