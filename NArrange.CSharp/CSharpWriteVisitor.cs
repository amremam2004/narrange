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
 *      - Code writer refactoring
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
	internal sealed class CSharpWriteVisitor : CodeWriteVisitor
	{
		#region Constructors

		/// <summary>
		/// Creates a new CSharpWriteVisitor
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="configuration"></param>
		public CSharpWriteVisitor(TextWriter writer, CodeConfiguration configuration)
			: base(writer, configuration)
		{
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
			TabCount++;
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
			    TabCount--;
			    WriteIndented(CSharpSymbol.EndBlock.ToString());
			}
		}

		private void WriteChildren(ICodeElement element)
		{
			CodeWriter.WriteVisitElements(element.Children, Writer, this);
		}

		private void WriteClosingComment(TextCodeElement element)
		{
			if (Configuration.ClosingComments.Enabled)
			{
			    string format = Configuration.ClosingComments.Format;
			    if (!string.IsNullOrEmpty(format))
			    {
			        string formatted = element.ToString(format);
			        Writer.Write(string.Format(CultureInfo.InvariantCulture,
			            " {0}{0} {1}", CSharpSymbol.BeginComment, formatted));
			    }
			}
		}

		private void WriteEndBlock()
		{
			Writer.WriteLine();
			TabCount--;
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

		private void WriteMemberAttributes(MemberModifiers memberAttributes)
		{
			if ((memberAttributes & MemberModifiers.Unsafe) == MemberModifiers.Unsafe)
			{
			    Writer.Write(CSharpKeyword.Unsafe);
			    Writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.Constant) == MemberModifiers.Constant)
			{
			    Writer.Write(CSharpKeyword.Constant);
			    Writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.Static) == MemberModifiers.Static)
			{
			    Writer.Write(CSharpKeyword.Static);
			    Writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.Abstract) == MemberModifiers.Abstract)
			{
			    Writer.Write(CSharpKeyword.Abstract);
			    Writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.External) == MemberModifiers.External)
			{
			    Writer.Write(CSharpKeyword.External);
			    Writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.New) == MemberModifiers.New)
			{
			    Writer.Write(CSharpKeyword.New);
			    Writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.Override) == MemberModifiers.Override)
			{
			    Writer.Write(CSharpKeyword.Override);
			    Writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.ReadOnly) == MemberModifiers.ReadOnly)
			{
			    Writer.Write(CSharpKeyword.ReadOnly);
			    Writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.Sealed) == MemberModifiers.Sealed)
			{
			    Writer.Write(CSharpKeyword.Sealed);
			    Writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.Virtual) == MemberModifiers.Virtual)
			{
			    Writer.Write(CSharpKeyword.Virtual);
			    Writer.Write(' ');
			}
		}

		private void WriteParameterList(string paramList)
		{
			Writer.Write(CSharpSymbol.BeginParameterList);
			TabCount++;

			if (paramList != null)
			{
			    string[] paramLines = paramList.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
			    for (int paramLineIndex = 0; paramLineIndex < paramLines.Length; paramLineIndex++)
			    {
			        string paramLine = paramLines[paramLineIndex];
			        if (paramLineIndex > 0)
			        {
			            Writer.WriteLine();
			            WriteIndented(paramLine.Trim());
			        }
			        else
			        {
			            Writer.Write(paramLine);
			        }
			    }
			}
			Writer.Write(CSharpSymbol.EndParameterList);
			TabCount--;
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
			            for (int tabIndex = 0; tabIndex < TabCount; tabIndex++)
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
			                        lineBuilder[index] == ' ' && tabIndex < TabCount)
			                    {
			                        spaceCount++;
			                        if (spaceCount == Configuration.Tabs.SpacesPerTab)
			                        {
			                            lineBuilder.Remove(0, Configuration.Tabs.SpacesPerTab);
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
			            Writer.WriteLine();
			            WriteIndented("\t");

			            Writer.Write(CSharpKeyword.Where);
			            Writer.Write(' ');
			            Writer.Write(typeParameter.Name);
			            Writer.Write(' ');
			            Writer.Write(CSharpSymbol.TypeImplements);
			            Writer.Write(' ');
			            for (int constraintIndex = 0; constraintIndex < typeParameter.Constraints.Count;
			                constraintIndex++)
			            {
			                string constraint = typeParameter.Constraints[constraintIndex];
			                Writer.Write(constraint);

			                if (constraintIndex < typeParameter.Constraints.Count - 1)
			                {
			                    Writer.Write(CSharpSymbol.AliasSeparator);
			                    Writer.Write(' ');
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
			    Writer.Write(CSharpSymbol.BeginGeneric);

			    for (int parameterIndex = 0; parameterIndex < genericElement.TypeParameters.Count; parameterIndex++)
			    {
			        TypeParameter typeParameter = genericElement.TypeParameters[parameterIndex];
			        Writer.Write(typeParameter.Name);
			        if (parameterIndex < genericElement.TypeParameters.Count - 1)
			        {
			            Writer.Write(CSharpSymbol.AliasSeparator);
			        }
			    }

			    Writer.Write(CSharpSymbol.EndGeneric);
			}
		}

		#endregion Private Methods

		#region Public Methods

		/// <summary>
		/// Processes an attribute element
		/// </summary>
		/// <param name="element"></param>
		public override void VisitAttributeElement(AttributeElement element)
		{
			this.WriteHeaderComments(element.HeaderComments);

			StringBuilder builder = new StringBuilder(DefaultBlockLength);
			builder.Append(CSharpSymbol.BeginAttribute);
			builder.Append(element.BodyText);
			builder.Append(CSharpSymbol.EndAttribute);

			WriteIndented(builder.ToString());
			if (element.Parent != null)
			{
			    Writer.WriteLine();
			}
		}

		/// <summary>
		/// Writes a comment line
		/// </summary>
		/// <param name="comment"></param>
		public override void VisitCommentElement(CommentElement comment)
		{
			StringBuilder builder = new StringBuilder(DefaultBlockLength);

			if (comment.Type == CommentType.Block)
			{
			    builder.Append("/*");
			    builder.Append(comment.Text);
			    builder.Append("*/");

			    WriteTextBlock(builder.ToString());
			    WriteIndentedLine();
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
			    WriteIndentedLine(builder.ToString());
			}
		}

		/// <summary>
		/// Processes a constructor element
		/// </summary>
		/// <param name="element"></param>
		public override void VisitConstructorElement(ConstructorElement element)
		{
			this.WriteHeaderComments(element.HeaderComments);
			this.WriteAttributes(element);

			WriteAccess(element.Access);

			WriteMemberAttributes(element.MemberModifiers);

			Writer.Write(element.Name);

			WriteParameterList(element.Parameters);
			Writer.WriteLine();

			if (element.Reference != null)
			{
			    TabCount++;
			    WriteIndentedLine(string.Format(CultureInfo.InvariantCulture,
			        "{0} {1}",
			        CSharpSymbol.TypeImplements, element.Reference));
			    TabCount--;
			}

			WriteBody(element);
		}

		/// <summary>
		/// Processes a delegate element
		/// </summary>
		/// <param name="element"></param>
		public override void VisitDelegateElement(DelegateElement element)
		{
			this.WriteHeaderComments(element.HeaderComments);
			this.WriteAttributes(element);

			WriteAccess(element.Access);

			WriteMemberAttributes(element.MemberModifiers);

			Writer.Write(CSharpKeyword.Delegate);
			Writer.Write(' ');

			Writer.Write(element.ReturnType);
			Writer.Write(' ');

			Writer.Write(element.Name);

			WriteTypeParameters(element);			
			WriteParameterList(element.Parameters);
			WriteTypeParameterConstraints(element);
			Writer.Write(CSharpSymbol.EndOfStatement);
		}

		/// <summary>
		/// Processes an event element
		/// </summary>
		/// <param name="element"></param>
		public override void VisitEventElement(EventElement element)
		{
			this.WriteHeaderComments(element.HeaderComments);
			this.WriteAttributes(element);

			WriteAccess(element.Access);

			WriteMemberAttributes(element.MemberModifiers);

			Writer.Write(CSharpKeyword.Event);
			Writer.Write(' ');

			Writer.Write(element.ReturnType);
			Writer.Write(' ');

			Writer.Write(element.Name);

			if (element.BodyText != null && element.BodyText.Length > 0)
			{
			    Writer.WriteLine();
			    WriteBody(element);
			}
			else
			{
			    Writer.Write(CSharpSymbol.EndOfStatement);
			}
		}

		/// <summary>
		/// Processes a field element
		/// </summary>
		/// <param name="element"></param>
		public override void VisitFieldElement(FieldElement element)
		{
			this.WriteHeaderComments(element.HeaderComments);
			this.WriteAttributes(element);

			WriteAccess(element.Access);

			WriteMemberAttributes(element.MemberModifiers);

			if (element.IsVolatile)
			{
			    Writer.Write(CSharpKeyword.Volatile);
			    Writer.Write(' ');
			}

			Writer.Write(element.ReturnType);
			Writer.Write(' ');

			Writer.Write(element.Name);

			if (!string.IsNullOrEmpty(element.InitialValue))
			{
			    Writer.Write(' ');
			    Writer.Write(CSharpSymbol.Assignment);
			    Writer.Write(' ');
			    if (element.InitialValue.IndexOf("\n") >= 0)
			    {
			        WriteTextBlock(element.InitialValue);
			    }
			    else
			    {
			        Writer.Write(element.InitialValue);
			    }
			}

			Writer.Write(CSharpSymbol.EndOfStatement);
		}

		/// <summary>
		/// Processes a method element
		/// </summary>
		/// <param name="element"></param>
		public override void VisitMethodElement(MethodElement element)
		{
			this.WriteHeaderComments(element.HeaderComments);
			this.WriteAttributes(element);

			if (element.IsPartial)
			{
			    Writer.Write(CSharpKeyword.Partial);
			    Writer.Write(' ');
			}

			WriteAccess(element.Access);

			WriteMemberAttributes(element.MemberModifiers);

			if (element.OperatorType == OperatorType.None)
			{
			    Writer.Write(element.ReturnType);
			    Writer.Write(' ');

			    if (element.IsOperator)
			    {
			        Writer.Write(CSharpKeyword.Operator);
			        Writer.Write(' ');
			    }

			    Writer.Write(element.Name);
			}
			else if(element.IsOperator)
			{
			    if (element.OperatorType == OperatorType.Explicit)
			    {
			        Writer.Write(CSharpKeyword.Explicit);
			    }
			    else if (element.OperatorType == OperatorType.Implicit)
			    {
			        Writer.Write(CSharpKeyword.Implicit);
			    }
			    Writer.Write(' ');

			    Writer.Write(CSharpKeyword.Operator);
			    Writer.Write(' ');
			    Writer.Write(element.ReturnType);
			}

			WriteTypeParameters(element);
			WriteParameterList(element.Parameters);
			WriteTypeParameterConstraints(element);

			if (element.BodyText == null)
			{
			    Writer.Write(CSharpSymbol.EndOfStatement);
			}
			else
			{
			    Writer.WriteLine();
			    WriteBody(element);
			}
		}

		/// <summary>
		/// Processes a namespace element
		/// </summary>
		/// <param name="element"></param>
		public override void VisitNamespaceElement(NamespaceElement element)
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
		public override void VisitPropertyElement(PropertyElement element)
		{
			this.WriteHeaderComments(element.HeaderComments);
			this.WriteAttributes(element);

			WriteAccess(element.Access);

			WriteMemberAttributes(element.MemberModifiers);

			Writer.Write(element.ReturnType);
			Writer.Write(' ');

			Writer.Write(element.Name);
			if (element.IndexParameter != null)
			{
				Writer.Write(CSharpSymbol.BeginAttribute);
				Writer.Write(element.IndexParameter);
				Writer.Write(CSharpSymbol.EndAttribute);
			}

			Writer.WriteLine();

			WriteBody(element);
		}

		/// <summary>
		/// Processes a region element
		/// </summary>
		/// <param name="element"></param>
		public override void VisitRegionElement(RegionElement element)
		{
			StringBuilder builder = new StringBuilder(DefaultBlockLength);
			builder.Append(CSharpSymbol.Preprocessor);
			builder.Append(CSharpKeyword.Region);
			builder.Append(' ');
			builder.Append(element.Name);

			WriteIndentedLine(builder.ToString());
			Writer.WriteLine();

			WriteChildren(element);

			if (element.Children.Count > 0)
			{
			    Writer.WriteLine();
			    Writer.WriteLine();
			}

			builder = new StringBuilder(DefaultBlockLength);
			builder.Append(CSharpSymbol.Preprocessor);
			builder.Append(CSharpKeyword.EndRegion);
			builder.Append(' ');
			builder.Append(element.Name);

			WriteIndented(builder.ToString());
			if (element.Parent == null)
			{
			    Writer.WriteLine();
			    Writer.WriteLine();
			}
		}

		/// <summary>
		/// Processes a type element
		/// </summary>
		/// <param name="element"></param>
		public override void VisitTypeElement(TypeElement element)
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
			    Writer.Write(CSharpKeyword.Unsafe);
			    Writer.Write(' ');
			}

			if (element.IsStatic)
			{
			    Writer.Write(CSharpKeyword.Static);
			    Writer.Write(' ');
			}

			if (element.IsSealed)
			{
			    Writer.Write(CSharpKeyword.Sealed);
			    Writer.Write(' ');
			}

			if (element.IsAbstract)
			{
			    Writer.Write(CSharpKeyword.Abstract);
			    Writer.Write(' ');
			}

			if (element.IsPartial)
			{
			    Writer.Write(CSharpKeyword.Partial);
			    Writer.Write(' ');
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

			Writer.Write(builder.ToString());

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

			    Writer.Write(builder.ToString());
			}


			WriteTypeParameterConstraints(element);
			Writer.WriteLine();

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
			        TabCount--;
			        WriteIndented(CSharpSymbol.EndBlock.ToString());
			    }

			    WriteClosingComment(element);
			}
		}

		/// <summary>
		/// Processes a using element
		/// </summary>
		/// <param name="element"></param>
		public override void VisitUsingElement(UsingElement element)
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