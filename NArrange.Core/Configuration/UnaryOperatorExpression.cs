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
using System.Globalization;
using System.Text;
using System.Threading;

namespace NArrange.Core.Configuration
{
	/// <summary>
	/// Unary operator expression
	/// </summary>
	public class UnaryOperatorExpression : IConditionExpression
	{
		#region Fields

		private IConditionExpression _innerExpression;
		private UnaryExpressionOperator _operatorType;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new unary operator expression.
		/// </summary>
		/// <param name="operatorType"></param>
		/// <param name="innerExpression"></param>
		public UnaryOperatorExpression(UnaryExpressionOperator operatorType, 
			IConditionExpression innerExpression)
		{
			_operatorType = operatorType;
			_innerExpression = innerExpression;
		}

		#endregion Constructors

		#region Internal Properties

		/// <summary>
		/// Left expression
		/// </summary>
		IConditionExpression IConditionExpression.Left
		{
			get 
			{
			    return _innerExpression;
			}
		}

		/// <summary>
		/// Right expression
		/// </summary>
		IConditionExpression IConditionExpression.Right
		{
			get 
			{
			    return null;
			}
		}

		#endregion Internal Properties

		#region Public Properties

		/// <summary>
		/// Inner expression
		/// </summary>
		public IConditionExpression InnerExpression
		{
			get
			{
			    return _innerExpression;
			}
		}

		/// <summary>
		/// Gets the expression operator
		/// </summary>
		public UnaryExpressionOperator Operator
		{
			get
			{
			    return _operatorType;
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
			string preOperatorString = string.Empty;

			switch(_operatorType)
			{
			    case UnaryExpressionOperator.Negate:
			        preOperatorString = "!";
			        break;

			    default:
			        preOperatorString = EnumUtilities.ToString(_operatorType);
			        break;
			}

			return string.Format(Thread.CurrentThread.CurrentCulture,
			    "{0}({1})", 
			    preOperatorString,
			    InnerExpression);
		}

		#endregion Public Methods
	}
}