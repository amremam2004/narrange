using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NArrange.Core;
using NArrange.Core.CodeElements;
using NArrange.Core.Configuration;

namespace NArrange.Tests.Core
{
	/// <summary>
	/// Test fixture for the ConditionExpressionEvaluator class
	/// </summary>
	[TestFixture]
	public class ConditionExpressionEvaluatorTests
	{
		#region Public Methods

		/// <summary>
		/// Tests the Evaluate method with an And expression
		/// </summary>
		[Test]
		public void EvaluateAndTest()
		{
			IConditionExpression expression =
			   new OperatorExpression(ExpressionOperator.And,
			   new OperatorExpression(ExpressionOperator.Equal,
			   new AttributeExpression(ElementAttribute.Name), new StringExpression("Test")),
			   new OperatorExpression(ExpressionOperator.Equal,
			   new AttributeExpression(ElementAttribute.Access), new StringExpression("Protected")));
			
			FieldElement element = new FieldElement();
			element.Name = "Test";
			element.Access = CodeAccess.Protected;
			
			bool result = ConditionExpressionEvaluator.Instance.Evaluate(
			    expression, element);
			Assert.IsTrue(result, "Unexpected expression evaluation result.");
			
			element.Name = "Foo";
			result = ConditionExpressionEvaluator.Instance.Evaluate(
			    expression, element);
			Assert.IsFalse(result, "Unexpected expression evaluation result.");
			
			element.Name = "Test";
			element.Access = CodeAccess.Private;
			result = ConditionExpressionEvaluator.Instance.Evaluate(
			    expression, element);
			Assert.IsFalse(result, "Unexpected expression evaluation result.");
		}

		/// <summary>
		/// Tests the Evaluate method with a Contains expression
		/// </summary>
		[Test]
		public void EvaluateContainsTest()
		{
			IConditionExpression expression = new OperatorExpression(
			   ExpressionOperator.Contains, new AttributeExpression(ElementAttribute.Name),
			   new StringExpression("Test"));
			
			FieldElement element = new FieldElement();
			element.Name = "Test";
			
			bool result = ConditionExpressionEvaluator.Instance.Evaluate(
			    expression, element);
			Assert.IsTrue(result, "Unexpected expression evaluation result.");
			
			element.Name = "OnTest1";
			result = ConditionExpressionEvaluator.Instance.Evaluate(
			    expression, element);
			Assert.IsTrue(result, "Unexpected expression evaluation result.");
			
			element.Name = "Foo";
			result = ConditionExpressionEvaluator.Instance.Evaluate(
			    expression, element);
			Assert.IsFalse(result, "Unexpected expression evaluation result.");
		}

		/// <summary>
		/// Tests the Evaluate method with an Equal expression
		/// </summary>
		[Test]
		public void EvaluateEqualTest()
		{
			IConditionExpression expression = new OperatorExpression(
			   ExpressionOperator.Equal, new AttributeExpression(ElementAttribute.Name),
			   new StringExpression("Test"));
			
			FieldElement element = new FieldElement();
			element.Name = "Test";
			
			bool result = ConditionExpressionEvaluator.Instance.Evaluate(
			    expression, element);
			Assert.IsTrue(result, "Unexpected expression evaluation result.");
			
			element.Name = "Test1";
			result = ConditionExpressionEvaluator.Instance.Evaluate(
			    expression, element);
			Assert.IsFalse(result, "Unexpected expression evaluation result.");
		}

		/// <summary>
		/// Tests the Evaluate method with a an operator element that has an 
		/// unknown operator type.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void EvaluateInvalidOperatorTest()
		{
			IConditionExpression expression = new OperatorExpression(
			    (ExpressionOperator)int.MinValue, new AttributeExpression(ElementAttribute.Name),
			    new StringExpression("Test"));
			
			FieldElement element = new FieldElement();
			element.Name = "Test";
			
			bool result = ConditionExpressionEvaluator.Instance.Evaluate(
			    expression, element);
		}

		/// <summary>
		/// Tests the Evaluate method with a null element
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void EvaluateNullElementTest()
		{
			IConditionExpression expression = new OperatorExpression(
			    ExpressionOperator.Equal, new AttributeExpression(ElementAttribute.Name),
			    new StringExpression("Test"));
			
			bool result = ConditionExpressionEvaluator.Instance.Evaluate(
			    expression, null);
		}

		/// <summary>
		/// Tests the Evaluate method with a null expression
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void EvaluateNullExpressionTest()
		{
			bool result = ConditionExpressionEvaluator.Instance.Evaluate(
			    null, new FieldElement());
		}

		/// <summary>
		/// Tests the Evaluate method with an Or expression
		/// </summary>
		[Test]
		public void EvaluateOrTest()
		{
			IConditionExpression expression =
			   new OperatorExpression(ExpressionOperator.Or,
			   new OperatorExpression(ExpressionOperator.Equal,
			   new AttributeExpression(ElementAttribute.Name), new StringExpression("Test")),
			   new OperatorExpression(ExpressionOperator.Equal,
			   new AttributeExpression(ElementAttribute.Access), new StringExpression("Protected")));
			
			FieldElement element = new FieldElement();
			element.Name = "Test";
			element.Access = CodeAccess.Protected;
			
			bool result = ConditionExpressionEvaluator.Instance.Evaluate(
			    expression, element);
			Assert.IsTrue(result, "Unexpected expression evaluation result.");
			
			element.Name = "Foo";
			result = ConditionExpressionEvaluator.Instance.Evaluate(
			    expression, element);
			Assert.IsTrue(result, "Unexpected expression evaluation result.");
			
			element.Name = "Test";
			element.Access = CodeAccess.Private;
			result = ConditionExpressionEvaluator.Instance.Evaluate(
			    expression, element);
			Assert.IsTrue(result, "Unexpected expression evaluation result.");
			
			element.Name = "Foo";
			result = ConditionExpressionEvaluator.Instance.Evaluate(
			    expression, element);
			Assert.IsFalse(result, "Unexpected expression evaluation result.");
		}

		#endregion Public Methods
	}
}