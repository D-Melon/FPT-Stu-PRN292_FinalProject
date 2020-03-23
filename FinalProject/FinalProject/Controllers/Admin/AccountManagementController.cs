using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProject.Controllers
{
    public class AccountManagementController : Controller
    {
        ////////////////////////JOB SEEKER///////////////////////////
        ///JOB SEEKER
        [HttpGet]
        public ActionResult JobSeekerManagement()
        {
            return View("~/Views/Admin/JobSeeker/ListJobSeeker.cshtml");
        }

        ///LIST
        [HttpPost]
        public ActionResult ListJobSeeker()
        {
            try
            {
                int start = Convert.ToInt32(Request["start"]);
                int length = Convert.ToInt32(Request["length"]);
                string searchValue = Request["search[value]"];
                string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
                string sortDirection = Request["order[0][dir]"];

                var mySql = @"select 
                            u.Username,
                            u.Password, 
                            u.Email, 
                            us.Name 
                            from [User] u join User_roles us on u.RoleID = us.ID 
                            order by " + sortColumnName + " " + sortDirection + " OFFSET " + start + " ROWS FETCH NEXT " + length + " ROWS ONLY";
                using (FinalProjectEntities db = new FinalProjectEntities())
                {
                    var listAccount = db.Database.SqlQuery<Account>(mySql).ToList();

                    int totalrows = listAccount.Count();
                    int totalrowsafterfiltering = totalrows;

                    return Json(new {recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering, listAccount = listAccount });
                }
            } catch (Exception e)
            {
                return View("~/Views/Error.cshtml");
            }
        }

        ///ADD
        [HttpPost]
        public ActionResult AddAccount()
        {
            try
            {
                var username = Request["username"];
                var password = Request["password"];

                using (FinalProjectEntities db = new FinalProjectEntities())
                {
                    //check dupicate username
                    //var duplicateSql = @"select * from [User]
                    //                    where Username = @username";
                    //var duplicateUser = db.Database.SqlQuery<User>(duplicateSql,
                    //    new SqlParameter("@username", username)).ToList();
                    var duplicateUser = db.Users.Where(s => s.Username == username).FirstOrDefault();
                    if (duplicateUser == null)
                    {
                        User us = new User();
                        us.Username = username;
                        us.Password = password;
                        db.Users.Add(us);
                        db.SaveChanges();
                        return Json(new { success = true, title = "Thành công", message = "Thêm tài khoản thành công."});
                    } else
                    {
                        return Json(new { success = false, title = "Lỗi", message = "Tên tài khoản đã có, hãy chọn tên tài khoản khác." });
                    }
                }
            } catch (Exception e)
            {
                return Json(new { error = true, message = "Có lỗi xảy ra." });
            }
        }

        ///Custom Entites
        public class Account {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
            public string Name { get; set; }
        }
    }
}