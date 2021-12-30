using eCommerceWebsite_1912C1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace eCommerceWebsite_1912C1.Controllers
{
    public class AdminController : Controller
    {
        private eCommerce_1912C1Entities db = new eCommerce_1912C1Entities();

        public ActionResult Index()
        {
            return View();
        }

        // GET: Admin



        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel objUser)
        {
            if (ModelState.IsValid)
            {
                var obj = db.Users.Where(a => a.Email.Equals(objUser.Email)
                && a.Password.Equals(objUser.Password)
                && a.Email_Verified.Equals("Y") && a.Role.Equals("Admin")).FirstOrDefault();

                if (obj != null) //on success
                {
                    Session["AdminName"] = obj.Name;
                    FormsAuthentication.SetAuthCookie(obj.Email, false);
                    return RedirectToAction("Index", "Admin");
                }
            }
            ModelState.AddModelError("InvalidLoginError", "Invalid credentials or email not verified!");
            return View();
        }

    }
}