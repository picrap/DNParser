/******************************************************************************
*
* CPI.DirectoryServices.RDN.cs
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
using System.IO;

namespace CPI.DirectoryServices
{

	/// <summary>
	/// a relative distinguished name (RDN) identifies an object in an LDAP directory, but does
	/// not identify its location in a tree.  A full DN is constructed from the RDN of an object
	/// plus the RDNs of all its container objects.
	///
	/// Multivalued RDNs are supported by this class.
	/// </summary>
	public class RDN
	{
		# region Enumerations
		
		private enum ParserState {DetermineAttributeType, GetTypeByOID, GetTypeByName, DetermineValueType, GetQuotedValue, GetUnquotedValue, GetHexValue};

		# endregion
	
		# region Data Members
		
		private readonly char[] charArray = new char[1];
		private int? hashCode;

		# endregion
		
		# region Properties
		
		
		/// <summary>
		/// Gets a collection of components that make up this RDN
		/// </summary>
		public IList<RDNComponent> Components { get; private set; }
		
		# endregion
		
		# region Construtors
		
		internal RDN(string rdnString)
		{
			ParseRDN(rdnString);
		}
		
		private RDN(IList<RDNComponent> components)
		{
			Components = new ReadOnlyCollection<RDNComponent>(components);
		}
		
		# endregion
	
		# region Methods
		
		/// <summary>
		/// Determines whether the specified object is equal to the current RDN
		/// </summary>
		/// <param name="obj">the object to compare to the current RDN</param>
		/// <returns>true if the specified object equals the current RDN; false otherwise</returns>
		public override bool Equals(object obj)
		{
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

			if (hashCode != obj.GetHashCode())
				return false;
		
			if (obj is RDN)
			{
				RDN rdnObj = (RDN)obj;
				
				if (rdnObj.Components.Count == Components.Count)
				{
					for (int i = 0; i < rdnObj.Components.Count; i++)
					{
						if (!(rdnObj.Components[i].Equals(Components[i])))
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
            if (!hashCode.HasValue)
            {
                // Start with a made-up seed
                hashCode = 0x74f8149a;

                for (int i = 0; i < Components.Count; i++)
                {
                    hashCode ^= Components[i].GetHashCode();
                }
            }

            return hashCode.Value;
		}

		/// <summary>
		/// Returns a string that represents the current RDN.
		/// </summary>
		/// <returns>a string that represents the current RDN</returns>
		public override string ToString()
		{
			return ToString(DN.DefaultEscapeChars);
		}
	
		/// <summary>
		/// Returns a string that represents the current RDN.
		/// </summary>
		/// <param name="escapeChars">the categories of characters to be escaped</param>
		/// <returns>a string that represents the current RDN</returns>
		public string ToString(EscapeChars escapeChars)
		{
			StringBuilder returnValue = new StringBuilder();
			
			foreach(RDNComponent component in Components)
			{
				returnValue.Append(component.ToString(escapeChars));
				returnValue.Append("+");
			}

			// Get rid of the last plus sign			
			if (returnValue.Length > 0)
				returnValue.Length--;
				
			return returnValue.ToString();
		}
		
		
		private void WriteUTF8Bytes(Stream s, char c)
		{
			charArray[0] = c;
			
			byte[] utf8Bytes = Encoding.UTF8.GetBytes(charArray);
			
			s.Write(utf8Bytes, 0, utf8Bytes.Length);
		}
		
		
		private void ParseRDN(string rdnString)
		{
			ArrayList rawTypes = new ArrayList();
			ArrayList rawValues = new ArrayList();
			ArrayList rawValueTypes = new ArrayList();
			
			MemoryStream rawData = new MemoryStream();
			
			ParserState state = ParserState.DetermineAttributeType;
			
			int position = 0;
			
			while(position < rdnString.Length)
			{
				switch(state)
				{
						# region case ParserState.DetermineAttributeType:
					
					case ParserState.DetermineAttributeType:
						
						# region Ignore any spaces at the beginning
						
						try
						{
							while (rdnString[position] == ' ')
								++position;
						}
						catch (IndexOutOfRangeException)
						{
							throw new ArgumentException("Invalid RDN: It's entirely blank spaces", rdnString);
						}	
						
						# endregion
						
						# region Are we looking at a letter?
						
						if (IsAlpha(rdnString[position]))
						{
							string startPoint = rdnString.Substring(position);
							
							// OID. is an optional string at the beginning of an object identifier.
							// if we find it, we ignore it, but we assume that the rest of the type
							// is going to be in dotted OID format.  We advance 3 positions, which puts
							// us at the dot, so at the next iteration, we start at the first number
							// of the OID.
							if (startPoint.StartsWith("OID.") ||
								startPoint.StartsWith("oid."))
							{
								position += 4;
								state = ParserState.GetTypeByOID;
							}
							else
							{
								// We're looking at the start of an attribute name.  We'll set the state
								// to GetTypeByName so we can parse the name from the beginning in the 
								// next iteration.
								state = ParserState.GetTypeByName;
							}
						}
						
							# endregion
						
							# region If not, are we looking at a digit?
						
						else if (IsDigit(rdnString[position]))
						{
							// If we're looking at a digit, that means we're looking at an OID.
							// We'll set the state to GetTypeByOID.
							state = ParserState.GetTypeByOID;
						}
						
							# endregion
						
							# region If not, we're looking at something that shouldn't be there.
						
						else
						{
							// If it's not a letter or a digit, then it's a wacky and invalid character,
							// and we'd do well to freak out.
							throw new ArgumentException("Invalid RDN: Invalid character in attribute name", rdnString);
						}
						
						# endregion
						
						break;
					
						# endregion
					
						# region case ParserState.GetTypeByName:
					
					case ParserState.GetTypeByName:
						
						try
						{
							# region The first character needs to be a letter
							
							if (IsAlpha(rdnString[position]))
							{
								WriteUTF8Bytes(rawData, rdnString[position]);
								position++;
							}
							else
							{
								throw new ArgumentException("Invalid Attribute Name: Name must start with a letter", rdnString);
							}
							
							# endregion
							
							# region The remaining characters can be letters, digits, or hyphens
							
							while (IsAlpha(rdnString[position]) || IsDigit(rdnString[position]) || rdnString[position] == '-')
							{
								WriteUTF8Bytes(rawData, rdnString[position]);
								position++;
							}

							# endregion
							
							# region The name can be followed by any number of blank spaces
							
							while (rdnString[position] == ' ')
							{
								position++;
							}
							
							# endregion
							
							# region And it needs to end with an equals sign
							
							// If we've found an equals sign, add the type name to the list, and
							// set the state to start looking for the attribute value.
							if (rdnString[position] == '=')
							{
								position++;
								rawTypes.Add(Encoding.UTF8.GetString(rawData.ToArray()));
								rawData.SetLength(0);
								
								if (position >= rdnString.Length)
								{
									// If we're at the end of the string, the RDN has an empty value.
									// We'll store a blank string, then we'll exit the loop gracefully.
									rawValues.Add("");
									rawValueTypes.Add(RDNComponent.RDNValueType.StringValue);
									state = ParserState.GetUnquotedValue;
								}
								else
								{
									state = ParserState.DetermineValueType;
								}
							}
							else
							{
								throw new ArgumentException("Invalid RDN: unterminated attribute name", rdnString);
							}
							
							# endregion
						}
						catch(IndexOutOfRangeException)
						{
							throw new ArgumentException("Invalid RDN: unterminated attribute name", rdnString);
						}

						break;
					
						# endregion	
					
						# region case ParserState.GetTypeByOID:
					
					case ParserState.GetTypeByOID:
						
						try
						{
							# region The first character needs to be a digit
							
							if (IsDigit(rdnString[position]))
							{
								WriteUTF8Bytes(rawData, rdnString[position]);
								position++;
							}
							else
							{
								throw new ArgumentException("Invalid Attribute OID: OID must start with a digit", rdnString);
							}
							
							# endregion
							
							# region The remaining characters need to be a digit or a period
							
							while (IsDigit(rdnString[position]) || rdnString[position] == '.')
							{
								WriteUTF8Bytes(rawData, rdnString[position]);
								position++;
							}	
							
							# endregion
							
							# region The OID can be followed by any number of blank spaces
							
							while (rdnString[position] == ' ')
							{
								position++;
							}
							
							# endregion
							
							# region And it needs to end with an equals sign
							
							// If we've found an equals sign, verify that the OID is well-formed,
							// then add it to the list of types.
							if (rdnString[position] == '=')
							{
								position++;
								
								string oid = Encoding.UTF8.GetString(rawData.ToArray());
							
								# region OIDs aren't allowed to end with a period
								
								if (oid.EndsWith("."))
								{
									throw new ArgumentException("Invalid RDN: OID cannot end with a period", rdnString);
								}
								
								# endregion
								
								# region You're also not allowed to have two periods in a row
								
								if (oid.IndexOf("..", StringComparison.Ordinal) > -1)
									throw new ArgumentException("Invalid RDN: OIDs cannot have two periods in a row", rdnString);

								# endregion
								
								# region Also, numbers aren't allowed to have a leading zero
								
								// Let's clarify that with an example.
								// The OID "12.3.0.2" is totally allowed, 
								// but "12.3.04.2" isn't.  It would have to be
								// "12.3.4.2".  If we see a leading zero, we don't just ignore it.
								// We complain about it.  LOUDLY.
								
								string[] oidPieces = oid.Split('.');
								
								foreach (string oidPiece in oidPieces)
								{
									if (oidPiece.Length > 1 && oidPiece[0] == '0')
										throw new ArgumentException("Invalid RDN: OIDs cannot have a leading zero", rdnString);
								}
								
								# endregion
							
								rawTypes.Add(oid);
								rawData.SetLength(0);
								state = ParserState.DetermineValueType;
							}
							else
							{
								throw new ArgumentException("Invalid RDN: unterminated attribute name", rdnString);
							}
							
							
							# endregion
						}
						catch(IndexOutOfRangeException)
						{
							throw new ArgumentException("Invalid RDN: unterminated attribute OID", rdnString);
						}
					
						break;
					
						# endregion
					
						# region case ParserState.DetermineValueType:
					
					case ParserState.DetermineValueType:
						
						try
						{
							# region Get rid of any leading spaces
						
							while (rdnString[position] == ' ')
							{
								position++;
							}
							
							# endregion
							
							# region Find out what the first character of the value is and set the state accordingly
							
							if (rdnString[position] == '"')
							{
								// It's a quoted string
								state = ParserState.GetQuotedValue;
							}
							else if (rdnString[position] == '#')
							{
								// It's a hex representation of a binary value
								state = ParserState.GetHexValue;
							}
							else
							{
								// It's a regular ol' unquoted string
								state = ParserState.GetUnquotedValue;
							}
							
							# endregion
						}
						catch (IndexOutOfRangeException)
						{
							// It's okay if we hit the end of the string without finding a value.
							// An empty value is allowed.  So we'll just drop this quietly and
							// go about our business, and the value will be reported as an empty string.
							state = ParserState.GetUnquotedValue;
							rawValues.Add("");
							rawValueTypes.Add(RDNComponent.RDNValueType.StringValue);
						}
						
						break;
					
						# endregion	
					
						# region case ParserState.GetHexValue:
					
					case ParserState.GetHexValue:

						# region The first character has to be a pound sign
						
						if (rdnString[position] == '#')
						{
							WriteUTF8Bytes(rawData, rdnString[position]);
							position++;
						}
						else
						{
							throw new ArgumentException("Invalid RDN: Hex values must start with #", rdnString);
						}
						
						# endregion
						
						# region The rest of the characters have to be hex digits
						
						while (position + 1 < rdnString.Length && IsHexDigit(rdnString[position]) && IsHexDigit(rdnString[position + 1]))
						{
							WriteUTF8Bytes(rawData, rdnString[position]);
							WriteUTF8Bytes(rawData, rdnString[position + 1]);
							position += 2;
						}
						
						# endregion
						
						# region Get rid of any trailing blank spaces
						
						while (position < rdnString.Length && rdnString[position] == ' ')
						{
							position++;
						}
						
						# endregion
						
						# region Check for end-of-string or + sign (which indicates a multi-valued RDN)
						
						if (position >= rdnString.Length)
						{
							// If we're at the end of the string, just store what we've found and 
							// we'll exit the loop gracefully.
							rawValues.Add(Encoding.UTF8.GetString(rawData.ToArray()));
							rawData.SetLength(0);
							rawValueTypes.Add(RDNComponent.RDNValueType.HexValue);
						}
						else if (rdnString[position] == '+')
						{
							// if we've found a plus sign, that means that there's another name/value
							// pair after it.  We'll store what we've found, advance to the next character,
							// and set the state to DetermineAttributeType.
							position++;
							rawValues.Add(Encoding.UTF8.GetString(rawData.ToArray()));
							rawData.SetLength(0);
							state = ParserState.DetermineAttributeType;
							rawValueTypes.Add(RDNComponent.RDNValueType.HexValue);
						}
						else
						{
							throw new ArgumentException("Invalid RDN: Invalid characters at end of value", rdnString);
						}
						
						# endregion
							
						break;
					
						# endregion
					
						# region case ParserState.GetQuotedValue:
					
					case ParserState.GetQuotedValue:
					
						# region The first character has to be a quote
						
						if (rdnString[position] == '"')
						{
							position++;
						}
						else
						{
							throw new ArgumentException("Invalid RDN: Quoted values must start with \"", rdnString);
						}
						
						# endregion
						
						# region Read characters until we find a closing quote
						
						try
						{
							while (rdnString[position] != '"')
							{
								if (rdnString[position] == '\\')
								{
									try
									{
										position++;
										
										if (IsHexDigit(rdnString[position]))
										{
											rawData.WriteByte(Convert.ToByte(rdnString.Substring(position, 2), 16));
											
											position += 2;
										}
										else if (IsSpecialChar(rdnString[position]) || rdnString[position] == ' ')
										{
											WriteUTF8Bytes(rawData, rdnString[position]);
											position++;
										}
										else
										{
											throw new ArgumentException("Invalid RDN: Escape sequence \\" + rdnString[position] + " is invalid.", rdnString);
										}
									}
									catch (IndexOutOfRangeException)
									{
										throw new ArgumentException("Invalid RDN: Invalid escape sequence", rdnString);
									}
								}
								else
								{
									WriteUTF8Bytes(rawData, rdnString[position]);
									position++;
								}
							}
						}
						catch (IndexOutOfRangeException)
						{
							throw new ArgumentException("Invalid RDN: Unterminated quoted value", rdnString);
						}

						position++;
												
						# endregion
						
						# region Remove any trailing spaces
						
						while (position < rdnString.Length && rdnString[position] == ' ')
						{
							position++;
						}
						
						# endregion
						
						# region Check for end-of-string or + sign (which indicates a multi-valued RDN)
						
						if (position >= rdnString.Length)
						{
							// If we're at the end of the string, just store what we've found and 
							// we'll exit the loop gracefully.
							rawValues.Add(Encoding.UTF8.GetString(rawData.ToArray()));
							rawData.SetLength(0);
							rawValueTypes.Add(RDNComponent.RDNValueType.StringValue);
						}
						else if (rdnString[position] == '+')
						{
							// if we've found a plus sign, that means that there's another name/value
							// pair after it.  We'll store what we've found, advance to the next character,
							// and set the state to DetermineAttributeType.
							rawValues.Add(Encoding.UTF8.GetString(rawData.ToArray()));
							rawData.SetLength(0);
							state = ParserState.DetermineAttributeType;
							rawValueTypes.Add(RDNComponent.RDNValueType.StringValue);
						}
						else
						{
							throw new ArgumentException("Invalid RDN: Invalid characters at end of value", rdnString);
						}
						
						# endregion
							
						break;
					
						# endregion	
					
						# region case ParserState.GetUnquotedValue:
					
					case ParserState.GetUnquotedValue:
					
						# region Is the first character an escaped space or pound sign?
						
						try
						{
							if (rdnString[position] == '\\' && (rdnString[position + 1] == ' ' || rdnString[position + 1] == '#'))
							{
								WriteUTF8Bytes(rawData, rdnString[position + 1]);
								position += 2;
							}
						}
						catch(IndexOutOfRangeException)
						{
							throw new ArgumentException("Invalid RDN: Invalid escape sequence", rdnString);
						}
						
						# endregion
						
						# region Read all characters, keeping track of trailing spaces
						
						int trailingSpaces = 0;
						
						while (position < rdnString.Length && rdnString[position] != '+')
						{
							if (rdnString[position] == ' ')
							{
								trailingSpaces++;
								
								WriteUTF8Bytes(rawData, rdnString[position]);
								
								position++;
							}
							else
							{
								trailingSpaces = 0;
								
								if (rdnString[position] == '\\')
								{
									try
									{
										position++;
										
										if (IsHexDigit(rdnString[position]))
										{
											rawData.WriteByte(Convert.ToByte(rdnString.Substring(position, 2), 16));
											
											position += 2;
										}
										else if (IsSpecialChar(rdnString[position]) || rdnString[position] == ' ')
										{
											WriteUTF8Bytes(rawData, rdnString[position]);
											position++;
										}
										else
										{
											throw new ArgumentException("Invalid RDN: Escape sequence \\" + rdnString[position] + " is invalid.", rdnString);
										}
									}
									catch (IndexOutOfRangeException)
									{
										throw new ArgumentException("Invalid RDN: Invalid escape sequence", rdnString);
									}
								}
								else if (IsSpecialChar(rdnString[position]))
								{
									throw new ArgumentException("Invalid RDN: Unquoted special character '" + rdnString[position] + "'", rdnString);
								}
								else
								{
									WriteUTF8Bytes(rawData, rdnString[position]);
									position++;
								}
							}
						}
						
						# endregion
						
						# region Remove any trailing spaces
						
						rawData.SetLength(rawData.Length - trailingSpaces);
						
						# endregion
						
						# region If the last character is a + sign, set state to look for another value, otherwise, finish up
						
						if (position < rdnString.Length)
						{
							if (rdnString[position] == '+')
							{
								// if we've found a plus sign, that means that there's another name/value
								// pair after it.  We'll store what we've found, advance to the next character,
								// and set the state to DetermineAttributeType.
								position++;
								rawValues.Add(Encoding.UTF8.GetString(rawData.ToArray()));
								rawData.SetLength(0);
								state = ParserState.DetermineAttributeType;
								rawValueTypes.Add(RDNComponent.RDNValueType.StringValue);
							}
							else
							{
								throw new ArgumentException("Invalid RDN: invalid trailing character", rdnString);
							}						
						}
						else
						{
							rawValues.Add(Encoding.UTF8.GetString(rawData.ToArray()));
							rawData.SetLength(0);
							rawValueTypes.Add(RDNComponent.RDNValueType.StringValue);
						}
						
						# endregion
					
						break;
					
						# endregion
				}
			}
			
			# region Check ending state
			
			if (!(state == ParserState.GetHexValue || 
				state == ParserState.GetQuotedValue || 
				state == ParserState.GetUnquotedValue
				))
			{
				throw new ArgumentException("Invalid RDN", rdnString);
			}
			
			# endregion
			
			# region Store the results we've collected
			
			RDNComponent[] componentArray = new RDNComponent[rawTypes.Count];
			
			for (int i = 0; i < componentArray.Length; i++)
			{
				componentArray[i] = new RDNComponent((string)rawTypes[i], (string)rawValues[i], (RDNComponent.RDNValueType)rawValueTypes[i]);
			}
			
			Components = new ReadOnlyCollection<RDNComponent>(componentArray);

			# endregion
		}
		
		
		# endregion
		
		# region Static Methods
		
		/// <summary>
		/// Determines whether the specified character is alphabetic (A-Z or a-z).
		/// </summary>
		/// <param name="c">the character to check</param>
		/// <returns>true if the specified character is alphabetic; false otherwise</returns>
		public static bool IsAlpha(char c)
		{
			return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
		}
		
		/// <summary>
		/// Determines whether the specified character is a numeric digit (0-9).
		/// </summary>
		/// <param name="c">the character to check</param>
		/// <returns>true if the specified character is a digit; false otherwise</returns>
		public static bool IsDigit(char c)
		{
			return (c >= '0' && c <= '9');
		}
		
		/// <summary>
		/// Determines whether the specified character is a hex digit (0-9 or A-F or a-f).
		/// </summary>
		/// <param name="c">the character to check</param>
		/// <returns>true if the specified character is a hex digit; false otherwise</returns>
		public static bool IsHexDigit(char c)
		{
			return (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');
		}
		
		/// <summary>
		/// Determines whether the specified character is an LDAP DN special character
		/// (, or = or + or &lt; or > or # or ; or \ or ")
		/// </summary>
		/// <param name="c">the character to check</param>
		/// <returns>true if the specified character is a special character; false otherwise</returns>
		public static bool IsSpecialChar(char c)
		{
			return c == ',' || c == '=' || c == '+' || c == '<' || c == '>' || c == '#' || c == ';' || c == '\\' || c == '"';
		}
		
		# endregion
		
		# region Overloaded Operators
		
		/// <summary>
		/// Checks to see whether two RDN objects are equal.
		/// </summary>
		/// <param name="obj1">an RDN object</param>
		/// <param name="obj2">an RDN object</param>
		/// <returns>true if the two objects are equal; false otherwise</returns>
		public static bool operator == (RDN obj1, RDN obj2)
		{
			if (ReferenceEquals(obj1, null))
			{
				return (ReferenceEquals(obj2, null));
			}
			else
			{
				return obj1.Equals(obj2);
			}
		}
		
		/// <summary>
		/// Checks to see whether two RDN objects are not equal.
		/// </summary>
		/// <param name="obj1">an RDN object</param>
		/// <param name="obj2">an RDN object</param>
		/// <returns>true if the two objects are not equal; false otherwise</returns>
		public static bool operator != (RDN obj1, RDN obj2)
		{
			return (!(obj1 == obj2));
		}
		
		# endregion
	}
}

