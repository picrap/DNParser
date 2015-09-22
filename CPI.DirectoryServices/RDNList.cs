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
    /// A read-only collection of RDN objects
    /// </summary>
    public class RDNList : IEnumerable
    {
        # region Data Members
		
        private RDN[] rDNs;
		
        # endregion
		
        # region Constructors
		
        internal RDNList(RDN[] rDNs)
        {
            this.rDNs = rDNs;
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
                return rDNs.Length;
            }
        }
		
        /// <summary>
        /// Gets the RDN object at the specified index
        /// </summary>
        public RDN this[int index]
        {
            get
            {
                return rDNs[index];
            }
        }
		
        # endregion
		
        #region IEnumerable Members

        /// <summary>
        /// Gets an enumerator object for the current RDNList
        /// </summary>
        /// <returns>an enumerator object for the current RDNList</returns>
        public IEnumerator GetEnumerator()
        {
            return rDNs.GetEnumerator();
        }

        #endregion
    }
}