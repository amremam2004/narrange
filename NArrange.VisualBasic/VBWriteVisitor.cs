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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

using NArrange.Core;
using NArrange.Core.CodeElements;
using NArrange.Core.Configuration;

namespace NArrange.VisualBasic
{
	/// <summary>
	/// Visits a tree of code elements for writing VB code. 
	/// </summary>
	internal sealed class VBWriteVisitor : ICodeElementVisitor
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
		/// Creates a new VBWriteVisitor
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="configuration"></param>
		public VBWriteVisitor(TextWriter writer, CodeConfiguration configuration)
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

		private static TypeElement GetTypeParent(ICodeElement element)
		{
			TypeElement parentTypeElement = element.Parent as TypeElement;

			if (parentTypeElement == null &&
			    (element.Parent is GroupElement || element.Parent is RegionElement))
			{
			    parentTypeElement = GetTypeParent(element.Parent);
			}

			return parentTypeElement;
		}

		private void WriteAccess(CodeAccess codeAccess)
		{
			string accessString = string.Empty;
			if (codeAccess != CodeAccess.None)
			{
			    accessString = EnumUtilities.ToString(codeAccess).Replace(",", string.Empty) + " ";
			    accessString = accessString.Replace(
			        EnumUtilities.ToString(CodeAccess.Internal), VBKeyword.Friend);
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
			_tabCount++;
		}

		private void WriteBody(TextCodeElement element)
		{
			MemberElement memberElement = element as MemberElement;
			TypeElement parentTypeElement = GetTypeParent(element);

			bool isAbstract = memberElement != null &&
			    (memberElement.MemberModifiers & MemberModifiers.Abstract) == MemberModifiers.Abstract;
			bool inInterface = memberElement != null &&
			    parentTypeElement != null && parentTypeElement.TypeElementType == TypeElementType.Interface;

			if (!(isAbstract || inInterface))
			{

			    WriteBeginBlock();

			    if (element.BodyText != null && element.BodyText.Trim().Length > 0)
			    {
			        _writer.WriteLine();
			        WriteTextBlock(element.BodyText);
			        WriteEndBlock(element);
			        WriteClosingComment(element);
			    }
			    else
			    {
			        WriteEndBlock(element);
			    }
			}
		}

		private void WriteChildren(ICodeElement element)
		{
			if (element.Children.Count > 0)
			{
			    _writer.WriteLine();
			}

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
			        !(textCodeElement is MethodElement || textCodeElement is DelegateElement ||
			         textCodeElement is PropertyElement || textCodeElement is EventElement))
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
			                textCodeElement is MethodElement || textCodeElement is DelegateElement || 
			                textCodeElement is TypeElement || textCodeElement is PropertyElement ||
			                textCodeElement is EventElement)
			            {
			                _writer.WriteLine();
			            }
			            _writer.WriteLine();
			        }
			    }
			}

			if (element.Children.Count > 0)
			{
			    _writer.WriteLine();
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
			            " {0}{1}", VBSymbol.BeginComment, formatted));
			    }
			}
		}

		private void WriteEndBlock(CodeElement codeElement)
		{
			_writer.WriteLine();
			_tabCount--;

			MemberElement memberElement = codeElement as MemberElement;
			string blockName = string.Empty;
			if (memberElement != null)
			{
			    if (memberElement.ElementType == ElementType.Method ||
			        memberElement.ElementType == ElementType.Constructor)
			    {
			        MethodElement methodElement = memberElement as MethodElement;
			        if (methodElement != null && methodElement.IsOperator)
			        {
			            blockName = VBKeyword.Operator;
			        }
			        else if (!string.IsNullOrEmpty(memberElement.ReturnType))
			        {
			            blockName = VBKeyword.Function;
			        }
			        else
			        {
			            blockName = VBKeyword.Sub;
			        }
			    }
			}

			if (string.IsNullOrEmpty(blockName))
			{
			    TypeElement typeElement = codeElement as TypeElement;
			    if (typeElement != null)
			    {
			        blockName = EnumUtilities.ToString(typeElement.TypeElementType);
			    }

			    if (string.IsNullOrEmpty(blockName))
			    {
			        blockName = EnumUtilities.ToString(codeElement.ElementType);
			    }
			}

			WriteIndented(VBKeyword.End + ' ' + blockName);
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

		private void WriteImplements(ReadOnlyCollection<InterfaceReference> interfaceReferences)
		{
			if (interfaceReferences.Count > 0)
			{
			    _writer.Write(' ');
			    _writer.Write(VBKeyword.Implements);

			    for(int index = 0; index < interfaceReferences.Count; index++)
			    {
			        InterfaceReference interfaceReference = interfaceReferences[index];

			        _writer.Write(' ');
			        _writer.Write(interfaceReference.Name);

			        if (interfaceReferences.Count > 1 && index < interfaceReferences.Count - 1)
			        {
			            _writer.Write(VBSymbol.AliasSeparator);
			        }
			    }
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

		private void WriteMemberAttributes(MemberModifiers memberAttributes, bool overloads)
		{
			if ((memberAttributes & MemberModifiers.Constant) == MemberModifiers.Constant)
			{
			    _writer.Write(VBKeyword.Constant);
			    _writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.Static) == MemberModifiers.Static)
			{
			    _writer.Write(VBKeyword.Shared);
			    _writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.Abstract) == MemberModifiers.Abstract)
			{
			    _writer.Write(VBKeyword.MustOverride);
			    _writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.New) == MemberModifiers.New)
			{
			    _writer.Write(VBKeyword.Shadows);
			    _writer.Write(' ');
			}

			if (overloads)
			{
			    _writer.Write(VBKeyword.Overloads);
			    _writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.Override) == MemberModifiers.Override)
			{
			    _writer.Write(VBKeyword.Overrides);
			    _writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.ReadOnly) == MemberModifiers.ReadOnly)
			{
			    _writer.Write(VBKeyword.ReadOnly);
			    _writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.Sealed) == MemberModifiers.Sealed)
			{
			    _writer.Write(VBKeyword.NotOverridable);
			    _writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.Virtual) == MemberModifiers.Virtual)
			{
			    _writer.Write(VBKeyword.Overridable);
			    _writer.Write(' ');
			}
		}

		private void WriteMethodType(string returnType)
		{
			if (string.IsNullOrEmpty(returnType))
			{
			    _writer.Write(VBKeyword.Sub);
			}
			else
			{
			    _writer.Write(VBKeyword.Function);
			}
			_writer.Write(' ');
		}

		private void WriteParameterList(string paramList)
		{
			_writer.Write(VBSymbol.BeginParameterList);
			_tabCount++;

			if (paramList != null)
			{
			    if (paramList.Length > 0 && paramList[0] == VBSymbol.LineContinuation)
			    {
			        _writer.Write(' ');
			    }

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

			_writer.Write(VBSymbol.EndParameterList);
			_tabCount--;
		}

		private void WriteReturnType(string returnType)
		{
			if (!string.IsNullOrEmpty(returnType))
			{
			    _writer.Write(' ');
			    _writer.Write(VBKeyword.As);
			    _writer.Write(' ');
			    _writer.Write(returnType);
			}
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

		private void WriteTypeParameterConstraints(TypeParameter typeParameter)
		{
			if (typeParameter.Constraints.Count > 0)
			{
			    _writer.Write(' ');
			    _writer.Write(VBKeyword.As);
			    _writer.Write(' ');

			    if (typeParameter.Constraints.Count > 1)
			    {
			        _writer.Write(VBSymbol.BeginTypeConstraintList);
			    }

			    for (int constraintIndex = 0; constraintIndex < typeParameter.Constraints.Count;
			        constraintIndex++)
			    {
			        string constraint = typeParameter.Constraints[constraintIndex];
			        _writer.Write(constraint);

			        if (constraintIndex < typeParameter.Constraints.Count - 1)
			        {
			            _writer.Write(VBSymbol.AliasSeparator);
			            _writer.Write(' ');
			        }
			    }

			    if (typeParameter.Constraints.Count > 1)
			    {
			        _writer.Write(VBSymbol.EndTypeConstraintList);
			    }
			}
		}

		private void WriteTypeParameters(IGenericElement genericElement)
		{
			if (genericElement.TypeParameters.Count > 0)
			{
			    _writer.Write(VBSymbol.BeginParameterList);

			    _writer.Write(VBKeyword.Of);
			    _writer.Write(' ');

			    for (int parameterIndex = 0; parameterIndex < genericElement.TypeParameters.Count; parameterIndex++)
			    {
			        TypeParameter typeParameter = genericElement.TypeParameters[parameterIndex];
			        _writer.Write(typeParameter.Name);

			        WriteTypeParameterConstraints(typeParameter);

			        if (parameterIndex < genericElement.TypeParameters.Count - 1)
			        {
			            _writer.Write(VBSymbol.AliasSeparator);
			        }
			    }

			    _writer.Write(VBSymbol.EndParameterList);
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

			// HACK: Create an explicit element type for Option (or compiler directive)
			if (element[VBExtendedProperties.Option] != null &&
			    (bool)element[VBExtendedProperties.Option])
			{
			    builder.Append(element.BodyText);
			}
			else
			{
			    builder.Append(VBSymbol.BeginAttribute);
			    builder.Append(element.BodyText);
			    builder.Append(VBSymbol.EndAttribute);

			    if (element.Parent is TextCodeElement)
			    {
			        builder.Append(" _");
			    }
			}

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
			    throw new InvalidOperationException("Block comments are not supported by VB.");
			}
			else
			{
			    if (comment.Type == CommentType.XmlLine)
			    {
			        builder.Append("'''");
			    }
			    else
			    {
			        builder.Append("'");
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

			WriteMemberAttributes(element.MemberModifiers,
			    element[VBExtendedProperties.Overloads] != null && 
			    (bool)element[VBExtendedProperties.Overloads]);

			_writer.Write(VBKeyword.Sub);
			_writer.Write(' ');

			_writer.Write(element.Name);

			WriteParameterList(element.Parameters);

			if (!string.IsNullOrEmpty(element.Reference))
			{
			    _tabCount++;
			    _writer.WriteLine();
			    WriteIndented(element.Reference);
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

			WriteMemberAttributes(element.MemberModifiers,
			    element[VBExtendedProperties.Overloads] != null &&
			    (bool)element[VBExtendedProperties.Overloads]);

			_writer.Write(VBKeyword.Delegate);
			_writer.Write(' ');

			WriteMethodType(element.ReturnType);

			_writer.Write(element.Name);

			WriteTypeParameters(element);			
			WriteParameterList(element.Parameters);

			WriteReturnType(element.ReturnType);
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

			WriteMemberAttributes(element.MemberModifiers,
			    element[VBExtendedProperties.Overloads] != null &&
			    (bool)element[VBExtendedProperties.Overloads]);

			bool isCustom = false;
			if (element.BodyText != null && element.BodyText.Trim().Length > 0)
			{
			    isCustom = true;
			    _writer.Write(VBKeyword.Custom);
			    _writer.Write(' ');
			}

			_writer.Write(VBKeyword.Event);
			_writer.Write(' ');

			_writer.Write(element.Name);

			if (element.Parameters != null)
			{
			    WriteParameterList(element.Parameters);
			}
			WriteReturnType(element.ReturnType);

			if (isCustom)
			{
			    WriteBody(element);
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

			WriteMemberAttributes(element.MemberModifiers,
			    element[VBExtendedProperties.Overloads] != null &&
			    (bool)element[VBExtendedProperties.Overloads]);

			if (element[VBExtendedProperties.Dim] != null &&
			    (bool)element[VBExtendedProperties.Dim])
			{
			    _writer.Write(' ');
			    _writer.Write(VBKeyword.Dim);
			    _writer.Write(' ');
			}

			if (element[VBExtendedProperties.WithEvents] != null &&
			    (bool)element[VBExtendedProperties.WithEvents])
			{
			    _writer.Write(' ');
			    _writer.Write(VBKeyword.WithEvents);
			    _writer.Write(' ');
			}

			_writer.Write(element.Name);

			WriteReturnType(element.ReturnType);

			if (!string.IsNullOrEmpty(element.InitialValue))
			{
			    _writer.Write(' ');
			    _writer.Write(VBSymbol.Assignment);
			    _writer.Write(' ');
			    _writer.Write(element.InitialValue);
			}
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

			if (!(element.Parent is RegionElement))
			{
			    WriteIndentedLine();
			}
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
			    _writer.Write(VBKeyword.Partial);
			    _writer.Write(' ');
			}

			WriteAccess(element.Access);

			if (element.IsExternal)
			{
			    _writer.Write(VBKeyword.Declare);
			    _writer.Write(' ');
			}

			if (element[VBExtendedProperties.ExternalModifier] != null)
			{
			    _writer.Write(element[VBExtendedProperties.ExternalModifier].ToString());
			    _writer.Write(' ');
			}

			WriteMemberAttributes(element.MemberModifiers,
			    element[VBExtendedProperties.Overloads] != null &&
			    (bool)element[VBExtendedProperties.Overloads]);

			if (element.IsOperator)
			{
			    if (element.OperatorType == OperatorType.Explicit)
			    {
			        _writer.Write(VBKeyword.Narrowing);
			        _writer.Write(' ');
			    }
			    else if (element.OperatorType == OperatorType.Implicit)
			    {
			        _writer.Write(VBKeyword.Widening);
			        _writer.Write(' ');
			    }

			    _writer.Write(VBKeyword.Operator);
			    _writer.Write(' ');
			}
			else
			{
			    WriteMethodType(element.ReturnType);
			}

			_writer.Write(element.Name);

			WriteTypeParameters(element);

			if (element[VBExtendedProperties.ExternalLibrary] != null)
			{
			    _writer.Write(' ');
			    _writer.Write(VBKeyword.Lib);
			    _writer.Write(' ');

			    _writer.Write(VBSymbol.BeginString);
			    _writer.Write(element[VBExtendedProperties.ExternalLibrary].ToString());
			    _writer.Write(VBSymbol.BeginString);
			    _writer.Write(' ');
			}

			if (element[VBExtendedProperties.ExternalAlias] != null)
			{
			    _writer.Write(VBKeyword.Alias);
			    _writer.Write(' ');

			    _writer.Write(VBSymbol.BeginString);
			    _writer.Write(element[VBExtendedProperties.ExternalAlias].ToString());
			    _writer.Write(VBSymbol.BeginString);
			    _writer.Write(' ');
			}

			WriteParameterList(element.Parameters);

			WriteReturnType(element.ReturnType);
			WriteImplements(element.Implements);

			string[] handles = element[VBExtendedProperties.Handles] as string[];
			if (handles != null && handles.Length > 0)
			{
			    _writer.Write(' ');
			    _writer.Write(VBKeyword.Handles);
			    foreach (string handleReference in handles)
			    {
			        _writer.Write(' ');
			        _writer.Write(handleReference);
			    }
			}

			if (!element.IsExternal)
			{
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
			builder.Append(VBKeyword.Namespace);
			builder.Append(' ');
			builder.Append(element.Name);

			WriteIndentedLine(builder.ToString());
			WriteBeginBlock();

			//
			// Process all children
			//
			WriteChildren(element);

			WriteEndBlock(element);
			_writer.WriteLine();
			_writer.WriteLine();
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

			WriteMemberAttributes(element.MemberModifiers,
			    element[VBExtendedProperties.Overloads] != null &&
			    (bool)element[VBExtendedProperties.Overloads]);

			if (element[VBExtendedProperties.Default] != null && 
			    (bool)element[VBExtendedProperties.Default])
			{
			    _writer.Write(VBKeyword.Default);
			    _writer.Write(' ');
			}

			if (element[VBExtendedProperties.AccessModifier] != null &&
			    element[VBExtendedProperties.AccessModifier].ToString() != VBKeyword.ReadOnly)
			{
			    _writer.Write(element[VBExtendedProperties.AccessModifier]);
			    _writer.Write(' ');
			}

			_writer.Write(VBKeyword.Property);
			_writer.Write(' ');

			_writer.Write(element.Name);
			_writer.Write(VBSymbol.BeginParameterList);
			if (element.IndexParameter != null)
			{
				_writer.Write(element.IndexParameter.Trim());
			}
			_writer.Write(VBSymbol.EndParameterList);

			WriteReturnType(element.ReturnType);
			WriteImplements(element.Implements);

			WriteBody(element);
		}

		/// <summary>
		/// Processes a region element
		/// </summary>
		/// <param name="element"></param>
		public void VisitRegionElement(RegionElement element)
		{
			StringBuilder builder = new StringBuilder(DefaultBlockLength);
			builder.Append(VBSymbol.Preprocessor);
			builder.Append(VBKeyword.Region);
			builder.Append(" \"");
			builder.Append(element.Name);
			builder.Append('"');

			WriteIndentedLine(builder.ToString());

			WriteChildren(element);

			if (element.Children.Count > 0 &&
			    !(element.Children.Count == 1 && element.Children[0] is GroupElement))
			{
			    _writer.WriteLine();
			}

			builder = new StringBuilder(DefaultBlockLength);
			builder.Append(VBSymbol.Preprocessor);
			builder.Append(VBKeyword.End);
			builder.Append(' ');
			builder.Append(VBKeyword.Region);
			builder.Append(" '");
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

			if (element.IsSealed)
			{
			    _writer.Write(VBKeyword.NotInheritable);
			    _writer.Write(' ');
			}

			if (element.IsAbstract)
			{
			    _writer.Write(VBKeyword.MustInherit);
			    _writer.Write(' ');
			}

			if (element.IsPartial)
			{
			    _writer.Write(VBKeyword.Partial);
			    _writer.Write(' ');
			}

			StringBuilder builder = new StringBuilder(DefaultBlockLength);

			switch (element.TypeElementType)
			{
			    case TypeElementType.Class:
			        builder.Append(VBKeyword.Class);
			        break;

			    case TypeElementType.Enum:
			        builder.Append(VBKeyword.Enumeration);
			        break;

			    case TypeElementType.Interface:
			        builder.Append(VBKeyword.Interface);
			        break;

			    case TypeElementType.Structure:
			        builder.Append(VBKeyword.Structure);
			        break;

			    case TypeElementType.Module:
			        builder.Append(VBKeyword.Module);
			        break;

			    default:
			        throw new ArgumentOutOfRangeException(
			            string.Format(Thread.CurrentThread.CurrentCulture,
			            "Unrecognized type element type {0}", element.TypeElementType));
			}

			builder.Append(' ');
			builder.Append(element.Name);

			_writer.Write(builder.ToString());

			WriteTypeParameters(element);

			if (element.Interfaces.Count > 0)
			{
			    if (element.TypeElementType == TypeElementType.Enum)
			    {
			        _writer.Write(' ');
			        _writer.Write(VBKeyword.As);
			        _writer.Write(' ');
			        _writer.Write(element.Interfaces[0].Name);
			    }
			    else
			    {
			        _tabCount++;
			        _writer.WriteLine();

			        for (int interfaceIndex = 0; interfaceIndex < element.Interfaces.Count; interfaceIndex++)
			        {
			            InterfaceReference interfaceReference = element.Interfaces[interfaceIndex];

			            builder = new StringBuilder();
			            if (interfaceReference.ReferenceType == InterfaceReferenceType.Class)
			            {
			                builder.Append(VBKeyword.Inherits);
			            }
			            else
			            {
			                builder.Append(VBKeyword.Implements);
			            }
			            builder.Append(' ');
			            builder.Append(interfaceReference);
			            WriteIndented(builder.ToString());
			            if (interfaceIndex < element.Interfaces.Count - 1)
			            {
			                _writer.WriteLine();
			            }
			        }

			        _tabCount--;
			    }
			}

			if (element.TypeElementType == TypeElementType.Enum)
			{
			    WriteBody(element);
			}
			else
			{
			    WriteBeginBlock();

			    if (element.Children.Count > 0)
			    {
			        _writer.WriteLine();
			        WriteChildren(element);
			    }
			    WriteEndBlock(element);

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
			builder.Append(VBKeyword.Imports);
			builder.Append(' ');
			if (!string.IsNullOrEmpty(element.Redefine))
			{
				builder.Append(element.Redefine);
				builder.Append(" " + VBSymbol.Assignment.ToString() + " ");
			}
			builder.Append(element.Name);

			WriteIndented(builder.ToString());
		}

		#endregion Public Methods
	}
}