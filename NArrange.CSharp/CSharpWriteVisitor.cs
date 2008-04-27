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
 *		- Fixed an extra line feed for namespace-nested using statements
 *		- Fixed writing of C# redefine using statements
 *      - Provided support for closing comments
 *      - Fixed extra tabs being written after field declaration statements
 *      - Handle writing of partial methods
 *      - Only write type parameter constraints when they are present.
 *      - Fixed writing of volatile fields
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

using NArrange.Core;                                        
using NArrange.Core.CodeElements;
using NArrange.Core.Configuration;

namespace NArrange.CSharp
{
	/// <summary>
	/// Visits a tree of code elements for writing C# code 
	/// </summary>
	internal sealed class CSharpWriteVisitor : ICodeElementVisitor
	{
		#region Constants

		private const int DefaultBlockLength = 256;

		#endregion Constants

		#region Fields

		private CodeConfiguration _configuration;
		private int _tabCount;
		private TextWriter _writer;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new CSharpWriteVisitor
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="configuration"></param>
		public CSharpWriteVisitor(TextWriter writer, CodeConfiguration configuration)
		{
			if (writer == null)
			{
			    throw new ArgumentNullException("writer");
			}

			Debug.Assert(configuration != null, "Configuration should not be null.");

			_writer = writer;
			_configuration = configuration;
		}

		#endregion Constructors

		#region Private Methods

		[SuppressMessage("Microsoft.Globalization", "CA1308")]
		private void WriteAccess(CodeAccess codeAccess)
		{
			string accessString = string.Empty;
			if (codeAccess != CodeAccess.None)
			{
			    accessString = EnumUtilities.ToString(codeAccess).ToLowerInvariant().Replace(",", string.Empty) + " ";
			}

			WriteIndented(accessString);
		}

		/// <summary>
		/// Writes a collection of element attributes
		/// </summary>
		/// <param name="element"></param>
		private void WriteAttributes(AttributedElement element)
		{
			foreach (IAttributeElement attribute in element.Attributes)
			{
			    attribute.Accept(this);
			}
		}

		private void WriteBeginBlock()
		{
			WriteIndentedLine(CSharpSymbol.BeginBlock.ToString());
			_tabCount++;
		}

		private void WriteBody(TextCodeElement element)
		{
			WriteBeginBlock();
			if (element.BodyText != null && element.BodyText.Trim().Length > 0)
			{
			    WriteTextBlock(element.BodyText);
			    WriteEndBlock();
			    WriteClosingComment(element);
			}
			else
			{
			    _tabCount--;
			    WriteIndented(CSharpSymbol.EndBlock.ToString());
			}
		}

		private void WriteChildren(ICodeElement element)
		{
			//
			// Process all children
			//
			for(int childIndex = 0; childIndex < element.Children.Count; childIndex++)
			{
			    ICodeElement childElement = element.Children[childIndex];

				TextCodeElement textCodeElement = childElement as TextCodeElement;
				if (childIndex > 0 && textCodeElement != null && 
					textCodeElement.BodyText == null && textCodeElement.Children.Count == 0 &&
			        textCodeElement.HeaderComments.Count > 0 && 
			        !(textCodeElement is MethodElement || textCodeElement is DelegateElement))
			    {
			        _writer.WriteLine();
			    }

			    childElement.Accept(this);

			    if (childIndex < element.Children.Count - 1)
			    {
			        if (!(childElement is GroupElement))
			        {
						textCodeElement = childElement as TextCodeElement;
			            if (textCodeElement == null || textCodeElement.BodyText != null || 
			                textCodeElement.Children.Count > 0 ||
			                textCodeElement is MethodElement || textCodeElement is DelegateElement)
			            {
			                _writer.WriteLine();
			            }
			            _writer.WriteLine();
			        }
			    }
			}
		}

		private void WriteClosingComment(TextCodeElement element)
		{
			if (_configuration.ClosingComments.Enabled)
			{
			    string format = _configuration.ClosingComments.Format;
			    if (!string.IsNullOrEmpty(format))
			    {
			        string formatted = element.ToString(format);
			        _writer.Write(string.Format(CultureInfo.InvariantCulture,
			            " {0}{0} {1}", CSharpSymbol.BeginComment, formatted));
			    }
			}
		}

