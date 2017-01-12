using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReadAllLibrary.Helpers
{
    /// <summary>
    /// Helper class to handle redirects throughout the program
    /// </summary>
    public class RedirectController : Controller
    {

        /// <summary>
        /// Method which handles the redirect to the login screen,
        /// passes a message to the login controller to be displayed in the view
        /// </summary>
        /// <returns>a redirect to the login screen with a message</returns>
        public ActionResult RedirectLogin()
        {

            return RedirectToAction("Login", "Account", new { Message = "You are not authorised" });
        }

        /// <summary>
        /// Method which when called redirects the user to the home page.
        /// </summary>
        /// <returns>a redirect to the home page</returns>
        public ActionResult RedirectHome()
        {

            return RedirectToAction("Index", "Home");
        }

    }
}