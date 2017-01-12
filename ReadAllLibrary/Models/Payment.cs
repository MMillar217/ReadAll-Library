using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.Models
{
    /// <summary>
    /// Model class for Payments
    /// holds properties:
    /// int PaymentID
    /// DateTime DatePaid
    /// string CardNumber
    /// string ExpiryMonth
    /// string ExpiryYear
    /// stringName
    /// string CardAddress
    /// string CardCity
    /// string CardPostCode
    /// PaymentMethod PaymentMethod
    /// string PaypalPaymentId
    /// </summary>
    public class Payment
    {

        public Payment()
        {
            Orders = new List<Order>();
        }



        public int PaymentID { get; set; }

        [Required]
        [Display(Name = "Date Paid")]
        public DateTime DatePaid { get; set; }

        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }

        [Display(Name = "Expiry Month")]
        public string ExpiryMonth { get; set; }
       
        [Display(Name = "Expiry Year")]
        public string ExpiryYear { get; set; }

        [StringLength(100, ErrorMessage = "Must enter your name", MinimumLength = 1)]
        public string Name { get; set; }


        [StringLength(100, ErrorMessage = "Must enter address", MinimumLength = 1)]
        [Display(Name = "Address")]
        public string CardAddress { get; set; }

        [StringLength(100, ErrorMessage = "must enter a city", MinimumLength = 1)]
        [Display(Name = "City")]
        public string CardCity { get; set; }

        [RegularExpression(@"^([A-Z]{1,2})([0-9][0-9A-Z]?) ([0-9])([ABDEFGHJLNPQRSTUWXYZ]{2})$", ErrorMessage =
        "Please enter a valid uk postcode (In the format XXX XXX)")]
        [Display(Name = "Post Code")]
        public string CardPostCode { get; set; }

        [Required]
        [Display(Name = "Payment Method")]
        public PaymentMethod PaymentMethod { get; set; }

        public string PayPalPaymentID { get; set; }

        //navigation properties
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Fine> Fines { get; set; }

    }

    /// <summary>
    /// enum which represents possible payment types
    /// Card or Paypal.
    /// </summary>
    public enum PaymentMethod
    {
        Card = 0, Paypal = 1
    }
}