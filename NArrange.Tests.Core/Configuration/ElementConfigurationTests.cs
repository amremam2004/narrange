using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NArrange.Core;
using NArrange.Core.Configuration;

namespace NArrange.Tests.Core.Configuration
{
	/// <summary>
	/// Test fixture for the ElementConfiguration class
	/// </summary>
	[TestFixture]
	public class ElementConfigurationTests	
	{
		#region Public Methods
		
		/// <summary>
		/// Tests the creation of a new ElementConfiguration
		/// </summary>
		[Test]
		public void CreateTest()		
		{
			ElementConfiguration elementConfiguration = new ElementConfiguration();
			
			//
			// Verify default state
			//
			Assert.AreEqual(ElementType.NotSpecified, elementConfiguration.ElementType,
			    "Unexpected default value for ElementType.");
			Assert.IsNull(elementConfiguration.FilterBy,
			    "Unexpected default value for FilterBy.");
			Assert.IsNull(elementConfiguration.GroupBy,
			    "Unexpected default value for GroupBy.");
			Assert.IsNull(elementConfiguration.SortBy,
			    "Unexpected default value for SortBy.");
			Assert.IsNotNull(elementConfiguration.Elements,
			    "Elements collection should not be null.");
			Assert.AreEqual(0, elementConfiguration.Elements.Count,
			    "Elements collection should be empty.");
		}		
		
		/// <summary>
		/// Tests the ToString method
		/// </summary>
		[Test]
		public void ToStringTest()		
		{
			ElementConfiguration elementConfiguration = new ElementConfiguration();
			elementConfiguration.ElementType = ElementType.Method;
			
			string str = elementConfiguration.ToString();
			
			Assert.AreEqual("Type: Method", str,
			    "Unexpected string representation.");
		}		
		
		#endregion Public Methods

	}
}