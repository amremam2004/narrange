using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using NUnit.Framework;

using NArrange.Core;
using NArrange.Core.CodeElements;
using NArrange.Core.Configuration;

namespace NArrange.Tests.Core
{
	/// <summary>
	/// Test fixture for the RegionedInserter class.
	/// </summary>
	[TestFixture]
	public class RegionedInserterTests
	{
		#region Public Methods

		/// <summary>
		/// Tests the creation of a RegionedInserter instance.
		/// </summary>
		[Test]
		public void CreateTest()
		{
			RegionConfiguration regionConfiguration = new RegionConfiguration();
			regionConfiguration.Name = "Test Region";
			
			ElementConfiguration elementConfiguration = new ElementConfiguration();
			elementConfiguration.ElementType = ElementType.Type;
			
			RegionedInserter regionedInserter = new RegionedInserter(regionConfiguration, elementConfiguration);
		}

		/// <summary>
		/// Test construction with a null configuration
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CreateWithNullParentConfigurationTest()
		{
			RegionedInserter regionedInserter = new RegionedInserter(new RegionConfiguration(), null);
		}

		/// <summary>
		/// Test construction with a null configuration
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CreateWithNullRegionConfigurationTest()
		{
			RegionedInserter regionedInserter = new RegionedInserter(null, new ElementConfiguration());
		}

		/// <summary>
		/// Tests inserting elements.
		/// </summary>
		[Test]
		public void InsertTest()
		{
			RegionConfiguration regionConfiguration = new RegionConfiguration();
			regionConfiguration.Name = "Test Region";
			
			ElementConfiguration typeConfiguration = new ElementConfiguration();
			typeConfiguration.ElementType = ElementType.Type;
			
			RegionedInserter regionedInserter = new RegionedInserter(
			    regionConfiguration, typeConfiguration);
			
			//
			// Create a parent element
			//
			GroupElement groupElement = new GroupElement();
			Assert.AreEqual(0, groupElement.Children.Count,
			    "Parent element should not have any children.");
			
			//
			// With no criteria specified, elements should just be inserted 
			// at the end of the collection.
			//
			
			FieldElement field1 = new FieldElement();
			field1.Name = "zooField";
			regionedInserter.InsertElement(groupElement, field1);
			Assert.AreEqual(1, groupElement.Children.Count,
			    "Region element was not inserted into the parent.");
			Assert.IsTrue(groupElement.Children[0] is RegionElement,
			    "Region element was not inserted into the parent.");
			Assert.AreEqual("Test Region", groupElement.Children[0].Name,
			    "Region element was not inserted into the parent.");
			Assert.AreEqual(1, groupElement.Children[0].Children.Count,
			    "Element was not inserted into the parent.");
			Assert.AreEqual(0, groupElement.Children[0].Children.IndexOf(field1),
			    "Element was not inserted at the correct index.");
			
			FieldElement field2 = new FieldElement();
			field2.Name = "newField";
			regionedInserter.InsertElement(groupElement, field2);
			Assert.AreEqual(1, groupElement.Children.Count,
			    "Region element was not inserted into the parent.");
			Assert.IsTrue(groupElement.Children[0] is RegionElement,
			    "Region element was not inserted into the parent.");
			Assert.AreEqual("Test Region", groupElement.Children[0].Name,
			    "Region element was not inserted into the parent.");
			Assert.AreEqual(2, groupElement.Children[0].Children.Count,
			    "Element was not inserted into the parent.");
			Assert.AreEqual(0, groupElement.Children[0].Children.IndexOf(field1),
			    "Element is not at the correct index.");
			Assert.AreEqual(1, groupElement.Children[0].Children.IndexOf(field2),
			    "Element is not at the correct index.");
			
			FieldElement field3 = new FieldElement();
			field3.Name = "booField";
			regionedInserter.InsertElement(groupElement, field3);
			Assert.AreEqual(1, groupElement.Children.Count,
			    "Region element was not inserted into the parent.");
			Assert.IsTrue(groupElement.Children[0] is RegionElement,
			    "Region element was not inserted into the parent.");
			Assert.AreEqual("Test Region", groupElement.Children[0].Name,
			    "Region element was not inserted into the parent.");
			Assert.AreEqual(3, groupElement.Children[0].Children.Count,
			    "Element was not inserted into the parent.");
			Assert.AreEqual(0, groupElement.Children[0].Children.IndexOf(field1),
			    "Element is not at the correct index[0].Children.");
			Assert.AreEqual(1, groupElement.Children[0].Children.IndexOf(field2),
			    "Element is not at the correct index.");
			Assert.AreEqual(2, groupElement.Children[0].Children.IndexOf(field3),
			    "Element is not at the correct index.");
		}

		#endregion Public Methods
	}
}