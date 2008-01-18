using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NArrange.Core;
using NArrange.Core.Configuration;

namespace NArrange.Tests.Core.Configuration
{
	/// <summary>
	/// Test fixture for the FilterExpressionParser class.
	/// </summary>
	[TestFixture]
	public class ConditionExpressionParserTests	
	{
		#region Public Methods
		
		/// <summary>
		/// Tests parsing an AND expression.
		/// </summary>
		[Test]
		public void ParseAndExpressionTest()		
		{
			Action<string> testExpression = delegate(string condition)
			{
			    IConditionExpression expression = ConditionExpressionParser.Instance.Parse(
			        condition);
			    Assert.IsNotNull(expression, "Expected an expression instance.");
			
			    OperatorExpression operatorExpression = expression as OperatorExpression;
			    Assert.IsNotNull(operatorExpression, "Expected an operator expression.");
			    Assert.AreEqual(ExpressionOperator.And, operatorExpression.Operator,
			        "Unexpected operator.");
			
			    //
			    // Left
			    //
			    OperatorExpression leftExpression = expression.Left as OperatorExpression;
			    Assert.IsNotNull(leftExpression, "Expected an operator expression.");
			    Assert.AreEqual(ExpressionOperator.Equal, leftExpression.Operator,
			        "Unexpected operator.");
			
			    AttributeExpression leftAttributeExpression = leftExpression.Left as AttributeExpression;
			    Assert.IsNotNull(leftAttributeExpression, "Unexpected left node type.");
			    Assert.AreEqual(ElementAttribute.Name, leftAttributeExpression.ElementAttribute,
			        "Attribute expression was not parsed correctly.");
			    Assert.IsNull(leftAttributeExpression.Left);
			    Assert.IsNull(leftAttributeExpression.Right);
			
			    StringExpression leftStringExpression = leftExpression.Right as StringExpression;
			    Assert.IsNotNull(leftStringExpression, "Unexpected right node type.");
			    Assert.AreEqual("Test 1", leftStringExpression.Text,
			        "String expression was not parsed correctly.");
			    Assert.IsNull(leftStringExpression.Left);
			    Assert.IsNull(leftStringExpression.Right);
			
			    //
			    // Right
			    //
			    OperatorExpression rightExpression = expression.Right as OperatorExpression;
			    Assert.IsNotNull(rightExpression, "Expected an operator expression.");
			    Assert.AreEqual(ExpressionOperator.Equal, rightExpression.Operator,
			        "Unexpected operator.");
			
			    AttributeExpression rightAttributeExpression = rightExpression.Left as AttributeExpression;
			    Assert.IsNotNull(rightAttributeExpression, "Unexpected left node type.");
			    Assert.AreEqual(ElementAttribute.Name, rightAttributeExpression.ElementAttribute,
			        "Attribute expression was not parsed correctly.");
			    Assert.IsNull(rightAttributeExpression.Left);
			    Assert.IsNull(rightAttributeExpression.Right);
			
			    StringExpression rightStringExpression = rightExpression.Right as StringExpression;
			    Assert.IsNotNull(rightStringExpression, "Unexpected right node type.");
			    Assert.AreEqual("Test 2", rightStringExpression.Text,
			        "String expression was not parsed correctly.");
			    Assert.IsNull(rightStringExpression.Left);
			    Assert.IsNull(rightStringExpression.Right);
			};
			
			string expressionText;
			expressionText = "($(Name) == 'Test 1') And ($(Name) == 'Test 2')";
			testExpression(expressionText);
			
			expressionText = "$(Name) == 'Test 1' And $(Name) == 'Test 2'";
			testExpression(expressionText);
		}		
		
