using ReadAllLibrary.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.Models
{
   /// <summary>
   /// Model class for Review
   /// Holds properties:
   /// int Id
   /// string Body
   /// DateTime CreatedAt
   /// string Author
   /// int Stars
   /// int BookId
   /// </summary>
    public class Review
    {

        public int Id { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Comment")]
        [Required(ErrorMessage = "You must include text")]
        public string Body { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedAt { get; set; }
        public string Author { get; set; }
        [Required(ErrorMessage = "You must include a rating")]
        [Range(0, 5, ErrorMessage = "Invalid Amount Added")]
        public int stars { get; set; }
        [Required]
        public int BookId { get; set; }

        //Navigation Properties
        public virtual ApplicationUser User { get; set; }
        public virtual Book book { get; set; }

    }
}