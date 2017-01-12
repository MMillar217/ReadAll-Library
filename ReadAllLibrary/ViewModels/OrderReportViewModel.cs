using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.ViewModels
{
    /// <summary>
    /// OrderReportViewModel class which is used in the OrderReport View
    /// Holds properties which are required to be displayed within the view
    /// properties:
    /// int OrderId
    /// DateTime OrderDate
    /// decimal Amount
    /// string UserId
    /// int OrderCount
    /// </summary>
    public class OrderReportViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Amount { get; set; }
        public string UserId { get; set; }
        public int OrderCount { get; set; }

    }
}