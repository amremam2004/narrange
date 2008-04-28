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
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace NArrange.Core.Configuration
{
	/// <summary>
	/// Element configuration
	/// </summary>
	[XmlType("Element")]
	public class ElementConfiguration : ConfigurationElement
	{
		#region Fields

		private ElementType _elementType;
		private FilterBy _filterBy;
		private GroupBy _groupBy;
		private SortBy _sortBy;

		#endregion Fields

		#region Public Properties

		/// <summary>
		/// Element type
		/// </summary>
		[XmlAttribute("Type")]
		public ElementType ElementType
		{
			get
			{
			    return _elementType;
			}
			set
			{
			    _elementType = value;
			}
		}

		/// <summary>
		/// Gets or sets the filter specification
		/// </summary>
		[XmlElement("Filter")]
		public FilterBy FilterBy
		{
			get
			{
			    return _filterBy;
			}
			set
			{
			    _filterBy = value;
			}
		}

		/// <summary>
		/// Specifies grouping of elements
		/// </summary>
		[XmlElement("Group")]
		public GroupBy GroupBy
		{
			get
			{
			    return _groupBy;
			}
			set
			{
			    _groupBy = value;
			}
		}

		/// <summary>
		/// Gets or sets the sort specification
		/// </summary>
		[XmlElement("Sort")]
		public SortBy SortBy
		{
			get
			{
			    return _sortBy;
			}
			set
			{
			    _sortBy = value;
			}
		}

		#endregion Public Properties

		#region Protected Methods

		/// <summary>
		/// Creates a clone of this instance
		/// </summary>
		/// <returns></returns>
		protected override ConfigurationElement DoClone()
		{
			ElementConfiguration clone = new ElementConfiguration();

			clone._elementType = _elementType;

			if (_filterBy != null)
			{
			    clone._filterBy = _filterBy.Clone() as FilterBy;
			}

			if (_groupBy != null)
			{
			    clone._groupBy = _groupBy.Clone() as GroupBy;
			}

			if (_sortBy != null)
			{
			    clone._sortBy = _sortBy.Clone() as SortBy;
			}

			return clone;
		}

		#endregion Protected Methods

		#region Public Methods

		/// <summary>
		/// Gets a string representation
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(Thread.CurrentThread.CurrentCulture,
			    "Type: {0}", this.ElementType);
		}

		#endregion Public Methods
	}
}