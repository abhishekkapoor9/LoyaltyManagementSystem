using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LMS_Web.Areas.Desk.Controllers
{
    public class NewTransactionController : Controller
    {

        public class PersonModel
        {
            public string Name { get; set; }
            public string DateTime { get; set; }
        }

        // GET: Desk/NewTransaction
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult PostMethod(string cardNo)
        {
            PersonModel person = new PersonModel
            {
                //Name = name,
                //DateTime = DateTime.Now.ToString()
            };
            return Json(person);
        }
    }
}