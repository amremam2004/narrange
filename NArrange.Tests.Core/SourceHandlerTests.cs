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
	/// Test fixture for the ProjectHandler class.
	/// </summary>
	[TestFixture]
	public class ProjectHandlerTests
	{
		#region Public Methods

		/// <summary>
		/// Tests creating a new project handler.
		/// </summary>
		[Test]
		public void CreateTest()
		{
			ProjectHandlerConfiguration configuration = new ProjectHandlerConfiguration();
			configuration.ParserType = "NArrange.Core.MonoDevelopProjectParser";

			ProjectHandler handler = new ProjectHandler(configuration);

			Assert.IsNotNull(handler.ProjectParser, "Project parser was not created.");
			Assert.IsInstanceOfType(typeof(MonoDevelopProjectParser), handler.ProjectParser);
		}

		/// <summary>
		/// Tests creating a new project handler.
		/// </summary>
		[Test]
		public void CreateWithAssemblyTest()
		{
			string assemblyName = "NArrange.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
			ProjectHandlerConfiguration configuration = new ProjectHandlerConfiguration();
			configuration.AssemblyName = assemblyName;
			configuration.ParserType = "NArrange.Core.MSBuildProjectParser";

			ProjectHandler handler = new ProjectHandler(configuration);

			Assert.IsNotNull(handler.ProjectParser, "Project parser was not created.");
			Assert.IsInstanceOfType(typeof(MSBuildProjectParser), handler.ProjectParser);
		}

		#endregion Public Methods
	}
}