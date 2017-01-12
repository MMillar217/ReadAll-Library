using ReadAllLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.ViewModels
{
    /// <summary>
    /// ShoppingCartViewModel class which is used in the ShoppingCart Views
    /// Holds properties which are required to be displayed within the view
    ///</summary>
    public class ShoppingCartViewModel
    {
        [Key]
        public int id { get; set; }
        public List<Cart> CartItems { get; set; }
        public decimal CartTotal { get; set; }

        public bool Permission { get; set; }

        public int PaperBackTotal { get; set; }
        public int AudioBookTotal { get; set; }
        public int EBookTotal { get; set; }

        public bool? CanPlaceOrder { get; set; }

        public bool? NeedsToUpgrade { get; set; }

        public string userId { get; set; }
    }
}