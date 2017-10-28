using LMS_Datas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS_Web.App_Data;
using System.Text;
using System.Data.Entity;

namespace LMS_Web.Areas.SuperAdmin.Controllers
{
    public class TransferVoucherController : Controller
    {
        int imageInt = 0;
        LoyaltyManagementSystemEntities entity = new LoyaltyManagementSystemEntities();
        StringBuilder builder = new StringBuilder();
        Sms sms = new Sms();
        mail Mail = new mail();
        // GET: Admin/TransferVoucher
        public ActionResult Index()
        {
            ViewBag.TransferTo = new SelectList(entity.Customers.Where(model => model.Active == true), "CustomerId", "Name");
            ViewBag.TransferFrom = new SelectList(entity.Customers.Where(model => model.Active == true), "CustomerId", "Name");
            ViewBag.VoucherDd = new SelectList(entity.Vouchers.Where(model => model.Activate == true), "VoucherId", "VoucherName");
            return View();
        }
        [HttpPost]
        public ActionResult Index(TransferVoucher model)
        {
            try
            {
                ViewBag.TransferTo = new SelectList(entity.Customers.Where(models => models.Active == true), "CustomerId", "Name");
                ViewBag.TransferFrom = new SelectList(entity.Customers.Where(models => models.Active == true), "CustomerId", "Name");
                ViewBag.VoucherDd = new SelectList(entity.Vouchers.Where(models => models.Activate == true), "VoucherId", "VoucherName");
                if (ModelState.IsValid)
                {
                    using (var context = new LoyaltyManagementSystemEntities())
                    {
                        var transferVoucher = new LMS_Datas.TransferVoucher()
                        {
                            TransferFromCustomerID = model.TransferFromCustomerID,
                            TransferToCustomerID = model.TransferToCustomerID,
                            VoucherID = model.VoucherID,
                            TranferDate = DateTime.Now
                        };
                        context.TransferVouchers.Add(transferVoucher);
                        context.SaveChanges();
                        ViewBag.alert = "Success";
                        string TransferFrom = (from e in entity.Customers where e.CustomerId == model.TransferFromCustomerID select e.Name).First().ToString();
                        string TransferTo = (from e in entity.Customers where e.CustomerId == model.TransferToCustomerID select e.Name).First().ToString();
                        string vouchername = (from e in entity.Vouchers where e.VoucherId == model.VoucherID select e.VoucherName).First().ToString();
                        string voucherno = (from e in entity.CustomerVouchers where e.VoucherId == model.VoucherID select e.AssignNo).First().ToString();
                        string mobileNumber = (from e in entity.Customers where e.CustomerId == model.TransferToCustomerID select e.MobileNo).First().ToString();
                        mobileNumber = mobileNumber.Replace(")", String.Empty);
                        mobileNumber = mobileNumber.Replace("-", String.Empty);
                        mobileNumber = mobileNumber.Replace(" ", String.Empty);
                        builder.Append("Hello " + TransferTo + ",").AppendLine();
                        builder.Append("Voucher " + vouchername + " is Transferred to you by " +TransferFrom+ ". Use Voucher No. " + voucherno + " for further references.").AppendLine();

                        builder.Append("Thanks");
                        //sms.send(mobileNumber, builder);
                        sms.send("8285601519", builder);
                        Mail.send(builder, "abhishekkpr9@gmail.com", "abhishekkpr9@gmail.com", "Voucher Assigned");
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
        [HttpPost]
        public JsonResult GetTransferFrom(string customerId)
        {
            string path= Server.MapPath("../../Images/ClientProfile/");
            string[] filePaths = Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories);
            
            foreach (var filepath in filePaths)
            {
                string[] filename = filepath.Split('\\');
                if (filename[(filename.Length)-1].Equals(customerId+".jpg"))
                {
                    imageInt = 1;
                    break;
                }
                else
                {
                    continue;
                }
            }
            int cusid = Convert.ToInt32(customerId);
            
            var transferTo = entity.Customers.Where(y=>y.CustomerId== cusid).Select(x => new
            {
                Id=x.CustomerId,
                Name = x.Name,
                Address = x.Address,
                MobileNo = x.MobileNo,
                ImageInt=imageInt
            }).ToList();
            ViewBag.header = "Transfer From Customer";
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

        [HttpPost]
        public JsonResult GetVoucher(string voucherId)
        {
            int voucherid = Convert.ToInt32(voucherId);
            var transferTo = entity.Vouchers.Where(y => y.VoucherId == voucherid).AsEnumerable().Select(x => new
            {
                Id = x.VoucherId,
                VoucherName = x.VoucherName,
                ValidFrom = Convert.ToDateTime(x.ValidFrom).ToString("yyyy-MM-dd"),
                ValidTo = Convert.ToDateTime(x.ValidTo).ToString("yyyy-MM-dd"),
                Description = x.description
            }).ToList();
            ViewBag.header = "Transfer To Customer";
            return Json(transferTo, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVoucherClient(string sidx, string sort, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
        {
            //EComsDBEntity db = new EComsDBEntity();
            sort = (sort == null) ? "" : sort;
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            var VoucherClientList = (from transfer in entity.TransferVouchers
                                     join voucher in entity.Vouchers on transfer.VoucherID equals voucher.VoucherId
                                     join transferTo in entity.Customers on transfer.TransferToCustomerID equals transferTo.CustomerId
                                     join transferFrom in entity.Customers on transfer.TransferFromCustomerID equals transferFrom.CustomerId
                                     select new { transferTo, transferFrom, voucher, transfer }).AsEnumerable().Select(row => new
                              {
                                  row.transfer.TransferVoucherID,
                                  VoucherName = row.voucher.VoucherName,
                                  ClientFrom = row.transferFrom.Name,
                                  ClientTo = row.transferTo.Name,
                                  ValidFrom = Convert.ToDateTime(row.voucher.ValidFrom).ToString("yyyy-MM-dd"),
                                  ValidTo = Convert.ToDateTime(row.voucher.ValidTo).ToString("yyyy-MM-dd")
                              });
            if (_search)
            {
                switch (searchField)
                {
                    case "VoucherName":
                        VoucherClientList = VoucherClientList.Where(t => t.VoucherName.Contains(searchString));
                        break;
                    case "ClientFrom":
                        VoucherClientList = VoucherClientList.Where(t => t.ClientFrom.Contains(searchString));
                        break;
                    case "ClientTo":
                        VoucherClientList = VoucherClientList.Where(t => t.ClientTo.Contains(searchString));
                        break;
                }
            }

            int totalRecords = VoucherClientList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            if (sort.ToUpper() == "DESC")
            {
                VoucherClientList = VoucherClientList.OrderByDescending(t => t.TransferVoucherID);
                VoucherClientList = VoucherClientList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                VoucherClientList = VoucherClientList.OrderBy(t => t.TransferVoucherID);
                VoucherClientList = VoucherClientList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = VoucherClientList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        // Admin is Client Here
        //public string EditClient(TransferVoucher Model)
        //{
        //    string msgClient;
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var enditedAdminValue = new LMS_Datas.TransferVoucher
        //            {
        //                Active = true,
        //                AdminId = Model.AdminId,
        //                Address = Model.Address,
        //                EmailId = Model.EmailId,
        //                dob = Model.dob,
        //                Name = Model.ClientName,
        //                MobileNo = Model.MobileNo
        //            };

        //            entity.Admins.Attach(enditedAdminValue);
        //            entity.Entry(enditedAdminValue).Property(x => x.cityId).IsModified = false;
        //            entity.Entry(enditedAdminValue).Property(x => x.stateId).IsModified = false;
        //            entity.Entry(enditedAdminValue).Property(x => x.dob).IsModified = false;
        //            entity.Entry(enditedAdminValue).Property(x => x.gender).IsModified = false;
        //            entity.Entry(enditedAdminValue).Property(x => x.countryId).IsModified = false;
        //            entity.Entry(enditedAdminValue).Property(x => x.Name).IsModified = true;
        //            entity.Entry(enditedAdminValue).Property(x => x.EmailId).IsModified = true;
        //            entity.Entry(enditedAdminValue).Property(x => x.MobileNo).IsModified = true;
        //            //entity.Entry(enditedValue).State = EntityState.Modified;
        //            entity.SaveChanges();

        //            int? loginid = (from loginId in entity.Logins where loginId.UserId == Model.AdminId select loginId.UserId).FirstOrDefault();
        //            var enditedAdminLoginValue = new Login
        //            {
        //                UserName = Model.UserName,
        //                Password = Model.Password,
        //                UserId = loginid,
        //                Activate = "true"
        //            };

        //            entity.Logins.Attach(enditedAdminLoginValue);
        //            entity.Entry(enditedAdminLoginValue).State = EntityState.Modified;
        //            entity.SaveChanges();
        //            msgClient = "Saved Successfully";
        //        }
        //        else
        //        {
        //            msgClient = "Validation data not successfully";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        msgClient = "Error in Retriving Data";
        //    }
        //    return msgClient;
        //}

        //public string DeleteClient(AdminViewModel Model)
        //{
        //    string msg;
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var enditedValue = new LMS_Datas.Admin
        //            {
        //                Active = false,
        //                AdminId = Model.AdminId,
        //                Address = Model.Address,
        //                EmailId = Model.EmailId,
        //                dob = Model.dob
        //            };
        //            entity.Entry(enditedValue).State = EntityState.Modified;
        //            entity.SaveChanges();
        //            msg = "Delete Successfully";
        //        }
        //        else
        //        {
        //            msg = "Validation data not successfully";
        //        }
        //    }
        //    catch (Exception e1)
        //    {
        //        msg = "Validation data not successfully";
        //    }
        //    return msg;
        //}
    }
}