/******************************************************************************
*
* CPI.DirectoryServices.RDNComponent.cs
*
* By: Pete Everett (pete@CynicalPirate.com)
*
* (C) 2005 Pete Everett (http://www.CynicalPirate.com)
*
*******************************************************************************/

using System;
using System.Text;
using System.IO;

namespace CPI.DirectoryServices
{

	/// <summary>
	/// Encapsulates a component of an RDN.  Most RDNs are made up of a single component,
	/// but a multivalued RDN has multiple components, separated by a plus sign.
	/// </summary>
	public class RDNComponent
	{
		# region Enumerations
		
		/// <summary>
		/// Specifies whether the value of the component is a string value, or a binary
		/// value represented as a string of hex digits
		/// </summary>
		public enum RDNValueType 
		{
			/// <summary>
			/// Represents a string value
			/// </summary>
			StringValue, 
			/// <summary>
			/// Represents a hex-encoded binary value
			/// </summary>
			HexValue
		};
		
		# endregion
	
		# region Data Members
		
		private string componentType;
		private string componentValue;
		private RDNValueType componentValueType;
		private int hashCode;
		
		# endregion
		
		# region Properties
		
		/// <summary>
		/// Gets the RDN component type ('CN', 'OU', 'DC', etc.)
		/// </summary>
		public string ComponentType
		{
			get
			{
				return componentType;
			}
		}
		
		/// <summary>
		/// Gets the value of the RDN component
		/// </summary>
		public string ComponentValue
		{
			get
			{
				return componentValue;
			}
		}
		
		/// <summary>
		/// Gets the encoding type of the RDN value (either a string or hex-encoded binary value)
		/// </summary>
		public RDNValueType ComponentValueType
		{
			get
			{
				return componentValueType;
			}
		}
		
		# endregion
		
		# region Constructors
		
		internal RDNComponent(string ComponentType, string ComponentValue, RDNValueType ComponentValueType)
		{
			componentType = ComponentType;
			componentValue = ComponentValue;
			componentValueType = ComponentValueType;
			
			GenerateHashCode();
		}
		
		# endregion
		
		# region Methods
		
		/// <summary>
		/// Determines whether the specified object is equal to the current RDNComponent
		/// </summary>
		/// <param name="obj">the object to compare to the current RDNComponent</param>
		/// <returns>true if the specified object equals the current RDNComponent; false otherwise</returns>
		public override bool Equals(object obj)
		{
            if (object.ReferenceEquals(obj, null))
            {
                return false;
            }

			if (this.hashCode != obj.GetHashCode())
				return false;
		
			if (obj is RDNComponent)
			{
				RDNComponent rdnObj = (RDNComponent)obj;

				return (rdnObj.componentValueType == this.componentValueType &&
					string.Compare(rdnObj.componentType, this.componentType, true) == 0 &&
					string.Compare(rdnObj.componentValue, this.componentValue, true) == 0
					);
			}
			else
			{
				return false;
			}
		}
		
		private void GenerateHashCode()
		{
			// start with a made-up seed
			hashCode = 0x48012e7a;
			
			hashCode ^= this.componentValueType.GetHashCode();
			
			if (this.componentType != null)
				hashCode ^= this.componentType.ToLower().GetHashCode();
				
			if (this.componentValue != null)
				hashCode ^= this.componentValue.ToLower().GetHashCode();
		}
		
		/// <summary>
		/// Serves as a hash function, suitable for use in hashing algorithms and data structures like a hash table.
		/// </summary>
		/// <returns>a 32-bit integer representing the hash code of the current object</returns>
		public override int GetHashCode()
		{
			return hashCode;
		}

		/// <summary>
		/// Returns a string that represents the current RDNComponent.
		/// </summary>
		/// <returns>a string that represents the current RDNComponent</returns>
		public override string ToString()
		{
			return ToString(DN.DefaultEscapeChars);
		}
		
		/// <summary>
		/// Returns a string that represents the current RDNComponent.
		/// </summary>
		/// <param name="escapeChars">the categories of characters to be escaped</param>
		/// <returns>a string that represents the current RDNComponent</returns>
		public string ToString(EscapeChars escapeChars)
		{
			if (this.componentValueType == RDNValueType.HexValue)
			{
				return this.componentType + "=" + this.componentValue;
			}
			else
			{
				return this.componentType + "=" + EscapeValue(this.componentValue, escapeChars);
			}
		}
		
		
		# endregion
		
		# region Static Methods
		
		/// <summary>
		/// Takes an input string and escapes the specified categories of characters.
		/// </summary>
		/// <param name="s">the input string</param>
		/// <param name="escapeChars">the categories of characters to escape</param>
		/// <returns>an escaped string</returns>
		public static string EscapeValue(string s, EscapeChars escapeChars)
		{
			StringBuilder ReturnValue = new StringBuilder();
			
			for(int i = 0; i < s.Length; i++)
			{
				if (RDN.IsSpecialChar(s[i]) || ((i == 0 || i == s.Length - 1) && s[i] == ' '))
				{
					if ((escapeChars & EscapeChars.SpecialChars) != EscapeChars.None)
						ReturnValue.Append('\\');

					ReturnValue.Append(s[i]);
				}
				else if (s[i] < 32 && ((escapeChars & EscapeChars.ControlChars) != EscapeChars.None))
				{
					ReturnValue.AppendFormat("\\{0:X2}", (int)s[i]);
				}
				else if (s[i] >= 128 && ((escapeChars & EscapeChars.MultibyteChars) != EscapeChars.None))
				{
					byte[] Bytes = Encoding.UTF8.GetBytes(new char[]{s[i]});
				
					foreach (byte b in Bytes)
					{
						ReturnValue.AppendFormat("\\{0:X2}", b);
					}
				}
				else
				{
					ReturnValue.Append(s[i]);
				}
			}
			
			return ReturnValue.ToString();
		}
		
		
		# endregion
		
		# region Overloaded Operators
		
		/// <summary>
		/// Checks to see whether two RDNComponent objects are equal.
		/// </summary>
		/// <param name="obj1">an RDNComponent object</param>
		/// <param name="obj2">an RDNComponent object</param>
		/// <returns>true if the two objects are equal; false otherwise</returns>
		public static bool operator == (RDNComponent obj1, RDNComponent obj2)
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
		/// Checks to see whether two RDNComponent objects are not equal.
		/// </summary>
		/// <param name="obj1">an RDNComponent object</param>
		/// <param name="obj2">an RDNComponent object</param>
		/// <returns>true if the two objects are not equal; false otherwise</returns>
		public static bool operator != (RDNComponent obj1, RDNComponent obj2)
		{
			return (!(obj1 == obj2));
		}
		
		# endregion
	}
}

