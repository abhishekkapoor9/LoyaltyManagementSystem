using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Web.Areas.Admin.Models
{
    public class AddDeskModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public Boolean Active { get; set; }
        public string DeskName { get; set; }
        public int DeskNo { get; set; }
    }
}