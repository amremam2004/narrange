using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NArrange.Core;
using NArrange.Core.Configuration;

namespace NArrange.Tests.Core.Configuration
{
	/// <summary>
	/// Test fixture for the Operator expression class.
	/// </summary>
	[TestFixture]
	public class OperatorExpressionTests
	{
		#region Public Methods

		/// <summary>
		/// Gets the string representation of the operator expression with an invalid operator type.
		/// </summary>
		[Test]
		public void ToStringInvalidOperatorTest()
		{
			AttributeExpression attributeExpression = new AttributeExpression(ElementAttributeType.Name);
			StringExpression stringExpression = new StringExpression("Test");
			OperatorExpression operatorExpression = new OperatorExpression((ExpressionOperator)int.MinValue,
			    attributeExpression, stringExpression);

			Assert.AreEqual(string.Format("($(Element.Name) {0} 'Test')", int.MinValue), operatorExpression.ToString());
		}

		/// <summary>
		/// Gets the string representation of the operator expression
		/// </summary>
		[Test]
		public void ToStringTest()
		{
			AttributeExpression attributeExpression = new AttributeExpression(ElementAttributeType.Name);
			StringExpression stringExpression = new StringExpression("Test");
			OperatorExpression operatorExpression = new OperatorExpression(ExpressionOperator.Equal,
			    attributeExpression, stringExpression);

			Assert.AreEqual("($(Element.Name) == 'Test')", operatorExpression.ToString());
		}

		#endregion Public Methods
	}
}