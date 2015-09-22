/******************************************************************************
*
* CPI.DirectoryServices.DN.cs
*
* By: Pete Everett (pete@CynicalPirate.com)
*
* (C) 2005 Pete Everett (http://www.CynicalPirate.com)
*
*******************************************************************************/

using System;

namespace CPI.DirectoryServices
{
    /// <summary>
    /// Bit flag that represents different categories of special characters and 
    /// whether they should be escaped when the DN is displayed as a string.
    /// </summary>
    [Flags]
    public enum EscapeChars
    {
        /// <summary>
        /// No special characters will be escaped.
        /// </summary>
        None = 0, 
        /// <summary>
        /// Characters lower than ascii 32, such as tab and linefeed
        /// </summary>
        ControlChars = 1, 
        /// <summary>
        /// The disinguished name special characters ',', '=', '+', '&lt;', '>', '#', ';', '\\', '"'
        /// </summary>
        SpecialChars = 2, 
        /// <summary>
        /// Any characters >= 128, which are represented as multiple bytes in UTF-8
        /// </summary>
        MultibyteChars = 4
    }
}