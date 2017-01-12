using Microsoft.AspNet.Identity;
using ReadAllLibrary.DAL;
using ReadAllLibrary.Models;
using ReadAllLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReadAllLibrary.Controllers
{
    /// <summary>
    /// Controller class which contains methods relevant to Reviews
    /// </summary>
    [HandleError]
    public class ReviewController : BaseController
    {


        /******************************VARIABLES****************************/


        private UnitOfWork uow = new UnitOfWork();



        /******************************METHODS****************************/
        


        /// <summary>
        /// Method which returns a view with a list of all the comments for a particular book
        /// </summary>
        /// <param name="id">the id of the book which reviews are being displayed</param>
        /// <returns>a list of the relevant reviews to the view</returns>
        // GET: Review
        public ActionResult Index(int id)
        {

            Book book = uow.BookRepository.Get(item => item.BookId == id);

            IEnumerable<Review> rvw = book.Reviews.ToList();

            ViewBag.Title = book.BookTitle;

            return View(rvw);
        }


        /// <summary>
        /// Method which returns the view needed to create a new review
        /// </summary>
        /// <returns>the create view</returns>
        // GET: Comments/Create
        public ActionResult Create()
        {

            return View();
        }



        /// <summary>
        /// Method which POSTS the created review to the UOW
        /// </summary>
        /// <param name="review">Review information to be posted</param>
        /// <param name="pvm">The parent viewmodel to be repopulated</param>
        /// <returns>the updated partial to the view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Review review, PartialTestViewModel pvm)
        {

            if (ModelState.IsValid)
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = uow.UserRepository.Get(x => x.Id == currentUserId);

                    review.Author = User.Identity.Name;
                    review.CreatedAt = DateTime.Now;
                    review.BookId = pvm.book.BookId;
                    review.User = currentUser;
                    int id = pvm.book.BookId;

                    currentUser.Reviews.Add(review);
                    
                    uow.ReviewRepository.Add(review);
                    uow.SaveChanges();
                    Book book = uow.BookRepository.Get(item => item.BookId == id);

                    ModelState.Clear();



                    PartialTestViewModel newPvm = new PartialTestViewModel();

                    newPvm.Reviews = book.Reviews.ToList();
                    newPvm.book = book;
                    newPvm.book.BookId = book.BookId;
                    newPvm.book.ArtworkURL = book.ArtworkURL;
                    newPvm.book.Genre = book.Genre;
                    newPvm.book.BookTitle = book.BookTitle;
                    newPvm.book.Publisher = book.Publisher;
                    newPvm.book.Description = book.Description;
                    newPvm.ReviewCount = book.Reviews.Count();
                    newPvm.user = GetCurrentUser();


                    int total = 0;

                    foreach (var r in book.Reviews)
                    {


                        int stars = r.stars;

                        total = total + stars;

                    }

                    if (newPvm.ReviewCount != 0)
                    {

                        newPvm.AvgStars = (total / newPvm.ReviewCount);
                        TempData["AvgStars"] = newPvm.AvgStars;
                    }



                    ViewData["CanLeaveReview"] = false;
                    ViewData["AlreadyReviewed"] = "You have already reviewed this book";

                 return PartialView("_ReviewPartial", newPvm);
                    

                }
               
                return View(pvm);

            }
            

      
        /// <summary>
        /// Method which gets the view required to edit a review
        /// </summary>
        /// <param name="id">id of the review to be edited</param>
        /// <returns>the edit review view</returns>
        // GET: Books/Edit/5
        public ActionResult Edit(int id)
        {

            Review review = uow.ReviewRepository.Get(item => item.Id == id);

            return View(review);
        }



        /// <summary>
        /// Method which POSTS the updated review to the UOW
        /// </summary>
        /// <param name="review">updated review to be posted</param>
        /// <returns>if successful a redirect to the details page of the book being reviewed else the edit review view</returns>
        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Review review)
        {
            

            if (ModelState.IsValid)
            {

                review.Author = User.Identity.Name;
                review.User = uow.UserRepository.Get(m => m.UserName.Equals(review.Author));
                review.CreatedAt = DateTime.Now;
                uow.ReviewRepository.Update(review);
                uow.SaveChanges();

                ViewData["CanLeaveReview"] = false;


                return RedirectToAction("Details", "Books", new { id = review.BookId });
            }
            return View(review);
        }



        /// <summary>
        /// Method which returns the view allowing for a review to be deleted
        /// </summary>
        /// <param name="id">id of review to be deleted</param>
        /// <returns>the delete review view</returns>
        // GET: Review/Delete/5
        public ActionResult Delete(int id)
        {

            Review review = uow.ReviewRepository.Get(item => item.Id == id);

            return View(review);
        }



        /// <summary>
        /// Method which POSTS the information about the deleted review to the UOW to allow for deletion from DB
        /// </summary>
        /// <param name="Id">id of review being deleted</param>
        /// <param name="bookId">id of the book which the review is attributed to</param>
        /// <param name="pvm">the viewmodel from the book details page</param>
        /// <returns>if success the updated partial to the details view else the un updated details view.</returns>
        // POST: Review/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int Id, int bookId, PartialTestViewModel pvm)
        {
            if (ModelState.IsValid)
            {
                Review review = uow.ReviewRepository.Get(item => item.Id == Id);
                uow.ReviewRepository.Delete(review);
                uow.SaveChanges();

                

                Book book = uow.BookRepository.Get(item => item.BookId == bookId);

                pvm.Reviews = book.Reviews.ToList();

                ViewData["CanLeaveReview"] = true;

                return PartialView("_ReviewPartial", pvm);
            }

            return View();
        }

    }

}
