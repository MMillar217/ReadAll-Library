using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReadAllLibrary.DAL;
using System.Web.Mvc;
using ReadAllLibrary.Models;
using System.Net.Mail;
using System.Threading.Tasks;
using PagedList;

namespace ReadAllLibrary.Controllers
{
    /// <summary>
    /// Controller which holds functions specific to bookings clerk
    /// </summary>
    
    [HandleError]
    public class BookingsClerkController : BaseController
    {
        UnitOfWork uow = new UnitOfWork();

        /// <summary>
        /// finds orders where the status code = 1, 2 or 3 and returns these to the view
        /// </summary>
        /// <returns>List of orders</returns>
        //orders status codes: 1 = active, no fine. 2 = late, fine issued. 3 = late, fine to be issued. 4 = returned. 5 = fine paid but still to be returned
        // GET: BookingsClerk
        [CustomAuthorize("SuperAdmin", "Manager", "Bookings Clerk", "Membership Clerk")]
        public ActionResult ViewAllActiveOrders(string searchString, int? page, string currentFilter, string orderStatusSearch)
        {
            var activeOrders = uow.OrderRepository.GetAll().Where(m => m.OrderStatus == 1 || m.OrderStatus == 2 || m.OrderStatus == 3).ToList();


            if (searchString != null)
            {
                page = 1;
            }
           
            else
            {
                searchString = currentFilter;
            }

            if (orderStatusSearch != null)
            {
                page = 1;
            }
            else
            {
                orderStatusSearch = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            /* moved to startup method

            foreach (var o in activeOrders)
            {
                if(o.ReturnDate < DateTime.Now && (o.OrderStatus == 1))
                {
                    o.OrderStatus = 3;
                    o.Payment = uow.PaymentRepository.Get(m => m.PaymentID == o.PaymentRefID);
                    uow.OrderRepository.Update(o);
                }
            }

            uow.SaveChanges();
            */





            //filter by searchstring
            if (!String.IsNullOrEmpty(orderStatusSearch))
            {
                if (orderStatusSearch.Equals("active", StringComparison.CurrentCultureIgnoreCase))
                {
                    activeOrders = activeOrders.Where(b => b.OrderStatus == 1).ToList();
                }
                else if(orderStatusSearch.Equals("fine", StringComparison.CurrentCultureIgnoreCase))
                {
                    activeOrders = activeOrders.Where(b => b.OrderStatus == 2).ToList();

                }
            }

            //filter by searchstring
            if (!String.IsNullOrEmpty(searchString))
            {
                activeOrders = activeOrders.Where(b => b.User.UserName.Contains(searchString)).ToList();
            }

            int pageSize = 9;
            int pageNumber = (page ?? 1);
            //return paged list to view
            return View(activeOrders.ToPagedList(pageNumber, pageSize));
         
        }

        /// <summary>
        /// View which shows selected order to be marked as returned
        /// </summary>
        /// <param name="id">order id</param>
        /// <returns>Order to be marked as returned</returns>
        //GET: Order to be marked as returned
        [CustomAuthorize("SuperAdmin", "Manager", "Bookings Clerk", "Membership Clerk")]
        public ActionResult MarkOrderAsReturnedView(int id)
        {

            var order = uow.OrderRepository.Get(m => m.OrderId == id);


            return View(order);
        }

        /// <summary>
        /// POST method which marks the selected order as returned. Also updates the status of each book within the order.
        /// </summary>
        /// <param name="id">order id</param>
        /// <returns>Redirect to ViewAllActiveOrders view</returns>
        [CustomAuthorize("SuperAdmin", "Manager", "Bookings Clerk", "Membership Clerk")]
        [HttpPost]
        public ActionResult MarkOrderAsReturned(int id)
        {

            var order = uow.OrderRepository.Get(m => m.OrderId == id);



            order.OrderStatus = 4;
            order.Payment = uow.PaymentRepository.Get(m => m.PaymentID == order.PaymentRefID);

            uow.OrderRepository.Update(order);



            foreach (var b in order.OrderLine)
            {
                if (b.BookCopy.BookCopyStatus == 3)
                {
                    ReservationList reservationList = uow.ReservationListRepository.Get(m => m.bookId == b.BookId);

                    List<ReservationListUser> usersOnList =
                        uow.ReservationListUserRepository.GetAll()
                        .Where(m => m.ReservationListId == reservationList.ReservationListId)
                        .OrderByDescending(m => m.DateAddedToList).ToList();

                    ReservationListUser firstOnList = usersOnList.FirstOrDefault();


                }

                b.BookCopy.BookCopyStatus = 0;

                b.BookCopy.Book.StockLevel++;

                uow.BookRepository.Update(b.BookCopy.Book);

                uow.BookCopyRepository.Update(b.BookCopy);

                uow.OrderLineRepository.Update(b);
            }


            ApplicationUser user = uow.UserRepository.Get(m => m.Id.Equals(order.UserOrderId));

            user.CanPlaceOrder = true;

            user.AccountRestricted = false;

            uow.UserRepository.Update(user);

            uow.SaveChanges();

            TempData["ReturnedMessage"] = "Order number: " + order.OrderId + " has now been marked as returned";

            return RedirectToAction("ViewAllActiveOrders");
        }

      
       
          /* moved to startup
        public async Task<ActionResult> IssueFines()
        {
            int lateOrderId = 0;
            List<Book> books = new List<Book>();
            DateTime whenOrderWasDueBack = new DateTime();
            DateTime whenOrderWasIssued = new DateTime();
            var message = new MailMessage();

            var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";

            //gets all late orders
            var lateOrders = uow.OrderRepository.GetAll().Where(m => m.OrderStatus == 3).ToList();

            List<ApplicationUser> users = new List<ApplicationUser>();

            //act on all users with late orders.
            foreach (var o in lateOrders)
            {
                var user = uow.UserRepository.Get(m => m.Id.Equals(o.UserOrderId));

                user.AccountRestricted = true;

                user.CanPlaceOrder = false;

                uow.UserRepository.Update(user);
                
                uow.SaveChanges();
                
                users.Add(user);

                //retrive information for fine/invoice
                foreach (var u in users)
                {

                    Fine fine = new Fine
                    {
                        Amount = 3.00m,
                        Issued = DateTime.Now,
                        User = u
                    };

                    uow.FineRepository.Add(fine);

                    o.fine = fine;

                    uow.SaveChanges();

                    foreach (var uo in u.Order.Where(m => m.OrderStatus == 3))
                    {
                        lateOrderId = uo.OrderId;
                        whenOrderWasDueBack = uo.ReturnDate;
                        whenOrderWasIssued = uo.OrderDate;

                        foreach (var b in uo.OrderLine)
                        {
                            books.Add(b.BookCopy.Book);
                        }


                        //create message invoice to be send to customer.

                        message.To.Add(new MailAddress(u.Email));

                        

                        // replace with valid value 
                        message.From = new MailAddress("readalltest@gmail.com");  // replace with valid value
                        message.Subject = "ReadAll Order Fine";
                        message.Body = string.Format(body, "ReadAll Library", "ReadAll@ReadAll.com", "<br> You have failed to return your order on time and have now had your account restricted and been issued a fine." +
                            "<br> Your order number is: " + lateOrderId +
                            "<br> Your order was issued: " + whenOrderWasIssued +
                            "<br> Your return date was: " + whenOrderWasDueBack + "<br>"
                            + "<br> Your total fine amount is: £3.00 <br>"
                            + "In order to have your account unrestricted please visit the 'my account' section of our site and pay the fine");


                        message.Body += "<table border = '1';'>";
                        message.Body += "<tr>";
                        message.Body += "<th>Book</th>";
                        message.Body += "<th>Category</th>";
                        message.Body += "</tr>";

                        foreach (var item in books)
                        {
                            message.Body += "<tr>";
                            message.Body += "<td stlye='color:blue;'>" + item.BookTitle + "</td>";
                            message.Body += "<td stlye='color:blue;'>" + item.Category.ToString() + "</td>";
                            message.Body += "</tr>";
                        }
                        message.Body += "</table>";

                        message.IsBodyHtml = true;

                        TempData["FineMessage"] = "Fines have now been issued to: " + lateOrders.Count + " members.";

                        //update order status
                        uo.OrderStatus = 2;
                        uo.Payment = uow.PaymentRepository.Get(m => m.PaymentID == uo.PaymentRefID);

                        uow.OrderRepository.Update(uo);

                        uow.SaveChanges();

                        try {

                            using (var smtp = new SmtpClient())
                            {
                                await smtp.SendMailAsync(message);

                            }


                        }
                        catch (Exception ex)
                        {
                            Console.Write(ex.ToString());
                            TempData["FineMessage"] = "Fines have now been issued to: " + lateOrders.Count + " members.";
                            return RedirectToAction("ViewAllActiveOrders");
                        }

                    }
                    
                }

            }

            return RedirectToAction("ViewAllActiveOrders");

        }//end of issue fines

    */


    }//end of controller
}//end of namespace