using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProject.Controllers.Admin
{
    public class JobManagementController : Controller
    {
        // GET: JobManager
        public ActionResult Manage()
        {
            ViewBag.user = (User)Session["account"];
            return View("~/Views/Admin/Job/ListJob.cshtml");
        }
    }
}