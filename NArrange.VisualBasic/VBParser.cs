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
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;

using NArrange.Core;
using NArrange.Core.CodeElements;
using NArrange.Core.Configuration;

namespace NArrange.VisualBasic
{
	/// <summary>
	/// NArrange Visual Basic parser implementation.
	/// </summary>
	public sealed class VBParser : CodeParser
	{
		#region Constants

		private const char LineContinuation = '_';

		#endregion Constants

		#region Private Methods

		/// <summary>
		/// Captures an type name alias from the stream.
		/// </summary>
		/// <returns></returns>
		private string CaptureTypeName()
		{
			return CaptureWord(true);
		}

		/// <summary>
		/// Captures an alias or keyword from the stream.
		/// </summary>
		/// <returns></returns>
		private string CaptureWord(bool captureGeneric)
		{
			EatLineContinuation();

			EatWhitespace(Whitespace.SpaceAndTab);

			StringBuilder word = new StringBuilder();

			char nextChar = NextChar;
			while (nextChar != EmptyChar)
			{
				if (captureGeneric && nextChar == VBSymbol.BeginParamList)
				{
					TryReadChar();
					word.Append(CurrentChar);
					EatWhitespace();

					if (char.ToLower(NextChar) == char.ToLower(VBKeyword.Of[0]))
					{
						TryReadChar();
						word.Append(CurrentChar);

						if (char.ToLower(NextChar) == char.ToLower(VBKeyword.Of[1]))
						{
							TryReadChar();
							word.Append(CurrentChar);
							word.Append(' ');

							word.Append(ParseNestedText(VBSymbol.BeginParamList, VBSymbol.EndParamList,
								false, true));
							word.Append(VBSymbol.EndParamList);

							nextChar = NextChar;
						}
					}
					else if (NextChar == VBSymbol.EndParamList)
					{
						TryReadChar();
						word.Append(CurrentChar);
						nextChar = NextChar;
					}
				}
				else if (IsWhitespace(nextChar) || IsAliasBreak(nextChar))
				{
					break;
				}
				else
				{
					TryReadChar();
					word.Append(CurrentChar);
					nextChar = NextChar;
				}
			}

			return word.ToString();
		}

		/// <summary>
		/// Captures an alias or keyword from the stream.
		/// </summary>
		/// <returns></returns>
		private string CaptureWord()
		{
			return CaptureWord(false);
		}

		private ConstructorElement CreateConstructor(MethodElement methodElement)
		{
			ConstructorElement constructor = new ConstructorElement();
			constructor.Name = methodElement.Name;
			constructor.Access = methodElement.Access;
			constructor.MemberModifiers = methodElement.MemberModifiers;
			constructor.Params = methodElement.Params;
			constructor.BodyText = methodElement.BodyText;

			return constructor;
		}

		private void EatLineContinuation()
		{
			EatWhitespace(Whitespace.SpaceAndTab);
			while (IsWhitespace(CurrentChar) && NextChar == LineContinuation)
			{
			    TryReadChar();
			    EatWhitespace();
			}
		}

		private void EatWord(string word)
		{
			this.EatWord(word, "Expected " + word);
		}

		private void EatWord(string word, string message)
		{
			EatLineContinuation();

			EatWhitespace(Whitespace.SpaceAndTab);

			foreach (char ch in word.ToCharArray())
			{
				TryReadChar();
				if (char.ToLower(CurrentChar) != char.ToLower(ch))
				{
					this.OnParseError(message);
				}
			}
		}

		private static CodeAccess GetAccess(StringCollection wordList)
		{
			CodeAccess access = CodeAccess.NotSpecified;

			if (wordList.Contains(VBKeyword.Public))
			{
				access = CodeAccess.Public;
			}
			else if (wordList.Contains(VBKeyword.Private))
			{
				access = CodeAccess.Private;
			}
			else
			{
				if (wordList.Contains(VBKeyword.Protected))
				{
					access |= CodeAccess.Protected;
				}

				if (wordList.Contains(VBKeyword.Friend))
				{
					access |= CodeAccess.Internal;
				}
			}

			return access;
		}

