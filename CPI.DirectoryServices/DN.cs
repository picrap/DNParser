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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CPI.DirectoryServices
{
	/// <summary>
	/// A distinguished name (DN) is a name that uniquely identifies an object in an LDAP
	/// directory by using the relative distinguished name (RDN) of the object itself, plus
	/// the names of its container object.  A DN identifies the object as well as its location 
	/// in a tree.  An example of a distinguished name is CN=Pete,OU=People,DC=example,DC=com
	/// </summary>
	public class DN
	{
		# region Enumerations

		private enum ParserState {LookingForSeparator, InQuotedString};
		
		# endregion
		
		# region Static Data Members

		private static EscapeChars defaultEscapeChars = EscapeChars.ControlChars | EscapeChars.SpecialChars;
		
		# endregion
		
		# region Data Members
		
		private EscapeChars escapeChars;
		private int hashCode;
		
		# endregion
		
		# region Properties
		
		/// <summary>
		/// Gets or sets the categories of special characters that will be 
		/// escaped with a backslash when the DN is printed as a string.
		/// </summary>
		public EscapeChars CharsToEscape
		{
			get
			{
				return escapeChars;
			}
			set
			{
				escapeChars = value;
			}
		}
		
		/// <summary>
		/// Gets a list of all of the relative distinguished names that
		/// make up this distinguished name.
		/// </summary>
		public IList<RDN> RDNs { get; private set; }
		
		/// <summary>
		/// Gets a DN object representing the object that contains the current DN.
		/// </summary>
		public DN Parent
		{
			get
			{
				if (RDNs.Count >= 1)
				{
					RDN[] parentRDNs = new RDN[RDNs.Count - 1];
					
					for (int i = 0; i < parentRDNs.Length; i++)
					{
						parentRDNs[i] = RDNs[i + 1];
					}
					
					return new DN(new ReadOnlyCollection<RDN>(parentRDNs), this.CharsToEscape);
				}
				else
				{
					throw new InvalidOperationException("Can't get the parent of an empty DN");
				}
			}
		}
		
		# endregion
		
		# region Constructors
		
		/// <summary>
		/// Constructs a new DN object based on a string representation of an LDAP distinguished name.
		/// </summary>
		/// <param name="dnString">a string representation of a distinguished name</param>
		public DN(string dnString):this(dnString, DefaultEscapeChars) {}
		
		/// <summary>
		/// Constructs a new DN object based on a string representation of an LDAP distinguished name.
		/// </summary>
		/// <param name="dnString">a string representation of a distinguished name</param>
		/// <param name="escapeChars">the categories of special characters to be escaped when the DN is printed as a string</param>
		public DN(string dnString, EscapeChars escapeChars)
		{
            if (dnString == null)
            {
                throw new ArgumentNullException("dnString");
            }

			this.escapeChars = escapeChars;
		
			ParseDN(dnString);
			
			GenerateHashCode();
		}
		
		private DN(IList<RDN> rdnList, EscapeChars escapeChars)
		{
			this.escapeChars = escapeChars;

		    RDNs = new ReadOnlyCollection<RDN>(rdnList);
			
			GenerateHashCode();
		}
		
		# endregion
		
		# region Methods
		
		/// <summary>
		/// Determines whether the specified object is equal to the current DN
		/// </summary>
		/// <param name="obj">the object to compare to the current DN</param>
		/// <returns>true if the specified object equals the current DN; false otherwise</returns>
		public override bool Equals(object obj)
		{
            if (object.ReferenceEquals(obj, null))
            {
                return false;
            }

			if (hashCode != obj.GetHashCode())
				return false;
		
			if (obj is DN)
			{
				DN dnObj = (DN)obj;
				
				if (dnObj.RDNs.Count == this.RDNs.Count)
				{
					for (int i = 0; i < this.RDNs.Count; i++)
					{
						if (!(dnObj.RDNs[i].Equals(this.RDNs[i])))
							return false;	
					}
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Serves as a hash function, suitable for use in hashing algorithms and data structures like a hash table.
		/// </summary>
		/// <returns>a 32-bit integer representing the hash code of the current object</returns>
		public override int GetHashCode()
		{
			return hashCode;
		}

		private void GenerateHashCode()
		{
			// start with a made-up seed
			hashCode = 0x28f527b4;
			
			for (int i = 0; i < this.RDNs.Count; i++)
			{
				hashCode ^= this.RDNs[i].GetHashCode();
			}
		}
		
		/// <summary>
		/// Returns a string that represents the current DN.
		/// </summary>
		/// <returns>a string that represents the current DN</returns>
		public override string ToString()
		{
			return ToString(this.escapeChars);
		}
		
		/// <summary>
		/// Returns a string that represents the current DN.
		/// </summary>
		/// <param name="escapeChars">the categories of characters to be escaped</param>
		/// <returns>a string that represents the current DN</returns>
		public string ToString(EscapeChars escapeChars)
		{
			StringBuilder ReturnValue = new StringBuilder();
			
			foreach (RDN rdn in RDNs)
			{
				ReturnValue.Append(rdn.ToString(escapeChars));
				ReturnValue.Append(",");
			}
			
			// Remove the trailing comma
			if (ReturnValue.Length > 0)
				ReturnValue.Length--;
				
			return ReturnValue.ToString();
		}

		
		private void ParseDN(string dnString)
		{
			// If the string has nothing in it, that's allowed.  Just return an empty array.
			if (dnString.Length == 0)
			{
				RDNs = new RDN[0];
				return;  // That was easy...
			}
			
			// Break the DN down into its component RDNs.
			// Don't check the validity of the RDNs; just find the separators.
			
			ArrayList rawRDNs = new ArrayList();
			ParserState state = ParserState.LookingForSeparator;
			StringBuilder rawRDN = new StringBuilder();
			
			
			for (int position = 0; position < dnString.Length; ++position)
			{
				switch(state)
				{
					# region case ParserState.LookingForSeparator:
					case ParserState.LookingForSeparator:
						// If we find a separator character, we've hit the end of an RDN.
						// We'll store the RDN, and we'll check to see if the RDN is actually
						// valid later.
						if (dnString[position] == ',' || dnString[position] == ';')
						{
							rawRDNs.Add(rawRDN.ToString()); // Add the string to the list of raw RDNs
							rawRDN.Length = 0;              // Clear the StringBuilder to prepare for the next RDN
						}
						else
						{
							// Add the character to our temporary RDN string
							rawRDN.Append(dnString[position]);

							// If we find an escape character, store character that follows it,
							// but don't consider it as a possible separator character.  If the
							// string ends with an escape character, that's bad, and we should
							// throw an exception
							if (dnString[position] == '\\')
							{
								try
								{
									rawRDN.Append(dnString[++position]);
								}
								catch (IndexOutOfRangeException)
								{
									throw new ArgumentException("Invalid DN: DNs aren't allowed to end with an escape character.", dnString);
								}
							
							}
								// If we find a quote, we'll change state so that we look for the closing quote
								// and ignore any separator characters within.
							else if (dnString[position] == '"')
							{
								state = ParserState.InQuotedString;
							}
						}
						
						break;
						
					# endregion
					
					# region case ParserState.InQuotedString:
					
					case ParserState.InQuotedString:
						// Store the character
						rawRDN.Append(dnString[position]);
						
						// You're allowed to escape special characters in a quoted string, but not required
						// to.  But if there's an escaped quote, we need to take special care to make sure
						// that we don't mistake that for the end of the quoted string.
						if (dnString[position] == '\\')
						{
							try
							{
								rawRDN.Append(dnString[++position]);
							}
							catch (IndexOutOfRangeException)
							{
								throw new ArgumentException("Invalid DN: DNs aren't allowed to end with an escape character.", dnString);
							}
						}
						else if (dnString[position] == '"')
							state = ParserState.LookingForSeparator;
						break;
						
					# endregion
				}
			}
			
			// Take the last RDN and add it to the list
			rawRDNs.Add(rawRDN.ToString());
			
			// Check parser's end state
			if (state == ParserState.InQuotedString)
				throw new ArgumentException("Invalid DN: Unterminated quoted string.", dnString);
			
			RDN[] results = new RDN[rawRDNs.Count];

			for (int i = 0; i < results.Length; i++)
			{
				results[i] = new RDN(rawRDNs[i].ToString());
			}	
			
			RDNs = new ReadOnlyCollection<RDN>(results);
		}
		
		/// <summary>
		/// Checks whether the current DN is a parent object of the specified DN.
		///
		/// For example:
		/// The DN OU=People,DC=example,DC=com
		/// contains CN=Mike,OU=Marketing,OU=People,DC=example,DC=com
		///
		/// </summary>
		/// <param name="childDN">The DN object to check against the current object</param>
		/// <returns>true if childDN is a child of the current DN; false otherwise</returns>
		public bool Contains(DN childDN)
		{
			if (childDN.RDNs.Count > this.RDNs.Count)
			{
				int Offset = childDN.RDNs.Count - this.RDNs.Count;
				
				for (int i = 0; i < this.RDNs.Count; i++)
				{
					if (childDN.RDNs[i + Offset] != this.RDNs[i])
						return false;
				}
				
				return true;
			}
			else
			{
				return false;
			}
		}
		
		/// <summary>
		/// Gets a DN object representing a child object of the current DN.
		/// </summary>
		/// <param name="childRDN">a string representing the relative path to the child object</param>
		/// <returns>a DN object representing a child object of the current DN</returns>
		public DN GetChild(string childRDN)
		{
			DN childDN = new DN(childRDN);
			
			if (childDN.RDNs.Count > 0)
			{
				RDN[] fullPath = new RDN[this.RDNs.Count + childDN.RDNs.Count];
				
				for (int i = 0; i < childDN.RDNs.Count; i++)
				{
					fullPath[i] = childDN.RDNs[i];
				}
				for (int j = 0; j < this.RDNs.Count; j++)
				{
					fullPath[j + childDN.RDNs.Count] = this.RDNs[j];
				}
				
				return new DN(fullPath, this.CharsToEscape);
			}
			else
			{
				return this;
			}
		}
		
		
		# endregion
		
		# region Overloaded Operators
		
		/// <summary>
		/// Checks to see whether two DN objects are equal.
		/// </summary>
		/// <param name="obj1">a DN object</param>
		/// <param name="obj2">a DN object</param>
		/// <returns>true if the two objects are equal; false otherwise</returns>
		public static bool operator == (DN obj1, DN obj2)
		{
			if (object.ReferenceEquals(obj1, null))
			{
				return (object.ReferenceEquals(obj2, null));
			}
			else
			{
				return obj1.Equals(obj2);
			}
		}
		
		/// <summary>
		/// Checks to see whether two DN objects are not equal.
		/// </summary>
		/// <param name="obj1">a DN object</param>
		/// <param name="obj2">a DN object</param>
		/// <returns>true if the two objects are not equal; false otherwise</returns>
		public static bool operator != (DN obj1, DN obj2)
		{
			return (!(obj1 == obj2));
		}
		
		# endregion
		
		# region Static Properties
		
		/// <summary>
		/// Gets or sets the categories of characters that will be 
		/// escaped by default when a DN is converted to a string
		/// </summary>
		public static EscapeChars DefaultEscapeChars
		{
			get
			{
				return defaultEscapeChars;
			}
			set
			{
				defaultEscapeChars = value;
			}
		}
		
		# endregion
	}
}
