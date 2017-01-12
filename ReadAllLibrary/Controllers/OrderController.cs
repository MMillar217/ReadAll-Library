using Microsoft.AspNet.Identity;
using ReadAllLibrary.DAL;
using ReadAllLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ReadAllLibrary.Controllers
{
    /// <summary>
    /// Controller which holds methods which relate to to Orders
    /// </summary>
    [HandleError]
    public class OrderController : BaseController
    {
        /*******************INSTANCE VARIABLES*************************/

        UnitOfWork uow = new UnitOfWork();
        

        /*******************CLASS METHODS************************/
        
        /// <summary>
        /// Method which GETS all the orders that the current user has made and returns them to the view
        /// </summary>
        /// <param name="message">message which may be passed to view from other action method</param>
        /// <returns>a list of the users orders</returns>
        public ActionResult ViewUserOrders(string message)
        {
            string currentUserId = User.Identity.GetUserId();

            var orders = uow.OrderRepository.GetAll(x => x.UserOrderId == currentUserId);

            ViewBag.Message = message;

            return View(orders.ToList());
        }


        /// <summary>
        /// Method which allows the user to extend the length of their loan by 1 month if there are no fines on their account
        /// </summary>
        /// <param name="id">id of loan which is being extended</param>
        /// <returns>Redirect to ViewUserOrders action with message to be displayed in the view.</returns>
        public ActionResult ExtendLoan(int id)
        {
            var order = uow.OrderRepository.Get(m => m.OrderId == id);

            order.ReturnDate = order.ReturnDate.AddMonths(1);

            order.Payment = order.Payment;

            uow.OrderRepository.Update(order);

            uow.SaveChanges();

           string message = "Your Loan has been extended, your new return date is: " + order.ReturnDate;


            return RedirectToAction("ViewUserOrders", new { message = message});
        }

        }//end of class
}//end of namespace