		private static void GetElementType(StringCollection wordList,
			out ElementType elementType, out TypeElementType? typeElementType)
		{
			elementType = ElementType.NotSpecified;
			typeElementType = null;

			if (wordList.Contains(VBKeyword.Class))
			{
				elementType = ElementType.Type;
				typeElementType = TypeElementType.Class;
				return;
			}

			if (wordList.Contains(VBKeyword.Structure))
			{
				elementType = ElementType.Type;
				typeElementType = TypeElementType.Structure;
				return;
			}

			if (wordList.Contains(VBKeyword.Enumeration))
			{
				elementType = ElementType.Type;
				typeElementType = TypeElementType.Enum;
				return;
			}

			if (wordList.Contains(VBKeyword.Interface))
			{
				elementType = ElementType.Type;
				typeElementType = TypeElementType.Interface;
				return;
			}

			if (wordList.Contains(VBKeyword.Property))
			{
				elementType = ElementType.Property;
				return;
			}

			if (wordList.Contains(VBKeyword.Sub) || wordList.Contains(VBKeyword.Function) ||
				wordList.Contains(VBKeyword.Operator))
			{
				elementType = ElementType.Method;
				return;
			}

			if (wordList.Contains(VBKeyword.Event))
			{
				elementType = ElementType.Event;
				return;
			}

			if (wordList.Contains(VBKeyword.Delegate))
			{
				elementType = ElementType.Delegate;
				return;
			}
		}

		private static MemberModifier GetMemberAttributes(StringCollection wordList)
		{
			MemberModifier memberAttributes;
			memberAttributes = MemberModifier.None;

			bool isSealed = wordList.Contains(VBKeyword.NotOverridable) ||
				wordList.Contains(VBKeyword.NotInheritable);
			if (isSealed)
			{
				memberAttributes |= MemberModifier.Sealed;
			}

			bool isAbstract = wordList.Contains(VBKeyword.MustOverride) ||
				wordList.Contains(VBKeyword.MustInherit);
			if (isAbstract)
			{
				memberAttributes |= MemberModifier.Abstract;
			}

			bool isStatic = wordList.Contains(VBKeyword.Shared);
			if (isStatic)
			{
				memberAttributes |= MemberModifier.Static;
			}

			bool isVirtual = wordList.Contains(VBKeyword.Overridable);
			if (isVirtual)
			{
				memberAttributes |= MemberModifier.Virtual;
			}

			bool isOverride = wordList.Contains(VBKeyword.Overrides);
			if (isOverride)
			{
				memberAttributes |= MemberModifier.Override;
			}

			bool isNew = wordList.Contains(VBKeyword.Shadows);
			if (isNew)
			{
				memberAttributes |= MemberModifier.New;
			}

			bool isConstant = wordList.Contains(VBKeyword.Constant);
			if (isConstant)
			{
				memberAttributes |= MemberModifier.Constant;
			}

			bool isReadOnly = wordList.Contains(VBKeyword.ReadOnly);
			if (isReadOnly)
			{
				memberAttributes |= MemberModifier.ReadOnly;
			}

			return memberAttributes;
		}

		private OperatorType GetOperatorType(StringCollection wordList)
		{
			OperatorType operatorType = OperatorType.NotSpecified;

			if (wordList.Contains(VBKeyword.Widening))
			{
				operatorType = OperatorType.Implicit;
			}
			else if (wordList.Contains(VBKeyword.Narrowing))
			{
				operatorType = OperatorType.Explicit;
			}

			return operatorType;
		}

		/// <summary>
		/// Determines whether or not the specified char is a VB special character
		/// that signals a break in an alias
		/// </summary>
		/// <param name="ch"></param>
		/// <returns></returns>
		private bool IsAliasBreak(char ch)
		{
			return ch == VBSymbol.BeginParamList ||
					ch == VBSymbol.EndParamList ||
					ch == VBSymbol.BeginTypeConstraintList ||
					ch == VBSymbol.EndTypeConstraintList ||
					ch == Environment.NewLine[0] ||
					ch == VBSymbol.AliasSeparator;
		}

		/// <summary>
		/// Parses an attribute
		/// </summary>
		/// <param name="comments"></param>
		/// <returns></returns>
		private AttributeElement ParseAttribute(ReadOnlyCollection<ICommentElement> comments)
		{
			AttributeElement attributeElement;
			string attributeText = ParseNestedText(VBSymbol.BeginAttribute, VBSymbol.EndAttribute,
				false, true);
			attributeElement = new AttributeElement();
			attributeElement.BodyText = attributeText;

			if (comments.Count > 0)
			{
				foreach (ICommentElement comment in comments)
				{
					attributeElement.AddHeaderComment(comment);
				}
			}

			return attributeElement;
		}

