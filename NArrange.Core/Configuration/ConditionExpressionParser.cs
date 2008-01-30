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

namespace NArrange.Core.Configuration
{
	/// <summary>
	/// Class for parsing filter expressions
	/// </summary>
	public sealed class ConditionExpressionParser
	{
		#region Static Fields

		private static ConditionExpressionParser _instance;
		private static object _instanceLock = new object();

		#endregion Static Fields

		#region Constructors

		/// <summary>
		/// Creates a new FilterExpressionParser
		/// </summary>
		private ConditionExpressionParser()
		{
		}

		#endregion Constructors

		#region Public Properties

		/// <summary>
		/// Gets the single instance of the expression parser
		/// </summary>
		public static ConditionExpressionParser Instance
		{
			get
			{
			    if (_instance == null)
			    {
			        lock (_instanceLock)
			        {
			            if (_instance == null)
			            {
			                _instance = new ConditionExpressionParser();
			            }
			        }
			    }
			
			    return _instance;
			}
		}

		#endregion Public Properties

		#region Public Methods

		/// <summary>
		/// Parses and expression to an expression tree
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public IConditionExpression Parse(string expression)
		{
			IConditionExpression conditionExpression = null;
			
			if (expression == null)
			{
			    throw new ArgumentNullException("expression");
			}
			else if (expression.Trim().Length == 0)
			{
			    throw new ArgumentException("expression");
			}
			
			List<IConditionExpression> nodes = new List<IConditionExpression>();
			
			StringReader reader = new StringReader(expression);
			
			StringBuilder expressionBuilder = new StringBuilder();
			
			bool inString = false;
			bool inAttribute = false;
			
			int data = reader.Read();
			while (data > 0)
			{
			    char ch = (char)data;
			
			    switch (ch)
			    {
			        case ' ':
			            if (inString)
			            {
			                expressionBuilder.Append(ch);
			            }
			            break;
			
			        case '$':
			            char nextCh = (char)reader.Peek();
			            if (nextCh == '(')
			            {
			                inAttribute = true;
			                reader.Read();
			            }
			            break;
			
			        case '=':
			            nextCh = (char)reader.Peek();
			            if (nextCh == '=')
			            {
			                nodes.Add(new OperatorExpressionPlaceholder(ExpressionOperator.Equal));
			                reader.Read();
			            }
			            break;
			
			        case ':':
			            nodes.Add(new OperatorExpressionPlaceholder(ExpressionOperator.Contains));
			            reader.Read();
			            break;
			
			        case 'O':
			            nextCh = (char)reader.Peek();
			            if (nextCh == 'r' && !(inString || inAttribute))
			            {
			                nodes.Add(new OperatorExpressionPlaceholder(ExpressionOperator.Or));
			                reader.Read();
			            }
			            else
			            {
			                expressionBuilder.Append(ch);
			            }
			            break;
			
			        case 'A':
			            nextCh = (char)reader.Peek();
			            if (nextCh == 'n' && !(inString || inAttribute))
			            {
			                reader.Read();
			                nextCh = (char)reader.Peek();
			                if (nextCh == 'd')
			                {
			                    nodes.Add(new OperatorExpressionPlaceholder(ExpressionOperator.And));
			                    reader.Read();
			                }
			            }
			            else
			            {
			                expressionBuilder.Append(ch);
			            }
			            break;
			
			        case ')':
			            if (inAttribute)
			            {
			                string attribute = expressionBuilder.ToString();
			                expressionBuilder = new StringBuilder();
			                ElementAttribute elementAttribute;
			                try
			                {
			                    elementAttribute = (ElementAttribute)
			                        Enum.Parse(typeof(ElementAttribute), attribute);
			                }
			                catch (ArgumentException ex)
			                {
			                    throw new FormatException(
			                        string.Format("Unknown attribute: {0}", ex.Message));
			                }
			                AttributeExpression attributeExpresion = new AttributeExpression(
			                    elementAttribute);
			                nodes.Add(attributeExpresion);
			                inAttribute = false;
			            }
			            break;
			
			        case '(':
			            StringBuilder childExpressionBuilder = new StringBuilder();
			            data = reader.Read();
			            int depth = 0;
			            while (data > 0)
			            {
			                ch = (char)data;
			
			                nextCh = (char)reader.Peek();
			                if (ch == '$' && nextCh == '(')
			                {
			                    inAttribute = true;
			                }
			                else if (ch == '(' && !inAttribute)
			                {
			                    depth++;
			                }
			                else if (ch == ')')
			                {
			                    if (inAttribute)
			                    {
			                        inAttribute = false;
			                    }
			                    else if (depth > 0)
			                    {
			                        depth--;
			                    }
			                    else
			                    {
			                        break;
			                    }
			                }
			
			                childExpressionBuilder.Append(ch);
			                data = reader.Read();
			            }
			            IConditionExpression nestedExpression =
			                Parse(childExpressionBuilder.ToString());
			            nodes.Add(nestedExpression);
			            break;
			
			        case '\'':
			            if (inString)
			            {
			                string str = expressionBuilder.ToString();
			                expressionBuilder = new StringBuilder();
			                StringExpression stringExpression = new StringExpression(str);
			                nodes.Add(stringExpression);
			                inString = false;
			            }
			            else
			            {
			                inString = true;
			            }
			            break;
			
			        default:
			            expressionBuilder.Append(ch);
			            break;
			    }
			
			    data = reader.Read();
			}
			
			List<ExpressionOperator> tempOperators = new List<ExpressionOperator>();
			List<IConditionExpression> tempNodes = new List<IConditionExpression>();
			
			Queue<ExpressionOperator> operatorPrecedence = new Queue<ExpressionOperator>();
			operatorPrecedence.Enqueue(ExpressionOperator.Equal);
			operatorPrecedence.Enqueue(ExpressionOperator.Contains);
			operatorPrecedence.Enqueue(ExpressionOperator.And);
			operatorPrecedence.Enqueue(ExpressionOperator.Or);
			
			ExpressionOperator currentOperator = operatorPrecedence.Dequeue();
			while (nodes.Count > 1)
			{
			    for (int nodeIndex = 1; nodeIndex < nodes.Count - 1; nodeIndex++)
			    {
			        OperatorExpressionPlaceholder operatorExpressionPlaceHolder =
			            nodes[nodeIndex] as OperatorExpressionPlaceholder;
			
			        if (operatorExpressionPlaceHolder != null &&
			            operatorExpressionPlaceHolder.Operator == currentOperator)
			        {
			            IConditionExpression left = nodes[nodeIndex - 1];
			            IConditionExpression right = nodes[nodeIndex + 1];
			
			            if ((operatorExpressionPlaceHolder.Operator == ExpressionOperator.Equal ||
			                operatorExpressionPlaceHolder.Operator == ExpressionOperator.Contains) &&
			                !(left is LeafExpression && right is LeafExpression))
			            {
			                throw new FormatException(
			                    string.Format("Invalid expression {0}", expression));
			            }
			
			            OperatorExpression operatorExpression = new OperatorExpression(
			                operatorExpressionPlaceHolder.Operator, left, right);
			
			            nodes[nodeIndex] = operatorExpression;
			            nodes.Remove(left);
			            nodes.Remove(right);
			
			            //
			            // Restart processing of this level
			            //
			            nodeIndex = 0;
			        }
			    }
			
			    if (operatorPrecedence.Count > 0)
			    {
			        currentOperator = operatorPrecedence.Dequeue();
			    }
			    else
			    {
			        break;
			    }
			}
			
			if (nodes.Count != 1)
			{
			    throw new FormatException(
			        string.Format("Invalid expression {0}", expression));
			}
			else
			{
			    conditionExpression = nodes[0] as OperatorExpression;
			    if (conditionExpression == null)
			    {
			        throw new FormatException(
			        string.Format("Invalid expression {0}", expression));
			    }
			}
			
			return conditionExpression;
		}

		#endregion Public Methods

		#region Other

		/// <summary>
		/// Operator expression
		/// </summary>
		private class OperatorExpressionPlaceholder : LeafExpression
		{
			private ExpressionOperator _operatorType;
			/// <summary>
			/// Creates a new operator expression.
			/// </summary>
			/// <param name="operatorType"></param>
			public OperatorExpressionPlaceholder(ExpressionOperator operatorType)
			{
				_operatorType = operatorType;
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
		}

		#endregion Other
	}
}