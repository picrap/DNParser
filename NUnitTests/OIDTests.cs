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
 * Modified by: Thomas Würtz to support .NET Standard 2019-01-21
 ******************************************************************************/

using System;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace CPI.DirectoryServices.Tests
{
    [TestFixture]
    public class OIDTests
    {
        [Test]
        public void Single()
        {
            DN dn1 = new DN(@"OID.3.43.128=Pete");
            DN dn2 = new DN(@"3.43.128=Pete");
            DN dn3 = new DN(@"oid.3.43.128=Pete");
			
            Assert.AreEqual(dn1,dn2);
            Assert.AreEqual(dn2,dn3);
        }
		
        [Test]
        public void Beginning()
        {
            DN dn1 = new DN(@"OID.3.43.128=Pete,OU=People,DC=example,DC=com");
            DN dn2 = new DN(@"3.43.128=Pete,OU=People,DC=example,DC=com");
            DN dn3 = new DN(@"oid.3.43.128=Pete,OU=People,DC=example,DC=com");
			
            Assert.AreEqual(dn1,dn2);
            Assert.AreEqual(dn2,dn3);
        }
		
        [Test]
        public void Middle()
        {
            DN dn1 = new DN(@"CN=Pete,OID.3.43.128=Pete,OU=People,DC=example,DC=com");
            DN dn2 = new DN(@"CN=Pete,3.43.128=Pete,OU=People,DC=example,DC=com");
            DN dn3 = new DN(@"CN=Pete,oid.3.43.128=Pete,OU=People,DC=example,DC=com");
			
            Assert.AreEqual(dn1,dn2);
            Assert.AreEqual(dn2,dn3);
        }
		
        [Test]
        public void End()
        {
            DN dn1 = new DN(@"OU=People,DC=example,OID.3.43.128=com");
            DN dn2 = new DN(@"OU=People,DC=example,3.43.128=com");
            DN dn3 = new DN(@"OU=People,DC=example,oid.3.43.128=com");
			
            Assert.AreEqual(dn1,dn2);
            Assert.AreEqual(dn2,dn3);
        }
		
        [Test]
        public void MixedCase()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                DN dn = new DN("oId.3.23.1=Pete");
            });
        }
		
        [Test]
        public void LeadingZeroBeginning1()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                DN dn1 = new DN("03.23.1=Pete");
            });
        }
		
        [Test]
        public void LeadingZeroBeginning2()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                DN dn = new DN("OID.03.23.1=Pete");
            });
        }
		
        [Test]
        // This next one actually valid.  You're allowed to have a zero...just not a 
        // non-zero number with a leading zero.
        public void LeadingZeroBeginning3()
        {
            DN dn1 = new DN("0.23.1=Pete");
            DN dn2 = new DN("oid.0.23.1=Pete");
			
            Assert.AreEqual(dn1, dn2);
        }
		
        [Test]
        public void LeadingZeroMiddle1()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                DN dn = new DN("34.03.21=Pete");
            });
        }
		
        [Test]
        public void LeadingZeroMiddle2()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                DN dn = new DN("OID.34.03.21=Pete");
            });
        }
		
        [Test]
        public void LeadingZeroMiddle3()
        {
            DN dn1 = new DN("34.0.21=Pete");
            DN dn2 = new DN("OID.34.0.21=Pete");
			
            Assert.AreEqual(dn1, dn2);
        }
		
        [Test]
        public void LeadingZeroEnd1()
        {

            Assert.Throws<ArgumentException>(() =>
            {
                DN dn = new DN("32.21.05=Pete");
            });
        }
		
        [Test]
        public void LeadingZeroEnd2()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                DN dn = new DN("OID.32.21.05=Pete");
            });
        }
		
        [Test]
        public void LeadingZeroEnd3()
        {
            DN dn1 = new DN("32.21.0=Pete");
            DN dn2 = new DN("OID.32.21.0=Pete");
			
            Assert.AreEqual(dn1, dn2);
        }
		
        [Test]
        public void Spaces()
        {
            DN dn1 = new DN("   32.21.0   =    Pete");
            DN dn2 = new DN("OID.32.21.0=Pete");
            DN dn3 = new DN("oid.32.21.0   =  Pete     ");
			
            Assert.AreEqual(dn1, dn2);
            Assert.AreEqual(dn2, dn3);
        }
		
        [Test]
        public void TrailingPeriod()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                DN dn = new DN("OID.34.54.15.=Pete");
            });
        }
		
        [Test]
        public void AdjacentPeriods()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                DN dn = new DN("34..32.15=Pete");
            });
        }
		
        [Test]
        // This is allowed.  We should only check for an oid string if it starts with
        // OID (or oid) followed by a period.
        public void NotQuiteAnOID()
        {
            DN dn = new DN("OID33A-2=Pete");
			
            Assert.AreEqual(dn.ToString(), "OID33A-2=Pete");
        }
    }
}