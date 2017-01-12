using ReadAllLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.DAL
{
    /// <summary>
    /// Unit of work class, implements IDisposable interface
    /// Class which maintains in memory updates to each repository and sends these as one transaction to the database
    /// </summary>
    public class UnitOfWork : IDisposable
    {

        private LibraryAppContext m_Context = null;
        private Repository<Book> bookRepository = null;
        private Repository<Review> reviewRepository = null;
        private Repository<ShoppingCart> shoppingCartRepository = null;
        private Repository<Cart> cartRepository = null;
        private Repository<Publisher> publisherRepository = null;
        private Repository<Payment> paymentRepository = null;
        private Repository<OrderLine> orderLineRepository = null;
        private Repository<ApplicationUser> userRepository = null;
        private Repository<Order> orderRepository = null;
        private Repository<BookCopy> bookCopyRepository = null;
        private Repository<Fine> fineRepository = null;
        private Repository<Supplier> supplierRepository = null;
        private Repository<ReservationList> reservationListRepository = null;
        private Repository<ReservationListUser> reservationListUserRepository = null;





        /// <summary>
        /// empty constructor
        /// creates a new instance of the context
        /// </summary>
        public UnitOfWork()
        {
            m_Context = new LibraryAppContext();
        }

        /// <summary>
        /// Commits any data changes to the database
        /// </summary>
        public void SaveChanges()
        {
            try
            {
                m_Context.SaveChanges();

            }
            catch (Exception ex) when (ex is DbEntityValidationException|| ex is DbUpdateException)
            {
                Console.WriteLine(ex);
               
                throw;
            }
        }


        public IRepository<Supplier> SupplierRepository
        {
            get
            {
                if (supplierRepository == null)
                {
                    supplierRepository = new Repository<Supplier>(m_Context);
                }
                return supplierRepository;
            }
        }

        public IRepository<ReservationList> ReservationListRepository
        {
            get
            {
                if (reservationListRepository == null)
                {
                    reservationListRepository = new Repository<ReservationList>(m_Context);
                }
                return reservationListRepository;
            }
        }

        public IRepository<ReservationListUser> ReservationListUserRepository
        {
            get
            {
                if (reservationListUserRepository == null)
                {
                    reservationListUserRepository = new Repository<ReservationListUser>(m_Context);
                }
                return reservationListUserRepository;
            }
        }

        public IRepository<Fine> FineRepository
        {
            get
            {
                if (fineRepository == null)
                {
                    fineRepository = new Repository<Fine>(m_Context);
                }
                return fineRepository;
            }
        }


        public IRepository<BookCopy> BookCopyRepository
        {
            get
            {
                if (bookCopyRepository == null)
                {
                    bookCopyRepository = new Repository<BookCopy>(m_Context);
                }
                return bookCopyRepository;
            }
        }


        public IRepository<Book> BookRepository
        {
            get
            {
                if (bookRepository == null)
                {
                    bookRepository = new Repository<Book>(m_Context);
                }
                return bookRepository;
            }
        }

        public IRepository<Order> OrderRepository
        {
            get
            {
                if (orderRepository == null)
                {
                    orderRepository = new Repository<Order>(m_Context);
                }
                return orderRepository;
            }
        }

        public IRepository<ApplicationUser> UserRepository
        {
            get
            {
                if (userRepository == null)
                {
                    userRepository = new Repository<ApplicationUser>(m_Context);
                }
                return userRepository;
            }
        }


        public IRepository<Payment> PaymentRepository
        {
            get
            {
                if (paymentRepository == null)
                {
                    paymentRepository = new Repository<Payment>(m_Context);
                }
                return paymentRepository;
            }
        }

        public IRepository<OrderLine> OrderLineRepository
        {
            get
            {
                if (orderLineRepository == null)
                {
                    orderLineRepository = new Repository<OrderLine>(m_Context);
                }
                return orderLineRepository;
            }
        }

        public Repository<Review> ReviewRepository
        {
            get
            {
                if (reviewRepository == null)
                {
                    reviewRepository = new Repository<Review>(m_Context);
                }
                return reviewRepository;
            }
        }

        public Repository<ShoppingCart> ShoppingCartRepository
        {
            get
            {
                if (shoppingCartRepository == null)
                {
                    shoppingCartRepository = new Repository<ShoppingCart>(m_Context);
                }
                return ShoppingCartRepository;
            }
        }

        public Repository<Cart> CartRepository
        {
            get
            {
                if (cartRepository == null)
                {
                    cartRepository = new Repository<Cart>(m_Context);
                }

                return cartRepository;
            }
        }

        public Repository<Publisher> PublisherRepository
        {
            get
            {
                if (publisherRepository == null)
                {
                    publisherRepository = new Repository<Publisher>(m_Context);
                }

                return publisherRepository;
            }
        }

        private bool disposed = false;

        /// <summary>
        /// handles the disposal of the database context
        /// </summary>
        /// <param name="disposing">true if disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    m_Context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


    }


}