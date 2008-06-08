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
 *		- Fixed sort direction for inner sorts
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using NArrange.Core.CodeElements;
using NArrange.Core.Configuration;

namespace NArrange.Core
{
	/// <summary>
	/// Sorted inserter
	/// </summary>
	public class SortedInserter : IElementInserter
	{
		#region Fields

		private Comparison<ICodeElement> _comparison;
		private ElementType _elementType;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new sorted inserter using the specified sorting configuration
		/// </summary>
		/// <param name="elementType"></param>
		/// <param name="sortBy"></param>
		public SortedInserter(ElementType elementType, SortBy sortBy)
		{
			if (sortBy == null)
			{
			    throw new ArgumentNullException("sortBy");
			}

			_elementType = elementType;
			_comparison = CreateComparison(sortBy);
		}

		#endregion Constructors

		#region Private Methods

		private Comparison<ICodeElement> CreateComparison(SortBy sortBy)
		{
			Comparison<ICodeElement> comparison = delegate(ICodeElement x, ICodeElement y)
			{
			    int compareValue = 0;

			    if (x == null && y != null)
			    {
			        compareValue = -1;
			    }
			    else if (x != null && y == null)
			    {
			        compareValue = 1;
			    }
			    else
			    {
			        switch (sortBy.By)
			        {
			            case ElementAttributeType.Access:
			                AttributedElement attributedX = x as AttributedElement;
			                AttributedElement attributedY = y as AttributedElement;
			                if (attributedX != null && attributedY != null)
			                {
			                    compareValue = attributedX.Access.CompareTo(attributedY.Access);
			                }
			                break;

			            case ElementAttributeType.ElementType:
			                compareValue = x.ElementType.CompareTo(y.ElementType);
			                break;

			            case ElementAttributeType.Type:
			                string xType = ElementUtilities.GetAttribute(ElementAttributeType.Type, x);
			                string yType = ElementUtilities.GetAttribute(ElementAttributeType.Type, y);
			                compareValue = xType.CompareTo(yType);
			                break;

			            case ElementAttributeType.Name:
			                    compareValue = StringComparer.Ordinal.Compare(
			                        x.Name, y.Name);
			                    break;

			            default:
			                compareValue = 0;
			                break;
			        }

			        //
			        // Inner sort?
			        //
			        if (compareValue == 0)
			        {
			            if (sortBy.InnerSortBy != null)
			            {
			                Comparison<ICodeElement> innerComparison = CreateComparison(sortBy.InnerSortBy);
			                compareValue = innerComparison(x, y);
			            }
			        }
					else if (sortBy.Direction == ListSortDirection.Descending)
					{
						compareValue = -compareValue;
					}
			    }

			    return compareValue;
			};

			return comparison;
		}

		#endregion Private Methods

		#region Public Methods

		/// <summary>
		/// Inserts an element into the parent using the strategy defined by the 
		/// sort configuration.
		/// </summary>
		/// <param name="parentElement"></param>
		/// <param name="codeElement"></param>
		public void InsertElement(ICodeElement parentElement, ICodeElement codeElement)
		{
			if (codeElement != null)
			{
			    ICodeElement compareElement = null;

			    int insertIndex = 0;

				if (parentElement.Children.Count > 0)
			    {
			        for (int elementIndex = 0; elementIndex < parentElement.Children.Count; elementIndex++)
			        {
			            compareElement = parentElement.Children[elementIndex];

			            bool greaterOrEqual =
			                (_elementType == ElementType.NotSpecified &&
			                _comparison(codeElement, compareElement) >= 0) ||
			                (_elementType != ElementType.NotSpecified &&
			                ((compareElement != null && compareElement.ElementType != _elementType) ||
			                _comparison(codeElement, compareElement) >= 0));
			            if (greaterOrEqual)
			            {
			                insertIndex++;
			            }
			            else
			            {
			                break;
			            }
			        }
			    }

			    parentElement.InsertChild(insertIndex, codeElement);
			}
		}

		#endregion Public Methods
	}
}