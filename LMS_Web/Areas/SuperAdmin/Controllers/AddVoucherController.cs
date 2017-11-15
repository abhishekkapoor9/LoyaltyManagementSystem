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
    public class AddVoucherController : Controller
    {
        LoyaltyManagementSystemEntities entity = new LoyaltyManagementSystemEntities();
        // GET: Admin/AddVoucher
        public ActionResult Index()
        {
            ViewBag.PackageDd = new SelectList(entity.Packages.Where(model => model.Active == true), "PackagesId", "PackageName");
            return View();
        }
        [HttpPost]
        public ActionResult Index(Voucher model, FormCollection form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string statusDDLValue = form["Statusddl"].ToString();
                    using (var context = new LoyaltyManagementSystemEntities())
                    {

                        var addVoucher = new LMS_Datas.Voucher()
                        {
                            Activate = true,
                            description = model.description,
                            Discountper = model.Discountper,
                            ValidFrom = model.ValidFrom,
                            ValidTo = model.ValidTo,
                            VoucherName = model.VoucherName,
                            PackagesId = model.PackagesId,
                            persons=model.persons
                        };
                        context.Vouchers.Add(addVoucher);

                        //context.ClientDetails.Add(clientDetails);
                        //etc add your other classes
                        context.SaveChanges();
                    }
                    ViewBag.PackageDd = new SelectList(entity.Packages.Where(models => models.Active == true), "PackagesId", "PackageName");
                    ViewBag.alert = "Success";
                }
            }
            catch (Exception e1)
            {
                ViewBag.alert = "Error";

            }
            return View();
        }
        public JsonResult GetVoucher(string sidx, string sort, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
        {
            //EComsDBEntity db = new EComsDBEntity();
            sort = (sort == null) ? "" : sort;
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            var voucherList = (from voucher in entity.Vouchers
                              where voucher.Activate == true
                              select new { voucher }).AsEnumerable().Select(row => new
                              {
                                  VoucherId = row.voucher.VoucherId,
                                  VoucherName = row.voucher.VoucherName,
                                  ValidTo = Convert.ToDateTime(row.voucher.ValidTo).ToString("yyyy-MM-dd"),
                                  ValidFrom = Convert.ToDateTime(row.voucher.ValidFrom).ToString("yyyy-MM-dd"), 
                                  Description=row.voucher.description,
                                  Persons=row.voucher.persons
                              });
            if (_search)
            {
                switch (searchField)
                {
                    case "Persons":
                        voucherList = voucherList.Where(t => t.Persons==Convert.ToInt32(searchString));
                        break;
                    case "VoucherName":
                        voucherList = voucherList.Where(t => t.VoucherName==(searchString));
                        break;
                }
            }

            int totalRecords = voucherList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            if (sort.ToUpper() == "DESC")
            {
                voucherList = voucherList.OrderByDescending(t => t.VoucherName);
                voucherList = voucherList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                voucherList = voucherList.OrderBy(t => t.VoucherName);
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

        // Admin is Client Here
        public string EditVoucher(Voucher Model)
        {
            string msgClient;
            try
            {
                if (ModelState.IsValid)
                {
                    var enditedVoucherValue = new LMS_Datas.Voucher
                    {
                       Activate=true,
                       VoucherName= Model.VoucherName,
                       description= Model.description,
                       ValidFrom=Model.ValidFrom,
                       ValidTo=Model.ValidTo,
                       VoucherId=Model.VoucherId,
                       persons=Model.persons
                    };

                    entity.Vouchers.Attach(enditedVoucherValue);
                   
                    entity.Entry(enditedVoucherValue).State = EntityState.Modified;
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

        public string DeleteVoucher(int Id)
        {
            string msg;
            try
            {
                if (ModelState.IsValid)
                {
                    var enditedValue = new LMS_Datas.Voucher
                    {
                        VoucherId = Id,
                        Activate = false
                    };
                    entity.Entry(enditedValue).State = EntityState.Modified;
                    entity.Entry(enditedValue).Property(x => x.PackagesId).IsModified = false;
                    entity.Entry(enditedValue).Property(x => x.ValidFrom).IsModified = false;
                    entity.Entry(enditedValue).Property(x => x.ValidTo).IsModified = false;
                    entity.Entry(enditedValue).Property(x => x.VoucherName).IsModified = false;
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