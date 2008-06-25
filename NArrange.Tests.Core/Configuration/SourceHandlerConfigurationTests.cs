using NArrange.Core.Configuration;

using NUnit.Framework;

namespace NArrange.Tests.Core.Configuration
{
	/// <summary>
	/// Test fixture for the SourceHandlerConfiguration class.
	/// </summary>
	[TestFixture]
	public class SourceHandlerConfigurationTests
	{
		#region Public Methods

		/// <summary>
		/// Tests the ToString method.
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