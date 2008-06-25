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
 *      - Fixed an ordering issue when inserting region elements
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
	/// Regioned inserter
	/// </summary>
	public class RegionedInserter : IElementInserter
	{
		#region Fields

		private readonly IElementInserter _innerInserter;
		private readonly ReadOnlyCollection<string> _levelRegions;
		private readonly ConfigurationElement _parentConfiguration;
		private readonly RegionConfiguration _regionConfiguration;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new RegionedInserter using the specified configuration
		/// </summary>
		/// <param name="regionConfiguration"></param>
		/// <param name="parentConfiguration"></param>
		public RegionedInserter(RegionConfiguration regionConfiguration, 
			ConfigurationElement parentConfiguration)
			: this(regionConfiguration, parentConfiguration, null)
		{
		}

		/// <summary>
		/// Creates a new RegionedInserter using the specified configuration
		/// and sorter.
		/// </summary>
		/// <param name="regionConfiguration"></param>
		/// <param name="parentConfiguration"></param>
		/// <param name="innerInserter"></param>
		public RegionedInserter(RegionConfiguration regionConfiguration, 
			ConfigurationElement parentConfiguration, IElementInserter innerInserter)
		{
			if (regionConfiguration == null)
			{
			    throw new ArgumentNullException("regionConfiguration");
			}

			if (parentConfiguration == null)
			{
			    throw new ArgumentNullException("parentConfiguration");
			}

			_regionConfiguration = regionConfiguration;
			_parentConfiguration = parentConfiguration;
			_innerInserter = innerInserter;

			List<string> levelRegions = new List<string>();
			foreach (ConfigurationElement siblingConfiguration in
			    _parentConfiguration.Elements)
			{
			    RegionConfiguration siblingRegionConfiguration = siblingConfiguration as RegionConfiguration;
			    if (siblingRegionConfiguration != null)
			    {
			        levelRegions.Add(siblingRegionConfiguration.Name);
			    }
			}

			_levelRegions = levelRegions.AsReadOnly();
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
			if (codeElement != null)
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

			        if (parentElement.Children.Count == 0)
			        {
			            parentElement.AddChild(region);
			        }
			        else
			        {
			            //
			            // Determine where to insert the new region
			            //
			            int insertIndex = 0;
			            int compareIndex = _levelRegions.IndexOf(region.Name);

			            for (int siblingIndex = 0; siblingIndex < parentElement.Children.Count;
			                siblingIndex++)
			            {
			                RegionElement siblingRegion = parentElement.Children[siblingIndex]
			                    as RegionElement;
							if (siblingRegion != null)
							{
								insertIndex = siblingIndex;

								int siblingCompareIndex = _levelRegions.IndexOf(siblingRegion.Name);
								if (compareIndex <= siblingCompareIndex)
								{
									break;
								}
								else
								{
									insertIndex++;
								}
							}
							else
							{
								insertIndex++;
							}
			            }

			            parentElement.InsertChild(insertIndex, region);
			        }
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
		}

		#endregion Public Methods
	}
}