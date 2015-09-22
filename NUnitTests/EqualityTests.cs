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
    public class EqualityTests
    {
        [Test, TestMethod]
        public void RDNCount()
        {
            DN dn1 = new DN("");
			
            Assert.AreEqual(dn1.RDNs.Count, 0);
			
            DN dn2 = new DN("CN=Pete");
			
            Assert.AreEqual(dn2.RDNs.Count, 1);
			
            DN dn3 = new DN("CN=Pete,OU=People,DC=example,DC=com");
			
            Assert.AreEqual(dn3.RDNs.Count, 4);
        }
		
        [Test, TestMethod]
        public void RDNComponentCount()
        {
            DN dn1 = new DN("CN=Pete,OU=People,DC=example,DC=com");
			
            Assert.AreEqual(dn1.RDNs[0].Components.Count, 1);
			
            DN dn2 = new DN("CN=Pete+SN=Everett,OU=People,DC=example,DC=com");
			
            Assert.AreEqual(dn2.RDNs[0].Components.Count, 2);
        }
		
        [Test, TestMethod]
        public void EqualsMethod()
        {
            DN dn1 = new DN("Cn=PETE,oU=pEoPle,DC=exAMplE,Dc=cOM");
			
            DN dn2 = new DN("CN=pete,OU=people,DC=example,DC=com");
			
            DN dn3 = new DN("CN=peter,OU=people,DC=example,DC=com");
			
            Assert.IsTrue(dn1.Equals(dn2));
			
            Assert.IsFalse(dn1.Equals(dn3));
        }
		
        [Test, TestMethod]
        public void EqualsOperator()
        {
            DN dn1 = new DN("Cn=PETE,oU=pEoPle,DC=exAMplE,Dc=cOM");
			
            DN dn2 = new DN("CN=pete,OU=people,DC=example,DC=com");
			
            DN dn3 = new DN("CN=peter,OU=people,DC=example,DC=com");
			
            Assert.IsTrue(dn1 == dn2);
			
            Assert.IsFalse(dn1 == dn3);
        }
		
        [Test, TestMethod]
        public void NotEqualsOperator()
        {
            DN dn1 = new DN("Cn=PETE,oU=pEoPle ,   DC=exAMplE,Dc=cOM");
			
            DN dn2 = new DN("CN=pete,OU=people,DC=example,DC=com");
			
            DN dn3 = new DN("CN=peter,OU=people,DC=example,DC=com");
			
            Assert.IsFalse(dn1 != dn2);
			
            Assert.IsTrue(dn1 != dn3);
        }
		
        [Test, TestMethod]
        public void HashCode()
        {
            DN dn1 = new DN("Cn=PETE,oU=pEoPle,DC=exAMplE,Dc=cOM");
			
            DN dn2 = new DN("CN=pete,OU=people ,  DC=example,DC=com");
			
            DN dn3 = new DN("CN=peter,OU=people,DC=example,DC=com");
			
            Assert.AreEqual(dn1.GetHashCode(), dn2.GetHashCode());
			
            Assert.IsFalse(dn1.GetHashCode() == dn3.GetHashCode());
        }
        [Test, TestMethod]
        public void Parent()
        {
            DN dn1 = new DN("CN=Pete,OU=People,DC=example,DC=com");
			
            Assert.AreEqual(dn1.Parent.ToString(), "OU=People,DC=example,DC=com");
        }
	
        [Test, TestMethod]
        [NUnit.Framework.ExpectedException(typeof(InvalidOperationException))]
        [Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(InvalidOperationException))]
        public void ParentOfEmptyString()
        {
            DN dn = new DN("CN=Pete,OU=People,DC=example,DC=com");
			
            for (int i = 0; i <= 5; i++)
            {
                dn = dn.Parent;
            }
        }
        [Test, TestMethod]
        public void Contains()
        {
            DN dn1 = new DN("CN=Pete,OU=People,DC=example,DC=com");
            DN dn2 = new DN("OU=People,DC=example,DC=com");
            DN dn3 = new DN("DC=example,DC=com");
			
            Assert.IsTrue(dn2.Contains(dn1));
            Assert.IsTrue(dn3.Contains(dn1));
            Assert.IsTrue(dn3.Contains(dn2));
			
            Assert.IsFalse(dn1.Contains(dn2));
            Assert.IsFalse(dn1.Contains(dn3));
            Assert.IsFalse(dn2.Contains(dn3));
        }
    }
}