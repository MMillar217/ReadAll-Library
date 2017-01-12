using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.ViewModels
{
    public class CustomerReportViewModel
    {
        [Display(Name ="First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Display(Name = "City")]
        public string City { get; set; }
        [Display(Name = "Postcode")]
        public string Postcode { get; set; }
        [Display(Name = "Total number of orders")]
        public int OrderCount { get; set; }
        [Display(Name = "Total number of fines")]
        public int NumberOfFines { get; set; }
        [Display(Name = "Total amount spent")]
        public decimal TotalSpent { get; set; }
        [Display(Name = "Username")]
        public string Username { get; set; }
        [Display(Name = "Current Status")]
        public bool? CurrentStatus { get; set; }

    }
}