using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LMS_Datas;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using LMS_Web.Areas.SuperAdmin.Models;

namespace LMS_Web.ViewModel
{
    public class AdminViewModel
    {
       
        public int AdminId { get; set; }
        public string ClientName { get; set; }
        public string Address { get; set; }
       
        [Required(ErrorMessage = "Please Enter Mobile No.")]
        [MobileValidation]
        public string MobileNo { get; set; }
        [EmailValidation]
        public string EmailId { get; set; }
        [DisplayName("Active")]
        public Nullable<bool> Active { get; set; }
        public string gender { get; set; }
        public Nullable<System.DateTime> dob { get; set; }
        public Nullable<int> countryId { get; set; }
        public Nullable<int> cityId { get; set; }
        public Nullable<int> stateId { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}