		private string ParseBlock(string blockName)
		{
			EatWhitespace();

			StringBuilder blockText = new StringBuilder();

			bool blockRead = false;

			while (!blockRead && NextChar != EmptyChar)
			{
				string line = ReadLine();
				string trimmedLine = line.Trim();

				if (trimmedLine.Length >= VBKeyword.End.Length &&
					trimmedLine.Substring(0, VBKeyword.End.Length).ToLower() == VBKeyword.End.ToLower())
				{
					if (trimmedLine.Length > VBKeyword.End.Length &&
						IsWhitespace(trimmedLine[VBKeyword.End.Length]))
					{
						string restOfLine = trimmedLine.Substring(VBKeyword.End.Length).Trim();

						if (restOfLine.ToLower() == blockName.ToLower())
						{
							blockRead = true;
						}
						else if (restOfLine.Length == 1 && restOfLine[0] == LineContinuation)
						{
							string continuationLine = ReadLine();
							if (continuationLine.Trim().ToLower() == blockName.ToLower())
							{
								blockRead = true;
							}
							else
							{
								blockText.AppendLine(line);
								blockText.AppendLine(continuationLine);
							}
						}
						else
						{
							blockText.AppendLine(line);
						}
					}
					else
					{
						this.OnParseError("Expected element block close");
					}
				}
				else
				{
					blockText.AppendLine(line);
				}
			}

			if (!blockRead)
			{
				this.OnParseError("Unexpected end of file. Expected End " + blockName.ToString());
			}

			return blockText.ToString();
		}

		/// <summary>
		/// Parses a comment line
		/// </summary>
		/// <returns></returns>
		private CommentElement ParseCommentLine()
		{
			CommentElement commentLine;

			StringBuilder commentTextBuilder = new StringBuilder();

			CommentType commentType = CommentType.Line;
			if (NextChar == VBSymbol.BeginComment)
			{
				TryReadChar();
				if (NextChar == VBSymbol.BeginComment)
				{
					commentType = CommentType.XmlLine;
					TryReadChar();
				}
				else
				{
					commentTextBuilder.Append(VBSymbol.BeginComment);
				}
			}

			commentTextBuilder.Append(ReadLine());
			commentLine = new CommentElement(commentTextBuilder.ToString(), commentType);
			return commentLine;
		}

		private DelegateElement ParseDelegate( 
			CodeAccess access, MemberModifier memberAttributes)
		{
			string delegateType = CaptureWord();

			bool isFunction = false;
			switch (VBKeyword.Normalize(delegateType))
			{
				case VBKeyword.Sub:
					isFunction = false;
					break;

				case VBKeyword.Function:
					isFunction = true;
					break;

				default:
					this.OnParseError(
						"Expected Sub or Function for delegate declaration");
					break;
			}

			MethodElement methodElement = ParseMethod(access, memberAttributes,
				isFunction, true, false, OperatorType.NotSpecified);

			DelegateElement delegateElement = new DelegateElement();
			delegateElement.Name = methodElement.Name;
			delegateElement.Access = methodElement.Access;
			delegateElement.MemberModifiers = methodElement.MemberModifiers;
			delegateElement.Params = methodElement.Params;
			delegateElement.BodyText = methodElement.BodyText;
			if (isFunction)
			{
				delegateElement.Type = methodElement.Type;
			}

			foreach (TypeParameter typeParameter in methodElement.TypeParameters)
			{
				delegateElement.AddTypeParameter(typeParameter);
			}

			return delegateElement;
		}

		/// <summary>
		/// Parses elements from the current point in the stream
		/// </summary>
		/// <param name="parentElement">Parent element</param>
		/// <returns></returns>
		private List<ICodeElement> ParseElements(ICodeElement parentElement)
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();
			List<ICommentElement> comments = new List<ICommentElement>();
			List<AttributeElement> attributes = new List<AttributeElement>();
			Stack<RegionElement> regionStack = new Stack<RegionElement>();

			StringBuilder elementBuilder = new StringBuilder();

			char nextChar;
			bool end = false;

