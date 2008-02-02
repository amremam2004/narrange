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
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using NArrange.Core.CodeElements;
using NArrange.Core.Configuration;

namespace NArrange.Core
{
	/// <summary>
	/// Grouped inserter
	/// </summary>
	public class GroupedInserter : IElementInserter
	{
		#region Fields

		private Regex _captureRegex;		
		private GroupBy _groupBy;		
		private IElementInserter _innerInserter;		
		
		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new GroupedInserter using the specified grouping configuration
		/// </summary>
		/// <param name="groupBy"></param>
		public GroupedInserter(GroupBy groupBy)
			: this(groupBy, null)
		{
		}

		/// <summary>
		/// Creates a new GroupedInserter using the specified grouping configuration
		/// and sorter.
		/// </summary>
		/// <param name="groupBy"></param>
		/// <param name="innerInserter"></param>
		public GroupedInserter(GroupBy groupBy, IElementInserter innerInserter)
		{
			if (groupBy == null)
			{
			    throw new ArgumentNullException("groupBy");
			}
			
			_groupBy = groupBy.Clone() as GroupBy;
			_innerInserter = innerInserter;
			
			if (!string.IsNullOrEmpty(groupBy.AttributeCapture))
			{
			    _captureRegex = new Regex(_groupBy.AttributeCapture);
			}
		}

		#endregion Constructors

		#region Private Methods

		/// <summary>
		/// Gets the name of the group the element falls into
		/// </summary>
		/// <param name="elementFilterType"></param>
		/// <param name="captureExpression"></param>
		/// <param name="codeElement"></param>
		/// <returns></returns>
		private string GetGroupName(ElementAttribute elementFilterType, string captureExpression, 
			ICodeElement codeElement)
		{
			string groupName = string.Empty;
			
			string attribute = ElementUtilities.GetAttribute(elementFilterType, codeElement);
			
			if (_captureRegex != null)
			{
			    Match match = _captureRegex.Match(attribute);
			    if (match != null && match.Groups.Count > 1)
			    {
			        groupName = match.Groups[1].Value;
			    }
			}
			else
			{
			    groupName = attribute;
			}
			
			return groupName;
		}

		#endregion Private Methods

		#region Public Methods

		/// <summary>
		/// Inserts the element within the parent
		/// </summary>
		/// <param name="parentElement"></param>
		/// <param name="codeElement"></param>
		public void InsertElement(ICodeElement parentElement, ICodeElement codeElement)
		{
			GroupElement group = null;
			
			string groupName = GetGroupName(_groupBy.By, _groupBy.AttributeCapture, codeElement);
			
			foreach (ICodeElement childElement in parentElement.Children)
			{
			    GroupElement groupElement = childElement as GroupElement;
			    if (groupElement != null && groupElement.Name == groupName)
			    {
			        group = groupElement;
			        break;
			    }
			}
			
			if (group == null)
			{
			    group = new GroupElement();
			    group.Name = groupName;
			    group.SeparatorType = _groupBy.SeparatorType;
			    group.CustomSeparator = _groupBy.CustomSeparator;
			    parentElement.AddChild(group);
			}
			
			if (_innerInserter != null)
			{
			    _innerInserter.InsertElement(group, codeElement);
			}
			else
			{
			    group.AddChild(codeElement);
			}
		}

		#endregion Public Methods
	}
}