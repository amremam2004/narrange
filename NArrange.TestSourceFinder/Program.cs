using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NArrange.Tests.CSharp;
using NArrange.Tests.Core;
using NArrange.Tests.VisualBasic;

namespace NArrange.TestSourceFinder
{
	internal class Program
	{
		#region Private Methods

		private static CompilerResults CompileSourceFile(FileInfo sourceFile, string source)
		{
			CompilerResults results = null;

			string extension = sourceFile.Extension.TrimStart('.').ToUpperInvariant();

			switch (extension)
			{
			    case "CS":
			        results = CSharpTestFile.Compile(source, sourceFile.GetHashCode().ToString());
			        break;

			    case "VB":
			        results = VBTestFile.Compile(source, sourceFile.GetHashCode().ToString());
			        break;
			}

			return results;
		}

		private static void FindFiles(string inputDirectory, string outputDirectory)
		{
			if (!Directory.Exists(outputDirectory))
			{
			    Directory.CreateDirectory(outputDirectory);
			}

			Console.WriteLine("Finding source files...");
			FileInfo[] allSourceFiles = GetSourceFileNames(inputDirectory);

			int processed = 0;
			int copied = 0;

			Console.WriteLine("Analyzing source files...");
			foreach (FileInfo sourceFile in allSourceFiles)
			{
			    Console.WriteLine("Analyzing {0}", sourceFile.FullName);
			    processed++;

			    string source;
			    using (StreamReader reader = sourceFile.OpenText())
			    {
			        source = reader.ReadToEnd();
			    }

			    if (source.ToLower().Contains("namespace"))
			    {
			        CompilerResults results = CompileSourceFile(
			            sourceFile, source);

			        CompilerError error = TestUtilities.GetCompilerError(results);
			        if (error == null)
			        {
			            Console.WriteLine("Succesfully compiled {0}", sourceFile.FullName);
			            string destination = Path.Combine(outputDirectory, sourceFile.Name);
			            sourceFile.CopyTo(destination, true);
			            copied++;
			        }
			    }
			}

			Console.WriteLine("Processed " +
			    processed.ToString() + " source files");
			Console.WriteLine("Copied " +
			    copied.ToString() + " source files");
		}

		private static FileInfo[] GetSourceFileNames(string path)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(path);

			List<FileInfo> sourceFiles = new List<FileInfo>();
			sourceFiles.AddRange(directoryInfo.GetFiles("*.cs", SearchOption.AllDirectories));
			sourceFiles.AddRange(directoryInfo.GetFiles("*.vb", SearchOption.AllDirectories));

			return sourceFiles.ToArray();
		}

		#endregion Private Methods

		#region Internal Methods

		static void Main(string[] args)
		{
			if (args.Length != 2)
			{
			    Console.WriteLine("Expected input and output directories as parameters.");
			}

			string inputDirectory = args[0];
			string outputDirectory = args[1];

			try
			{
			    FindFiles(inputDirectory, outputDirectory);
			}
			catch (Exception ex)
			{
			    Console.WriteLine(ex.Message);
			}
		}

		#endregion Internal Methods
	}
}