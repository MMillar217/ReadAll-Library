using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ReadAllLibrary.DAL;
using ReadAllLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using ReadAllLibrary.ViewModels;
using Stripe;
using ReadAllLibrary.Helpers;

/// <summary>
/// Namespace which holds the controllers for the project
/// </summary>
namespace ReadAllLibrary.Controllers
{
    /// <summary>
    /// Abstact class which extends Controller and is implemented be the other controllers in the project
    /// allows for the collection of common methods which may be used in more than one controller
    /// </summary>
    [HandleError]
    public abstract class BaseController : Controller
    {
        
        UnitOfWork uow = new UnitOfWork();
        LibraryAppContext db = new LibraryAppContext();
       
        /// <summary>
        /// uses the current users identity to access their role.
        /// used to work out if the current user is the in the role superadmin
        /// </summary>
        /// <returns>true if super admin, false if not</returns>
        public bool IsSuperAdmin()
        {
            var currentUserId = this.User.Identity.GetUserId();
            var isSuperAdmin = (currentUserId != null && this.User.IsInRole("SuperAdmin"));
            return isSuperAdmin;
        }


        /// <summary>
        /// uses the current users identity to access their role.
        /// used to work out if the current user is the in the role Manager
        /// </summary>
        /// <returns>true if manager, false if not</returns>
        public bool IsManager()
        {
            var currentUserId = this.User.Identity.GetUserId();
            var isManager = (currentUserId != null && this.User.IsInRole("Manager"));
            return isManager;
        }


        /// <summary>
        /// uses the current users identity to access their role.
        /// used to work out if the current user is the in the role bookings clerk
        /// </summary>
        /// <returns>true if bookings clerk, false if not</returns>
        public bool IsBookingsClerk()
        {
            var currentUserId = this.User.Identity.GetUserId();
            var isBookingsClerk = (currentUserId != null && this.User.IsInRole("Bookings Clerk"));
            return isBookingsClerk;
        }



        /// <summary>
        /// uses the current users identity to access their role.
        /// used to work out if the current user is the in the role Membership Clerk
        /// </summary>
        /// <returns>true if Membership clerk, false if not</returns>
        public bool IsMembershipClerk()
        {
            var currentUserId = this.User.Identity.GetUserId();
            var isMembershipClerk = (currentUserId != null && this.User.IsInRole("Membership Clerk"));
            return isMembershipClerk;
        }


        /// <summary>
        /// uses the current users identity to access whether they are logged in
        /// </summary>
        /// <returns>true if Logged in, false if not</returns>
        public bool IsLoggedIn()
        {
            ApplicationUser currentUser = GetCurrentUser();
            bool loggedIn = false;
            
            if (currentUser != null)
            {
                loggedIn = true;
                return loggedIn;
            }
            else
            {
                loggedIn = false;
                return loggedIn;
            }
        }

        /// <summary>
        /// Method which gets the current logged in user of the systems identity
        /// uses the current user id from the identity
        /// </summary>
        /// <returns>the current user object</returns>
        public ApplicationUser GetCurrentUser()
        {
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = uow.UserRepository.Get(x => x.Id == currentUserId);

            return currentUser;
        }

