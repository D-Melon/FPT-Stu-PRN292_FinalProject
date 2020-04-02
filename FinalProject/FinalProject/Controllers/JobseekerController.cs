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
                  Where Employer.Company_Name like '%"+key+"%' or Location.Name like'%"+key+"%' or Job.Detail like '%"+key+"%' ").ToList<Job_Full>();
            
            User u = Session["account"] as User;
            ViewBag.name = u.Name;
            ViewBag.jobs = jobs;
            ViewBag.key = key;
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

            ViewBag.job = job;
            ViewBag.key = id;
            return View();
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