using LMS_Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS_Web.App_Data;
using System.Text;
using System.Data.Entity;
using System.IO;

namespace LMS_Web.Areas.SuperAdmin.Controllers
{
    public class AssignCardController : Controller
    {
        LoyaltyManagementSystemEntities entity = new LoyaltyManagementSystemEntities();
        StringBuilder builder = new StringBuilder();
        Sms sms = new Sms();
        mail Mail = new mail();
        // GET: Admin/AssignCard
        public ActionResult Index()
        {
            ViewBag.CustomerId = new SelectList(entity.Customers.Where(model => model.Active == true), "CustomerId", "Name");
            ViewBag.CardId = new SelectList(entity.Card1.Where(model => model.Activate == true), "Cardid", "CardName");
            ViewBag.alert = "";
            return View();
        }
        [HttpPost]
        public ActionResult Index(CustomerCard model, FormCollection form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string customerName = (from e in entity.Customers where e.CustomerId == model.CustomerId select e.Name).First().ToString();
                    string AssignNo = form["Card Code"].ToString();
                    string mobileNumber = (from e in entity.Customers where e.CustomerId == model.CustomerId select e.MobileNo).First().ToString();
                    mobileNumber = mobileNumber.Replace("(", String.Empty);
                    mobileNumber = mobileNumber.Replace(")", String.Empty);
                    mobileNumber = mobileNumber.Replace("-", String.Empty);
                    mobileNumber = mobileNumber.Replace(" ", String.Empty);
                    string cardname = (from e in entity.Card1 where e.CardId == model.Cardid select e.CardName).First().ToString();
                    string mailId = (from e in entity.Customers where e.CustomerId == model.CustomerId select e.EmailId).First().ToString();
                    using (var context = new LoyaltyManagementSystemEntities())
                    {
                        var addCustomerVoucher = new LMS_Datas.CustomerCard()
                        {
                            CustomerId = model.CustomerId,
                            Cardid = model.Cardid,
                            referenceNo=AssignNo,
                            Activate=true
                        };
                        context.CustomerCards.Add(addCustomerVoucher);

                        //context.ClientDetails.Add(clientDetails);
                        //etc add your other classes
                        context.SaveChanges();
                    }
                    ViewBag.CustomerId = new SelectList(entity.Customers.Where(models => models.Active == true), "CustomerId", "Name");
                    ViewBag.CardId = new SelectList(entity.Card1.Where(models => models.Activate == true), "CardId", "CardName");
                    ViewBag.alert = "Success";
                    builder.Append("Hello " + customerName + ",").AppendLine();
                    builder.Append("Voucher " + cardname + " is assigned to you. Use Reference No. " + AssignNo + " for further references.").AppendLine();
                    builder.Append("Thanks");
                    //sms.send(mobileNumber, builder);
                    sms.send("8285601519", builder);
                    Mail.send(builder, "abhishekkpr9@gmail.com", "abhishekkpr9@gmail.com", "Voucher Assigned");
                }
            }
            catch (Exception e1)
            {
                ViewBag.alert = "Error";

            }
            return View();
        }

        public JsonResult GetCardDetails(int cardId)
        {
            var transferTo = entity.Card1.Where(y => y.CardId == cardId).AsEnumerable().Select(x => new
            {
                Id = x.CardId,
                Name = x.CardName,
                ValidFrom = Convert.ToDateTime(x.ValidFrom).ToString("yyyy-MM-dd"),
                ValidTo = Convert.ToDateTime(x.ValidTo).ToString("yyyy-MM-dd"),
                Discount=(x.discountPer).ToString()
            }).ToList();
            return Json(transferTo, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetTransferTo(string customerId)
        {
            string path = Server.MapPath("../../Images/ClientProfile/");
            string[] filePaths = Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories);

            foreach (var filepath in filePaths)
            {
                string[] filename = filepath.Split('\\');
                if (filename[(filename.Length) - 1].Equals(customerId + ".jpg"))
                {
                    break;
                }
                else
                {
                    continue;
                }
            }
            int cusid = Convert.ToInt32(customerId);
            var transferTo = entity.Customers.Where(y => y.CustomerId == cusid).Select(x => new
            {
                Id = x.CustomerId,
                Name = x.Name,
                Address = x.Address,
                MobileNo = x.MobileNo
            }).ToList();
            ViewBag.header = "Transfer To Customer";
            return Json(transferTo, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPayMent(string sidx, string sort, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
        {
            //EComsDBEntity db = new EComsDBEntity();
            sort = (sort == null) ? "" : sort;
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            var cardCustomerList = (from paymode in entity.CustomerCards
                               join  cus in entity.Customers
                               on paymode.CustomerId equals cus.CustomerId
                               join card in entity.Card1
                               on paymode.Cardid equals card.CardId
                               where paymode.Activate==true
                               select new { cus,card, paymode }).AsEnumerable().Select(row => new
                               {
                                  CardCustomerId=row.paymode.CustomerCardId,
                                  CustomerName= row.cus.Name,
                                  CardName= row.card.CardName,
                                  ReferenceNo=row.paymode.referenceNo,
                                  Activate=(row.paymode.Activate==true)?"Activate":"Deactivate"
                               });
            if (_search)
            {
                switch (searchField)
                {
                    case "CardName":
                        cardCustomerList = cardCustomerList.Where(t => t.CardName.Contains(searchString));
                        break;
                    case "CustomerName":
                        cardCustomerList = cardCustomerList.Where(t => t.CustomerName.Contains(searchString));
                        break;
                }
            }

            int totalRecords = cardCustomerList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            if (sort.ToUpper() == "DESC")
            {
                cardCustomerList = cardCustomerList.OrderByDescending(t => t.CardName);
                cardCustomerList = cardCustomerList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                cardCustomerList = cardCustomerList.OrderBy(t => t.CardName);
                cardCustomerList = cardCustomerList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = cardCustomerList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        public string EditCustomerCard(CustomerCard Model, FormCollection form)
        {
            string CustomerName = form["CustomerName"].ToString();
            string CardName = form["CardName"].ToString();
            string referenceNo = form["ReferenceNo"].ToString();
            int id =Convert.ToInt16(form["id"].ToString());
            int customerId = (from customer in entity.Customers where customer.Name == CustomerName select customer.CustomerId).FirstOrDefault();
            int CardId = (from card in entity.Card1 where card.CardName == CardName select card.CardId).FirstOrDefault();
            string msgClient=null;
            if (customerId != 0 && CardId != 0)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var enditedCustomerCardValue = new LMS_Datas.CustomerCard
                        {
                            CustomerId = customerId,
                            Cardid = CardId,
                            referenceNo = referenceNo,
                            CustomerCardId=id
                        };

                        entity.CustomerCards.Attach(enditedCustomerCardValue);
                        entity.Entry(enditedCustomerCardValue).State = EntityState.Modified;
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
            }
            else
            {
                msgClient = "Error in Retriving Data";
            }
            return msgClient;
        }

        public string DeleteCustomerCard(int Id)
        {
            string msg;
            try
            {
                if (ModelState.IsValid)
                {
                    var enditedCustomerCardDiscountper = new LMS_Datas.CustomerCard
                    {
                        CustomerCardId = Id,
                        Activate = false
                    };
                    entity.CustomerCards.Attach(enditedCustomerCardDiscountper);
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
            return msg;
        }
    }
}