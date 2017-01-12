using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.Helpers
{

    ///<summary>
    /// A simple list data structure
    /// </summary>

    interface ISimpleList
    {
        ///<summary>
        /// the number of entries in the list
        /// </summary>
        int Size
        {
            get;
        }

        ///<summary>
        /// tests whether list is full
        /// </summary>
        /// <returns> true if list is full</returns>
        bool IsFull();

        ///<summary>
        /// tests whether list is empty
        /// </summary>
        /// <returns> true if list is empty</returns>
        bool IsEmpty();


        ///<summary>
        /// Add object to beginning of list
        /// </summary>
        /// <param name ="index">the position</param>
        /// <param name ="o">the entry to add</param>
        void Add(int index, Object o);

        ///<summary>
        /// add to end of list
        /// </summary>
        /// <param name ="o">the entry to add</param>
        void Add(Object o);


        ///<summary>
        /// removes first entry that matches the target object
        /// </summary>
        /// <param name ="o">the object target</param>
        /// <returns></returns>
        bool Remove(Object o);


        ///<summary>
        /// returns index of first entry that matches the target object
        /// </summary>
        /// <param name ="o">the object target</param>
        /// <returns></returns>
        int IndexOf(Object o);

        ///<summary>
        /// gets entry at specified position
        /// </summary>
        /// <param name ="index">the position</param>
        /// <returns>the entry</returns>
        Object Get(int index);


        ///<summary>
        /// sets entry at specified position
        /// </summary>
        /// <param name ="index">the position</param>
        /// <param name ="o">the object target</param>
        /// <returns></returns>
        Object Set(int index, Object o);



    }
}