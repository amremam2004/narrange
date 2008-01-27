//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~                                                                        
// Copyright (c) 2007-2008 James Nies and NArrange contributors. 	      
// 	    All rights reserved.                   				      
//                                                                             
// This program and the accompanying materials are made available under       
// the terms of the Common Public License v1.0 which accompanies this         
// distribution.							      
//                                                                             
// Redistribution and use in source and binary forms, with or                 
// without modification, are permitted provided that the following            
// conditions are met:                                                        
//                                                                             
// Redistributions of source code must retain the above copyright             
// notice, this list of conditions and the following disclaimer.              
// Redistributions in binary form must reproduce the above copyright          
// notice, this list of conditions and the following disclaimer in            
// the documentation and/or other materials provided with the distribution.   
//                                                                             
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS        
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT          
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS          
// FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT   
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,      
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED   
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,        
// OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY     
// OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING    
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS         
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.               
//                                                                             
// Contributors:
//      James Nies
//      - Initial creation
//      - Fixed parsing of events with generic return types
//      - Improved parsing performance by reducing the number of calls to 
//        TryParseElement
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
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

namespace NArrange.CSharp
{
	/// <summary>
	/// Standard NArrange CSharp parser implementation.
	/// </summary>
	public sealed class CSharpParser : ICodeParser
	{
		#region Constants

		private const char EmptyChar = '\0';

		#endregion Constants

		#region Read-Only Fields

		private static readonly char[] WhitespaceChars = { ' ', '\t', '\r', '\n' };

		#endregion Read-Only Fields

		#region Fields

		private char _ch = '\0';		
		private char[] _charBuffer = new char[1];		
		private char _lastCh = '\0';		
		private int _lineNumber = 1;		
		private int _position = 1;		
		private TextReader _reader;		
		
		#endregion Fields

		#region Public Methods

		/// <summary>
		/// Parses a collection of code elements from a stream reader.
		/// </summary>
		/// <param name="reader">Code stream reader</param>
		/// <returns></returns>
		public ReadOnlyCollection<ICodeElement> Parse(TextReader reader)
		{
			if (reader == null)
			{
			    throw new ArgumentNullException("reader");
			}
			
			List<ICodeElement> codeElements = new List<ICodeElement>();
			
			Reset();
			_reader = reader;
			
			codeElements = ParseElements();
			
			return codeElements.AsReadOnly();
		}

		#endregion Public Methods

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
		private string CaptureWord()
		{
			return CaptureWord(false);
		}

		/// <summary>
		/// Captures an alias or keyword from the stream.
		/// </summary>
		/// <returns></returns>
		private string CaptureWord(bool captureGeneric)
		{
			EatWhitespace();
			
			StringBuilder word = new StringBuilder();
			
			int data = _reader.Peek();
			while (data > 0)
			{
			    char ch = (char)data;
			
			    if (IsWhitespace(ch) ||
			        (IsAliasBreak(ch) && 
			        !(ch == CSharpSymbol.TypeImplements && (word.ToString() == CSharpKeyword.Global || word.ToString() == CSharpKeyword.Global + CSharpSymbol.TypeImplements.ToString())))||
			        (!captureGeneric &&
			        (ch == CSharpSymbol.BeginGeneric ||
			        ch == CSharpSymbol.EndGeneric)))
			    {
			        break;
			    }
			    else
			    {
			        TryReadChar();
			        word.Append(_ch);
			        data = _reader.Peek();
			    }
			}
			
			return word.ToString();
		}

		/// <summary>
		/// Creates a field with the specified information
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="type"></param>
		/// <param name="access"></param>
		/// <param name="memberAttributes"></param>
		/// <param name="isVolatile"></param>
		/// <returns></returns>
		private FieldElement CreateField(string fieldName, string type,
			CodeAccess access, MemberModifier memberAttributes, bool isVolatile)
		{
			FieldElement field = new FieldElement();
			field.Name = fieldName;
			field.Type = type;
			field.Access = access;
			field.MemberModifiers = memberAttributes;
			field.IsVolatile = isVolatile;
			
			return field;
		}

		/// <summary>
		/// Eats the specified character
		/// </summary>
		/// <param name="ch">Character to eat</param>
		private void EatChar(char ch)
		{
			EatWhitespace();
			TryReadChar();
			if (_ch != ch)
			{
			    this.OnParseError("Expected " + ch);
			}
		}

		/// <summary>
		/// Reads until the next non-whitespace character is reached.
		/// </summary>
		private void EatWhitespace()
		{
			EatWhitespace(false);
		}

		/// <summary>
		/// Reads until the next non-whitespace character is reached.
		/// </summary>
		private void EatWhitespace(bool spacesOnly)
		{
			int data = _reader.Peek();
			while (data > 0)
			{
			    char ch = (char)data;
			
			    if (spacesOnly && ch == ' ')
			    {
			        TryReadChar();
			    }
			    else if (!spacesOnly && IsWhitespace(ch))
			    {
			        TryReadChar();
			    }
			    else
			    {
			        return;
			    }
			
			    data = _reader.Peek();
			    if (data <= 0)
			    {
			        UnexpectedEndOfFile();
			    }
			}
		}

