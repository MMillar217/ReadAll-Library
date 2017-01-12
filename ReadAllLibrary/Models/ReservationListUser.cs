using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.Models
{
    /// <summary>
    /// Model class for ReservationListUser
    /// Holds the information about each individual item in a reservation list
    /// includes properties:
    /// int ReservationListId
    /// string UserId
    /// DateTime DateAddedToList
    /// </summary>
    public class ReservationListUser
    {
        [Key Column(Order = 0)]
        public int ReservationListId { get; set; }
        [Key Column(Order = 1)]
        public string UserId { get; set; }

        public DateTime? DateAddedToList { get; set; }

        //Navigation Properties
        [ForeignKey("ReservationListId")]
        public ReservationList ReservationList { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

    }
}