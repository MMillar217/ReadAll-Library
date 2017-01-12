using ReadAllLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReadAllLibrary.DAL
{
    /// <summary>
    /// Class which handles the business logic involving the shopping cart actions
    /// allows for instance of shopping cart to be accessed anywhere in the program.
    /// </summary>
    //book copy status codes: 1 = free, 2 = out on order, 3 = reserved when returned.
    public  class ShoppingCart
    {
        UnitOfWork uow = new UnitOfWork();
        string ShoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";

        /// <summary>
        /// Uses context from GetCart to get shopping cart for user.
        /// </summary>
        /// <param name="context">Context passed from envoking controller</param>
        /// <returns>cart object</returns>
        public static ShoppingCart GetCart(HttpContextBase context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }
        /// <summary>
        /// method which is called in the controller to get the shopping cart
        /// </summary>
        /// <param name="controller">controller which is calling the cart</param>
        /// <returns>A call to the getcart method passing in the context from the controller which is calling</returns>
        // Helper method to simplify shopping cart calls
        public static ShoppingCart GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }

        /// <summary>
        /// Method to handle the addition of items to the shopping cart
        /// </summary>
        /// <param name="book">the item being added to the cart</param>
        /// <returns>1 if successfully added, -1 if not</returns>
    public int AddToCart(Book book)
    {
        // Get the matching cart and book instances
        var cartItem = uow.CartRepository.Get(
            c => c.CartId == ShoppingCartId
            && c.BookId == book.BookId);

        List<Cart> cartItems = GetCartItems();

      
        if (cartItem == null)
        {
            // Create a new cart item if no cart item exists
            cartItem = new Cart
            {
                BookId = book.BookId,
                CartId = ShoppingCartId,
                Count = 1,
                DateCreated = DateTime.Now
            };

              
            uow.CartRepository.Add(cartItem);
            // Save changes
            uow.SaveChanges();
            //book not in cart
            return 1;
                
        }
        else
        {
            return -1;
        }

            
    }

        /// <summary>
        /// Method which handles the removal of items from the shopping cart
        /// </summary>
        /// <param name="id">id of item to be removed</param>
        /// <returns>the new item count</returns>
    public int RemoveFromCart(int id)
        {
            // Get the cart
            var cartItem = uow.CartRepository.Get(
                cart => cart.CartId == ShoppingCartId
                && cart.CopyId == id);

        int bookID = cartItem.BookId;

        var book = uow.BookRepository.Get(b => b.BookId == bookID);

            

            int itemCount = 0;

            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                    
                }
                else
                {
                    uow.CartRepository.Delete(cartItem);
                   
                }
                // Save changes
                uow.SaveChanges();
            }
            return itemCount;
        }

      
        /// <summary>
        /// method which removes all items from the cart
        /// </summary>
        public void EmptyCart()
        {
            var cartItems = uow.CartRepository.GetAll(
                cart => cart.CartId == ShoppingCartId);

            foreach (var cartItem in cartItems)
            {
                uow.CartRepository.Delete(cartItem);
            }
        // Save changes
        uow.SaveChanges();
        }

        /// <summary>
        /// method which gets all items currently in cart
        /// </summary>
        /// <returns>The cart items</returns>
        public List<Cart> GetCartItems()
        {
            return uow.CartRepository.GetAll(
                cart => cart.CartId == ShoppingCartId).ToList();
        }

        /// <summary>
        /// Counts the number of cart items in the cart
        /// </summary>
        /// <returns>the count</returns>
        public int GetCount()
        {
        // Get the count of each item in the cart and sum them up
        int? count = (from cartItems in uow.CartRepository.GetAll()
                    where cartItems.CartId == ShoppingCartId
                  select (int?)cartItems.Count).Sum();
        // Return 0 if all entries are null
        return count ?? 0;
      
        }

        /// <summary>
        /// Method to get the total cost of the items in the cart.
        /// Uses the item category to apply specific pricing to each item.
        /// Also uses the current users role to apply appropriate pricing
        /// </summary>
        /// <param name="user">the owner of the cart</param>
        /// <returns>the cart total</returns>
        public decimal GetCartTotal(ApplicationUser user)
        {
            decimal total = 0;
            int paperbackTotal = 0;
            int ebookTotal = 0;
            int audioBooktotal = 0;

            var cartItems = GetCartItems();

            //get the total number of each category of item in cart
            foreach(var i in cartItems)
            {
                if (i.Book.Category.ToString().Equals("Paperback"))
                {
                    paperbackTotal += 1;
                }
                if (i.Book.Category.ToString().Equals("Audio"))
                {
                    audioBooktotal += 1;
                }
                if (i.Book.Category.ToString().Equals("eBook"))
                {
                    ebookTotal += 1;
                }
            }

            //apply pricing based on role.
            if (user.Role.Equals("Unlimited Member"))
            {
                if (paperbackTotal == 1 || paperbackTotal == 2)
                {
                    total += 8.00m;
                }
                if (paperbackTotal > 2 && paperbackTotal <= 4)
                {
                    total += 10.00m;
                }
                if (paperbackTotal > 4 && paperbackTotal <= 6)
                {
                    total += 13.50m;
                }

                if (ebookTotal == 1 || ebookTotal == 2)
                {
                    total += 4.00m;
                }
                if (ebookTotal > 2 && ebookTotal <= 4)
                {
                    total += 6.00m;
                }
                if (ebookTotal > 4 && ebookTotal <= 6)
                {
                    total += 9.50m;
                }

                if (audioBooktotal == 1)
                {
                    total += 9.00m;
                }
                if (audioBooktotal > 1 && audioBooktotal <= 4)
                {
                    total += 14.00m;
                }
                if (audioBooktotal > 4 && audioBooktotal <= 6)
                {
                    total += 18.00m;
                }
            }
                if (user.Role.Equals("Limited Member"))
                {
                    if (paperbackTotal == 1 || paperbackTotal == 2)
                    {
                        total += 6.00m;
                    }
                    if (paperbackTotal > 2 && paperbackTotal <= 4)
                    {
                        total += 8.00m;
                    }
                    if (paperbackTotal > 4 && paperbackTotal <= 6)
                    {
                        total += 11.00m;
                    }

                

                    if (audioBooktotal == 1)
                    {
                        total += 7.00m;
                    }
                    if (audioBooktotal > 1 && audioBooktotal <= 4)
                    {
                        total += 12.00m;
                    }
                    if (audioBooktotal > 4 && audioBooktotal <= 6)
                    {
                        total += 17.00m;
                    }

                }
            return total;
        }


        /// <summary>
        /// Method which gets the cartId from the session object.
        /// if not cart exists for the user a new one is made and the key is the users username
        /// if a cart does exist the key is returned
        /// </summary>
        /// <param name="context">the current context</param>
        /// <returns>the key to access the cart(the user's username)</returns>
        // We're using HttpContextBase to allow access to cookies.
        public string GetCartId(HttpContextBase context)
            {
                var currentUser = context.User.Identity;
            
                
                if (context.Session[CartSessionKey] == null)
                {
                    if (!string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name))
                    {
                        context.Session[CartSessionKey] =
                            context.User.Identity.Name;
                    }
                    else
                    {
                        // Generate a new random GUID using System.Guid class
                        Guid tempCartId = Guid.NewGuid();
                        // Send tempCartId back to client as a cookie
                        context.Session[CartSessionKey] = tempCartId.ToString();
                    }
                }
                return context.Session[CartSessionKey].ToString();
            }

        /// <summary>
        /// method which handles the creation of the orderlines for an order.
        /// iterates through the cart items adding each item to an orderline.
        /// decreases the stocklevel of each item being added the the order and then emptys cart when the order is succesful
        /// </summary>
        /// <param name="order">the current order which orderlines need to be generated for</param>
        /// <returns>the order.id if successful</returns>
            public int CreateOrder(Order order)
            {
                    

                var cartItems = GetCartItems();
                // Iterate over the items in the cart, 
                // adding the order details for each

                    
                    

                foreach (var item in cartItems)
                {
                    var orderline = new OrderLine
                    {
                        OrderId = order.OrderId,
                        BookId = item.BookId,
                        BookCopy = uow.BookCopyRepository.Get(x => x.Book.BookId == item.BookId &&
                        (x.BookCopyStatus != 1 && x.BookCopyStatus != 2 && x.BookCopyStatus != 3))
                          
                    };


            // Set the order total of the shopping cart
                
                    orderline.BookCopy.BookCopyStatus = 2;        

                    uow.OrderLineRepository.Add(orderline);

                    item.Book.StockLevel--;//reduce stock when book is ordered
                }
                   
                // Save the order
                uow.SaveChanges();
                // Empty the shopping cart
                EmptyCart();
                // Return the OrderId as the confirmation number
                return order.OrderId;
            }

    }

}
