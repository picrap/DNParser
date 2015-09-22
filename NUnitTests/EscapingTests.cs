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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace CPI.DirectoryServices.Tests
{
    [TestFixture, TestClass]
    public class EscapingTests
    {
        [Test, TestMethod]
        public void SpacesAtBeginningAndEnd()
        {
            DN dn = new DN(@"CN=\     Pete    \ ", EscapeChars.SpecialChars);
			
            Assert.AreEqual(dn.ToString(), @"CN=\     Pete    \ ");
			
            dn.CharsToEscape = EscapeChars.None;
			
            Assert.AreEqual(dn.ToString(), @"CN=     Pete     ");
        }
		
        [Test, TestMethod]
        public void SpecialCharacters()
        {
            DN dn = new DN(@"CN=\#Pound\,Comma\=Equals Sign\+Plus sign\<Less than\>Greater than\;Semicolon\\Backslash\""Quote", EscapeChars.SpecialChars);

            Assert.AreEqual(dn.ToString(), @"CN=\#Pound\,Comma\=Equals Sign\+Plus sign\<Less than\>Greater than\;Semicolon\\Backslash\""Quote");
			
            dn.CharsToEscape = EscapeChars.None;
			
            Assert.AreEqual(dn.ToString(), @"CN=#Pound,Comma=Equals Sign+Plus sign<Less than>Greater than;Semicolon\Backslash""Quote");
        }
		
        [Test, TestMethod]
        public void HexEscapeNonSpecialCharacter()
        {
            DN dn = new DN(@"CN=Pete\20Everett,OU=People,DC=example,DC=com");
			
            Assert.AreEqual(dn.ToString(), @"CN=Pete Everett,OU=People,DC=example,DC=com");
        }
		
        [Test, TestMethod]
        [NUnit.Framework.ExpectedException(typeof(ArgumentException))]
        [Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(ArgumentException))]
        public void UnescapedSpecialCharacter()
        {
            DN dn = new DN(@"CN=Winkin, Blinkin, and Nod,OU=People,DC=example,DC=com");
        }
		
        [Test, TestMethod]
        public void MultibyteCharacter()
        {
            char MusicalNote  = (char)0x266B;
			
            DN dn1 = new DN("CN=" + MusicalNote + " Music Man,OU=People,DC=example,DC=com", EscapeChars.MultibyteChars);
			
            Assert.AreEqual(dn1.ToString(), @"CN=\E2\99\AB Music Man,OU=People,DC=example,DC=com");
			
            dn1.CharsToEscape = EscapeChars.None;
			
            Assert.AreEqual(dn1.ToString(), @"CN=" + MusicalNote + " Music Man,OU=People,DC=example,DC=com");
			
            DN dn2 = new DN(@"CN=\E2\99\AB Music Man,OU=People,DC=example,DC=com");
			
            Assert.AreEqual(dn1, dn2);
        }
    }
}