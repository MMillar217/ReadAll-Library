using ReadAllLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.ViewModels
{
    /// <summary>
    /// SupplierReportViewModel class which is used in the SupplierReport View
    /// Holds properties which are required to be displayed within the view
   ///</summary>
    public class SupplierReportViewModel
    {
        public int SupplierId { get; set; }
        public string BookTitle { get; set; }
        public string Publisher { get; set; }
        public string Description { get; set; }
        public Genre Genre { get; set; }
        public int ReviewCount { get; set; }

        public List<Book> Books { get; set; }
    }
}