using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS_Datas;
using System.Data.Entity;

namespace LMS_Web.Areas.SuperAdmin.Controllers
{
    public class DiscountPerController : Controller
    {
        LoyaltyManagementSystemEntities entity = new LoyaltyManagementSystemEntities();
        // GET: SuperAdmin/DiscountPer
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(Discountper model, FormCollection form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string statusDDLValue = form["Statusddl"].ToString();
                    //var clientId= entity.InsertUpdateSelectAdmin(0, model.name, model.Address, model.MobileNo, model.EmailId, genderDDLValue, model.dob, model.countryId, model.stateId, model.cityId, 1, );
                    using (var context = new LoyaltyManagementSystemEntities())
                    {
                        var discountper = new LMS_Datas.Discountper()
                        {
                            Persons = model.Persons,
                            DiscountRate = model.DiscountRate,
                            Activate = Boolean.Parse(statusDDLValue)
                        };
                        context.Discountpers.Add(discountper);
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

        public JsonResult GetDiscount(string sidx, string sort, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
        {
            //EComsDBEntity db = new EComsDBEntity();
            sort = (sort == null) ? "" : sort;
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            var Pointslist = (from discount in entity.Discountpers
                              where discount.Activate== true
                              select new { discount.DiscountRate, discount.Persons,discount.DiscountPerId}).AsEnumerable().Select(row => new
                              {
                                  row.DiscountRate,
                                  row.Persons,
                                  row.DiscountPerId
                              });
            if (_search)
            {
                switch (searchField)
                {
                    case "Persons":
                        Pointslist = Pointslist.Where(t => t.Persons == (Convert.ToInt32(searchString)));
                        break;
                    case "DiscountRate":
                        Pointslist = Pointslist.Where(t => t.DiscountRate == (Convert.ToInt32(searchString)));
                        break;
                }
            }
            int totalRecords = Pointslist.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            if (sort.ToUpper() == "DESC")
            {
                Pointslist = Pointslist.OrderByDescending(t => t.DiscountPerId);
                Pointslist = Pointslist.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                Pointslist = Pointslist.OrderBy(t => t.DiscountPerId);
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

        public string EditDiscount(Discountper Model)
        {
            string msgClient;
            try
            {
                if (ModelState.IsValid)
                {
                    var enditedDiscountValue = new LMS_Datas.Discountper
                    {
                       DiscountRate=Model.DiscountRate,
                       Persons=Model.Persons,
                       Activate=true,
                       DiscountPerId=Model.DiscountPerId
                    };

                    entity.Discountpers.Attach(enditedDiscountValue);
                    entity.Entry(enditedDiscountValue).Property(x => x.DiscountPerId).IsModified = true;
                    entity.Entry(enditedDiscountValue).Property(x => x.Activate).IsModified = true;
                    entity.Entry(enditedDiscountValue).Property(x => x.Persons).IsModified = true;
                    entity.Entry(enditedDiscountValue).Property(x => x.DiscountRate).IsModified = true;
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

        public string DeleteDiscount(int id)
        {
            string msg;
            try
            {
                if (ModelState.IsValid)
                {
                    var enditedDiscountper = new LMS_Datas.Discountper
                    {
                         DiscountPerId= id,
                         Activate=false

                    };
                    entity.Discountpers.Attach(enditedDiscountper);
                    entity.Entry(enditedDiscountper).Property(x => x.DiscountPerId).IsModified = true;
                    entity.Entry(enditedDiscountper).Property(x => x.Activate).IsModified = true;
                    entity.Entry(enditedDiscountper).Property(x => x.DiscountRate).IsModified = false;
                    entity.Entry(enditedDiscountper).Property(x => x.Persons).IsModified = false;
                    //entity.Entry(enditedDiscountper).State = EntityState.Modified;
                    entity.SaveChanges();
                    msg = "Delete Successfully";
                }
                else
                {
                    msg = "Validation data not successfully";
                }
                return msg;
            }
            catch (Exception e1)
            {
                msg = "Validation data not successfully";
                return msg;
            }
        }
    }
}