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
    public class EmpController : Controller
    {
        ECSWebEntities db = new ECSWebEntities();
        // GET: Emp
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EmpDetails()
        {
            if (Session["Empid"] != null)
            {
                var employee = db.Employees.Find(Session["Empid"]);

                if (employee != null && employee.Department.DepartName == "HR Management")
                {
                    var employees = db.Employees.Include(e => e.Department).Include(e => e.Service);
                    employees = employees.Where(e => e.Department.DepartName != "HR Management");
                    return View(employees.ToList());
                }
            }

            return RedirectToAction("Login", "Employees");
        }


        //public ActionResult OrderDetails()
        //{
        //    int empSrvc;
        //    if (Session["Empid"] != null && int.TryParse(Session["EmpSrvc"]?.ToString(), out empSrvc))
        //    {
        //        var orderDetails = db.PreferredServices
        //           .Where(p => p.ServicePreferred == empSrvc)
        //            .ToList();

        //        return View(orderDetails);
        //    }

        //    return RedirectToAction("Login", "Employees");
        //}



        public ActionResult OrderDetails()
        {
            int empSrvc;
            if (Session["Empid"] != null && int.TryParse(Session["EmpSrvc"]?.ToString(), out empSrvc))
            {
                var employee = db.Employees.Find(Session["Empid"]);
                if (employee != null && employee.Department.DepartName == "Service")
                {
                    var orderDetails = db.PreferredServices
                        .Where(p => p.ServicePreferred == empSrvc)
                        .ToList();

                    return View(orderDetails);
                }
                else if (employee != null && employee.Department.DepartName == "HR Management")
                {
                    return RedirectToAction("HRaddEmp");
                }
            }

            return RedirectToAction("Login", "Employees");
        }

        public ActionResult HRaddEmp()
        {
            if (Session["Empid"] != null)
            {
                var employee = db.Employees.Find(Session["Empid"]);

                if (employee != null && employee.Department.DepartName == "HR Management")
                {
                    ViewBag.EmpDesignation = new SelectList(db.Departments, "DepartID", "DepartName");
                    ViewBag.EmpService = new SelectList(db.Services, "ServiceID", "ServiceName");
                    return View();
                }
            }

            return RedirectToAction("Login", "Employees");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HRaddEmp(Employee employee, HttpPostedFileBase EmpImg)
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
                return RedirectToAction("EmpDetails");
            }

            ViewBag.EmpDesignation = new SelectList(db.Departments, "DepartID", "DepartName", employee.EmpDesignation);
            ViewBag.EmpService = new SelectList(db.Services, "ServiceID", "ServiceName", employee.EmpService);
            return View(employee);
        }

        public ActionResult LogOut()
        {
            Session["Empid"] = null;
            Session["EmpSrvc"] = null;
            return RedirectToAction("Login", "Employees");
        }

    }

}