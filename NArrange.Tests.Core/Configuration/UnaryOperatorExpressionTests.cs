using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NArrange.Core;
using NArrange.Core.Configuration;

namespace NArrange.Tests.Core.Configuration
{
	/// <summary>
	/// Test fixture for the unary operator expression class.
	/// </summary>
	[TestFixture]
	public class UnaryOperatorExpressionTests
	{
		#region Public Methods

		/// <summary>
		/// Gets the string representation of the operator expression with an invalid operator type.
		/// </summary>
		[Test]
		public void ToStringInvalidOperatorTest()
		{
			ElementAttributeExpression attributeExpression = new ElementAttributeExpression(ElementAttributeType.Name);
			StringExpression stringExpression = new StringExpression("Test");
			BinaryOperatorExpression equalsExpression = new BinaryOperatorExpression(BinaryExpressionOperator.Equal,
			    attributeExpression, stringExpression);
			UnaryOperatorExpression operatorExpression = new UnaryOperatorExpression((UnaryExpressionOperator)int.MinValue,
			    equalsExpression);

			Assert.AreEqual(string.Format("{0}(($(Element.Name) == 'Test'))", int.MinValue), operatorExpression.ToString());
		}

		/// <summary>
		/// Gets the string representation of the operator expression.
		/// </summary>
		[Test]
		public void ToStringTest()
		{
			ElementAttributeExpression attributeExpression = new ElementAttributeExpression(ElementAttributeType.Name);
			StringExpression stringExpression = new StringExpression("Test");
			BinaryOperatorExpression equalsExpression = new BinaryOperatorExpression(BinaryExpressionOperator.Equal,
			    attributeExpression, stringExpression);
			UnaryOperatorExpression operatorExpression = new UnaryOperatorExpression(UnaryExpressionOperator.Negate,
			    equalsExpression);

			Assert.AreEqual("!(($(Element.Name) == 'Test'))", operatorExpression.ToString());
		}

		#endregion Public Methods
	}
}