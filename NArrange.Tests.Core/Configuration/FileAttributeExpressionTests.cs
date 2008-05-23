using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NArrange.Core;
using NArrange.Core.Configuration;

namespace NArrange.Tests.Core.Configuration
{
	/// <summary>
	/// Test fixture for the FileAttributeExpression class.
	/// </summary>
	[TestFixture]
	public class FileAttributeExpressionTests
	{
		#region Public Methods

		/// <summary>
		/// Tests the ToString method.
		/// </summary>
		[Test]
		public void ToStringTest()
		{
			FileAttributeExpression expression = new FileAttributeExpression(FileAttributeType.Path);

			Assert.AreEqual("$(File.Path)", expression.ToString(),
				"Unexpected string representation.");
		}

		#endregion Public Methods
	}
}