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
	/// Member code element
	/// </summary>
	public abstract class MemberElement : AttributedElement	
	{
		#region Fields
		
		private MemberModifier _memberModifiers;		
		private string _type;		
		
		#endregion Fields
		
		#region Public Properties
		
		/// <summary>
		/// Gets whether or not the member is abstract.
		/// </summary>
		public bool IsAbstract		
		{
			get
			{
			    return (_memberModifiers & MemberModifier.Abstract) == MemberModifier.Abstract;
			}
		}		
		
		/// <summary>
		/// Gets whether or not the member is new.
		/// </summary>
		public bool IsNew		
		{
			get
			{
			    return (_memberModifiers & MemberModifier.New) == MemberModifier.New;
			}
		}		
		
		/// <summary>
		/// Gets whether or not the member is an override.
		/// </summary>
		public bool IsOverride		
		{
			get
			{
			    return (_memberModifiers & MemberModifier.Override) == MemberModifier.Override;
			}
		}		
		
		/// <summary>
		/// Gets whether or not the member is sealed.
		/// </summary>
		public bool IsSealed		
		{
			get
			{
			    return (_memberModifiers & MemberModifier.Sealed) == MemberModifier.Sealed;
			}
		}		
		
		/// <summary>
		/// Gets whether or not the member is static.
		/// </summary>
		public bool IsStatic		
		{
			get
			{
			    return (_memberModifiers & MemberModifier.Static) == MemberModifier.Static;
			}
		}		
		
		/// <summary>
		/// Gets whether or not the member is unsafe.
		/// </summary>
		public bool IsUnsafe		
		{
			get
			{
			    return (_memberModifiers & MemberModifier.Unsafe) == MemberModifier.Unsafe;
			}
		}		
		
		/// <summary>
		/// Gets whether or not the member is virtual.
		/// </summary>
		public bool IsVirtual		
		{
			get
			{
			    return (_memberModifiers & MemberModifier.Virtual) == MemberModifier.Virtual;
			}
		}		
		
		/// <summary>
		/// Gets or sets the member attributes
		/// </summary>
		public MemberModifier MemberModifiers		
		{
			get
			{
			    return _memberModifiers;
			}
			set
			{
			    _memberModifiers = value;
			}
		}		
		
		/// <summary>
		/// Gets or sets the type of the member.
		/// </summary>
		public string Type		
		{
			get
			{
			    return _type;
			}
			set
			{
			    _type = value;
			}
		}		
		
		#endregion Public Properties
		
		#region Protected Methods
		
		/// <summary>
		/// Creates a clone of this instance
		/// </summary>
		/// <returns></returns>
		protected override sealed AttributedElement DoAttributedClone()		
		{
			MemberElement clone = DoMemberClone();
			
			//
			// Copy state
			//
			clone._memberModifiers = _memberModifiers;
			clone._type = _type;
			
			return clone;
		}		
		
		/// <summary>
		/// Clones this instance
		/// </summary>
		/// <returns></returns>
		protected abstract MemberElement DoMemberClone();		
		
		#endregion Protected Methods

	}
}