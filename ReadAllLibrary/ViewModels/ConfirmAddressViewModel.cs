using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.ViewModels
{
    /// <summary>
    /// ConfirmAddressViewModel class which is used in the ConfirmAddress View
    /// Holds properties which are required to be displayed within the view
    /// properties:
    /// string Address
    /// string City
    /// string Postcode
    /// bool FastShipping
    /// bool UseSavedAddress
    /// decimal Amount
    /// </summary>
    public class ConfirmAddressViewModel
    {
        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }
        [Required(ErrorMessage = "Post code is required")]
        [RegularExpression(@"^([A-Z]{1,2})([0-9][0-9A-Z]?) ([0-9])([ABDEFGHJLNPQRSTUWXYZ]{2})$", ErrorMessage =
        "Please enter a valid uk postcode (In the format XXX XXX)")]
        public string Postcode { get; set; }
        [Display(Name = "Fast Shipping")]
        public bool FastShipping { get; set; }
        [Display(Name = "Use Saved Address")]
        public bool UseSavedAddress { get; set; }

        public decimal Amount { get; set; }

    }
}