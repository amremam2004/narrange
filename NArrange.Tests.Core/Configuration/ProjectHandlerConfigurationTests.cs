using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NArrange.Core.Configuration;

namespace NArrange.Tests.Core.Configuration
{
	/// <summary>
	/// Test fixture for the ProjectHandlerConfiguration class.
	/// </summary>
	[TestFixture]
	public class ProjectHandlerConfigurationTests
	{
		#region Public Methods

		/// <summary>
		/// Tests the ToString method.
		/// </summary>
		[Test]
		public void ToStringTest()
		{
			ProjectHandlerConfiguration handlerConfiguration = new ProjectHandlerConfiguration();
			handlerConfiguration.ParserType = "TestAssembly.TestParser";

			string str = handlerConfiguration.ToString();
			Assert.AreEqual("Project Handler: TestAssembly.TestParser", str,
			    "Unexpected string representation.");
		}

		#endregion Public Methods
	}
}