using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;

using NUnit.Framework;

using NArrange.CSharp;

namespace NArrange.Tests.CSharp
{
	/// <summary>
	/// Test fixture for the CSharpProjectParser class
	/// </summary>
	[TestFixture]
	public class CSharpProjectParserTests
	{
		#region Fields

		private string _testProjectFile;		
		
		#endregion Fields

		#region Public Methods

		/// <summary>
		/// Tests parsing a null project filename
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ParseNullTest()
		{
			CSharpProjectParser projectParser = new CSharpProjectParser();
			projectParser.Parse(null);
		}

		/// <summary>
		/// Tests parsing project source files
		/// </summary>
		[Test]
		public void ParseTest()
		{
			string[] testSourceFiles = new string[]{
			    Path.Combine(Path.GetTempPath(), "ClassMembers.cs"),
			    Path.Combine(Path.GetTempPath(), "ClassDefinition.cs"),
			    Path.Combine(Path.GetTempPath(), "BlahBlahBlah.cs"),
			    Path.Combine(Path.GetTempPath(), "Folder1\\Class2.cs"),
			    Path.Combine(Path.GetTempPath(), "Folder1\\Folder2\\Class3.cs"),
			    Path.Combine(Path.GetTempPath(), "Properties\\AssemblyInfo.cs")
			};
			
			CSharpProjectParser projectParser = new CSharpProjectParser();
			
			ReadOnlyCollection<string> sourceFiles = projectParser.Parse(_testProjectFile);
			
			Assert.AreEqual(testSourceFiles.Length, sourceFiles.Count, 
			    "Unexpected number of source files.");
			
			foreach (string testSourceFile in testSourceFiles)
			{
			    Assert.IsTrue(sourceFiles.Contains(testSourceFile),
			        "Test source file {0} was not included in the source file list.", 
			        testSourceFile);
			}
		}

		/// <summary>
		/// Performs test fixture setup
		/// </summary>
		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			_testProjectFile = Path.GetTempFileName() + ".csproj";
			
			WriteTestProject(_testProjectFile);
		}

		/// <summary>
		/// Performs test fixture cleanup
		/// </summary>
		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			try
			{
			    if (_testProjectFile != null)
			    {
			        File.Delete(_testProjectFile);
			    }
			}
			catch
			{
			}
		}

		/// <summary>
		/// Writes the test project to a file
		/// </summary>
		/// <param name="filename"></param>
		public static void WriteTestProject(string filename)
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			using (Stream stream = assembly.GetManifestResourceStream(
			   typeof(CSharpProjectParserTests), "TestProject.csproj"))
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