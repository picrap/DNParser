using System;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace CPI.DirectoryServices.Tests
{
    [TestFixture]
    public class QuotedStringTests
	{
		[Test]
		public void Single()
		{
			DN dn = new DN(@"OU=""Pete is cool""");
			
			Assert.AreEqual(dn.ToString(), "OU=Pete is cool");
		}
		
		[Test]
		public void Beginning()
		{
			DN dn = new DN(@"OU=""Pete"",OU=is,OU=cool");
			
			Assert.AreEqual(dn.ToString(), "OU=Pete,OU=is,OU=cool");
		}
		
		[Test]
		public void Middle()
		{
			DN dn = new DN(@"OU=Pete,OU=""is"",OU=cool");
			
			Assert.AreEqual(dn.ToString(), "OU=Pete,OU=is,OU=cool");
		}
		
		[Test]
		public void End()
		{
			DN dn = new DN(@"OU=Pete,OU=is,OU=""cool""");
			
			Assert.AreEqual(dn.ToString(), "OU=Pete,OU=is,OU=cool");
		}
		
		[Test]
		public void WithUnescapedSpecialChars()
		{
			DN dn = new DN(@"OU="",=+<>#; """);
			
			Assert.AreEqual(dn.ToString(), @"OU=\,\=\+\<\>\#\;\ ");
		}
		
		[Test]
		public void WithEscapedSpecialChars()
		{
			DN dn = new DN(@"OU=""\,\=\+\<\>\#\;\\\ """);
			
			Assert.AreEqual(dn.ToString(), @"OU=\,\=\+\<\>\#\;\\\ ");
		}
		
		[Test]
        public void WithUnterminatedQuotedString()
		{
		    Assert.Throws<ArgumentException>(() =>
		    {
		        DN dn = new DN(@"OU=""Pete is cool");
		    });
		}
	}
}
