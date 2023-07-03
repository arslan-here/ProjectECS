using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectECS.Models;

namespace ProjectECS.Controllers
{
    public class ClientsController : Controller
    {
        private ECSWebEntities db = new ECSWebEntities();

        // GET: Clients
        public ActionResult Index()
        {
            if (Session["AdminId"] != null) 
            { 
               return View(db.Clients.ToList());
            }
            return RedirectToAction("Login", "Admin");
        }

        // GET: Clients/Login

        public ActionResult Login()
        {
            return View();
        }

        // POST: Clients/Login
        [HttpPost]
        public ActionResult Login(ClientLogin clientLogin)
        {

            var login = db.Clients.Where(c => c.ClientEmail == clientLogin.Email && c.ClientPwd == clientLogin.Password).FirstOrDefault();
            if (login != null)
            {
                Session["id"] = login.ClientID;
                return RedirectToAction("Create", "PreferredServices");
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid credentials. Please try again.";
            }
            return View();



        }




        public ActionResult Logout()
        {
            // Clear the Client's authentication 
            Session["id"] = null;
            return View("Login"); // Redirect to the Client login page
        }


        // GET: Clients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // GET: Clients/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClientID,ClientName,ClientEmail,ClientPwd,ClientStatus")] Client client)
        {
            if (ModelState.IsValid)
            {
                client.ClientStatus = 1; // Set the status explicitly
                db.Clients.Add(client);
                db.SaveChanges();
                return RedirectToAction("Login");
            }

            return View(client);
        }

        // GET: Clients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClientID,ClientName,ClientEmail,ClientPwd,ClientStatus")] Client client)
        {
            if (ModelState.IsValid)
            {
                db.Entry(client).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(client);
        }

        // GET: Clients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Client client = db.Clients.Find(id);
            db.Clients.Remove(client);
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
