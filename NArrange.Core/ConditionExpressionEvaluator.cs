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
using System.IO;
using System.Text;

using NArrange.Core.CodeElements;
using NArrange.Core.Configuration;

namespace NArrange.Core
{
	/// <summary>
	/// Class for evaluating filter expressions
	/// </summary>
	public sealed class ConditionExpressionEvaluator	
	{
		#region Fields
		
		private static ConditionExpressionEvaluator _instance;		
		private static object _instanceLock = new object();		
		
		#endregion Fields
		
		#region Constructors
		
		/// <summary>
		/// Creates a new ConditionExpressionEvaluator
		/// </summary>
		private ConditionExpressionEvaluator()		
		{
		}		
		
		#endregion Constructors
		
		#region Public Methods
		
		/// <summary>
		/// Evaluates an expression against the specified element
		/// </summary>
		/// <param name="conditionExpression"></param>
		/// <param name="element"></param>
		/// <returns></returns>
		public bool Evaluate(IConditionExpression conditionExpression, ICodeElement element)		
		{
			bool result = false;
			
			if (conditionExpression == null)
			{
			    throw new ArgumentNullException("conditionExpression");
			}
			else if (element == null)
			{
			    throw new ArgumentNullException("element");
			}
			
			OperatorExpression operatorExpression = conditionExpression as OperatorExpression;
			if (operatorExpression != null)
			{
			    string leftStr, rightStr;
			    bool leftResult, rightResult;
			
			    switch (operatorExpression.Operator)
			    {
			        case ExpressionOperator.Equal:
			            leftStr = GetExpressionValue(operatorExpression.Left, element);
			            rightStr = GetExpressionValue(operatorExpression.Right, element);
			            result = leftStr == rightStr;
			            break;
			
			        case ExpressionOperator.Contains:
			            leftStr = GetExpressionValue(operatorExpression.Left, element);
			            rightStr = GetExpressionValue(operatorExpression.Right, element);
			            result = leftStr.Contains(rightStr);
			            break;
			
			        case ExpressionOperator.And:
			            leftResult = Evaluate(operatorExpression.Left, element);
			            rightResult = Evaluate(operatorExpression.Right, element);
			            result = leftResult && rightResult;
			            break;
			
			        case ExpressionOperator.Or:
			            leftResult = Evaluate(operatorExpression.Left, element);
			            rightResult = Evaluate(operatorExpression.Right, element);
			            result = leftResult || rightResult;
			            break;
			
			        default:
			            throw new ArgumentOutOfRangeException(
			                string.Format(
			                "Unsupported operator type {0}", operatorExpression.Operator));
			    }
			}
			
			return result;
		}		
		
		#endregion Public Methods
		
		#region Private Methods
		
		private string GetExpressionValue(IConditionExpression expression, ICodeElement element)		
		{
			string value = string.Empty;
			
			if (expression != null && element != null)
			{
			    StringExpression stringExpression = expression as StringExpression;
			    if (stringExpression != null)
			    {
			        value = stringExpression.Text;
			    }
			    else
			    {
			        AttributeExpression attributeExpression = expression as AttributeExpression;
			        if (attributeExpression != null)
			        {
			            value = ElementUtilities.GetAttribute(attributeExpression.ElementAttribute,
			                element);
			        }
			    }
			}
			
			return value;
		}		
		
		#endregion Private Methods
		
		#region Public Properties
		
		/// <summary>
		/// Gets the single instance of the expression evaluator
		/// </summary>
		public static ConditionExpressionEvaluator Instance		
		{
			get
			{
			    if (_instance == null)
			    {
			        lock (_instanceLock)
			        {
			            if (_instance == null)
			            {
			                _instance = new ConditionExpressionEvaluator();
			            }
			        }
			    }
			
			    return _instance;
			}
		}		
		
		#endregion Public Properties
	}
}