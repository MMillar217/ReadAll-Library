using ReadAllLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.ViewModels
{
    /// <summary>
    /// StripeChargeModel class which is used in the CardPayment Views
    /// Holds properties which are required to be displayed within the view
    public class StripeChargeModel
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public double Amount { get; set; }

        // These fields are optional and are not 
        // required for the creation of the token
        
        [Display(Name = "Name On Card")]
        [Required(ErrorMessage = "Name required")]
        public string CardHolderName { get; set; }
       
        [Display(Name = "Address")]
        [Required(ErrorMessage = "Address required")]
        public string Address { get; set; }
        
        [Display(Name = "City")]
        [Required(ErrorMessage = "City required")]
        public string AddressCity { get; set; }
        
        [Display(Name = "Post Code")]
        [Required(ErrorMessage = "Name required")]
        [RegularExpression(@"^([A-Z]{1,2})([0-9][0-9A-Z]?) ([0-9])([ABDEFGHJLNPQRSTUWXYZ]{2})$", ErrorMessage =
        "Please enter a valid uk postcode (In the format XXX XXX)")]
        public string AddressPostcode { get; set; }
       
        [Display(Name = "Card Number")]
        [Required(ErrorMessage = "Card Number Required")]
        public string CardNumber { get; set; }
        
        [Display(Name = "Expiry Month")]
        [Required(ErrorMessage = "Exipry Month required")]
        public string ExpiryMonth { get; set; }
       
        [Display(Name = "Expiry Year")]
        [Required(ErrorMessage = "Exipry Year required")]
        public string ExpiryYear { get; set; }

        [Required(ErrorMessage = "CCV required")]
        public string Cvv { get; set; }

        public Fine Fine { get; set; }

    }
}