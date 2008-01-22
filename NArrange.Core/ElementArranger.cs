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

using NArrange.Core.CodeElements;

namespace NArrange.Core
{
	/// <summary>
	/// Standard IElementArranger implementation
	/// </summary>
	public class ElementArranger : IElementArranger
	{
		#region Fields

		private IElementArranger _childrenArranger;		
		private ElementType _elementType;		
		private IElementFilter _filter;		
		private IElementInserter _inserter;		
		
		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new ElementArranger
		/// </summary>
		/// <param name="elementType"></param>
		/// <param name="inserter"></param>
		/// <param name="filter"></param>
		/// <param name="childrenArranger"></param>
		protected internal ElementArranger(ElementType elementType,
			IElementInserter inserter, IElementFilter filter, IElementArranger childrenArranger)
		{
			if (inserter == null)
			{
			    throw new ArgumentNullException("inserter");
			}
			
			_elementType = elementType;
			_inserter = inserter;
			_filter = filter;
			_childrenArranger = childrenArranger;
		}

		#endregion Constructors

		#region Public Methods

		/// <summary>
		/// Arranges the element in within the code tree represented in the specified
		/// builder.
		/// </summary>
		/// <param name="parentElement"></param>
		/// <param name="codeElement"></param>
		public virtual void ArrangeElement(ICodeElement parentElement, ICodeElement codeElement)
		{
			if (_childrenArranger != null)
			{
			    List<ICodeElement> children = new List<ICodeElement>(codeElement.Children);
			    codeElement.ClearChildren();
			
			    foreach (ICodeElement childElement in children)
			    {
			        _childrenArranger.ArrangeElement(codeElement, childElement);
			    }
			}
			
			_inserter.InsertElement(parentElement, codeElement);
		}

		/// <summary>
		/// Determines whether or not the specified element can be arranged by 
		/// this arranger.
		/// </summary>
		/// <param name="codeElement"></param>
		/// <returns></returns>
		public virtual bool CanArrange(ICodeElement codeElement)
		{
			return (_elementType == ElementType.NotSpecified ||
			    codeElement.ElementType == _elementType) && 
			    (_filter == null || _filter.IsMatch(codeElement));
		}

		#endregion Public Methods
	}
}