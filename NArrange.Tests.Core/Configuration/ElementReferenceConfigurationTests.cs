using NArrange.Core.Configuration;
using NUnit.Framework;

namespace NArrange.Tests.Core.Configuration
{
	/// <summary>
	/// Test fixture for the ElementReferenceConfiguration class.
	/// </summary>
	[TestFixture]
	public class ElementReferenceConfigurationTests
	{
		#region Public Methods

		/// <summary>
		/// Tests the creation of a new ElementReferenceConfiguration.
		/// </summary>
		[Test]
		public void CreateTest()
		{
			ElementReferenceConfiguration ElementReferenceConfiguration = new ElementReferenceConfiguration();

			//
			// Verify default state
			//
			Assert.IsNull(ElementReferenceConfiguration.Id,
			    "Unexpected default value for Id.");
		}

		/// <summary>
		/// Tests the ToString method
		/// </summary>
		[Test]
		public void ToStringTest()
		{
			ElementReferenceConfiguration elementReference = new ElementReferenceConfiguration();
			elementReference.Id = "SomeId";

			string str = elementReference.ToString();

			Assert.AreEqual("Element Reference: SomeId", str,
			    "Unexpected string representation.");
		}

		#endregion Public Methods
	}
}