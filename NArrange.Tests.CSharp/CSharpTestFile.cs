using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

using Microsoft.CSharp;

using NUnit.Framework;

namespace NArrange.Tests.CSharp
{
	/// <summary>
	/// C# test file information
	/// </summary>
	public class CSharpTestFile	
	{
		#region Fields
		
		private Assembly _assembly;		
		private static Dictionary<string, Assembly> _compiledSourceFiles = new Dictionary<string, Assembly>();		
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
		
		#endregion Public Properties
		
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
			parameters.OutputAssembly = name;
			parameters.CompilerOptions = "/unsafe";
			
			parameters.ReferencedAssemblies.Add("mscorlib.dll");
			parameters.ReferencedAssemblies.Add("System.dll");
			
			
			CompilerResults results = provider.CompileAssemblyFromSource(parameters, source);
			
			return results;
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
		
		#region Private Methods
		
		private static Assembly GetAssembly(string resourceName)		
		{
			if (_compiledSourceFiles.ContainsKey(resourceName))
			{
			    return _compiledSourceFiles[resourceName];
			}
			else
			{
			    Assembly assembly = null;
			
			    using (TextReader reader = GetTestFileReader(resourceName))
			    {
			        string source = reader.ReadToEnd();
			
			        CompilerResults results = Compile(source, resourceName);
			
			        if (results.Errors.Count > 0)
			        {
			            CompilerError error = null;
			
			            error = GetCompilerError(results);
			
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
			
			    return assembly;
			}
		}		
		
		#endregion Private Methods

	}
}