		/// <summary>
		/// Tests parsing an expression.
		/// </summary>
		[Test]
		public void ParseAndOrExpressionTest()		
		{
			Action<string> testExpression = delegate(string condition)
			{
			    IConditionExpression expression = ConditionExpressionParser.Instance.Parse(
			        condition);
			    Assert.IsNotNull(expression, "Expected an expression instance.");
			
			    OperatorExpression operatorExpression = expression as OperatorExpression;
			    Assert.IsNotNull(operatorExpression, "Expected an operator expression.");
			    Assert.AreEqual(ExpressionOperator.And, operatorExpression.Operator,
			        "Unexpected operator.");
			
			    //
			    // And left
			    //
			    OperatorExpression testExpression3 = operatorExpression.Left as OperatorExpression;
			    Assert.IsNotNull(testExpression3, "Expected an operator expression.");
			    Assert.AreEqual(ExpressionOperator.Equal, testExpression3.Operator,
			        "Unexpected operator.");
			
			    AttributeExpression test3AttributeExpression = testExpression3.Left as AttributeExpression;
			    Assert.IsNotNull(test3AttributeExpression, "Unexpected left node type.");
			    Assert.AreEqual(ElementAttribute.Name, test3AttributeExpression.ElementAttribute,
			        "Attribute expression was not parsed correctly.");
			    Assert.IsNull(test3AttributeExpression.Left);
			    Assert.IsNull(test3AttributeExpression.Right);
			
			    StringExpression test3StringExpression = testExpression3.Right as StringExpression;
			    Assert.IsNotNull(test3StringExpression, "Unexpected right node type.");
			    Assert.AreEqual("Test 3", test3StringExpression.Text,
			        "String expression was not parsed correctly.");
			    Assert.IsNull(test3StringExpression.Left);
			    Assert.IsNull(test3StringExpression.Right);
			
			    //
			    // And right
			    //
			    OperatorExpression orExpression = operatorExpression.Right as OperatorExpression;
			    Assert.IsNotNull(orExpression, "Expected an operator expression.");
			    Assert.AreEqual(ExpressionOperator.Or, orExpression.Operator,
			        "Unexpected operator.");
			
			    //
			    // Or Left
			    //
			    OperatorExpression testExpression1 = orExpression.Left as OperatorExpression;
			    Assert.IsNotNull(testExpression1, "Expected an operator expression.");
			    Assert.AreEqual(ExpressionOperator.Equal, testExpression1.Operator,
			        "Unexpected operator.");
			
			    AttributeExpression test1AttributeExpression = testExpression1.Left as AttributeExpression;
			    Assert.IsNotNull(test1AttributeExpression, "Unexpected left node type.");
			    Assert.AreEqual(ElementAttribute.Name, test1AttributeExpression.ElementAttribute,
			        "Attribute expression was not parsed correctly.");
			    Assert.IsNull(test1AttributeExpression.Left);
			    Assert.IsNull(test1AttributeExpression.Right);
			
			    StringExpression test1StringExpression = testExpression1.Right as StringExpression;
			    Assert.IsNotNull(test1StringExpression, "Unexpected right node type.");
			    Assert.AreEqual("Test 1", test1StringExpression.Text,
			        "String expression was not parsed correctly.");
			    Assert.IsNull(test1StringExpression.Left);
			    Assert.IsNull(test1StringExpression.Right);
			
			    //
			    // Or Right
			    //
			    OperatorExpression testExpression2 = orExpression.Right as OperatorExpression;
			    Assert.IsNotNull(testExpression2, "Expected an operator expression.");
			    Assert.AreEqual(ExpressionOperator.Equal, testExpression2.Operator,
			        "Unexpected operator.");
			
			    AttributeExpression test2AttributeExpression = testExpression2.Left as AttributeExpression;
			    Assert.IsNotNull(test2AttributeExpression, "Unexpected left node type.");
			    Assert.AreEqual(ElementAttribute.Name, test2AttributeExpression.ElementAttribute,
			        "Attribute expression was not parsed correctly.");
			    Assert.IsNull(test2AttributeExpression.Left);
			    Assert.IsNull(test2AttributeExpression.Right);
			
			    StringExpression test2StringExpression = testExpression2.Right as StringExpression;
			    Assert.IsNotNull(test2StringExpression, "Unexpected right node type.");
			    Assert.AreEqual("Test 2", test2StringExpression.Text,
			        "String expression was not parsed correctly.");
			    Assert.IsNull(test2StringExpression.Left);
			    Assert.IsNull(test2StringExpression.Right);
			};
			
			string expressionText;
			expressionText = "($(Name) == 'Test 3') And (($(Name) == 'Test 1') Or ($(Name) == 'Test 2'))";
			testExpression(expressionText);
			
			expressionText = "$(Name) == 'Test 3' And ($(Name) == 'Test 1' Or $(Name) == 'Test 2')";
			//testExpression(expressionText);
		}		
		
