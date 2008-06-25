using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

using Microsoft.CSharp;

using NArrange.Tests.Core;

using NUnit.Framework;

namespace NArrange.Tests.CSharp
{
	/// <summary>
	/// C# test file information
	/// </summary>
	public class CSharpTestFile : ISourceCodeTestFile
	{
		#region Static Fields

		private static Dictionary<string, Assembly> _compiledSourceFiles = new Dictionary<string, Assembly>();

		#endregion Static Fields

		#region Fields

		private Assembly _assembly;
		private string _resourceName;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new test file using the specified resource.
		/// </summary>
		/// <param name="resourceName"></param>
		public CSharpTestFile(string resourceName)
		{
			_resourceName = resourceName;
			_assembly = GetAssembly(resourceName);
		}

		#endregion Constructors

		#region Public Properties

		/// <summary>
		/// Gets the assembly associated with the test file
		/// </summary>
		public Assembly Assembly
		{
			get
			{
			    return _assembly;
			}
		}

		/// <summary>
		/// Gets the name of the test file.
		/// </summary>
		public string Name
		{
			get
			{
			    return _resourceName;
			}
		}

		#endregion Public Properties

		#region Private Methods

		private static Assembly GetAssembly(string resourceName)
		{
			Assembly assembly = null;
			if (!_compiledSourceFiles.TryGetValue(resourceName, out assembly))
			{
			    using (TextReader reader = GetTestFileReader(resourceName))
			    {
			        string source = reader.ReadToEnd();

			        CompilerResults results = Compile(source, resourceName);

			        if (results.Errors.Count > 0)
			        {
			            CompilerError error = null;

			            error = TestUtilities.GetCompilerError(results);

			            if (error != null)
			            {
			                Assert.Fail("Test source code should not produce compiler errors. " +
			                    "Error: {0} - {1}, line {2}, column {3} ",
			                    error.ErrorText, resourceName,
			                    error.Line, error.Column);
			            }

			            assembly = results.CompiledAssembly;
			        }
			    }

			    if (assembly != null)
			    {
			        _compiledSourceFiles.Add(resourceName, assembly);
			    }
			}

			return assembly;
		}

		#endregion Private Methods

		#region Public Methods

		/// <summary>
		/// Compiles C# source code
		/// </summary>
		/// <param name="source"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static CompilerResults Compile(string source, string name)
		{
			//
			// Compile the test source file
			//
			CodeDomProvider provider = CSharpCodeProvider.CreateProvider("CSharp");

			CompilerParameters parameters = new CompilerParameters();
			parameters.GenerateInMemory = true;
			parameters.GenerateExecutable = false;
			parameters.CompilerOptions = "/unsafe";

			parameters.ReferencedAssemblies.Add("mscorlib.dll");
			parameters.ReferencedAssemblies.Add("System.dll");
			parameters.ReferencedAssemblies.Add("System.Data.dll");
			parameters.ReferencedAssemblies.Add("System.Xml.dll");

			CompilerResults results = provider.CompileAssemblyFromSource(parameters, source);

			return results;
		}

		/// <summary>
		/// Gets a TextReader for this test file
		/// </summary>
		/// <returns></returns>
		public TextReader GetReader()
		{
			return GetTestFileReader(_resourceName);
		}

		/// <summary>
		/// Retrieves a reader for the specified resource
		/// </summary>
		/// <param name="resourceName"></param>
		/// <returns></returns>
		public static TextReader GetTestFileReader(string resourceName)
		{
			return new StreamReader(GetTestFileStream(resourceName), Encoding.Default);
		}

		/// <summary>
		/// Opens a test file resource stream.
		/// </summary>
		/// <param name="resourceName"></param>
		/// <returns></returns>
		public static Stream GetTestFileStream(string resourceName)
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			Stream stream = assembly.GetManifestResourceStream(
			   typeof(CSharpTestUtilities), "TestSourceFiles." + resourceName);

			Assert.IsNotNull(stream,
			    "Test stream could not be retrieved.");

			return stream;
		}

		#endregion Public Methods
	}
}