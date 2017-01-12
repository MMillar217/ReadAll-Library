using ReadAllLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.Models
{
    /// <summary>
    /// Model class for ReservationList
    /// holds properties:
    /// int ReservationListId
    /// int BookId
    /// </summary>
    public class ReservationList
    {
        public ReservationList()
        {
            ReservationListUser = new List<ReservationListUser>();
        }

        public int ReservationListId { get; set; }

        public int bookId { get; set; }

        //Navigation Properties
        [Required]
        public virtual Book Book { get; set; }

        public ICollection<ReservationListUser> ReservationListUser { get; set; }
    }
}