using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectECS.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        // POST: Admin/Login
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            // Check if the provided credentials match the admin credentials
            if (username == "admin" && password == "admin")
            {
                // Store the admin's authentication state (e.g., set a session variable, create a cookie)
                Session["AdminId"] = true;
                return RedirectToAction("Create", "Employees"); // Redirect to the admin dashboard or any other appropriate action
            }
            else
            {
                // Invalid credentials, add a model error
                ModelState.AddModelError("", "Invalid username or password.");

                return View();
            }
        }


        // GET: Admin/Logout
        public ActionResult Logout()
        {
            // Clear the admin's authentication state (e.g., session variable, cookie)
            Session["AdminId"] = null;
            return RedirectToAction("Login", "Admin"); // Redirect to the admin login page
        }

        // Example admin-only action requiring authentication
        [Authorize]
        public ActionResult Dashboard()
        {
            // Only accessible by authenticated admins
            return View();
        }
    }
}