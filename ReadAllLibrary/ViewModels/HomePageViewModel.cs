using ReadAllLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.ViewModels
{
    /// <summary>
    /// HomePageViewModel class which is used in the Home page View
    /// Holds properties which are required to be displayed within the view
    /// properties:
    /// int bookId
    /// string BookTitle
    /// string Description
    /// string Author
    /// int ReviewCount
    /// string ArtworkURL
    /// DateTime Added
    /// Book book
    /// int AvgStars
    /// </summary>
    public class HomePageViewModel
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public int ReviewsCount { get; set; }
        public string ArtworkURL { get; set; }
        public DateTime Added { get; set; }
        public Book book { get; set; }
        public int AvgStars { get; set; }


    }
}