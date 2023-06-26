using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using ProjectECS.Models;

namespace ProjectECS.Controllers
{
    public class EmployeesController : Controller
    {
        private ECSWebEntities db = new ECSWebEntities();

        // GET: Employees
        public ActionResult Index()
        {
            if (Session["AdminId"] != null) { 
            var employees = db.Employees.Include(e => e.Department).Include(e => e.Service);
            return View(employees.ToList());
            }
            return RedirectToAction("Login", "Admin");
        }

        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            if (Session["AdminId"] != null) { 
            ViewBag.EmpDesignation = new SelectList(db.Departments, "DepartID", "DepartName");
            ViewBag.EmpService = new SelectList(db.Services, "ServiceID", "ServiceName");
            return View();
            } 
            return RedirectToAction("Login", "Admin");
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "EmpID,EmpName,EmpEmail,EmpDesignation,EmpService,EmpPwd,EmpImg,EmpStatus")] Employee employee, HttpPostedFileBase EmpImg)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        if (EmpImg.ContentLength > 0)
        //        {
        //            EmpImg.SaveAs(Server.MapPath("~/Content/images/" + EmpImg.FileName));
        //        }
        //        employee.EmpImg = EmpImg.FileName;
        //        employee.EmpStatus = 1;
        //        db.Employees.Add(employee);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.EmpDesignation = new SelectList(db.Departments, "DepartID", "DepartName", employee.EmpDesignation);
        //    ViewBag.EmpService = new SelectList(db.Services, "ServiceID", "ServiceName", employee.EmpService);
        //    return View(employee);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee employee, HttpPostedFileBase EmpImg)
        {
            if (ModelState.IsValid)
            {
                // Check if an employee with the same email already exists
                if (db.Employees.Any(e => e.EmpEmail == employee.EmpEmail))
                {
                    ModelState.AddModelError("EmpEmail", "An employee with this email already exists.");
                    // Re-populate dropdown lists if needed
                    ViewBag.EmpDesignation = new SelectList(db.Departments, "DepartID", "DepartName", employee.EmpDesignation);
                    ViewBag.EmpService = new SelectList(db.Services, "ServiceID", "ServiceName", employee.EmpService);
                    return View(employee);
                }

                if (EmpImg != null && EmpImg.ContentLength > 0)
                {
                    EmpImg.SaveAs(Server.MapPath("~/Content/images/" + EmpImg.FileName));
                    employee.EmpImg = EmpImg.FileName;
                    // Store the image file name in TempData
                    TempData["ImageFileName"] = employee.EmpImg;
                }

                employee.EmpStatus = 1;
                db.Employees.Add(employee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EmpDesignation = new SelectList(db.Departments, "DepartID", "DepartName", employee.EmpDesignation);
            ViewBag.EmpService = new SelectList(db.Services, "ServiceID", "ServiceName", employee.EmpService);
            return View(employee);
        }




        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
              
            ViewBag.EmpDesignation = new SelectList(db.Departments, "DepartID", "DepartName", employee.EmpDesignation);
            ViewBag.EmpService = new SelectList(db.Services, "ServiceID", "ServiceName", employee.EmpService);
             

            return View(employee);
        }




        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmpID,EmpName,EmpEmail,EmpDesignation,EmpService,EmpPwd,EmpImg,EmpStatus")] Employee employee, HttpPostedFileBase EmpImg)
        {
            if (ModelState.IsValid)
            {
                if (EmpImg != null && EmpImg.ContentLength > 0)
                {
                    // New file is uploaded, save it and update EmpImg property
                    EmpImg.SaveAs(Server.MapPath("~/Content/images/" + EmpImg.FileName));
                    employee.EmpImg = EmpImg.FileName;
                }
                employee.EmpStatus = 1;
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmpDesignation = new SelectList(db.Departments, "DepartID", "DepartName", employee.EmpDesignation);
            ViewBag.EmpService = new SelectList(db.Services, "ServiceID", "ServiceName", employee.EmpService);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Login()
        {
            return View();
        }

        // POST: Clients/Login
        [HttpPost]
        public ActionResult Login(LoginViewModel employee)
        {
            if (ModelState.IsValid)
            {

                var login = db.Employees.Where(c => c.EmpEmail == employee.Email && c.EmpPwd == employee.Password).FirstOrDefault();
                if (login != null)
                {
                    Session["Empid"] = login.EmpID;
                    Session["EmpSrvc"] = login.EmpService;
                    return RedirectToAction("OrderDetails", "Emp");
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid credentials. Please try again.";
                }
            }
            
            return View();



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
