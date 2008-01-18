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
using System.Collections.ObjectModel;
using System.Text;

namespace NArrange.Core.CodeElements
{
	/// <summary>
	/// Code element base class for elements with header comments.
	/// </summary>
	public abstract class CommentedElement : CodeElement	
	{
		#region Fields
		
		private List<ICommentLine> _commentLines;		
		private object _commentLinesLock = new object();		
		
		#endregion Fields
		
		#region Protected Properties
		
		/// <summary>
		/// Base header comment lines collection
		/// </summary>
		protected List<ICommentLine> BaseHeaderCommentLines		
		{
			get
			{
			    if (_commentLines == null)
			    {
			        lock (_commentLinesLock)
			        {
			            if (_commentLines == null)
			            {
			                _commentLines = new List<ICommentLine>();
			            }
			        }
			    }
			
			    return _commentLines;
			}
		}		
		
		#endregion Protected Properties
		
		#region Public Properties
		
		/// <summary>
		/// Gets the collection of header comment lines
		/// </summary>
		public ReadOnlyCollection<ICommentLine> HeaderCommentLines		
		{
			get
			{
			    return BaseHeaderCommentLines.AsReadOnly();
			}
		}		
		
		#endregion Public Properties
		
		#region Public Methods
		
		/// <summary>
		/// Adds a header comment line to this element
		/// </summary>
		/// <param name="commentLine"></param>
		public void AddHeaderCommentLine(ICommentLine commentLine)		
		{
			BaseHeaderCommentLines.Add(commentLine);
		}		
		
		/// <summary>
		/// Adds a header comment line to this element
		/// </summary>
		/// <param name="commentLine"></param>
		public void AddHeaderCommentLine(string commentLine)		
		{
			BaseHeaderCommentLines.Add(new CommentLine(commentLine));
		}		
		
		/// <summary>
		/// Adds a header comment line to this element
		/// </summary>
		/// <param name="commentLine"></param>
		/// <param name="xmlComment"></param>
		public void AddHeaderCommentLine(string commentLine, bool xmlComment)		
		{
			BaseHeaderCommentLines.Add(new CommentLine(commentLine, xmlComment));
		}		
		
		/// <summary>
		/// Clears all header comments.
		/// </summary>
		public void ClearHeaderCommentLines()		
		{
			this.BaseHeaderCommentLines.Clear();
		}		
		
		/// <summary>
		/// Creates a clone of the instance and assigns any state
		/// </summary>
		/// <returns></returns>
		public override object Clone()		
		{
			CommentedElement clone = base.Clone() as CommentedElement;
			
			foreach (ICommentLine commentLine in HeaderCommentLines)
			{
			    ICommentLine commentLineClone = commentLine.Clone() as ICommentLine;
			    clone.AddHeaderCommentLine(commentLineClone);
			}
			
			return clone;
		}		
		
		#endregion Public Methods
	}
}