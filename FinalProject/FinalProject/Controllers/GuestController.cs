using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProject.Controllers
{
    public class GuestController : Controller
    {
        // GET: Guest
        public ActionResult Index()
        {
            FinalProjectEntities db = new FinalProjectEntities();
            ViewData["Notification"] = Session["Notification"] as string;
            String cur_page = Request["page"];
            if (cur_page == null) cur_page = "1";

            int cur = Convert.ToInt32(cur_page);
            int company_in_pages = 5;
            int total_company = db.Employers.Count();

            int total_pages = total_company % company_in_pages == 0 ? total_company / company_in_pages : total_company / company_in_pages + 1;

            int from = cur * company_in_pages - (company_in_pages - 1);
            int to = from + company_in_pages - 1;
            List<Company_Full> companies = db.Database.SqlQuery<Company_Full>(@"select * from (
                    	                                                        SELECT ROW_NUMBER() over (order by Employer.ID) as number ,Employer.ID, Employer.Company_Name, Employer.Company_Web, Employer.Name, Employer.Title, Employer.Email, Employer.Phone, Location.Name AS LocationName
                                                                                FROM Employer INNER JOIN
                                                                                Location ON Employer.ID_Location = Location.ID)
                    	                                                        as Data where Number between " + from + " and " + to).ToList();
            
            ViewBag.companies = companies;
            ViewBag.cur = cur;
            ViewBag.pages = total_pages;
            return View();
        }




        public class Company_Full : Employer
        {
            public string LocationName { get; set; }
        }
    }
}