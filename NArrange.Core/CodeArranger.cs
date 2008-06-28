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
 *		- Added special processing for arranging condition directive
 *		  elements
 *		Justin Dearing
 *		- Code cleanup via ReSharper 4.0 (http://www.jetbrains.com/resharper/)
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using NArrange.Core.CodeElements;
using NArrange.Core.Configuration;

namespace NArrange.Core
{
	/// <summary>
	/// Code arranger
	/// </summary>
	public sealed class CodeArranger : ICodeArranger
	{
		#region Fields

		private readonly object _codeArrangeChainLock = new object();
		private readonly CodeConfiguration _configuration;
		private ChainElementArranger _elementArrangerChain;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new code arranger with the specified configuration
		/// </summary>
		/// <param name="configuration">Configuration</param>
		public CodeArranger(CodeConfiguration configuration)
		{
			if (configuration == null)
			{
			    throw new ArgumentNullException("configuration");
			}

			//
			// Clone the configuration information so we don't have to worry about it
			// changing during processing.
			//
			_configuration = configuration.Clone() as CodeConfiguration;
		}

		#endregion Constructors

		#region Private Properties

		private ChainElementArranger ArrangerChain
		{
			get
			{
			    if (_elementArrangerChain == null)
			    {
			        lock (_codeArrangeChainLock)
			        {
			            if (_elementArrangerChain == null)
			            {
			                _elementArrangerChain = new ChainElementArranger();
			                foreach (ConfigurationElement configuration in _configuration.Elements)
			                {
			                    IElementArranger elementArranger = ElementArrangerFactory.CreateElementArranger(
			                        configuration, _configuration, null);
			                    if (elementArranger != null)
			                    {
			                        _elementArrangerChain.AddArranger(elementArranger);
			                    }
			                }
			            }
			        }
			    }

			    return _elementArrangerChain;
			}
		}

		#endregion Private Properties

		#region Public Methods

		/// <summary>
		/// Arranges the code elements according to the configuration supplied 
		/// in the constructor.
		/// </summary>
		/// <param name="originalElements">Original elements</param>
		/// <returns></returns>
		public ReadOnlyCollection<ICodeElement> Arrange(ReadOnlyCollection<ICodeElement> originalElements)
		{
			GroupElement rootElement = new GroupElement();

			if (originalElements != null)
			{
			    foreach (ICodeElement element in originalElements)
			    {
			        ICodeElement elementClone = element.Clone() as ICodeElement;
			        ArrangerChain.ArrangeElement(rootElement, elementClone);
			    }
			}

			List<ICodeElement> arranged = new List<ICodeElement>(rootElement.Children);
			foreach (ICodeElement arrangedElement in arranged)
			{
			    // Remove the root element as the parent.
			    arrangedElement.Parent = null;
			}

			return arranged.AsReadOnly();
		}

		#endregion Public Methods
	}
}