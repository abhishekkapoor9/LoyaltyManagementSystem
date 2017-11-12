using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS_Datas;
using LMS_Web.ViewModel;
using System.Data.Entity;
using System.Web.Script.Serialization;
using System.Collections;

namespace LMS_Web.Areas.SuperAdmin.Controllers
{
    public class AddCountryController : Controller
    {
        LoyaltyManagementSystemEntities entity = new LoyaltyManagementSystemEntities();
        // GET: Admin/AddCountry
        public ActionResult Index()
           
        {
            ViewBag.CountryDd = new SelectList(entity.countries.Where(models => models.Active == true), "countryId", "countryName");
            ViewBag.alert = "Null";
            return View();
        }
        [HttpPost]
        public ActionResult Index(CountryViewModel model, string submit_Country, string submit_State, string submit_City)
        {
            try
            {
                if (submit_Country != (null))
                {
                    entity.InsertUpdateSelectCountry(0, model.countryName, true, 1);
                }
                else if (submit_State != (null))
                {

                    entity.InsertUpdateSelectState(0, model.stateName, true, 1, model.countryId);
                }
                else if (submit_City != (null))
                {

                    entity.InsertUpdateSelectCity(0, model.cityName, true, 1, model.countryId2, model.stateId);
                }

                ViewBag.CountryDd = new SelectList(entity.countries.Where(models => models.Active == true), "countryId", "countryName");
                ViewBag.stateDd = new SelectList(entity.states.Where(models => models.Active == true), "stateId", "stateName");
                ViewBag.cityDd = new SelectList(entity.cities.Where(models => models.Active == true), "cityId", "cityName");
                ViewBag.alert = "Success";
            } 
            catch(Exception e1)
            {
                ViewBag.alert = "Error";

            }
            return View();
        }

        public JsonResult GetCountry(string sidx, string sort, int page, int rows)
        {
            //EComsDBEntity db = new EComsDBEntity();
            sort = (sort == null) ? "" : sort;
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            var CountryList = entity.countries.AsEnumerable().Where(w => w.Active == true).Select((x, index)=>
                     new
                    {
                        index = index + 1,
                        x.countryId,
                        x.countryName
                    });
            int totalRecords = CountryList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            if (sort.ToUpper() == "DESC")
            {
                CountryList = CountryList.OrderByDescending(t => t.countryId);
                CountryList = CountryList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                CountryList = CountryList.OrderBy(t => t.countryId);
                CountryList = CountryList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = CountryList
            };
            //ViewBag.CountryDd = new SelectList(entity.countries.Where(models => models.Active == true), "countryId", "countryName");
            ////ViewBag.stateDd = new SelectList(entity.states.Where(models => models.Active == true), "stateId", "stateName");
            ////ViewBag.cityDd = new SelectList(entity.cities.Where(models => models.Active == true), "cityId", "cityName");
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        public string EditCountry(country Model)
        {
           
            string msg;
            try
            {
                if (ModelState.IsValid)
                {
                    entity.Entry(Model).State = EntityState.Modified;
                    entity.SaveChanges();
                    msg = "Saved Successfully";
                }
                else
                {
                    msg = "Validation data not successfully";
                }
            }
            catch (Exception ex)
            {
                msg = "Error occured:" + ex.Message;
            }
            return msg;
        }

        public string DeleteCountry(int Id)
        {
            string msg;
            if (ModelState.IsValid)
            {
                entity.InsertUpdateSelectCountry(Id, "abc", true, 3);
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

        public JsonResult GetState(string sidx, string sort, int page, int rows)
        {
            //EComsDBEntity db = new EComsDBEntity();
            sort = (sort == null) ? "" : sort;
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            var StateList = entity.states.AsEnumerable().Where(w => (w.Active == true)).Select((x, index) =>
                    new {
                        index = index + 1,
                        x.country.countryName,
                        x.stateId,
                        x.stateName
                    });
            //var StateLists = entity.states.GroupJoin(entity.countries, std => std.countryId, //outerKeySelector 
            //                      s => s.countryId, (x, index) => new
            //                      {
            //                                         x.country.countryName,
            //                          x.stateId,
            //                          x.stateName
            //                      });


            int totalRecords = StateList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            if (sort.ToUpper() == "DESC")
            {
                StateList = StateList.OrderByDescending(t => t.stateId);
                StateList = StateList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                StateList = StateList.OrderBy(t => t.stateId);
                StateList = StateList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = StateList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCity(string sidx, string sort, int page, int rows)
        {
            //EComsDBEntity db = new EComsDBEntity();
            sort = (sort == null) ? "" : sort;
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            var CityList = entity.cities.AsEnumerable().Where(w=>(w.Active==true)).Select((x, index) =>
                    new {
                       index= index+1,
                        x.country.countryName,
                        x.state.stateName,
                        x.cityId,
                        x.cityName
                    });
            int totalRecords = CityList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            if (sort.ToUpper() == "DESC")
            {
                CityList = CityList.OrderByDescending(t => t.cityId);
                CityList = CityList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                CityList = CityList.OrderBy(t => t.cityId);
                CityList = CityList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = CityList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetStateDdl(string countryId)
        {
            //List<city> lstcity = new List<city>();
            int id = Convert.ToInt32(countryId);
            using (LoyaltyManagementSystemEntities db = new LoyaltyManagementSystemEntities())
            {
                IEnumerable lststate = (db.states.Where(x => x.countryId == id)).Select(o => new
                {
                    Stateid = o.stateId,
                    StateName = o.stateName
                }).ToList();
                
                return Json(lststate, JsonRequestBehavior.AllowGet);
            }
           
        }
        public string EditState(CountryViewModel Model)
        {
            string msg;
            
            int? countryId = (from id in entity.countries  where id.countryName == Model.countryName select id.countryId).FirstOrDefault();
            
            try
            {
                if (countryId != 0 && ModelState.IsValid)
                {
                    entity.InsertUpdateSelectState(Model.stateId, Model.stateName,true, 2,Convert.ToInt32(countryId));
                    //entity.Entry(Model).State = EntityState.Modified;
                    //entity.SaveChanges();
                    msg = "Saved Successfully";
                }
                else
                {
                    msg = "Validation data not successfully";
                }
            }
            catch (Exception ex)
            {
                msg = "Error occured:" + ex.Message;
            }
            return msg;
        }

        public string DeleteState(int Id)
        {
            string msg;
            if (ModelState.IsValid)
            {
                entity.InsertUpdateSelectState(Id,"abc",true,3,1);
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

        public string EditCity(CountryViewModel Model)
        {
            string msg;

            int? countryId = (from id in entity.countries where id.countryName == Model.countryName select id.countryId).FirstOrDefault();
            int? stateId = (from id in entity.states where id.stateName == Model.stateName select id.stateId).FirstOrDefault();

            try
            {
                if (countryId != 0 && stateId != 0 && ModelState.IsValid)
                {
                    entity.InsertUpdateSelectCity(Model.cityId, Model.cityName, true, 2, Convert.ToInt32(countryId), Convert.ToInt32(stateId));
                    //entity.Entry(Model).State = EntityState.Modified;
                    //entity.SaveChanges();
                    msg = "Edit Successfully";
                }
                else
                {
                    msg = "Validation data not successfully";
                }
            }
            catch (Exception ex)
            {
                msg = "Error occured:" + ex.Message;
            }
            return msg;
        }

        public string DeleteCity(int Id)
        {
            string msg;
            if (ModelState.IsValid)
            {
                entity.InsertUpdateSelectCity(Id,"abc", true, 2, 3, 4);
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