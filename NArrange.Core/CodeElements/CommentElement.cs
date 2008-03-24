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
using System.Text;

namespace NArrange.Core.CodeElements
{
	/// <summary>
	/// Comment line implementation
	/// </summary>
	public sealed class CommentElement : CodeElement, ICommentElement
	{
		#region Fields

		private readonly CommentType _commentType;		
		private readonly string _text;		
		
		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new comment line
		/// </summary>
		/// <param name="text">Comment text</param>
		public CommentElement(string text)
		{
			_text = text;
		}

		/// <summary>
		/// Creates a new comment line
		/// </summary>
		/// <param name="text">Comment text</param>
		/// <param name="commentType">Whether or not this is an XML comment or block comment</param>
		public CommentElement(string text, CommentType commentType)
			: this(text)
		{
			_commentType = commentType;
		}

		#endregion Constructors

		#region Public Properties

		/// <summary>
		/// Gets the code element type
		/// </summary>
		public override ElementType ElementType
		{
			get
			{
			    return ElementType.CommentLine;
			}
		}

		/// <summary>
		/// Gets the comment text.
		/// </summary>
		public string Text
		{
			get
			{
			    return _text;
			}
		}

		/// <summary>
		/// Gets the type of the comment
		/// </summary>
		public CommentType Type
		{
			get
			{
			    return _commentType;
			}
		}

		#endregion Public Properties

		#region Protected Methods

		/// <summary>
		/// Creates a clone of this instance
		/// </summary>
		/// <returns></returns>
		protected override CodeElement DoClone()
		{
			CommentElement clone = new CommentElement(_text, _commentType);
			
			return clone;
		}

		#endregion Protected Methods

		#region Public Methods

		/// <summary>
		/// Allows an ICodeElementVisitor to process (or visit) this element.
		/// </summary>
		/// <remarks>See the Gang of Four Visitor design pattern.</remarks>
		/// <param name="visitor"></param>
		public override void Accept(ICodeElementVisitor visitor)
		{
			visitor.VisitCommentElement(this);
		}

		/// <summary>
		/// Gets the string representation of this object.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _text;
		}

		#endregion Public Methods
	}
}