		/// <summary>
		/// Tests parsing a complex expression.
		/// </summary>
		[Test]
		public void ParseComplexTest()		
		{
			string expression =
			    "$(Name) : 'Test' Or $(Access) == 'Protected' Or " +
			    "$(Access) == 'Private' And $(Name) : 'OrAnd' And $(Type) == 'int'";
			
			IConditionExpression conditionExpression = ConditionExpressionParser.Instance.Parse(
			    expression);
			Assert.IsNotNull(conditionExpression, "Expected an expression instance.");
			
			string expressionString = conditionExpression.ToString();
			Assert.AreEqual(
			    "((($(Name) : 'Test') Or ($(Access) == 'Protected')) Or " +
			    "((($(Access) == 'Private') And ($(Name) : 'OrAnd')) And ($(Type) == 'int')))",
			    expressionString,
			    "Unexpected parsed expression.");
		}		
		
		/// <summary>
		/// Tests parsing a null expression.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void ParseEmptyTest()		
		{
			IConditionExpression expression = ConditionExpressionParser.Instance.Parse(
			    string.Empty);
		}		
		
		/// <summary>
		/// Tests parsing an equals expression.
		/// </summary>
		[Test]
		public void ParseEqualsExpressionTest()		
		{
			string expressionText = "$(Name) == 'Test'";
			
			IConditionExpression expression = ConditionExpressionParser.Instance.Parse(
			    expressionText);
			Assert.IsNotNull(expression, "Expected an expression instance.");
			
			OperatorExpression operatorExpression = expression as OperatorExpression;
			Assert.IsNotNull(operatorExpression, "Expected an operator expression.");
			Assert.AreEqual(ExpressionOperator.Equal, operatorExpression.Operator,
			    "Unexpected operator.");
			
			AttributeExpression attributeExpression = operatorExpression.Left as AttributeExpression;
			Assert.IsNotNull(attributeExpression, "Unexpected left node type.");
			Assert.AreEqual(ElementAttribute.Name, attributeExpression.ElementAttribute,
			    "Attribute expression was not parsed correctly.");
			Assert.IsNull(attributeExpression.Left);
			Assert.IsNull(attributeExpression.Right);
			
			StringExpression stringExpression = operatorExpression.Right as StringExpression;
			Assert.IsNotNull(stringExpression, "Unexpected right node type.");
			Assert.AreEqual("Test", stringExpression.Text,
			    "String expression was not parsed correctly.");
			Assert.IsNull(stringExpression.Left);
			Assert.IsNull(stringExpression.Right);
		}		
		
		/// <summary>
		/// Tests parsing an invalid attribute name
		/// </summary>
		[Test]
		[ExpectedException(typeof(FormatException))]
		public void ParseInvalidAttributeTest()		
		{
			string expression = "$(Foo) : 'Test'";
			
			IConditionExpression conditionExpression = ConditionExpressionParser.Instance.Parse(
			    expression);
		}		
		
		/// <summary>
		/// Tests parsing an invalid expression
		/// </summary>
		[Test]
		[ExpectedException(typeof(FormatException))]
		public void ParseInvalidExpressionTest1()		
		{
			string expression = "$(Name) == 'Test' == $(Name)";
			
			IConditionExpression conditionExpression = ConditionExpressionParser.Instance.Parse(
			    expression);
		}		
		
		/// <summary>
		/// Tests parsing an invalid expression
		/// </summary>
		[Test]
		[ExpectedException(typeof(FormatException))]
		public void ParseInvalidExpressionTest2()		
		{
			string expression = "$(Name)";
			
			IConditionExpression conditionExpression = ConditionExpressionParser.Instance.Parse(
			    expression);
		}		
		
		/// <summary>
		/// Tests parsing an invalid expression
		/// </summary>
		[Test]
		[ExpectedException(typeof(FormatException))]
		public void ParseInvalidExpressionTest3()		
		{
			string expression = "$(Name) == 'Test' $(Name) == 'Foo'";
			
			IConditionExpression conditionExpression = ConditionExpressionParser.Instance.Parse(
			    expression);
		}		
		
		/// <summary>
		/// Tests parsing a null expression.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ParseNullTest()		
		{
			IConditionExpression expression = ConditionExpressionParser.Instance.Parse(
			    null);
		}		
		
