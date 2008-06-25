using System.CodeDom.Compiler;

using NArrange.Tests.Core;
using NArrange.VisualBasic;

using NUnit.Framework;

namespace NArrange.Tests.VisualBasic
{
	/// <summary>
	/// Test fixture for parsing, arranging and writing VB test source code files.
	/// </summary>
	[TestFixture]
	public class VBWriteArrangedTests : WriteArrangedTests<VBParser, VBWriter>
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
			        VBTestUtilities.GetAssemblyAttributesFile(),
			        VBTestUtilities.GetClassAttributesFile(),
			        VBTestUtilities.GetClassDefinitionFile(),
			        VBTestUtilities.GetClassMembersFile(),
			        VBTestUtilities.GetInterfaceDefinitionFile(),
			        VBTestUtilities.GetMultiClassDefinitionFile(),
			        VBTestUtilities.GetMultipleNamespaceFile(), 
			        VBTestUtilities.GetOperatorsFile(),
			        VBTestUtilities.GetSingleNamespaceFile(),
			        VBTestUtilities.GetStructDefinitionFile(),
			        VBTestUtilities.GetModuleDefinitionFile(),
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
			return VBTestFile.Compile(text, assemblyName);
		}

		#endregion Protected Methods
	}
}