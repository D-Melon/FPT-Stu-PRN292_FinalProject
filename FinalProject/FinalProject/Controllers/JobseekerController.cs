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
            List<Company_Full> companies = db.Database.SqlQuery<Company_Full>(@"SELECT Top 10  Employer.ID, Employer.Company_Name, Employer.Company_Web, Employer.Name, Employer.Title, Employer.Email, Employer.Phone, Location.Name AS LocationName
                                                                                FROM Employer INNER JOIN
                                                                                Location ON Employer.ID_Location = Location.ID").ToList();
            User u = (User)Session["account"];
            ViewBag.user = u;
            ViewBag.companies = companies;
            return View();
        }


        public class Company_Full : Employer
        {
            public string LocationName { get; set; }
        }
    }
}