		private static CodeAccess GetAccess(string processedElementText)
		{
			CodeAccess access = CodeAccess.NotSpecified;
			
			if (processedElementText.Contains(CSharpKeyword.Public))
			{
			    access = CodeAccess.Public;
			}
			else if (processedElementText.Contains(CSharpKeyword.Private))
			{
			    access = CodeAccess.Private;
			}
			else
			{
			    if (processedElementText.Contains(CSharpKeyword.Protected))
			    {
			        access |= CodeAccess.Protected;
			    }
			
			    if (processedElementText.Contains(CSharpKeyword.Internal))
			    {
			        access |= CodeAccess.Internal;
			    }
			}
			
			return access;
		}

		private static MemberModifier GetMemberAttributes(StringCollection wordList)
		{
			MemberModifier memberAttributes;
			memberAttributes = MemberModifier.None;
			
			bool isSealed = wordList.Contains(CSharpKeyword.Sealed);
			if (isSealed)
			{
			    memberAttributes |= MemberModifier.Sealed;
			}
			
			bool isAbstract = wordList.Contains(CSharpKeyword.Abstract);
			if (isAbstract)
			{
			    memberAttributes |= MemberModifier.Abstract;
			}
			
			bool isStatic = wordList.Contains(CSharpKeyword.Static);
			if (isStatic)
			{
			    memberAttributes |= MemberModifier.Static;
			}
			
			bool isUnsafe = wordList.Contains(CSharpKeyword.Unsafe);
			if (isUnsafe)
			{
			    memberAttributes |= MemberModifier.Unsafe;
			}
			
			bool isVirtual = wordList.Contains(CSharpKeyword.Virtual);
			if (isVirtual)
			{
			    memberAttributes |= MemberModifier.Virtual;
			}
			
			bool isOverride = wordList.Contains(CSharpKeyword.Override);
			if (isOverride)
			{
			    memberAttributes |= MemberModifier.Override;
			}
			
			bool isNew = wordList.Contains(CSharpKeyword.New);
			if (isNew)
			{
			    memberAttributes |= MemberModifier.New;
			}
			
			bool isConstant = wordList.Contains(CSharpKeyword.Constant);
			if (isConstant)
			{
			    memberAttributes |= MemberModifier.Constant;
			}
			
			bool isReadOnly = wordList.Contains(CSharpKeyword.ReadOnly);
			if (isReadOnly)
			{
			    memberAttributes |= MemberModifier.ReadOnly;
			}
			
			bool isExternal = wordList.Contains(CSharpKeyword.External);
			if (isExternal)
			{
			    memberAttributes |= MemberModifier.External;
			}
			
			return memberAttributes;
		}

		/// <summary>
		/// Extracts a member name.
		/// </summary>
		/// <param name="words"></param>
		/// <param name="name"></param>
		/// <param name="returnType"></param>
		/// <returns></returns>
		private void GetMemberNameAndType(string[] words,
			out string name, out string returnType)
		{
			name = null;
			returnType = null;
			   
			List<string> wordList = new List<string>(words);
			for (int wordIndex = 0; wordIndex < wordList.Count; wordIndex++)
			{
			    string wordGroup = wordList[wordIndex];
			    int separatorIndex = wordGroup.IndexOf(CSharpSymbol.AliasSeparator);
			    if (separatorIndex >= 0)
			    {
			        if (separatorIndex < wordGroup.Length - 1)
			        {
			            //
			            // Format words with commas to have a space after comma
			            //
			            string[] aliases = wordGroup.Split(CSharpSymbol.AliasSeparator);
			            wordGroup = string.Empty;
			            for (int aliasIndex = 0; aliasIndex < aliases.Length; aliasIndex++)
			            {
			                string alias = aliases[aliasIndex];
			                wordGroup += alias.Trim();
			                if (aliasIndex < aliases.Length - 1)
			                {
			                    wordGroup += ", ";
			                }
			            }
			
			            wordGroup = wordGroup.TrimEnd();
			            wordList[wordIndex] = wordGroup;
			        }
			
			        //
			        // Concatenate comma separated values into logical groups
			        //
			        if (wordGroup[0] == CSharpSymbol.AliasSeparator && wordIndex > 0)
			        {
			            if (wordGroup.Length == 1 && wordIndex < wordList.Count - 1)
			            {
			                wordList[wordIndex - 1] = wordList[wordIndex - 1] +
			                    CSharpSymbol.AliasSeparator + " " +
			                    wordList[wordIndex + 1];
			                wordList.RemoveAt(wordIndex);
			                wordList.RemoveAt(wordIndex);
			                wordIndex--;
			                wordIndex--;
			            }
			            else
			            {
			                wordList[wordIndex - 1] = wordList[wordIndex - 1] + wordGroup;
			                wordList.RemoveAt(wordIndex);
			                wordIndex--;
			            }
			        }
			        else if (wordIndex < wordList.Count &&
			            wordGroup[wordGroup.Length - 1] == CSharpSymbol.AliasSeparator)
			        {
			            wordGroup = wordGroup + " " + wordList[wordIndex + 1];
			            wordList[wordIndex] = wordGroup;
			            wordList.RemoveAt(wordIndex + 1);
			            wordIndex--;
			        }
			    }
			}
			
			if (wordList.Count > 1)
			{
			    int nameIndex = wordList.Count - 1;
			    name = wordList[nameIndex];
			
			    int typeIndex = nameIndex - 1;
			    string typeCandidate = wordList[typeIndex];
			    if (typeCandidate == CSharpKeyword.Operator)
			    {
			        typeIndex--;
			        typeCandidate = wordList[typeIndex];
			    }
			
			    if (name.EndsWith(CSharpSymbol.EndAttribute.ToString()) && wordList.Count > 2)
			    {
			        //
			        // Property indexer
			        //
			        name = wordList[wordList.Count - 2] + " " + name;
			        typeCandidate = wordList[wordList.Count - 3];
			    }
			
			    if (typeCandidate != CSharpKeyword.Abstract &&
			        typeCandidate != CSharpKeyword.Constant &&
			        typeCandidate != CSharpKeyword.Internal &&
			        typeCandidate != CSharpKeyword.New &&
			        typeCandidate != CSharpKeyword.Override &&
			        typeCandidate != CSharpKeyword.Private &&
			        typeCandidate != CSharpKeyword.Protected &&
			        typeCandidate != CSharpKeyword.Public &&
			        typeCandidate != CSharpKeyword.ReadOnly &&
			        typeCandidate != CSharpKeyword.Sealed &&
			        typeCandidate != CSharpKeyword.Static &&
			        typeCandidate != CSharpKeyword.Virtual)
			    {
			        returnType = typeCandidate;
			    }
			}
			else
			{
			    name = wordList[0];
			}
		}