			while (TryReadChar() && !end)
			{
				switch (CurrentChar)
				{
					//
					// Comments
					//
					case VBSymbol.BeginComment:
						CommentElement commentLine = ParseCommentLine();
						comments.Add(commentLine);
						break;

					//
					// Preprocessor
					//
					case VBSymbol.Preprocessor:
						//
						// TODO: Besides regions, parse preprocessor elements so that
						// member preprocessor information is preserved.
						//
						string line = ReadLine().Trim();
						string[] words = line.Split(WhitespaceChars, StringSplitOptions.RemoveEmptyEntries);
						if (words.Length > 0 &&  VBKeyword.Normalize(words[0]) == VBKeyword.Region)
						{
							RegionElement regionElement = ParseRegion(line);
							regionStack.Push(regionElement);
						}
						else if (words.Length > 1 &&
							VBKeyword.Normalize(words[0]) == VBKeyword.End && 
							VBKeyword.Normalize(words[1]) == VBKeyword.Region)
						{
							RegionElement regionElement = regionStack.Pop();

							if (regionStack.Count > 0)
							{
								regionStack.Peek().AddChild(regionElement);
							}
							else
							{
								codeElements.Add(regionElement);
							}
						}
						else
						{
							this.OnParseError(
								"Cannot arrange files with preprocessor directives " +
								"other than #Region and #End Region");
						}
						break;

					//
					// Attribute
					//
					case VBSymbol.BeginAttribute:
						nextChar = NextChar;

						//
						// Parse attribute
						//
						AttributeElement attributeElement = ParseAttribute(comments.AsReadOnly());

						attributes.Add(attributeElement);
						codeElements.Add(attributeElement);
						comments.Clear();
						break;

					case LineContinuation:
						if (!(IsWhitespace(PreviousChar) && IsWhitespace(NextChar)))
						{
							elementBuilder.Append(CurrentChar);
						}
						break;

					// Eat any unneeded whitespace
					case ' ':
					case '\n':
					case '\r':
					case '\t':
						if (elementBuilder.Length > 0 &&
							elementBuilder[elementBuilder.Length - 1] != ' ')
						{
							elementBuilder.Append(' ');
						}
						break;

					default:
						elementBuilder.Append(CurrentChar);

						if (elementBuilder.ToString().ToLower() == VBKeyword.End.ToLower())
						{
							end = true;
						}
						else
						{
							nextChar = NextChar;

							if (char.IsWhiteSpace(nextChar) || VBSymbol.IsVBSymbol(CurrentChar))
							{
								string elementText = VBKeyword.Normalize(elementBuilder.ToString());
								bool isImplements = elementText.StartsWith(VBKeyword.Implements);
								bool isInherits = !isImplements && elementText.StartsWith(VBKeyword.Inherits);
								TypeElement typeElement = parentElement as TypeElement;
								if ((isImplements || isInherits) && typeElement != null)
								{
									string typeName = CaptureTypeName();
									typeElement.AddInterface(typeName);
									elementBuilder = new StringBuilder();
								}
								else
								{
									//
									// Try to parse a code element
									//
									ICodeElement element = TryParseElement(
										elementBuilder, comments.AsReadOnly(), attributes.AsReadOnly());
									if (element != null)
									{
										if (regionStack.Count > 0)
										{
											regionStack.Peek().AddChild(element);
										}
										else
										{
											codeElements.Add(element);
										}
										elementBuilder = new StringBuilder();
										comments.Clear();
										if (element is IAttributedElement)
										{
											foreach (AttributeElement attribute in attributes)
											{
												codeElements.Remove(attribute);
											}

											attributes = new List<AttributeElement>();
										}
									}
								}
							}
						}

						break;
				}

				int data = Reader.Peek();
				char nextCh = (char)data;
			}

			if (comments.Count > 0)
			{
				foreach (ICommentElement comment in comments)
				{
					codeElements.Add(comment);
				}
			}

			//
			// Make sure that all region elements have been closed
			//
			if (regionStack.Count > 0)
			{
				this.OnParseError("Expected #End Region");
			}

			return codeElements;
		}

		private EventElement ParseEvent(CodeAccess access, MemberModifier memberAttributes,
			bool isCustom)
		{
			EventElement eventElement = new EventElement();
			string name = CaptureWord();
			eventElement.Name = name;

			EatWhitespace();
			if (NextChar == VBSymbol.BeginParamList)
			{
			    eventElement.Params = ParseNestedText(
			        VBSymbol.BeginParamList, VBSymbol.EndParamList, true, false);
			}
			else
			{
			    EatWord(VBKeyword.As);

			    string eventType = CaptureTypeName();
			    if (string.IsNullOrEmpty(eventType))
			    {
			        this.OnParseError("Expected type identifier");
			    }
			    eventElement.Type = eventType;
			}

			string blockTemp;
			string[] implements = TryParseImplements(out blockTemp);
			foreach (string implementation in implements)
			{
			    eventElement.AddImplementation(implementation);
			}

			if (isCustom)
			{
			    eventElement.BodyText = blockTemp + this.ParseBlock(VBKeyword.Event);
			}

			return eventElement;
		}

		private FieldElement ParseField(StringCollection wordList, 
			CodeAccess access, MemberModifier memberAttributes)
		{
			FieldElement field = new FieldElement();

			StringBuilder nameBuilder = new StringBuilder();

			foreach (string word in wordList)
			{
				string trimmedWord = word.Trim(' ', VBSymbol.AliasSeparator);

				if (!VBKeyword.IsVBKeyword(trimmedWord) && 
					trimmedWord.Length > 0)
				{
					nameBuilder.Append(trimmedWord);
					nameBuilder.Append(VBSymbol.AliasSeparator);
					nameBuilder.Append(' ');
				}
			}

			field.Name = nameBuilder.ToString().TrimEnd(VBSymbol.AliasSeparator, ' ');

			EatWhitespace();
			if (NextChar == VBSymbol.AliasSeparator)
			{

			}

			field.Type = CaptureTypeName();
			field.Access = access;
			field.MemberModifiers = memberAttributes;

			EatWhitespace();

			bool isAssignment = NextChar == VBSymbol.Assignment;
			if (isAssignment)
			{
				EatChar(VBSymbol.Assignment);

				string initialValue = ParseInitialValue();
				field.InitialValue = initialValue;
			}

			return field;
		}

