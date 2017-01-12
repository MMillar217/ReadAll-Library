using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.ViewModels
{
    /// <summary>
    /// ListAllUsers class which is used in the ListAllUsers View
    /// Holds properties which are required to be displayed within the view
    /// properties:
    /// string id
    /// string UserName
    /// string LName
    /// string FName
    /// string Address
    /// String City
    /// string Postcode
    /// </summary>
    public class ListAllUsers
    {
        public string Id { get; set; }

        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "Last Name")]
        public string LName { get; set; }

        [Display(Name = "First Name")]
        public string FName { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "Post code")]
        public string Postcode { get; set; }
    }
}