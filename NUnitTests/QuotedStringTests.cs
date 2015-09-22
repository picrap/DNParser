using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace CPI.DirectoryServices.Tests
{
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
