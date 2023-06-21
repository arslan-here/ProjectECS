using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ProjectECS.Models;

namespace ProjectECS.Controllers
{
    public class AdminController : Controller
    {
        private ECSWebEntities db = new ECSWebEntities();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PreferredServiceDetails()
        {
            if (Session["AdminId"] != null)
            {
                var preferredServicesAdmin = db.PreferredServices.Include(p => p.Charge).Include(p => p.ClientProduct).Include(p => p.Client).Include(p => p.Service);
                return View(preferredServicesAdmin.ToList());
            }

            return RedirectToAction("Login", "Admin");
        }

        // GET: PreferredServices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PreferredService preferredService = db.PreferredServices.Find(id);
            if (preferredService == null)
            {
                return HttpNotFound();
            }
            return View(preferredService);
        }

        // POST: PreferredServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PreferredService preferredService = db.PreferredServices.Find(id);
            db.PreferredServices.Remove(preferredService);
            db.SaveChanges();
            return RedirectToAction("PreferredServiceDetails");

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