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

using NArrange.Core.Configuration;

namespace NArrange.Core
{
	/// <summary>
	/// Class for creating ElementArranger instances based on configuration
	/// information.
	/// </summary>
	public static class ElementArrangerFactory
	{
		#region Public Methods

		/// <summary>
		/// Creates an element arranger using the specified configuration information.
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="parentRegion"></param>
		/// <returns>Returns an IElementArranger if succesful, otherwise null</returns>
		public static IElementArranger CreateElementArranger(
			ConfigurationElement configuration, RegionConfiguration parentRegion)
		{
			IElementArranger arranger = null;
			
			if (configuration == null)
			{
			    throw new ArgumentNullException("configuration");
			}
			
			RegionConfiguration regionConfiguration = configuration as RegionConfiguration;
			
			ChainElementArranger childrenArranger = new ChainElementArranger();
			foreach (ConfigurationElement childConfiguration in configuration.Elements)
			{
			    IElementArranger childElementArranger = CreateElementArranger(childConfiguration, regionConfiguration);
			    if (childElementArranger != null)
			    {
			        childrenArranger.AddArranger(childElementArranger);
			    }
			}
			
			ElementConfiguration elementConfiguration = configuration as ElementConfiguration;
			if (elementConfiguration != null)
			{
			    ElementArranger elementArranger = null;
			
			    IElementInserter inserter =
			        CreateElementInserter(elementConfiguration.ElementType,
			        elementConfiguration.SortBy, elementConfiguration.GroupBy, parentRegion);
			
			    IElementFilter elementFilter =
			       CreateElementFilter(elementConfiguration.FilterBy);
			
			    elementArranger = new ElementArranger(elementConfiguration.ElementType,
			        inserter, elementFilter, childrenArranger);
			
			    arranger = elementArranger;
			}
			else
			{
			    arranger = childrenArranger;
			}
			
			return arranger;
		}

		#endregion Public Methods

		#region Private Methods

		/// <summary>
		/// Creates an element filter
		/// </summary>
		/// <param name="filterBy"></param>
		/// <returns></returns>
		private static IElementFilter CreateElementFilter(FilterBy filterBy)
		{
			IElementFilter filter = null;
			
			if (filterBy != null)
			{
			    filter = new ElementFilter(filterBy.Condition);
			}
			
			return filter;
		}

		/// <summary>
		/// Creates an element inserter
		/// </summary>
		/// <param name="elementType"></param>
		/// <param name="sortBy"></param>
		/// <param name="groupBy"></param>
		/// <param name="parentRegion"></param>
		/// <returns></returns>
		private static IElementInserter CreateElementInserter(
			ElementType elementType, SortBy sortBy, GroupBy groupBy,
			RegionConfiguration parentRegion)
		{
			IElementInserter inserter = null;
			
			if (sortBy != null)
			{
			    inserter = new SortedInserter(elementType, sortBy);
			}
			
			if (groupBy != null)
			{
			    inserter = new GroupedInserter(groupBy, inserter);
			}
			
			if (parentRegion != null)
			{
			    inserter = new RegionedInserter(parentRegion, inserter);
			}
			
			if (inserter == null)
			{
			    inserter = new DefaultElementInserter();
			}
			
			return inserter;
		}

		#endregion Private Methods
	}
}