		/// <summary>
		/// Tests parsing an expression.
		/// </summary>
		[Test]
		public void ParseOrAndExpressionTest1()		
		{
			Action<string> testExpression = delegate(string condition)
			{
			    IConditionExpression expression = ConditionExpressionParser.Instance.Parse(
			        condition);
			    Assert.IsNotNull(expression, "Expected an expression instance.");
			
			    OperatorExpression operatorExpression = expression as OperatorExpression;
			    Assert.IsNotNull(operatorExpression, "Expected an operator expression.");
			    Assert.AreEqual(ExpressionOperator.Or, operatorExpression.Operator,
			        "Unexpected operator.");
			
			    //
			    // Or left
			    //
			    OperatorExpression andExpression = operatorExpression.Left as OperatorExpression;
			    Assert.IsNotNull(andExpression, "Expected an operator expression.");
			    Assert.AreEqual(ExpressionOperator.And, andExpression.Operator,
			        "Unexpected operator.");
			
			    //
			    // And Left
			    //
			    OperatorExpression testExpression1 = andExpression.Left as OperatorExpression;
			    Assert.IsNotNull(testExpression1, "Expected an operator expression.");
			    Assert.AreEqual(ExpressionOperator.Equal, testExpression1.Operator,
			        "Unexpected operator.");
			
			    AttributeExpression test1AttributeExpression = testExpression1.Left as AttributeExpression;
			    Assert.IsNotNull(test1AttributeExpression, "Unexpected left node type.");
			    Assert.AreEqual(ElementAttribute.Name, test1AttributeExpression.ElementAttribute,
			        "Attribute expression was not parsed correctly.");
			    Assert.IsNull(test1AttributeExpression.Left);
			    Assert.IsNull(test1AttributeExpression.Right);
			
			    StringExpression test1StringExpression = testExpression1.Right as StringExpression;
			    Assert.IsNotNull(test1StringExpression, "Unexpected right node type.");
			    Assert.AreEqual("Test 1", test1StringExpression.Text,
			        "String expression was not parsed correctly.");
			    Assert.IsNull(test1StringExpression.Left);
			    Assert.IsNull(test1StringExpression.Right);
			
			    //
			    // And Right
			    //
			    OperatorExpression testExpression2 = andExpression.Right as OperatorExpression;
			    Assert.IsNotNull(testExpression2, "Expected an operator expression.");
			    Assert.AreEqual(ExpressionOperator.Equal, testExpression2.Operator,
			        "Unexpected operator.");
			
			    AttributeExpression test2AttributeExpression = testExpression2.Left as AttributeExpression;
			    Assert.IsNotNull(test2AttributeExpression, "Unexpected left node type.");
			    Assert.AreEqual(ElementAttribute.Name, test2AttributeExpression.ElementAttribute,
			        "Attribute expression was not parsed correctly.");
			    Assert.IsNull(test2AttributeExpression.Left);
			    Assert.IsNull(test2AttributeExpression.Right);
			
			    StringExpression test2StringExpression = testExpression2.Right as StringExpression;
			    Assert.IsNotNull(test2StringExpression, "Unexpected right node type.");
			    Assert.AreEqual("Test 2", test2StringExpression.Text,
			        "String expression was not parsed correctly.");
			    Assert.IsNull(test2StringExpression.Left);
			    Assert.IsNull(test2StringExpression.Right);
			
			    //
			    // Or right
			    //
			    OperatorExpression testExpression3 = operatorExpression.Right as OperatorExpression;
			    Assert.IsNotNull(testExpression3, "Expected an operator expression.");
			    Assert.AreEqual(ExpressionOperator.Equal, testExpression3.Operator,
			        "Unexpected operator.");
			
			    AttributeExpression test3AttributeExpression = testExpression3.Left as AttributeExpression;
			    Assert.IsNotNull(test3AttributeExpression, "Unexpected left node type.");
			    Assert.AreEqual(ElementAttribute.Name, test3AttributeExpression.ElementAttribute,
			        "Attribute expression was not parsed correctly.");
			    Assert.IsNull(test3AttributeExpression.Left);
			    Assert.IsNull(test3AttributeExpression.Right);
			
			    StringExpression test3StringExpression = testExpression3.Right as StringExpression;
			    Assert.IsNotNull(test3StringExpression, "Unexpected right node type.");
			    Assert.AreEqual("Test 3", test3StringExpression.Text,
			        "String expression was not parsed correctly.");
			    Assert.IsNull(test3StringExpression.Left);
			    Assert.IsNull(test3StringExpression.Right);
			};
			
			string expressionText;
			expressionText = "(($(Name) == 'Test 1') And ($(Name) == 'Test 2')) Or ($(Name) == 'Test 3')";
			testExpression(expressionText);
			
			expressionText = "$(Name) == 'Test 1' And $(Name) == 'Test 2' Or $(Name) == 'Test 3'";
			testExpression(expressionText);
		}		
		