		private OperatorType GetOperatorType(StringCollection wordList)
		{
			OperatorType operatorType = OperatorType.NotSpecified;
			if (wordList.Contains(CSharpKeyword.Explicit))
			{
			    operatorType = OperatorType.Explicit;
			}
			else if (wordList.Contains(CSharpKeyword.Implicit))
			{
			    operatorType = OperatorType.Implicit;
			}
			
			return operatorType;
		}

		/// <summary>
		/// Gets a type element type
		/// </summary>
		/// <param name="isClass"></param>
		/// <param name="isStruct"></param>
		/// <param name="isInterface"></param>
		/// <param name="isEnum"></param>
		/// <returns></returns>
		private TypeElementType GetTypeElementType(bool isClass, bool isStruct, bool isInterface, bool isEnum)
		{
			TypeElementType type = TypeElementType.Class;
			
			bool singleDefinition = isClass ^ isStruct ^ isEnum ^ isInterface;
			Debug.Assert(singleDefinition, "Invalid type definition.");
			
			if (isClass)
			{
			    type = TypeElementType.Class;
			}
			else if (isStruct)
			{
			    type = TypeElementType.Structure;
			}
			else if (isInterface)
			{
			    type = TypeElementType.Interface;
			}
			else if (isEnum)
			{
			    type = TypeElementType.Enum;
			}
			
			return type;
		}

		/// <summary>
		/// Determines whether or not the specified char is a C# special character
		/// that signals a break in an alias
		/// </summary>
		/// <param name="ch"></param>
		/// <returns></returns>
		private bool IsAliasBreak(char ch)
		{
			return  ch == CSharpSymbol.BeginParamList ||
			        ch == CSharpSymbol.EndParamList ||
			        ch == CSharpSymbol.EndOfStatement ||
			        ch == CSharpSymbol.AliasSeparator ||
			        ch == CSharpSymbol.TypeImplements ||
			        ch == CSharpSymbol.BeginBlock ||
			        ch == CSharpSymbol.EndBlock;
		}

		/// <summary>
		/// Determines whether or not the specified character is whitespace.
		/// </summary>
		/// <param name="ch"></param>
		/// <returns></returns>
		private static bool IsWhitespace(char ch)
		{
			return ch == ' ' || ch == '\t' ||
			    ch == '\n' || ch == '\r';
		}

		/// <summary>
		/// Returns the next character in the file, if any
		/// </summary>
		/// <returns></returns>
		private char NextChar()
		{
			int data = _reader.Peek();
			if (data > 0)
			{
			    char ch = (char)data;
			    return ch;
			}
			else
			{
			    return EmptyChar;
			}
		}

		/// <summary>
		/// Throws a parse error 
		/// </summary>
		/// <param name="message"></param>
		private void OnParseError(string message)
		{
			throw new ParseException(message, _lineNumber, _position);
		}

		private string[] ParseAliasList()
		{
			List<string> aliases = new List<string>();
			
			EatWhitespace();
			
			char nextChar = NextChar();
			if (nextChar == CSharpSymbol.BeginBlock)
			{
			    this.OnParseError("Expected a class or interface name");
			}
			else
			{
			    while (nextChar != EmptyChar && nextChar != CSharpSymbol.BeginBlock)
			    {
			        string alias = CaptureWord(false);
			
			        nextChar = NextChar();
			        if (nextChar == CSharpSymbol.BeginGeneric)
			        {
			            while (_ch != CSharpSymbol.EndGeneric)
			            {
			                TryReadChar();
			                if (_ch == CSharpSymbol.BeginBlock)
			                {
			                    this.OnParseError("Expected " + CSharpSymbol.EndGeneric);
			                }
			
			                alias += _ch;
			            }
			        }
			
			        if (alias == CSharpKeyword.New)
			        {
			            // new(), for type parameter constraint lists
			            if (TryReadChar(CSharpSymbol.BeginParamList) &&
			                TryReadChar(CSharpSymbol.EndParamList))
			            {
			                alias = CSharpKeyword.NewConstraint;
			            }
			            else
			            {
			                this.OnParseError("Invalid new constraint, use new()");
			            }
			        }
			
			        aliases.Add(alias);
			
			        EatWhitespace();
			
			        nextChar = NextChar();
			        if (nextChar != CSharpSymbol.AliasSeparator)
			        {
			            break;
			        }
			        else
			        {
			            TryReadChar();
			        }
			    }
			}
			
			return aliases.ToArray();
		}