		private void WriteEndBlock()
		{
			_writer.WriteLine();
			_tabCount--;
			WriteIndented(CSharpSymbol.EndBlock.ToString());
		}

		/// <summary>
		/// Writes a collection of header comment lines
		/// </summary>
		/// <param name="headerComments"></param>
		private void WriteHeaderComments(ReadOnlyCollection<ICommentElement> headerComments)
		{
			foreach (ICommentElement comment in headerComments)
			{
			    comment.Accept(this);
			}
		}

		private void WriteIndented(string text)
		{
			for (int tabIndex = 0; tabIndex < _tabCount; tabIndex++)
			{
			    if (_configuration.Tabs.Style == TabStyle.Tabs)
			    {
			        _writer.Write("\t");
			    }
			    else if (_configuration.Tabs.Style == TabStyle.Spaces)
			    {
			        _writer.Write(new string(' ', _configuration.Tabs.SpacesPerTab));
			    }
			    else
			    {
			        throw new InvalidOperationException(
			            string.Format(Thread.CurrentThread.CurrentCulture,
			            "Unknown tab style {0}.", _configuration.Tabs.Style.ToString()));
			    }
			}

			_writer.Write(text);
		}

		private void WriteIndentedLine(string text)
		{
			if (!string.IsNullOrEmpty(text))
			{
			    WriteIndented(text);
			}
			_writer.WriteLine();
		}

		private void WriteIndentedLine()
		{
			WriteIndentedLine(string.Empty);
		}