		/// <summary>
		/// Tests parsing an expression.
		/// </summary>
		[Test]
		public void ParseOrAndExpressionTest2()		
		{
			Action<string> testExpression = delegate(string condition)
			{
			    IConditionExpression expression = ConditionExpressionParser.Instance.Parse(
			        condition);
			    Assert.IsNotNull(expression, "Expected an expression instance.");
			
			    OperatorExpression operatorExpression = expression as OperatorExpression;
			    Assert.IsNotNull(operatorExpression, "Expected an operator expression.");
			    Assert.AreEqual(ExpressionOperator.Or, operatorExpression.Operator,
			        "Unexpected operator.");
			
			    //
			    // Or left
			    //
			    OperatorExpression testExpression1 = operatorExpression.Left as OperatorExpression;
			    Assert.IsNotNull(testExpression1, "Expected an operator expression.");
			    Assert.AreEqual(ExpressionOperator.Equal, testExpression1.Operator,
			        "Unexpected operator.");
			
			    AttributeExpression test1AttributeExpression = testExpression1.Left as AttributeExpression;
			    Assert.IsNotNull(test1AttributeExpression, "Unexpected left node type.");
			    Assert.AreEqual(ElementAttribute.Name, test1AttributeExpression.ElementAttribute,
			        "Attribute expression was not parsed correctly.");
			    Assert.IsNull(test1AttributeExpression.Left);
			    Assert.IsNull(test1AttributeExpression.Right);
			
			    StringExpression test1StringExpression = testExpression1.Right as StringExpression;
			    Assert.IsNotNull(test1StringExpression, "Unexpected right node type.");
			    Assert.AreEqual("Test 1", test1StringExpression.Text,
			        "String expression was not parsed correctly.");
			    Assert.IsNull(test1StringExpression.Left);
			    Assert.IsNull(test1StringExpression.Right);
			
			    //
			    // Or right
			    //
			    OperatorExpression andExpression = operatorExpression.Right as OperatorExpression;
			    Assert.IsNotNull(andExpression, "Expected an operator expression.");
			    Assert.AreEqual(ExpressionOperator.And, andExpression.Operator,
			        "Unexpected operator.");
			
			    //
			    // And Left
			    //
			    OperatorExpression testExpression2 = andExpression.Left as OperatorExpression;
			    Assert.IsNotNull(testExpression2, "Expected an operator expression.");
			    Assert.AreEqual(ExpressionOperator.Equal, testExpression2.Operator,
			        "Unexpected operator.");
			
			    AttributeExpression test2AttributeExpression = testExpression2.Left as AttributeExpression;
			    Assert.IsNotNull(test2AttributeExpression, "Unexpected left node type.");
			    Assert.AreEqual(ElementAttribute.Name, test2AttributeExpression.ElementAttribute,
			        "Attribute expression was not parsed correctly.");
			    Assert.IsNull(test2AttributeExpression.Left);
			    Assert.IsNull(test2AttributeExpression.Right);
			
			    StringExpression test2StringExpression = testExpression2.Right as StringExpression;
			    Assert.IsNotNull(test2StringExpression, "Unexpected right node type.");
			    Assert.AreEqual("Test 2", test2StringExpression.Text,
			        "String expression was not parsed correctly.");
			    Assert.IsNull(test2StringExpression.Left);
			    Assert.IsNull(test2StringExpression.Right);
			
			    //
			    // And Right
			    //
			    OperatorExpression testExpression3 = andExpression.Right as OperatorExpression;
			    Assert.IsNotNull(testExpression3, "Expected an operator expression.");
			    Assert.AreEqual(ExpressionOperator.Equal, testExpression3.Operator,
			        "Unexpected operator.");
			
			    AttributeExpression test3AttributeExpression = testExpression3.Left as AttributeExpression;
			    Assert.IsNotNull(test3AttributeExpression, "Unexpected left node type.");
			    Assert.AreEqual(ElementAttribute.Name, test3AttributeExpression.ElementAttribute,
			        "Attribute expression was not parsed correctly.");
			    Assert.IsNull(test3AttributeExpression.Left);
			    Assert.IsNull(test3AttributeExpression.Right);
			
			    StringExpression test3StringExpression = testExpression3.Right as StringExpression;
			    Assert.IsNotNull(test3StringExpression, "Unexpected right node type.");
			    Assert.AreEqual("Test 3", test3StringExpression.Text,
			        "String expression was not parsed correctly.");
			    Assert.IsNull(test3StringExpression.Left);
			    Assert.IsNull(test3StringExpression.Right);
			};
			
			string expressionText;
			expressionText = "(($(Name) == 'Test 1') Or (($(Name) == 'Test 2') And ($(Name) == 'Test 3'))";
			testExpression(expressionText);
			
			expressionText = "$(Name) == 'Test 1' Or $(Name) == 'Test 2' And $(Name) == 'Test 3'";
			testExpression(expressionText);
		}		
		
