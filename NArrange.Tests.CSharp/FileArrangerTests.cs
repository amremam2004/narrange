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
		
		private string _testInvalidExtensionFile;		
		private string _testInvalidSourceFile;		
		private string _testProjectFile;		
		private string _testSolutionFile;		
		private string _testValidSourceFile1;		
		private string _testValidSourceFile2;		
		
		#endregion Fields
		
		#region Public Methods
		
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
			Assert.IsTrue(logger.HasMessage(LogLevel.Verbose, "0 files processed."));
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
		/// Tests arranging a project file
		/// </summary>
		[Test]
		public void ArrangeProjectTest()		
		{
			TestLogger logger = new TestLogger();
			FileArranger fileArranger = new FileArranger(null, logger);
			
			bool success = fileArranger.Arrange(_testProjectFile, null);
			
			Assert.IsTrue(success, "Expected file to be arranged succesfully.");
			Assert.IsTrue(logger.HasMessage(LogLevel.Verbose, "2 files processed."));
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
			    Assert.IsTrue(logger.HasPartialMessage(LogLevel.Warning, "Unable to write file"));
			    Assert.IsTrue(logger.HasMessage(LogLevel.Verbose, "0 files processed."));
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
			Assert.IsTrue(logger.HasMessage(LogLevel.Verbose, "1 files processed."));
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
			Assert.IsTrue(logger.HasMessage(LogLevel.Verbose, "2 files processed."));
		}		
		
		/// <summary>
		/// Performs test fixture setup
		/// </summary>
		[TestFixtureSetUp]
		public void TestFixtureSetup()		
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			
			string contents = GetTestFileContents("ClassMembers.cs");
			_testValidSourceFile1 = Path.Combine(Path.GetTempPath(), "ClassMembers.cs");
			File.WriteAllText(_testValidSourceFile1, contents);
			
			contents = GetTestFileContents("ClassMembers.cs");
			_testValidSourceFile2 = Path.Combine(Path.GetTempPath(), "ClassMembers.cs");
			File.WriteAllText(_testValidSourceFile2, contents);
			
			_testProjectFile = Path.Combine(Path.GetTempPath(), "TestProject.csproj");
			CSharpProjectParserTests.WriteTestProject(_testProjectFile);
			
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
		[TestFixtureTearDown]
		public void TestFixtureTearDown()		
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
			    }
			}
			catch
			{
			}
		}		
		
		#endregion Public Methods
		
		#region Private Methods
		
		private static string GetTestFileContents(string filename)		
		{
			string contents = null;
			
			using (Stream stream = CSharpTestFile.GetTestFileStream(filename))
			{
			    Assert.IsNotNull(stream,
			        "Test stream could not be retrieved.");
			
			    StreamReader reader = new StreamReader(stream);
			    contents = reader.ReadToEnd();
			}
			
			return contents;
		}		
		
		#endregion Private Methods

	}
}