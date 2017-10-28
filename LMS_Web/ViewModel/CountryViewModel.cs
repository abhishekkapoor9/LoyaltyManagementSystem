using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Web.ViewModel
{
    public class CountryViewModel
    {
        public int countryId { get; set; }
        public string countryName { get; set; }
        public Nullable<bool> Active { get; set; }
        public int countryId2 { get; set; }
        public int cityId { get; set; }
        public int stateId { get; set; }
        //public Nullable<int> countryId { get; set; }
        public string cityName { get; set; }


        //public int stateId { get; set; }
        //public Nullable<int> countryId { get; set; }
        public string stateName { get; set; }
        //public Nullable<bool> Active { get; set; }
    }
}