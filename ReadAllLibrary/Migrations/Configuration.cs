namespace ReadAllLibrary.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ReadAllLibrary.Models.LibraryAppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "ReadAllLibrary.Models.LibraryAppContext";
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(ReadAllLibrary.Models.LibraryAppContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

           // if (System.Diagnostics.Debugger.IsAttached == false)
               // System.Diagnostics.Debugger.Launch();

            try
            {


                if (!(context.Users.Any(u => u.UserName.Equals("superadmin@admin.com"))))
                {
                    var userStore = new UserStore<ApplicationUser>(context);
                    var userManager = new UserManager<ApplicationUser>(userStore);
                    var userToInsert = new ApplicationUser { FName = "Admin", LName = "McAdmin", Email = "superadmin@admin.com", UserName = "superadmin@admin.com", PhoneNumber = "0712346888", PostalCode = "G11 7XF", Address = "18 Penrith Drive", City = "Glasgow", Role = "SuperAdmin" };
                    userManager.Create(userToInsert, "Password@123");


                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new LibraryAppContext()));


                    roleManager.Create(new IdentityRole { Name = "SuperAdmin" });



                    var adminUser = userManager.FindByName("superadmin@admin.com");

                    userManager.AddToRoles(adminUser.Id, new string[] { "SuperAdmin" });

                }

                if (!(context.Users.Any(u => u.UserName.Equals("unlimited1@test.com"))))
                {
                    var userStore = new UserStore<ApplicationUser>(context);
                    var userManager = new UserManager<ApplicationUser>(userStore);
                    var userToInsert = new ApplicationUser { FName = "Tester", LName = "McTester", UserName = "unlimited1@test.com", Email = "unlimited1@test.com", PhoneNumber = "07891644310", PostalCode = "G12 0DJ", Address = "18 Penrith Drive", City = "Glasgow", Role = "Unlimited Member", CanPlaceOrder = true, AccountRestricted = false };
                    userManager.Create(userToInsert, "Unlimited@123");


                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new LibraryAppContext()));//instance of role manager


                    roleManager.Create(new IdentityRole { Name = "Unlimited Member" });//create role



                    var unlimitedMember = userManager.FindByName("unlimited1@test.com");//find user

                    userManager.AddToRoles(unlimitedMember.Id, new string[] { "Unlimited Member" });//add found user to role.

                }


                if (!(context.Users.Any(u => u.UserName.Equals("limited1@test.com"))))
                {
                    var userStore = new UserStore<ApplicationUser>(context);
                    var userManager = new UserManager<ApplicationUser>(userStore);
                    var userToInsert = new ApplicationUser { FName = "Limited", LName = "McLimited", UserName = "limited1@test.com", Email = "limited1@test.com", PhoneNumber = "07777777777", PostalCode = "G1 0EJ", Address = "1 Fletcher Drive", City = "Glasgow", Role = "Limited Member", CanPlaceOrder = true, AccountRestricted = false };
                    userManager.Create(userToInsert, "Limited@123");


                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new LibraryAppContext()));//instance of role manager


                    roleManager.Create(new IdentityRole { Name = "Limited Member" });//create role



                    var limitedMember = userManager.FindByName("limited1@test.com");//find user

                    userManager.AddToRoles(limitedMember.Id, new string[] { "Limited Member" });//add found user to role.

                }


                if (!(context.Users.Any(u => u.UserName.Equals("manager@test.com"))))
                {
                    var userStore = new UserStore<ApplicationUser>(context);
                    var userManager = new UserManager<ApplicationUser>(userStore);
                    var userToInsert = new ApplicationUser { FName = "Manager", LName = "McManager", UserName = "manager@test.com", Email = "manager@test.com", PhoneNumber = "07123456789", PostalCode = "G18 6ZJ", Address = "12 London Road", City = "Glasgow", Role = "Manager" };
                    userManager.Create(userToInsert, "Manager@123");


                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new LibraryAppContext()));//instance of role manager


                    roleManager.Create(new IdentityRole { Name = "Manager" });//create role



                    var manager = userManager.FindByName("manager@test.com");//find user

                    userManager.AddToRoles(manager.Id, new string[] { "Manager" });//add found user to role.

                }

                if (!(context.Users.Any(u => u.UserName.Equals("bookings@test.com"))))
                {
                    var userStore = new UserStore<ApplicationUser>(context);
                    var userManager = new UserManager<ApplicationUser>(userStore);
                    var userToInsert = new ApplicationUser { FName = "Bookings", LName = "McBookings", UserName = "bookings@test.com", Email = "bookings@test.com", PhoneNumber = "07123456789", PostalCode = "G18 6ZJ", Address = "14 Aspley Street", City = "Glasgow", Role = "Bookings Clerk" };
                    userManager.Create(userToInsert, "Bookings@123");


                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new LibraryAppContext()));//instance of role manager


                    roleManager.Create(new IdentityRole { Name = "Bookings Clerk" });//create role



                    var bookings = userManager.FindByName("bookings@test.com");//find user

                    userManager.AddToRoles(bookings.Id, new string[] { "Bookings Clerk" });//add found user to role.

                }

                if (!(context.Users.Any(u => u.UserName.Equals("membership@test.com"))))
                {
                    var userStore = new UserStore<ApplicationUser>(context);
                    var userManager = new UserManager<ApplicationUser>(userStore);
                    var userToInsert = new ApplicationUser { FName = "Membership", LName = "McMembership", UserName = "membership@test.com", Email = "membership@test.com", PhoneNumber = "071414141414", PostalCode = "G20 9ED", Address = "144 Maryhill Road", City = "Glasgow", Role = "Membership Clerk" };
                    userManager.Create(userToInsert, "Membership@123");


                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new LibraryAppContext()));//instance of role manager


                    roleManager.Create(new IdentityRole { Name = "Membership Clerk" });//create role



                    var membership = userManager.FindByName("membership@test.com");//find user

                    userManager.AddToRoles(membership.Id, new string[] { "Membership Clerk" });//add found user to role.

                }
            }
            catch (Exception e)
            {
               
                Console.WriteLine(e);
            }
        }
        }
}

