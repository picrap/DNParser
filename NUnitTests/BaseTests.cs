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
    public class BaseTests
    {
        [Test, TestMethod]
        public void DCNaming()
        {
            DN dn = new DN(@"CN=Pete,OU=People,DC=example,DC=com");
			
            Assert.AreEqual(dn.ToString(), "CN=Pete,OU=People,DC=example,DC=com");
        }
		
        [Test, TestMethod]
        public void NonDCNaming()
        {
            DN dn = new DN(@"CN=Pete,OU=People,O=Example Inc.,C=US");
			
            Assert.AreEqual(dn.ToString(), "CN=Pete,OU=People,O=Example Inc.,C=US");
        }

        [Test, TestMethod]
        public void ExtraSpaces()
        {
            DN dn = new DN(@"    CN     =    Pete  , OU =  People,DC   =    example,DC = com");
			
            Assert.AreEqual(dn.ToString(), "CN=Pete,OU=People,DC=example,DC=com");
        }

        [Test, TestMethod]
        public void Semicolons()
        {
            DN dn = new DN(@"CN=Pete;OU=People,DC=example;DC=com");
			
            Assert.AreEqual(dn.ToString(), "CN=Pete,OU=People,DC=example,DC=com");
        }

        [Test, TestMethod]
        public void EmptyString()
        {
            DN dn = new DN("");
			
            Assert.AreEqual(dn.ToString(), "");
        }

        [Test, TestMethod]
        [NUnit.Framework.ExpectedException(typeof(ArgumentException))]
        [Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(ArgumentException))]
        public void AllSpaces()
        {
            DN dn = new DN("     ");
        }

        [Test, TestMethod]
        [NUnit.Framework.ExpectedException(typeof(ArgumentException))]
        [Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(ArgumentException))]
        public void TypeBeginsWithNumber()
        {
            DN dn = new DN("3N=Pete");
        }

        [Test, TestMethod]
        public void BlankRDNValue()
        {
            DN dn = new DN("CN=,OU=People,DC=example,DC=com");
			
            Assert.AreEqual(dn.ToString(), "CN=,OU=People,DC=example,DC=com");
        }

        [Test, TestMethod]
        [NUnit.Framework.ExpectedException(typeof(ArgumentException))]
        [Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(ArgumentException))]
        public void MalformedRDN()
        {
            DN dn = new DN("CN=Pete,People,DC=example,DC=com");
        }

        [Test, TestMethod]
        public void HexEncodedBinaryValueSingle()
        {
            DN dn = new DN("CN=#324312af34e4");
			
            Assert.AreEqual(dn.ToString(), "CN=#324312af34e4");
        }

        [Test, TestMethod]
        public void HexEncodedBinaryValueBeginning()
        {
            DN dn = new DN("CN=#324312af34e4,OU=People,DC=example,DC=com");
			
            Assert.AreEqual(dn.ToString(), "CN=#324312af34e4,OU=People,DC=example,DC=com");
        }

        [Test, TestMethod]
        public void HexEncodedBinaryValueMiddle()
        {
            DN dn = new DN("CN=Pete,OU=#324312af34e4,DC=example,DC=com");
			
            Assert.AreEqual(dn.ToString(), "CN=Pete,OU=#324312af34e4,DC=example,DC=com");
        }

        [Test, TestMethod]
        public void HexEncodedBinaryValueEnd()
        {
            DN dn = new DN("CN=Pete,OU=People,DC=example,DC=#324312af34e4");
			
            Assert.AreEqual(dn.ToString(), "CN=Pete,OU=People,DC=example,DC=#324312af34e4");
        }
		
        [Test, TestMethod]
        [NUnit.Framework.ExpectedException(typeof(ArgumentException))]
        [Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(ArgumentException))]
        public void InvalidCharsInHexEncodedBinaryValue()
        {
            DN dn = new DN("CN=#34fer4,OU=People,DC=example,DC=com");
        }
		
        [Test, TestMethod]
        [NUnit.Framework.ExpectedException(typeof(ArgumentException))]
        [Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(ArgumentException))]
        public void InvalidHexEncodedBinaryValueLength()
        {
            DN dn = new DN("CN=#35fe1,OU=People,DC=example,DC=com");
        }
		
        [Test, TestMethod]
        public void MultvaluedRDN()
        {
            DN dn = new DN("CN=Pete + SN=Everett");
			
            Assert.AreEqual(dn.ToString(), "CN=Pete+SN=Everett");
        }
		
        [Test, TestMethod]
        public void GetChild()
        {
            DN baseDN = new DN("DC=example,DC=com");
            DN childDN = baseDN.GetChild("CN=pete,OU=people");
			
            DN fullDN = new DN("CN=pete,OU=people,DC=example,DC=com");
			
            Assert.AreEqual(childDN, fullDN);
        }
    }
}