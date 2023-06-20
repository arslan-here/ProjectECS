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
    public class ClientProductsController : Controller
    {
        private ECSWebEntities db = new ECSWebEntities();

        // GET: ClientProducts
        public ActionResult Index()
        {
            var clientProducts = db.ClientProducts.Include(c => c.Client);
            return View(clientProducts.ToList());
        }

        // GET: ClientProducts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientProduct clientProduct = db.ClientProducts.Find(id);
            if (clientProduct == null)
            {
                return HttpNotFound();
            }
            return View(clientProduct);
        }

        // GET: ClientProducts/Create
        public ActionResult Create()
        {
            if (Session["id"] != null) {

                int clientId = (int)Session["id"]; //Getting client id from session

                ViewBag.Client_id = clientId;
                //ViewBag.Client_id = new SelectList(db.Clients, "ClientID", "ClientName");
                return View();
            }
            return RedirectToAction("Login", "Clients");
        }

        // POST: ClientProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,Client_id,ProductName,ProductServiceDetails,Status")] ClientProduct clientProduct)
        {
            //if (ModelState.IsValid)
            //{

            //        clientProduct.Status = 1;
            //        db.ClientProducts.Add(clientProduct);
            //        db.SaveChanges();
            //        return RedirectToAction("Create", "PreferredServices");

            //}

            //ViewBag.Client_id = new SelectList(db.Clients, "ClientID", "ClientName", clientProduct.Client_id);
            //return View(clientProduct);

            if (ModelState.IsValid)
            {
                clientProduct.Status = 1;
                db.ClientProducts.Add(clientProduct);
                db.SaveChanges();
                return RedirectToAction("Create", "PreferredServices");
            }

            ViewBag.Client_id = clientProduct.Client_id;
            return View(clientProduct);

        }

        // GET: ClientProducts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientProduct clientProduct = db.ClientProducts.Find(id);
            if (clientProduct == null)
            {
                return HttpNotFound();
            }
            ViewBag.Client_id = new SelectList(db.Clients, "ClientID", "ClientName", clientProduct.Client_id);
            return View(clientProduct);
        }

        // POST: ClientProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,Client_id,ProductName,ProductServiceDetails,Status")] ClientProduct clientProduct)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clientProduct).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Client_id = new SelectList(db.Clients, "ClientID", "ClientName", clientProduct.Client_id);
            return View(clientProduct);
        }

        // GET: ClientProducts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientProduct clientProduct = db.ClientProducts.Find(id);
            if (clientProduct == null)
            {
                return HttpNotFound();
            }
            return View(clientProduct);
        }

        // POST: ClientProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ClientProduct clientProduct = db.ClientProducts.Find(id);
            db.ClientProducts.Remove(clientProduct);
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
