using ReadAllLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.ViewModels
{
    /// <summary>
    /// PayPalViewModel class which is used in the PayPal Views
    /// Holds properties which are required to be displayed within the view
    /// properties:
    /// double amount
    /// Fine Fine
    ///</summary>
    public class PayPalViewModel
    {
        
        [Required]
        public double Amount { get; set; }


        public Fine Fine { get; set; }
    }
}