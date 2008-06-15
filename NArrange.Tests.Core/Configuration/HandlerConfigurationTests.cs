using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NArrange.Core.Configuration;

namespace NArrange.Tests.Core.Configuration
{
	/// <summary>
	/// Test fixture for the HandlerConfiguration class
	/// </summary>
	[TestFixture]
	public class HandlerConfigurationTests
	{
		#region Public Methods

		/// <summary>
		/// Tests the ToString method
		/// </summary>
		[Test]
		public void ToStringTest()
		{
			SourceHandlerConfiguration handlerConfiguration = new SourceHandlerConfiguration();
			handlerConfiguration.Language = "TestLanguage";

			string str = handlerConfiguration.ToString();
			Assert.AreEqual("Source Handler: TestLanguage", str,
			    "Unexpected string representation.");
		}

		#endregion Public Methods
	}
}