using System;
using CPI.DirectoryServices;

namespace SampleCode
{
	/// <summary>
	/// A couple of examples of the use of the DN class
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			# region Walking the Tree
			
			Console.WriteLine("Walking the Tree:");
			DN dn1 = new DN("CN=Pete,OU=People,DC=example,DC=com");
			Console.WriteLine(dn1);
			
			while (dn1.RDNs.Length > 0)
			{
				dn1 = dn1.Parent;
				Console.WriteLine(dn1);
			}
			
			Console.WriteLine("-------------------------------------------");
			
			# endregion
			
			# region Equality Test
			
			Console.WriteLine("Equality Test:");
			
			string dn2String = "CN = pete ; ou  = people;DC=exaMPLE, dc= COM";
			string dn3String = "cn=PETE, OU=People,dc=Example  ,   DC = com";
			
			Console.WriteLine(dn2String);
			Console.WriteLine(new DN(dn2String) == new DN(dn3String) ? "is equal to" : "is not equal to");
			Console.WriteLine(dn3String);
			
			Console.WriteLine("-------------------------------------------");
			
			# endregion
			
			# region Escaping Test
			
			Console.WriteLine("Escaping Test:");
			
			char MusicalNote = (char)9835;
			
			DN dn4 = new DN("CN=" + MusicalNote + " Music Man,OU=People,DC=example,DC=com");
			
			Console.WriteLine(dn4.ToString(EscapeChars.None));
			Console.WriteLine(dn4.ToString(EscapeChars.MultibyteChars));
			Console.WriteLine("-------------------------------------------");
			
			
			# endregion
		}
	}
}