		private string ParseInitialValue()
		{
			EatWhitespace(Whitespace.Space);

			string initialValue = ReadLine().Trim();

			if (string.IsNullOrEmpty(initialValue))
			{
				this.OnParseError("Expected an initial value");
			}

			return initialValue;
		}

		private MethodElement ParseMethod(
			CodeAccess access, MemberModifier memberAttributes, bool isFunction, bool isDelegate,
			bool isOperator, OperatorType operatorType)
		{
			MethodElement method = new MethodElement();
			method.Name = CaptureWord();
			method.Access = access;
			method.MemberModifiers = memberAttributes;

			method.IsOperator = isOperator;
			method.OperatorType = operatorType;

			EatChar(VBSymbol.BeginParamList);
			EatWhitespace();
			string paramsTemp = string.Empty;

			if (char.ToLower(NextChar) == char.ToLower(VBKeyword.Of[0]))
			{
				TryReadChar();
				paramsTemp += CurrentChar;

				if (char.ToLower(NextChar) == char.ToLower(VBKeyword.Of[1]))
				{
					TryReadChar();
					paramsTemp = string.Empty;

					this.ParseTypeParameters(method);

					EatChar(VBSymbol.BeginParamList);
					EatWhitespace();
				}
			}

			method.Params = paramsTemp + ParseNestedText(
				VBSymbol.BeginParamList, VBSymbol.EndParamList, false, false);

			if (isFunction || isOperator)
			{
			    EatWhitespace();
			    EatWord(VBKeyword.As);
			    method.Type = CaptureTypeName();
			}

			EatWhitespace();

			string blockTemp;
			string[] implements = TryParseImplements(out blockTemp);
			foreach (string implementation in implements)
			{
				method.AddImplementation(implementation);
			}

			if (isFunction || isOperator)
			{
				if (isOperator)
				{
					method.BodyText = blockTemp + this.ParseBlock(VBKeyword.Operator);
				}
				else if (!isDelegate)
				{
					method.BodyText = blockTemp + this.ParseBlock(VBKeyword.Function);
				}
			}
			else if (!isDelegate)
			{
				method.BodyText = blockTemp + this.ParseBlock(VBKeyword.Sub);
			}

			return method;
		}

		/// <summary>
		/// Parses a namespace definition
		/// </summary>
		/// <returns></returns>
		private NamespaceElement ParseNamespace()
		{
			NamespaceElement namespaceElement = new NamespaceElement();
			string namepaceName = CaptureWord();
			namespaceElement.Name = namepaceName;

			//
			// Parse child elements
			//
			List<ICodeElement> childElements = DoParseElements();
			foreach (ICodeElement childElement in childElements)
			{
				namespaceElement.AddChild(childElement);
			}

			EatWhitespace();
			EatWord(VBKeyword.Namespace, "Expected End Namespace");

			return namespaceElement;
		}

		private string ParseNestedText(char beginChar, char endChar, bool beginExpected, bool trim)
		{
			if (beginChar != EmptyChar && beginExpected)
			{
				EatWhitespace();
				EatChar(beginChar);
			}

			StringBuilder blockText = new StringBuilder();

			int depth = 1;

			char nextChar = NextChar;
			if (nextChar == EmptyChar)
			{
				this.OnParseError("Unexpected end of file. Expected " + endChar);
			}
			else if (nextChar == endChar)
			{
				TryReadChar();
			}
			else
			{
				bool inString = false;
				bool inLineComment = false;

				while (depth > 0)
				{
					bool charRead = TryReadChar();
					if (!charRead)
					{
						this.OnParseError("Unexpected end of file. Expected " + endChar);
					}

					nextChar = NextChar;

					bool inComment = inLineComment;

					if (!inComment)
					{
						if (CurrentChar == VBSymbol.BeginString)
						{
							inString = !inString;
						}
					}

					if (!inString)
					{
						if (CurrentChar == VBSymbol.BeginComment)
						{
							inLineComment = true;
						}
						else if (inLineComment && CurrentChar == '\r' && nextChar == '\n')
						{
							inLineComment = false;
						}
					}

					inComment = inLineComment;
					if (beginChar != EmptyChar && CurrentChar == beginChar &&
						!inString && !inComment)
					{
						blockText.Append(CurrentChar);
						depth++;
					}
					else
					{
						blockText.Append(CurrentChar);
					}

					if (nextChar == endChar && !inString && !inComment)
					{
						if (depth == 1)
						{
							EatChar(endChar);
							break;
						}
						else
						{
							depth--;
						}
					}
				}
			}

			if (trim)
			{
				return blockText.ToString().Trim();
			}
			else
			{
				return blockText.ToString();
			}
		}

