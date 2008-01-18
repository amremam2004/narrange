using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace NArrange.Tests.CSharp
{
	/// <summary>
	/// CSharp test utilities
	/// </summary>
	public static class CSharpTestUtilities	
	{
		#region Public Methods
		
		/// <summary>
		/// Assembly attributes test file
		/// </summary>
		/// <returns></returns>
		public static CSharpTestFile GetAssemblyAttributesFile()		
		{
			return new CSharpTestFile("AssemblyAttributes.cs");
		}		
		
		/// <summary>
		/// Class attributes test file
		/// </summary>
		/// <returns></returns>
		public static CSharpTestFile GetClassAttributesFile()		
		{
			return new CSharpTestFile("ClassAttributes.cs");
		}		
		
		/// <summary>
		/// Class definition test file
		/// </summary>
		/// <returns></returns>
		public static CSharpTestFile GetClassDefinitionFile()		
		{
			return new CSharpTestFile("ClassDefinition.cs");
		}		
		
		/// <summary>
		/// Class members test file
		/// </summary>
		/// <returns></returns>
		public static CSharpTestFile GetClassMembersFile()		
		{
			return new CSharpTestFile("ClassMembers.cs");
		}		
		
		/// <summary>
		/// Interface definition test file
		/// </summary>
		/// <returns></returns>
		public static CSharpTestFile GetInterfaceDefinitionFile()		
		{
			return new CSharpTestFile("InterfaceDefinition.cs");
		}		
		
		/// <summary>
		/// Multiple class definition test file
		/// </summary>
		/// <returns></returns>
		public static CSharpTestFile GetMultiClassDefinitionFile()		
		{
			return new CSharpTestFile("MultiClassDefinition.cs");
		}		
		
		/// <summary>
		/// Multiple namespace test file
		/// </summary>
		/// <returns></returns>
		public static CSharpTestFile GetMultipleNamespaceFile()		
		{
			return new CSharpTestFile("MultipleNamespace.cs");
		}		
		
		/// <summary>
		/// Operators test file
		/// </summary>
		/// <returns></returns>
		public static CSharpTestFile GetOperatorsFile()		
		{
			return new CSharpTestFile("Operators.cs");
		}		
		
		/// <summary>
		/// Single namespace test file
		/// </summary>
		/// <returns></returns>
		public static CSharpTestFile GetSingleNamespaceFile()		
		{
			return new CSharpTestFile("SingleNamespace.cs");
		}		
		
		/// <summary>
		/// Structure definition test file
		/// </summary>
		/// <returns></returns>
		public static CSharpTestFile GetStructDefinitionFile()		
		{
			return new CSharpTestFile("StructDefinition.cs");
		}		
		
		#endregion Public Methods
	}
}