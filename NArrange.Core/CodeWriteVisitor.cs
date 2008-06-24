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
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using NArrange.Core.CodeElements;
using NArrange.Core.Configuration;

namespace NArrange.Core
{
	/// <summary>
	/// Abstract code write visitor implementation.
	/// </summary>
	public abstract class CodeWriteVisitor : ICodeElementVisitor
	{
		#region Constants

		/// <summary>
		/// Default element block length.
		/// </summary>
		protected const int DefaultBlockLength = 256;

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
		protected CodeWriteVisitor(TextWriter writer, CodeConfiguration configuration)
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

		#region Protected Properties

		/// <summary>
		/// Gets the code configuration.
		/// </summary>
		protected CodeConfiguration Configuration
		{
			get
			{
			    return _configuration;
			}
		}

		/// <summary>
		/// Gets or sets the current tab count used for writing indented text.
		/// </summary>
		protected int TabCount
		{
			get
			{
			    return _tabCount;
			}
			set
			{
			    _tabCount = value;
			}
		}

		/// <summary>
		/// Gets the writer.
		/// </summary>
		protected TextWriter Writer
		{
			get
			{
			    return _writer;
			}
		}

		#endregion Protected Properties

		#region Protected Methods

		/// <summary>
		/// Writes a closing comment.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="commentPrefix">Comment prefix.</param>
		protected void WriteClosingComment(TextCodeElement element, string commentPrefix)
		{
			if (Configuration.ClosingComments.Enabled)
			{
			    string format = Configuration.ClosingComments.Format;
			    if (!string.IsNullOrEmpty(format))
			    {
			        string formatted = element.ToString(format);
			        Writer.Write(string.Format(CultureInfo.InvariantCulture,
			            " {0}{1}", commentPrefix, formatted));
			    }
			}
		}

		/// <summary>
		/// Writes a collection of comment lines.
		/// </summary>
		/// <param name="comments"></param>
		protected void WriteComments(ReadOnlyCollection<ICommentElement> comments)
		{
			foreach (ICommentElement comment in comments)
			{
			    comment.Accept(this);
			    WriteIndentedLine();
			}
		}

		/// <summary>
		/// Writes the specified text using the current TabCount.
		/// </summary>
		/// <param name="text"></param>
		protected void WriteIndented(string text)
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

		/// <summary>
		/// Writes a line of text using the current TabCount.
		/// </summary>
		/// <param name="text"></param>
		protected void WriteIndentedLine(string text)
		{
			if (!string.IsNullOrEmpty(text))
			{
			    WriteIndented(text);
			}
			_writer.WriteLine();
		}

		/// <summary>
		/// Writes a new line using the current TabCount.
		/// </summary>
		protected void WriteIndentedLine()
		{
			WriteIndentedLine(string.Empty);
		}

		#endregion Protected Methods

		#region Public Methods

		/// <summary>
		/// Processes an attribute element.
		/// </summary>
		/// <param name="element"></param>
		public abstract void VisitAttributeElement(AttributeElement element);

		/// <summary>
		/// Processes a comment element.
		/// </summary>
		/// <param name="element"></param>
		public abstract void VisitCommentElement(CommentElement element);

		/// <summary>
		/// Processes a constructor element.
		/// </summary>
		/// <param name="element"></param>
		public abstract void VisitConstructorElement(ConstructorElement element);

		/// <summary>
		/// Processes a delegate element.
		/// </summary>
		/// <param name="element"></param>
		public abstract void VisitDelegateElement(DelegateElement element);

		/// <summary>
		/// Processes a event element.
		/// </summary>
		/// <param name="element"></param>
		public abstract void VisitEventElement(EventElement element);

		/// <summary>
		/// Processes a field element.
		/// </summary>
		/// <param name="element"></param>
		public abstract void VisitFieldElement(FieldElement element);

		/// <summary>
		/// Processes a group element
		/// </summary>
		/// <param name="element"></param>
		public virtual void VisitGroupElement(GroupElement element)
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

			    if (childIndex < element.Children.Count - 1)
			    {
			        if (element.SeparatorType == GroupSeparatorType.Custom)
			        {
			            WriteIndentedLine(element.CustomSeparator);
			        }
			        else
			        {
			            WriteIndentedLine();
			        }
			    }
			}
		}

		/// <summary>
		/// Processes a method element.
		/// </summary>
		/// <param name="element"></param>
		public abstract void VisitMethodElement(MethodElement element);

		/// <summary>
		/// Processes a namespace element.
		/// </summary>
		/// <param name="element"></param>
		public abstract void VisitNamespaceElement(NamespaceElement element);

		/// <summary>
		/// Processes a property element.
		/// </summary>
		/// <param name="element"></param>
		public abstract void VisitPropertyElement(PropertyElement element);

		/// <summary>
		/// Processes a region element.
		/// </summary>
		/// <param name="element"></param>
		public abstract void VisitRegionElement(RegionElement element);

		/// <summary>
		/// Processes a type element.
		/// </summary>
		/// <param name="element"></param>
		public abstract void VisitTypeElement(TypeElement element);

		/// <summary>
		/// Processes a using element.
		/// </summary>
		/// <param name="element"></param>
		public abstract void VisitUsingElement(UsingElement element);

		#endregion Public Methods
	}
}