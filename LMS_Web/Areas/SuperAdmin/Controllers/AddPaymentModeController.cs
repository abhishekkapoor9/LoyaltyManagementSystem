using LMS_Datas;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LMS_Web.Areas.SuperAdmin.Controllers
{
    public class AddPaymentModeController : Controller
    {
        LoyaltyManagementSystemEntities entity = new LoyaltyManagementSystemEntities();
        // GET: Admin/AddPaymentMode
        public ActionResult Index()
        {
            ViewBag.alert = "Null";
            return View();
        }

        [HttpPost]
        public ActionResult Index(PayMode model)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    using (var context = new LoyaltyManagementSystemEntities())
                    {
                        var payMode = new LMS_Datas.PayMode()
                        {
                            PayMode1 = model.PayMode1,
                            Active = true
                        };
                        context.PayModes.Add(payMode);
                        context.SaveChanges();
                    }
                }
                ViewBag.alert = "Success";
                return View();
            }
            catch (Exception e1)
            {
                ViewBag.alert = "Error";
                return View();
            }
        }

        public JsonResult GetPayMent(string sidx, string sort, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
        {
            //EComsDBEntity db = new EComsDBEntity();
            sort = (sort == null) ? "" : sort;
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            var paymentList = (from paymode in entity.PayModes
                                where paymode.Active == true
                                select new { paymode.PayMode1, paymode.PayModeId}).AsEnumerable().Select(row => new
                                {
                                 row.PayMode1,
                                   row.PayModeId
                                });
            if (_search)
            {
                switch (searchField)
                {
                    case "PayMode1":
                        paymentList = paymentList.Where(t => t.PayMode1.Contains(searchString));
                        break;
                }
            }

            int totalRecords = paymentList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            if (sort.ToUpper() == "DESC")
            {
                paymentList = paymentList.OrderByDescending(t => t.PayModeId);
                paymentList = paymentList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                paymentList = paymentList.OrderBy(t => t.PayModeId);
                paymentList = paymentList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = paymentList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        public string EditPayment(PayMode Model)
        {

            string msgClient;
            try
            {
                if (ModelState.IsValid)
                {
                    var enditedPaymentValue = new LMS_Datas.PayMode
                    {
                         Active = true,
                         PayModeId=Model.PayModeId,
                         PayMode1=Model.PayMode1
                    };

                    entity.PayModes.Attach(enditedPaymentValue);
                    entity.Entry(enditedPaymentValue).Property(x => x.PayMode1).IsModified = true;
                    entity.Entry(enditedPaymentValue).State = EntityState.Modified;
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

        public string DeletePayment(int Id)
        {
            string msg;
            if (ModelState.IsValid)
            {
                entity.InsertUpdateSelectPaymode(Id, "abc", 3,true);
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