		private string ParseBlock(bool beginExpected)
		{
			return ParseNestedText(CSharpSymbol.BeginBlock, CSharpSymbol.EndBlock, beginExpected, true);
		}

		/// <summary>
		/// Parses a constructor
		/// </summary>
		/// <param name="memberName"></param>
		/// <param name="access"></param>
		/// <param name="memberAttributes"></param>
		/// <returns></returns>
		private ConstructorElement ParseConstructor(string memberName, CodeAccess access, MemberModifier memberAttributes)
		{
			ConstructorElement constructor = new ConstructorElement();
			constructor.Name = memberName;
			constructor.Access = access;
			constructor.MemberModifiers = memberAttributes;
			
			constructor.Params = this.ParseParams();
			
			EatWhitespace();
			bool hasReference = TryReadChar(CSharpSymbol.TypeImplements);
			if (hasReference)
			{
			    EatWhitespace();
			
			    StringBuilder referenceBuilder = new StringBuilder();
			    char nextChar = NextChar();
			    while (nextChar != CSharpSymbol.BeginBlock)
			    {
			        if (nextChar == EmptyChar)
			        {
			            this.UnexpectedEndOfFile();
			        }
			        else
			        {
			            TryReadChar();
			            referenceBuilder.Append(_ch);
			        }
			
			        nextChar = NextChar();
			    }
			
			    constructor.Reference = referenceBuilder.ToString().Trim();
			}
			
			constructor.BodyText = this.ParseBlock(true);
			
			return constructor;
		}

		/// <summary>
		/// Parses a delegate
		/// </summary>
		/// <param name="memberName">Member name</param>
		/// <param name="access">Code access</param>
		/// <param name="memberAttributes">Member attributes</param>
		/// <param name="returnType">Return type</param>
		/// <returns></returns>
		private DelegateElement ParseDelegate(string memberName, CodeAccess access, MemberModifier memberAttributes,
			string returnType)
		{
			DelegateElement delegateElement = new DelegateElement();
			delegateElement.Name = memberName;
			delegateElement.Access = access;
			delegateElement.Type = returnType;
			delegateElement.MemberModifiers = memberAttributes;
			
			delegateElement.Params = this.ParseParams();
			
			EatChar(CSharpSymbol.EndOfStatement);
			
			return delegateElement;
		}

		/// <summary>
		/// Parses elements from the current point in the stream
		/// </summary>
		/// <returns></returns>
		private List<ICodeElement> ParseElements()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();
			List<ICommentLine> commentLines = new List<ICommentLine>();
			List<AttributeElement> attributes = new List<AttributeElement>();
			
			StringBuilder elementBuilder = new StringBuilder();
			
			char nextChar;
			
