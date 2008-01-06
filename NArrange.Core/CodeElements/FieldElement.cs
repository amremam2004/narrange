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

namespace NArrange.Core.CodeElements
{
	/// <summary>
	/// Field code element
	/// </summary>
	public class FieldElement : MemberElement	
	{
		#region Fields
		
		private string _initialValue;		
		private bool _isVolatile;		
		
		#endregion Fields
		
		#region Public Properties
		
		/// <summary>
		/// Gets the element type
		/// </summary>
		public override ElementType ElementType		
		{
			get 
			{
			    return ElementType.Field;
			}
		}		
		
		/// <summary>
		/// Gets or sets the initial value of the field.
		/// </summary>
		public string InitialValue		
		{
			get
			{
			    return _initialValue;
			}
			set
			{
			    _initialValue = value;
			}
		}		
		
		/// <summary>
		/// Gets whether or not the field is a constant.
		/// </summary>
		public bool IsConstant		
		{
			get
			{
			    return (MemberModifiers & MemberModifier.Constant) == MemberModifier.Constant;
			}
		}		
		
		/// <summary>
		/// Gets whether or not the field is read-only.
		/// </summary>
		public bool IsReadOnly		
		{
			get
			{
			    return (MemberModifiers & MemberModifier.ReadOnly) == MemberModifier.ReadOnly;
			}
		}		
		
		/// <summary>
		/// Gets or sets whether or not the field is volatile.
		/// </summary>
		public bool IsVolatile		
		{
			get
			{
			    return _isVolatile;
			}
			set
			{
			    _isVolatile = value;
			}
		}		
		
		#endregion Public Properties
		
		#region Protected Methods
		
		/// <summary>
		/// Creates a clone of this instance
		/// </summary>
		/// <returns></returns>
		protected override MemberElement DoMemberClone()		
		{
			FieldElement fieldElement = new FieldElement();
			
			//
			// Copy state
			//
			fieldElement._initialValue = _initialValue;
			fieldElement._isVolatile = _isVolatile;
			
			return fieldElement;
		}		
		
		#endregion Protected Methods
		
		#region Public Methods
		
		/// <summary>
		/// Allows an ICodeElementVisitor to process (or visit) this element.
		/// </summary>
		/// <remarks>See the Gang of Four Visitor design pattern.</remarks>
		/// <param name="visitor"></param>
		public override void Accept(ICodeElementVisitor visitor)		
		{
			visitor.VisitFieldElement(this);
		}		
		
		#endregion Public Methods

	}
}