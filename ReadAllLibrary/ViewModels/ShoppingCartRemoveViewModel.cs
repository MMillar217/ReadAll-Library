using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.ViewModels
{
    /// <summary>
    /// ShoppingCartRemoveViewModel class which is used in the Shoppingcart View when item is removed
    /// Holds properties which are required to be displayed within the view
   ///</summary>
    public class ShoppingCartRemoveViewModel
    {

        public string Message { get; set; }
        public decimal CartTotal { get; set; }
        public int CartCount { get; set; }
        public int ItemCount { get; set; }
        public int DeleteId { get; set; }


        public bool Permission { get; set; }

        public int PaperBackTotal { get; set; }
        public int AudioBookTotal { get; set; }
        public int EBookTotal { get; set; }

    }
}