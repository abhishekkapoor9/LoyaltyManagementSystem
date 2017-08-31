using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS_Datas;
using LMS_Web.ViewModel;
namespace LMS_Web.Areas.SuperAdmin.Controllers
{
    public class AddClientController : Controller
    {
        LoyaltyManagementSystemEntities entity = new LoyaltyManagementSystemEntities();
        // GET: Admin/AddClient
        public ActionResult Index()
        {

            ViewBag.CountryDd = new SelectList(entity.countries.Where(model=>model.Active==true), "countryId", "countryName");
            ViewBag.stateDd = new SelectList(entity.countries.Where(model => model.Active == true), "stateId", "stateName");
            ViewBag.cityDd = new SelectList(entity.countries.Where(model => model.Active == true), "cityId", "cityName");
            ViewBag.AdminDd = new SelectList(entity.Admins.Where(model => model.Active == true), "AdminId", "Name");

            return View();
        }
        [HttpPost]
        public ActionResult Index(AdminViewModel model)
        {
            return View(model);
        }
    }
}