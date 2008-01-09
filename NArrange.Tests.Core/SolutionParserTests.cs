using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;

using NUnit.Framework;

using NArrange.Core;

namespace NArrange.Tests.Core
{
	/// <summary>
	/// Test fixture for the SolutionParser class
	/// </summary>
	[TestFixture]
	public class SolutionParserTests	
	{
		#region Fields
		
		private string _testSolutionFile;		
		
		#endregion Fields
		
		#region Public Methods
		
		/// <summary>
		/// Tests parsing a null filename
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ParseNullTest()		
		{
			SolutionParser solutionParser = new SolutionParser();
			solutionParser.Parse(null);
		}		
		
		/// <summary>
		/// Tests parsing project source files
		/// </summary>
		[Test]
		public void ParseTest()		
		{
			string[] testProjectFiles = new string[]{
			    Path.Combine(Path.GetTempPath(), "TestProject.csproj")};
			
			SolutionParser solutionParser = new SolutionParser();
			
			ReadOnlyCollection<string> projectFiles = solutionParser.Parse(_testSolutionFile);
			
			Assert.AreEqual(testProjectFiles.Length, projectFiles.Count, 
			    "Unexpected number of source files.");
			
			foreach (string testProjectFile in testProjectFiles)
			{
			    Assert.IsTrue(projectFiles.Contains(testProjectFile),
			        "Test project file {0} was not included in the project file list.", 
			        testProjectFile);
			}
		}		
		
		/// <summary>
		/// Performs test fixture setup
		/// </summary>
		[TestFixtureSetUp]
		public void TestFixtureSetup()		
		{
			_testSolutionFile = Path.GetTempFileName() + ".csproj";
			
			WriteTestSolution(_testSolutionFile);
		}		
		
		/// <summary>
		/// Performs test fixture cleanup
		/// </summary>
		[TestFixtureTearDown]
		public void TestFixtureTearDown()		
		{
			try
			{
			    if (_testSolutionFile != null)
			    {
			        File.Delete(_testSolutionFile);
			    }
			}
			catch
			{
			}
		}		
		
		/// <summary>
		/// Writes the test solution to a file
		/// </summary>
		/// <param name="filename"></param>
		public static void WriteTestSolution(string filename)		
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			using (Stream stream = assembly.GetManifestResourceStream(
			   typeof(SolutionParserTests), "TestProject.sln"))
			{
			    Assert.IsNotNull(stream,
			        "Test stream could not be retrieved.");
			
			    StreamReader reader = new StreamReader(stream);
			    string contents = reader.ReadToEnd();
			
			    File.WriteAllText(filename, contents);
			}
		}		
		
		#endregion Public Methods

	}
}