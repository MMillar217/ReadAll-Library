using ReadAllLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.ViewModels
{
    /// <summary>
    /// PartialTestViewModel Class which is used to hold properties relevant to Details view
    /// Properties:
    /// List Reviews
    /// Review Review
    /// Book book
    /// int ReviewCount
    /// int AvgStars
    /// ApplicationUser User
    /// </summary>
    public class PartialTestViewModel
    {

        public List<Review> Reviews { get; set; }
        public Review Review { get; set; }
        public Book book { get; set; }
        public int ReviewCount { get; set; }
        public int AvgStars { get; set; }

        public ApplicationUser user { get; set; }



    }
}