		private void WriteMemberAttributes(MemberModifiers memberAttributes)
		{
			if ((memberAttributes & MemberModifiers.Unsafe) == MemberModifiers.Unsafe)
			{
			    _writer.Write(CSharpKeyword.Unsafe);
			    _writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.Constant) == MemberModifiers.Constant)
			{
			    _writer.Write(CSharpKeyword.Constant);
			    _writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.Static) == MemberModifiers.Static)
			{
			    _writer.Write(CSharpKeyword.Static);
			    _writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.Abstract) == MemberModifiers.Abstract)
			{
			    _writer.Write(CSharpKeyword.Abstract);
			    _writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.External) == MemberModifiers.External)
			{
			    _writer.Write(CSharpKeyword.External);
			    _writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.New) == MemberModifiers.New)
			{
			    _writer.Write(CSharpKeyword.New);
			    _writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.Override) == MemberModifiers.Override)
			{
			    _writer.Write(CSharpKeyword.Override);
			    _writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.ReadOnly) == MemberModifiers.ReadOnly)
			{
			    _writer.Write(CSharpKeyword.ReadOnly);
			    _writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.Sealed) == MemberModifiers.Sealed)
			{
			    _writer.Write(CSharpKeyword.Sealed);
			    _writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.Virtual) == MemberModifiers.Virtual)
			{
			    _writer.Write(CSharpKeyword.Virtual);
			    _writer.Write(' ');
			}
		}

		private void WriteParameterList(string paramList)
		{
			_writer.Write(CSharpSymbol.BeginParameterList);
			_tabCount++;

			if (paramList != null)
			{
			    string[] paramLines = paramList.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
			    for (int paramLineIndex = 0; paramLineIndex < paramLines.Length; paramLineIndex++)
			    {
			        string paramLine = paramLines[paramLineIndex];
			        if (paramLineIndex > 0)
			        {
			            _writer.WriteLine();
			            WriteIndented(paramLine.Trim());
			        }
			        else
			        {
			            _writer.Write(paramLine);
			        }
			    }
			}
			_writer.Write(CSharpSymbol.EndParameterList);
			_tabCount--;
		}

		private void WriteTextBlock(string text)
		{
			if (!string.IsNullOrEmpty(text))
			{
			    string[] lines = text.Split(new string[] { Environment.NewLine },
			        StringSplitOptions.None);

			    for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
			    {
			        string line = lines[lineIndex];

			        StringBuilder lineBuilder = new StringBuilder(line);

			        if (lineBuilder.Length > 0)
			        {
			            for (int tabIndex = 0; tabIndex < _tabCount; tabIndex++)
			            {
			                if (lineBuilder.Length > 0 && lineBuilder[0] == '\t')
			                {
			                    lineBuilder.Remove(0, 1);
			                }
			                else
			                {
			                    int spaceCount = 0;
			                    int index = 0;
			                    while (lineBuilder.Length > 0 && index < lineBuilder.Length &&
			                        lineBuilder[index] == ' ' && tabIndex < _tabCount)
			                    {
			                        spaceCount++;
			                        if (spaceCount == _configuration.Tabs.SpacesPerTab)
			                        {
			                            lineBuilder.Remove(0, _configuration.Tabs.SpacesPerTab);
			                            spaceCount = 0;
			                            index = 0;
			                            tabIndex++;
			                        }
			                        else
			                        {
			                            index++;
			                        }
			                    }
			                }
			            }
			        }

			        if (lineIndex < lines.Length - 1)
			        {
			            WriteIndentedLine(lineBuilder.ToString());
			        }
			        else
			        {
			            WriteIndented(lineBuilder.ToString());
			        }
			    }
			}
		}

		private void WriteTypeParameterConstraints(IGenericElement genericElement)
		{
			if (genericElement.TypeParameters.Count > 0)
			{
			    foreach (TypeParameter typeParameter in genericElement.TypeParameters)
			    {
			        if (typeParameter.Constraints.Count > 0)
			        {
			            _writer.WriteLine();
			            WriteIndented("\t");

			            _writer.Write(CSharpKeyword.Where);
			            _writer.Write(' ');
			            _writer.Write(typeParameter.Name);
			            _writer.Write(' ');
			            _writer.Write(CSharpSymbol.TypeImplements);
			            _writer.Write(' ');
			            for (int constraintIndex = 0; constraintIndex < typeParameter.Constraints.Count;
			                constraintIndex++)
			            {
			                string constraint = typeParameter.Constraints[constraintIndex];
			                _writer.Write(constraint);

			                if (constraintIndex < typeParameter.Constraints.Count - 1)
			                {
			                    _writer.Write(CSharpSymbol.AliasSeparator);
			                    _writer.Write(' ');
			                }
			            }
			        }
			    }
			}
		}

		private void WriteTypeParameters(IGenericElement genericElement)
		{
			if (genericElement.TypeParameters.Count > 0)
			{
			    _writer.Write(CSharpSymbol.BeginGeneric);

			    for (int parameterIndex = 0; parameterIndex < genericElement.TypeParameters.Count; parameterIndex++)
			    {
			        TypeParameter typeParameter = genericElement.TypeParameters[parameterIndex];
			        _writer.Write(typeParameter.Name);
			        if (parameterIndex < genericElement.TypeParameters.Count - 1)
			        {
			            _writer.Write(CSharpSymbol.AliasSeparator);
			        }
			    }

			    _writer.Write(CSharpSymbol.EndGeneric);
			}
		}

		#endregion Private Methods

		#region Public Methods

		/// <summary>
		/// Processes an attribute element
		/// </summary>
		/// <param name="element"></param>
		public void VisitAttributeElement(AttributeElement element)
		{
			this.WriteHeaderComments(element.HeaderComments);

			StringBuilder builder = new StringBuilder(DefaultBlockLength);
			builder.Append(CSharpSymbol.BeginAttribute);
			builder.Append(element.BodyText);
			builder.Append(CSharpSymbol.EndAttribute);

			WriteIndentedLine(builder.ToString());
		}

		/// <summary>
		/// Writes a comment line
		/// </summary>
		/// <param name="comment"></param>
		public void VisitCommentElement(CommentElement comment)
		{
			StringBuilder builder = new StringBuilder(DefaultBlockLength);

			if (comment.Type == CommentType.Block)
			{
			    builder.Append("/*");
			    builder.Append(comment.Text);
			    builder.Append("*/");
			}
			else
			{
			    if (comment.Type == CommentType.XmlLine)
			    {
			        builder.Append("///");
			    }
			    else
			    {
			        builder.Append("//");
			    }

			    builder.Append(comment.Text);
			}

			WriteIndentedLine(builder.ToString());
		}

		/// <summary>
		/// Processes a constructor element
		/// </summary>
		/// <param name="element"></param>
		public void VisitConstructorElement(ConstructorElement element)
		{
			this.WriteHeaderComments(element.HeaderComments);
			this.WriteAttributes(element);

			WriteAccess(element.Access);

			WriteMemberAttributes(element.MemberModifiers);

			_writer.Write(element.Name);

			WriteParameterList(element.Parameters);
			_writer.WriteLine();

			if (element.Reference != null)
			{
			    _tabCount++;
			    WriteIndentedLine(string.Format(CultureInfo.InvariantCulture,
			        "{0} {1}",
			        CSharpSymbol.TypeImplements, element.Reference));
			    _tabCount--;
			}

			WriteBody(element);
		}

		/// <summary>
		/// Processes a delegate element
		/// </summary>
		/// <param name="element"></param>
		public void VisitDelegateElement(DelegateElement element)
		{
			this.WriteHeaderComments(element.HeaderComments);
			this.WriteAttributes(element);

			WriteAccess(element.Access);

			WriteMemberAttributes(element.MemberModifiers);

			_writer.Write(CSharpKeyword.Delegate);
			_writer.Write(' ');

			_writer.Write(element.ReturnType);
			_writer.Write(' ');

			_writer.Write(element.Name);

			WriteTypeParameters(element);			
			WriteParameterList(element.Parameters);
			WriteTypeParameterConstraints(element);
			_writer.Write(CSharpSymbol.EndOfStatement);
		}

		/// <summary>
		/// Processes an event element
		/// </summary>
		/// <param name="element"></param>
		public void VisitEventElement(EventElement element)
		{
			this.WriteHeaderComments(element.HeaderComments);
			this.WriteAttributes(element);

			WriteAccess(element.Access);

			WriteMemberAttributes(element.MemberModifiers);

			_writer.Write(CSharpKeyword.Event);
			_writer.Write(' ');

			_writer.Write(element.ReturnType);
			_writer.Write(' ');

			_writer.Write(element.Name);

			if (element.BodyText != null && element.BodyText.Length > 0)
			{
			    _writer.WriteLine();
			    WriteBody(element);
			}
			else
			{
			    _writer.Write(CSharpSymbol.EndOfStatement);
			}
		}

		/// <summary>
		/// Processes a field element
		/// </summary>
		/// <param name="element"></param>
		public void VisitFieldElement(FieldElement element)
		{
			this.WriteHeaderComments(element.HeaderComments);
			this.WriteAttributes(element);

			WriteAccess(element.Access);

			WriteMemberAttributes(element.MemberModifiers);

			if (element.IsVolatile)
			{
			    _writer.Write(CSharpKeyword.Volatile);
			    _writer.Write(' ');
			}

			_writer.Write(element.ReturnType);
			_writer.Write(' ');

			_writer.Write(element.Name);

			if (!string.IsNullOrEmpty(element.InitialValue))
			{
			    _writer.Write(' ');
			    _writer.Write(CSharpSymbol.Assignment);
			    _writer.Write(' ');
			    _writer.Write(element.InitialValue);
			}

			_writer.Write(CSharpSymbol.EndOfStatement);
		}

		/// <summary>
		/// Processes a group element
		/// </summary>
		/// <param name="element"></param>
		public void VisitGroupElement(GroupElement element)
		{
			//
			// Process all children
			//
			for (int childIndex = 0; childIndex < element.Children.Count; childIndex++)
			{
			    ICodeElement childElement = element.Children[childIndex];

			    FieldElement childFieldElement = childElement as FieldElement;
			    if (childIndex > 0 && childFieldElement != null &&
			        childFieldElement.HeaderComments.Count > 0)
			    {
			        WriteIndentedLine();
			    }

			    childElement.Accept(this);

			    if (childIndex < element.Children.Count - 1 &&
			        element.SeparatorType == GroupSeparatorType.Custom)
			    {
			        WriteIndentedLine(element.CustomSeparator);
			    }
			    else
			    {
			        WriteIndentedLine();
			    }
			}

			WriteIndentedLine();
		}

		/// <summary>
		/// Processes a method element
		/// </summary>
		/// <param name="element"></param>
		public void VisitMethodElement(MethodElement element)
		{
			this.WriteHeaderComments(element.HeaderComments);
			this.WriteAttributes(element);

			if (element.IsPartial)
			{
			    _writer.Write(CSharpKeyword.Partial);
			    _writer.Write(' ');
			}

			WriteAccess(element.Access);

			WriteMemberAttributes(element.MemberModifiers);

			if (element.OperatorType == OperatorType.None)
			{
			    _writer.Write(element.ReturnType);
			    _writer.Write(' ');

			    if (element.IsOperator)
			    {
			        _writer.Write(CSharpKeyword.Operator);
			        _writer.Write(' ');
			    }

			    _writer.Write(element.Name);
			}
			else if(element.IsOperator)
			{
			    if (element.OperatorType == OperatorType.Explicit)
			    {
			        _writer.Write(CSharpKeyword.Explicit);
			    }
			    else if (element.OperatorType == OperatorType.Implicit)
			    {
			        _writer.Write(CSharpKeyword.Implicit);
			    }
			    _writer.Write(' ');

			    _writer.Write(CSharpKeyword.Operator);
			    _writer.Write(' ');
			    _writer.Write(element.ReturnType);
			}

			WriteTypeParameters(element);
			WriteParameterList(element.Parameters);
			WriteTypeParameterConstraints(element);

			if (element.BodyText == null)
			{
			    _writer.Write(CSharpSymbol.EndOfStatement);
			}
			else
			{
			    _writer.WriteLine();
			    WriteBody(element);
			}
		}

		/// <summary>
		/// Processes a namespace element
		/// </summary>
		/// <param name="element"></param>
		public void VisitNamespaceElement(NamespaceElement element)
		{
			this.WriteHeaderComments(element.HeaderComments);

			StringBuilder builder = new StringBuilder(DefaultBlockLength);
			builder.Append(CSharpKeyword.Namespace);
			builder.Append(' ');
			builder.Append(element.Name);

			WriteIndentedLine(builder.ToString());
			WriteBeginBlock();

			//
			// Process all children
			//
			WriteChildren(element);

			WriteEndBlock();
		}

		/// <summary>
		/// Processes a property element
		/// </summary>
		/// <param name="element"></param>
		public void VisitPropertyElement(PropertyElement element)
		{
			this.WriteHeaderComments(element.HeaderComments);
			this.WriteAttributes(element);

			WriteAccess(element.Access);

			WriteMemberAttributes(element.MemberModifiers);

			_writer.Write(element.ReturnType);
			_writer.Write(' ');

			_writer.Write(element.Name);
			if (element.IndexParameter != null)
			{
				_writer.Write(CSharpSymbol.BeginAttribute);
				_writer.Write(element.IndexParameter);
				_writer.Write(CSharpSymbol.EndAttribute);
			}

			_writer.WriteLine();

			WriteBody(element);
		}

		/// <summary>
		/// Processes a region element
		/// </summary>
		/// <param name="element"></param>
		public void VisitRegionElement(RegionElement element)
		{
			StringBuilder builder = new StringBuilder(DefaultBlockLength);
			builder.Append(CSharpSymbol.Preprocessor);
			builder.Append(CSharpKeyword.Region);
			builder.Append(' ');
			builder.Append(element.Name);

			WriteIndentedLine(builder.ToString());
			_writer.WriteLine();

			WriteChildren(element);

			if (element.Children.Count > 0 &&
			    !(element.Children.Count == 1 && element.Children[0] is GroupElement))
			{
			    _writer.WriteLine();
			    _writer.WriteLine();
			}

			builder = new StringBuilder(DefaultBlockLength);
			builder.Append(CSharpSymbol.Preprocessor);
			builder.Append(CSharpKeyword.EndRegion);
			builder.Append(' ');
			builder.Append(element.Name);

			WriteIndented(builder.ToString());
		}

		/// <summary>
		/// Processes a type element
		/// </summary>
		/// <param name="element"></param>
		public void VisitTypeElement(TypeElement element)
		{
			this.WriteHeaderComments(element.HeaderComments);
			this.WriteAttributes(element);

			if (element.Access != CodeAccess.None)
			{
			    WriteAccess(element.Access);
			}
			else
			{
			    WriteIndented(string.Empty);
			}

			if (element.IsUnsafe)
			{
			    _writer.Write(CSharpKeyword.Unsafe);
			    _writer.Write(' ');
			}

			if (element.IsStatic)
			{
			    _writer.Write(CSharpKeyword.Static);
			    _writer.Write(' ');
			}

			if (element.IsSealed)
			{
			    _writer.Write(CSharpKeyword.Sealed);
			    _writer.Write(' ');
			}

			if (element.IsAbstract)
			{
			    _writer.Write(CSharpKeyword.Abstract);
			    _writer.Write(' ');
			}

			if (element.IsPartial)
			{
			    _writer.Write(CSharpKeyword.Partial);
			    _writer.Write(' ');
			}

			StringBuilder builder = new StringBuilder(DefaultBlockLength);

			switch (element.TypeElementType)
			{
			    case TypeElementType.Class:
			        builder.Append(CSharpKeyword.Class);
			        break;

			    case TypeElementType.Enum:
			        builder.Append(CSharpKeyword.Enumeration);
			        break;

			    case TypeElementType.Interface:
			        builder.Append(CSharpKeyword.Interface);
			        break;

			    case TypeElementType.Structure:
			        builder.Append(CSharpKeyword.Structure);
			        break;

			    default:
			        throw new ArgumentOutOfRangeException(
			            string.Format(Thread.CurrentThread.CurrentCulture,
			            "Unhandled type element type {0}", element.TypeElementType));
			}

			builder.Append(' ');
			builder.Append(element.Name);

			_writer.Write(builder.ToString());

			WriteTypeParameters(element);

			if (element.Interfaces.Count > 0)
			{
			    builder = new StringBuilder(DefaultBlockLength);
			    builder.Append(' ');
			    builder.Append(CSharpSymbol.TypeImplements);
			    builder.Append(' ');

			    for (int interfaceIndex = 0; interfaceIndex < element.Interfaces.Count; interfaceIndex++)
			    {
			        InterfaceReference interfaceReference = element.Interfaces[interfaceIndex];
			        builder.Append(interfaceReference.Name);

			        if (interfaceIndex < element.Interfaces.Count - 1)
			        {
			            builder.Append(CSharpSymbol.AliasSeparator);
			            builder.Append(' ');
			        }
			    }

			    _writer.Write(builder.ToString());
			}


			WriteTypeParameterConstraints(element);
			_writer.WriteLine();

			if (element.TypeElementType == TypeElementType.Enum)
			{
			    WriteBody(element);
			}
			else
			{
			    WriteBeginBlock();

			    if (element.Children.Count > 0)
			    {
			        WriteChildren(element);

			        WriteEndBlock();
			    }
			    else
			    {
			        _writer.Write(CSharpSymbol.EndBlock);
			    }

			    WriteClosingComment(element);
			}
		}

		/// <summary>
		/// Processes a using element
		/// </summary>
		/// <param name="element"></param>
		public void VisitUsingElement(UsingElement element)
		{
			this.WriteHeaderComments(element.HeaderComments);

			StringBuilder builder = new StringBuilder(DefaultBlockLength);
			builder.Append(CSharpKeyword.Using);
			builder.Append(' ');
			if (!string.IsNullOrEmpty(element.Redefine))
			{
				builder.Append(element.Redefine);
				builder.Append(" " + CSharpSymbol.Assignment.ToString() + " ");
			}
			builder.Append(element.Name);
			builder.Append(CSharpSymbol.EndOfStatement);

			WriteIndented(builder.ToString());
		}

		#endregion Public Methods
	}
}