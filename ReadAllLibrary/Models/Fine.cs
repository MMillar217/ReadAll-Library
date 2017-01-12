using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.Models
{
    /// <summary>
    /// Model class for Fine
    /// Holds properties:
    /// int FineId
    /// decimal Amount
    /// DateTime Issued
    /// DateTime Paid
    /// </summary>
    public class Fine
    {
        public int FineId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? Issued { get; set; }
        public DateTime? Paid { get; set; }


        //Navigation properties
        public virtual Payment payment { get; set; }

        [Required]
        public virtual ApplicationUser User { get; set; }

        [Required]
        public virtual Order order { get; set; }
     
    }
}