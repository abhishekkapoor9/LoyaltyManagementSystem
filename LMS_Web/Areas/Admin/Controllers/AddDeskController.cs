using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS_Datas;
using System.Collections;
using System.IO;
using System.Data.Entity;


namespace LMS_Web.Areas.Admin.Controllers
{
    public class AddDeskController : Controller
    {
        LoyaltyManagementSystemEntities entity = new LoyaltyManagementSystemEntities();
        // GET: Admin/AddDesk
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(LMS_Datas.Desk model, FormCollection form, HttpPostedFileBase files)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var context = new LoyaltyManagementSystemEntities())
                    {
                        var addDesk = new LMS_Datas.Desk()
                        {
                           DeskName=model.DeskName,
                           Password=model.Password,
                           DeskNo=model.DeskNo,
                           Active=model.Active,
                           UserName=model.UserName
                        };
                        context.Desks.Add(addDesk);

                        context.SaveChanges();
                       
                        if (files.FileName != null)
                        {
                            var ext = Path.GetExtension(files.FileName);
                            string myfile = addDesk.DeskId + ext;
                            var path = Path.Combine(Server.MapPath("~/Images/DeskProfile"), myfile);
                            files.SaveAs(path);
                        }
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
        public JsonResult GetDesk(string sidx, string sort, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
        {
            //EComsDBEntity db = new EComsDBEntity();
            sort = (sort == null) ? "" : sort;
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            var DeskList = (from desk in entity.Desks
                            join admin in entity.Admins on desk.AdminId equals admin.AdminId
                            where desk.Active == true

                              select new { desk,admin }).AsEnumerable().Select(row => new
                              {
                                  row.desk.DeskId,
                                  DeskName = row.desk.DeskName,
                                  DeskNo = row.desk.DeskNo,
                                  UserName = row.desk.UserName,
                                  Passsword = row.desk.Password,
                                  ClientName= row.admin.Name
                              });
            if (_search)
            {
                switch (searchField)
                {
                    case "DeskName":
                        DeskList = DeskList.Where(t => t.DeskName.Contains(searchString));
                        break;
                    case "ClientName":
                        DeskList = DeskList.Where(t => t.ClientName.Contains(searchString));
                        break;
                    case "DeskNo":
                        DeskList = DeskList.Where(t => t.DeskNo==Convert.ToInt32(searchString));
                        break;
                }
            }

            int totalRecords = DeskList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            if (sort.ToUpper() == "DESC")
            {
                DeskList = DeskList.OrderByDescending(t => t.DeskId);
                DeskList = DeskList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                DeskList = DeskList.OrderBy(t => t.DeskId);
                DeskList = DeskList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = DeskList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public string EditClient(LMS_Datas.Desk Model)
        {
            string msgClient;
            try
            {
                if (ModelState.IsValid)
                {
                    var enditedDeskValue = new LMS_Datas.Desk
                    {
                        Active = true,
                        AdminId = Model.AdminId,
                        DeskName=Model.DeskName,
                        UserName=Model.UserName,
                        Password=Model.Password
                    };

                    entity.Desks.Attach(enditedDeskValue);
                    //entity.Entry(enditedDeskValue).Property(x => x.cityId).IsModified = false;
                    //entity.Entry(enditedDeskValue).Property(x => x.stateId).IsModified = false;
                    //entity.Entry(enditedDeskValue).Property(x => x.dob).IsModified = false;
                    //entity.Entry(enditedDeskValue).Property(x => x.gender).IsModified = false;
                    //entity.Entry(enditedDeskValue).Property(x => x.countryId).IsModified = false;
                    //entity.Entry(enditedDeskValue).Property(x => x.Name).IsModified = true;
                    //entity.Entry(enditedDeskValue).Property(x => x.EmailId).IsModified = true;
                    //entity.Entry(enditedDeskValue).Property(x => x.MobileNo).IsModified = true;
                    //entity.Entry(enditedValue).State = EntityState.Modified;
                    entity.SaveChanges();

                    int? loginid = (from loginId in entity.Logins where loginId.UserId == Model.AdminId select loginId.UserId).FirstOrDefault();
                    var enditedAdminLoginValue = new Login
                    {
                        UserName = Model.UserName,
                        Password = Model.Password,
                        UserId = loginid,
                        Activate = "true"
                    };

                    entity.Logins.Attach(enditedAdminLoginValue);
                    entity.Entry(enditedAdminLoginValue).State = EntityState.Modified;
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

        public string DeleteClient(LMS_Datas.Desk Model)
        {
            string msg;
            //try
            //{
            //    if (ModelState.IsValid)
            //    {
            //        var enditedValue = new LMS_Datas.Admin
            //        {
            //            Active = false,
            //            AdminId = Model.AdminId,
            //            Address = Model.Address,
            //            EmailId = Model.EmailId,
            //            dob = Model.dob
            //        };
            //        entity.Entry(enditedValue).State = EntityState.Modified;
            //        entity.SaveChanges();
            //        msg = "Delete Successfully";
            //    }
            //    else
            //    {
            //        msg = "Validation data not successfully";
            //    }
            //}
            //catch (Exception e1)
            //{
            //    msg = "Validation data not successfully";
            //}
            return "Hello";
        }
    }
}