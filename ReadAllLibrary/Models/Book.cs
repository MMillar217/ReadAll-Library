using ReadAllLibrary.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

/// <summary>
/// Models namespace
/// Holds POCO Classes
/// </summary>
namespace ReadAllLibrary.Models
{
    /// <summary>
    /// Model Class for Book entity in Database
    /// Contains properties:
    /// int BookId
    /// string BookTitle
    /// String Description
    /// Genre Genre
    /// Category Category
    /// string Author
    /// string ArtWorkURL
    /// int PublisherID
    /// int SupplierId
    /// DateTime Added
    /// </summary>
    public class Book
    {
        public Book()
        {
            Reviews = new List<Review>();
            Category = new Category();
            BookCopies = new List<BookCopy>();
        }

        
        public int BookId { get; set; }

        [Required (ErrorMessage = "A book title is required")]
        [Display(Name = "Title")]
        public string BookTitle { get; set; }
        [Required (ErrorMessage = "Please enter a description")]
        [DataType(DataType.MultilineText)]
        public string  Description { get; set; }
   
        public Genre Genre { get; set; }
        
        public Category Category { get; set; }

        [Required(ErrorMessage = "An author is required")]
        public string Author { get; set; }
       


        public string ArtworkURL { get; set; }

        public int PublisherId { get; set; }

        public int SupplierId { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime Added { get; set; }

        [Display(Name = "Stock")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Invalid Amount Added")]
        public int StockLevel { get; set; }

        public int ? Status { get; set; }

       //Navigation properties 

        public virtual ICollection<Review> Reviews { get; set; }

        public virtual Publisher Publisher { get; set; }

        public virtual Supplier Supplier { get; set; }

        public virtual ReservationList ReservationList { get; set; }

        public virtual ICollection<BookCopy> BookCopies { get; set; }

    }

 
    /// <summary>
    /// Enum for Genre types
    /// </summary>
    public enum Genre
    {
        [Display(Name = "Horror")]
        Horror,
        [Display(Name = "Scifi")]
        SciFi,
        [Display(Name = "Factual")]
        Factual,
        [Display(Name = "Mystery")]
        Mystery,
        [Display(Name = "Fantasy")]
        Fantasy,
        [Display(Name = "Romance")]
        Romance,
        [Display(Name = "Thriller")]
        Thirller,
        [Display(Name = "Educational")]
        Educational

    }


    /// <summary>
    /// Enum for Category Types
    /// </summary>
    public enum Category
    {
        [Display(Name = "Paperback")]
        Paperback,
        [Display(Name = "Audio")]
        Audio,
        [Display(Name = "eBook")]
        eBook 

    }

}