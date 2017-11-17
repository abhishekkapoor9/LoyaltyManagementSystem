using LMS_Datas;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace LMS_Web.Areas.SuperAdmin.Controllers
{
    public class RechargeController : Controller
    {
        LoyaltyManagementSystemEntities entity = new LoyaltyManagementSystemEntities();
        // GET: SuperAdmin/Recharge
        public ActionResult Index()
        {
            ViewBag.Payment = new SelectList(entity.PayModes.Where(model => model.Active == true), "PayModeId", "PayMode1");
            return View();
        }

        public JsonResult GetStateDdl(string cardNo)
        {
            //List<city> lstcity = new List<city>();
            int id = Convert.ToInt32(cardNo);
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
    }
}