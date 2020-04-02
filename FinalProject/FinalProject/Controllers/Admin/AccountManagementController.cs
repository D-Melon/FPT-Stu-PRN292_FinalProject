using FinalProject.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            //get list Role
            using (FinalProjectEntities db = new FinalProjectEntities())
            {
                var listRole = db.User_roles.ToList();
                ViewBag.user = (User)Session["account"];
                ViewBag.listRole = listRole;
            }
            return View("~/Views/Admin/JobSeeker/ListJobSeeker.cshtml");
        }

        ///LIST
        [HttpPost]
        public ActionResult ListJobSeeker()
        {
            try
            {
                //params
                string username_filter = Request["username_filter"];

                int start = Convert.ToInt32(Request["start"]);
                int length = Convert.ToInt32(Request["length"]);
                string searchValue = Request["search[value]"];
                string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
                string sortDirection = Request["order[0][dir]"];

                var mySql = "";
                if (username_filter == null || username_filter == "")
                {
                    mySql = @"select
                            u.ID,
                            u.Username,
                            u.Name, 
                            u.Email, 
                            us.Name as 'RoleName'
                            from [User] u join User_roles us on u.RoleID = us.ID 
                            order by " + sortColumnName + " " + sortDirection + " OFFSET " + start + " ROWS FETCH NEXT " + length + " ROWS ONLY";
                }
                else
                {
                    mySql = @"select
                            u.ID,
                            u.Username,
                            u.Name, 
                            u.Email, 
                            us.Name as 'RoleName'
                            from [User] u join User_roles us on u.RoleID = us.ID 
                            where Username = @username_filter
                            order by " + sortColumnName + " " + sortDirection + " OFFSET " + start + " ROWS FETCH NEXT " + length + " ROWS ONLY";
                }
                using (FinalProjectEntities db = new FinalProjectEntities())
                {
                    var listAccount = (username_filter == null || username_filter == "") 
                        ? db.Database.SqlQuery<Account>(mySql).ToList() 
                        : db.Database.SqlQuery<Account>(mySql, new SqlParameter("@username_filter", username_filter)).ToList();

                    int totalrows = listAccount.Count();
                    int totalrowsafterfiltering = totalrows;

                    return Json(new { recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering, listAccount = listAccount });
                }
            }
            catch (Exception e)
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
                    var duplicateUser = db.Users.Where(s => s.Username == username).FirstOrDefault();
                    if (duplicateUser == null)
                    {
                        User us = new User();
                        us.Username = username;
                        us.Password = password;
                        us.RoleID = 2;
                        db.Users.Add(us);
                        db.SaveChanges();
                        return Json(new { success = true, title = "Thành công", message = "Thêm tài khoản thành công." });
                    }
                    else
                    {
                        return Json(new { success = false, title = "Lỗi", message = "Tên tài khoản đã có, hãy chọn tên tài khoản khác." });
                    }
                }
            }
            catch (Exception e)
            {
                return Json(new { error = true, message = "Có lỗi xảy ra." });
            }
        }

        ///Get Account By ID
        [HttpPost]
        public ActionResult getAccountByID()
        {
            try
            {
                var user_id = Request["user_id"];
                using (FinalProjectEntities db = new FinalProjectEntities())
                {
                    var mySql = @"select
                            u.*, 
                            us.Name as 'RoleName'
                            from [User] u join User_roles us on u.RoleID = us.ID 
							where u.ID = @user_id";
                    var accountByID = db.Database.SqlQuery<Account>(mySql, new SqlParameter("@user_id", user_id)).FirstOrDefault();
                    if (accountByID != null)
                    {
                        return Json(new { success = true, accountByID = accountByID });
                    }
                    else
                    {
                        return Json(new { success = false, title = "Lỗi", message = "Lấy dữ liệu tài khoản gặp lỗi." });
                    }
                }

            }
            catch (Exception e)
            {
                return Json(new { error = true, message = "Có lỗi xảy ra." });
            }
        }

        ///UPDATE
        [HttpPost]
        public ActionResult EditAccount()
        {
            try
            {
                var account = Request["account"];
                var j_account = JObject.Parse(account);
                using (FinalProjectEntities db = new FinalProjectEntities())
                {
                    var u_account = db.Users.Find(Convert.ToInt32(j_account.GetValue("id")));
                    if (u_account != null)
                    {
                        u_account.Password = j_account.GetValue("password").ToString();
                        u_account.Name = j_account.GetValue("name").ToString();
                        u_account.Email = j_account.GetValue("email").ToString();
                        u_account.CV_Link = j_account.GetValue("cv_link").ToString();
                        u_account.RoleID = Convert.ToInt32(j_account.GetValue("role_id").ToString());
                        db.SaveChanges();
                        return Json(new { success = true, title = "Thành công", message = "Cập nhật tài khoản thành công." });
                    }
                    else
                    {
                        return Json(new { success = false, title = "Lỗi", message = "Cập nhật tài khoản lỗi." });
                    }
                }
            }
            catch (Exception e)
            {
                return Json(new { error = true, message = "Có lỗi xảy ra." });
            }
        }

        ///DELETE
        [HttpPost]
        public ActionResult DeleteAccount()
        {
            try
            {
                var user_id = Request["user_id"];
                using (FinalProjectEntities db = new FinalProjectEntities())
                {
                    var account = db.Users.Find(Convert.ToInt32(user_id));
                    if (account != null)
                    {
                        db.Users.Remove(account);
                        db.SaveChanges();
                        return Json(new { success = true, title = "Thành công", message = "Xóa tài khoản thành công." });
                    }
                    else
                    {
                        return Json(new { success = true, title = "Lỗi", message = "Không tìm thấy tài khoản cần xóa." });
                    }
                }
            }
            catch (Exception e)
            {
                return Json(new { error = true, message = "Có lỗi xảy ra." });
            }
        }

        ///Custom Entites
        public class Account : User
        {
            public string RoleName { get; set; }
        }
    }
}