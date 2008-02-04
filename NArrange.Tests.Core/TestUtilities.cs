using System;
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

		#endregion Public Methods
	}
}