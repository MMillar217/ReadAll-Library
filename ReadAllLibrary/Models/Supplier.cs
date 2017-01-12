using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.Models
{
    /// <summary>
    /// Model class for supplier
    /// Holds properties:
    /// int SupplierId
    /// string SupplierName
    /// </summary>
    public class Supplier
    {
        [Key]
        public int SupplierId { get; set; }
        [Required(ErrorMessage = "Supplier name is required")]
        [Display(Name = "Supplier")]
        public string SupplierName { get; set; }

        //Navigation property
        public virtual ICollection<Book> Books { get; set; }

    }
}