		/// <summary>
		/// Tests parsing an OR expression.
		/// </summary>
		[Test]
		public void ParseOrExpressionTest()		
		{
			Action<string> testExpression = delegate(string condition)
			{
			    IConditionExpression expression = ConditionExpressionParser.Instance.Parse(
			        condition);
			    Assert.IsNotNull(expression, "Expected an expression instance.");
			
			    OperatorExpression operatorExpression = expression as OperatorExpression;
			    Assert.IsNotNull(operatorExpression, "Expected an operator expression.");
			    Assert.AreEqual(ExpressionOperator.Or, operatorExpression.Operator,
			        "Unexpected operator.");
			
			    //
			    // Left
			    //
			    OperatorExpression leftExpression = expression.Left as OperatorExpression;
			    Assert.IsNotNull(leftExpression, "Expected an operator expression.");
			    Assert.AreEqual(ExpressionOperator.Equal, leftExpression.Operator,
			        "Unexpected operator.");
			
			    AttributeExpression leftAttributeExpression = leftExpression.Left as AttributeExpression;
			    Assert.IsNotNull(leftAttributeExpression, "Unexpected left node type.");
			    Assert.AreEqual(ElementAttribute.Name, leftAttributeExpression.ElementAttribute,
			        "Attribute expression was not parsed correctly.");
			    Assert.IsNull(leftAttributeExpression.Left);
			    Assert.IsNull(leftAttributeExpression.Right);
			
			    StringExpression leftStringExpression = leftExpression.Right as StringExpression;
			    Assert.IsNotNull(leftStringExpression, "Unexpected right node type.");
			    Assert.AreEqual("Test 1", leftStringExpression.Text,
			        "String expression was not parsed correctly.");
			    Assert.IsNull(leftStringExpression.Left);
			    Assert.IsNull(leftStringExpression.Right);
			
			    //
			    // Right
			    //
			    OperatorExpression rightExpression = expression.Right as OperatorExpression;
			    Assert.IsNotNull(rightExpression, "Expected an operator expression.");
			    Assert.AreEqual(ExpressionOperator.Equal, rightExpression.Operator,
			        "Unexpected operator.");
			
			    AttributeExpression rightAttributeExpression = rightExpression.Left as AttributeExpression;
			    Assert.IsNotNull(rightAttributeExpression, "Unexpected left node type.");
			    Assert.AreEqual(ElementAttribute.Name, rightAttributeExpression.ElementAttribute,
			        "Attribute expression was not parsed correctly.");
			    Assert.IsNull(rightAttributeExpression.Left);
			    Assert.IsNull(rightAttributeExpression.Right);
			
			    StringExpression rightStringExpression = rightExpression.Right as StringExpression;
			    Assert.IsNotNull(rightStringExpression, "Unexpected right node type.");
			    Assert.AreEqual("Test 2", rightStringExpression.Text,
			        "String expression was not parsed correctly.");
			    Assert.IsNull(rightStringExpression.Left);
			    Assert.IsNull(rightStringExpression.Right);
			};
			
			string expressionText;
			expressionText = "($(Name) == 'Test 1') Or ($(Name) == 'Test 2')";
			testExpression(expressionText);
			
			expressionText = "$(Name) == 'Test 1' Or $(Name) == 'Test 2'";
			testExpression(expressionText);
		}		
		
		#endregion Public Methods
	}
}