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

                // Retrieve the list of available service types from the database or any other source
                var serviceTypes = db.Services.Select(s => new SelectListItem { Value = s.ServiceName, Text = s.ServiceName }).ToList();

                ViewBag.ServiceTypes = serviceTypes;


                return View(preferredServicesAdmin.ToList());
            }

            return RedirectToAction("Login", "Admin");
        }

        public ActionResult Edit(int? id)
        {
            if (Session["AdminId"] == null)
            {
                // Redirect to the login page if the session is not set
                return RedirectToAction("Login", "Admin");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PreferredService preferredService = db.PreferredServices.Find(id);
            if (preferredService == null)
            {
                return HttpNotFound();
            }
             

            var clientProducts = db.ClientProducts.Where(cp => cp.Client.ClientID == preferredService.Client_id).ToList();
            SelectList productList = new SelectList(clientProducts, "ProductID", "ProductName", preferredService.ProductID);

            ViewBag.Charges_id = new SelectList(db.Charges, "ChargesID", "ChargesPerDay", preferredService.Charges_id);
            ViewBag.ProductID = productList;
            ViewBag.Client_id = new SelectList(db.Clients, "ClientID", "ClientName", preferredService.Client_id);
            ViewBag.ServicePreferred = new SelectList(db.Services, "ServiceID", "ServiceName", preferredService.ServicePreferred);

            return View(preferredService);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PreferredServiceID,Client_id,ProductID,ServicePreferred,Charges_id,ServiceDays,Status,StartDate,EndDate")] PreferredService preferredService)
        {
            if (ModelState.IsValid)
            {
                // Update the status if needed
                preferredService.Status = "Pending";

                db.Entry(preferredService).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("PreferredServiceDetails");
            }

            // Repopulate the ViewBag with the appropriate SelectList
            //var clientProducts = db.ClientProducts.Where(cp => cp.Client.ClientID == preferredService.Client_id).ToList();

            int clientId = (int)Session["AdminId"]; // Getting client id from session
            var clientProducts = db.ClientProducts.Where(cp => cp.Client.ClientID == clientId).ToList();
            var selectedProduct = clientProducts.FirstOrDefault(cp => cp.ProductID == preferredService.ProductID);

            ViewBag.Charges_id = new SelectList(db.Charges, "ChargesID", "ChargesPerDay", preferredService.Charges_id);
            ViewBag.ProductID = new SelectList(clientProducts, "ProductID", "ProductName", selectedProduct);
            ViewBag.Client_id = new SelectList(db.Clients, "ClientID", "ClientName", preferredService.Client_id);
            ViewBag.ServicePreferred = new SelectList(db.Services, "ServiceID", "ServiceName", preferredService.ServicePreferred);

            return View(preferredService);

        }

        [HttpGet]
        public ActionResult FilterPreferredServices(string clientName, string productName, string serviceType, string startDate, string endDate, string status, bool resetForm = false, string sortOrder = "", string serviceDaysFilter = "", string totalAmountFilter = "")
        {
            var filteredServices = db.PreferredServices.Include(p => p.Charge).Include(p => p.ClientProduct).Include(p => p.Client).Include(p => p.Service);

           

            if (!string.IsNullOrEmpty(clientName))
            {
                filteredServices = filteredServices.Where(p => p.Client.ClientName.Contains(clientName));
            }

            if (!string.IsNullOrEmpty(productName))
            {
                filteredServices = filteredServices.Where(p => p.ClientProduct.ProductName.Contains(productName));
            }

            if (!string.IsNullOrEmpty(serviceType))
            {
                filteredServices = filteredServices.Where(p => p.Service.ServiceName.Contains(serviceType));
            }

            // Apply filters for Service Days
            if (!string.IsNullOrEmpty(serviceDaysFilter))
            {
                if (serviceDaysFilter == "HighToLow")
                {
                    filteredServices = filteredServices.OrderByDescending(p => p.ServiceDays);
                }
                else if (serviceDaysFilter == "LowToHigh")
                {
                    filteredServices = filteredServices.OrderBy(p => p.ServiceDays);
                }
            }

            // Apply filters for Total Amount
            if (!string.IsNullOrEmpty(totalAmountFilter))
            {
                if (totalAmountFilter == "HighToLow")
                {
                    filteredServices = filteredServices.OrderByDescending(p => p.ServiceDays * p.Charge.ChargesPerDay);
                }
                else if (totalAmountFilter == "LowToHigh")
                {
                    filteredServices = filteredServices.OrderBy(p => p.ServiceDays * p.Charge.ChargesPerDay);
                }
            }

            if (!string.IsNullOrEmpty(startDate))
            {
                DateTime startDateTime;
                if (DateTime.TryParse(startDate, out startDateTime))
                {
                    filteredServices = filteredServices.Where(p => p.StartDate >= startDateTime);
                }
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                DateTime endDateTime;
                if (DateTime.TryParse(endDate, out endDateTime))
                {
                    filteredServices = filteredServices.Where(p => p.EndDate <= endDateTime);
                }
            }

            if (!string.IsNullOrEmpty(status))
            {
                filteredServices = filteredServices.Where(p => p.Status.Contains(status));
            }
             

            if (resetForm)
            {
                ModelState.Clear(); // Clear the model state
            }

            ViewBag.ServiceDaysFilter = serviceDaysFilter;
            ViewBag.TotalAmountFilter = totalAmountFilter;

            return View("PreferredServiceDetails", filteredServices.ToList());
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