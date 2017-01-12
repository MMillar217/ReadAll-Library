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
  /// BookViewModel Class which is used in the book details view
  /// Holds properties:
  /// int BookId
  /// string Title
  /// int ReviewCount
  /// string Category
  /// string Genre
  /// string ArtworkUrl
  /// string Description
  /// Review Review
  /// IEnumberable Reviews
  /// </summary>
    public class BookViewModel
    {

        public int BookId { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string PublisherName { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string ArtworkUrl { get; set; }
        public int ReviewCount { get; set; }

       
        public Review Review { get; set; }
        
        public IEnumerable<ReviewViewModel> Reviews { get; set; }



    }
}