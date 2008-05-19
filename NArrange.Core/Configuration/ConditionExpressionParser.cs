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
 *      - Added parsing for file attribute expressions
 *      - Added a Matches binary operator for regular expression support in 
 *        condition expressions
 *      - Allow apostrophes in string expressions by escaping with another
 *        apostrophe
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading;

using NArrange.Core.CodeElements;

namespace NArrange.Core.Configuration
{
	/// <summary>
	/// Class for parsing filter expressions
	/// </summary>
	public sealed class ConditionExpressionParser
	{
		#region Constants

		/// <summary>
		/// Expression end
		/// </summary>
		public const char ExpressionEnd = ')';

		/// <summary>
		/// Character that marks the start of an attribute expression
		/// </summary>
		public const char ExpressionPrefix = '$';

		/// <summary>
		/// Expression start
		/// </summary>
		public const char ExpressionStart = '(';

		/// <summary>
		/// File attribute scope
		/// </summary>
		public const string FileAttributeScope = "File";

		/// <summary>
		/// Scope separator
		/// </summary>
		public const char ScopeSeparator = '.';

		#endregion Constants

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

		#region Private Methods

		/// <summary>
		/// Takes in a list of expressions and operator expression placeholders and
		/// builds an expression tree node.
		/// </summary>
		/// <param name="originalNodes"></param>
		/// <param name="originalExpression"></param>
		/// <returns></returns>
		private static IConditionExpression AssembleExpressionTree(
			ReadOnlyCollection<IConditionExpression> originalNodes, string originalExpression)
		{
			IConditionExpression conditionExpression = null;

			List<IConditionExpression> nodes = new List<IConditionExpression>(originalNodes);

			//
			// Build a queue that represents the binary operator precedence
			//
			Queue<BinaryExpressionOperator> operatorPrecedence = new Queue<BinaryExpressionOperator>();
			operatorPrecedence.Enqueue(BinaryExpressionOperator.Equal);
			operatorPrecedence.Enqueue(BinaryExpressionOperator.NotEqual);
			operatorPrecedence.Enqueue(BinaryExpressionOperator.Contains);
			operatorPrecedence.Enqueue(BinaryExpressionOperator.Matches);
			operatorPrecedence.Enqueue(BinaryExpressionOperator.And);
			operatorPrecedence.Enqueue(BinaryExpressionOperator.Or);

			//
			// Loop through the nodes and merge them by operator precedence
			//
			BinaryExpressionOperator currentOperator = operatorPrecedence.Dequeue();
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

			            if ((operatorExpressionPlaceHolder.Operator == BinaryExpressionOperator.Equal ||
			                operatorExpressionPlaceHolder.Operator == BinaryExpressionOperator.Contains) &&
			                !(left is LeafExpression && right is LeafExpression))
			            {
			                throw new FormatException(
			                    string.Format(Thread.CurrentThread.CurrentCulture,
			                    "Invalid expression {0}", originalExpression));
			            }

			            BinaryOperatorExpression operatorExpression = new BinaryOperatorExpression(
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

			//
			// At the end of everything, we should have a single binary or unary
			// condition expression.  Anything else is invalid and a format exception
			// will be thrown.
			//
			if (nodes.Count == 1)
			{
			    conditionExpression = nodes[0] as BinaryOperatorExpression;
			    if (conditionExpression == null)
			    {
			        conditionExpression = nodes[0] as UnaryOperatorExpression;
			    }
			}

			if (conditionExpression == null)
			{
			    throw new FormatException(
			    string.Format(Thread.CurrentThread.CurrentCulture,
			    "Invalid expression {0}", originalExpression));
			}

			return conditionExpression;
		}

		#endregion Private Methods

		#region Public Methods

		/// <summary>
		/// Parses and expression to an expression tree
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public IConditionExpression Parse(string expression)
		{
			const int DefaultExpressionLength = 128;
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

			StringBuilder expressionBuilder = new StringBuilder(DefaultExpressionLength);

			bool inString = false;
			bool inAttribute = false;

			int data = reader.Read();
			while (data > 0)
			{
			    char ch = (char)data;
			    char nextCh = (char)reader.Peek();

			    if (inString && ch != '\'')
			    {
			        expressionBuilder.Append(ch);
			    }
			    else
			    {
			        switch (ch)
			        {
			            case ' ':
			            case '\t':
			            case '\r':
			            case '\n':
			                // Eat whitespace
			                break;

			            case ExpressionPrefix:
			                if (nextCh == ExpressionStart)
			                {
			                    inAttribute = true;
			                    reader.Read();
			                }
			                break;

			            case '=':
			                if (nextCh == '=')
			                {
			                    nodes.Add(new OperatorExpressionPlaceholder(BinaryExpressionOperator.Equal));
			                    reader.Read();
			                }
			                else if (nextCh == '~')
			                {
			                    nodes.Add(new OperatorExpressionPlaceholder(BinaryExpressionOperator.Matches));
			                    reader.Read();
			                }
			                break;

			            case '!':
			                if (nextCh == '=')
			                {
			                    nodes.Add(new OperatorExpressionPlaceholder(BinaryExpressionOperator.NotEqual));
			                    reader.Read();
			                }
			                else
			                {
			                    expressionBuilder.Append(ch);
			                }
			                break;

			            case ':':
			                nodes.Add(new OperatorExpressionPlaceholder(BinaryExpressionOperator.Contains));
			                reader.Read();
			                break;

			            case 'O':
			                if (nextCh == 'r' && !inAttribute)
			                {
			                    nodes.Add(new OperatorExpressionPlaceholder(BinaryExpressionOperator.Or));
			                    reader.Read();
			                }
			                else
			                {
			                    expressionBuilder.Append(ch);
			                }
			                break;

			            case 'A':
			                if (nextCh == 'n' && !inAttribute)
			                {
			                    reader.Read();
			                    nextCh = (char)reader.Peek();
			                    if (nextCh == 'd')
			                    {
			                        nodes.Add(new OperatorExpressionPlaceholder(BinaryExpressionOperator.And));
			                        reader.Read();
			                    }
			                }
			                else
			                {
			                    expressionBuilder.Append(ch);
			                }
			                break;

			            case ExpressionEnd:
			                if (inAttribute)
			                {
			                    string attribute = expressionBuilder.ToString();
			                    expressionBuilder = new StringBuilder(DefaultExpressionLength);
			                    ElementAttributeScope elementScope = ElementAttributeScope.Element;
			                    bool isFileExpression = false;

			                    int separatorIndex = attribute.LastIndexOf(ScopeSeparator);
			                    if (separatorIndex > 0)
			                    {
			                        try
			                        {
			                            string attributeScope = attribute.Substring(0, separatorIndex);
			                            attribute = attribute.Substring(separatorIndex + 1);

			                            if (attributeScope == FileAttributeScope)
			                            {
			                                isFileExpression = true;
			                            }
			                            else
			                            {
			                                elementScope = (ElementAttributeScope)
			                                    Enum.Parse(typeof(ElementAttributeScope), attributeScope);
			                            }
			                        }
			                        catch (ArgumentException ex)
			                        {
			                            throw new FormatException(
			                                string.Format(Thread.CurrentThread.CurrentCulture,
			                                "Unknown element scope: {0}", ex.Message));
			                        }
			                    }

			                    if (isFileExpression)
			                    {
			                        FileAttributeType fileAttribute;

			                        try
			                        {
			                            fileAttribute = (FileAttributeType)
			                                Enum.Parse(typeof(FileAttributeType), attribute);
			                        }
			                        catch (ArgumentException ex)
			                        {
			                            throw new FormatException(
			                                string.Format(Thread.CurrentThread.CurrentCulture,
			                                "Unknown attribute: {0}", ex.Message));
			                        }

			                        FileAttributeExpression attributeExpresion = new FileAttributeExpression(
			                            fileAttribute);
			                        nodes.Add(attributeExpresion);
			                    }
			                    else
			                    {
			                        ElementAttributeType elementAttribute;

			                        try
			                        {
			                            elementAttribute = (ElementAttributeType)
			                                Enum.Parse(typeof(ElementAttributeType), attribute);
			                        }
			                        catch (ArgumentException ex)
			                        {
			                            throw new FormatException(
			                                string.Format(Thread.CurrentThread.CurrentCulture,
			                                "Unknown attribute: {0}", ex.Message));
			                        }

			                        ElementAttributeExpression attributeExpresion = new ElementAttributeExpression(
			                            elementAttribute, elementScope);
			                        nodes.Add(attributeExpresion);
			                    }

			                    inAttribute = false;
			                }
			                else if (expressionBuilder.Length > 0 && nodes.Count > 0)
			                {
			                    IConditionExpression innerExpression = nodes[nodes.Count - 1];
			                    nodes.RemoveAt(nodes.Count - 1);

			                    string unaryOperatorString = expressionBuilder.ToString().Trim();
			                    expressionBuilder = new StringBuilder(DefaultExpressionLength);

			                    UnaryExpressionOperator unaryOperator;

			                    if (unaryOperatorString == "!")
			                    {
			                        unaryOperator = UnaryExpressionOperator.Negate;
			                    }
			                    else
			                    {
			                        throw new FormatException(
			                        string.Format(Thread.CurrentThread.CurrentCulture,
			                        "Invalid operator {0}", unaryOperatorString));
			                    }

			                    UnaryOperatorExpression unaryOperatorExpression = new UnaryOperatorExpression(
			                        unaryOperator, innerExpression);

			                    nodes.Add(unaryOperatorExpression);
			                }
			                else
			                {
			                    data = reader.Read();
			                }
			                break;

			            case ExpressionStart:
			                IConditionExpression nestedExpression = null;
			                StringBuilder childExpressionBuilder = new StringBuilder(DefaultExpressionLength);
			                data = reader.Read();
			                int depth = 0;
			                while (data > 0)
			                {
			                    ch = (char)data;

			                    nextCh = (char)reader.Peek();
			                    if (ch == ExpressionPrefix && nextCh == ExpressionStart)
			                    {
			                        inAttribute = true;
			                        childExpressionBuilder.Append(ExpressionPrefix);
			                        data = reader.Read();
			                        childExpressionBuilder.Append(ExpressionStart);
			                    }
			                    else if (ch == ExpressionStart && !inAttribute)
			                    {
			                        depth++;
			                        childExpressionBuilder.Append(ExpressionStart);
			                    }
			                    else if (nextCh == ExpressionEnd)
			                    {
			                        if (inAttribute || depth > 0)
			                        {
			                            if (inAttribute)
			                            {
			                                inAttribute = false;
			                            }
			                            else if (depth > 0)
			                            {
			                                depth--;
			                            }

			                            childExpressionBuilder.Append(ch);
			                            data = reader.Read();
			                            childExpressionBuilder.Append(ExpressionEnd);
			                        }
			                        else
			                        {
			                            childExpressionBuilder.Append(ch);
			                            break;
			                        }
			                    }
			                    else
			                    {
			                        childExpressionBuilder.Append(ch);
			                    }

			                    data = reader.Read();
			                }
			                nestedExpression = Parse(childExpressionBuilder.ToString());
			                nodes.Add(nestedExpression);
			                break;

			            case '\'':
			                if (inString)
			                {
			                    if (nextCh == '\'')
			                    {
			                        expressionBuilder.Append(ch);
			                        reader.Read();
			                    }
			                    else
			                    {
			                        string str = expressionBuilder.ToString();
			                        expressionBuilder = new StringBuilder(DefaultExpressionLength);
			                        StringExpression stringExpression = new StringExpression(str);
			                        nodes.Add(stringExpression);
			                        inString = false;
			                    }
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
			    }

			    data = reader.Read();
			}

			//
			// Assembly the flat list of expressions and expression placeholders into an
			// expression tree.
			//
			conditionExpression = AssembleExpressionTree(nodes.AsReadOnly(), expression);

			return conditionExpression;
		}

		#endregion Public Methods

		#region Other

		/// <summary>
		/// Operator expression
		/// </summary>
		private class OperatorExpressionPlaceholder : LeafExpression
		{
			private BinaryExpressionOperator _operatorType;

			/// <summary>
			/// Creates a new operator expression.
			/// </summary>
			/// <param name="operatorType"></param>
			public OperatorExpressionPlaceholder(BinaryExpressionOperator operatorType)
			{
				_operatorType = operatorType;
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
		}

		#endregion Other
	}
}