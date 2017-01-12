using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ReadAllLibrary.Models;
using ReadAllLibrary.DAL;
using PagedList;
using ReadAllLibrary.ViewModels;

namespace ReadAllLibrary.Controllers
{
    /// <summary>
    /// Controller class which holds functions specific to books
    /// </summary>
    [HandleError]
    public class BooksController : BaseController
    {
        UnitOfWork UoW = new UnitOfWork();


        /// <summary>
        /// Displays the index view for the books.
        /// </summary>
        /// <param name="sortOrder">sorts books depending on user input</param>
        /// <param name="currentFilter">stores what books are currently filtered by</param>
        /// <param name="searchString">allows for books to be filtered dependent on user input</param>
        /// <param name="page">current page of results</param>
        /// <param name="categoryFilter">filters books by category depending on user input</param>
        /// <param name="genreFilter">filters books by genre depending on user input</param>
        /// <returns></returns>
        // GET: Books
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? categoryFilter, int? genreFilter, int? currentGenre, int? currentCategory)
        {

            //grant access to edit/delete if manager/superadmin
            if (IsManager() == true || IsSuperAdmin() == true)
            {
                ViewData["Permission"] = true;
            }

            if (sortOrder != null)
            {
                TempData["currentSort"] = sortOrder;
            }
            else
            {
                TempData["currentSort"] = "null";
            }

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.GenreSortParm = String.IsNullOrEmpty(sortOrder) ? "genre_desc" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_asc" : "";



            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;


            //filter by category
            if (categoryFilter != null)
            {

                page = 1;
            }
            else
            {
                categoryFilter = currentCategory;

            }

            ViewBag.CurrentCategory = categoryFilter.ToString();

            //filter by genre
            if (genreFilter != null)
            {
                page = 1;
            }
            else
            {
                genreFilter = currentGenre;
            }

            ViewBag.CurrentGenre = genreFilter.ToString();

            var books = UoW.BookRepository.GetAll();

            //filter by searchstring
            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.BookTitle.ToUpper().Contains(searchString.ToUpper()));
            }

            //filter by searchstring
            if (categoryFilter != null)
            {
                books = FilterBookByCategory(categoryFilter, books);
            }

            //filter by searchstring
            if (genreFilter != null)
            {
                books = FilterBookByGenre(genreFilter, books);
            }


            //sort by various conditions
            switch (sortOrder)
            {
                case "name_desc":
                    books = books.OrderByDescending(b => b.BookTitle);
                    break;

                case "genre_desc":
                    books = books.OrderByDescending(b => b.Genre);
                    break;
                case "genre_asc":
                    books = books.OrderBy(b => b.Genre);
                    break;
                case "date_desc":
                    books = books.OrderByDescending(b => b.Added);
                    break;
                case "date_asc":
                    books = books.OrderBy(b => b.Added);
                    break;

                default:  // Name ascending 
                    books = books.OrderBy(b => b.BookTitle);
                    break;
            }

            int pageSize = 9;
            int pageNumber = (page ?? 1);
            //return paged list to view
            return View(books.ToPagedList(pageNumber, pageSize));
        }


        /// <summary>
        /// method which displays details of a specific book in database
        /// </summary>
        /// <param name="id">book to be displayed</param>
        /// <returns>a formated view of the book to the view</returns>
        // GET: Books/Details/5
        public ActionResult Details(int id)
        {

            ViewData["CanUpdate"] = false;
            ViewData["CanLeaveReview"] = false;
            ViewData["AlreadyReviewed"] = " ";


            if (TempData["AlreadyInCart"] == null)
            {
                TempData["AlreadyInCart"] = "";
            }
            
            //get book
            Book book = UoW.BookRepository.Get(item => item.BookId == id);

            //add book details to viewmodel
            PartialTestViewModel pvm = new PartialTestViewModel();

            pvm.book = book;
            pvm.book.Author = book.Author;
            pvm.book.BookTitle = book.BookTitle;
            pvm.book.Description = book.Description;
           
            pvm.book.Added = book.Added;
            pvm.book.BookId = book.BookId;
            pvm.book.Publisher = book.Publisher;
            pvm.book.Supplier = book.Supplier;
            pvm.Reviews = book.Reviews.ToList();

            pvm.book.ArtworkURL = "http://placehold.it/300x450";
            pvm.ReviewCount = book.Reviews.Count();
            pvm.user = GetCurrentUser();

            //show/hide items based on conditions
            if (GetCurrentUser() != null && GetCurrentUser().AccountRestricted == true)
            {
                ViewData["AlreadyReviewed"] = "Your account is restricted, you are not permitted to leave reviews.";
                ViewData["CanLeaveReview"] = false;
            }

            //get reviews for the book
            var reviews = UoW.ReviewRepository.GetAll();

            List<string> reviewAuthors = new List<string>();

            //review permissions based on user
            foreach (var r in reviews)
            {
                reviewAuthors.Add(r.Author);

                if (GetCurrentUser() != null && (r.Author.Equals(GetCurrentUser().UserName) && GetCurrentUser().AccountRestricted == false))
                {
                    ViewData["CanUpdate"] = true;
                }
                else
                {
                    ViewData["CanUpdate"] = false;
                }
            }

            if (GetCurrentUser() != null && reviewAuthors.Contains(GetCurrentUser().UserName))
            {
                ViewData["AlreadyReviewed"] = "You have already reviewed this book";
                ViewBag.MustLogIn = "";
                ViewData["CanLeaveReview"] = false;
            }
            else if(GetCurrentUser() != null && GetCurrentUser().AccountRestricted == true)
            {
                ViewData["AlreadyReviewed"] = "Your account is restricted, you are not permitted to leave reviews.";
                ViewData["CanLeaveReview"] = false;
            }
            else
            {
                ViewData["CanLeaveReview"] = true;
            }
            if(IsLoggedIn() == false)
            {
                ViewBag.MustLogIn = "You must be logged in to leave a review";
                ViewData["AlreadyReviewed"] = "";
                ViewData["CanLeaveReview"] = false;
            }


            int total = 0;

            //work out average stars for book
            foreach (var review in book.Reviews)
            {


                int stars = review.stars;

                total = total + stars;

            }

            if (pvm.ReviewCount != 0)
            {

                pvm.AvgStars = (total / pvm.ReviewCount);
            }

            return View(pvm);

        }

        /// <summary>
        /// method which allows for the creation of copies of a book when it is added to stock.
        /// uses the stock number added to count how many copies should be made.
        /// </summary>
        /// <param name="book">Book for which copy objects are being produced</param>
        public void CreateCopies(Book book)
        {

            for (int i = 0; i < book.StockLevel; i++)
            {
                var bookCopy = new BookCopy
                {

                    Book = book,
                    BookCopyStatus = 0 // 0 = free, 1 = reserved

                };

                UoW.BookCopyRepository.Add(bookCopy);
            }

        }

    
        /// <summary>
        /// View which allows a user to create a new book
        /// </summary>
        /// <returns>the Create view</returns>
        // GET: Books/Create
        [CustomAuthorize("SuperAdmin", "Manager")]
        public ActionResult Create()
        {
            ViewBag.PublisherId = new SelectList(UoW.PublisherRepository.GetAll(), "PublisherId", "PublisherName");
            ViewBag.SupplierId = new SelectList(UoW.SupplierRepository.GetAll(), "SupplierId", "SupplierName");
            return View();
        }

        /// <summary>
        /// POST action method which posts the created book from the create view to the UOW
        /// </summary>
        /// <param name="book">Book information from create view</param>
        /// <param name="PublisherId">selected publisher id</param>
        /// <param name="SupplierId">selected supplier id</param>
        /// <returns>if successfull a redirect to the Index of books, if not the create view</returns>
        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Create(Book book, int PublisherId, int SupplierId)
        {
            

            if (ModelState.IsValid)
            {
                book.PublisherId = PublisherId;
                book.SupplierId = SupplierId;
                CreateCopies(book);
                book.Added = DateTime.Now;
                UoW.BookRepository.Add(book);

                UoW.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PublisherId = new SelectList(UoW.PublisherRepository.GetAll(), "PublisherId", "PublisherName", book.PublisherId);
            ViewBag.SupplierId = new SelectList(UoW.SupplierRepository.GetAll(), "SupplierId","SupplierName", book.SupplierId);


            return View(book);
        }

        /// <summary>
        /// View which allows a user to edit a specific book
        /// </summary>
        /// <param name="id">id of book to be edited</param>
        /// <returns>the book to be edited to the view</returns>
        // GET: Books/Edit/5
        [CustomAuthorize("SuperAdmin", "Manager")]
        public ActionResult Edit(int id)
        {
            //get book
            Book book = UoW.BookRepository.Get(item => item.BookId == id);
            //populate publisher dropdown
            ViewBag.PublisherId = new SelectList(UoW.PublisherRepository.GetAll(), "PublisherId", "PublisherName", book.PublisherId);
            //populate supplier dropdown
            ViewBag.SupplierId = new SelectList(UoW.SupplierRepository.GetAll(), "SupplierId", "SupplierName", book.SupplierId);


            return View(book);
        }

        /// <summary>
        /// POST method for editing a book which posts the edited information to the UOW.
        /// Retrives info posted from the GET edit method
        /// </summary>
        /// <param name="book">edited book information</param>
        /// <param name="PublisherId">edited publisher information</param>
        /// <param name="SupplierId">edited supplier information</param>
        /// <returns>redirect to index page if success, edit view if not</returns>
        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Book book, int PublisherId, int SupplierId)
        {
            if (ModelState.IsValid)
            {

                book.PublisherId = PublisherId;
                book.SupplierId = SupplierId;
                book.Added = DateTime.Now;
                if(book.StockLevel > 0)
                {
                    CreateCopies(book);

                }
                UoW.BookRepository.Update(book);
                UoW.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PublisherId = new SelectList(UoW.PublisherRepository.GetAll(), "PublisherId", "PublisherName", book.PublisherId);
            ViewBag.SupplierId = new SelectList(UoW.SupplierRepository.GetAll(), "SupplierId", "SupplierName", book.SupplierId);


            return View(book);
        }


        /// <summary>
        /// method which returns the view to delete a book
        /// </summary>
        /// <param name="id">id of book to be deleted</param>
        /// <returns>the selected book to the view</returns>
        // GET: Books/Delete/5
        [CustomAuthorize("SuperAdmin", "Manager")]
        public ActionResult Delete(int id)
        {
            Book book = UoW.BookRepository.Get(item => item.BookId == id);

            return View(book);
        }

        /// <summary>
        /// POST method to delete a book which uses the book id in the UOW delete method
        /// </summary>
        /// <param name="id">id of book to be deleted</param>
        /// <returns>redirect to index view</returns>
        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
       
        public ActionResult DeleteConfirmed(int id)
        {
            Book book = UoW.BookRepository.Get(item => item.BookId == id);
            UoW.BookRepository.Delete(book);
            UoW.SaveChanges();
            return RedirectToAction("Index");
        }

        


        /*******************PUBLISHER SECTION**************************/

        /// <summary>
        /// view which displays an index of all book publishers
        /// </summary>
        /// <returns>a list of publishers to the view</returns>
        public ActionResult IndexOfPublishers()
        {
            var publishers = UoW.PublisherRepository.GetAll();

            return View(publishers);
        }

        /// <summary>
        /// method which displays the view to create a new publisher
        /// </summary>
        /// <returns>Create publisher view</returns>
        //Get: Books/CreatePublisher
        [CustomAuthorize("SuperAdmin", "Manager")]
        public ActionResult CreatePublisher()
        {

            return View();
        }

        /// <summary>
        /// POST method which posts publisher details from the view to the UOW
        /// </summary>
        /// <param name="publisher">publisher object from view</param>
        /// <returns>Redirect to the create book action</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePublisher(Publisher publisher)
        {
            if (ModelState.IsValid)
            {
                UoW.PublisherRepository.Add(publisher);
                UoW.SaveChanges();

            }


            return RedirectToAction("Create");
        }

        /*******************SUPPLIER SECTION**************************/

        /// <summary>
        /// view which displays an index of all book suppliers
        /// </summary>
        /// <returns>a list of suppliers to the view</returns>
        public ActionResult IndexOfSuppliers()
        {
            var suppliers = UoW.SupplierRepository.GetAll();

            return View(suppliers);
        }

        /// <summary>
        /// Method which displays the create supplier view
        /// </summary>
        /// <returns>the view to the user</returns>
        //Get: Books/CreateSupplier
        [CustomAuthorize("SuperAdmin", "Manager")]
        public ActionResult CreateSupplier()
        {

            return View();
        }

        /// <summary>
        /// POST method which posts the publisher details from the view to the UOW
        /// </summary>
        /// <param name="supplier">Supplier to be created</param>
        /// <returns>Redirect to the Create book view.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSupplier(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                UoW.SupplierRepository.Add(supplier);
                UoW.SaveChanges();

            }


            return RedirectToAction("Create");
        }


    }//end of controller



}//end of namespace


