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
        public string EditCard(Card1 Model, FormCollection form)
        {
            string msgClient = null;
            string statusDDLValue = form["PackageName"].ToString();
            Model.PackagesId = (from pack in entity.Packages where pack.PackageName == statusDDLValue select pack.PackagesId).FirstOrDefault();
            if (Model.PackagesId != null)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var enditedCardValue = new LMS_Datas.Card1
                        {
                            Activate = true,
                            CardName = Model.CardName,
                            discountPer = Model.discountPer,
                            ValidFrom = Model.ValidFrom,
                            ValidTo = Model.ValidTo,
                            PackagesId = Model.PackagesId
                        };
                        entity.Card1.Attach(enditedCardValue);
                        entity.SaveChanges();
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
            }
            else
            {
                msgClient = "Invalid Package Selected";
            }
            return msgClient;
        }
        public string DeleteCard(int id)
        {
            string msg;
            try
            {
                    var enditedDiscountper = new LMS_Datas.Card1
                    {
                        CardId = id,
                        Activate = false
                    };
                    entity.Card1.Attach(enditedDiscountper);
                    //entity.Entry(enditedDiscountper).Property(x => x.CardId).IsModified = true;
                    entity.Entry(enditedDiscountper).Property(x => x.Activate).IsModified = true;
                    entity.Entry(enditedDiscountper).Property(x => x.CardName).IsModified = false;
                    entity.Entry(enditedDiscountper).Property(x => x.ValidFrom).IsModified = false;
                    entity.Entry(enditedDiscountper).Property(x => x.ValidTo).IsModified = false;
                    entity.Entry(enditedDiscountper).Property(x => x.persons).IsModified = false;
                    entity.Entry(enditedDiscountper).Property(x => x.discountPer).IsModified = false;
                    entity.Entry(enditedDiscountper).Property(x => x.TransactionDetails).IsModified = false;
                    entity.SaveChanges();
                    msg = "Delete Successfully";
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