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
 *		Justin Dearing
 *		- Code cleanup via ReSharper 4.0 (http://www.jetbrains.com/resharper/)
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

using System.Threading;

namespace NArrange.Core.Configuration
{
	/// <summary>
	/// Binary operator expression
	/// </summary>
	public class BinaryOperatorExpression : IConditionExpression
	{
		#region Fields

		private readonly IConditionExpression _left;
		private readonly BinaryExpressionOperator _operatorType;
		private readonly IConditionExpression _right;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new operator expression.
		/// </summary>
		/// <param name="operatorType"></param>
		/// <param name="left"></param>
		/// <param name="right"></param>
		public BinaryOperatorExpression(BinaryExpressionOperator operatorType, 
			IConditionExpression left, IConditionExpression right)
		{
			_operatorType = operatorType;
			_left = left;
			_right = right;
		}

		#endregion Constructors

		#region Public Properties

		/// <summary>
		/// Left expression
		/// </summary>
		public IConditionExpression Left
		{
			get 
			{
			    return _left;
			}
		}

		/// <summary>
		/// Gets the expression operator
		/// </summary>
		public BinaryExpressionOperator Operator
		{
			get
			{
			    return _operatorType;
			}
		}

		/// <summary>
		/// Right expression
		/// </summary>
		public IConditionExpression Right
		{
			get 
			{
			    return _right;
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
			string operatorString = string.Empty;
			switch(_operatorType)
			{
			    case BinaryExpressionOperator.Equal:
			        operatorString = "==";
			        break;

			    case BinaryExpressionOperator.Contains:
			        operatorString = ":";
			        break;

			    case BinaryExpressionOperator.And:
			        operatorString = "And";
			        break;

			    case BinaryExpressionOperator.Or:
			        operatorString = "Or";
			        break;

			    default:
			        operatorString = EnumUtilities.ToString(_operatorType);
			        break;
			}

			return string.Format(Thread.CurrentThread.CurrentCulture,
			    "({0} {1} {2})", 
			    Left, operatorString, Right);
		}

		#endregion Public Methods
	}
}