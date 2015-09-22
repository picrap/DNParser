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
    public class OIDTests
    {
        [Test, TestMethod]
        public void Single()
        {
            DN dn1 = new DN(@"OID.3.43.128=Pete");
            DN dn2 = new DN(@"3.43.128=Pete");
            DN dn3 = new DN(@"oid.3.43.128=Pete");
			
            Assert.AreEqual(dn1,dn2);
            Assert.AreEqual(dn2,dn3);
        }
		
        [Test, TestMethod]
        public void Beginning()
        {
            DN dn1 = new DN(@"OID.3.43.128=Pete,OU=People,DC=example,DC=com");
            DN dn2 = new DN(@"3.43.128=Pete,OU=People,DC=example,DC=com");
            DN dn3 = new DN(@"oid.3.43.128=Pete,OU=People,DC=example,DC=com");
			
            Assert.AreEqual(dn1,dn2);
            Assert.AreEqual(dn2,dn3);
        }
		
        [Test, TestMethod]
        public void Middle()
        {
            DN dn1 = new DN(@"CN=Pete,OID.3.43.128=Pete,OU=People,DC=example,DC=com");
            DN dn2 = new DN(@"CN=Pete,3.43.128=Pete,OU=People,DC=example,DC=com");
            DN dn3 = new DN(@"CN=Pete,oid.3.43.128=Pete,OU=People,DC=example,DC=com");
			
            Assert.AreEqual(dn1,dn2);
            Assert.AreEqual(dn2,dn3);
        }
		
        [Test, TestMethod]
        public void End()
        {
            DN dn1 = new DN(@"OU=People,DC=example,OID.3.43.128=com");
            DN dn2 = new DN(@"OU=People,DC=example,3.43.128=com");
            DN dn3 = new DN(@"OU=People,DC=example,oid.3.43.128=com");
			
            Assert.AreEqual(dn1,dn2);
            Assert.AreEqual(dn2,dn3);
        }
		
        [Test, TestMethod]
        [NUnit.Framework.ExpectedException(typeof(ArgumentException))]
        [Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(ArgumentException))]
        public void MixedCase()
        {
            DN dn = new DN("oId.3.23.1=Pete");
        }
		
        [Test, TestMethod]
        [NUnit.Framework.ExpectedException(typeof(ArgumentException))]
        [Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(ArgumentException))]
        public void LeadingZeroBeginning1()
        {
            DN dn1 = new DN("03.23.1=Pete");
        }
		
        [Test, TestMethod]
        [NUnit.Framework.ExpectedException(typeof(ArgumentException))]
        [Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(ArgumentException))]
        public void LeadingZeroBeginning2()
        {
            DN dn = new DN("OID.03.23.1=Pete");
        }
		
        [Test, TestMethod]
        // This next one actually valid.  You're allowed to have a zero...just not a 
        // non-zero number with a leading zero.
        public void LeadingZeroBeginning3()
        {
            DN dn1 = new DN("0.23.1=Pete");
            DN dn2 = new DN("oid.0.23.1=Pete");
			
            Assert.AreEqual(dn1, dn2);
        }
		
        [Test, TestMethod]
        [NUnit.Framework.ExpectedException(typeof(ArgumentException))]
        [Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(ArgumentException))]
        public void LeadingZeroMiddle1()
        {
            DN dn = new DN("34.03.21=Pete");
        }
		
        [Test, TestMethod]
        [NUnit.Framework.ExpectedException(typeof(ArgumentException))]
        [Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(ArgumentException))]
        public void LeadingZeroMiddle2()
        {
            DN dn = new DN("OID.34.03.21=Pete");
        }
		
        [Test, TestMethod]
        public void LeadingZeroMiddle3()
        {
            DN dn1 = new DN("34.0.21=Pete");
            DN dn2 = new DN("OID.34.0.21=Pete");
			
            Assert.AreEqual(dn1, dn2);
        }
		
        [Test, TestMethod]
        [NUnit.Framework.ExpectedException(typeof(ArgumentException))]
        [Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(ArgumentException))]
        public void LeadingZeroEnd1()
        {
            DN dn = new DN("32.21.05=Pete");
        }
		
        [Test, TestMethod]
        [NUnit.Framework.ExpectedException(typeof(ArgumentException))]
        [Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(ArgumentException))]
        public void LeadingZeroEnd2()
        {
            DN dn = new DN("OID.32.21.05=Pete");
        }
		
        [Test, TestMethod]
        public void LeadingZeroEnd3()
        {
            DN dn1 = new DN("32.21.0=Pete");
            DN dn2 = new DN("OID.32.21.0=Pete");
			
            Assert.AreEqual(dn1, dn2);
        }
		
        [Test, TestMethod]
        public void Spaces()
        {
            DN dn1 = new DN("   32.21.0   =    Pete");
            DN dn2 = new DN("OID.32.21.0=Pete");
            DN dn3 = new DN("oid.32.21.0   =  Pete     ");
			
            Assert.AreEqual(dn1, dn2);
            Assert.AreEqual(dn2, dn3);
        }
		
        [Test, TestMethod]
        [NUnit.Framework.ExpectedException(typeof(ArgumentException))]
        [Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(ArgumentException))]
        public void TrailingPeriod()
        {
            DN dn = new DN("OID.34.54.15.=Pete");
        }
		
        [Test, TestMethod]
        [NUnit.Framework.ExpectedException(typeof(ArgumentException))]
        [Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(ArgumentException))]
        public void AdjacentPeriods()
        {
            DN dn = new DN("34..32.15=Pete");
        }
		
        [Test, TestMethod]
        // This is allowed.  We should only check for an oid string if it starts with
        // OID (or oid) followed by a period.
        public void NotQuiteAnOID()
        {
            DN dn = new DN("OID33A-2=Pete");
			
            Assert.AreEqual(dn.ToString(), "OID33A-2=Pete");
        }
    }
}