		private string ParseParams()
		{
			EatLineContinuation();

			return ParseNestedText(VBSymbol.BeginParamList, VBSymbol.EndParamList, true, false);
		}

		private PropertyElement ParseProperty(CodeAccess access, MemberModifier memberAttributes,
			bool isDefault, string modifyAccess)
		{
			PropertyElement property = new PropertyElement();
			property.Name = CaptureWord();
			property.Access = access;
			property.MemberModifiers = memberAttributes;
			property[VBExtendedProperties.Default] = isDefault;
			property[VBExtendedProperties.ModifyAccess] = modifyAccess;

			string indexParam = this.ParseParams();
			if (indexParam.Length > 0)
			{
			    property.IndexParameter = indexParam;
			}

			EatWord(VBKeyword.As, "Expected As");

			string type = CaptureTypeName();
			if (string.IsNullOrEmpty(type))
			{
				this.OnParseError("Expected return type");
			}

			property.Type = type;

			string blockTemp;
			string[] implements = TryParseImplements(out blockTemp);
			foreach (string implementation in implements)
			{
			    property.AddImplementation(implementation);
			}

			property.BodyText = blockTemp + this.ParseBlock(VBKeyword.Property);

			return property;
		}

		/// <summary>
		/// Parses a region from the preprocessor line
		/// </summary>
		/// <param name="line"></param>
		/// <returns></returns>
		private RegionElement ParseRegion(string line)
		{
			RegionElement regionElement;
			string regionName = line.Substring(VBKeyword.Region.Length).Trim(' ','"');

			if (string.IsNullOrEmpty(regionName))
			{
				this.OnParseError("Expected region name");
			}

			regionElement = new RegionElement();
			regionElement.Name = regionName;
			return regionElement;
		}

		/// <summary>
		/// Parses a type definition
		/// </summary>
		/// <returns></returns>
		private TypeElement ParseType(
			CodeAccess access, TypeModifier typeAttributes,
			TypeElementType elementType)
		{
			TypeElement typeElement = new TypeElement();

			EatWhitespace();
			string className = CaptureWord();
			typeElement.Name = className;

			if (access == CodeAccess.NotSpecified &&
				((typeAttributes & TypeModifier.Partial) != TypeModifier.Partial))
			{
				access = CodeAccess.Internal;
			}

			typeElement.Access = access;
			typeElement.Type = elementType;
			typeElement.TypeModifiers = typeAttributes;

			EatWhitespace();

			if (elementType == TypeElementType.Enum)
			{
				string enumText = ParseBlock(VBKeyword.Enumeration);

				// TODO: Parse enum values as fields
				typeElement.BodyText = enumText;
			}
			else
			{
				bool isGeneric = TryReadChar(VBSymbol.BeginParamList);
				if (isGeneric)
				{
					EatWord(VBKeyword.Of, "Expected Of");

					this.ParseTypeParameters(typeElement);
				}

				EatWhitespace();

				//
				// Parse child elements
				//
				List<ICodeElement> childElements = ParseElements(typeElement);
				foreach (ICodeElement childElement in childElements)
				{
					typeElement.AddChild(childElement);
				}

				EatWhitespace();
				EatWord(elementType.ToString(), "Expected End " + elementType.ToString());
			}

			return typeElement;
		}

		private string ParseTypeParameterConstraint()
		{
			string typeParameterConstraint;
			typeParameterConstraint = CaptureTypeName();

			if (VBKeyword.Normalize(typeParameterConstraint) == VBKeyword.As)
			{
				this.OnParseError("Invalid identifier");
			}

			if (NextChar == VBSymbol.AliasSeparator)
			{
				TryReadChar();
			}

			EatWhitespace();
			return typeParameterConstraint;
		}

