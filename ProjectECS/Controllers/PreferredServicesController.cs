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
    public class PreferredServicesController : Controller
    {
        private ECSWebEntities db = new ECSWebEntities();

        // GET: PreferredServices
        public ActionResult Index()
        {
            
                // Check if the client ID is stored in the session
                if (Session["id"] != null)
                { 
                // Retrieve the client ID from the session
                int clientId = (int)Session["id"];

                // Query the preferred services only for the client ID
                var preferredServices = db.PreferredServices
                    .Where(p => p.Client_id == clientId)
                    .Include(p => p.Charge)
                    .Include(p => p.ClientProduct)
                    .Include(p => p.Client)
                    .Include(p => p.Service)
                    .ToList();

                return View(preferredServices);
            }
            

            return RedirectToAction("Login", "Clients");

        }

        // GET: PreferredServices/Details/5
        public ActionResult Details(int? id)
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

        // GET: PreferredServices/Create
        public ActionResult Create()
        {
            if (Session["id"] != null) { 

            int clientId = (int)Session["id"]; //Getting client id from session

                ViewBag.ClientId = clientId;

                ViewBag.Charges_id = new SelectList(db.Charges, "ChargesID", "ChargesPerDay");



                var clientProducts = db.ClientProducts.Where(cp => cp.Client.ClientID == clientId).ToList();
                 

                ViewBag.ProductID = new SelectList(clientProducts, "ProductID", "ProductName"); 
                //ViewBag.ProductID = new SelectList(db.ClientProducts, "ProductID", "ProductName");
                //ViewBag.Client_id = new SelectList(db.Clients, "ClientID", "ClientName", clientId);
                ViewBag.ServicePreferred = new SelectList(db.Services, "ServiceID", "ServiceName");
            return View();

            }
            return RedirectToAction("Login", "Clients");
        }

        [HttpGet]
        public JsonResult getCharges(int? getid)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<Charge> chargesList = new List<Charge>();
            chargesList = db.Charges.Where(x => x.ChargesForServiceID == getid).ToList();
            return Json(chargesList, JsonRequestBehavior.AllowGet);
        }

        // POST: PreferredServices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PreferredServiceID,Client_id,ProductID,ServicePreferred,Charges_id,ServiceDays,Status,StartDate,EndDate")] PreferredService preferredService)
        {
             
            // Check if the required fields are empty
            if (ModelState.IsValid)
            {
                preferredService.Status = "Pending"; // Set the status explicitly   
                db.PreferredServices.Add(preferredService);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Repopulate the ViewBag with the appropriate SelectList
            int clientId = (int)Session["id"]; // Getting client id from session
            var clientProducts = db.ClientProducts.Where(cp => cp.Client.ClientID == clientId).ToList();

            ViewBag.Charges_id = new SelectList(db.Charges, "ChargesID", "ChargesPerDay", preferredService.Charges_id);
            ViewBag.ProductID = new SelectList(clientProducts, "ProductID", "ProductName");
            ViewBag.Client_id = new SelectList(db.Clients, "ClientID", "ClientName", preferredService.Client_id);
            ViewBag.ServicePreferred = new SelectList(db.Services, "ServiceID", "ServiceName", preferredService.ServicePreferred); 
             
            return View(preferredService);


        }

        //GET: PreferredServices/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (Session["id"] == null)
        //    {
        //        // Redirect to the login page if the session is not set
        //        return RedirectToAction("Login", "Clients");
        //    }
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    PreferredService preferredService = db.PreferredServices.Find(id);
        //    if (preferredService == null)
        //    {
        //        return HttpNotFound();
        //    }



        //    // Repopulate the ViewBag with the appropriate SelectList
        //    int clientId = (int)Session["id"]; // Getting client id from session
        //    ViewBag.ClientId = clientId;
        //    var clientProducts = db.ClientProducts.Where(cp => cp.Client.ClientID == preferredService.Client_id).ToList();

        //    SelectList productList = new SelectList(clientProducts, "ProductID", "ProductName", preferredService.ProductID);

        //    ViewBag.Charges_id = new SelectList(db.Charges, "ChargesID", "ChargesPerDay", preferredService.Charges_id);
        //    //ViewBag.ProductID = new SelectList(clientProducts, "ProductID", "ProductName");

        //    ViewBag.ProductID = productList;
        //    ViewBag.Client_id = new SelectList(db.Clients, "ClientID", "ClientName", preferredService.Client_id);
        //    ViewBag.ServicePreferred = new SelectList(db.Services, "ServiceID", "ServiceName", preferredService.ServicePreferred);


        //    return View(preferredService);
        //}


        public ActionResult Edit(int? id)
        {
            if (Session["id"] == null)
            {
                // Redirect to the login page if the session is not set
                return RedirectToAction("Login", "Clients");
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

            int clientId = (int)Session["id"]; // Getting client id from session
            ViewBag.ClientId = clientId;

            var clientProducts = db.ClientProducts.Where(cp => cp.Client.ClientID == preferredService.Client_id).ToList();
            SelectList productList = new SelectList(clientProducts, "ProductID", "ProductName", preferredService.ProductID);

            ViewBag.Charges_id = new SelectList(db.Charges, "ChargesID", "ChargesPerDay", preferredService.Charges_id);
            ViewBag.ProductID = productList;
            ViewBag.Client_id = new SelectList(db.Clients, "ClientID", "ClientName", preferredService.Client_id);
            ViewBag.ServicePreferred = new SelectList(db.Services, "ServiceID", "ServiceName", preferredService.ServicePreferred);

            return View(preferredService);
        }


        // POST: PreferredServices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "PreferredServiceID,Client_id,ProductID,ServicePreferred,Charges_id,ServiceDays,Status,StartDate,EndDate")] PreferredService preferredService)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            db.Entry(preferredService).State = EntityState.Modified;
        //            db.SaveChanges();
        //            return RedirectToAction("Index");
        //        }

        //        var clientProducts = db.ClientProducts.Where(cp => cp.Client.ClientID == preferredService.Client_id).ToList();

        //        var productList = new SelectList(clientProducts, "ProductID", "ProductName", preferredService.ProductID);

        //        ViewBag.Charges_id = new SelectList(db.Charges, "ChargesID", "ChargesPerDay", preferredService.Charges_id);

        //        ViewBag.ProductID = productList;
        //        //ViewBag.ProductID = new SelectList(db.ClientProducts, "ProductID", "ProductName");
        //        ViewBag.Client_id = new SelectList(db.Clients, "ClientID", "ClientName", preferredService.Client_id);
        //        ViewBag.ServicePreferred = new SelectList(db.Services, "ServiceID", "ServiceName", preferredService.ServicePreferred);
        //        return View(preferredService);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle the exception or log the error for debugging purposes
        //        // You can examine the 'ex' variable to get more information about the exception
        //        ModelState.AddModelError("", "An error occurred while saving the changes. Please try again.");
        //        return View(preferredService);
        //    }

        //}
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
                    return RedirectToAction("Index");
                }

            // Repopulate the ViewBag with the appropriate SelectList
            //var clientProducts = db.ClientProducts.Where(cp => cp.Client.ClientID == preferredService.Client_id).ToList();

            int clientId = (int)Session["id"]; // Getting client id from session
            var clientProducts = db.ClientProducts.Where(cp => cp.Client.ClientID == clientId).ToList();
            var selectedProduct = clientProducts.FirstOrDefault(cp => cp.ProductID == preferredService.ProductID);

                ViewBag.Charges_id = new SelectList(db.Charges, "ChargesID", "ChargesPerDay", preferredService.Charges_id);
                ViewBag.ProductID = new SelectList(clientProducts, "ProductID", "ProductName", selectedProduct);
                ViewBag.Client_id = new SelectList(db.Clients, "ClientID", "ClientName", preferredService.Client_id);
                ViewBag.ServicePreferred = new SelectList(db.Services, "ServiceID", "ServiceName", preferredService.ServicePreferred);

                return View(preferredService);
             
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
            return RedirectToAction("Index");
             
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
