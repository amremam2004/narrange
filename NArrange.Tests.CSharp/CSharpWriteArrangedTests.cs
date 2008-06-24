using System.CodeDom.Compiler;
using NArrange.CSharp;
using NArrange.Tests.Core;
using NUnit.Framework;

namespace NArrange.Tests.CSharp
{
	/// <summary>
	/// Test fixture for parsing, arranging and writing C# test source code files.
	/// </summary>
	[TestFixture]
	public class CSharpWriteArrangedTests : WriteArrangedTests<CSharpParser, CSharpWriter>
	{
		#region Public Properties

		/// <summary>
		/// Gets a list of valid test files.
		/// </summary>
		public override ISourceCodeTestFile[] ValidTestFiles
		{
			get
			{
			    return new ISourceCodeTestFile[]
			    {
			        CSharpTestUtilities.GetAssemblyAttributesFile(),
			        CSharpTestUtilities.GetClassAttributesFile(),
			        CSharpTestUtilities.GetClassDefinitionFile(),
			        CSharpTestUtilities.GetClassMembersFile(),
			        CSharpTestUtilities.GetInterfaceDefinitionFile(),
			        CSharpTestUtilities.GetMultiClassDefinitionFile(),
			        CSharpTestUtilities.GetMultipleNamespaceFile(),
			        CSharpTestUtilities.GetOperatorsFile(),
			        CSharpTestUtilities.GetSingleNamespaceFile(),
			        CSharpTestUtilities.GetStructDefinitionFile(),
			        CSharpTestUtilities.GetUTF8File()
			    };
			}
		}

		#endregion Public Properties

		#region Protected Methods

		/// <summary>
		/// Compiles source code text to the specified assembly.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="assemblyName"></param>
		/// <returns></returns>
		protected override CompilerResults Compile(string text, string assemblyName)
		{
			return CSharpTestFile.Compile(text, assemblyName);
		}

		#endregion Protected Methods
	}
}