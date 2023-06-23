using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectECS.Models;

namespace ProjectECS.Controllers
{
    public class PaymentsController : Controller
    {
        private ECSWebEntities db = new ECSWebEntities();

        // GET: Payments
        public ActionResult Index()
        {
            if (Session["AdminId"] != null) { 
                var payments = db.Payments.Include(p => p.PreferredService.Client)
                    .Include(p => p.PreferredService.Service)
                    .Include(p => p.PreferredService.Charge)
                    .Include(p => p.PreferredService.ClientProduct);
                return View(payments.ToList());
            }
            return RedirectToAction("Login", "Admin");
        }

        public ActionResult Search(string query)
        {

            //var results = db.Payments
            //    .Where(item => item.PreferredService.Client.ClientName.Contains(query) ||
            //    item.PreferredService.Service.ServiceName.Contains(query)) 
            //    .ToList();

            //return View("Index", results);

            IQueryable<Payment> queryablePayments = db.Payments;
            List<Payment> results;

            if (!string.IsNullOrEmpty(query))
            {
                results = queryablePayments
                    .Where(item => item.PreferredService.Client.ClientName.Contains(query) ||
                                   item.PreferredService.Service.ServiceName.Contains(query) || 
                                   item.PreferredService.ServiceDays.ToString() == query ||
                                   item.PreferredService.ClientProduct.ProductName.Contains(query)).ToList();
            }
            else
            {
                results = queryablePayments.ToList();
            }

            ViewBag.SearchQuery = query; // Pass the search query to the view

            return View("Index", results);

        }


        // GET: Payments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // GET: Payments/Create
        //public ActionResult Create()
        //{
        //    ViewBag.PaymentFor = new SelectList(db.PreferredServices, "PreferredServiceID", "Status");
        //    return View();
        //}

        //Working Code

        //public ActionResult Create(int id, decimal totalAmount)
        //{ 
        //    // Retrieve the preferred service details based on the PreferredServiceID
        //    var preferredService = db.PreferredServices.FirstOrDefault(s => s.PreferredServiceID == id);

        //    // Set the default values for the Payment model
        //    var payment = new Payment
        //    {
        //        PaymentFor = id,
        //        TotalAmount = (int)totalAmount,
        //        Status = "Paid",
        //        DateTime = DateTime.Now
        //    };

        //    return View(payment);
        //}


        public ActionResult Create(int? id, decimal? totalAmount)
        {
            if (Session["id"] != null && (id == null || id == 0 || totalAmount == null || totalAmount == 0))
            {
                return RedirectToAction("Index", "PreferredServices");
            }

            if (Session["id"] == null || id == null || id == 0 || totalAmount == null || totalAmount == 0)
            {
                 
                return RedirectToAction("Login", "Clients");
            }

            int nonNullableId = id ?? 0; // Explicitly convert nullable id to non-nullable with default value

            // Retrieve the preferred service details based on the PreferredServiceID
            var preferredService = db.PreferredServices.FirstOrDefault(s => s.PreferredServiceID == nonNullableId);

            // Set the default values for the Payment model
            var payment = new Payment
            {
                PaymentFor = nonNullableId,
                TotalAmount = (int)totalAmount.Value,
                Status = "Paid",
                DateTime = DateTime.Now
            };

            return View(payment);
        }



        // POST: Payments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "PaymentID,PaymentFor,TotalAmount,Status,DateTime")] Payment payment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Payments.Add(payment);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.PaymentFor = new SelectList(db.PreferredServices, "PreferredServiceID", "Status", payment.PaymentFor);
        //    return View(payment);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PaymentID,PaymentFor,TotalAmount,Status,DateTime")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                db.Payments.Add(payment);
                db.SaveChanges();

                // Update the PreferredService status
                var preferredService = db.PreferredServices.FirstOrDefault(s => s.PreferredServiceID == payment.PaymentFor);
                if (preferredService != null)
                {
                    preferredService.Status = "Amount Paid";
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            ViewBag.PaymentFor = new SelectList(db.PreferredServices, "PreferredServiceID", "Status", payment.PaymentFor);
            return View(payment);
        }


        // GET: Payments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            ViewBag.PaymentFor = new SelectList(db.PreferredServices, "PreferredServiceID", "Status", payment.PaymentFor);
            return View(payment);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PaymentID,PaymentFor,TotalAmount,Status,DateTime")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(payment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PaymentFor = new SelectList(db.PreferredServices, "PreferredServiceID", "Status", payment.PaymentFor);
            return View(payment);
        }

        // GET: Payments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Payment payment = db.Payments.Find(id);
            db.Payments.Remove(payment);
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
