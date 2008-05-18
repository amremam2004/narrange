using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;

using NUnit.Framework;

using NArrange.CSharp;
using NArrange.Core;
using NArrange.Core.Configuration;
using NArrange.Tests.CSharp;
using NArrange.Tests.Core;

namespace NArrange.Tests.Core
{
	/// <summary>
	/// Test fixture for the FileArranger class
	/// </summary>
	[TestFixture]
	public class FileArrangerTests
	{
		#region Fields

		private string _testFilteredFile;
		private string _testFilteredProjectFile;
		private string _testInvalidExtensionFile;
		private string _testInvalidSourceFile;
		private string _testProjectFile;
		private string _testSolutionFile;
		private string _testValidSourceFile1;
		private string _testValidSourceFile2;

		#endregion Fields

		#region Private Methods

		private static string GetTestFileContents(string fileName)
		{
			string contents = null;

			using (Stream stream = CSharpTestFile.GetTestFileStream(fileName))
			{
			    Assert.IsNotNull(stream,
			        "Test stream could not be retrieved.");

			    StreamReader reader = new StreamReader(stream);
			    contents = reader.ReadToEnd();
			}

			return contents;
		}

		#endregion Private Methods

		#region Public Methods

		/// <summary>
		/// Tests arranging an empty project file
		/// </summary>
		[Test]
		public void ArrangeEmptyProjectTest()
		{
			TestLogger logger = new TestLogger();
			FileArranger fileArranger = new FileArranger(null, logger);

			string emptyProjectFile = Path.Combine(Path.GetTempPath(), 
			    Guid.NewGuid().ToString().Replace('-','_') + ".csproj");
			File.WriteAllText(emptyProjectFile, "<Project></Project>");

			try
			{
			    bool success = fileArranger.Arrange(emptyProjectFile, null);

			    Assert.IsTrue(success, "Expected file to be arranged succesfully.");
			    Assert.IsTrue(logger.HasPartialMessage(LogLevel.Warning, 
			        "does not contain any supported source files"));
			}
			finally
			{
			    try
			    {
			        File.Delete(emptyProjectFile);
			    }
			    catch
			    {
			    }
			}
		}

		/// <summary>
		/// Tests arranging a single source file with an invalid configuration
		/// </summary>
		[Test]
		public void ArrangeInvalidConfigurationTest()
		{
			string testConfiguration = Path.GetTempFileName();
			File.WriteAllText(testConfiguration, "<xml");

			try
			{
			    TestLogger logger = new TestLogger();
			    FileArranger fileArranger = new FileArranger(testConfiguration, logger);

			    bool success = fileArranger.Arrange(_testValidSourceFile1, null);

			    Assert.IsFalse(success, "Expected file to not be arranged succesfully.");
			    Assert.IsTrue(logger.HasPartialMessage(LogLevel.Error, "Unable to load configuration file"));
			}
			finally
			{
			    try
			    {
			        File.Delete(testConfiguration);
			    }
			    catch
			    {
			    }
			}
		}

