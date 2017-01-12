using Microsoft.AspNet.Identity;
using ReadAllLibrary.DAL;
using ReadAllLibrary.Models;
using ReadAllLibrary.ViewModels;
using Stripe;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using Braintree;
using ReadAllLibrary.Helpers;

namespace ReadAllLibrary.Controllers
{
    /// <summary>
    /// Controller class which contains methods related to the checkout process
    /// </summary>
    [HandleError]
    public class CheckoutController : BaseController
    {


        /**********************VARIABLES*********************************/


        UnitOfWork uow = new UnitOfWork();

        public IBraintreeConfiguration config = new BraintreeConfiguration();

        public static readonly TransactionStatus[] transactionSuccessStatuses = {
                                                                                    TransactionStatus.AUTHORIZED,
                                                                                    TransactionStatus.AUTHORIZING,
                                                                                    TransactionStatus.SETTLED,
                                                                                    TransactionStatus.SETTLING,
                                                                                    TransactionStatus.SETTLEMENT_CONFIRMED,
                                                                                    TransactionStatus.SETTLEMENT_PENDING,
                                                                                    TransactionStatus.SUBMITTED_FOR_SETTLEMENT
                                                                                };





        /**************************************CLASS METHODS***************************************/


        /**********************************CONFIRM ADDRESS SECTION*********************************/


        /// <summary>
        /// Method which returns a view asking the user to confirm their shipping address details
        /// </summary>
        /// <param name="amount">Order amount</param>
        /// <returns>the ConfirmAddressViewModel to the view</returns>
        //get
        public ActionResult ConfirmAddress(decimal amount)
        {

            ApplicationUser currentUser = GetCurrentUser();

            string address = currentUser.Address;
            string city = currentUser.City;
            string postcode = currentUser.PostalCode;

            ConfirmAddressViewModel confirmAddressViewModel = new ConfirmAddressViewModel
            {
                Address = address,
                City = city,
                Postcode = postcode,
                FastShipping = false,
                UseSavedAddress = false,
                Amount = amount
            };

            return View(confirmAddressViewModel);
        }



        /// <summary>
        /// Posts the address information which has been selected by the customer 
        /// </summary>
        /// <param name="confirmAddressViewModel">Information that the customer has entered</param>
        /// <param name="id">Used to identify wheter the user has selected to user their saved address or a new one</param>
        /// <returns>A redirect to the choose payment type view.</returns>
        [HttpPost]
        public ActionResult AddressConfirmed(ConfirmAddressViewModel confirmAddressViewModel, int id)
        {
            ApplicationUser currentUser = GetCurrentUser();

            ShippingDetails ShippingDetails = new ShippingDetails();

            if (id == 1)
            {
                ShippingDetails shippingDetails = new ShippingDetails
                {
                    Address = currentUser.Address,
                    City = currentUser.City,
                    PostCode = currentUser.PostalCode,
                    fastShipping = confirmAddressViewModel.FastShipping


                };

                Session["ShippingDetails"] = shippingDetails;
            }
            else if (id ==2)
            {
                ShippingDetails shippingDetails = new ShippingDetails
                {
                    Address = confirmAddressViewModel.Address,
                    City = confirmAddressViewModel.City,
                    PostCode = confirmAddressViewModel.Postcode,
                    fastShipping = confirmAddressViewModel.FastShipping

                };

                Session["ShippingDetails"] = shippingDetails;

            }

            Session["OrderAmount"] = confirmAddressViewModel.Amount;


            return RedirectToAction("ChoosePaymentType");
        }



        /**********************************CHOOSE PAYMENT SECTION*********************************/



        /// <summary>
        /// Method which displays a view and allows the user to select their payment method
        /// </summary>
        /// <returns>ChoosePaymentType view if shipping details arent null, else it redirects to ConfirmAddressView</returns>
        public ActionResult ChoosePaymentType()
        {
            if (Session["ShippingDetails"] == null)
            {
                return RedirectToAction("ConfirmAddress");

            }

            return View();
        }




