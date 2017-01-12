using ReadAllLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// namespace which holds ViewModels for ReadAllLibrary Project
/// </summary>
namespace ReadAllLibrary.ViewModels
{
    /// <summary>
    /// BookReportViewModel Class which is used in the book report view
    /// Holds properties:
    /// int BookId
    /// string BookTitle
    /// string Description
    /// int ReviewCount
    /// Category Category
    /// Genre Genre
    /// string PublisherName
    /// </summary>
    public class BookReportViewModel
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string Description { get; set; }
        public int ReviewCount { get; set; }
        public Category Category { get; set; }
        public Genre Genre { get; set; }
        public string PublisherName { get; set; }
    }
}