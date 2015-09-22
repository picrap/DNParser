/******************************************************************************
*
* CPI.DirectoryServices.RDNComponent.cs
*
* By: Pete Everett (pete@CynicalPirate.com)
*
* (C) 2005 Pete Everett (http://www.CynicalPirate.com)
*
*******************************************************************************/

namespace CPI.DirectoryServices
{
    /// <summary>
    /// Specifies whether the value of the component is a string value, or a binary
    /// value represented as a string of hex digits
    /// </summary>
    public enum RDNValueType 
    {
        /// <summary>
        /// Represents a string value
        /// </summary>
        StringValue, 
        /// <summary>
        /// Represents a hex-encoded binary value
        /// </summary>
        HexValue
    };
}