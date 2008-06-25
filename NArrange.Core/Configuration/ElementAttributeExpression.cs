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
 *      - Allow scoping in element attribute expression evaluation
 *		Justin Dearing
 *		- Code cleanup via ReSharper 4.0 (http://www.jetbrains.com/resharper/)
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

using System.Threading;

namespace NArrange.Core.Configuration
{
	/// <summary>
	/// Element attribute expression
	/// </summary>
	public class ElementAttributeExpression : LeafExpression
	{
		#region Fields

		private readonly ElementAttributeType _elementAttributeType;
		private readonly ElementAttributeScope _elementScope;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new element attribute expression
		/// </summary>
		/// <param name="elementAttribute"></param>
		public ElementAttributeExpression(ElementAttributeType elementAttribute)
			: this(elementAttribute, ElementAttributeScope.Element)
		{
		}

		/// <summary>
		/// Creates a new element attribute expression
		/// </summary>
		/// <param name="elementAttribute"></param>
		/// <param name="scope"></param>
		public ElementAttributeExpression(ElementAttributeType elementAttribute, ElementAttributeScope scope)
		{
			_elementAttributeType = elementAttribute;
			_elementScope = scope;
		}

		#endregion Constructors

		#region Public Properties

		/// <summary>
		/// Gets the element attribute specified by the expression
		/// </summary>
		public ElementAttributeType ElementAttribute
		{
			get
			{
			    return _elementAttributeType;
			}
		}

		/// <summary>
		/// Gets the element scope specified by the expression
		/// </summary>
		public ElementAttributeScope Scope
		{
			get
			{
			    return _elementScope;
			}
		}

		#endregion Public Properties

		#region Public Methods

		/// <summary>
		/// Gets the string representation of this expression
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(
			    Thread.CurrentThread.CurrentCulture, 
			    "$({0}.{1})", 
			    _elementScope, 
			    _elementAttributeType);
		}

		#endregion Public Methods
	}
}