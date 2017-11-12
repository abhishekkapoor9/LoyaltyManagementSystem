using LMS_Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS_Web.App_Data;
using System.Text;
using System.Data.Entity;

namespace LMS_Web.Areas.SuperAdmin.Controllers
{
    public class AssignVoucherController : Controller
    {
        Sms sms = new Sms();
        mail Mail = new mail();
        LoyaltyManagementSystemEntities entity = new LoyaltyManagementSystemEntities();
        StringBuilder builder = new StringBuilder();
        // GET: Admin/AssignVoucher
        public ActionResult Index()
        {
            ViewBag.CustomerId = new SelectList(entity.Customers.Where(model => model.Active == true), "CustomerId", "Name");
            ViewBag.VoucherId = new SelectList(entity.Vouchers.Where(model => model.Activate == true), "VoucherId", "VoucherName");
            //sms.send("8826848969", "abc");
            return View();
        }

        [HttpPost]
        public ActionResult Index(CustomerVoucher model, FormCollection form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string customerName = (from e in entity.Customers where e.CustomerId == model.CustomerId select  e.Name ).First().ToString();
                    string mobileNumber = (from e in entity.Customers where e.CustomerId == model.CustomerId select  e.MobileNo ).First().ToString();
                    mobileNumber= mobileNumber.Replace("(", String.Empty);
                    mobileNumber = mobileNumber.Replace(")", String.Empty);
                    mobileNumber = mobileNumber.Replace("-", String.Empty);
                    mobileNumber = mobileNumber.Replace(" ", String.Empty);
                    string vouchername = (from e in entity.Vouchers where e.VoucherId == model.VoucherId select  e.VoucherName ).First().ToString();
                    string mailId = (from e in entity.Customers where e.CustomerId == model.CustomerId select e.EmailId).First().ToString();
                    using (var context = new LoyaltyManagementSystemEntities())
                    {
                        var addCustomerVoucher = new LMS_Datas.CustomerVoucher()
                        {
                            CustomerId = model.CustomerId,
                            VoucherId = model.VoucherId,
                            AssignNo = model.AssignNo,
                            AssignOn = DateTime.Now,
                            Active=true
                        };
                        context.CustomerVouchers.Add(addCustomerVoucher);

                        //context.ClientDetails.Add(clientDetails);
                        //etc add your other classes
                        context.SaveChanges();
                    }
                    ViewBag.CustomerId = new SelectList(entity.Customers.Where(models => models.Active == true), "CustomerId", "Name");
                    ViewBag.VoucherId = new SelectList(entity.Vouchers.Where(models => models.Activate == true), "VoucherId", "VoucherName");
                    ViewBag.alert = "Success";
                    builder.Append("Hello " + customerName +",").AppendLine();
                    builder.Append("Voucher " + vouchername + " is assigned to you. Use Voucher No. " + model.AssignNo + " for further references.").AppendLine();
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

        public JsonResult GetCustomerVoucher(string sidx, string sort, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
        {
            //EComsDBEntity db = new EComsDBEntity();
            sort = (sort == null) ? "" : sort;
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            var voucherList = (from customervoucher in entity.CustomerVouchers
                              
                               join customer in entity.Customers on customervoucher.CustomerId equals customer.CustomerId
                               join voucher in entity.Vouchers on customervoucher.VoucherId equals voucher.VoucherId
                               select new { voucher, customer, customervoucher }).AsEnumerable().Select(row => new
                               {
                                   CustomerVoucherId = row.customervoucher.CustomerVoucherId,
                                   VoucherName = row.voucher.VoucherName,
                                   CustomerName=row.customer.Name,
                                   VoucherNo=row.customervoucher.AssignNo,
                                   Status=(row.customervoucher.Active==true)?"Active":"Inactive"
                               });
            if (_search)
            {
                switch (searchField)
                {
                    case "CustomerName":
                        voucherList = voucherList.Where(t => t.CustomerName.Contains(searchString));
                        break;
                    case "VoucherName":
                        voucherList = voucherList.Where(t => t.VoucherName == (searchString));
                        break;
                }
            }

            int totalRecords = voucherList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            if (sort.ToUpper() == "DESC")
            {
                voucherList = voucherList.OrderByDescending(t => t.CustomerName);
                voucherList = voucherList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                voucherList = voucherList.OrderBy(t => t.CustomerName);
                voucherList = voucherList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = voucherList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        
        public string EditCustomerVoucher(CustomerVoucher Model,FormCollection form)
        {
            string statusDDLValue = form["VoucherName"].ToString();
            string msgClient;
            try
            {
                if (ModelState.IsValid)
                {
                    var enditedCustomerVoucherValue = new LMS_Datas.CustomerVoucher
                    {
                       Active=true,
                       CustomerId=Model.CustomerId,
                       VoucherId=Model.VoucherId,
                       AssignNo=Model.AssignNo

                    };

                    entity.CustomerVouchers.Attach(enditedCustomerVoucherValue);

                    entity.Entry(enditedCustomerVoucherValue).State = EntityState.Modified;
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

        public string DeleteCustomerVoucher(Voucher Model)
        {
            string msg;
            try
            {
                if (ModelState.IsValid)
                {
                    var enditedValue = new LMS_Datas.Voucher
                    {
                        Activate = false,
                        VoucherId = Model.VoucherId,
                        VoucherName = Model.VoucherName,
                        ValidFrom = Model.ValidFrom,
                        ValidTo = Model.ValidTo,
                        description = Model.description
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