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
 *		- Improved performance by checking whether or not CanArrange needs
 *		  to evaluate an expression with a parent scope.
 *		Justin Dearing
 *		- Code cleanup via ReSharper 4.0 (http://www.jetbrains.com/resharper/)
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

using System;
using System.Collections.Generic;

using NArrange.Core.CodeElements;

namespace NArrange.Core
{
	/// <summary>
	/// Standard IElementArranger implementation
	/// </summary>
	public class ElementArranger : IElementArranger
	{
		#region Fields

		private readonly IElementArranger _childrenArranger;
		private readonly ElementType _elementType;
		private readonly IElementFilter _filter;
		private readonly IElementInserter _inserter;

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

		#region Private Methods

		private void ArrangeChildElement(ICodeElement codeElement, ICodeElement childElement)
		{
			//
			// Region elements are ignored.  Only process their children.
			//
			RegionElement regionElement = childElement as RegionElement;
			if (regionElement != null)
			{
				List<ICodeElement> regionChildren = new List<ICodeElement>(regionElement.Children);
				regionElement.ClearChildren();

				foreach (ICodeElement regionChildElement in regionChildren)
				{
					_childrenArranger.ArrangeElement(codeElement, regionChildElement);
				}
			}
			else
			{
				_childrenArranger.ArrangeElement(codeElement, childElement);
			}
		}

		#endregion Private Methods

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
					ArrangeChildElement(codeElement, childElement);
				}

				//
				// For condition directives, arrange the children of each node in the list.
				//
				ConditionDirectiveElement conditionDirective = codeElement as ConditionDirectiveElement;
				if (conditionDirective != null)
				{
					//
					// Skip the first instance since we've already arranged those child elements.
					//
					conditionDirective = conditionDirective.ElseCondition;
				}
				while(conditionDirective != null)
				{
					children = new List<ICodeElement>(conditionDirective.Children);
					conditionDirective.ClearChildren();

					foreach (ICodeElement childElement in children)
					{
						ArrangeChildElement(conditionDirective, childElement);
					}
					conditionDirective = conditionDirective.ElseCondition;
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
			return CanArrange(null, codeElement);
		}

		/// <summary>
		/// Determines whether or not the specified element can be arranged by 
		/// this arranger.
		/// </summary>
		/// <param name="codeElement"></param>
		/// <param name="parentElement"></param>
		/// <returns></returns>
		public virtual bool CanArrange(ICodeElement parentElement, ICodeElement codeElement)
		{
			// Clone the instance and assign the parent
			ICodeElement testCodeElement = codeElement;
			if (parentElement != null &&
				_filter != null && _filter.RequiredScope == ElementAttributeScope.Parent)
			{
				testCodeElement = codeElement.Clone() as ICodeElement;
				testCodeElement.Parent = parentElement.Clone() as ICodeElement;
			}

			return (_elementType == ElementType.NotSpecified ||
				codeElement.ElementType == _elementType) &&
				(_filter == null || _filter.IsMatch(testCodeElement));
		}

		#endregion Public Methods
	}
}