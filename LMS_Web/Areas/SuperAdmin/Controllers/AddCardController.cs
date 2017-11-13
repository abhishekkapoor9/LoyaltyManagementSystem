using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS_Datas;
namespace LMS_Web.Areas.SuperAdmin.Controllers
{
    public class AddCardController : Controller
    {

        LoyaltyManagementSystemEntities entity = new LoyaltyManagementSystemEntities();
        // GET: Admin/AddCard
        public ActionResult Index()
        {
            ViewBag.Packages = new SelectList(entity.Packages.Where(models => models.Active == true), "PackagesId", "PackageName");
            return View();
        }

        [HttpPost]
        public ActionResult Index(Card1 model, FormCollection form)
        {
            try
            {
                ViewBag.Packages = new SelectList(entity.Packages.Where(models => models.Active == true), "PackagesId", "PackageName");
                string statusDDLValue = form["Statusddl"].ToString();
                string packageDDLValue = form["PackagesId"].ToString();
                if (ModelState.IsValid)
                {
                    using (var context = new LoyaltyManagementSystemEntities())
                    {
                        var addCard = new LMS_Datas.Card1()
                        {
                            CardName = model.CardName,
                            persons = model.persons,
                            discountPer = model.discountPer,
                            ValidFrom = model.ValidFrom,
                            ValidTo = model.ValidTo,
                            Activate = Boolean.Parse(statusDDLValue),
                            PackagesId = Convert.ToInt32(packageDDLValue)
                        };
                        context.Card1.Add(addCard);
                        context.SaveChanges();
                        ViewBag.alert = "Success";
                    }
                }
                return View();
            }
            catch (Exception e1)
            {
                ViewBag.alert = "Error";
                return View();
            }
        }

        public JsonResult GetCard(string sidx, string sort, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
        {
            //EComsDBEntity db = new EComsDBEntity();
            sort = (sort == null) ? "" : sort;
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            var Cardslist = (from cards in entity.Card1
                             join packages in entity.Packages
                             on cards.PackagesId equals packages.PackagesId
                             where cards.Activate == true
                             select new { cards, packages }).AsEnumerable().Select(row => new
                             {
                                  CardId = row.cards.CardId,
                                  CardName = row.cards.CardName,
                                  ValidFrom = Convert.ToDateTime(row.cards.ValidFrom).ToString("yyyy-MM-dd"),
                                  ValidTo = Convert.ToDateTime(row.cards.ValidTo).ToString("yyyy-MM-dd"),
                                  discountPer =row.cards.discountPer,
                                  Activate=(row.cards.Activate==true)?"Activate" : "Deactivate",
                                  PackageName=row.packages.PackageName
                              });
            if (_search)
            {
                switch (searchField)
                {
                    case "CardName":
                        Cardslist = Cardslist.Where(t => t.CardName == ((searchString)));
                        break;
                    case "PackageName":
                        Cardslist = Cardslist.Where(t => t.PackageName == ((searchString)));
                        break;
                }
            }
            int totalRecords = Cardslist.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            if (sort.ToUpper() == "DESC")
            {
                Cardslist = Cardslist.OrderByDescending(t => t.CardId);
                Cardslist = Cardslist.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                Cardslist = Cardslist.OrderBy(t => t.CardId);
                Cardslist = Cardslist.Skip(pageIndex * pageSize).Take(pageSize);
            }

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = Cardslist
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
                        DiscountRate = Model.DiscountRate,
                        Persons = Model.Persons,
                        Activate = true,
                        DiscountPerId = Model.DiscountPerId
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
                        DiscountPerId = id,
                        Activate = false

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