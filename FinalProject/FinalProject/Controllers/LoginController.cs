using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProject.Controllers
{
    public class LoginController : Controller
    {
        private FinalProjectEntities db = new FinalProjectEntities();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username,string password)
        {
            Session.Remove("Notification");
            var checkUser = db.Users.Where(s => s.Username.Equals(username)).Where(y => y.Password.Equals(password)).SingleOrDefault();
            if(checkUser == null)
            {
                Session["Notification"] = "Tên tài khoản / mật khẩu không đúng";
                return Redirect("/Guest/Index");
            }
            else
            {
                Session["account"] = checkUser;
                if(checkUser.RoleID == 2)
                {
                    return Redirect("/Jobseeker/Index");
                }
                else if(checkUser.RoleID == 1)
                {
                    return Redirect("/AccountManagement/ListJobSeeker");
                }
            }
            return null;
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(string username,string password,string repassword,string name,string email)
        {
            var checkExist = db.Users.Where(s => s.Username.Equals(username) || s.Email.Equals(email)).FirstOrDefault();
            if (checkExist != null)
            {
                ViewData["Notification"] = "Tên đăng nhập hoặc email đã được sử dụng";
                return View();
            }
            else
            {
                if (!password.Equals(repassword))
                {
                    ViewData["Notification"] = "Mật khẩu không khớp";
                    return View();
                }
                else
                {
                    User u = new User();
                    u.Username = username;
                    u.Password = password;
                    u.Email = email;
                    u.Name = name;
                    u.RoleID = 2;
                    u.SkillID = 0;
                    db.Users.Add(u);
                    db.SaveChanges();
                    return Redirect("/Guest/Index");
                }
            }
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return Redirect("/Guest/Index");
        }
    }
}