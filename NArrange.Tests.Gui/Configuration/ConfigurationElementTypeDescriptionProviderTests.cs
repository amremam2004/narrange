using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Text;

using NUnit.Framework;

using NArrange.Core.Configuration;
using NArrange.Gui.Configuration;

namespace NArrange.Tests.Gui.Configuration
{
	/// <summary>
	/// Test fixture for the ConfigurationElementTypeDescriptionProvider
	/// class.
	/// </summary>
	[TestFixture]
	public class ConfigurationElementTypeDescriptionProviderTests
	{
		#region Public Methods

		/// <summary>
		/// Tests the GetTypeDescriptor method.
		/// </summary>
		[Test]
		public void GetTypeDescriptorTest()
		{
			Type configType = typeof(ElementConfiguration);

			ConfigurationElementTypeDescriptionProvider typeDescriptionProvider =
				new ConfigurationElementTypeDescriptionProvider(configType);

			ICustomTypeDescriptor typeDescriptor =
				typeDescriptionProvider.GetTypeDescriptor(configType);

			Assert.IsNotNull(typeDescriptor,
				"Expected a valid type descriptor instance to be returned from GetTypeDescriptor().");

			PropertyDescriptorCollection properties = typeDescriptor.GetProperties();

			PropertyDescriptor elementsProperty = properties["Elements"];
			Assert.IsNotNull(elementsProperty, "Expected property 'Elements' to be present.");

			object editor = elementsProperty.GetEditor(typeof(UITypeEditor));
			Assert.IsNotNull(editor, "Expected an editor instance.");
			Assert.AreEqual(typeof(ConfigurationElementCollectionEditor), editor.GetType(),
				"Unexpected editor type for the Elements property.");
		}

		#endregion Public Methods
	}
}