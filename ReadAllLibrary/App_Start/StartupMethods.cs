using ReadAllLibrary.DAL;
using ReadAllLibrary.Helpers;
using ReadAllLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ReadAllLibrary.App_Start
{
    /// <summary>
    /// methods to be run when the application starts
    /// </summary>
    public class StartupMethods
    {

        

        /// <summary>
        /// changes status of orders to fine to be issued then calls method to issue fine
        /// </summary>
        public static async void IssueFines()
        {
            UnitOfWork uow = new UnitOfWork();

            var activeOrders = uow.OrderRepository.GetAll().Where(m => m.OrderStatus == 1 || m.OrderStatus == 2 || m.OrderStatus == 3).ToList();


            foreach (var o in activeOrders)
            {


                if (o.ReturnDate < DateTime.Now && (o.OrderStatus == 1))
                {
                    o.OrderStatus = 3;
                    o.Payment = uow.PaymentRepository.Get(m => m.PaymentID == o.PaymentRefID);
                    uow.OrderRepository.Update(o);
                }

                uow.SaveChanges();

                await new StartupMethods().IssueFineInvoice();
            }
        }


        /// <summary>
        /// Creates fines for books which are overdue. Emails an invoice to members and restricts their account
        /// </summary>
        public async Task IssueFineInvoice()
        {

            UnitOfWork uow = new UnitOfWork();


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
                        uo.OrderStatus = 2;
                        

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

                       

                        //update order status
                        uo.OrderStatus = 2;
                        uo.Payment = uow.PaymentRepository.Get(m => m.PaymentID == uo.PaymentRefID);

                        uow.OrderRepository.Update(uo);

                        uow.UserRepository.Update(user);

                        uow.SaveChanges();

                        try
                        {

                            using (var smtp = new SmtpClient())
                            {
                                await smtp.SendMailAsync(message);

                            }


                        }
                        catch (Exception ex)
                        {
                            Console.Write(ex.ToString());

                            new RedirectController().RedirectHome();
                        }

                    }

                }

            }
            

        }//end of issue fines


    }
}