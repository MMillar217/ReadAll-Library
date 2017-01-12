using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using ReadAllLibrary.ViewModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadAllLibrary.Models
{
    /// <summary>
    /// Model class holding user data
    /// Holds properties:
    /// string Address
    /// string City
    /// string State
    /// string FName
    /// string LName
    /// string PostCode
    /// bool CanPlaceOrder
    /// bool AccountRestricted
    /// string Role
    /// string DisplayAddress
    /// </summary>
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        
        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }
        public string State { get; set; }



        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First Name")]
        public string FName { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last Name")]
        public string LName { get; set; }


        [Required(ErrorMessage = "Post code is required")]
        [Display(Name = "Post Code")]
        [RegularExpression(@"^([A-Z]{1,2})([0-9][0-9A-Z]?) ([0-9])([ABDEFGHJLNPQRSTUWXYZ]{2})$", ErrorMessage =
        "Please enter a valid uk postcode (In the format XXX XXX)")]
        public string PostalCode { get; set; }


        public bool? CanPlaceOrder { get; set; }

        public bool? AccountRestricted { get; set; }

   


        public string Role { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                string dspFName = this.FName;
                string dspLName = this.LName;

                return string.Format("{0} {1}", dspFName, dspLName);
            }
        }

        //Navigation properties
        public virtual ICollection<Order> Order { get; set; }
        public virtual ICollection<Fine> Fines { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<ReservationListUser> ReservationListUser { get; set; }

        // Concatenate the address info for display in tables and such:
        [Display(Name = "Address")]
        public string DisplayAddress
        {
            get
            {
                string dspAddress =
                    string.IsNullOrWhiteSpace(this.Address) ? "" : this.Address;
                string dspCity =
                    string.IsNullOrWhiteSpace(this.City) ? "" : this.City;

                string dspPostalCode =
                    string.IsNullOrWhiteSpace(this.PostalCode) ? "" : this.PostalCode;

                return string
                    .Format("{0} {1} {2}", dspAddress, dspCity, dspPostalCode);
            }
        }



    }

    /// <summary>
    /// LibraryAppContext class, implements IdentityDBContext
    /// Handles database initialisation and Database Context
    /// </summary>
    public class LibraryAppContext : IdentityDbContext<ApplicationUser>
    {
        public LibraryAppContext() : base("name=LibraryConnection")
        {

            Database.SetInitializer<LibraryAppContext>(new DropCreateDatabaseIfModelChanges<LibraryAppContext>());

        }

        public IDbSet<Book> Books { get; set; }
        public IDbSet<Review> Reviews { get; set; }
        public IDbSet<Cart> Carts { get; set; }
        public IDbSet<Order> Orders { get; set; }
        public IDbSet<OrderLine> OrderLine { get; set; }
        public IDbSet<Publisher> Publishers { get; set; }
        public IDbSet<Payment> Payments { get; set; }
        public IDbSet<ShippingDetails> ShippingDetails { get; set; }
        public IDbSet<BookCopy> BookCopies { get; set; }
        public IDbSet<Fine> Fines { get; set; }
        public IDbSet<Supplier> Suppliers { get; set; }
        public IDbSet<ReservationList> ReservationLists { get; set; }
        public IDbSet<ReservationListUser> ReservationListUsers { get; set; }


        public static LibraryAppContext Create()
        {
            return new LibraryAppContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //identity configs
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id).ToTable("AspNetRoles");

            modelBuilder.Entity<IdentityUser>().ToTable("AspNetUsers");

            modelBuilder.Entity<IdentityUserLogin>().HasKey(l => new { l.UserId, l.LoginProvider, l.ProviderKey }).ToTable("AspNetUserLogins");
            modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId }).ToTable("AspNetUserRoles");

            modelBuilder.Entity<IdentityUserClaim>().ToTable("AspNetUserClaims");
        }
    }
}