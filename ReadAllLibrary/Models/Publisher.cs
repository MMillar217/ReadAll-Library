using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.Models
{
    /// <summary>
    /// Model class for publisher
    /// holds properties:
    /// int Publisher Id
    /// string PublisherName
    /// </summary>
    public class Publisher
    {
        [Key]
        public int PublisherId { get; set; }
        [Required(ErrorMessage = "Publisher name is required")]
        [Display (Name = "Publisher")]
        public string PublisherName { get; set; }

        //Navigation property
        public virtual ICollection<Book> Books { get; set; }

    }
}