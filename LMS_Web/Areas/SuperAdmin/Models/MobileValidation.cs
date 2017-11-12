using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using LMS_Datas;
namespace LMS_Web.Areas.SuperAdmin.Models
{
    public class MobileValidation : ValidationAttribute
    {
        LoyaltyManagementSystemEntities entity = new LoyaltyManagementSystemEntities();
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
           
            if (value != null)
            {

                string number = (from num in entity.Admins where num.MobileNo==value.ToString() select num.MobileNo).FirstOrDefault();

                if (number==null)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Mobile Number Already Present Number");
                }
            }
            else
            {
                return new ValidationResult("" + validationContext.DisplayName + " is required");
            }
        }
    }
}