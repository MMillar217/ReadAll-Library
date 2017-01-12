using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.Models
{
    /// <summary>
    /// Model class which holds ShippingDetails properties
    /// holds properties:
    /// int ShippingDetailsId
    /// string Address
    /// string City
    /// string PostCode
    /// bool FastShipping
    /// </summary>
    public class ShippingDetails
    {
        public int ShippingDetailsId { get; set; }
        [Required(ErrorMessage = "You must enter an address")]
        public string Address { get; set; }
        [Required(ErrorMessage = "You must enter a city")]
        public string City { get; set; }
        [Required(ErrorMessage = "You must enter a postcode")]
        [Display(Name = "Post code")]
        public string PostCode { get; set; }
        [Display(Name = "Ship next")]
        public bool fastShipping { get; set; }


        //Navigation properites
        public virtual ICollection<Order> Orders { get; set; }
    }
}