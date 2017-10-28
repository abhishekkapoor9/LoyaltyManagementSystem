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
    public class AddPointsController : Controller
    {
        LoyaltyManagementSystemEntities entity = new LoyaltyManagementSystemEntities();
        // GET: Admin/AddPoints
        public ActionResult Index()
        {
            ViewBag.alert = "Null";
            return View();
        }
        [HttpPost]
        public ActionResult Index(Point model, FormCollection form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string statusDDLValue = form["Statusddl"].ToString();

                    //var clientId= entity.InsertUpdateSelectAdmin(0, model.name, model.Address, model.MobileNo, model.EmailId, genderDDLValue, model.dob, model.countryId, model.stateId, model.cityId, 1, );
                    using (var context = new LoyaltyManagementSystemEntities())
                    {
                        var points = new LMS_Datas.Point()
                        {
                            Points = model.Points,
                            Amount = model.Amount,
                            Active = Boolean.Parse(statusDDLValue)
                        };
                        context.Points.Add(points);

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
        public JsonResult GetPoints(string sidx, string sort, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
        {
            //EComsDBEntity db = new EComsDBEntity();
            sort = (sort == null) ? "" : sort;
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            var Pointslist = (from point in entity.Points
                               where point.Active == true
                               select new { point.Amount, point.Points, point.PointsId }).AsEnumerable().Select(row => new
                               {
                                   row.PointsId,
                                   row.Amount,
                                   row.Points
                               });
            if (_search)
            {
                switch (searchField)
                {
                    case "Points":
                        Pointslist = Pointslist.Where(t => t.Amount>=(Convert.ToInt32(searchString)));
                        break;
                }
            }
            int totalRecords = Pointslist.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            if (sort.ToUpper() == "DESC")
            {
                Pointslist = Pointslist.OrderByDescending(t => t.Amount);
                Pointslist = Pointslist.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                Pointslist = Pointslist.OrderBy(t => t.Amount);
                Pointslist = Pointslist.Skip(pageIndex * pageSize).Take(pageSize);
            }

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = Pointslist
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public string EditPackage(Point Model)
        {
            string msgClient;
            try
            {
                if (ModelState.IsValid)
                {
                    var enditedPointsValue = new LMS_Datas.Point
                    {
                        Active = true,
                        Points = Model.Points,
                        Amount = Model.Amount,
                        PointsId = Model.PointsId
                    };

                    entity.Points.Attach(enditedPointsValue);
                    entity.Entry(enditedPointsValue).Property(x => x.Active).IsModified = true;
                    entity.Entry(enditedPointsValue).Property(x => x.Points).IsModified = true;
                    entity.Entry(enditedPointsValue).Property(x => x.PointsId).IsModified = true;
                    entity.Entry(enditedPointsValue).Property(x => x.Amount).IsModified = true;
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

        public string DeletePoints(int Id)
        {
            string msg;
            if (ModelState.IsValid)
            {
                entity.InsertUpdateSelectPoints(Id, "abc", 3, true, 3);
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