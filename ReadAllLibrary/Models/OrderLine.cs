using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.Models
{
    /// <summary>
    /// Orderline model class
    /// holds navigation properties:
    /// int OrderId
    /// intBookId
    /// </summary>
    public class OrderLine
    {
        [Key Column(Order = 0)]
        public int OrderId { get; set; }
        [Key Column(Order = 1)]
        public int BookId { get; set; }
        
       
        //Navigation Properties
        [Required]
        public virtual BookCopy BookCopy { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

    }
}