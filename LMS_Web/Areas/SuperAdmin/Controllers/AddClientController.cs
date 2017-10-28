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
    public class AddClientController : Controller
    {
        LoyaltyManagementSystemEntities entity = new LoyaltyManagementSystemEntities();
        // GET: Admin/AddClient
        public ActionResult Index()
        {
            ViewBag.CountryDd = new SelectList(entity.countries.Where(model => model.Active == true), "countryId", "countryName");
            ViewBag.alert = "Null";
            return View();
        }
        [HttpPost]
        public ActionResult Index(AdminViewModel model, FormCollection form, HttpPostedFileBase files)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string statusDDLValue = form["Statusddl"].ToString();
                    string genderDDLValue = form["genderddl"].ToString();

                    //var clientId= entity.InsertUpdateSelectAdmin(0, model.name, model.Address, model.MobileNo, model.EmailId, genderDDLValue, model.dob, model.countryId, model.stateId, model.cityId, 1, );
                    using (var context = new LoyaltyManagementSystemEntities())
                    {
                        // Client is Admin
                        var admin = new LMS_Datas.Admin()
                        {
                            Name = model.ClientName,
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
                        context.Admins.Add(admin);

                        //context.ClientDetails.Add(clientDetails);
                        //etc add your other classes
                        context.SaveChanges();
                        admin.AdminId = admin.AdminId;

                        var adminLogin = new LMS_Datas.Login ()
                        {
                            LoginId = admin.AdminId,
                            UserName = model.UserName,
                            Role= "Client",
                            Password = model.Password
                        };
                        context.Logins.Add(adminLogin);
                        context.SaveChanges();
                        if (files.FileName != null)
                        {
                            var ext = Path.GetExtension(files.FileName);
                            string myfile = admin.AdminId + ext;
                            var path = Path.Combine(Server.MapPath("~/Images/ClientProfile"), myfile);
                            files.SaveAs(path);
                        }
                    }
                    ViewBag.CountryDd = new SelectList(entity.countries.Where(models => models.Active == true), "countryId", "countryName");
                    ViewBag.alert = "Success";
                    return View();
                }
                ViewBag.CountryDd = new SelectList(entity.countries.Where(models => models.Active == true), "countryId", "countryName");
                ViewBag.alert = "Error";
                return View(model);
            }
            catch (Exception e1)
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

        public JsonResult GetClient(string sidx, string sort, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
        {
            //EComsDBEntity db = new EComsDBEntity();
            sort = (sort == null) ? "" : sort;
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            var ClientList = (from admin in entity.Admins
                              where admin.Active == true
                              join country in entity.countries on admin.countryId equals country.countryId
                              join city in entity.cities on admin.cityId equals city.cityId
                              join states in entity.states on admin.stateId equals states.stateId
                              join login in entity.Logins on admin.AdminId equals login.UserId

                              select new { admin, country, city, states, login }).AsEnumerable().Select(row => new
                              {
                                  row.admin.AdminId,
                                  ClientName = row.admin.Name,
                                  MobileNo = row.admin.MobileNo,
                                  StateName = row.states.stateName,
                                  CityName = row.city.cityName,
                                  CountryName = row.country.countryName,
                                  row.admin.EmailId,
                                  dob = Convert.ToDateTime(row.admin.dob).ToString("yyyy-MM-dd"),
                                  UserName = row.login.UserName,
                                  Password = row.login.Password
                              });
            if (_search)
            {
                switch (searchField)
                {
                    case "ClientName":
                        ClientList = ClientList.Where(t => t.ClientName.Contains(searchString));
                        break;
                    case "MobileNo":
                        ClientList = ClientList.Where(t => t.MobileNo.Contains(searchString));
                        break;
                    case "EmailId":
                        ClientList = ClientList.Where(t => t.EmailId.Contains(searchString));
                        break;
                }
            }

            int totalRecords = ClientList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            if (sort.ToUpper() == "DESC")
            {
                ClientList = ClientList.OrderByDescending(t => t.AdminId);
                ClientList = ClientList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                ClientList = ClientList.OrderBy(t => t.AdminId);
                ClientList = ClientList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = ClientList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        // Admin is Client Here
        public string EditClient(AdminViewModel Model)
        {
            string msgClient;
            try
            {
                if (ModelState.IsValid)
                {
                    var enditedAdminValue = new LMS_Datas.Admin
                    {
                        Active = true,
                        AdminId = Model.AdminId,
                        Address = Model.Address,
                        EmailId = Model.EmailId,
                        dob = Model.dob,
                        Name = Model.ClientName,
                        MobileNo = Model.MobileNo
                    };

                    entity.Admins.Attach(enditedAdminValue);
                    entity.Entry(enditedAdminValue).Property(x => x.cityId).IsModified = false;
                    entity.Entry(enditedAdminValue).Property(x => x.stateId).IsModified = false;
                    entity.Entry(enditedAdminValue).Property(x => x.dob).IsModified = false;
                    entity.Entry(enditedAdminValue).Property(x => x.gender).IsModified = false;
                    entity.Entry(enditedAdminValue).Property(x => x.countryId).IsModified = false;
                    entity.Entry(enditedAdminValue).Property(x => x.Name).IsModified = true;
                    entity.Entry(enditedAdminValue).Property(x => x.EmailId).IsModified = true;
                    entity.Entry(enditedAdminValue).Property(x => x.MobileNo).IsModified = true;
                    //entity.Entry(enditedValue).State = EntityState.Modified;
                    entity.SaveChanges();

                    int ?loginid = (from loginId in entity.Logins where loginId.UserId == Model.AdminId select loginId.UserId).FirstOrDefault();
                    var enditedAdminLoginValue = new Login
                    {
                        UserName = Model.UserName,
                        Password = Model.Password,
                        UserId = loginid,
                        Activate="true"
                    };

                    entity.Logins.Attach(enditedAdminLoginValue);
                    entity.Entry(enditedAdminLoginValue).State = EntityState.Modified;
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

        public string DeleteClient(AdminViewModel Model)
        {
            string msg;
            try
            {
                if (ModelState.IsValid)
                {
                    var enditedValue = new LMS_Datas.Admin
                    {
                        Active = false,
                        AdminId = Model.AdminId,
                        Address = Model.Address,
                        EmailId = Model.EmailId,
                        dob = Model.dob
                    };
                    entity.Entry(enditedValue).State = EntityState.Modified;
                    entity.SaveChanges();
                    msg = "Delete Successfully";
                }
                else
                {
                    msg = "Validation data not successfully";
                }
            }
            catch (Exception e1)
            {
                msg = "Validation data not successfully";
            }
            return msg;
        }
    }
}
