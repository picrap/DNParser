/******************************************************************************
*
* CPI.DirectoryServices.DN.cs
*
* By: Pete Everett (pete@CynicalPirate.com)
*
* (C) 2005 Pete Everett (http://www.CynicalPirate.com)
*
*******************************************************************************/
/******************************************************************************
 * Modified by: Thomas Würtz (thomas.wurtz@outlook.com) to support .NET Standard 2019-01-21
 ******************************************************************************/

using System;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace CPI.DirectoryServices.Tests
{
    [TestFixture]
    public class BaseTests
    {
        [Test]
        public void DCNaming()
        {
            DN dn = new DN(@"CN=Pete,OU=People,DC=example,DC=com");
			
            Assert.AreEqual(dn.ToString(), "CN=Pete,OU=People,DC=example,DC=com");
        }
		
        [Test]
        public void NonDCNaming()
        {
            DN dn = new DN(@"CN=Pete,OU=People,O=Example Inc.,C=US");
			
            Assert.AreEqual(dn.ToString(), "CN=Pete,OU=People,O=Example Inc.,C=US");
        }

        [Test]
        public void ExtraSpaces()
        {
            DN dn = new DN(@"    CN     =    Pete  , OU =  People,DC   =    example,DC = com");
			
            Assert.AreEqual(dn.ToString(), "CN=Pete,OU=People,DC=example,DC=com");
        }

        [Test]
        public void Semicolons()
        {
            DN dn = new DN(@"CN=Pete;OU=People,DC=example;DC=com");
			
            Assert.AreEqual(dn.ToString(), "CN=Pete,OU=People,DC=example,DC=com");
        }

        [Test]
        public void EmptyString()
        {
            DN dn = new DN("");
			
            Assert.AreEqual(dn.ToString(), "");
        }

        [Test]
        public void AllSpaces()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                DN dn = new DN("     ");
            });
        }

        [Test]
        public void TypeBeginsWithNumber()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                DN dn = new DN("3N=Pete");
            });
        }

        [Test]
        public void BlankRDNValue()
        {
            DN dn = new DN("CN=,OU=People,DC=example,DC=com");
			
            Assert.AreEqual(dn.ToString(), "CN=,OU=People,DC=example,DC=com");
        }

        [Test]
        public void MalformedRDN()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                DN dn = new DN("CN=Pete,People,DC=example,DC=com");
            });
        }

        [Test]
        public void HexEncodedBinaryValueSingle()
        {
            DN dn = new DN("CN=#324312af34e4");
			
            Assert.AreEqual(dn.ToString(), "CN=#324312af34e4");
        }

        [Test]
        public void HexEncodedBinaryValueBeginning()
        {
            DN dn = new DN("CN=#324312af34e4,OU=People,DC=example,DC=com");
			
            Assert.AreEqual(dn.ToString(), "CN=#324312af34e4,OU=People,DC=example,DC=com");
        }

        [Test]
        public void HexEncodedBinaryValueMiddle()
        {
            DN dn = new DN("CN=Pete,OU=#324312af34e4,DC=example,DC=com");
			
            Assert.AreEqual(dn.ToString(), "CN=Pete,OU=#324312af34e4,DC=example,DC=com");
        }

        [Test]
        public void HexEncodedBinaryValueEnd()
        {
            DN dn = new DN("CN=Pete,OU=People,DC=example,DC=#324312af34e4");
			
            Assert.AreEqual(dn.ToString(), "CN=Pete,OU=People,DC=example,DC=#324312af34e4");
        }
		
        [Test]
        public void InvalidCharsInHexEncodedBinaryValue()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                DN dn = new DN("CN=#34fer4,OU=People,DC=example,DC=com");
            });
        }
		
        [Test]
        public void InvalidHexEncodedBinaryValueLength()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                DN dn = new DN("CN=#35fe1,OU=People,DC=example,DC=com");
            });
        }
		
        [Test]
        public void MultvaluedRDN()
        {
            DN dn = new DN("CN=Pete + SN=Everett");
			
            Assert.AreEqual(dn.ToString(), "CN=Pete+SN=Everett");
        }
		
        [Test]
        public void GetChild()
        {
            DN baseDN = new DN("DC=example,DC=com");
            DN childDN = baseDN.GetChild("CN=pete,OU=people");
			
            DN fullDN = new DN("CN=pete,OU=people,DC=example,DC=com");
			
            Assert.AreEqual(childDN, fullDN);
        }
    }
}