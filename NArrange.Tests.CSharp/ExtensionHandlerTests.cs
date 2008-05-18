using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

using NUnit.Framework;

using NArrange.Core;
using NArrange.Core.Configuration;

namespace NArrange.Tests.Core
{
	/// <summary>
	/// Test fixture for the ExtensionHandler class
	/// </summary>
	[TestFixture]
	public class ExtensionHandlerTests
	{
		#region Public Methods

		/// <summary>
		/// Tests creating a new extension handler
		/// </summary>
		[Test]
		public void CreateTest()
		{
			string assemblyName = "NArrange.CSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
			HandlerConfiguration configuration = new HandlerConfiguration();
			configuration.AssemblyName = assemblyName;

			SourceHandler handler = new SourceHandler(configuration);

			Assert.IsNotNull(handler.CodeParser, "Parser was not created.");
			Assert.IsNotNull(handler.Writer, "Writer was not created.");
			Assert.IsNotNull(handler.ProjectParser, "ProjectParser was not created.");
		}

		#endregion Public Methods
	}
}