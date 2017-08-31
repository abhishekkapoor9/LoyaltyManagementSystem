using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LMS_Datas;
namespace LMS_Web.ViewModel
{
    public class AdminViewModel
    {
        public int AdminId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string MobileNo { get; set; }
        public string EmailId { get; set; }
        public Nullable<bool> Active { get; set; }
        public string gender { get; set; }
        public Nullable<System.DateTime> dob { get; set; }
        public Nullable<int> countryId { get; set; }
        public Nullable<int> cityId { get; set; }
        public Nullable<int> stateId { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
    }
}