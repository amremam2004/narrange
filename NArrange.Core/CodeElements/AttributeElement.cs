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
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
using System;
using System.Collections.Generic;
using System.Text;

namespace NArrange.Core.CodeElements
{
	/// <summary>
	/// Attribute code element
	/// </summary>
	public class AttributeElement : TextCodeElement, IAttribute	
	{
		#region Constructors
		
		/// <summary>
		/// Creates a new attribute element
		/// </summary>
		public AttributeElement()		
		{
		}		
		
		/// <summary>
		/// Creates a new attribute element with the specified attribute text
		/// </summary>
		/// <param name="bodyText"></param>
		public AttributeElement(string bodyText)		
		{
			this.BodyText = bodyText;
		}		
		
		#endregion Constructors
		
		#region Protected Methods
		
		/// <summary>
		/// Clones this instance
		/// </summary>
		/// <returns></returns>
		protected override CodeElement DoClone()		
		{
			return new AttributeElement();
		}		
		
		#endregion Protected Methods
		
		#region Public Properties
		
		/// <summary>
		/// Gets the element type
		/// </summary>
		public override ElementType ElementType		
		{
			get
			{
			    return ElementType.Attribute;
			}
		}		
		
		#endregion Public Properties
		
		#region Public Methods
		
		/// <summary>
		/// Allows an ICodeElementVisitor to process (or visit) this element.
		/// </summary>
		/// <remarks>See the Gang of Four Visitor design pattern.</remarks>
		/// <param name="visitor"></param>
		public override void Accept(ICodeElementVisitor visitor)		
		{
			visitor.VisitAttributeElement(this);
		}		
		
		#endregion Public Methods

	}
}