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
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ToStringInvalidOperatorTest()
		{
			AttributeExpression attributeExpression = new AttributeExpression(ElementAttribute.Name);
			StringExpression stringExpression = new StringExpression("Test");
			OperatorExpression operatorExpression = new OperatorExpression((ExpressionOperator)int.MinValue,
			    attributeExpression, stringExpression);

			string str = operatorExpression.ToString();
		}

		/// <summary>
		/// Gets the string representation of the operator expression
		/// </summary>
		public void ToStringTest()
		{
			AttributeExpression attributeExpression = new AttributeExpression(ElementAttribute.Name);
			StringExpression stringExpression = new StringExpression("Test");
			OperatorExpression operatorExpression = new OperatorExpression(ExpressionOperator.Equal,
			    attributeExpression, stringExpression);

			Assert.AreEqual("$(Name) == 'Test'", operatorExpression.ToString());
		}

		#endregion Public Methods
	}
}