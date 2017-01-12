using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ReadAllLibrary.Helpers;
using ReadAllLibrary.ViewModels;
using ReadAllLibrary.DAL;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rotativa;
using PagedList;
using ReadAllLibrary.Models;

namespace ReadAllLibrary.Controllers
{

    /// <summary>
    /// Controller which holds methods relevant to managers
    /// </summary>
    [HandleError]
    [CustomAuthorize("SuperAdmin", "Manager")]
    public class ManagerController : Controller
    {
        /*******************Instance Variables*******************************/

        UnitOfWork uow = new UnitOfWork();


        /**********************BOOK REPORT SECTION***************************/


        /// <summary>
        /// Method which returns a view which a list of all books currently in the library
        /// displays buttons which allows the manager to transform this view into a book report
        /// </summary>
        /// <param name="searchString">filter which may be applied to the book list</param>
        /// <param name="page">current page</param>
        /// <param name="currentFilter">current filter which has been applied</param>
        /// <returns>a list of books to the view</returns>
        public ActionResult BookReport(string searchString, int? page, string currentFilter)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var books = uow.BookRepository.GetAll();


                     if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.BookTitle.ToUpper().Contains(searchString.ToUpper()));
            }

            IEnumerable<BookReportViewModel> brvm = from b in books
                                                    select new BookReportViewModel
                                                    {
                                                        BookId = b.BookId,
                                                        BookTitle = b.BookTitle,
                                                        Description = b.Description,
                                                        PublisherName = b.Publisher.PublisherName,
                                                        Category = b.Category,
                                                        Genre = b.Genre,
                                                        ReviewCount = b.Reviews.Count


                                                    };

            TempData["ExportedToExcell"] = " ";

            return View(brvm.ToList());
        }

        /// <summary>
        /// Method which allows the export of data from the view to an excel document.
        /// </summary>
        /// <param name="searchString">filter which may be applied to books</param>
        /// <returns>An excel spreadsheet</returns>
        public ActionResult ExportBookData(string searchString)
        {

            //get books
            var books = uow.BookRepository.GetAll();


            //filter books
            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.BookTitle.ToUpper().Contains(searchString.ToUpper()));
            }


            //add list of books to a gridview
            //format available data using viewmodel.
            GridView gv = new GridView();
            gv.DataSource = from b in books
                            select new BookReportViewModel
                            {
                                BookId = b.BookId,
                                BookTitle = b.BookTitle,
                                Description = b.Description,
                                PublisherName = b.Publisher.PublisherName,
                                Category = b.Category,
                                Genre = b.Genre,
                                ReviewCount = b.Reviews.Count
                            };

            string FileName = "BookReport" + DateTime.Now + ".xls";


            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + FileName);//name file
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            TempData["ExportedToExcell"] = "List Exported to Excel";
            return RedirectToAction("BookReport");
        }


        /// <summary>
        /// Method which generates a PDF report from data in the Bookreport view
        /// </summary>
        /// <param name="searchString">filter which may be applied to book list</param>
        /// <returns>A PDF with the contents of the report</returns>
        public ActionResult GenerateBookPDF(string searchString)
        {
            var books = uow.BookRepository.GetAll();

            //apply filter if necessary
            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.BookTitle.ToUpper().Contains(searchString.ToUpper()));
            }

            //populate viewmodel
            IEnumerable<BookReportViewModel> brvm = from b in books
            select new BookReportViewModel
            {
                BookId = b.BookId,
                BookTitle = b.BookTitle,
                Description = b.Description,
                PublisherName = b.Publisher.PublisherName,
                Category = b.Category,
                Genre = b.Genre,
                ReviewCount = b.Reviews.Count
            };

            
            
            //generate pdf from view
            return new ViewAsPdf("BookReport", brvm)
            {
                FileName = "BookReport.pdf",
                CustomSwitches = "	--footer-center \"Page [page] of [toPage]\""//add page numbers

            };
        }



        /**********************SUPPLIER REPORT SECTION***************************/



        /// <summary>
        /// Method which returns a view which a list of all suppliers currently in the library and their books
        /// displays buttons which allows the manager to transform this view into a supplier report
        /// </summary>
        /// <param name="searchString">filter which may be applied to the supplier list</param>
        /// <param name="page">current page</param>
        /// <param name="currentFilter">current filter which has been applied</param>
        /// <returns>a list of suppliers to the view</returns>
        public ActionResult SupplierReport(string searchString, int? page, string currentFilter)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var suppliers = uow.SupplierRepository.GetAll();


            if (!String.IsNullOrEmpty(searchString))
            {
                suppliers = suppliers.Where(b => b.SupplierId == Convert.ToInt32(searchString));
            }

            IEnumerable<SupplierReportViewModel> srvm = from s in suppliers select
                                                        new SupplierReportViewModel
            {
                SupplierId = s.SupplierId,
                Books = s.Books.ToList()
            };

            TempData["ExportedToExcell"] = " ";

            return View(srvm.ToList());
        }

        /// <summary>
        /// Method which allows the export of data from the view to an excel document.
        /// </summary>
        /// <param name="searchString">filter which may be applied to suppliers</param>
        /// <returns>An excel spreadsheet</returns>
        public ActionResult ExportSupplierData(string searchString)
        {


            var suppliers = uow.SupplierRepository.GetAll();

            var books = uow.BookRepository.GetAll();

            if (!String.IsNullOrEmpty(searchString))
            {
                suppliers = suppliers.Where(b => b.SupplierId == Convert.ToInt32(searchString));
            }


            string FileName = "SupplierReport" + DateTime.Now + ".xls";

            IEnumerable<SupplierReportViewModel> supplierReportView = from b in books
                                                   select new SupplierReportViewModel
                                                   {
                                                       SupplierId = b.SupplierId,
                                                       ReviewCount = b.Reviews.Count,
                                                       Description = b.Description,
                                                       Genre = b.Genre,
                                                       Publisher = b.Publisher.PublisherName,
                                                       BookTitle = b.BookTitle
                                                       
                                                   };

            supplierReportView.OrderBy(m => m.SupplierId);

            GridView gv = new GridView();

            
            gv.DataSource = supplierReportView;

            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + FileName);
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            TempData["ExportedToExcell"] = "List Exported to Excel";
            return RedirectToAction("SupplierReport");
        }


        /// <summary>
        /// Method which generates a PDF report from data in the SupplierReport view
        /// </summary>
        /// <param name="searchString">filter which may be applied to supplier list</param>
        /// <returns>A PDF with the contents of the report</returns>
        public ActionResult GenerateSupplierPDF(string searchString)
        {
            var suppliers = uow.SupplierRepository.GetAll();


            if (!String.IsNullOrEmpty(searchString))
            {
                suppliers = suppliers.Where(b => b.SupplierId == Convert.ToInt32(searchString));
            }

            IEnumerable<SupplierReportViewModel> srvm = from b in suppliers
                                                        select
                                  new SupplierReportViewModel
                                  {
                                      SupplierId = b.SupplierId,
                                     
                                      Books = b.Books.ToList()
                                  };

            return new ViewAsPdf("SupplierReport", srvm)
            {
                FileName = "SupplierReport.pdf",
                CustomSwitches = "	--footer-center \"Page [page] of [toPage]\""

            };
        }




        /**********************ORDER REPORT SECTION***************************/



        /// <summary>
        /// Method which returns a view which a list of all orders which have been placed
        /// displays buttons which allows the manager to transform this view into an order report
        /// </summary>
        /// <param name="searchString">filter which may be applied to the order list</param>
        /// <param name="page">current page</param>
        /// <param name="currentFilter">current filter which has been applied</param>
        /// <returns>a list of orders to the view</returns>
        public ActionResult OrderReport(string searchString, int? page, string currentFilter)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var orders = uow.OrderRepository.GetAll();

            var user = uow.UserRepository.GetAll();

           


            if (!String.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(b => b.OrderId == Convert.ToInt32(searchString));
            }

            IEnumerable<OrderReportViewModel> orvm = from b in orders
                                                    select new OrderReportViewModel
                                                    {
                                                       OrderId = b.OrderId,
                                                       OrderDate = b.OrderDate,
                                                       Amount = b.Total,
                                                       UserId = b.User.UserName,
                                                       OrderCount = uow.OrderRepository.GetAll().Where(m => m.UserOrderId.Equals(b.UserOrderId)).Count()


                                                    };

       

          

            TempData["ExportedToExcell"] = " ";

            return View(orvm.ToList());
        }


        /// <summary>
        /// Method which allows the export of data from the view to an excel document.
        /// </summary>
        /// <param name="searchString">filter which may be applied to orders</param>
        /// <returns>An excel spreadsheet</returns>
        public ActionResult ExportOrderData(string searchString)
        {


            var orders = uow.OrderRepository.GetAll();

            if (!String.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(b => b.OrderId == Convert.ToInt32(searchString));
            }

            var user = uow.UserRepository.GetAll();

            IEnumerable<OrderReportViewModel> orvm = from b in orders
                                                     select new OrderReportViewModel
                                                     {
                                                         OrderId = b.OrderId,
                                                         OrderDate = b.OrderDate,
                                                         Amount = b.Total,
                                                         UserId = b.UserOrderId,
                                                         OrderCount = uow.OrderRepository.GetAll().Where(m => m.UserOrderId.Equals(b.UserOrderId)).Count()
                                                         
                                                     };


            string FileName = "OrderReport" + DateTime.Now + ".xls";


            GridView gv = new GridView();
            gv.DataSource = orvm;
            
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + FileName);
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            TempData["ExportedToExcell"] = "List Exported to Excel";
            return RedirectToAction("OrderReport");
        }



        /// <summary>
        /// Method which generates a PDF report from data in the OrderReport view
        /// </summary>
        /// <param name="searchString">filter which may be applied to order list</param>
        /// <returns>A PDF with the contents of the report</returns>
        public ActionResult GenerateOrderPDF(string searchString)
        {
            var orders = uow.OrderRepository.GetAll();

            if (!String.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(b => b.OrderId == Convert.ToInt32(searchString));
            }

            IEnumerable<OrderReportViewModel> orvm = from b in orders
                                                     select new OrderReportViewModel
                                                     {
                                                         OrderId = b.OrderId,
                                                         OrderDate = b.OrderDate,
                                                         Amount = b.Total,
                                                         UserId = b.UserOrderId,
                                                         OrderCount = uow.OrderRepository.GetAll().Where(m => m.UserOrderId.Equals(b.UserOrderId)).Count()


                                                     };

            return new ViewAsPdf("OrderReport", orvm)
            {
                FileName = "OrderReport.pdf",
                CustomSwitches = "	--footer-center \"Page [page] of [toPage]\""

            };
        }


        /**********************CUSTOMER REPORT SECTION***************************/



        /// <summary>
        /// Method which displays report info about a customer
        /// displays buttons which allows the manager to transform this view into an customer report
        /// </summary>
        /// <param name="searchString">filter which may be applied to the order list</param>
        /// <returns>a custoemr to the view</returns>
        public ActionResult CustomerReport(string id)
        {
            if (id != null)
            {


                var user = uow.UserRepository.Get(m => m.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase));

                decimal totalSpent = 0.00m;

                foreach (var o in user.Order)
                {
                    totalSpent += o.Total;
                }


                CustomerReportViewModel crvm = new CustomerReportViewModel
                {
                    FirstName = user.FName,
                    LastName = user.LName,
                    OrderCount = user.Order.Count,
                    NumberOfFines = user.Fines.Count,
                    TotalSpent = totalSpent,
                    Username = user.UserName,
                    City = user.City,
                    Address = user.Address,
                    Postcode = user.PostalCode

                };

                return View(crvm);
            }
            else
            {
                return View();
            }

        }


       



        /// <summary>
        /// Method which generates a PDF report from data in the CustomerReport view
        /// </summary>
        /// <param name="username">filter which may be applied to customer</param>
        /// <returns>A PDF with the contents of the report</returns>
        public ActionResult GenerateCustomerPDF(string username)
        {
            var user = uow.UserRepository.Get(m => m.UserName.Equals(username, StringComparison.CurrentCultureIgnoreCase));

            decimal totalSpent = 0.00m;

            foreach (var o in user.Order)
            {
                totalSpent += o.Total;
            }


            CustomerReportViewModel crvm = new CustomerReportViewModel
            {
                FirstName = user.FName,
                LastName = user.LName,
                OrderCount = user.Order.Count,
                NumberOfFines = user.Fines.Count,
                TotalSpent = totalSpent,
                Username = user.UserName,
                City = user.City,
                Address = user.Address,
                Postcode = user.PostalCode

            };
            return new ViewAsPdf("CustomerReport", crvm)
            {
                FileName = "CustomerReport.pdf",
                CustomSwitches = "	--footer-center \"Page [page] of [toPage]\""

            };
        }


    }//end of controller
}