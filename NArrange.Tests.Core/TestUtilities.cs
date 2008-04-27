using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NUnit.Framework;

namespace NArrange.Tests.Core
{
	/// <summary>
	/// Test utilities
	/// </summary>
	public static class TestUtilities
	{
		#region Public Properties

		/// <summary>
		/// Test code configuration files.
		/// </summary>
		public static FileInfo[] TestConfigurationFiles
		{
			get
			{
			    DirectoryInfo testConfigDirectory = new DirectoryInfo("TestConfigurations");
			    FileInfo[] testConfigFiles = testConfigDirectory.GetFiles("*.xml");

			    return testConfigFiles;
			}
		}

		#endregion Public Properties

		#region Public Methods

		/// <summary>
		/// Verifies that a file is not empty
		/// </summary>
		/// <param name="fileName"></param>
		public static void AssertNotEmpty(string fileName)
		{
			using (FileStream fs = new FileStream(fileName, FileMode.Open))
			{
			    Assert.IsTrue(fs.Length > 0, "File {0} should not be empty.", fileName);
			}
		}

		/// <summary>
		/// Retrieves a compiler error from a compiler result
		/// </summary>
		/// <param name="results"></param>
		/// <returns></returns>
		public static CompilerError GetCompilerError(CompilerResults results)
		{
			CompilerError error = null;

			foreach (CompilerError compilerError in results.Errors)
			{
			    if (!compilerError.IsWarning)
			    {
			        error = compilerError;
			        break;
			    }
			}

			return error;
		}

		#endregion Public Methods
	}
}