        /**********************************PAYPAL SECTION*********************************/


        /// <summary>
        /// Method which returns a view to retrieve customers paypal payment details 
        /// </summary>
        /// <returns>PayPalPaymentViewModel to the view</returns>
        public ActionResult PayPalPayment()
        {
            if (Session["ShippingDetails"] == null)
            {
                return RedirectToAction("ConfirmAddress");

            }

            PayPalViewModel ppvm = new PayPalViewModel();

            ppvm.Amount = Convert.ToDouble(Session["OrderAmount"]);

            ShippingDetails sd = (ShippingDetails)Session["ShippingDetails"];

            //add on shipping cost
            if (sd.fastShipping == true)
            {
                ppvm.Amount += 3.50;
            }



            var gateway = config.GetGateway();
            var clientToken = gateway.ClientToken.generate();
            ViewBag.ClientToken = clientToken;

            return View(ppvm);
        }
     

        /// <summary>
        /// Method which uses paypal details generated from view to make paypal payment.
        /// </summary>
        /// <param name="collection">information from submitted form including payment nonce and amount</param>
        /// <returns>Redirect to orderconfirmation page if successful, else Redirect to show errors page.</returns>
        public ActionResult CreatePaypalPayment(FormCollection collection){


                var gateway = config.GetGateway();
                decimal amount;


                try
                {
                    amount = Convert.ToDecimal(Request["amount"]);
                }
                catch (FormatException e)
                {
                    TempData["Flash"] = "Error: 81503: Amount is an invalid format.";
                    return RedirectToAction("Charge");
                }

                var nonce = Request.Form["payment_method_nonce"];


                var request = new TransactionRequest
                {
                    Amount = amount,
                    PaymentMethodNonce = nonce
                };

                Result<Transaction> result = gateway.Transaction.Sale(request);


                if (result.IsSuccess())
                {


                     Transaction transaction = result.Target;
                    
                

                        Payment payment = new Payment
                        {
                            Name = transaction.PayPalDetails.PayerFirstName + " " + transaction.PayPalDetails.PayerLastName,

                            DatePaid = DateTime.Now,

                            PayPalPaymentID = transaction.PayPalDetails.PaymentId,

                            PaymentMethod = (Models.PaymentMethod)1,

                        };
                

                        Session["PaypalPaymentConfirmed"] = true;
                        Session["PaypalPayment"] = payment;


                    var order = CreateFinalOrder(Convert.ToDecimal(transaction.Amount)); //create the corresponding order for the payment

                    int orderid = order.OrderId;
                    return RedirectToAction("OrderConfirmation", new { id = orderid });

           
                }
                else if (result.Transaction != null)
                {
                    Session["PaypalPaymentConfirmed"] = false;
                    return RedirectToAction("ShowErrors", new { id = result.Transaction.Id });
                }
                else
                {
                    string errorMessages = "";
                    foreach (ValidationError error in result.Errors.DeepAll())
                    {
                        errorMessages += "Error: " + (int)error.Code + " - " + error.Message + "\n";
                    }
                    TempData["Flash"] = errorMessages;
                    Session["PaypalPaymentConfirmed"] = false;
                    return RedirectToAction("ShowErrors");
                }

            }


        /// <summary>
        /// Method which displays a view with any payment errors on it
        /// </summary>
        /// <returns>Show Errors view</returns>
        public ActionResult ShowErrors()
        {
            return View();
        }



        /**********************************CARD PAYMENT SECTION*********************************/


