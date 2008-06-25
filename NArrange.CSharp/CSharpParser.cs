#region Header

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
 *      - Fixed parsing of events with generic return types
 *      - Improved parsing performance by reducing the number of calls to 
 *        TryParseElement
 *      - Parse regions to the element tree
 *      - Preserve block comments
 *		- Fixed parsing of interface event types and non-specified 
 *		  access constructors
 *		- Fixed parsing of string and character literals containing
 *		  backslashes
 *		- Fixed parsing of equal and not equal operators
 *		- Fixed parsing of verbatim string literals (e.g. @"\\Server\")
 *      - Added parsing support for partial methods
 *      - Fixed parsing of array return types with intermixed spaces
 *      - Fixed a parsing error where type parameters were always expected
 *        when parsing generic types
 *      - Preserve header comments without associating w/ using elements
 *      - Fixed parsing of properties with multiple index parameters
 *      - Handle fixed size buffer fields
 *      - Parse attribute names and params to the code element model
 *        vs. entire attribute text
 *      - Improved handling of unhandled element text
 *		- Fixed parsing of new lines in attributes
 *		- Preserve element access when None
 *		- Preserve trailing comments for fields
 *		Justin Dearing
 *		- Removed unused using statements
 *		- Code cleanup via ReSharper 4.0 (http://www.jetbrains.com/resharper/)
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Threading;

using NArrange.Core;
using NArrange.Core.CodeElements;

namespace NArrange.CSharp
{
	/// <summary>
	/// NArrange CSharp parser implementation.
	/// </summary>
	public sealed class CSharpParser : CodeParser
	{
		#region Constants

		/// <summary>
		/// Escape character
		/// </summary>
		private const char EscapeChar = '\\';

		#endregion Constants

		#region Private Methods

		/// <summary>
		/// Captures an type name alias from the stream.
		/// </summary>
		/// <returns></returns>
		private string CaptureTypeName()
		{
			return CaptureTypeName(true);
		}

		/// <summary>
		/// Captures an type name alias from the stream.
		/// </summary>
		/// <param name="captureGeneric"></param>
		/// <returns></returns>
		private string CaptureTypeName(bool captureGeneric)
		{
			string typeName = CaptureWord(captureGeneric);
			EatWhiteSpace();

			//
			// Array with space in between?
			//
			if (CurrentChar == CSharpSymbol.BeginAttribute)
			{
			    EatWhiteSpace();
			    EatChar(CSharpSymbol.EndAttribute);

			    typeName += CSharpSymbol.BeginAttribute.ToString() + CSharpSymbol.EndAttribute.ToString();
			}

			return typeName;
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
			EatWhiteSpace();

			StringBuilder word = new StringBuilder(DefaultWordLength);

			char nextChar = NextChar;
			while (nextChar != EmptyChar)
			{
				if (IsWhiteSpace(nextChar) ||
					(IsAliasBreak(nextChar) &&
					!(nextChar == CSharpSymbol.TypeImplements && (word.ToString() == CSharpKeyword.Global || word.ToString() == CSharpKeyword.Global + CSharpSymbol.TypeImplements.ToString()))) ||
			        (!captureGeneric &&
					(nextChar == CSharpSymbol.BeginGeneric ||
					nextChar == CSharpSymbol.EndGeneric)))
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

		private void EatTrailingEndOfStatement()
		{
			EatWhiteSpace(WhiteSpaceTypes.SpaceAndTab);
			if (NextChar == CSharpSymbol.EndOfStatement)
			{
			    EatChar(CSharpSymbol.EndOfStatement);
			}
		}

		private static CodeAccess GetAccess(StringCollection wordList)
		{
			CodeAccess access = CodeAccess.None;

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Public))
			{
			    access = CodeAccess.Public;
			}
			else if (TryFindAndRemoveWord(wordList, CSharpKeyword.Private))
			{
			    access = CodeAccess.Private;
			}
			else
			{
			    if (TryFindAndRemoveWord(wordList, CSharpKeyword.Protected))
			    {
			        access |= CodeAccess.Protected;
			    }

			    if (TryFindAndRemoveWord(wordList, CSharpKeyword.Internal))
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

			if(TryFindAndRemoveWord(wordList, CSharpKeyword.Class))
			{
				elementType = ElementType.Type;
				typeElementType = TypeElementType.Class;
				return;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Structure))
			{
				elementType = ElementType.Type;
				typeElementType = TypeElementType.Structure;
				return;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Enumeration))
			{
				elementType = ElementType.Type;
				typeElementType = TypeElementType.Enum;
				return;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Interface))
			{
				elementType = ElementType.Type;
				typeElementType = TypeElementType.Interface;
				return;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Event))
			{
				elementType = ElementType.Event;
				return;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Delegate))
			{
				elementType = ElementType.Delegate;
				return;
			}
		}

		private static MemberModifiers GetMemberAttributes(StringCollection wordList)
		{
			MemberModifiers memberAttributes;
			memberAttributes = MemberModifiers.None;

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Sealed))
			{
			    memberAttributes |= MemberModifiers.Sealed;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Abstract))
			{
			    memberAttributes |= MemberModifiers.Abstract;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Static))
			{
			    memberAttributes |= MemberModifiers.Static;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Unsafe))
			{
			    memberAttributes |= MemberModifiers.Unsafe;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Virtual))
			{
			    memberAttributes |= MemberModifiers.Virtual;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Override))
			{
			    memberAttributes |= MemberModifiers.Override;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.New))
			{
			    memberAttributes |= MemberModifiers.New;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Constant))
			{
			    memberAttributes |= MemberModifiers.Constant;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.ReadOnly))
			{
			    memberAttributes |= MemberModifiers.ReadOnly;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.External))
			{
			    memberAttributes |= MemberModifiers.External;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Partial))
			{
			    memberAttributes |= MemberModifiers.Partial;
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
		private static void GetMemberNameAndType(StringCollection words,
			out string name, out string returnType)
		{
			name = null;
			returnType = null;

			for (int wordIndex = 0; wordIndex < words.Count; wordIndex++)
			{
			    string wordGroup = words[wordIndex];
			    int separatorIndex = wordGroup.IndexOf(CSharpSymbol.AliasSeparator);
			    if (separatorIndex >= 0 && wordGroup[wordGroup.Length - 1] != CSharpSymbol.EndAttribute)
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
			            words[wordIndex] = wordGroup;
			        }

			        //
			        // Concatenate comma separated values into logical groups
			        //
			        if (wordGroup[0] == CSharpSymbol.AliasSeparator && wordIndex > 0)
			        {
			            if (wordGroup.Length == 1 && wordIndex < words.Count - 1)
			            {
			                words[wordIndex - 1] = words[wordIndex - 1] +
			                    CSharpSymbol.AliasSeparator + " " +
			                    words[wordIndex + 1];
			                words.RemoveAt(wordIndex);
			                words.RemoveAt(wordIndex);
			                wordIndex--;
			                wordIndex--;
			            }
			            else
			            {
			                words[wordIndex - 1] = words[wordIndex - 1] + wordGroup;
			                words.RemoveAt(wordIndex);
			                wordIndex--;
			            }
			        }
			        else if (wordIndex < words.Count &&
			            wordGroup[wordGroup.Length - 1] == CSharpSymbol.AliasSeparator)
			        {
			            wordGroup = wordGroup + " " + words[wordIndex + 1];
			            words[wordIndex] = wordGroup;
			            words.RemoveAt(wordIndex + 1);
			            wordIndex--;
			        }
			    }
			}

			if (words.Count > 1)
			{
			    int nameIndex = words.Count - 1;
			    name = words[nameIndex];
			    words.RemoveAt(nameIndex);

			    int typeIndex = nameIndex;
			    string typeCandidate;

			    do
			    {
			        typeIndex--;
			        typeCandidate = words[typeIndex];
			        words.RemoveAt(typeIndex);
			    }
			    while (words.Count > 0 &&
			        (typeCandidate == CSharpKeyword.Operator ||
			        typeCandidate == CSharpKeyword.Implicit ||
			        typeCandidate == CSharpKeyword.Explicit));

			    if (name[name.Length - 1] == CSharpSymbol.EndAttribute && words.Count > 0)
			    {
			        //
			        // Property indexer
			        //
			        while (typeIndex > 0 && name.IndexOf(CSharpSymbol.BeginAttribute) < 0)
			        {
			            name = typeCandidate + " " + name;
			            typeIndex--;
			            typeCandidate = words[typeIndex];
			            words.RemoveAt(typeIndex);
			        }

			        if (name[0] == CSharpSymbol.BeginAttribute)
			        {
			            name = typeCandidate + name;
			            typeIndex--;
			            typeCandidate = words[typeIndex];
			            words.RemoveAt(typeIndex);
			        }
			    }

			    //
			    // Array return type with spaces?
			    //
			    while (typeCandidate[typeCandidate.Length - 1] == CSharpSymbol.EndAttribute &&
			        typeCandidate[0] == CSharpSymbol.BeginAttribute)
			    {
			        typeIndex--;
			        typeCandidate = words[typeIndex] + typeCandidate;
			        words.RemoveAt(typeIndex);
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
			        typeCandidate != CSharpKeyword.Virtual &&
			        typeCandidate != CSharpKeyword.Operator &&
			        typeCandidate != CSharpKeyword.Implicit &&
			        typeCandidate != CSharpKeyword.Explicit)
			    {
			        returnType = typeCandidate;
			    }
			}
			else
			{
			    name = words[0];
			    words.RemoveAt(0);
			}
		}

		private static OperatorType GetOperatorType(StringCollection wordList)
		{
			OperatorType operatorType = OperatorType.None;
			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Explicit))
			{
			    operatorType = OperatorType.Explicit;
			}
			else if (TryFindAndRemoveWord(wordList, CSharpKeyword.Implicit))
			{
			    operatorType = OperatorType.Implicit;
			}

			return operatorType;
		}

		/// <summary>
		/// Determines whether or not the specified char is a C# special character
		/// that signals a break in an alias
		/// </summary>
		/// <param name="ch"></param>
		/// <returns></returns>
		private static bool IsAliasBreak(char ch)
		{
			return ch == CSharpSymbol.BeginParameterList ||
			        ch == CSharpSymbol.EndParameterList ||
			        ch == CSharpSymbol.EndOfStatement ||
			        ch == CSharpSymbol.AliasSeparator ||
			        ch == CSharpSymbol.TypeImplements ||
			        ch == CSharpSymbol.BeginBlock ||
			        ch == CSharpSymbol.EndBlock ||
			        ch == CSharpSymbol.Negate ||
			        ch == CSharpSymbol.Assignment ||
			        ch == CSharpSymbol.BeginAttribute ||
			        ch == CSharpSymbol.EndAttribute;
		}

		private string[] ParseAliasList()
		{
			List<string> aliases = new List<string>();

			EatWhiteSpace();

			char nextChar = NextChar;
			if (nextChar == CSharpSymbol.BeginBlock)
			{
			    this.OnParseError("Expected a class or interface name");
			}
			else
			{
			    while (nextChar != EmptyChar && nextChar != CSharpSymbol.BeginBlock)
			    {
			        string alias = CaptureWord(false);

			        EatWhiteSpace();

			        nextChar = NextChar;
			        if (nextChar == CSharpSymbol.BeginGeneric)
			        {
			            alias += CSharpSymbol.BeginGeneric.ToString() +
			                ParseNestedText(CSharpSymbol.BeginGeneric, CSharpSymbol.EndGeneric, true, true) +
			                CSharpSymbol.EndGeneric.ToString();
			        }

			        if (alias == CSharpKeyword.New)
			        {
			            // new(), for type parameter constraint lists
			            if (TryReadChar(CSharpSymbol.BeginParameterList) &&
			                TryReadChar(CSharpSymbol.EndParameterList))
			            {
			                alias = CSharpKeyword.NewConstraint;
			            }
			            else
			            {
			                this.OnParseError("Invalid new constraint, use new()");
			            }
			        }

			        aliases.Add(alias);

			        EatWhiteSpace();

			        nextChar = NextChar;
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

		/// <summary>
		/// Parses an attribute
		/// </summary>
		/// <param name="comments"></param>
		/// <returns></returns>
		private AttributeElement ParseAttribute(ReadOnlyCollection<ICommentElement> comments)
		{
			return ParseAttribute(comments, false);
		}

		/// <summary>
		/// Parses an attribute
		/// </summary>
		/// <param name="comments"></param>
		/// <param name="nested"></param>
		/// <returns></returns>
		private AttributeElement ParseAttribute(ReadOnlyCollection<ICommentElement> comments, bool nested)
		{
			AttributeElement attributeElement = new AttributeElement();

			string typeName = CaptureTypeName(false);
			EatWhiteSpace();

			//
			// Check for an attribute target
			//
			if (TryReadChar(CSharpSymbol.TypeImplements))
			{
			    attributeElement.Target = typeName;
			    typeName = CaptureTypeName(false);
				EatWhiteSpace();
			}

			attributeElement.Name = typeName;

			if (NextChar == CSharpSymbol.BeginParameterList)
			{
			    string attributeText = ParseNestedText(CSharpSymbol.BeginParameterList, CSharpSymbol.EndParameterList,
			        true, false);
			    attributeElement.BodyText = attributeText;
			}

			EatWhiteSpace();

			while(!nested && TryReadChar(CSharpSymbol.AliasSeparator))
			{
			    if (NextChar != CSharpSymbol.AliasSeparator)
			    {
			        AttributeElement childAttributeElement = ParseAttribute(null, true);
			        if (string.IsNullOrEmpty(childAttributeElement.Target))
			        {
			            childAttributeElement.Target = attributeElement.Target;
			        }
			        attributeElement.AddChild(childAttributeElement);
			    }
			}

			EatWhiteSpace();

			if (!nested)
			{
			    EatChar(CSharpSymbol.EndAttribute);
			}

			if (comments != null && comments.Count > 0)
			{
			    foreach (ICommentElement comment in comments)
			    {
			        attributeElement.AddHeaderComment(comment);
			    }
			}

			return attributeElement;
		}

		private string ParseBlock(bool beginExpected, CommentedElement parentElement)
		{
			List<ICommentElement> extraComments = new List<ICommentElement>();

			if (beginExpected)
			{
				// TODO: Assign any parsed comments to the parent element
				extraComments.AddRange(ParseComments());

			    if (parentElement != null)
			    {
			        foreach (ICommentElement comment in extraComments)
			        {
			            parentElement.AddHeaderComment(comment);
			        }
			    }
			}

			return ParseNestedText(CSharpSymbol.BeginBlock, CSharpSymbol.EndBlock, beginExpected, true);
		}

		/// <summary>
		/// Parses a comment block
		/// </summary>
		/// <returns></returns>
		private CommentElement ParseCommentBlock()
		{
			TryReadChar();
			TryReadChar();
			TryReadChar();

			StringBuilder blockComment = new StringBuilder(DefaultBlockLength);

			while (!(PreviousChar == CSharpSymbol.BlockCommentModifier &&
			    CurrentChar == CSharpSymbol.BeginComment))
			{
				blockComment.Append(PreviousChar);
			    TryReadChar();
			}

			return new CommentElement(blockComment.ToString(), CommentType.Block);
		}

		/// <summary>
		/// Parses a comment line
		/// </summary>
		/// <returns></returns>
		private CommentElement ParseCommentLine()
		{
			CommentElement commentLine;
			TryReadChar();

			CommentType commentType = CommentType.Line;
			if (NextChar == CSharpSymbol.BeginComment)
			{
			    commentType = CommentType.XmlLine;
			    TryReadChar();
			}

			string commentText = ReadLine();
			commentLine = new CommentElement(commentText, commentType);
			return commentLine;
		}

		private ReadOnlyCollection<ICommentElement> ParseComments()
		{
			EatWhiteSpace();

			List<ICommentElement> comments = new List<ICommentElement>();

			char nextChar = NextChar;
			while (nextChar == CSharpSymbol.BeginComment)
			{
				TryReadChar();

				nextChar = NextChar;
				if (nextChar == CSharpSymbol.BeginComment)
				{
					CommentElement commentLine = ParseCommentLine();
					comments.Add(commentLine);
				}
				else if (nextChar == CSharpSymbol.BlockCommentModifier)
				{
					CommentElement commentBlock = ParseCommentBlock();
					comments.Add(commentBlock);
				}
				else
				{
					this.OnParseError(
						string.Format(Thread.CurrentThread.CurrentCulture,
			            "Invalid character '{0}'", CSharpSymbol.BeginComment));
				}

			    EatWhiteSpace();

			    nextChar = NextChar;
			}

			EatWhiteSpace();

			return comments.AsReadOnly();
		}

		/// <summary>
		/// Parses a constructor
		/// </summary>
		/// <param name="memberName"></param>
		/// <param name="access"></param>
		/// <param name="memberAttributes"></param>
		/// <returns></returns>
		private ConstructorElement ParseConstructor(string memberName, CodeAccess access, MemberModifiers memberAttributes)
		{
			ConstructorElement constructor = new ConstructorElement();
			constructor.Name = memberName;
			constructor.Access = access;
			constructor.MemberModifiers = memberAttributes;

			constructor.Parameters = this.ParseParams();

			EatWhiteSpace();

			List<ICommentElement> extraComments = new List<ICommentElement>();
			extraComments.AddRange(ParseComments());

			EatWhiteSpace();

			bool hasReference = TryReadChar(CSharpSymbol.TypeImplements);
			if (hasReference)
			{
			    EatWhiteSpace();

			    extraComments.AddRange(ParseComments());

			    StringBuilder referenceBuilder = new StringBuilder(DefaultWordLength);

			    EatWhiteSpace();
			    referenceBuilder.Append(CaptureWord());

			    EatWhiteSpace();
			    string referenceParams =
			        ParseNestedText(CSharpSymbol.BeginParameterList, CSharpSymbol.EndParameterList, true, true);
			    referenceBuilder.Append(CSharpSymbol.BeginParameterList);
			    referenceBuilder.Append(referenceParams);
			    referenceBuilder.Append(CSharpSymbol.EndParameterList);

			    constructor.Reference = referenceBuilder.ToString();
			}

			constructor.BodyText = this.ParseBlock(true, constructor);

			foreach (ICommentElement comment in extraComments)
			{
			    constructor.AddHeaderComment(comment);
			}

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
		private DelegateElement ParseDelegate(string memberName, CodeAccess access, MemberModifiers memberAttributes,
			string returnType)
		{
			DelegateElement delegateElement = new DelegateElement();
			delegateElement.Name = memberName;
			delegateElement.Access = access;
			delegateElement.Type = returnType;
			delegateElement.MemberModifiers = memberAttributes;

			int genericIndex = memberName.IndexOf(CSharpSymbol.BeginGeneric);
			bool isGeneric = genericIndex >= 0 && genericIndex < memberName.Length - 1;
			if (isGeneric)
			{
				delegateElement.Name = memberName.Substring(0, genericIndex);
				string typeParameterString = memberName.TrimEnd(CSharpSymbol.EndGeneric).Substring(
					genericIndex + 1);

				string[] typeParameterNames = typeParameterString.Split(new char[] { CSharpSymbol.AliasSeparator, ' ' },
					StringSplitOptions.RemoveEmptyEntries);
				foreach (string typeParameterName in typeParameterNames)
				{
					TypeParameter typeParameter = new TypeParameter();
					typeParameter.Name = typeParameterName;
					delegateElement.AddTypeParameter(typeParameter);
				}
			}

			delegateElement.Parameters = this.ParseParams();

			if (isGeneric)
			{
				ParseTypeParameterConstraints(delegateElement);
			}

			EatChar(CSharpSymbol.EndOfStatement);

			return delegateElement;
		}

		/// <summary>
		/// Parses a collection of elements.
		/// </summary>
		/// <param name="parentElement">Parent element</param>
		/// <returns></returns>
		private List<ICodeElement> ParseElements(ICodeElement parentElement)
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();
			List<ICommentElement> comments = new List<ICommentElement>();
			List<AttributeElement> attributes = new List<AttributeElement>();
			Stack<RegionElement> regionStack = new Stack<RegionElement>();

			StringBuilder elementBuilder = new StringBuilder(DefaultBlockLength);

			char nextChar;

			while (TryReadChar())
			{
			    switch (CurrentChar)
			    {
			        //
			        // Comments
			        //
			        case CSharpSymbol.BeginComment:
			            nextChar = NextChar;
			            if (nextChar == CSharpSymbol.BeginComment)
			            {
			                CommentElement commentLine = ParseCommentLine();
			                comments.Add(commentLine);
			            }
			            else if (nextChar == CSharpSymbol.BlockCommentModifier)
			            {
			                CommentElement commentBlock = ParseCommentBlock();
			                comments.Add(commentBlock);
			            }
			            else
			            {
			                elementBuilder.Append(CurrentChar);
			            }
			            break;

			        //
			        // Preprocessor
			        //
			        case CSharpSymbol.Preprocessor:
			            //
			            // TODO: Besides regions, parse preprocessor elements so that
			            // member preprocessor information is preserved.
			            //
			            string line = ReadLine().Trim();
			            if (line.StartsWith(CSharpKeyword.Region, StringComparison.Ordinal))
			            {
			                if (comments.Count > 0)
			                {
			                    foreach (ICommentElement commentElement in comments)
			                    {
			                        codeElements.Add(commentElement);
			                    }
			                    comments.Clear();
			                }

			                RegionElement regionElement = ParseRegion(line);
			                regionStack.Push(regionElement);
			            }
			            else if (line.StartsWith(CSharpKeyword.EndRegion, StringComparison.Ordinal)
			                && regionStack.Count > 0)
			            {
			                RegionElement regionElement = regionStack.Pop();

			                if (comments.Count > 0)
			                {
			                    foreach (ICommentElement commentElement in comments)
			                    {
			                        regionElement.AddChild(commentElement);
			                    }
			                    comments.Clear();
			                }

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
			                    "other than #region and #endregion");
			            }
			            break;

			        //
			        // Attribute
			        //
			        case CSharpSymbol.BeginAttribute:
			            nextChar = NextChar;

			            //
			            // Parse array definition
			            //
			            if (elementBuilder.Length > 0)
			            {
			                EatWhiteSpace();
			                nextChar = NextChar;

			                if (nextChar == CSharpSymbol.EndAttribute)
			                {
			                    // Array type
			                    EatChar(CSharpSymbol.EndAttribute);

			                    elementBuilder.Append(CSharpSymbol.BeginAttribute);
			                    elementBuilder.Append(CSharpSymbol.EndAttribute);
			                    elementBuilder.Append(' ');
			                }
			                else
			                {
			                    string nestedText = ParseNestedText(
			                        CSharpSymbol.BeginAttribute,
			                        CSharpSymbol.EndAttribute,
			                        false,
			                        true);

			                    elementBuilder.Append(CSharpSymbol.BeginAttribute);
			                    elementBuilder.Append(nestedText);
			                    elementBuilder.Append(CSharpSymbol.EndAttribute);
			                }
			            }
			            else
			            {
			                //
			                // Parse attribute
			                //
			                AttributeElement attributeElement = ParseAttribute(comments.AsReadOnly());

			                attributes.Add(attributeElement);
			                codeElements.Add(attributeElement);
			                comments.Clear();
			            }
			            break;

			        //
			        // Trim generics
			        //
			        case CSharpSymbol.BeginGeneric:
			            string elementText = elementBuilder.ToString();
			            if (elementBuilder.Length > 0 &&
			                !(elementText.Trim().EndsWith(CSharpKeyword.Operator, StringComparison.Ordinal)))
			            {
			                string nestedText = ParseNestedText(
			                    CSharpSymbol.BeginGeneric,
			                    CSharpSymbol.EndGeneric,
			                    false,
			                    true);

			                elementBuilder.Append(CSharpSymbol.BeginGeneric);
			                elementBuilder.Append(nestedText);
			                elementBuilder.Append(CSharpSymbol.EndGeneric);
			            }
			            else
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
			            nextChar = NextChar;

			            if (char.IsWhiteSpace(nextChar) || CSharpSymbol.IsCSharpSymbol(CurrentChar))
			            {
			                //
			                // Try to parse a code element
			                //
			                ICodeElement element = TryParseElement(parentElement,
			                    elementBuilder, comments.AsReadOnly(), attributes.AsReadOnly());
			                if (element != null)
			                {
			                    if (element is CommentedElement)
			                    {
			                        UsingElement usingElement = element as UsingElement;

			                        //
			                        // If this is the first using statement, then don't attach
			                        // header comments to the element.
			                        //
			                        if (usingElement != null && parentElement == null && codeElements.Count == 0)
			                        {
			                            foreach (ICommentElement commentElement in usingElement.HeaderComments)
			                            {
			                                if (regionStack.Count > 0)
			                                {
			                                    regionStack.Peek().AddChild(commentElement);
			                                }
			                                else
			                                {
			                                    codeElements.Add(commentElement);
			                                }
			                            }
			                            usingElement.ClearHeaderCommentLines();
			                        }
			                        comments.Clear();
			                    }

			                    if (regionStack.Count > 0)
			                    {
			                        regionStack.Peek().AddChild(element);
			                    }
			                    else
			                    {
			                        codeElements.Add(element);
			                    }
			                    
			                    elementBuilder = new StringBuilder(DefaultBlockLength);
			                    
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

			    char nextCh = NextChar;

			    //
			    // Elements should capture closing block characters
			    //
			    if (nextCh == CSharpSymbol.EndBlock)
			    {
			        break;
			    }
			}

			if (comments.Count > 0)
			{
			    foreach (ICommentElement comment in comments)
			    {
			        codeElements.Insert(0, comment);
			    }
			}

			//
			// Make sure that all region elements have been closed
			//
			if (regionStack.Count > 0)
			{
			    this.OnParseError("Expected #endregion");
			}

			if (elementBuilder.Length > 0)
			{
			    this.OnParseError(
			        string.Format(Thread.CurrentThread.CurrentCulture,
			        "Unhandled element text '{0}'", elementBuilder));
			}

			return codeElements;
		}

		/// <summary>
		/// Parses an event
		/// </summary>
		/// <param name="access"></param>
		/// <param name="memberAttributes"></param>
		/// <returns></returns>
		private EventElement ParseEvent(CodeAccess access, MemberModifiers memberAttributes)
		{
			EventElement eventElement = new EventElement();
			eventElement.Type = CaptureTypeName();
			eventElement.Name = CaptureWord();
			eventElement.Access = access;
			eventElement.MemberModifiers = memberAttributes;

			EatWhiteSpace();

			char nextChar = NextChar;
			if (nextChar == CSharpSymbol.EndOfStatement)
			{
			    EatChar(CSharpSymbol.EndOfStatement);
			}
			else
			{
			    eventElement.BodyText = this.ParseBlock(true, eventElement);
			}

			return eventElement;
		}

		private FieldElement ParseField(bool isAssignment, CodeAccess access,
			MemberModifiers memberAttributes, string memberName,
			string returnType, bool isVolatile, bool isFixed)
		{
			FieldElement field = new FieldElement();
			field.Name = memberName;
			field.Type = returnType;
			field.Access = access;
			field.MemberModifiers = memberAttributes;
			field.IsVolatile = isVolatile;
			field[CSharpExtendedProperties.Fixed] = isFixed;

			if (isAssignment)
			{
			    string initialValue = ParseInitialValue();
			    field.InitialValue = initialValue;
			}

			EatWhiteSpace(WhiteSpaceTypes.SpaceAndTab);
			if (NextChar == CSharpSymbol.BeginComment)
			{
			    EatChar(CSharpSymbol.BeginComment);
			    if (NextChar == CSharpSymbol.BeginComment)
			    {
					field.TrailingComment = ParseCommentLine();
			    }
				else if (NextChar == CSharpSymbol.BlockCommentModifier)
				{
					field.TrailingComment = ParseCommentBlock();
				}
			}
			return field;
		}

		private string ParseInitialValue()
		{
			EatWhiteSpace(WhiteSpaceTypes.SpaceAndTab);

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
		private MethodElement ParseMethod(string memberName, CodeAccess access, MemberModifiers memberAttributes,
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
			    (operatorType == OperatorType.Implicit || operatorType == OperatorType.Explicit))
			{
			    method.Type = memberName;
			    method.Name = null;
			}

			int genericIndex = memberName.LastIndexOf(CSharpSymbol.BeginGeneric);
			int lastQualifierIndex = memberName.LastIndexOf(CSharpSymbol.AliasQualifier);
			bool isGeneric = !isOperator &&
			    (genericIndex >= 0 && genericIndex < memberName.Length - 1 &&
			    (lastQualifierIndex < 0 || lastQualifierIndex < genericIndex));
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
			        method.AddTypeParameter(typeParameter);
			    }
			}

			method.Parameters = this.ParseParams();

			if (isGeneric)
			{
			    ParseTypeParameterConstraints(method);
			}

			EatWhiteSpace();
			bool endOfStatement = NextChar == CSharpSymbol.EndOfStatement;
			if (endOfStatement)
			{
			    TryReadChar();
			    method.BodyText = null;
			}
			else
			{
			    method.BodyText = this.ParseBlock(true, method);
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

			EatWhiteSpace();

			if (NextChar != CSharpSymbol.EndBlock)
			{
				//
				// Parse child elements
				//
				List<ICodeElement> childElements = ParseElements(namespaceElement);
				foreach (ICodeElement childElement in childElements)
				{
					namespaceElement.AddChild(childElement);
				}
			}

			EatChar(CSharpSymbol.EndBlock);

			//
			// Namespaces allow a trailing semi-colon
			//
			EatTrailingEndOfStatement();

			return namespaceElement;
		}

		private string ParseNestedText(char beginChar, char endChar, bool beginExpected, bool trim)
		{
			StringBuilder blockText = new StringBuilder(DefaultBlockLength);

			if (beginChar != EmptyChar && beginExpected)
			{
				while (IsWhiteSpace(NextChar))
				{
					TryReadChar();
					if (!trim)
					{
						blockText.Append(CurrentChar);
					}
				}

			    EatChar(beginChar);
			}

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
			    bool inCharLiteral = false; 
			    bool inLineComment = false;
			    bool inBlockComment = false;
				bool inVerbatimString = false;

			    while (depth > 0)
			    {
					char previousPreviousChar = PreviousChar;

			        bool charRead = TryReadChar();
			        if (!charRead)
			        {
			            this.OnParseError("Unexpected end of file. Expected " + endChar);
			        }

			        nextChar = NextChar;

			        bool inComment = inBlockComment || inLineComment;

			        if(!inComment)
			        {
			            if (!inCharLiteral && CurrentChar == CSharpSymbol.BeginString 
							&& (inVerbatimString || 
							(PreviousChar != EscapeChar || 
							(PreviousChar == EscapeChar && previousPreviousChar == EscapeChar))))
			            {
			                inString = !inString;
							inVerbatimString = inString && PreviousChar == CSharpSymbol.BeginVerbatimString;
			            }
						else if (!inString && CurrentChar == CSharpSymbol.BeginCharLiteral
							&& (PreviousChar != EscapeChar ||
							(PreviousChar == EscapeChar && previousPreviousChar == EscapeChar)))
			            {
			                inCharLiteral = !inCharLiteral;
			            }
			        }

			        if (!inCharLiteral && !inString)
			        {
						if (!inBlockComment && CurrentChar == CSharpSymbol.BeginComment && 
							nextChar == CSharpSymbol.BeginComment)
			            {
			                inLineComment = true;
			            }
						else if (inLineComment && ((CurrentChar == Environment.NewLine[0] && 
			                (Environment.NewLine.Length == 1 || nextChar == Environment.NewLine[1])) ||
			                CurrentChar == '\n'))
			            {
			                inLineComment = false;
			            }
			            else if (!inLineComment && !inBlockComment &&
							CurrentChar == CSharpSymbol.BeginComment && 
							nextChar == CSharpSymbol.BlockCommentModifier)
			            {
			                inBlockComment = true;
			            }
			            else if (inBlockComment &&
							CurrentChar == CSharpSymbol.BlockCommentModifier && 
							nextChar == CSharpSymbol.BeginComment)
			            {
			                inBlockComment = false;
			            }
			        }

			        inComment = inBlockComment || inLineComment;
					if (beginChar != EmptyChar && CurrentChar == beginChar && 
			            !inCharLiteral && !inString && !inComment)
			        {
						blockText.Append(CurrentChar);
			            depth++;
			        }
			        else
			        {
						blockText.Append(CurrentChar);
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
			return ParseNestedText(CSharpSymbol.BeginParameterList, CSharpSymbol.EndParameterList, false, false);
		}

		/// <summary>
		/// Parses a property
		/// </summary>
		/// <param name="memberName"></param>
		/// <param name="returnType"></param>
		/// <param name="access"></param>
		/// <param name="memberAttributes"></param>
		/// <returns></returns>
		private PropertyElement ParseProperty(string memberName, string returnType, CodeAccess access, MemberModifiers memberAttributes)
		{
			PropertyElement property = new PropertyElement();

			int indexStart = memberName.IndexOf(CSharpSymbol.BeginAttribute);
			if (indexStart >= 0)
			{
				string indexParameter = memberName.Substring(indexStart).Trim().Trim(
					CSharpSymbol.BeginAttribute, CSharpSymbol.EndAttribute).Trim();
				property.IndexParameter = indexParameter;
				memberName = memberName.Substring(0, indexStart);
			}

			property.Name = memberName;
			property.Access = access;
			property.Type = returnType;
			property.MemberModifiers = memberAttributes;

			property.BodyText = this.ParseBlock(false, property);

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
			string regionName = line.Substring(CSharpKeyword.Region.Length).Trim();

			// A region name is not required, so allow empty string

			regionElement = new RegionElement();
			regionElement.Name = regionName;
			return regionElement;
		}

		/// <summary>
		/// Parses a type definition
		/// </summary>
		/// <returns></returns>
		private TypeElement ParseType(
			CodeAccess access, TypeModifiers typeAttributes,
			TypeElementType elementType)
		{
			TypeElement typeElement = new TypeElement();

			EatWhiteSpace();
			string className = CaptureWord();
			typeElement.Name = className;
			typeElement.Access = access;
			typeElement.Type = elementType;
			typeElement.TypeModifiers = typeAttributes;

			EatWhiteSpace();

			if (elementType == TypeElementType.Enum)
			{
			    EatWhiteSpace();

			    if (NextChar == CSharpSymbol.TypeImplements)
			    {
			        TryReadChar();
			        string interfaceName = CaptureTypeName();
			        InterfaceReference interfaceReference = 
			            new InterfaceReference(interfaceName, InterfaceReferenceType.None);
			        typeElement.AddInterface(interfaceReference);
			    }

			    string enumText = ParseBlock(true, typeElement);

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
						typeElement.AddTypeParameter(typeParameter);
			        }

			        EatWhiteSpace();

			        if (!TryReadChar(CSharpSymbol.EndGeneric))
			        {
			            this.OnParseError("Expected " + CSharpSymbol.EndGeneric);
			        }
			    }

			    EatWhiteSpace();

			    bool implements = TryReadChar(CSharpSymbol.TypeImplements);

			    if (implements)
			    {
			        string[] typeList = ParseAliasList();
			        foreach (string type in typeList)
			        {
			            InterfaceReference interfaceReference = 
			                new InterfaceReference(type, InterfaceReferenceType.None);
			            typeElement.AddInterface(interfaceReference);
			        }
			    }

			    EatWhiteSpace();

			    ParseTypeParameterConstraints(typeElement);

				// Associate any additional comments in the type definition with the type.
				ReadOnlyCollection<ICommentElement> extraComments = ParseComments();
			    foreach (ICommentElement comment in extraComments)
			    {
			        typeElement.AddHeaderComment(comment);
			    }

			    EatChar(CSharpSymbol.BeginBlock);

				EatWhiteSpace();

				if (NextChar != CSharpSymbol.EndBlock)
				{
					//
					// Parse child elements
					//
					List<ICodeElement> childElements = ParseElements(typeElement);
					foreach (ICodeElement childElement in childElements)
					{
						typeElement.AddChild(childElement);
					}
				}

			    EatChar(CSharpSymbol.EndBlock);
			}

			//
			// Types allow a trailing semi-colon
			//
			EatTrailingEndOfStatement();

			return typeElement;
		}

		private void ParseTypeParameterConstraints(IGenericElement genericElement)
		{
			EatWhiteSpace();

			if (NextChar == CSharpKeyword.Where[0])
			{
			    List<ICommentElement> extraComments = new List<ICommentElement>();

			    while (genericElement.TypeParameters.Count > 0 &&
			        NextChar != CSharpSymbol.BeginBlock &&
			        NextChar != CSharpSymbol.EndOfStatement)
			    {

			        // 
			        // Parse type parameter constraints
			        //
			        string keyWord = CaptureWord();
			        if (keyWord == CSharpKeyword.Where)
			        {
			            extraComments.AddRange(ParseComments());

			            string parameterName = CaptureWord();

			            TypeParameter parameter = null;
			            foreach (TypeParameter typeParameter in genericElement.TypeParameters)
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

			            extraComments.AddRange(ParseComments());

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
			        }
			        else
			        {
			            this.OnParseError("Expected type parameter constraint");
			        }

			        extraComments.AddRange(ParseComments());
			    }

			    CommentedElement commentedElement = genericElement as CommentedElement;
			    if (commentedElement != null)
			    {
			        foreach (ICommentElement comment in extraComments)
			        {
			            commentedElement.AddHeaderComment(comment);
			        }
			    }
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

			EatWhiteSpace();

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
			            string.Format(Thread.CurrentThread.CurrentCulture,
			            "Expected {0} or {1}.",
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

		private static bool TryFindAndRemoveWord(StringCollection wordList, string word)
		{
			bool removed = false;

			int wordIndex = wordList.IndexOf(word);
			if (wordIndex >= 0)
			{
			    wordList.RemoveAt(wordIndex);
			    removed = true;
			}

			return removed;
		}

		/// <summary>
		/// Tries to parse a code element
		/// </summary>
		/// <param name="parentElement"></param>
		/// <param name="elementBuilder"></param>
		/// <param name="comments"></param>
		/// <param name="attributes"></param>
		/// <returns></returns>
		private ICodeElement TryParseElement(
			ICodeElement parentElement,
			StringBuilder elementBuilder,
			ReadOnlyCollection<ICommentElement> comments,
			ReadOnlyCollection<AttributeElement> attributes)
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
			    string tempElementText = processedElementText;
			    bool isOperator = processedElementText.Contains(' ' + CSharpKeyword.Operator);
			    if (!isOperator)
			    {
			        tempElementText = processedElementText.TrimEnd(CSharpSymbol.Assignment);
			    }

			    string[] words = tempElementText.TrimEnd(
			        CSharpSymbol.EndOfStatement,
			        CSharpSymbol.BeginParameterList,
			        CSharpSymbol.BeginBlock).Split(
			        WhiteSpaceCharacters,
			        StringSplitOptions.RemoveEmptyEntries);

				char lastChar = processedElementText[processedElementText.Length - 1];
				bool isStatement = lastChar == CSharpSymbol.EndOfStatement;
				bool hasParams = lastChar == CSharpSymbol.BeginParameterList;
				bool isProperty = lastChar == CSharpSymbol.BeginBlock;

			    if (words.Length > 0 &&
			        (words.Length > 1 ||
			        words[0] == CSharpKeyword.Class ||
			        words[0] == CSharpKeyword.Structure ||
			        words[0] == CSharpKeyword.Interface ||
			        words[0] == CSharpKeyword.Enumeration ||
					words[0] == CSharpKeyword.Event ||
			        words[0][0] == CSharpSymbol.BeginFinalizer ||
					isStatement || hasParams || isProperty))
			    {
					bool isAssignment = !isOperator &&
			            lastChar == CSharpSymbol.Assignment && 
						NextChar != CSharpSymbol.Assignment &&
						PreviousChar != CSharpSymbol.Assignment &&
						PreviousChar != CSharpSymbol.Negate;

			        StringCollection wordList = new StringCollection();
			        int operatorLength = CSharpKeyword.Operator.Length;
			        foreach (string word in words)
			        {
			            if (word.Length > operatorLength &&
			                word.StartsWith(CSharpKeyword.Operator, StringComparison.Ordinal) &&
			                !char.IsLetterOrDigit(word[operatorLength]))
			            {
			                wordList.Add(CSharpKeyword.Operator);
			                wordList.Add(word.Substring(operatorLength));
			            }
			            else
			            {
			                wordList.Add(word);
			            }
			        }

					ElementType elementType;
					TypeElementType? typeElementType = null;
					if (isProperty)
					{
						elementType = ElementType.Property;
					}
					else
					{
						GetElementType(wordList, out elementType, out typeElementType);
					}

			        CodeAccess access = CodeAccess.None;
			        MemberModifiers memberAttributes = MemberModifiers.None;
			        OperatorType operatorType = OperatorType.None;

			        if (isStatement || isAssignment || hasParams ||
						elementType == ElementType.Property ||
						elementType == ElementType.Event ||
						elementType == ElementType.Delegate ||
						elementType == ElementType.Type)
			        {
						access = GetAccess(wordList);
			            memberAttributes = GetMemberAttributes(wordList);
			            operatorType = GetOperatorType(wordList);
			        }

			        //
			        // Type definition?
			        //
					if (elementType == ElementType.Type)
			        {
			            TypeModifiers typeAttributes = (TypeModifiers)memberAttributes;

			            //
			            // Parse a type definition
			            //
			            codeElement = ParseType(access, typeAttributes, typeElementType.Value);
			        }
			        else if (elementType == ElementType.Event)
			        {
			            codeElement = ParseEvent(access, memberAttributes);
			        }

			        if (codeElement == null)
			        {
			            string memberName = null;
			            string returnType = null;

			            if (isStatement || isAssignment || hasParams || isProperty)
			            {
			                GetMemberNameAndType(wordList, out memberName, out returnType);
			            }

			            if (hasParams)
			            {
			                if (elementType == ElementType.Delegate)
			                {
			                    codeElement = ParseDelegate(memberName, access, memberAttributes, returnType);
			                }
			                else
			                {
			                    if (returnType == null && operatorType == OperatorType.None)
			                    {
			                        //
			                        // Constructor/finalizer
			                        //
			                        codeElement = ParseConstructor(memberName, access, memberAttributes);
			                    }
			                    else
			                    {
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
			                bool isVolatile = TryFindAndRemoveWord(wordList, CSharpKeyword.Volatile);
			                bool isFixed = TryFindAndRemoveWord(wordList, CSharpKeyword.Fixed);
			                FieldElement field = ParseField(isAssignment, access, memberAttributes, 
			                    memberName, returnType, isVolatile, isFixed);

			                codeElement = field;
			            }
			            else if (elementType == ElementType.Property)
			            {
			                codeElement = ParseProperty(memberName, returnType, access, memberAttributes);
			            }
			        }

			        // Check for any unhandled element text
			        if (codeElement != null && wordList.Count > 0)
			        {
			            StringBuilder remainingWords = new StringBuilder();
			            foreach (string word in wordList)
			            {
			                remainingWords.Append(word + " ");
			            }

			            this.OnParseError(
			                string.Format(Thread.CurrentThread.CurrentCulture,
			                "Unhandled element text '{0}'", remainingWords.ToString().Trim()));
			        }
			    }
			}

			if (codeElement is InterfaceMemberElement || codeElement is TypeElement ||
			    codeElement is ConstructorElement || codeElement is NamespaceElement)
			{
			    //
			    // Eat closing comments
			    //
			    if (NextChar != EmptyChar)
			    {
			        EatWhiteSpace(WhiteSpaceTypes.SpaceAndTab);
			        if (NextChar == CSharpSymbol.BeginComment)
			        {
			            EatChar(CSharpSymbol.BeginComment);
			            ReadLine();
			        }
			    }
			}

			if (codeElement != null)
			{
				ApplyCommentsAndAttributes(codeElement, comments, attributes);
			}

			return codeElement;
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