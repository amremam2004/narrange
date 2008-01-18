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
using System.Text.RegularExpressions;

using NArrange.Core.CodeElements;
using NArrange.Core.Configuration;

namespace NArrange.Core
{
	/// <summary>
	/// Regioned inserter
	/// </summary>
	public class RegionedInserter : IElementInserter	
	{
		#region Fields
		
		private IElementInserter _innerInserter;		
		private RegionConfiguration _regionConfiguration;		
		
		#endregion Fields
		
		#region Constructors
		
		/// <summary>
		/// Creates a new GroupedInserter using the specified grouping configuration
		/// </summary>
		/// <param name="regionConfiguration"></param>
		public RegionedInserter(RegionConfiguration regionConfiguration)		
			: this(regionConfiguration, null)
		{
		}		
		
		/// <summary>
		/// Creates a new GroupedInserter using the specified grouping configuration
		/// and sorter.
		/// </summary>
		/// <param name="regionConfiguration"></param>
		/// <param name="innerInserter"></param>
		public RegionedInserter(RegionConfiguration regionConfiguration, IElementInserter innerInserter)		
		{
			if (regionConfiguration == null)
			{
			    throw new ArgumentNullException("regionConfiguration");
			}
			
			_regionConfiguration = regionConfiguration.Clone() as RegionConfiguration;
			_innerInserter = innerInserter;
		}		
		
		#endregion Constructors
		
		#region Public Methods
		
		/// <summary>
		/// Inserts the element within the parent
		/// </summary>
		/// <param name="parentElement"></param>
		/// <param name="codeElement"></param>
		public void InsertElement(ICodeElement parentElement, ICodeElement codeElement)		
		{
			RegionElement region = null;
			
			string regionName = _regionConfiguration.Name;
			
			foreach (ICodeElement childElement in parentElement.Children)
			{
			    RegionElement regionElement = childElement as RegionElement;
			    if (regionElement != null && regionElement.Name == regionName)
			    {
			        region = regionElement;
			        break;
			    }
			}
			
			if (region == null)
			{
			    region = new RegionElement();
			    region.Name = regionName;
			    parentElement.AddChild(region);
			}
			
			if (_innerInserter != null)
			{
			    _innerInserter.InsertElement(region, codeElement);
			}
			else
			{
			    region.AddChild(codeElement);
			}
		}		
		
		#endregion Public Methods
	}
}