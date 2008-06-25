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
 *		- Fixed arranging of region-nested using statements
 *		Justin Dearing
 *		- Code cleanup via ReSharper 4.0 (http://www.jetbrains.com/resharper/)
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

using System;
using System.Collections.Generic;
using System.Threading;
using NArrange.Core.CodeElements;

namespace NArrange.Core
{
	/// <summary>
	/// Uses the chain of responsibility pattern to arrange an element using
	/// the provided ArrangedCodeBuilder.
	/// </summary>
	public sealed class ChainElementArranger : IElementArranger
	{
		#region Fields

		private readonly List<IElementArranger> _arrangers;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new element arranger chain
		/// </summary>
		public ChainElementArranger()
		{
			_arrangers = new List<IElementArranger>();
		}

		#endregion Constructors

		#region Public Methods

		/// <summary>
		/// Adds an arranger to the responsibility chain
		/// </summary>
		/// <param name="arranger"></param>
		public void AddArranger(IElementArranger arranger)
		{
			if (arranger == null)
			{
			    throw new ArgumentNullException("arranger");
			}

			_arrangers.Add(arranger);
		}

		/// <summary>
		/// Arranges an element, delegating the responsibility to the first arranger
		/// encountered who can process the request.
		/// </summary>
		/// <param name="parentElement"></param>
		/// <param name="codeElement"></param>
		/// <returns></returns>
		public void ArrangeElement(ICodeElement parentElement, ICodeElement codeElement)
		{
			bool arranged = false;

			//
			// Region elements are ignored.  Only process their children.
			//
			RegionElement regionElement = codeElement as RegionElement;
			if (regionElement != null)
			{
				List<ICodeElement> regionChildren = new List<ICodeElement>(regionElement.Children);
				regionElement.ClearChildren();

				foreach (ICodeElement regionChildElement in regionChildren)
				{
					ArrangeElement(parentElement, regionChildElement);
				}
			}
			else
			{
				foreach (IElementArranger arranger in _arrangers)
				{
					if (arranger.CanArrange(parentElement, codeElement))
					{
						arranger.ArrangeElement(parentElement, codeElement);
						arranged = true;
						break;
					}
				}

				if (!arranged)
				{
			        if (parentElement != null)
			        {
			            parentElement.AddChild(codeElement);
			        }
			        else
			        {
			            throw new InvalidOperationException(
			                string.Format(
			        		Thread.CurrentThread.CurrentCulture,
			                "Cannot arrange element of type {0} with name '{1}'.",
			                codeElement.GetType().Name, codeElement.Name));
			        }
				}
			}
		}

		/// <summary>
		/// Determines if the specified code element can be arranged by
		/// any arranger in the chain.
		/// </summary>
		/// <param name="codeElement"></param>
		/// <returns></returns>
		public bool CanArrange(ICodeElement codeElement)
		{
			return CanArrange(null, codeElement);
		}

		/// <summary>
		/// Determines if the specified code element can be arranged by
		/// any arranger in the chain.
		/// </summary>
		/// <param name="parentElement"></param>
		/// <param name="codeElement"></param>
		/// <returns></returns>
		public bool CanArrange(ICodeElement parentElement, ICodeElement codeElement)
		{
			bool canArrange = false;

			if (codeElement != null)
			{
			    foreach (IElementArranger arranger in _arrangers)
			    {
			        if (arranger != null && arranger.CanArrange(parentElement, codeElement))
			        {
			            canArrange = true;
			            break;
			        }
			    }
			}

			return canArrange;
		}

		#endregion Public Methods
	}
}