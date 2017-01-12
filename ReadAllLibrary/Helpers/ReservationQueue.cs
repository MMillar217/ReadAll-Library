using ReadAllLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.Helpers
{
    public class ReservationQueue<T>
    {
        //instance variables for start and end of queue
        public Node head;
        public Node tail;

        public int Size
        {
            get
            {
                int count = 0;
                for (Node current = head; current != null; current = current.nextNode)
                    count++;

                return count;
            }
        }//end size

        /// <summary>
        /// destroy linked list
        /// </summary>
        public void Destroy()
        {
            //instance variables
            Node temp = new Node();
            Node setNull = new Node();

            temp = head;//set temp node to head

            while (temp != null)
            {
                setNull = temp; //make setNull head
                temp = temp.nextNode; //set next node to temp each time to null
                setNull = null; //set null to nukk
            }

            head = null;
            tail = null;
        }

        /// <summary>
        /// returns true if no head node
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty() { return head == null; }

        /// <summary>
        /// linked list is dynamic so always false
        /// </summary>
        /// <returns></returns>
        public bool IsFull() { return false; }


        /// <summary>
        /// add object to queue
        /// </summary>
        /// <param name="o">entry being added</param>
        public void EnQueue(ReservationListUser o)
        {
            Node newNode = new Node();//create new node object
            newNode.dataItem = o;//add object data to node

            //if queue is empty
            if (tail == null)
            {
                head = newNode;
                tail = newNode;
            }

            //add to end
            else
            {
                tail.nextNode = newNode;
                tail = newNode;
            }
        }

        /// <summary>
        /// remove from queue
        /// </summary>
        /// <returns>data from head node</returns>
        public Object Dequeue()
        {
            //queue is empty
            //cant dequeue
            if (head == null)
            {
                return null;
            }
            else
            {
                Node temp = new Node();//instance of node named temp
                temp = head;//set temp to head node
                head = head.nextNode;//make previous node new head node.

                //if only item in queue queue is now empty
                if (head == null)
                {
                    tail = null;

                }

                return temp.dataItem;//returning value of temp node(which is value of head node)
            }
        }

    }//end of class
}//end of namespace