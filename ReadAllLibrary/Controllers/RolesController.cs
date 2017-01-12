using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using ReadAllLibrary.DAL;
using ReadAllLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace ReadAllLibrary.Controllers
{
    [HandleError]
    public class RolesController : Controller
    {


        /***************************VARIABLES*************************/
       
            private LibraryAppContext db = new LibraryAppContext();//TO ACCESS IDENTITY ROLE!

            UnitOfWork uow = new UnitOfWork();



        /***************************CLASS METHODS*************************/





        /// <summary>
        /// Method which displays a list of roles in the index view
        /// </summary>
        /// <returns>a list of roles to the index view</returns>

    [CustomAuthorize("SuperAdmin", "Manager")]
    public ActionResult Index()
    {
    var roles = db.Roles.ToList();
        return View(roles);
    }




        /// <summary>
        /// Method which returns the view allowing the user to create a new role
        /// </summary>
        /// <returns>the create view</returns>
        // GET: /Roles/Create
        [CustomAuthorize("SuperAdmin", "Manager")]
        public ActionResult Create()
    {
            ViewBag.Blank = "";
        return View();
    }




        /// <summary>
        /// Method which POSTS the information gathered from the create view to the DB
        /// </summary>
        /// <param name="collection">role information from view form post</param>
        /// <returns>if success redirect to view index view, else return create view</returns>
        // POST: /Roles/Create
        [CustomAuthorize("SuperAdmin", "Manager")]
        [HttpPost]
    public ActionResult Create(FormCollection collection)
    {
            
                try
                {
                    if (collection["RoleName"].Length != 0)
                    {
                        db.Roles.Add(new IdentityRole()
                        {

                            Name = collection["RoleName"]

                        });
                        db.SaveChanges();
                        ViewBag.ResultMessage = "Role created successfully !";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Blank = "You didnt enter a role name!";
                        return View();
                    }


                }
                catch
                {
                    return View();
                }
            
            
    }




        /// <summary>
        /// Method which gets the edit view for a role
        /// </summary>
        /// <param name="roleName">the name of the role to be edited</param>
        /// <returns>the role to be edited to the view</returns>
        // GET: /Roles/Edit/5
        [CustomAuthorize("SuperAdmin", "Manager")]
        public ActionResult Edit(string roleName)
    {
        var thisRole = db.Roles.Where(r => r.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

        return View(thisRole);
    }
    


    
    /// <summary>
    /// Method which POSTS the edited role to the DB
    /// </summary>
    /// <param name="role">the edited role to be saved</param>
    /// <returns>if success a redirect to the index view, else the edit view</returns>
    // POST: /Roles/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
        [CustomAuthorize("SuperAdmin", "Manager")]
        public ActionResult Edit(Microsoft.AspNet.Identity.EntityFramework.IdentityRole role)
    {
        try
        {
            db.Entry(role).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        catch
        {
            return View();
        }
    }




        /// <summary>
        /// Method which POSTS to the DB to delete the selected role
        /// </summary>
        /// <param name="RoleName">the role to be deleted</param>
        /// <returns>the index view of roles</returns>
 [CustomAuthorize("SuperAdmin", "Manager")]
        public ActionResult Delete(string RoleName)
    {


  try { 


        var thisRole = db.Roles.Where(r => r.Name.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
        db.Roles.Remove(thisRole);
        db.SaveChanges();
        return RedirectToAction("Index");
        }


  catch(Exception ex) { 
       
                Console.WriteLine(ex.ToString());

                ViewBag.ResultMessage("Please select a role");
                return RedirectToAction("ManageUserRoles");

                        }

}


        /// <summary>
        /// Method which displays a view allowing for the user to manage the roles of the system users
        /// </summary>
        /// <returns>the manager user roles view</returns>
        [CustomAuthorize("SuperAdmin", "Manager")]
        public ActionResult ManageUserRoles()
    {
            try
            {


                // populate dropdown
                var list = db.Roles.OrderBy(r => r.Name).ToList().Select(rr =>

            new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.Roles = list;
                return View();

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());

                ViewBag.ResultMessage("Please select a role");
                return RedirectToAction("ManageUserRoles");

            }
        }
    


    /// <summary>
    /// Method which allows for a role to be added to a selected user
    /// </summary>
    /// <param name="UserName">the username of the user whos role is to be updated</param>
    /// <param name="RoleName">the name of the role they are to be added to</param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult RoleAddToUser(string UserName, string RoleName)
    {

            if (!ModelState.IsValid)
            {
                return RedirectToAction("ManageUserRoles");
            }

        ApplicationUser user = uow.UserRepository.Get(m => m.UserName.Equals(UserName));
        var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new LibraryAppContext()));

            try {
                if (user != null)
                {
                    var idResult = um.AddToRole(user.Id, RoleName);

                    ViewBag.ResultMessage = "User successfully added to role!";

                    user.Role = RoleName;


                    uow.SaveChanges();
                }
                else
                {
                    ViewBag.ResultMessage = "User does not exist!";

                }
                // populate roles for the view dropdown
                var list = db.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.Roles = list;

                return View("ManageUserRoles");

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());

                return RedirectToAction("ManageUserRoles");
            }
    }
    



    /// <summary>
    /// Method which allows for the display of all roles assigned to a particular user
    /// </summary>
    /// <param name="UserName">the username of the user whos roles are to be displayed</param>
    /// <returns>The manage user roles view</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult GetRoles(string UserName)
    {

  try { 

        if (!string.IsNullOrWhiteSpace(UserName))
        {
            ApplicationUser user = db.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new LibraryAppContext()));

        try
        {

            ViewBag.RolesForThisUser = um.GetRoles(user.Id);

        }
                catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            ViewBag.ResultMessage = "User does not exist!";
        }

            // prepopulat roles for the view dropdown
            var list = db.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;
        }

        return View("ManageUserRoles");

            }

            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());

                ViewBag.ResultMessage("Please select a role");
                return RedirectToAction("ManageUserRoles");

            }
        }




        /// <summary>
        /// Method which allows for the deletion of a role from a user
        /// </summary>
        /// <param name="UserName">the username of the user whos to be deleted from the role</param>
        /// <param name="RoleName">the name of the role which the user is to be deleted from</param>
        /// <returns>The manage user roles view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleForUser(string UserName, string RoleName)
        {

            try
            {
                var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new LibraryAppContext()));

                ApplicationUser user = db.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();


                if (um.IsInRole(user.Id, RoleName))
                {
                    um.RemoveFromRole(user.Id, RoleName);
                    ViewBag.ResultMessage = "User Removed From Role!";
                }
                else
                {
                    ViewBag.ResultMessage = "User Doesn't Belong To This Role";
                }
                // prepopulat roles for the view dropdown
                var list = db.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.Roles = list;

                return View("ManageUserRoles");
            }

            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());

                
                return RedirectToAction("ManageUserRoles");

            }
        }




    /// <summary>
    /// Method which returns a view allowing a user to change their role
    /// </summary>
    /// <param name="id">id of the user whos role is to be changed</param>
    /// <returns>the change membership view</returns>
    public ActionResult ChangeMembership(string id)
    {

        var user = uow.UserRepository.Get(x => x.Id.Equals(id));

        var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new LibraryAppContext()));

        var userRoles = um.GetRoles(user.Id).FirstOrDefault().ToString();

        user.Role = userRoles;

                
                

        return View(user);

    }
    




    /// <summary>
    /// Method which POSTS the role update to the DB
    /// </summary>
    /// <param name="id">id of the user whos role is to be changed</param>
    /// <returns>a redirect to the index view of the manage controller</returns>
    [HttpPost]
    public ActionResult ChangeMembershipPost(string id)
    {

        var user = uow.UserRepository.Get(x => x.Id.Equals(id));

        var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new LibraryAppContext()));

        var userRoles = um.GetRoles(user.Id).FirstOrDefault().ToString();

           
     //change to limited
    if (userRoles == "Unlimited Member")
        {
            um.RemoveFromRole(user.Id, userRoles);
            um.AddToRole(user.Id, "Limited Member");

        var authenticationManager = HttpContext.GetOwinContext().Authentication;

        authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);//sign user out


        var identity = um.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

        authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);//sign user back in so change takes place

        user.Role = "Limited Member";

    }
    //change to unlimited
    else if(userRoles == "Limited Member")
        {
            um.RemoveFromRole(user.Id, userRoles);
            um.AddToRole(user.Id, "Unlimited Member");

        var authenticationManager = HttpContext.GetOwinContext().Authentication;

        authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);//sign user out


         var identity = um.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

        authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);//sign user back in so change takes place

                user.Role = "Unlimited Member";

    }
            
        db.SaveChanges();
        uow.SaveChanges();

        return RedirectToAction("Index","Manage");

    }


/// <summary>
/// Method which returns the signup view for users, allowing them to chose the membership type
/// </summary>
/// <returns>the sign up view</returns>
    public ActionResult SignUp()
    {
            return View();
    }




    /// <summary>
    /// Method which saves the users membership choice in tempdata and redirects to the register view
    /// </summary>
    /// <param name="choice">the users membership choice from the sign up view</param>
    /// <returns>a redirect to the register view of the account controller</returns>
    [HttpPost]
    public ActionResult MembershipChoice(int choice)
    {

        TempData["MembershipChoice"] = choice;
            
        return RedirectToAction("Register", "Account");
    }






}//end of controller

   

}