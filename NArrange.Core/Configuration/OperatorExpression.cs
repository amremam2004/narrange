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

namespace NArrange.Core.Configuration
{
	/// <summary>
	/// Operator expression
	/// </summary>
	public class OperatorExpression : IConditionExpression	
	{
		#region Fields
		
		private IConditionExpression _left;		
		private ExpressionOperator _operatorType;		
		private IConditionExpression _right;		
		
		#endregion Fields
		
		#region Constructors
		
		/// <summary>
		/// Creates a new operator expression.
		/// </summary>
		/// <param name="operatorType"></param>
		/// <param name="left"></param>
		/// <param name="right"></param>
		public OperatorExpression(ExpressionOperator operatorType, 
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
		public ExpressionOperator Operator		
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
			    case ExpressionOperator.Equal :
			        operatorString = "==";
			        break;
			
			    case ExpressionOperator.Contains:
			        operatorString = ":";
			        break;
			
			    case ExpressionOperator.And:
			        operatorString = "And";
			        break;
			
			    case ExpressionOperator.Or:
			        operatorString = "Or";
			        break;
			
			    default:
			        throw new ArgumentOutOfRangeException(
			            string.Format(
			            "Unsupported operator type {0}", _operatorType));
			}
			
			return string.Format("({0} {1} {2})", 
			    Left, operatorString, Right);
		}		
		
		#endregion Public Methods

	}
}