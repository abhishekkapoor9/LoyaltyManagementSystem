using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LMS_Web.ViewModel
{
    public class CustomerViewModel
    {
        [Key]
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }

        [Required(ErrorMessage = "Please Enter Mobile No.")]
        public string MobileNo { get; set; }
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