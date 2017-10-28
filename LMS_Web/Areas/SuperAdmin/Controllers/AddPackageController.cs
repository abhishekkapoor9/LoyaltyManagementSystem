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
   
    public class AddPackageController : Controller
    {
        LoyaltyManagementSystemEntities entity = new LoyaltyManagementSystemEntities();
        // GET: Admin/AddPackage
        public ActionResult Index()
        {
            ViewBag.alert = "Null";
            return View();
        }

        [HttpPost]
        public ActionResult Index(Package model, FormCollection form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string statusDDLValue = form["Statusddl"].ToString();

                    //var clientId= entity.InsertUpdateSelectAdmin(0, model.name, model.Address, model.MobileNo, model.EmailId, genderDDLValue, model.dob, model.countryId, model.stateId, model.cityId, 1, );
                    using (var context = new LoyaltyManagementSystemEntities())
                    {
                        var package = new LMS_Datas.Package()
                        {
                           PackageName=model.PackageName,
                           Description=model.Description,
                            Active = Boolean.Parse(statusDDLValue)
                        };
                        context.Packages.Add(package);

                        //context.ClientDetails.Add(clientDetails);
                        //etc add your other classes
                        context.SaveChanges();
                    }
                    ViewBag.alert = "Success";
                    return View();
                }
                ViewBag.alert = "Error";
                return View(model);
            }
            catch (Exception e1)
            {
                ViewBag.alert = "Error";
                return View();
            }
        }
        public JsonResult GetPackage(string sidx, string sort, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
        {
            //EComsDBEntity db = new EComsDBEntity();
            sort = (sort == null) ? "" : sort;
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            var PackageList = (from package in entity.Packages
                               where package.Active == true
                               select new { package.PackageName, package.Description,package.PackagesId }).AsEnumerable().Select(row => new
                               {
                                   row.PackagesId,
                                   row.Description,
                                   row.PackageName
                               });
            if (_search)
            {
                switch (searchField)
                {
                    case "PackageName":
                        PackageList = PackageList.Where(t => t.PackageName.Contains(searchString));
                        break;
                }
            }
            int totalRecords = PackageList.Count();
                var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
                if (sort.ToUpper() == "DESC")
                {
                    PackageList = PackageList.OrderByDescending(t => t.PackageName);
                    PackageList = PackageList.Skip(pageIndex * pageSize).Take(pageSize);
                }
                else
                {
                    PackageList = PackageList.OrderBy(t => t.PackageName);
                    PackageList = PackageList.Skip(pageIndex * pageSize).Take(pageSize);
                }
            
                var jsonData = new
                {
                    total = totalPages,
                    page,
                    records = totalRecords,
                    rows = PackageList
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }

        public string EditPackage(Package Model)
        {
            string msgClient;
            try
            {
                if (ModelState.IsValid)
                {
                    var enditedPackageValue = new LMS_Datas.Package
                    {
                        Active = true,
                       PackageName=Model.PackageName,
                       Description=Model.Description,
                       PackagesId=Model.PackagesId
                    };

                    entity.Packages.Attach(enditedPackageValue);
                    entity.Entry(enditedPackageValue).Property(x => x.Active).IsModified = true;
                    entity.Entry(enditedPackageValue).Property(x => x.PackagesId).IsModified = true;
                    entity.Entry(enditedPackageValue).Property(x => x.Description).IsModified = true;
                    entity.Entry(enditedPackageValue).Property(x => x.PackageName).IsModified = true;
                    //entity.Entry(enditedPackageValue).State = EntityState.Modified;
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

        public string DeletePackage(int Id)
        {
            string msg;
            if (ModelState.IsValid)
            {
                entity.InsertUpdateSelectPackage(Id, "abc",3,true,"ann");
                //entity.Entry(Model).State = EntityState.Modified;
                entity.SaveChanges();
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