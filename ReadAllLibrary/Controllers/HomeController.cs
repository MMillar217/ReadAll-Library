using ReadAllLibrary.DAL;
using ReadAllLibrary.Models;
using ReadAllLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReadAllLibrary.Controllers
{
    /// <summary>
    /// Controller class which contains methods relevant to the homepage
    /// </summary>
    [HandleError]
    public class HomeController : BaseController
    {


        /********************************VARIABLES*****************************/

        UnitOfWork uow = new UnitOfWork();


        /*******************************CLASS METHODS*********************************/



        /// <summary>
        /// Method which returns the items displayed on the homepage
        /// displays a list of the most recently added books
        /// </summary>
        /// <returns>the homepage view with a list of books</returns>
        public ActionResult Index()
        {

            IEnumerable<Book> books = uow.BookRepository.GetAll();

            int totalBookStars = 0;
            int avg = 0;

            try {
                foreach (var b in books)
                {
                    foreach (var r in b.Reviews)
                    {
                        totalBookStars += r.stars;
                    }
                }
            }catch(Exception e)
            {
                Console.WriteLine(e);
            }

            IEnumerable<HomePageViewModel> vm = from b in books
                                                select new HomePageViewModel
                                                {
                                                    BookId = b.BookId,
                                                    BookTitle = b.BookTitle,
                                                    Description = b.Description.Length > 50 ? b.Description.Substring(0, 50) : b.Description,
                                                    ArtworkURL = "http://placehold.it/320x150",
                                                    Added = b.Added,
                                                    ReviewsCount = b.Reviews.Count(),
                                                    book = b,
                                                                                                 
                                                   
                                                };
         
           foreach(var v in vm)
            {
                if (v.ReviewsCount != 0)
                {
                    avg = totalBookStars / v.ReviewsCount;
                    
                }
            }

            return View(vm.ToList());
        }


        /// <summary>
        /// Method which displays the about page
        /// </summary>
        /// <returns>the about page view</returns>
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }


        /// <summary>
        /// Method which displays the contact page view
        /// </summary>
        /// <returns>the contact page details to the view</returns>
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

       
    }//end of class
}//end of namespace