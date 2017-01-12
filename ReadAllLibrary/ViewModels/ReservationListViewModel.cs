using ReadAllLibrary.Helpers;
using ReadAllLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.ViewModels
{
    /// <summary>
    /// ReservationListViewModel class which is used in the reservationlist View
    /// Holds properties which are required to be displayed within the view
    /// properties:
    /// string bookTitle
    /// DateTime? DateReserved
    /// int Status
    /// Book book
    ///</summary>
    public class ReservationListViewModel
    {
        public string bookTitle { get; set; }

        public DateTime? DateReserved { get; set; }

        public int Status { get; set; }

        public Book book { get; set; }
    }
}