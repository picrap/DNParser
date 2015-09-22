/******************************************************************************
*
* CPI.DirectoryServices.DN.cs
*
* By: Pete Everett (pete@CynicalPirate.com)
*
* (C) 2005 Pete Everett (http://www.CynicalPirate.com)
*
*******************************************************************************/

using System.Collections;

namespace CPI.DirectoryServices
{
    /// <summary>
    /// A read-only collection of RDNComponent objects
    /// </summary>
    public class RDNComponentList : IEnumerable
    {
        # region Data Members
		
        private RDNComponent[] components;
		
        # endregion
		
        # region Constructors
		
        internal RDNComponentList(RDNComponent[] components)
        {
            this.components = components;
        }
		
        # endregion
		
        # region Properties
		
        /// <summary>
        /// Gets a 32-bit integer that represents the total number of elements in the collection.
        /// </summary>
        public int Length
        {
            get
            {
                return components.Length;
            }
        }
		
        /// <summary>
        /// Gets the RDNComponent object at the specified index
        /// </summary>
        public RDNComponent this[int index]
        {
            get
            {
                return components[index];
            }
        }
		
        # endregion

        #region IEnumerable Members

        /// <summary>
        /// Gets an enumerator object for the current RDNComponentList
        /// </summary>
        /// <returns>an enumerator object for the current RDNComponentList</returns>
        public IEnumerator GetEnumerator()
        {
            return components.GetEnumerator();
        }

        #endregion
    }
}