        /// <summary>
        /// Method which returns a view to retrieve payment details
        /// </summary>
        /// <param name="error">if there is an error with the payment details entered this is true and message is displayed</param>
        /// <returns>StripeChargeModel to the view</returns>
        public ActionResult ChargeView(bool? error)
        {

            //redirect if no shippinng details
            if (Session["ShippingDetails"] == null)
            {
                return RedirectToAction("ConfirmAddress");

            }
            else
            {
                if (error == true)
                {
                    ViewBag.Error = "Whoops, something went wrong please try again. Ensure all fields are filled in and postcode is in the format XXX XXX";
                }

                //add order amount to model.
                StripeChargeModel scm = new StripeChargeModel();
                scm.Amount = Convert.ToDouble(TempData["Amount"]);


                ShippingDetails sd = (ShippingDetails)Session["ShippingDetails"];

                //add shipping amount to total
                if (sd.fastShipping == true)
                {
                    scm.Amount += 3.50;
                }

                scm.Amount += Convert.ToDouble(Session["OrderAmount"]);
                scm.Address = sd.Address;
                scm.AddressCity = sd.City;
                scm.AddressPostcode = sd.PostCode;



                return View(scm);
            }
        }


        /// <summary>
        /// Method which creates a card payment using stripe
        /// </summary>
        /// <param name="model">values entered by the customer from the payment view</param>
        /// <returns>OrderConfirmation view if success, else return view</returns>
        [HttpPost]
        public async Task<ActionResult> Charge(StripeChargeModel model)
        {
            //if model state is not valid return view
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Whoops, something went wrong please try again.";

                return RedirectToAction("ChargeView", new { error = true});
            }

            ViewBag.Error = "";

            //redirect if no shippinng details
            if (Session["ShippingDetails"] == null)
            {
                RedirectToAction("ConfirmAddress");

            }
            else
            {

                var chargeId = await ProcessPayment(model);


                var cart = ShoppingCart.GetCart(this.HttpContext);

                //if process payment is successful create payment
                if (chargeId != null)
                {
                    string cardNum = model.CardNumber.ToString();
                    string exM = model.ExpiryMonth.ToString();
                    string exY = model.ExpiryYear.ToString();
                    string Name = model.CardHolderName.ToString();
                    string billingAddress = model.Address.ToString();
                    string billingCity = model.AddressCity.ToString();
                    string billingPostcode = model.AddressPostcode.ToString();


                    Payment payment = new Payment
                    {
                        CardAddress = billingAddress,
                        CardCity = billingCity,
                        CardPostCode = billingPostcode,
                        CardNumber = cardNum,
                        ExpiryMonth = exM,
                        ExpiryYear = exY,
                        Name = Name,

                        DatePaid = DateTime.Now
                    };

                    Session["PaymentConfirmed"] = payment; // add payment to session object

                    decimal amount = Convert.ToDecimal(model.Amount);

                    var order = CreateFinalOrder(amount); //create the corresponding order for the payment

                    int orderid = order.OrderId;

                    if (order == null)
                    {
                        return RedirectToAction("Index", "Home", null);
                    }

                    return RedirectToAction("OrderConfirmation", new { id = orderid });
                }
            }

            //if charge fails
            return View(model);

        }




        /**********************************CREATE FINAL ORDER SECTION*********************************/



