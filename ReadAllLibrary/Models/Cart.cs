using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.Models
{
    /// <summary>
    /// Model Class for Cart
    /// Holds properties:
    /// int CopyId
    /// string CartId
    /// int BookId
    /// int Count
    /// DateTime DateCreated
    /// </summary>
    public class Cart
    {
        [Key]
        public int CopyId { get; set; }
        public string CartId { get; set; }
        public int BookId { get; set; }
        public int Count { get; set; }
        public DateTime DateCreated { get; set; }

        //Navigation Properties
        public virtual Book Book { get; set; }
        public virtual BookCopy BookCopy { get; set; }

       

    }
}