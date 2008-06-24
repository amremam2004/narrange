using NArrange.Core.Configuration;
using NUnit.Framework;

namespace NArrange.Tests.Core.Configuration
{
	/// <summary>
	/// Test fixture for the RegionsConfiguration class.
	/// </summary>
	[TestFixture]
	public class RegionsConfigurationTests
	{
		#region Public Methods

		/// <summary>
		/// Tests the ICloneable implementation.
		/// </summary>
		[Test]
		public void CloneTest()
		{
			RegionsConfiguration regionsConfiguration = new RegionsConfiguration();
			regionsConfiguration.EndRegionNameEnabled = true;

			RegionsConfiguration clone = regionsConfiguration.Clone() as RegionsConfiguration;
			Assert.IsNotNull(clone, "Clone did not return a valid instance.");

			Assert.AreEqual(regionsConfiguration.EndRegionNameEnabled, clone.EndRegionNameEnabled);
		}

		/// <summary>
		/// Tests the creation of a new RegionsConfiguration.
		/// </summary>
		[Test]
		public void CreateTest()
		{
			RegionsConfiguration regionsConfiguration = new RegionsConfiguration();

			//
			// Verify default state
			//
			Assert.IsTrue(regionsConfiguration.EndRegionNameEnabled,
			    "Unexpected default value for EndRegionNameEnabled.");
		}

		/// <summary>
		/// Tests the ToString method.
		/// </summary>
		[Test]
		public void ToStringTest()
		{
			RegionsConfiguration regionsConfiguration = new RegionsConfiguration();
			regionsConfiguration.EndRegionNameEnabled = true;

			string str = regionsConfiguration.ToString();

			Assert.AreEqual("Regions: EndRegionNameEnabled - True", str,
			    "Unexpected string representation.");
		}

		#endregion Public Methods
	}
}