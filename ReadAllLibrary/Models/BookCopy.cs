using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.Models
{
    /// <summary>
    /// Model class for BookCopy
    /// Holds properties:
    /// int BookCopyID
    /// int BookCopyStatus
    /// </summary>
    public class BookCopy
    {
        public int BookCopyID { get; set; }
        public int BookCopyStatus { get; set; }
        
        //Navigation properties
        [Required]
        public virtual Book Book { get; set; }
        public virtual ICollection<OrderLine> Orderline { get; set; }

    }
}