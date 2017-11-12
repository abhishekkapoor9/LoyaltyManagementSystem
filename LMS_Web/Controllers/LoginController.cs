using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS_Datas;
using System.Web.Security;

namespace LMS_Web.Controllers
{
    public class LoginController : Controller
    {
        public new class User
        {
            public string UserName { get; set; }
            public string Role { get; set; }
            public string Password { get; set; }
            public int? UserId { get; set; }
        }
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        public static Login GetUserDetails(User user)
        {
            LoyaltyManagementSystemEntities entity = new LoyaltyManagementSystemEntities();
            return (entity.Logins.Where(u => u.UserName.ToLower() == user.UserName.ToLower() && u.Password == user.Password)).FirstOrDefault();
        }

        [HttpPost]
        public ActionResult Index(Login login)
        {
            User user = new User() { UserName = login.UserName, Password = login.Password,Role=login.Role,UserId= login.UserId};
            Login users = GetUserDetails(user);
            TempData["user"] = users.UserId;
            if (users != null)
            {
                FormsAuthentication.SetAuthCookie(users.UserName, false);
                var authTicket = new FormsAuthenticationTicket(1, users.UserName, DateTime.Now, DateTime.Now.AddMinutes(30), false, users.Role);
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                HttpContext.Response.Cookies.Add(authCookie);

                if (users.Role.Equals("SuperAdmin"))
                {

                    return RedirectToAction("Index", "Dashboard", new { area = "SuperAdmin" });
                }
                else if (users.Role.Equals("Customer"))
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Customer" });
                }
                //Admin is Client
                else if (users.Role.Equals("Client"))
                {

                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                if (users.Role.Equals("Desk"))
                {

                    return RedirectToAction("Index", "Dashboard", new { area = "Desk" });
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(login);
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(login);
            }
        }
    }
}