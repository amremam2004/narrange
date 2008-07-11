using NArrange.Core.Configuration;

using NUnit.Framework;

namespace NArrange.Tests.Core.Configuration
{
	/// <summary>
	/// Test fixture for the RegionFormattingConfiguration class.
	/// </summary>
	[TestFixture]
	public class RegionFormattingConfigurationTests
	{
		#region Public Methods

		/// <summary>
		/// Tests the ICloneable implementation.
		/// </summary>
		[Test]
		public void CloneTest()
		{
			RegionFormattingConfiguration regionsConfiguration = new RegionFormattingConfiguration();
			regionsConfiguration.EndRegionNameEnabled = true;

			RegionFormattingConfiguration clone = regionsConfiguration.Clone() as RegionFormattingConfiguration;
			Assert.IsNotNull(clone, "Clone did not return a valid instance.");

			Assert.AreEqual(regionsConfiguration.EndRegionNameEnabled, clone.EndRegionNameEnabled);
		}

		/// <summary>
		/// Tests the creation of a new RegionsConfiguration.
		/// </summary>
		[Test]
		public void CreateTest()
		{
			RegionFormattingConfiguration regionsConfiguration = new RegionFormattingConfiguration();

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
			RegionFormattingConfiguration regionsConfiguration = new RegionFormattingConfiguration();
			regionsConfiguration.EndRegionNameEnabled = true;

			string str = regionsConfiguration.ToString();

			Assert.AreEqual("Regions: EndRegionNameEnabled - True", str,
			    "Unexpected string representation.");
		}

		#endregion Public Methods
	}
}