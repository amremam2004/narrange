/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 * Copyright (c) 2007-2008 James Nies and NArrange contributors. 	      
 * 	    All rights reserved.                   				      
 *                                                                             
 * This program and the accompanying materials are made available under       
 * the terms of the Common Public License v1.0 which accompanies this         
 * distribution.							      
 *                                                                             
 * Redistribution and use in source and binary forms, with or                 
 * without modification, are permitted provided that the following            
 * conditions are met:                                                        
 *                                                                             
 * Redistributions of source code must retain the above copyright             
 * notice, this list of conditions and the following disclaimer.              
 * Redistributions in binary form must reproduce the above copyright          
 * notice, this list of conditions and the following disclaimer in            
 * the documentation and/or other materials provided with the distribution.   
 *                                                                             
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS        
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT          
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS          
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT   
 * OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,      
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED   
 * TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,        
 * OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY     
 * OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING    
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS         
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.               
 *                                                                             
 * Contributors:
 *      James Nies
 *      - Initial creation
 *      - Added a Format method for getting a formatted string representation 
 *        of a code element
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NArrange.Core.Configuration;

namespace NArrange.Core.CodeElements
{
	/// <summary>
	/// Element utility methods
	/// </summary>
	public static class ElementUtilities
	{
		#region Static Fields

		private static Dictionary<Enum, string> _enumStrings = 
            new Dictionary<Enum,string>();

		#endregion Static Fields

		#region Private Methods

		private static string EnumToString(Enum enumValue)
		{
			string enumString = null;

			if (!(_enumStrings.TryGetValue(enumValue, out enumString)))
			{
			    //
			    // Cache the string representation of the enumeration
			    //
			    enumString = enumValue.ToString();
			    _enumStrings[enumValue] = enumString;
			}

			return enumString;
		}

		private static string GetTypeAttribute(ICodeElement codeElement)
		{
			string attributeString = string.Empty;

			MemberElement memberElement = codeElement as MemberElement;
			if (memberElement != null)
			{
			    attributeString = memberElement.ReturnType;
			}
			else
			{
			    TypeElement typeElement = codeElement as TypeElement;
			    if (typeElement != null)
			    {
			        attributeString = EnumUtilities.ToString(typeElement.TypeElementType);
			    }
			}

			return attributeString;
		}

		#endregion Private Methods

		#region Public Methods

		/// <summary>
		/// Gets a string representation of a code element using the specified
		/// format.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="codeElement"></param>
		/// <returns></returns>
		public static string Format(string format, ICodeElement codeElement)
		{
			if (format == null)
			{
			    throw new ArgumentNullException("format");
			}
			else if (codeElement == null)
			{
			    throw new ArgumentNullException("codeElement");
			}

			StringBuilder formatted = new StringBuilder(format.Length * 2);
			StringBuilder attributeBuilder = null;
			bool inAttribute = false;

			using (StringReader reader = new StringReader(format))
			{
			    int data = reader.Read();
			    while (data > 0)
			    {
			        char ch = (char)data;

			        if (ch == AttributeExpression.ExpressionPrefix &&
			            (char)(reader.Peek()) == ConditionExpressionParser.ExpressionStart)
			        {
			            reader.Read();
			            attributeBuilder = new StringBuilder(16);
			            inAttribute = true;
			        }
			        else if (inAttribute)
			        {
			            if (ch == ConditionExpressionParser.ExpressionEnd)
			            {
			                ElementAttributeType elementAttribute = (ElementAttributeType)Enum.Parse(
			                    typeof(ElementAttributeType), attributeBuilder.ToString());

			                string attribute = GetAttribute(elementAttribute, codeElement);
			                formatted.Append(attribute);
			                attributeBuilder = new StringBuilder(16);
			                inAttribute = false;
			            }
			            else
			            {
			                attributeBuilder.Append(ch);
			            }
			        }
			        else
			        {
			            formatted.Append(ch);
			        }

			        data = reader.Read();
			    }
			}

			return formatted.ToString();
		}

		/// <summary>
		/// Gets the string representation of a code element attribute.
		/// </summary>
		/// <param name="attributeType"></param>
		/// <param name="codeElement"></param>
		/// <returns></returns>
		public static string GetAttribute(ElementAttributeType attributeType, ICodeElement codeElement)
		{
			string attributeString = null;
			MemberElement memberElement;
			TypeElement typeElement;

			switch (attributeType)
			{
			    case ElementAttributeType.Name:
			        attributeString = codeElement.Name;
			        break;

			    case ElementAttributeType.Access:
			        AttributedElement attributedElement = codeElement as AttributedElement;
			        if (attributedElement != null)
			        {
			            if (attributedElement.Access == CodeAccess.None)
			            {
			                attributeString = EnumToString(CodeAccess.Internal);
			            }
			            else
			            {
			                attributeString = EnumToString(attributedElement.Access);
			            }
			        }
			        break;

			    case ElementAttributeType.ElementType:
			        attributeString = EnumToString(codeElement.ElementType);
			        break;

			    case ElementAttributeType.Type:
			        attributeString = GetTypeAttribute(codeElement);
			        break;

			    case ElementAttributeType.Modifier:
			        memberElement = codeElement as MemberElement;
			        if (memberElement != null)
			        {
			            attributeString = EnumToString(memberElement.MemberModifiers);
			        }
			        else
			        {
			            typeElement = codeElement as TypeElement;
			            if (typeElement != null)
			            {
			                attributeString = EnumToString(typeElement.TypeModifiers);
			            }
			        }
			        break;

			    default:
			        attributeString = string.Empty;
			        break;
			}

			if (attributeString == null)
			{
			    attributeString = string.Empty;
			}

			return attributeString;
		}

		#endregion Public Methods
	}
}