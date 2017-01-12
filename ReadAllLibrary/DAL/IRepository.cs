using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ReadAllLibrary.DAL
{

    /// <summary>
    /// Interface which defines the method signatures used to implement generic repository
    /// </summary>
    /// <typeparam name="T">Class Type</typeparam>
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate = null);
        T Get(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        int Count();
    }
}