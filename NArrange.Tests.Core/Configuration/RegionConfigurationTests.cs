using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NArrange.Core;
using NArrange.Core.Configuration;

namespace NArrange.Tests.Core.Configuration
{
	/// <summary>
	/// Test fixture for the RegionConfiguration class
	/// </summary>
	[TestFixture]
	public class RegionConfigurationTests
	{
		#region Public Methods

		/// <summary>
		/// Tests the creation of a new RegionConfiguration
		/// </summary>
		[Test]
		public void CreateTest()
		{
			RegionConfiguration regionConfiguration = new RegionConfiguration();
			
			//
			// Verify default state
			//
			Assert.IsNull(regionConfiguration.Name,
			    "Unexpected default value for Name.");
			Assert.IsNotNull(regionConfiguration.Elements,
			    "Elements collection should not be null.");
			Assert.AreEqual(0, regionConfiguration.Elements.Count,
			    "Elements collection should be empty.");
		}

		/// <summary>
		/// Tests the ToString method
		/// </summary>
		[Test]
		public void ToStringTest()
		{
			RegionConfiguration regionConfiguration = new RegionConfiguration();
			regionConfiguration.Name = "Test Region";
			
			string str = regionConfiguration.ToString();
			
			Assert.AreEqual("Region: Test Region", str,
			    "Unexpected string representation.");
		}

		#endregion Public Methods
	}
}