		private void ParseTypeParameters(IGenericElement genericElement)
		{
			EatWhitespace();

			if (NextChar == VBSymbol.EndParamList ||
				NextChar == EmptyChar)
			{
				this.OnParseError("Expected type parameter");
			}

			while (NextChar != VBSymbol.EndParamList &&
				NextChar != EmptyChar)
			{
				if (genericElement.TypeParameters.Count > 0 && NextChar == VBSymbol.AliasSeparator)
				{
					TryReadChar();
				}

				string typeParameterName = CaptureWord();

				EatWhitespace();

				if (NextChar == EmptyChar)
				{
					break;
				}

				TypeParameter typeParameter = new TypeParameter();
				typeParameter.Name = typeParameterName;

				if (NextChar != VBSymbol.AliasSeparator &&
					NextChar != VBSymbol.EndParamList)
				{
					if (char.ToLower(NextChar) == char.ToLower(VBKeyword.As[0]))
					{
						TryReadChar();

						if (char.ToLower(NextChar) == char.ToLower(VBKeyword.As[1]))
						{
							TryReadChar();

							EatWhitespace();

							if (NextChar == VBSymbol.EndParamList)
							{
								this.OnParseError("Expected type parameter constraint");
							}

							if (NextChar == VBSymbol.BeginTypeConstraintList)
							{
								TryReadChar();

								while (NextChar != VBSymbol.EndTypeConstraintList &&
									NextChar != EmptyChar)
								{
									string typeParameterConstraint;
									typeParameterConstraint = ParseTypeParameterConstraint();
									typeParameter.AddConstraint(typeParameterConstraint);
								}

								EatChar(VBSymbol.EndTypeConstraintList);
							}
							else
							{
								while (NextChar != VBSymbol.EndParamList &&
									NextChar != EmptyChar)
								{
									string typeParameterConstraint;
									typeParameterConstraint = ParseTypeParameterConstraint();
									typeParameter.AddConstraint(typeParameterConstraint);
								}
							}
						}
					}
				}

				genericElement.AddTypeParameter(typeParameter);
			}

			EatChar(VBSymbol.EndParamList);
		}

		private UsingElement ParseUsing()
		{
			UsingElement usingElement = new UsingElement();
			string alias = CaptureWord();
			if (string.IsNullOrEmpty(alias))
			{
				this.OnParseError("Expected a namepace name");
			}

			EatWhitespace(Whitespace.SpaceAndTab);

			bool endOfStatement = TryReadChar(Environment.NewLine[0]);
			if (endOfStatement || NextChar == EmptyChar)
			{
				usingElement.Name = alias;
			}
			else
			{
				bool assign = TryReadChar(VBSymbol.Assignment);
				if (!assign)
				{
					this.OnParseError(
						string.Format("Expected {0} or end of statement.",
						VBSymbol.Assignment));
				}
				else
				{
					string name = CaptureWord();
					if (string.IsNullOrEmpty(name))
					{
						this.OnParseError("Expected a type or namepace name");
					}
					else
					{
						usingElement.Name = name;
						usingElement.Redefine = alias;
						TryReadChar(Environment.NewLine[0]);
					}
				}
			}

			return usingElement;
		}

		[Obsolete]
		private string ReadCodeLine()
		{
			string line = ReadLine().Trim();

			if (line != null && line.Length > 1 && !line.StartsWith(VBSymbol.BeginComment.ToString()) &&
				line[line.Length - 1] == LineContinuation && IsWhitespace(line[line.Length - 2]))
			{
				line = line.TrimEnd(LineContinuation).TrimEnd(WhitespaceChars) + " " + '_' + " " + 
					ReadCodeLine();
			}

			return line;
		}

