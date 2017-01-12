using ReadAllLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

/// <summary>
/// Namspace which holds classes involved in Data Access
/// </summary>
namespace ReadAllLibrary.DAL
{
    /// <summary>
    /// Generic Repository class whcih implements the IRepository Interface
    /// Contains the methods required for CRUD actions.
    /// </summary>
    /// <typeparam name="T">Class type</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        //Variables
        private LibraryAppContext m_Context = null;

        DbSet<T> m_DbSet;

        /// <summary>
        /// Constructor which creates a repository object
        /// </summary>
        /// <param name="context">Database Context</param>
        public Repository(LibraryAppContext context)
        {
            m_Context = context;
            m_DbSet = m_Context.Set<T>();
        }

        /// <summary>
        /// Generic method which retrieves all objects relating to repository type
        /// </summary>
        /// <param name="predicate">may be used to limit results if true</param>
        /// <returns></returns>
        public IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate != null)
            {
                return m_DbSet.Where(predicate);
            }

            return m_DbSet.AsEnumerable();
        }

        /// <summary>
        /// Gets a specific result based on predicate value
        /// </summary>
        /// <param name="predicate">true/false value for get method</param>
        /// <returns></returns>
        public T Get(Expression<Func<T, bool>> predicate)
        {
            return m_DbSet.FirstOrDefault(predicate);
        }

      
        /// <summary>
        /// Method which addes new entity to dataset
        /// </summary>
        /// <param name="entity">Entity to be added</param>
        public void Add(T entity)
        {
            m_DbSet.Add(entity);
        }

        /// <summary>
        /// Method which updates and entity within the dataset
        /// </summary>
        /// <param name="entity">updated Entity to replace previous object</param>
        public void Update(T entity)
        {
            m_DbSet.Attach(entity);
            ((IObjectContextAdapter)m_Context).ObjectContext.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);
        }

        /// <summary>
        /// Delete method used to delete entity from dataset
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        public void Delete(T entity)
        {
            m_DbSet.Remove(entity);
        }

        /// <summary>
        /// Counts the total number of objects in a dataset
        /// </summary>
        /// <returns>Total number of objects</returns>
        public int Count()
        {
            return m_DbSet.Count();
        }



    }
}