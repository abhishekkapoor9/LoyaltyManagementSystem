using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LMS_Web.Areas.Desk.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Desk/Dashboard
        public ActionResult Index()
        {
            return View();
        }
    }
}