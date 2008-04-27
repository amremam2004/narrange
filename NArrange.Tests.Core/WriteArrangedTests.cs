using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

using NUnit.Framework;

using NArrange.Core;
using NArrange.Core.CodeElements;
using NArrange.Core.Configuration;

namespace NArrange.Tests.Core
{
	/// <summary>
	/// Base test fixture for writing arranged code elements.
	/// </summary>
	public abstract class WriteArrangedTests<TCodeParser,TCodeWriter>
		where TCodeParser : ICodeElementParser, new()
		where TCodeWriter : ICodeElementWriter, new()
	{
		#region Public Properties

		/// <summary>
		/// Gets an array of valid test files.
		/// </summary>
		public abstract ISourceCodeTestFile[] ValidTestFiles
		{
			get;
		}

		#endregion Public Properties

		#region Protected Methods

		/// <summary>
		/// Compiles source code text to an assembly with the specified name.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="assemblyName"></param>
		/// <returns></returns>
		protected abstract CompilerResults Compile(string text, string assemblyName);

		#endregion Protected Methods

		#region Public Methods

		/// <summary>
		/// Tests writing a tree of arranged elements.
		/// </summary>
		[Test]
		public void WriteArrangedElementTest()
		{
			CodeArranger arranger = new CodeArranger(CodeConfiguration.Default);

			ReadOnlyCollection<ICodeElement> testElements;

			ISourceCodeTestFile[] testFiles = ValidTestFiles;
			foreach (ISourceCodeTestFile testFile in testFiles)
			{
			    using (TextReader reader = testFile.GetReader())
			    {
			        TCodeParser parser = new TCodeParser();
			        testElements = parser.Parse(reader);

			        Assert.IsTrue(testElements.Count > 0,
			            "Test file does not contain any elements.");
			    }

			    ReadOnlyCollection<ICodeElement> arranged = arranger.Arrange(testElements);

			    //
			    // Write the arranged elements
			    //
			    StringWriter writer = new StringWriter();
			    TCodeWriter codeWriter = new TCodeWriter();
			    codeWriter.Write(arranged, writer);

			    string text = writer.ToString();

			    //
			    // Verify that the arranged file still compiles sucessfully.
			    //
			    CompilerResults results = Compile(text, testFile.Name);
			    CompilerError error = TestUtilities.GetCompilerError(results);
			    if (error != null)
			    {
			        Assert.Fail("Arranged source code should not produce compiler errors. " +
			            "Error: {0} - {1}, line {2}, column {3} ",
			            error.ErrorText, testFile.Name,
			            error.Line, error.Column);
			    }
			}
		}

		#endregion Public Methods
	}
}