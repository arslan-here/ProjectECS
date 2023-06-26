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

        public ActionResult OrderDetails()
        {
            int empSrvc;
            if (Session["Empid"] != null && int.TryParse(Session["EmpSrvc"]?.ToString(), out empSrvc))
            {
                var orderDetails = db.PreferredServices
                   .Where(p => p.ServicePreferred == empSrvc)
                    .ToList();

                return View(orderDetails);
            }

            return RedirectToAction("Login", "Employees");
        }


    }

}