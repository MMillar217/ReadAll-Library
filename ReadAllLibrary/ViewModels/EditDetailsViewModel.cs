using ReadAllLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.ViewModels
{
    public class EditDetailsViewModel
    {

        [Required(ErrorMessage = "First Name is required")]
        [Display(Name = "First Name")]
        public string FName { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required")]
        [Display(Name = "City")]
        public string City { get; set; }
  
        [Required(ErrorMessage = "Post code is required")]
        [Display(Name = "Post Code")]
        [RegularExpression(@"^([A-Z]{1,2})([0-9][0-9A-Z]?) ([0-9])([ABDEFGHJLNPQRSTUWXYZ]{2})$", ErrorMessage =
        "Please enter a valid uk postcode (In the format XXX XXX)")]
        public string PostCode { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [Display(Name = "Last Name")]
        public string LName { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        public ApplicationUser User { get; set; }
    }
}