       /// <summary>
       /// Method which allows for a user to added to a Identity role.
       /// Updates the users role and updates the user through the UOW
       /// </summary>
       /// <param name="UserName">User to be added to role</param>
       /// <param name="RoleName">Role to which user is the be added</param>
        public void RoleAddToUser(string UserName, string RoleName)
        {
            ApplicationUser user = uow.UserRepository.Get(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase));
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new LibraryAppContext()));

            try
            {
                if (user != null)
            {
                var idResult = um.AddToRole(user.Id, RoleName);
                var authenticationManager = HttpContext.GetOwinContext().Authentication;

                authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);


                var identity = um.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

                authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);
                
                user.Role = RoleName;

                uow.UserRepository.Update(user);

                db.SaveChanges();

                uow.SaveChanges();
            }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


        }


        /// <summary>
        /// Method which allows for a user to removed from an Identity role.
        /// Updates the users role and updates the user through the UOW
        /// </summary>
        /// <param name="UserName">User to be added to role</param>
        /// <param name="RoleName">Role to which user is the be added</param>
        public void DeleteRoleForUser(string UserName, string RoleName)
        {
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new LibraryAppContext()));

            ApplicationUser user = uow.UserRepository.Get(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase));

            try {
                if (um.IsInRole(user.Id, RoleName))
                {
                    um.RemoveFromRole(user.Id, RoleName);
                    
                    uow.UserRepository.Update(user);

                    uow.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
          
        }

        /// <summary>
        /// Method which allows books to be filtered by category
        /// </summary>
        /// <param name="categoryFilter">category which books are to be filtered by</param>
        /// <param name="books">collection of books to be filtered</param>
        /// <returns>filtered collection of books based on category</returns>
        public IEnumerable<Book> FilterBookByCategory(int? categoryFilter, IEnumerable<Book> books)
        {

            switch (categoryFilter)
            {
                case 0:
                    books = books.Where(b => b.Category == 0);
                    break;
                case 1:
                    books = books.Where(b => Convert.ToInt32(b.Category) == 1);
                    break;
                case 2:
                    books = books.Where(b => Convert.ToInt32(b.Category) == 2);
                    break;
                case 3:
                    books = books.Where(b => Convert.ToInt32(b.Category) == 3);
                    break;

                case -1:
                    books = books.OrderBy(b => b.BookTitle);
                    break;

            }

            return books;

        }


        /// <summary>
        /// Method which allows books to be filtered by genre
        /// </summary>
        /// <param name="genreFilter">genre which books are to be filtered by</param>
        /// <param name="books">collection of books to be filtered</param>
        /// <returns>filtered collection of books based on genre</returns>
        public IEnumerable<Book> FilterBookByGenre(int? genreFilter, IEnumerable<Book> books)
        {

            switch (genreFilter)
            {
                case 0:
                    books = books.Where(b => b.Genre == 0);
                    break;
                case 1:
                    books = books.Where(b => Convert.ToInt32(b.Genre) == 1);
                    break;
                case 2:
                    books = books.Where(b => Convert.ToInt32(b.Genre) == 2);
                    break;
                case 3:
                    books = books.Where(b => Convert.ToInt32(b.Genre) == 3);
                    break;
                case 4:
                    books = books.Where(b => Convert.ToInt32(b.Genre) == 4);
                    break;
                case 5:
                    books = books.Where(b => Convert.ToInt32(b.Genre) == 5);
                    break;
                case 6:
                    books = books.Where(b => Convert.ToInt32(b.Genre) == 6);
                    break;

                case -1:
                    books = books.OrderBy(b => b.BookTitle);
                    break;

            }

            return books;
        }

        /// <summary>
        /// helper method to deal with processing payment.
        /// creates stripe payment token and processes the payment
        /// </summary>
        /// <param name="model">payment details</param>
        /// <returns>chargeId</returns>
        public static async Task<string> ProcessPayment(StripeChargeModel model)
        {
            return await Task.Run(() =>
            {
                var myCharge = new StripeChargeCreateOptions
                {
                    // convert the amount of £12.50 to pennies i.e. 1250
                    Amount = (int)(model.Amount * 100),
                    Currency = "gbp",
                    Description = "Description for test charge",
                    Source = new StripeSourceOptions
                    {
                        TokenId = model.Token//create token
                    }
                };

                var chargeService = new StripeChargeService("sk_test_6sQFkxBnUVABC6P0Q2G0PK3H");
                var stripeCharge = chargeService.Create(myCharge);

                return stripeCharge.Id;
            });
        }


    }

    /// <summary>
    /// Custom authorization attribute class, extends the AuthorizAttribute class
    /// Allows for custom roles based authorization to be applied to controllers limiting access based on role.
    /// </summary>
      public class CustomAuthorizeAttribute : AuthorizeAttribute
        {
            private LibraryAppContext db = new LibraryAppContext();

            private readonly string[] allowedroles;

      
       //contructor, 1 parameter
        public CustomAuthorizeAttribute(params string[] roles)
                {
                    this.allowedroles = roles;
                }


        /// <summary>
        /// method which determines the current users role and sets whether they have the required access permission
        /// </summary>
        /// <param name="httpContext">the current context</param>
        /// <returns>true if the user is in the required role, false if not</returns>
            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {

                bool authorize = false;


                foreach (var role in allowedroles)
                {
                   if(httpContext.User.IsInRole(role))
                    { 
                        authorize = true; /* return true if Entity has current user(active) with specific role */
                    }
                }

                

                return authorize;
            }

        /// <summary>
        /// method which allows for the handling of an unauthorized access request.
        /// Redirects user to the login screen if they arent logged in with the appropriate access permission
        /// </summary>
        /// <param name="filterContext">AuthorizationContext instance which returns the result of the request</param>
            protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
            {
            filterContext.Result = (new RedirectController()).RedirectLogin();
            }

      
    }
}//end of namespace