			while (TryReadChar())
			{
			    switch (_ch)
			    {
			        //
			        // Comments
			        //
			        case CSharpSymbol.BeginComment:
			            nextChar = NextChar();
			            if (nextChar == CSharpSymbol.BeginComment)
			            {
			                TryReadChar();
			
			                bool isXmlComment = false;
			                if (_reader.Peek() == (int)CSharpSymbol.BeginComment)
			                {
			                    isXmlComment = true;
			                    TryReadChar();
			                }
			
			                string commentText = ReadLine();
			                CommentLine commentLine = new CommentLine(
			                    commentText, isXmlComment);
			                commentLines.Add(commentLine);
			            }
			            else if (nextChar == CSharpSymbol.BlockCommentModifier)
			            {
			                TryReadChar();
			                TryReadChar();
			                TryReadChar();
			
			                StringBuilder blockComment = new StringBuilder();
			
			                while (!(_lastCh == CSharpSymbol.BlockCommentModifier &&
			                    _ch == CSharpSymbol.BeginComment))
			                {
			                    blockComment.Append(_lastCh);
			                    TryReadChar();
			                }
			
			                string[] lines = blockComment.ToString().Split(
			                    new string[] { Environment.NewLine }, StringSplitOptions.None);
			
			                foreach (string blockLine in lines)
			                {
			                    CommentLine commentLine = new CommentLine(blockLine);
			                    commentLines.Add(commentLine);
			                }
			            }
			            break;
			
			        //
			        // Preprocessor
			        //
			        case CSharpSymbol.Preprocessor:
			            //
			            // TODO: Besides regions, parse preprocessor elements so that
			            // member preprocessor information is preserved.
			            string line = ReadLine().Trim();
			            if (!(line.StartsWith(CSharpKeyword.Region) || line.StartsWith(CSharpKeyword.EndRegion)))
			            {
			                this.OnParseError("Cannot arrange files with preprocessor directives");
			            }
			            break;
			
			        //
			        // Attribute
			        //
			        case CSharpSymbol.BeginAttribute:
			            nextChar = NextChar();
			            if (elementBuilder.Length > 0)
			            {
			                if (nextChar == CSharpSymbol.EndAttribute)
			                {
			                    // Array type
			                    EatChar(CSharpSymbol.EndAttribute);
			
			                    elementBuilder.Append(CSharpSymbol.BeginAttribute);
			                    elementBuilder.Append(CSharpSymbol.EndAttribute);
			                }
			                else
			                {
			                    elementBuilder.Append(CSharpSymbol.BeginAttribute);
			                }
			            }
			            else
			            {
			                string attributeText = ParseNestedText(CSharpSymbol.BeginAttribute, CSharpSymbol.EndAttribute,
			                    false, true);
			                AttributeElement attributeElement = new AttributeElement();
			                attributeElement.BodyText = attributeText;
			
			                if (commentLines.Count > 0)
			                {
			                    foreach (CommentLine commentLine in commentLines)
			                    {
			                        attributeElement.AddHeaderCommentLine(commentLine);
			                    }
			                    commentLines = new List<ICommentLine>();
			                }
			
			                attributes.Add(attributeElement);
			                codeElements.Add(attributeElement);
			            }
			            break;
			
			        case ' ':
			            if (elementBuilder.Length > 0 &&
			                elementBuilder[elementBuilder.Length - 1] != ' ')
			            {
			                elementBuilder.Append(' ');
			            }
			            break;
			
			        // Eat any unneeded whitespace
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
			            elementBuilder.Append(_ch);
			            nextChar = NextChar();
			
			            if (char.IsWhiteSpace(nextChar) || CSharpSymbol.IsCSharpSymbol(_ch))
			            {
			                //
			                // Try to parse a code element
			                //
			                ICodeElement element = TryParseElement(
			                    elementBuilder, commentLines, attributes);
			                if (element != null)
			                {
			                    codeElements.Add(element);
			                    elementBuilder = new StringBuilder();
			                    commentLines = new List<ICommentLine>();
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
			
			            break;
			    }
			
			    int data = _reader.Peek();
			    char nextCh = (char)data;
			
			    //
			    // Elements should capture closing block characters
			    //
			    if (nextCh == CSharpSymbol.EndBlock)
			    {
			        break;
			    }
			}
			
			return codeElements;
		}

		/// <summary>
		/// Parses an event
		/// </summary>
		/// <param name="access"></param>
		/// <param name="memberAttributes"></param>
		/// <returns></returns>
		private EventElement ParseEvent(CodeAccess access, MemberModifier memberAttributes)
		{
			EventElement eventElement = new EventElement();
			eventElement.Type = CaptureTypeName();
			eventElement.Name = CaptureWord();
			eventElement.Access = access;
			eventElement.MemberModifiers = memberAttributes;
			
			EatWhitespace();
			
			char nextChar = NextChar();
			if (nextChar == CSharpSymbol.EndOfStatement)
			{
			    EatChar(CSharpSymbol.EndOfStatement);
			}
			else
			{
			    eventElement.BodyText = this.ParseBlock(true);
			}
			
			return eventElement;
		}

		private string ParseInitialValue()
		{
			EatWhitespace(true);
			
			string initialValue = ParseNestedText(EmptyChar, CSharpSymbol.EndOfStatement, false, false);
			
			if (string.IsNullOrEmpty(initialValue))
			{
			    this.OnParseError("Expected an initial value");
			}
			    
			return initialValue;
		}

		/// <summary>
		/// Parses a method
		/// </summary>
		/// <param name="memberName">Member name</param>
		/// <param name="access">Code access</param>
		/// <param name="memberAttributes">Member attributes</param>
		/// <param name="returnType">Return type</param>
		/// <param name="isOperator"></param>
		/// <param name="operatorType"></param>
		/// <returns></returns>
		private MethodElement ParseMethod(string memberName, CodeAccess access, MemberModifier memberAttributes,
			string returnType, bool isOperator, OperatorType operatorType)
		{
			MethodElement method = new MethodElement();
			method.Name = memberName;
			method.Access = access;
			method.Type = returnType;
			method.MemberModifiers = memberAttributes;
			method.IsOperator = isOperator;
			method.OperatorType = operatorType;
			if (isOperator &&
			    (returnType == CSharpKeyword.Implicit || returnType == CSharpKeyword.Explicit))
			{
			    method.Type = memberName;
			    method.Name = null;
			}
			
			int genericIndex = memberName.IndexOf(CSharpSymbol.BeginGeneric);
			bool isGeneric = genericIndex >= 0 && genericIndex < memberName.Length - 1;
			if (isGeneric)
			{
			    method.Name = memberName.Substring(0, genericIndex);
			    string typeParameterString = memberName.TrimEnd(CSharpSymbol.EndGeneric).Substring(
			        genericIndex + 1);
			
			    string[] typeParameterNames = typeParameterString.Split(new char[] { CSharpSymbol.AliasSeparator, ' ' },
			        StringSplitOptions.RemoveEmptyEntries);
			    foreach (string typeParameterName in typeParameterNames)
			    {
			        TypeParameter typeParameter = new TypeParameter();
			        typeParameter.Name = typeParameterName;
			        method.TypeParameters.Add(typeParameter);
			    }
			}
			
			method.Params = this.ParseParams();
			
			if (isGeneric)
			{
			    ParseTypeParameterConstraints(method.TypeParameters);
			}
			
			EatWhitespace();
			bool endOfStatement = NextChar() == CSharpSymbol.EndOfStatement;
			if (endOfStatement)
			{
			    TryReadChar();
			    method.BodyText = null;
			}
			else
			{
			    method.BodyText = this.ParseBlock(true);
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
			
			EatChar(CSharpSymbol.BeginBlock);
			
			//
			// Parse child elements
			//
			List<ICodeElement> childElements = ParseElements();
			foreach (ICodeElement childElement in childElements)
			{
			    namespaceElement.AddChild(childElement);
			}
			
			EatChar(CSharpSymbol.EndBlock);
			
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
			
			char nextChar = NextChar();
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
			    bool inCharLiteral = false;
			    bool inLineComment = false;
			    bool inBlockComment = false;
			
			    while (depth > 0)
			    {
			        bool charRead = TryReadChar();
			        if (!charRead)
			        {
			            this.OnParseError("Unexpected end of file. Expected " + endChar);
			        }
			
			        nextChar = NextChar();
			
			        bool inComment = inBlockComment || inLineComment;
			
			        if(!inComment)
			        {
			            if (!inCharLiteral && _ch == CSharpSymbol.BeginString && _lastCh != '\\')
			            {
			                inString = !inString;
			            }
			            else if (!inString && _ch == CSharpSymbol.BeginCharLiteral && _lastCh != '\\')
			            {
			                inCharLiteral = !inCharLiteral;
			            }
			        }
			
			        if (!inCharLiteral && !inString)
			        {
			            if (!inBlockComment && _ch == CSharpSymbol.BeginComment && nextChar == CSharpSymbol.BeginComment)
			            {
			                inLineComment = true;
			            }
			            else if (inLineComment && _ch == '\r' && nextChar == '\n')
			            {
			                inLineComment = false;
			            }
			            else if (!inLineComment && !inBlockComment &&
			                _ch == CSharpSymbol.BeginComment && nextChar == CSharpSymbol.BlockCommentModifier)
			            {
			                inBlockComment = true;
			            }
			            else if (inBlockComment &&
			                _ch == CSharpSymbol.BlockCommentModifier && nextChar == CSharpSymbol.BeginComment)
			            {
			                inBlockComment = false;
			            }
			        }
			
			        inComment = inBlockComment || inLineComment;
			        if (beginChar != EmptyChar && _ch == beginChar && 
			            !inCharLiteral && !inString && !inComment)
			        {
			            blockText.Append(_ch);
			            depth++;
			        }
			        else
			        {
			            blockText.Append(_ch);
			        }
			
			        if (nextChar == endChar && !inString && !inCharLiteral && !inComment)
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
			return ParseNestedText(CSharpSymbol.BeginParamList, CSharpSymbol.EndParamList, false, false);
		}

		/// <summary>
		/// Parses a property
		/// </summary>
		/// <param name="memberName"></param>
		/// <param name="returnType"></param>
		/// <param name="access"></param>
		/// <param name="memberAttributes"></param>
		/// <returns></returns>
		private PropertyElement ParseProperty(string memberName, string returnType, CodeAccess access, MemberModifier memberAttributes)
		{
			PropertyElement property = new PropertyElement();
			property.Name = memberName;
			property.Access = access;
			property.Type = returnType;
			property.MemberModifiers = memberAttributes;
			
			property.BodyText = this.ParseBlock(false);
			
			return property;
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
			    string enumText = ParseBlock(true);
			
			    // TODO: Parse enum values as fields
			    typeElement.BodyText = enumText;
			}
			else
			{
			    bool isGeneric = TryReadChar(CSharpSymbol.BeginGeneric);
			    if (isGeneric)
			    {
			        string[] typeParameterNames = ParseAliasList();
			        foreach (string typeParameterName in typeParameterNames)
			        {
			            TypeParameter typeParameter = new TypeParameter();
			            typeParameter.Name = typeParameterName;
			            typeElement.TypeParameters.Add(typeParameter);
			        }
			
			        EatWhitespace();
			
			        if (!TryReadChar(CSharpSymbol.EndGeneric))
			        {
			            this.OnParseError("Expected " + CSharpSymbol.EndGeneric);
			        }
			    }
			
			    EatWhitespace();
			
			    bool implements = TryReadChar(CSharpSymbol.TypeImplements);
			
			    if (implements)
			    {
			        string[] typeList = ParseAliasList();
			        foreach (string type in typeList)
			        {
			            typeElement.AddInterface(type);
			        }
			    }
			
			    EatWhitespace();
			
			    ParseTypeParameterConstraints(typeElement.TypeParameters);
			
			    EatChar(CSharpSymbol.BeginBlock);
			
			    //
			    // Parse child elements
			    //
			    List<ICodeElement> childElements = ParseElements();
			    foreach (ICodeElement childElement in childElements)
			    {
			        typeElement.AddChild(childElement);
			    }
			
			    EatChar(CSharpSymbol.EndBlock);
			}
			
			return typeElement;
		}

		private void ParseTypeParameterConstraints(List<TypeParameter> parameters)
		{
			char nextChar = EmptyChar;
			while (parameters.Count > 0 && nextChar != CSharpSymbol.BeginBlock)
			{
			    // 
			    // Parse type parameter constraints
			    //
			    string keyWord = CaptureWord();
			    if (keyWord == CSharpKeyword.Where)
			    {
			        EatWhitespace();
			
			        string parameterName = CaptureWord();
			
			        TypeParameter parameter = null;
			        foreach (TypeParameter typeParameter in parameters)
			        {
			            if (typeParameter.Name == parameterName)
			            {
			                parameter = typeParameter;
			                break;
			            }
			        }
			
			        if (parameter == null)
			        {
			            this.OnParseError("Unknown type parameter '" + parameterName + "'");
			        }
			
			        EatWhitespace();
			
			        bool separatorFound = TryReadChar(CSharpSymbol.TypeImplements);
			        if (!separatorFound)
			        {
			            this.OnParseError("Expected " + CSharpSymbol.TypeImplements);
			        }
			
			        string[] typeList = ParseAliasList();
			        foreach (string type in typeList)
			        {
			            parameter.AddConstraint(type);
			        }
			
			        int newIndex = parameter.Constraints.IndexOf(
			            CSharpKeyword.NewConstraint);
			        if (newIndex >= 0 && newIndex + 1 != parameter.Constraints.Count)
			        {
			            this.OnParseError("The " + CSharpKeyword.NewConstraint +
			                " must be the last declared type parameter constraint");
			        }
			
			        nextChar = NextChar();
			    }
			    else
			    {
			        this.OnParseError("Expected type parameter constraint");
			    }
			
			    EatWhitespace();
			}
		}

		private UsingElement ParseUsing()
		{
			UsingElement usingElement = new UsingElement();
			string alias = CaptureWord();
			if (string.IsNullOrEmpty(alias))
			{
			    this.OnParseError("Expected a namepace name");
			}
			
			EatWhitespace();
			
			bool endOfStatement = TryReadChar(CSharpSymbol.EndOfStatement);
			if (endOfStatement)
			{
			    usingElement.Name = alias;
			}
			else
			{
			    bool assign = TryReadChar(CSharpSymbol.Assignment);
			    if (!assign)
			    {
			        this.OnParseError(
			            string.Format("Expected {0} or {1}.",
			            CSharpSymbol.Assignment, CSharpSymbol.EndOfStatement));
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
			            EatChar(CSharpSymbol.EndOfStatement);
			        }
			    }
			}
			
			return usingElement;
		}

		private string ReadLine()
		{
			string commentText = _reader.ReadLine();
			_lineNumber++;
			
			return commentText;
		}

		private void Reset()
		{
			_ch = '\0';
			_lastCh = '\0';
			_lineNumber = 1;
			_position = 1;
		}

		/// <summary>
		/// Tries to parse a code element
		/// </summary>
		/// <param name="elementBuilder"></param>
		/// <param name="commentLines"></param>
		/// <param name="attributes"></param>
		/// <returns></returns>
		private ICodeElement TryParseElement(StringBuilder elementBuilder, List<ICommentLine> commentLines,
			List<AttributeElement> attributes)
		{
			CodeElement codeElement = null;
			
			string processedElementText =
			    elementBuilder.ToString().Trim();
			
			switch (processedElementText)
			{
			    case CSharpKeyword.Namespace:
			        codeElement = ParseNamespace();
			        break;
			
			    case CSharpKeyword.Using:
			        codeElement = ParseUsing();
			        break;
			}
			
			if (codeElement == null)
			{
			    string[] words = processedElementText.TrimEnd(
			        CSharpSymbol.EndOfStatement,
			        CSharpSymbol.Assignment,
			        CSharpSymbol.BeginParamList,
			        CSharpSymbol.BeginBlock).Split(
			        WhitespaceChars,
			        StringSplitOptions.RemoveEmptyEntries);
			
			    if (words.Length > 0 &&
			        (words.Length > 1 ||
			        words[0] == CSharpKeyword.Class ||
			        words[0] == CSharpKeyword.Structure ||
			        words[0] == CSharpKeyword.Interface ||
			        words[0] == CSharpKeyword.Enumeration ||
			        words[0][0] == CSharpSymbol.BeginFinalizer))
			    {
			        char lastChar = processedElementText[processedElementText.Length - 1];
			        bool isStatement = lastChar == CSharpSymbol.EndOfStatement;
			        bool isAssignment = lastChar == CSharpSymbol.Assignment;
			        bool hasParams = lastChar == CSharpSymbol.BeginParamList;
			        bool isProperty = lastChar == CSharpSymbol.BeginBlock;
			
			        StringCollection wordList = new StringCollection();
			        wordList.AddRange(words);                
			
			        bool isClass = false;
			        bool isStruct = false;
			        bool isEnum = false;
			        bool isInterface = false;
			        bool isEvent = false;
			        bool isDelegate = false;
			
			        if (!isProperty)
			        {
			            isClass = wordList.Contains(CSharpKeyword.Class);
			            isStruct = !isClass &&
			                wordList.Contains(CSharpKeyword.Structure);
			            isEnum = !isClass && !isStruct &&
			                wordList.Contains(CSharpKeyword.Enumeration);
			            isInterface = !isClass && !isStruct && !isEnum &&
			                wordList.Contains(CSharpKeyword.Interface);
			            isEvent = !isClass && !isStruct && !isEnum && !isInterface &&
			                wordList.Contains(CSharpKeyword.Event);
			            isDelegate = !isClass && !isStruct && !isEnum && !isInterface && !isDelegate &&
			                wordList.Contains(CSharpKeyword.Delegate);
			        }
			
			        CodeAccess access = CodeAccess.NotSpecified;
			        MemberModifier memberAttributes = MemberModifier.None;
			
			        if (isStatement || isAssignment || hasParams ||
			            isProperty || isStruct || isClass || isInterface || isEnum || isEvent)
			        {
			            access = GetAccess(processedElementText);
			            memberAttributes = GetMemberAttributes(wordList);
			        }
			
			        //
			        // Type definition?
			        //
			        if (isClass || isStruct || isInterface || isEnum)
			        {
			            TypeElementType type = GetTypeElementType(isClass, isStruct, isInterface, isEnum);
			
			            TypeModifier typeAttributes = (TypeModifier)memberAttributes;
			
			            if (wordList.Contains(CSharpKeyword.Partial))
			            {
			                typeAttributes |= TypeModifier.Partial;
			            }
			
			            //
			            // Parse a type definition
			            //
			            codeElement = ParseType(access, typeAttributes, type);
			        }
			        else if (isEvent)
			        {
			            codeElement = ParseEvent(access, memberAttributes);
			        }
			
			        if (codeElement == null)
			        {
			            string memberName = null;
			            string returnType = null;
			
			            if (isStatement || isAssignment || hasParams || isProperty || isEvent)
			            {
			                GetMemberNameAndType(words, out memberName, out returnType);
			            }
			
			            if (hasParams)
			            {
			                if (isDelegate)
			                {
			                    codeElement = ParseDelegate(memberName, access, memberAttributes, returnType);
			                }
			                else
			                {
			                    if (returnType == null)
			                    {
			                        //
			                        // Constructor/finalizer
			                        //
			                        codeElement = ParseConstructor(memberName, access, memberAttributes);
			                    }
			                    else
			                    {
			                        bool isOperator = wordList.Contains(CSharpKeyword.Operator);
			                        OperatorType operatorType = OperatorType.NotSpecified;
			                        if (isOperator)
			                        {
			                            operatorType = GetOperatorType(wordList);
			                        }
			
			                        //
			                        // Method
			                        //
			                        codeElement = ParseMethod(memberName, access, memberAttributes,
			                            returnType, isOperator, operatorType);
			                    }
			                }
			            }
			            else if (isStatement || isAssignment)
			            {
			                //
			                // Field
			                //
			                bool isVolatile = wordList.Contains(CSharpKeyword.Volatile);
			                FieldElement field = CreateField(
			                    memberName, returnType, access, memberAttributes, isVolatile);
			
			                if (isAssignment)
			                {
			                    string initialValue = ParseInitialValue();
			                    field.InitialValue = initialValue;
			                }
			
			                codeElement = field;
			            }
			            else if (isProperty)
			            {
			                codeElement = ParseProperty(memberName, returnType, access, memberAttributes);
			            }
			        }
			    }
			}
			
			CommentedElement commentedElement = codeElement as CommentedElement;
			if (commentedElement != null)
			{
			    //
			    // Add any header comments
			    //
			    foreach (ICommentLine commentLine in commentLines)
			    {
			        commentedElement.AddHeaderCommentLine(commentLine);
			    }
			}
			
			AttributedElement attributedElement = codeElement as AttributedElement;
			if (attributedElement != null)
			{
			    foreach (AttributeElement attribute in attributes)
			    {
			        attributedElement.AddAttribute(attribute);
			
			        //
			        // Treat attribute comments as header comments
			        //
			        if (attribute.HeaderCommentLines.Count > 0)
			        {
			            foreach (ICommentLine commentLine in attribute.HeaderCommentLines)
			            {
			                attributedElement.AddHeaderCommentLine(commentLine);
			            }
			
			            attribute.ClearHeaderCommentLines();
			        }
			    }
			}
			
			return codeElement;
		}

		/// <summary>
		/// Tries to read any character from the stream
		/// </summary>
		/// <returns></returns>
		private bool TryReadChar()
		{
			if (_reader.Read(_charBuffer, 0, 1) > 0)
			{
			    _lastCh = _ch;
			    _ch = _charBuffer[0];
			
			    if (_lastCh == '\r' && _ch == '\n')
			    {
			        _lineNumber++;
			        _position = 1;
			    }
			    else
			    {
			        _position++;
			    }
			
			    return true;
			}
			
			return false;
		}

		/// <summary>
		/// Tries to read the specified character from the stream.
		/// </summary>
		/// <param name="ch"></param>
		/// <returns></returns>
		private bool TryReadChar(char ch)
		{
			int data = _reader.Peek();
			char nextCh = (char)data;
			if (nextCh == ch)
			{
			    TryReadChar();
			    return true;
			}
			
			return false;
		}

		/// <summary>
		/// Throws an unexpected end of file error.
		/// </summary>
		private void UnexpectedEndOfFile()
		{
			throw new ParseException("Unexpected end of file", _lineNumber, _position);
		}

		#endregion Private Methods
	}
}