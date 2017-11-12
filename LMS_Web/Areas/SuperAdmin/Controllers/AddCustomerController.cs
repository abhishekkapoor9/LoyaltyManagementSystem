using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS_Datas;
using LMS_Web.ViewModel;
using System.Collections;
using System.IO;
using System.Data.Entity;

namespace LMS_Web.Areas.SuperAdmin.Controllers
{
    public class AddCustomerController : Controller
    {
        LoyaltyManagementSystemEntities entity = new LoyaltyManagementSystemEntities();
        // GET: Admin/AddCustomer
        public ActionResult Index()
        {
            ViewBag.CountryDd = new SelectList(entity.countries.Where(model => model.Active == true), "countryId", "countryName");
            ViewBag.alert = "Null";
            return View();
        }
        [HttpPost]
        public ActionResult Index(CustomerViewModel model, FormCollection form, HttpPostedFileBase files)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    string statusDDLValue = form["Statusddl"].ToString();
                    string genderDDLValue = form["genderddl"].ToString();
                    using (var context = new LoyaltyManagementSystemEntities())
                    {
                        var customer = new LMS_Datas.Customer()
                        {
                            Name = model.CustomerName,
                            Address = model.Address,
                            dob = model.dob,
                            EmailId = model.EmailId,
                            MobileNo = model.MobileNo,
                            gender = genderDDLValue,
                            countryId = model.countryId,
                            stateId = model.stateId,
                            cityId = model.cityId,
                            Active = Boolean.Parse(statusDDLValue)
                        };
                        context.Customers.Add(customer);
                        context.SaveChanges();
                        customer.CustomerId = customer.CustomerId;
                        var CustomerLogin = new LMS_Datas.Login()
                        {
                            UserId = customer.CustomerId,
                            UserName = model.UserName,
                            Password = model.Password,
                            Activate="true",
                            Role="Customer"
                        };
                        context.Logins.Add(CustomerLogin);
                        context.SaveChanges();
                        if (files.FileName != null)
                        {
                            var ext = Path.GetExtension(files.FileName);
                            string myfile = customer.CustomerId + ext;
                            var path = Path.Combine(Server.MapPath("~/Images/CustomerProfile"), myfile);
                            files.SaveAs(path);
                        }
                    }
                }
                ViewBag.CountryDd = new SelectList(entity.countries.Where(models => models.Active == true), "countryId", "countryName");
                ViewBag.alert = "Success";
                return View();
            }
            catch(Exception e1)
            {
                ViewBag.CountryDd = new SelectList(entity.countries.Where(models => models.Active == true), "countryId", "countryName");
                ViewBag.alert = "Error";
                return View();
            }
        }
        [HttpPost]
        public JsonResult GetStateDdl(string countryId)
        {
            //List<city> lstcity = new List<city>();
            int id = Convert.ToInt32(countryId);
            using (LoyaltyManagementSystemEntities db = new LoyaltyManagementSystemEntities())
            {
                IEnumerable lststate = (db.states.Where(x => x.countryId == id)).Select(o => new
                {
                    Stateid = o.stateId,
                    StateName = o.stateName
                }).ToList();

                return Json(lststate, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult GetCityDdl(string countryId, string stateId)
        {
            //List<city> lstcity = new List<city>();
            int Countryid = Convert.ToInt32(countryId);
            int Stateid = Convert.ToInt32(stateId);
            using (LoyaltyManagementSystemEntities db = new LoyaltyManagementSystemEntities())
            {
                IEnumerable lststate = (db.cities.Where(x => (x.countryId == Countryid && x.stateId == Stateid)).Select(o => new
                {
                    CityId = o.cityId,
                    CityName = o.cityName,

                })).ToList();

                return Json(lststate, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetCustomer(string sidx, string sort, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
        {
            //EComsDBEntity db = new EComsDBEntity();
            sort = (sort == null) ? "" : sort;
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            var CustomerList = (from customer in entity.Customers
                              where customer.Active == true
                              join country in entity.countries on customer.countryId equals country.countryId
                              join city in entity.cities on customer.cityId equals city.cityId
                              join states in entity.states on customer.stateId equals states.stateId
                              join login in entity.Logins on customer.CustomerId equals login.UserId

                              select new { customer, country, city, states, login }).AsEnumerable().Select(row => new
                              {
                                  row.customer.CustomerId,
                                  CustomerName = row.customer.Name,
                                  MobileNo = row.customer.MobileNo,
                                  StateName = row.states.stateName,
                                  CityName = row.city.cityName,
                                  CountryName = row.country.countryName,
                                  row.customer.EmailId,
                                  dob = Convert.ToDateTime(row.customer.dob).ToString("yyyy-MM-dd"),
                                  UserName = row.login.UserName,
                                  Password = row.login.Password
                              });
            if (_search)
            {
                switch (searchField)
                {
                    case "CustomerName":
                        CustomerList = CustomerList.Where(t => t.CustomerName.Contains(searchString));
                        break;
                    case "MobileNo":
                        CustomerList = CustomerList.Where(t => t.MobileNo.Contains(searchString));
                        break;
                    case "EmailId":
                        CustomerList = CustomerList.Where(t => t.EmailId.Contains(searchString));
                        break;
                }
            }

            int totalRecords = CustomerList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            if (sort.ToUpper() == "DESC")
            {
                CustomerList = CustomerList.OrderByDescending(t => t.CustomerId);
                CustomerList = CustomerList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                CustomerList = CustomerList.OrderBy(t => t.CustomerId);
                CustomerList = CustomerList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = CustomerList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        public string EditCustomer(CustomerViewModel Model)
        {

            string msgClient;
            try
            {
                if (ModelState.IsValid)
                {
                    var enditedCustomerValue = new LMS_Datas.Customer
                    {
                        Active = true,
                         CustomerId = Model.CustomerId,
                        Address = Model.Address,
                        EmailId = Model.EmailId,
                        dob = Model.dob,
                        Name = Model.CustomerName,
                        MobileNo = Model.MobileNo
                    };

                    entity.Customers.Attach(enditedCustomerValue);
                    entity.Entry(enditedCustomerValue).Property(x => x.cityId).IsModified = false;
                    entity.Entry(enditedCustomerValue).Property(x => x.stateId).IsModified = false;
                    entity.Entry(enditedCustomerValue).Property(x => x.dob).IsModified = false;
                    entity.Entry(enditedCustomerValue).Property(x => x.gender).IsModified = false;
                    entity.Entry(enditedCustomerValue).Property(x => x.countryId).IsModified = false;
                    entity.Entry(enditedCustomerValue).Property(x => x.Name).IsModified = true;
                    entity.Entry(enditedCustomerValue).Property(x => x.EmailId).IsModified = true;
                    entity.Entry(enditedCustomerValue).Property(x => x.MobileNo).IsModified = true;
                    //entity.Entry(enditedValue).State = EntityState.Modified;
                    entity.SaveChanges();

                    int ?loginid = (from loginId in entity.Logins where loginId.UserId == Model.CustomerId select loginId.UserId).FirstOrDefault();
                    var enditedCustomerLoginValue = new LMS_Datas.Login
                    {

                        UserId = Model.CustomerId,
                        UserName = Model.UserName,
                        Password = Model.Password,
                        Activate = "true",
                        Role="Customer"
                    };

                    entity.Logins.Attach(enditedCustomerLoginValue);
                    entity.Entry(enditedCustomerLoginValue).State = EntityState.Modified;
                    entity.SaveChanges();
                    msgClient = "Saved Successfully";

                }
                else
                {

                    msgClient = "Validation data not successfully";

                }
            }
            catch (Exception ex)
            {
                msgClient = "Error in Retriving Data";
            }
            return msgClient;
        }

        public string DeleteCustomer(int Id)
        {
            string msg;
            if (ModelState.IsValid)
            {
                entity.InsertUpdateSelectCountry(Id, "abc", true, 3);
                //entity.Entry(Model).State = EntityState.Modified;
                //entity.SaveChanges();
                msg = "Deleted Successfully";
            }
            else
            {
                msg = "Validation data not successfully";
            }
            return msg;
        }
    }
}