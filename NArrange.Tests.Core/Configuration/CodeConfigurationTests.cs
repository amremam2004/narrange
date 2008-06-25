using System.IO;

using NArrange.Core;
using NArrange.Core.Configuration;

using NUnit.Framework;

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

			Assert.AreEqual(6, defaultConfig.Elements.Count,
			    "Unexpected number of root level elements.");

			CodeConfiguration clonedConfig = defaultConfig.Clone() as CodeConfiguration;
			Assert.IsNotNull(clonedConfig, "Clone should return an instance.");

			Assert.AreNotSame(defaultConfig, clonedConfig,
			    "Clone should be a different instance.");

			Assert.AreEqual(defaultConfig.Elements.Count, clonedConfig.Elements.Count,
			    "Child element state was not copied correctly.");
			Assert.AreEqual(defaultConfig.Handlers.Count, clonedConfig.Handlers.Count,
			    "Handler state was not copied correctly.");
			Assert.AreEqual(defaultConfig.Tabs.Style, clonedConfig.Tabs.Style,
			    "Tab configuration was not copied correctly.");
			Assert.AreEqual(defaultConfig.Encoding.CodePage, clonedConfig.Encoding.CodePage,
				"Encoding configuration was not copied correctly.");
			Assert.AreEqual(defaultConfig.Regions.EndRegionNameEnabled, clonedConfig.Regions.EndRegionNameEnabled,
				"Regions configuration was not copied correctly.");
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

			//
			// Test the default tab configuration
			//
			Assert.IsNotNull(configuration.Tabs, "Tabs configuration should not be null.");
			Assert.AreEqual(TabStyle.Tabs, configuration.Tabs.Style,
			    "Unexpected default tab style.");
			Assert.AreEqual(4, configuration.Tabs.SpacesPerTab,
			    "Unexpected defatult number of spaces per tab.");
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

			Assert.AreEqual(6, defaultConfig.Elements.Count,
			    "Unexpected number of root level elements.");

			//
			// Handlers
			//
			Assert.IsNotNull(defaultConfig.Handlers,
			    "Handlers collection should not be null.");
			Assert.AreEqual(4, defaultConfig.Handlers.Count,
			    "Unexpected number of default handlers.");

			ProjectHandlerConfiguration msbuildProjectHandlerConfiguration =
				defaultConfig.Handlers[0] as ProjectHandlerConfiguration;
			Assert.IsNotNull(msbuildProjectHandlerConfiguration, "Expected a project handler configuration.");
			Assert.AreEqual(2, msbuildProjectHandlerConfiguration.ProjectExtensions.Count,
				"Unexpected number of project handler extensions.");

			ProjectHandlerConfiguration monoDevelopProjectHandlerConfiguration =
				defaultConfig.Handlers[1] as ProjectHandlerConfiguration;
			Assert.IsNotNull(monoDevelopProjectHandlerConfiguration, "Expected a project handler configuration.");
			Assert.AreEqual(1, monoDevelopProjectHandlerConfiguration.ProjectExtensions.Count,
				"Unexpected number of project handler extensions.");

			SourceHandlerConfiguration csharpHandler =
				defaultConfig.Handlers[2] as SourceHandlerConfiguration;
			Assert.IsNotNull(csharpHandler, "Expected a source handler configuration.");
			Assert.IsTrue(csharpHandler.AssemblyName.Contains("NArrange.CSharp"));
			Assert.AreEqual("CSharp", csharpHandler.Language);
			Assert.IsNotNull(csharpHandler.SourceExtensions[0].FilterBy);

			SourceHandlerConfiguration vbHandler =
				defaultConfig.Handlers[3] as SourceHandlerConfiguration;
			Assert.IsNotNull(vbHandler, "Expected a source handler configuration.");
			Assert.IsTrue(vbHandler.AssemblyName.Contains("NArrange.VisualBasic"));
			Assert.AreEqual("VisualBasic", vbHandler.Language);
			Assert.IsNotNull(vbHandler.SourceExtensions[0].FilterBy);

			//
			// Tabs
			//
			Assert.IsNotNull(defaultConfig.Tabs,
			    "Tab configuration should not be null.");
			Assert.AreEqual(TabStyle.Tabs, defaultConfig.Tabs.Style, 
			    "Unexpected tab style.");
			Assert.AreEqual(4, defaultConfig.Tabs.SpacesPerTab, 
			    "Unexpected number of spaces per tab.");

			//
			// Global region settings
			//
			Assert.IsTrue(defaultConfig.Regions.EndRegionNameEnabled);

			//
			// Header comment region
			//
			RegionConfiguration commentRegion = defaultConfig.Elements[0] as RegionConfiguration;
			Assert.IsNotNull(commentRegion, "Expected a RegionConfiguration.");
			ElementConfiguration commentElement = commentRegion.Elements[0] as ElementConfiguration;
			Assert.AreEqual(ElementType.Comment, commentElement.ElementType,
			    "Unexpected element type.");
			Assert.IsNull(commentElement.GroupBy, "Expected grouping to not be specified.");
			Assert.IsNotNull(commentElement.FilterBy, "Expected a filter to be specified.");

			//
			// Using elements
			//
			ElementConfiguration usingElement = defaultConfig.Elements[1] as ElementConfiguration;
			Assert.IsNotNull(usingElement, "Expected an ElementConfiguration.");
			Assert.AreEqual(ElementType.Using, usingElement.ElementType,
			    "Unexpected element type.");
			Assert.IsNotNull(usingElement.GroupBy, "Expected grouping to be specified.");
			Assert.AreEqual(ElementAttributeType.Name, usingElement.GroupBy.By, 
			    "Expected name grouping.");
			Assert.IsNotNull(usingElement.SortBy, "Expected a sort to be specified.");
			Assert.AreEqual(ElementAttributeType.Name, usingElement.SortBy.By,
			    "Expected name sorting.");

			//
			// Assembly attributes
			//
			ElementConfiguration attributeElement = defaultConfig.Elements[2] as ElementConfiguration;
			Assert.IsNotNull(attributeElement, "Expected an ElementConfiguration");
			Assert.AreEqual(ElementType.Attribute, attributeElement.ElementType,
			    "Unexpected element type.");
			Assert.IsNull(attributeElement.SortBy, "Expected a sort to not be specified.");

			//
			// Element references
			//
			ElementReferenceConfiguration interfaceReference = defaultConfig.Elements[3] as ElementReferenceConfiguration;
			Assert.AreEqual("DefaultInterface", interfaceReference.Id, "Unexpected reference Id.");
			Assert.IsNotNull(interfaceReference.ReferencedElement, "Referenced element should not be null.");

			ElementReferenceConfiguration typeReference = defaultConfig.Elements[4] as ElementReferenceConfiguration;
			Assert.AreEqual("DefaultType", typeReference.Id, "Unexpected reference Id.");
			Assert.IsNotNull(typeReference.ReferencedElement, "Referenced element should not be null.");

			//
			// Namespace elements
			//
			ElementConfiguration namespaceElement = defaultConfig.Elements[5] as ElementConfiguration;
			Assert.IsNotNull(namespaceElement, "Expected an ElementConfiguration.");
			Assert.AreEqual(ElementType.Namespace, namespaceElement.ElementType,
			    "Unexpected element type.");

			// TODO: Verify entire heirarchy
		}

		/// <summary>
		/// Tests loading a configuration with EndRegionNamesEnabled set to false.
		/// </summary>
		[Test]
		public void NoEndRegionNamesTest()
		{
			CodeConfiguration configuration = CodeConfiguration.Load(@"TestConfigurations\NoEndRegionNames.xml");
			Assert.IsNotNull(configuration);
			Assert.IsFalse(configuration.Regions.EndRegionNameEnabled, "Unexpected value for EndRegionNameEnabled.");
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
			elementConfiguration1.Id = "TestId";
			origConfig.Elements.Add(elementConfiguration1);

			ElementConfiguration elementConfiguration2 = new ElementConfiguration();
			elementConfiguration2.ElementType = ElementType.Namespace;
			origConfig.Elements.Add(elementConfiguration2);

			ElementReferenceConfiguration elementReferenceConfiguration = new ElementReferenceConfiguration();
			elementReferenceConfiguration.Id = "TestId";
			origConfig.Elements.Add(elementReferenceConfiguration);

			RegionConfiguration regionConfiguration = new RegionConfiguration();
			regionConfiguration.Name = "Test Region";
			origConfig.Elements.Add(regionConfiguration);

			origConfig.ResolveReferences();
			Assert.AreEqual(elementConfiguration1.Elements.Count, 
			    elementReferenceConfiguration.ReferencedElement.Elements.Count,
			    "Element reference was not resolved.");

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
			        else if (origConfig.Elements[index] is ElementReferenceConfiguration)
			        {
			            ElementReferenceConfiguration origElement =
			                origConfig.Elements[index] as ElementReferenceConfiguration;
			            ElementReferenceConfiguration loadedElement =
			                loadedConfig.Elements[index] as ElementReferenceConfiguration;

			            Assert.AreEqual(origElement.Id, loadedElement.Id,
			                "Unexpected element type.");
			            Assert.AreEqual(origElement.ReferencedElement.Id,
			                loadedElement.ReferencedElement.Id,
			                "Unexpected referenced element.");
			        }
			        else if(origConfig.Elements[index] is RegionConfiguration)
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

		/// <summary>
		/// Tests loading a configuration with spaces specified.
		/// </summary>
		[Test]
		public void SpacesTest()
		{
			CodeConfiguration configuration = CodeConfiguration.Load(@"TestConfigurations\SpacesConfig.xml");
			Assert.IsNotNull(configuration);
			Assert.AreEqual(TabStyle.Spaces, configuration.Tabs.Style,
			    "Unexpected tab style.");
		}

		/// <summary>
		/// Tests conversion from the old project extension format (MSBuild only) to the new.
		/// </summary>
		[Test]
		public void UpgradeProjectExtensionsTest()
		{
			string filename = Path.GetTempFileName();
			try
			{
				CodeConfiguration oldConfiguration = new CodeConfiguration();

				SourceHandlerConfiguration sourceHandler = new SourceHandlerConfiguration();
				ExtensionConfiguration oldExtension = new ExtensionConfiguration();
				oldExtension.Name = "csproj";
				sourceHandler.ProjectExtensions.Add(oldExtension);
				oldConfiguration.Handlers.Add(sourceHandler);
				oldConfiguration.Save(filename);

				CodeConfiguration newConfiguration = CodeConfiguration.Load(filename);
				Assert.AreEqual(2, newConfiguration.Handlers.Count,
					"New handler was not created.");
				ProjectHandlerConfiguration projectHandlerConfiguration =
					newConfiguration.Handlers[0] as ProjectHandlerConfiguration;
				Assert.IsNotNull(projectHandlerConfiguration,
					"Expected a project handler config to be created.");
				Assert.IsNull(projectHandlerConfiguration.AssemblyName);
				Assert.AreEqual(typeof(MSBuildProjectParser).FullName, projectHandlerConfiguration.ParserType);
				Assert.AreEqual(1, projectHandlerConfiguration.ProjectExtensions.Count,
					"Unexpected number of project extensions.");
				Assert.AreEqual(oldExtension.Name, projectHandlerConfiguration.ProjectExtensions[0].Name);
			}
			finally
			{
				try
				{
					File.Delete(filename);
				}
				catch
				{
				}
			}
		}

		#endregion Public Methods
	}
}