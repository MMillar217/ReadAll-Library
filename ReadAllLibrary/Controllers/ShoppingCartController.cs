using Braintree;
using ReadAllLibrary.DAL;
using ReadAllLibrary.Models;
using ReadAllLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace ReadAllLibrary.Controllers
{

    /// <summary>
    /// Controller class which contains the methods relevant to the shopping cart functionality
    /// </summary>
    [HandleError]
    public class ShoppingCartController : BaseController
    {

        /***************************VARIABLES*************************/

        UnitOfWork uow = new UnitOfWork();



        /***************************CLASS METHODS*************************/



        /*************************** WAITING LIST (UNFINISHED FUNCTIONALITY) *************************/

        
        /// <summary>
        /// Method which adds a user to a waiting list for a particular book
        /// </summary>
        /// <param name="user">user to be added to list</param>
        /// <param name="book">book which the waiting list applies to</param>
        /// <returns>a message stating if they have been added or not</returns>
        public string AddToWaitingList(ApplicationUser user, Book book)
        {

            var waitingList = uow.ReservationListRepository.Get(m => m.bookId == book.BookId);


            string message;


            //entity to be added to waiting list
            //includes a date added, the list they are being added to and the user being added
            ReservationListUser reservationListUser = new ReservationListUser
            {
                ReservationList = waitingList,
                User = user,
                UserId = user.Id,
                DateAddedToList = DateTime.Now

                //POSSIBLY NEED TO ADD BOOK COPY?

            };


            //if no waiting list exists create one
            if (waitingList == null)
            {
                ReservationList newReservationList = new ReservationList
                {
                    ReservationListUser = new List<ReservationListUser>(),
                    Book = book,
                    bookId = book.BookId
                };

                newReservationList.ReservationListUser.Add(reservationListUser);

                uow.ReservationListRepository.Add(newReservationList);
                uow.ReservationListUserRepository.Add(reservationListUser);

                var bookCopyToBeReserved = uow.BookCopyRepository.Get(m => m.Book.BookId == book.BookId && m.BookCopyStatus == 2);

                bookCopyToBeReserved.BookCopyStatus = 3;
                bookCopyToBeReserved.Book = book;

                uow.BookCopyRepository.Update(bookCopyToBeReserved);

                message = "added to list";


                uow.SaveChanges();


                return message;
            }

            //if waiting list exists add user to it
            else if (waitingList.ReservationListUser.Contains(reservationListUser) == false)
            {
                uow.ReservationListUserRepository.Add(reservationListUser);
                waitingList.ReservationListUser.Add(reservationListUser);
                waitingList.Book = book;
                uow.ReservationListRepository.Update(waitingList);
                uow.SaveChanges();

                message = "added to list";

                return message;
            }
            else
            {
                message = "Already on this waiting list";
                return message;
            }
        }



        /// <summary>
        /// Method which returns a view showing the users reserved books
        /// </summary>
        /// <returns>list of reserved books to the view</returns>
        public ActionResult ReservationList()
        {
            var user = GetCurrentUser();

            var listItemsUserIsIn = uow.ReservationListUserRepository.GetAll().Where(m => m.UserId == user.Id);

            List<Book> books = new List<Book>();

           
            foreach (var i in listItemsUserIsIn)
            {
                var book = uow.BookRepository.Get(m => m.BookId == i.ReservationListId);
               
                books.Add(book);

              
            }

            int id = 1;

            var bookCopy = uow.BookCopyRepository.Get(m => m.BookCopyID ==id);

            
            //populate viewmodel
            IEnumerable<ReservationListViewModel> viewModel = from b in listItemsUserIsIn
                                                       select new ReservationListViewModel
                                                       {
                                                           
                                                           book = uow.BookRepository.Get(m => m.BookId == b.ReservationListId),
                                                           DateReserved = b.DateAddedToList,
                                                                                                    
                                                           
                                                       };


            return View(viewModel.ToList());
        }





        /***************************SHOPPING CART*************************/


        /// <summary>
        /// Method which gets the index view of the users shopping cart displaying the items within the cart
        /// </summary>
        /// <returns>a list of shopping cart items to the view</returns>
        public ActionResult Index()
        {
            //apply messages depending on users current conditions

            if (IsLoggedIn() == false)
            {
                TempData["message"] = "Please login to view cart";
                return RedirectToAction("Login", "Account", null);
            }

            TempData["CheckoutPermission"] = true;

            var cart = ShoppingCart.GetCart(this.HttpContext);

            // Set up our viewmodel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetCartTotal(GetCurrentUser()),
                CanPlaceOrder = GetCurrentUser().CanPlaceOrder,
                userId = GetCurrentUser().Id
               
            };

            //check if limited member has tried to add ebook to cart
            //set permissions appropriately
            foreach(var r in viewModel.CartItems)
            {
                if(User.IsInRole("Limited Member") && r.Book.Category == (Category)2)
                {
                    viewModel.Permission = false;
            

                    TempData["NeedsToUpgrade"] = true;
                }
            }

            
            
            //empty cart hide checkout button
            if (viewModel.CartTotal == 0)
            {
                viewModel.Permission = false;//shows/hides checkout button
            }
            else
            {

                TempData["CartMessage"] = "";
                viewModel.Permission = true;
            }

            if(Convert.ToBoolean(TempData["NeedsToUpgrade"]) == true)
            {
                viewModel.Permission = false;
                viewModel.NeedsToUpgrade = true;//shows/hides upgrade button
            }

            TempData["OrderTotal"] = viewModel.CartTotal;

            // Return the view
            return View(viewModel);
        }
        


        /// <summary>
        /// Method which allows the user to add a book to their cart
        /// checks various permissions and redirects accordingly depending if allowed or not
        /// </summary>
        /// <param name="id">id of book to be added to cart</param>
        /// <returns>if success a redirect to the book index view to allow further shopping else the book details view</returns>
        public ActionResult AddToCart(int id)
        {
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser user = uow.UserRepository.Get(x => x.Id == currentUserId);

            string message;
            List<Order>ordersThisMonth = new List<Order>();

            //find out whether the user has place an order this month
            if (IsLoggedIn())
            {
                List<Order> userOrders = user.Order.ToList();

                ordersThisMonth = userOrders.Where(m => (m.OrderDate - DateTime.Now).TotalDays < 30).ToList();

            }

            //if not a logged in member refirect to login page
           else if (IsLoggedIn() == false)
            {
                TempData["NeedToLogin"] = "Please login to add items to cart";

                var book = uow.BookRepository.Get(m => m.BookId == id);

                TempData["BookForUnloggedInUser"] = book;

                TempData["RedirectedFromAddToCart"] = true;

                return RedirectToAction("Login", "Account", null);

            }

            //get book from repo
            var addedBook = uow.BookRepository.Get(book => book.BookId == id);
        
            //change status to ready to order in cart
            if (addedBook.StockLevel > 0)
            {
                addedBook.Status = 0;
            }

            //change status to reserved and add to reservation list for user
            else if(addedBook.StockLevel <= 0)
            {
                addedBook.Status = 1;
                message = AddToWaitingList(user, addedBook);

                if (message.Equals("added to list")) 
                {
                    return RedirectToAction("ReservationList");
                }

            }


            //below are the permissions associated with each account as per requirements

            if (User.IsInRole("Limited Member") && addedBook.Category == (Category)2)
            {
                TempData["NeedsToUpgrade"] = true;
                return RedirectToAction("Index");
            }
            else if(User.IsInRole("Limited Member") && ordersThisMonth.Any())
            {
                TempData["NeedsToUpgrade"] = true;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["NeedsToUpgrade"] = false;
                // Add it to the shopping cart
                var cart = ShoppingCart.GetCart(this.HttpContext);

                int addToCart = cart.AddToCart(addedBook);

                if (addToCart == 1)
                {

                    uow.SaveChanges();

                    // Go to book list for more shopping
                    return RedirectToAction("Index", "Books");
                }
                if (addToCart == -1)
                {
                    TempData["AlreadyInCart"] = "Book is already in cart";
                    return RedirectToAction("Details", "Books", new { id = id });
                }
            }

            return View();
        }



        /// <summary>
        /// Method which displays the updated shopping cart after an item has been removed
        /// </summary>
        /// <param name="id">id of the book to be removed</param>
        /// <returns>a JSON to the view with the updated shopping cart contents</returns>
        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {

            // Remove the item from the cart
            var cart = ShoppingCart.GetCart(this.HttpContext);

            string bookTitle = uow.CartRepository.Get(item => item.CopyId == id).Book.BookTitle;

            // Remove from cart
            int itemCount = cart.RemoveFromCart(id);
            
            // Display the confirmation message
            var results = new ShoppingCartRemoveViewModel
            {
                Message = Server.HtmlEncode(bookTitle) +
                    " has been removed from your cart.",
              
                CartCount = cart.GetCount(),
                ItemCount = itemCount,
                DeleteId = id,
                CartTotal = cart.GetCartTotal(GetCurrentUser())
                
                
            };

            foreach (var r in cart.GetCartItems())
            {
                if (User.IsInRole("Limited Member") && r.Book.Category == (Category)2)
                {
                    results.Permission = false;


                    TempData["NeedsToUpgrade"] = true;
                }
                else
                {
                    results.Permission = true;


                    TempData["NeedsToUpgrade"] = false;
                }

            }
            
           


            if (results.CartTotal == 0)
            {
                results.Message = Server.HtmlEncode(bookTitle) +
                     " has been removed from your shopping cart.";
                results.Permission = false;
            }
            else
            {
                
                results.Permission = true;
            }

            return Json(results);
        }



           
        

    }//end of controller
}//end of namespace