		/// <summary>
		/// Tries to parse a code element
		/// </summary>
		/// <param name="elementBuilder"></param>
		/// <param name="comments"></param>
		/// <param name="attributes"></param>
		/// <returns></returns>
		private ICodeElement TryParseElement(StringBuilder elementBuilder,
			ReadOnlyCollection<ICommentElement> comments,
			ReadOnlyCollection<AttributeElement> attributes)
		{
			CodeElement codeElement = null;

			string processedElementText =
				elementBuilder.ToString().Trim();

			switch (VBKeyword.Normalize(processedElementText))
			{
				case VBKeyword.Namespace:
					codeElement = ParseNamespace();
					break;

				case VBKeyword.Imports:
					codeElement = ParseUsing();
					break;
			}

			if (codeElement == null)
			{
				string[] words = processedElementText.TrimEnd(
					VBSymbol.Assignment,
					VBSymbol.BeginParamList).Split(
					WhitespaceChars,
					StringSplitOptions.RemoveEmptyEntries);

				if (words.Length > 0)
				{
					string normalizedKeyWord = VBKeyword.Normalize(words[0]);

					if (words.Length > 1 ||
						normalizedKeyWord == VBKeyword.Class ||
						normalizedKeyWord == VBKeyword.Structure ||
						normalizedKeyWord == VBKeyword.Interface ||
						normalizedKeyWord == VBKeyword.Enumeration)
					{
						StringCollection wordList = new StringCollection();
						wordList.AddRange(words);

						StringCollection normalizedWordList = new StringCollection();
						foreach (string word in wordList)
						{
							normalizedWordList.Add(VBKeyword.Normalize(word));
						}

						string name = string.Empty;
						ElementType elementType;
						CodeAccess access = CodeAccess.NotSpecified;
						MemberModifier memberAttributes = MemberModifier.None;
						TypeElementType? typeElementType = null;

						bool isField = normalizedWordList[normalizedWordList.Count - 1] == VBKeyword.As;
						if (isField)
						{
							elementType = ElementType.Field;
						}
						else
						{
							GetElementType(normalizedWordList, out elementType, out typeElementType);
						}

						if (elementType == ElementType.Method ||
							elementType == ElementType.Property ||
							elementType == ElementType.Event ||
							elementType == ElementType.Delegate ||
							elementType == ElementType.Type ||
							elementType == ElementType.Field)
						{
							access = GetAccess(normalizedWordList);
							memberAttributes = GetMemberAttributes(normalizedWordList);
						}

						switch (elementType)
						{
							case ElementType.Type:
								TypeModifier typeAttributes = (TypeModifier)memberAttributes;
								if (normalizedWordList.Contains(VBKeyword.Partial))
								{
									typeAttributes |= TypeModifier.Partial;
								}

								codeElement = ParseType(access, typeAttributes, typeElementType.Value);
								break;

							case ElementType.Event:
								codeElement = ParseEvent(access, memberAttributes,
									normalizedWordList.Contains(VBKeyword.Custom));
								break;

							case ElementType.Field:
								codeElement = ParseField(wordList, access, memberAttributes);
								break;

							case ElementType.Property:
			                    string modifyAccess = null;
			                    if (normalizedWordList.Contains(VBKeyword.ReadOnly))
			                    {
			                        modifyAccess = VBKeyword.ReadOnly;
			                    }
			                    else if (normalizedWordList.Contains(VBKeyword.ReadWrite))
			                    {
			                        modifyAccess = VBKeyword.ReadWrite;
			                    }
			                    else if (normalizedWordList.Contains(VBKeyword.WriteOnly))
			                    {
			                        modifyAccess = VBKeyword.WriteOnly;
			                    }

			                    bool isDefault = normalizedWordList.Contains(VBKeyword.Default);
								codeElement = ParseProperty(access, memberAttributes, isDefault, modifyAccess);
								break;

							case ElementType.Delegate:
								codeElement = ParseDelegate(access, memberAttributes);
								break;

							case ElementType.Method:
								bool isOperator = normalizedWordList.Contains(VBKeyword.Operator);
								OperatorType operatorType = OperatorType.NotSpecified;
								if (isOperator)
								{
									operatorType = GetOperatorType(wordList);
								}

								//
								// Method
								//
								MethodElement methodElement = ParseMethod(
										access, memberAttributes,
										normalizedWordList.Contains(VBKeyword.Function), false,
										isOperator, operatorType);
								if (VBKeyword.Normalize(methodElement.Name) == VBKeyword.New)
								{
									codeElement = CreateConstructor(methodElement);
								}
								else
								{
									codeElement = methodElement;
								}
								break;
						}
					}
				}
			}

			if (codeElement != null)
			{
				ApplyCommentsAndAttributes(codeElement, comments, attributes);
			}

			return codeElement;
		}

		private string[] TryParseImplements(out string textRead)
		{
			EatWhitespace();

			List<string> implements = new List<string>();

			StringBuilder read = new StringBuilder();

			Action<StringBuilder> eatLineContinuation = delegate(StringBuilder builder)
			{
				while (true)
				{
					while (IsWhitespace(NextChar))
					{
						TryReadChar();
						builder.Append(CurrentChar);
					}

					if (NextChar != LineContinuation)
					{
						break;
					}
					TryReadChar();
					builder.Append(CurrentChar);

			        while (IsWhitespace(NextChar))
			        {
			            TryReadChar();
			            builder.Append(CurrentChar);
			        }

					if (!IsWhitespace(NextChar))
					{
						break;
					}
				}
			};

			eatLineContinuation(read);

			bool implementRead = false;
			foreach (char ch in VBKeyword.Implements.ToCharArray())
			{
				if (char.ToLower(NextChar) == char.ToLower(ch))
				{
					TryReadChar();
					read.Append(CurrentChar);
					implementRead = true;
				}
				else
				{
					implementRead = false;
					break;
				}
			}

			if (implementRead)
			{
				do
				{
					eatLineContinuation(read);

					if (implements.Count > 0 && NextChar == VBSymbol.AliasSeparator)
					{
						TryReadChar();
					}

					eatLineContinuation(read);

					string interfaceMember = CaptureTypeName();
					if (string.IsNullOrEmpty(interfaceMember))
					{
						this.OnParseError("Expected an interface member name.");
					}

					implements.Add(interfaceMember);
					read = new StringBuilder();
					eatLineContinuation(read);
				}
				while (NextChar == VBSymbol.AliasSeparator);
			}

			textRead = read.ToString();

			return implements.ToArray();
		}

		#endregion Private Methods

		#region Protected Methods

		/// <summary>
		/// Parses elements from the current point in the stream
		/// </summary>
		/// <returns></returns>
		protected override List<ICodeElement> DoParseElements()
		{
			return ParseElements(null);
		}

		#endregion Protected Methods
	}
}