		/// <summary>
		/// Tests arranging a single source file with an invalid configuration
		/// </summary>
		[Test]
		public void ArrangeInvalidExtensionAssemblyTest()
		{
			string testConfiguration = Path.GetTempFileName();
			File.WriteAllText(testConfiguration, 
			    @"<CodeConfiguration xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
			        <Handlers>
			            <SourceHandler Assembly='NArrange.BlahBlahBlahBlah, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'>
			                <ProjectExtensions>
				                <Extension Name='csproj'/>
			                </ProjectExtensions>
			                <SourceExtensions>
				                <Extension Name='cs'/>			 
			                </SourceExtensions>
			            </SourceHandler>
			        </Handlers>
			    </CodeConfiguration>");

			try
			{
			    TestLogger logger = new TestLogger();
			    FileArranger fileArranger = new FileArranger(testConfiguration, logger);

			    bool success = fileArranger.Arrange(_testValidSourceFile1, null);

			    Assert.IsFalse(success, "Expected file to not be arranged succesfully.");
			    Assert.IsTrue(logger.HasPartialMessage(LogLevel.Error, "Unable to load configuration file"));
			}
			finally
			{
			    try
			    {
			        File.Delete(testConfiguration);
			    }
			    catch
			    {
			    }
			}
		}

		/// <summary>
		/// Tests arranging a single source file with an invalid extension
		/// </summary>
		[Test]
		public void ArrangeInvalidExtensionTest()
		{
			TestLogger logger = new TestLogger();
			FileArranger fileArranger = new FileArranger(null, logger);

			bool success = fileArranger.Arrange(_testInvalidExtensionFile, null);

			Assert.IsFalse(success, "Expected file to not be arranged succesfully.");
			Assert.IsTrue(logger.HasPartialMessage(LogLevel.Warning, "No assembly is registered to handle file"));
		}

		/// <summary>
		/// Tests arranging a single invalid source file
		/// </summary>
		[Test]
		public void ArrangeInvalidSourceFileTest()
		{
			TestLogger logger = new TestLogger();
			FileArranger fileArranger = new FileArranger(null, logger);

			bool success = fileArranger.Arrange(_testInvalidSourceFile, null);

			Assert.IsFalse(success, "Expected file to not be arranged succesfully.");
			Assert.IsTrue(logger.HasMessage(LogLevel.Verbose, "0 files written."));
		}

		/// <summary>
		/// Tests arranging a single source file with an invalid configuration
		/// </summary>
		[Test]
		public void ArrangeNonExistantConfigurationTest()
		{
			TestLogger logger = new TestLogger();
			FileArranger fileArranger = new FileArranger("blahblahblahblah.xml", logger);

			bool success = fileArranger.Arrange(_testValidSourceFile1, null);

			Assert.IsFalse(success, "Expected file to not be arranged succesfully.");
			Assert.IsTrue(logger.HasPartialMessage(LogLevel.Error, "Unable to load configuration file"));
		}

		/// <summary>
		/// Tests arranging a project file that is excluded in the configuration.
		/// </summary>
		[Test]
		public void ArrangeProjectFilteredTest()
		{
			CodeConfiguration filterProjectConfig = CodeConfiguration.Default.Clone() as CodeConfiguration;

            // Set up the filter
            FilterBy filter = new FilterBy();
            filter.Condition = "!($(File.Path) : '.Filtered.')";
            filterProjectConfig.Handlers[0].ProjectExtensions[0].FilterBy = filter;

			string filterProjectConfigFile = Path.Combine(Path.GetTempPath(), "FilterProjectConfig.xml");

			try
			{
			    filterProjectConfig.Save(filterProjectConfigFile);

			    TestLogger logger = new TestLogger();
			    FileArranger fileArranger = new FileArranger(filterProjectConfigFile, logger);

			    bool success = fileArranger.Arrange(_testFilteredProjectFile, null);

			    Assert.IsTrue(success, "Expected file to be arranged succesfully.");
			    Assert.IsTrue(logger.HasMessage(LogLevel.Verbose, "0 files written."),
			        "Expected 0 files to be written - " + logger.ToString());
			}
			finally
			{
			    try
			    {
			        File.Delete(filterProjectConfigFile);
			    }
			    catch
			    {
			    }
			}
		}

		/// <summary>
		/// Tests arranging a project file
		/// </summary>
		[Test]
		public void ArrangeProjectTest()
		{
			TestLogger logger = new TestLogger();
			FileArranger fileArranger = new FileArranger(null, logger);

			bool success = fileArranger.Arrange(_testProjectFile, null);

			Assert.IsTrue(success, "Expected file to be arranged succesfully.");
			Assert.IsTrue(logger.HasMessage(LogLevel.Verbose, "2 files written."),
			    "Expected 2 files to be written - " + logger.ToString());
		}

		/// <summary>
		/// Tests arranging a read-only source file
		/// </summary>
		[Test]
		public void ArrangeReadOnlySourceFileTest()
		{
			TestLogger logger = new TestLogger();
			FileArranger fileArranger = new FileArranger(null, logger);

			File.SetAttributes(_testValidSourceFile1, FileAttributes.ReadOnly);

			try
			{
			    bool success = fileArranger.Arrange(_testValidSourceFile1, null);

			    Assert.IsFalse(success, "Expected file to not be arranged succesfully.");
			    Assert.IsTrue(logger.HasMessage(LogLevel.Verbose, "0 files written."));
			}
			finally
			{
			    File.SetAttributes(_testValidSourceFile1, FileAttributes.Normal);
			}
		}

		/// <summary>
		/// Tests arranging a single source file
		/// </summary>
		[Test]
		public void ArrangeSingleSourceFileTest()
		{
			TestLogger logger = new TestLogger();
			FileArranger fileArranger = new FileArranger(null, logger);

			bool success = fileArranger.Arrange(_testValidSourceFile1, null);

			Assert.IsTrue(success, "Expected file to be arranged succesfully.");
			Assert.IsTrue(logger.HasMessage(LogLevel.Verbose, "1 files written."), 
			    "Expected 1 file to be written. - " + logger.ToString());
		}

		/// <summary>
		/// Tests arranging a solution file
		/// </summary>
		[Test]
		public void ArrangeSolutionTest()
		{
			TestLogger logger = new TestLogger();
			FileArranger fileArranger = new FileArranger(null, logger);

			bool success = fileArranger.Arrange(_testSolutionFile, null);

			Assert.IsTrue(success, "Expected file to be arranged succesfully.");
			Assert.IsTrue(logger.HasMessage(LogLevel.Verbose, "2 files written."),
			    "Expected 2 files to be written. - " + logger.ToString());
		}

		/// <summary>
		/// Performs test setup
		/// </summary>
		[SetUp]
		public void TestSetup()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();

			string contents = GetTestFileContents("ClassMembers.cs");
			_testValidSourceFile1 = Path.Combine(Path.GetTempPath(), "ClassMembers.cs");
			File.WriteAllText(_testValidSourceFile1, contents);

			contents = GetTestFileContents("ClassMembers.cs");
			_testValidSourceFile2 = Path.Combine(Path.GetTempPath(), "ClassMembers.cs");
			File.WriteAllText(_testValidSourceFile2, contents);

			_testFilteredFile = Path.Combine(Path.GetTempPath(), "Test.Designer.cs");
			File.WriteAllText(_testFilteredFile, "//This file should be excluded\r\n"  + contents);

			_testProjectFile = Path.Combine(Path.GetTempPath(), "TestProject.csproj");
			CSharpProjectParserTests.WriteTestProject(_testProjectFile);

			_testFilteredProjectFile = Path.Combine(Path.GetTempPath(), "Test.Filtered.csproj");
			CSharpProjectParserTests.WriteTestProject(_testFilteredProjectFile);

			_testSolutionFile = Path.Combine(Path.GetTempPath(), "TestSolution.sln");
			SolutionParserTests.WriteTestSolution(_testSolutionFile);

			contents = GetTestFileContents("ClassDefinition.cs");
			_testValidSourceFile2 = Path.Combine(Path.GetTempPath(), "ClassDefinition.cs");
			File.WriteAllText(_testValidSourceFile2, contents);

			contents = GetTestFileContents("ExpectedBlockClose.cs");
			_testInvalidSourceFile = Path.GetTempFileName() + ".cs";
			File.WriteAllText(_testInvalidSourceFile, contents);

			contents = GetTestFileContents("ClassMembers.cs");
			_testInvalidExtensionFile = Path.GetTempFileName() + ".zzz";
			File.WriteAllText(_testInvalidExtensionFile, contents);
		}

		/// <summary>
		/// Performs test fixture cleanup
		/// </summary>
		[TearDown]
		public void TestTearDown()
		{
			try
			{
			    if (_testValidSourceFile1 != null)
			    {
			        File.Delete(_testValidSourceFile1);
			        File.Delete(_testInvalidExtensionFile);
			        File.Delete(_testInvalidSourceFile);
			        File.Delete(_testValidSourceFile2);
			        File.Delete(_testProjectFile);
			        File.Delete(_testSolutionFile);
			        File.Delete(_testFilteredFile);
			        File.Delete(_testFilteredProjectFile);
			    }
			}
			catch
			{
			}
		}

		#endregion Public Methods
	}
}