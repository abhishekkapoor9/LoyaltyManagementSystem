using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS_Datas;
using System.Collections;
using System.IO;

namespace LMS_Web.Areas.Desk.Controllers
{
    public class NewTransactionController : Controller
    {
        LoyaltyManagementSystemEntities entity = new LoyaltyManagementSystemEntities();
        int imageInt = 0;
        // GET: Desk/NewTransaction
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetCustomerDetails(string mobileNo)
        {
            int customerId = (entity.Customers.Where(x => x.MobileNo == mobileNo).Select(x => x.CustomerId)).FirstOrDefault();
            var CustomerDetails = (from customers in entity.Customers
                                   join countrys in entity.countries on customers.countryId equals countrys.countryId
                                   join states in entity.states on customers.stateId equals states.stateId
                                   join cities in entity.cities on customers.cityId equals cities.cityId
                                   where customers.MobileNo == mobileNo
                                   select new { customers, countrys, states, cities }).ToList();

            string paths = Server.MapPath("~/Images/ClientProfile/");
            string[] filePaths = Directory.GetFiles(paths, "*.jpg", SearchOption.AllDirectories);
            
            foreach (var filepath in filePaths)
            {
                string[] filename = filepath.Split('\\');
                if (filename[(filename.Length) - 1].Equals(customerId + ".jpg"))
                {
                    imageInt = 1;
                    break;
                }
                else
                {
                    continue;
                }
            }

            IEnumerable lstcustomer = CustomerDetails.Select(o => new
            {
                Id=o.customers.CustomerId,
                Name = o.customers.Name,
                Email = o.customers.EmailId,
                mobileNo = o.customers.MobileNo,
                CountryName = o.countrys.countryName,
                StateName = o.states.stateName,
                CityName = o.cities.cityName,
                Address = o.customers.Address,
                imageInt=imageInt
            }).ToList();
            return Json(lstcustomer, JsonRequestBehavior.AllowGet);
        }
    }
}