        /// <summary>
        /// Creates the order using the shipping details and payment information provided
        /// </summary>
        /// <returns>Order details to the FinalOrderConfirmation view</returns>
        public Order CreateFinalOrder(decimal amount)
        {

            Order order = new Order();

            //get user
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = uow.UserRepository.Get(x => x.Id == currentUserId);


            var shippingDetails = (ShippingDetails)Session["ShippingDetails"];
            var payment = (Payment)Session["PaymentConfirmed"];
            bool payPalPaymentConfirmed = Convert.ToBoolean(Session["PaypalPaymentConfirmed"]);
            var PaypalPayment = (Payment)Session["PaypalPayment"];

            //if payment and shipping != null create order
            if ((payment != null && shippingDetails != null) || (payPalPaymentConfirmed = true && shippingDetails != null && PaypalPayment != null))
            {

                order.OrderDate = DateTime.Now;
                order.ReturnDate = order.OrderDate.AddMinutes(5);
                order.User = currentUser;
                order.User.FName = currentUser.FName;
                order.User.LName = currentUser.LName;
                order.User.UserName = currentUser.UserName;
                order.Total = amount;
                order.User.PhoneNumber = currentUser.PhoneNumber;
                order.User.Email = currentUser.UserName;
                order.OrderStatus = 1;
                order.UserOrderId = currentUserId;
                order.ShippingDetails = shippingDetails;
                order.ShippingDetailsId = shippingDetails.ShippingDetailsId;

                if (payPalPaymentConfirmed == false)
                {
                    order.Payment = payment;
                }
                else
                {
                    order.Payment = PaypalPayment;
                }


                currentUser.Order.Add(order);

                currentUser.CanPlaceOrder = false;

                uow.UserRepository.Update(currentUser);

                uow.OrderRepository.Add(order);

                uow.SaveChanges();// need to save order before adding orderlines to avoid context tracking errors

                var cart = ShoppingCart.GetCart(this.HttpContext);

                cart.CreateOrder(order);

                TempData["PayPalPayment"] = payPalPaymentConfirmed;

                return order;
            }

            return null;
        }


        /// <summary>
        /// Method which returns the users successful order to the view
        /// </summary>
        /// <param name="id">id of the order which has been created</param>
        /// <returns>Details of the successful order to the view</returns>
        public async Task<ActionResult> OrderConfirmation(int id)
        {
            var currentOrder = uow.OrderRepository.Get(x => x.OrderId == id);

            ViewData["OrderId"] = currentOrder.OrderId;

            ViewBag.Error = "Error - Transaction Failed";

            if (currentOrder.Payment.PaymentMethod == (Models.PaymentMethod)1)
            {
                ViewData["PayPalPayment"] = true;
            }
            else
            {
                ViewData["PayPalPayment"] = false;
            }


            //ensure email isnt resent when customer re-views order by comparing the
            //current time with a time a minute after the order.
            var dateResult = DateTime.Compare(currentOrder.OrderDate.AddMinutes(1), DateTime.Now);

            if (dateResult > 0)
            {
                try
                {

                    await SendEmail(currentOrder);
                }
                catch (Exception ex)
                {
                    return View(currentOrder);
                }
            }

            return View(currentOrder);
        }


        /// <summary>
        /// Method which is used to generate an email invoice to send to the customer containing the order details
        /// </summary>
        /// <param name="currentOrder">The customers order which details are being retrieved from</param>
        /// <returns>The email message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> SendEmail(Order currentOrder)
        {


            var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
            var message = new MailMessage();
            message.To.Add(new MailAddress(GetCurrentUser().Email));  // replace with valid value 
            message.From = new MailAddress("readalltest@gmail.com");  // replace with valid value
            message.Subject = "ReadAll Order Confirmation";
            message.Body = string.Format(body, "ReadAll Library", "ReadAll@ReadAll.com", "<br> Your order is confirmed! <br> Your order number is: " + currentOrder.OrderId +
                "<br> Your order total is: " + currentOrder.Total +
                "<br> Your return date is: " + currentOrder.ReturnDate + "<br>");


            message.Body += "<table border = '1';'>";
            message.Body += "<tr>";
            message.Body += "<th>Book</th>";
            message.Body += "<th>Category</th>";
            message.Body += "</tr>";

            foreach (var item in currentOrder.OrderLine)
            {
                message.Body += "<tr>";
                message.Body += "<td stlye='color:blue;'>" + item.BookCopy.Book.BookTitle + "</td>";
                message.Body += "<td stlye='color:blue;'>" + item.BookCopy.Book.Category.ToString() + "</td>";
                message.Body += "</tr>";
            }
            message.Body += "</table>";


            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                await smtp.SendMailAsync(message);
               
            }

            return message.To.ToString();

        }//end of sendEmail


    }//end of controller
}//end of namespace