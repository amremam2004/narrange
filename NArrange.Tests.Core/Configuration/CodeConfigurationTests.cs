using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NUnit.Framework;

using NArrange.Core;
using NArrange.Core.Configuration;

namespace NArrange.Tests.Core.Configuration
{
	/// <summary>
	/// Test fixture for the CodeConfiguration class
	/// </summary>
	[TestFixture]
	public class CodeConfigurationTests	
	{
		#region Public Methods
		
		/// <summary>
		/// Tests the Clone method
		/// </summary>
		[Test]
		public void CloneTest()		
		{
			CodeConfiguration defaultConfig = CodeConfiguration.Default;
			Assert.IsNotNull(defaultConfig,
			    "Default configuration should not be null.");
			
			Assert.AreEqual(3, defaultConfig.Elements.Count,
			    "Unexpected number of root level elements.");
			
			CodeConfiguration clonedConfig = defaultConfig.Clone() as CodeConfiguration;
			Assert.IsNotNull(clonedConfig, "Clone should return an instance.");
			
			Assert.AreNotSame(defaultConfig, clonedConfig,
			    "Clone should be a different instance.");
			
			Assert.AreEqual(defaultConfig.Elements.Count, clonedConfig.Elements.Count,
			    "Child element state was not copied correctly.");
		}		
		
		/// <summary>
		/// Tests the creation of a new CodeConfiguration
		/// </summary>
		[Test]
		public void CreateTest()		
		{
			CodeConfiguration configuration = new CodeConfiguration();
			
			Assert.IsNotNull(configuration.Elements,
			    "Elements collection should not be null.");
			Assert.AreEqual(0, configuration.Elements.Count,
			    "Elements collection should be empty.");
		}		
		
		/// <summary>
		/// Tests the Default configuration property
		/// </summary>
		[Test]
		public void DefaultTest()		
		{
			CodeConfiguration defaultConfig = CodeConfiguration.Default;
			Assert.IsNotNull(defaultConfig,
			    "Default configuration should not be null.");
			
			Assert.AreEqual(3, defaultConfig.Elements.Count,
			    "Unexpected number of root level elements.");
			
			
			
			//
			// Using elements
			//
			ElementConfiguration usingElement = defaultConfig.Elements[0] as ElementConfiguration;
			Assert.IsNotNull(usingElement, "Expected an ElementConfiguration.");
			Assert.AreEqual(ElementType.Using, usingElement.ElementType,
			    "Unexpected element type.");
			Assert.IsNotNull(usingElement.GroupBy, "Expected grouping to be specified.");
			Assert.AreEqual(ElementAttribute.Name, usingElement.GroupBy.By, 
			    "Expected name grouping.");
			Assert.IsNotNull(usingElement.SortBy, "Expected a sort to be specified.");
			Assert.AreEqual(ElementAttribute.Name, usingElement.SortBy.By,
			    "Expected name sorting.");
			
			//
			// Assembly attributes
			//
			ElementConfiguration attributeElement = defaultConfig.Elements[1] as ElementConfiguration;
			Assert.IsNotNull(attributeElement, "Expected an ElementConfiguration");
			Assert.AreEqual(ElementType.Attribute, attributeElement.ElementType,
			    "Unexpected element type.");
			Assert.IsNotNull(attributeElement.SortBy, "Expected a sort to be specified.");
			Assert.AreEqual(ElementAttribute.Name, attributeElement.SortBy.By,
			    "Expected name sorting.");
			
			//
			// Namespace elements
			//
			ElementConfiguration namespaceElement = defaultConfig.Elements[2] as ElementConfiguration;
			Assert.IsNotNull(namespaceElement, "Expected an ElementConfiguration.");
			Assert.AreEqual(ElementType.Namespace, namespaceElement.ElementType,
			    "Unexpected element type.");
			
			// TODO: Verify entire heirarchy
		}		
		
		/// <summary>
		/// Tests serialization and deserialization
		/// </summary>
		[Test]
		public void SerializeAndDeserializeTest()		
		{
			CodeConfiguration origConfig = new CodeConfiguration();
			
			ElementConfiguration elementConfiguration1 = new ElementConfiguration();
			elementConfiguration1.ElementType = ElementType.Using;
			origConfig.Elements.Add(elementConfiguration1);
			
			ElementConfiguration elementConfiguration2 = new ElementConfiguration();
			elementConfiguration2.ElementType = ElementType.Namespace;
			origConfig.Elements.Add(elementConfiguration2);
			
			RegionConfiguration regionConfiguration = new RegionConfiguration();
			regionConfiguration.Name = "Test Region";
			origConfig.Elements.Add(regionConfiguration);
			
			string tempFile = Path.GetTempFileName();
			try
			{
			    //
			    // Save the configuration to an XML file
			    //
			    origConfig.Save(tempFile);
			
			    //
			    // Load the configuration from the XML file
			    //
			    CodeConfiguration loadedConfig = CodeConfiguration.Load(tempFile);
			    Assert.IsNotNull(loadedConfig,
			        "Loaded configuration should not be null.");
			
			    Assert.AreEqual(origConfig.Elements.Count, loadedConfig.Elements.Count,
			        "An unexpected number of config elements were deserialized.");
			
			    for (int index = 0; index < origConfig.Elements.Count; index++)
			    {
			        if (origConfig.Elements[index] is ElementConfiguration)
			        {
			            ElementConfiguration origElement =
			                origConfig.Elements[index] as ElementConfiguration;
			            ElementConfiguration loadedElement =
			                loadedConfig.Elements[index] as ElementConfiguration;
			
			            Assert.AreEqual(origElement.ElementType, loadedElement.ElementType,
			                "Unexpected element type.");
			        }
			        else
			        {
			            RegionConfiguration origRegion =
			                origConfig.Elements[index] as RegionConfiguration;
			            RegionConfiguration loadedRegion =
			                loadedConfig.Elements[index] as RegionConfiguration;
			
			            Assert.AreEqual(origRegion.Name, loadedRegion.Name,
			                "Unexpected region name.");
			        }
			    }
			}
			finally
			{
			    File.Delete(tempFile);
			}
		}		
		
		#endregion Public Methods

	}
}