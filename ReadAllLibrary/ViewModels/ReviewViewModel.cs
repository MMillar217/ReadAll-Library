using ReadAllLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ReadAllLibrary.ViewModels
{
    /// <summary>
    /// ReviewViewModel class which is used in the Review Views
    /// Holds properties which are required to be displayed within the view
    /// properties:
    /// string Body
    ///DateTime? CreatedAt
    /// string Author
    /// int Starts
    /// int Id
    ///</summary>
    public class ReviewViewModel
    {
        [Required]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedAt { get; set; }
        public string Author { get; set; }
        public int Stars { get; set; }
        public int Id { get; set; }
        

    }
}