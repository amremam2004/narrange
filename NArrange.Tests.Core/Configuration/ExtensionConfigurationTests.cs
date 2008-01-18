using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NArrange.Core;
using NArrange.Core.Configuration;

namespace NArrange.Tests.Core.Configuration
{
	/// <summary>
	/// Test fixture for the ExtensionConfiguration class
	/// </summary>
	[TestFixture]
	public class ExtensionConfigurationTests	
	{
		#region Public Methods
		
		/// <summary>
		/// Tests the ICloneable implementation
		/// </summary>
		[Test]
		public void CloneTest()		
		{
			ExtensionConfiguration extensionConfiguration = new ExtensionConfiguration();
			extensionConfiguration.Name = "cs";
			
			ExtensionConfiguration clone = extensionConfiguration.Clone() as ExtensionConfiguration;
			Assert.IsNotNull(clone, "Clone did not return a valid instance.");
			
			Assert.AreEqual(extensionConfiguration.Name, clone.Name);
		}		
		
		/// <summary>
		/// Tests the creation of a new ExtensionConfiguration
		/// </summary>
		[Test]
		public void CreateTest()		
		{
			ExtensionConfiguration extensionConfiguration = new ExtensionConfiguration();
			
			//
			// Verify default state
			//
			Assert.IsNull(extensionConfiguration.Name,
			    "Unexpected default value for Name.");
		}		
		
		/// <summary>
		/// Tests the ToString method
		/// </summary>
		[Test]
		public void ToStringTest()		
		{
			ExtensionConfiguration extensionConfiguration = new ExtensionConfiguration();
			extensionConfiguration.Name = "cs";
			
			string str = extensionConfiguration.ToString();
			
			Assert.AreEqual("Extension: cs", str,
			    "Unexpected string representation.");
		}		
		
		#endregion Public Methods
	}
}