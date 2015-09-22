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


    [TestFixture, TestClass]
    public class EqualityTests
	{
		[Test, TestMethod]
		public void RDNCount()
		{
			DN dn1 = new DN("");
			
			Assert.AreEqual(dn1.RDNs.Length, 0);
			
			DN dn2 = new DN("CN=Pete");
			
			Assert.AreEqual(dn2.RDNs.Length, 1);
			
			DN dn3 = new DN("CN=Pete,OU=People,DC=example,DC=com");
			
			Assert.AreEqual(dn3.RDNs.Length, 4);
		}
		
		[Test, TestMethod]
		public void RDNComponentCount()
		{
			DN dn1 = new DN("CN=Pete,OU=People,DC=example,DC=com");
			
			Assert.AreEqual(dn1.RDNs[0].Components.Length, 1);
			
			DN dn2 = new DN("CN=Pete+SN=Everett,OU=People,DC=example,DC=com");
			
			Assert.AreEqual(dn2.RDNs[0].Components.Length, 2);
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


    [TestFixture, TestClass]
    public class QuotedStringTests
	{
		[Test, TestMethod]
		public void Single()
		{
			DN dn = new DN(@"OU=""Pete is cool""");
			
			Assert.AreEqual(dn.ToString(), "OU=Pete is cool");
		}
		
		[Test, TestMethod]
		public void Beginning()
		{
			DN dn = new DN(@"OU=""Pete"",OU=is,OU=cool");
			
			Assert.AreEqual(dn.ToString(), "OU=Pete,OU=is,OU=cool");
		}
		
		[Test, TestMethod]
		public void Middle()
		{
			DN dn = new DN(@"OU=Pete,OU=""is"",OU=cool");
			
			Assert.AreEqual(dn.ToString(), "OU=Pete,OU=is,OU=cool");
		}
		
		[Test, TestMethod]
		public void End()
		{
			DN dn = new DN(@"OU=Pete,OU=is,OU=""cool""");
			
			Assert.AreEqual(dn.ToString(), "OU=Pete,OU=is,OU=cool");
		}
		
		[Test, TestMethod]
		public void WithUnescapedSpecialChars()
		{
			DN dn = new DN(@"OU="",=+<>#; """);
			
			Assert.AreEqual(dn.ToString(), @"OU=\,\=\+\<\>\#\;\ ");
		}
		
		[Test, TestMethod]
		public void WithEscapedSpecialChars()
		{
			DN dn = new DN(@"OU=""\,\=\+\<\>\#\;\\\ """);
			
			Assert.AreEqual(dn.ToString(), @"OU=\,\=\+\<\>\#\;\\\ ");
		}
		
		[Test, TestMethod]
		[NUnit.Framework.ExpectedException(typeof(ArgumentException))]
        [Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(ArgumentException))]
        public void WithUnterminatedQuotedString()
		{
			DN dn = new DN(@"OU=""Pete is cool");
		}
	}
}
