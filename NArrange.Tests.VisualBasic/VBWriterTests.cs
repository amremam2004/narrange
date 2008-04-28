using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;

using NUnit.Framework;

using NArrange.Core;
using NArrange.Core.CodeElements;
using NArrange.Core.Configuration;
using NArrange.Tests.Core;
using NArrange.VisualBasic;

namespace NArrange.Tests.VisualBasic
{
	/// <summary>
	/// Test fixture for the VBWriter class
	/// </summary>
	[TestFixture]
	public class VBWriterTests : CodeWriterTests<VBWriter>
	{
		#region Public Methods

		/// <summary>
		/// Tests writing an element with closing comments
		/// </summary>
		[Test]
		public void ClosingCommentsTest()
		{
			TypeElement classElement = new TypeElement();
			classElement.Name = "TestClass";
			classElement.Type = TypeElementType.Class;
			classElement.Access = CodeAccess.Public;

			MethodElement methodElement = new MethodElement();
			methodElement.Name = "DoSomething";
			methodElement.Access = CodeAccess.Public;
			methodElement.Type = "Boolean";
			methodElement.BodyText = "\tReturn False";

			classElement.AddChild(methodElement);

			List<ICodeElement> codeElements = new List<ICodeElement>();

			StringWriter writer;
			codeElements.Add(classElement);

			CodeConfiguration configuration = new CodeConfiguration();
			VBWriter VBWriter = new VBWriter();
			VBWriter.Configuration = configuration;

			configuration.ClosingComments.Enabled = true;
			configuration.ClosingComments.Format = "$(Name)";

			writer = new StringWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "Public Class TestClass\r\n" +
			    "\r\n" +
			    "\tPublic Function DoSomething() As Boolean\r\n" +
			    "\t\tReturn False\r\n" +
			    "\tEnd Function 'DoSomething\r\n" +
			    "\r\n" + 
			    "End Class 'TestClass", text,
			    "Unexpected element text.");
		}

		/// <summary>
		/// Tests writing a region with and without end region names enabled.
		/// </summary>
		[Test]
		public void EndRegionNameTest()
		{
			RegionElement regionElement = new RegionElement();
			regionElement.Name = "TestRegion";

			List<ICodeElement> codeElements = new List<ICodeElement>();

			StringWriter writer;
			codeElements.Add(regionElement);

			CodeConfiguration configuration = new CodeConfiguration();
			VBWriter codeWriter = new VBWriter();
			codeWriter.Configuration = configuration;

			configuration.Regions.EndRegionNameEnabled = true;

			writer = new StringWriter();
			codeWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "#Region \"TestRegion\"\r\n" + 
			    "#End Region 'TestRegion", text,
			    "Unexpected element text.");

			configuration.Regions.EndRegionNameEnabled = false;

			writer = new StringWriter();
			codeWriter.Write(codeElements.AsReadOnly(), writer);

			text = writer.ToString();
			Assert.AreEqual(
			    "#Region \"TestRegion\"\r\n" +
			    "#End Region", text,
			    "Unexpected element text.");
		}

		/// <summary>
		/// Tests writing an element with different tab styles
		/// </summary>
		[Test]
		public void TabStyleTest()
		{
			TypeElement classElement = new TypeElement();
			classElement.Name = "TestClass";
			classElement.Type = TypeElementType.Class;
			classElement.Access = CodeAccess.Public;

			MethodElement methodElement = new MethodElement();
			methodElement.Name = "DoSomething";
			methodElement.Access = CodeAccess.Public;
			methodElement.Type = "Boolean";
			methodElement.BodyText = "\tReturn False";

			classElement.AddChild(methodElement);

			List<ICodeElement> codeElements = new List<ICodeElement>();

			StringWriter writer; 
			codeElements.Add(classElement);

			CodeConfiguration configuration = new CodeConfiguration();
			VBWriter VBWriter = new VBWriter();
			VBWriter.Configuration = configuration;

			//
			// Tabs
			//
			configuration.Tabs.SpacesPerTab = 4;
			configuration.Tabs.Style = TabStyle.Tabs;

			writer = new StringWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "Public Class TestClass\r\n" +
			    "\r\n" +
			    "\tPublic Function DoSomething() As Boolean\r\n" +
			    "\t\tReturn False\r\n" +
			    "\tEnd Function\r\n" + 
			    "\r\n" + 
			    "End Class", text,
			    "Unexpected element text.");

			//
			// Spaces(4)
			//
			configuration.Tabs.SpacesPerTab = 4;
			configuration.Tabs.Style = TabStyle.Spaces;

			writer = new StringWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			text = writer.ToString();
			Assert.AreEqual(
			    "Public Class TestClass\r\n" +
			    "\r\n" +
			    "    Public Function DoSomething() As Boolean\r\n" +
			    "        Return False\r\n" +
			    "    End Function\r\n" +
			    "\r\n" +
			    "End Class", text,
			    "Unexpected element text.");

			//
			// Spaces(8)
			//
			configuration.Tabs.SpacesPerTab = 8;
			configuration.Tabs.Style = TabStyle.Spaces;

			writer = new StringWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			text = writer.ToString();
			Assert.AreEqual(
			    "Public Class TestClass\r\n" +
			    "\r\n" +
			    "        Public Function DoSomething() As Boolean\r\n" +
			    "                Return False\r\n" +
			    "        End Function\r\n" +
			    "\r\n" +
			    "End Class", text,
			    "Unexpected element text.");

			//
			// Parse spaces
			//
			configuration.Tabs.SpacesPerTab = 4;
			configuration.Tabs.Style = TabStyle.Tabs;
			methodElement.BodyText = "    Return False";

			writer = new StringWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			text = writer.ToString();
			Assert.AreEqual(
			    "Public Class TestClass\r\n" +
			    "\r\n" +
			    "\tPublic Function DoSomething() As Boolean\r\n" +
			    "\t\tReturn False\r\n" +
			    "\tEnd Function\r\n" +
			    "\r\n" +
			    "End Class", text,
			    "Unexpected element text.");
		}

		/// <summary>
		/// Tests writing a tree of arranged elements
		/// </summary>
		[Test]
		public void WriteArrangedElementTest()
		{
			CodeArranger arranger = new CodeArranger(CodeConfiguration.Default);

			ReadOnlyCollection<ICodeElement> testElements;

			VBTestFile testFile = VBTestUtilities.GetClassMembersFile();
			using (TextReader reader = testFile.GetReader())
			{
			    VBParser parser = new VBParser();
			    testElements = parser.Parse(reader);

			    Assert.IsTrue(testElements.Count > 0,
			        "Test file does not contain any elements.");
			}

			ReadOnlyCollection<ICodeElement> arranged = arranger.Arrange(testElements);

			//
			// Write the arranged elements
			//
			StringWriter writer = new StringWriter();
			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(arranged, writer);

			string text = writer.ToString();

			//
			// Verify that the arranged file still compiles sucessfully.
			//
			CompilerResults results = VBTestFile.Compile(text, "ArrangedClassMembers.vb");
			CompilerError error = TestUtilities.GetCompilerError(results);
			if (error != null)
			{
			    Assert.Fail("Arranged source code should not produce compiler errors. " +
			        "Error: {0} - {1}, line {2}, column {3} ",
			        error.ErrorText, "ArrangedClassMembers.vb",
			        error.Line, error.Column);
			}
		}

		/// <summary>
		/// Tests writing an attribute element
		/// </summary>
		[Test]
		public void WriteAttributeElementTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			AttributeElement attributeElement = new AttributeElement();
			attributeElement.BodyText = "Obsolete(\"This is obsolete\")";
			attributeElement.AddHeaderCommentLine(
			    "<summary>We no longer need this...</summary>", true);

			StringWriter writer = new StringWriter();
			codeElements.Add(attributeElement);

			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "'''<summary>We no longer need this...</summary>\r\n" +
			    "<Obsolete(\"This is obsolete\")>",
			    text,
			    "Attribute element was not written correctly.");
		}

		/// <summary>
		/// Tests writing a generic class.
		/// </summary>
		[Test]
		public void WriteClassDefinitionGenericTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			TypeElement classElement = new TypeElement();
			classElement.Access = CodeAccess.Public;
			classElement.TypeModifiers = TypeModifiers.Sealed;
			classElement.Type = TypeElementType.Class;
			classElement.Name = "TestClass";
			classElement.AddInterface(
			    new InterfaceReference("IDisposable", InterfaceReferenceType.Interface));
			classElement.AddTypeParameter(new TypeParameter("T"));

			codeElements.Add(classElement);

			StringWriter writer = new StringWriter();
			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "Public NotInheritable Class TestClass(Of T)\r\n" + 
			    "\tImplements IDisposable\r\n" + 
			    "End Class",
			    text,
			    "Class element was not written correctly.");

			classElement = new TypeElement();
			classElement.Access = CodeAccess.Public;
			classElement.TypeModifiers = TypeModifiers.Sealed;
			classElement.Type = TypeElementType.Class;
			classElement.Name = "TestClass";
			classElement.AddInterface(
			    new InterfaceReference("IDisposable", InterfaceReferenceType.Interface));
			classElement.AddTypeParameter(
			    new TypeParameter("T", "IDisposable", "New"));

			codeElements[0] = classElement;

			writer = new StringWriter();
			VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			text = writer.ToString();
			Assert.AreEqual(
			    "Public NotInheritable Class TestClass(Of T As {IDisposable, New})\r\n" + 
			    "\tImplements IDisposable\r\n" +
			    "End Class",
			    text,
			    "Class element was not written correctly.");
		}

		/// <summary>
		/// Tests writing a partial class.
		/// </summary>
		[Test]
		public void WriteClassDefinitionPartialTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			TypeElement classElement = new TypeElement();
			classElement.Access = CodeAccess.Public;
			classElement.TypeModifiers = TypeModifiers.Sealed | TypeModifiers.Partial;
			classElement.Type = TypeElementType.Class;
			classElement.Name = "TestClass";
			classElement.AddTypeParameter(
			    new TypeParameter("T", "IDisposable", "New"));
			classElement.AddInterface(
			    new InterfaceReference("IDisposable", InterfaceReferenceType.Interface));

			StringWriter writer = new StringWriter();
			codeElements.Add(classElement);

			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "Public NotInheritable Partial Class TestClass(Of T As {IDisposable, New})\r\n" + 
			    "\tImplements IDisposable\r\n" +
			    "End Class",
			    text,
			    "Class element was not written correctly.");
		}

		/// <summary>
		/// Tests writing a class.
		/// </summary>
		[Test]
		public void WriteClassDefinitionTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			TypeElement classElement = new TypeElement();
			classElement.Access = CodeAccess.Public;
			classElement.TypeModifiers = TypeModifiers.Sealed;
			classElement.Type = TypeElementType.Class;
			classElement.Name = "TestClass";
			classElement.AddInterface(
			    new InterfaceReference("IDisposable", InterfaceReferenceType.Interface));
			classElement.AddInterface(
			    new InterfaceReference("IEnumerable", InterfaceReferenceType.Interface));

			StringWriter writer; 
			codeElements.Add(classElement);

			VBWriter VBWriter = new VBWriter();
			writer = new StringWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "Public NotInheritable Class TestClass\r\n" + 
			    "\tImplements IDisposable\r\n" + 
			    "\tImplements IEnumerable\r\n" + 
			    "End Class",
			    text,
			    "Class element was not written correctly.");

			classElement.TypeModifiers = TypeModifiers.Abstract;
			VBWriter = new VBWriter();
			writer = new StringWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			text = writer.ToString();
			Assert.AreEqual(
			    "Public MustInherit Class TestClass\r\n" +
			    "\tImplements IDisposable\r\n" +
			    "\tImplements IEnumerable\r\n" +
			    "End Class",
			    text,
			    "Class element was not written correctly.");

			classElement.TypeModifiers = TypeModifiers.Static;
			VBWriter = new VBWriter();
			writer = new StringWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			text = writer.ToString();
			Assert.AreEqual(
			    "Public Class TestClass\r\n" +
			    "\tImplements IDisposable\r\n" +
			    "\tImplements IEnumerable\r\n" +
			    "End Class",
			    text,
			    "Class element was not written correctly.");

			classElement.TypeModifiers = TypeModifiers.Unsafe;
			VBWriter = new VBWriter();
			writer = new StringWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			text = writer.ToString();
			Assert.AreEqual(
			    "Public Class TestClass\r\n" +
			    "\tImplements IDisposable\r\n" +
			    "\tImplements IEnumerable\r\n" +
			    "End Class",
			    text,
			    "Class element was not written correctly.");
		}

		/// <summary>
		/// Tests writing a class with regions.
		/// </summary>
		[Test]
		public void WriteClassDefinitionWithRegionsTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			TypeElement classElement = new TypeElement();
			classElement.Access = CodeAccess.Public;
			classElement.Type = TypeElementType.Class;
			classElement.Name = "TestClass";

			RegionElement fieldsRegion = new RegionElement();
			fieldsRegion.Name = "Fields";

			FieldElement field1 = new FieldElement();
			field1.Name = "_val1";
			field1.Access = CodeAccess.Private;
			field1.Type = "Integer";

			FieldElement field2 = new FieldElement();
			field2.Name = "_val2";
			field2.Access = CodeAccess.Private;
			field2.Type = "Integer";

			fieldsRegion.AddChild(field1);
			fieldsRegion.AddChild(field2);
			classElement.AddChild(fieldsRegion);

			RegionElement methodsRegion = new RegionElement();
			methodsRegion.Name = "Methods";

			MethodElement method = new MethodElement();
			method.Name = "DoSomething";
			method.Access = CodeAccess.Public;
			method.Type = null;
			method.BodyText = string.Empty;

			methodsRegion.AddChild(method);
			classElement.AddChild(methodsRegion);

			StringWriter writer;
			codeElements.Add(classElement);

			VBWriter VBWriter = new VBWriter();
			writer = new StringWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "Public Class TestClass\r\n" + 
			    "\r\n" +
			    "\t#Region \"Fields\"\r\n\r\n" + 
			    "\tPrivate _val1 As Integer\r\n" +
			    "\tPrivate _val2 As Integer\r\n\r\n" + 
			    "\t#End Region 'Fields\r\n\r\n" + 
			    "\t#Region \"Methods\"\r\n\r\n" + 
			    "\tPublic Sub DoSomething()\r\n" + 
			    "\tEnd Sub\r\n\r\n" + 
			    "\t#End Region 'Methods\r\n" +
			    "\r\n" + 
			    "End Class",
			    text,
			    "Class element was not written correctly.");
		}

		/// <summary>
		/// Tests writing a class with unspecified access.
		/// </summary>
		[Test]
		public void WriteClassUnspecifiedAccessTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			TypeElement classElement = new TypeElement();
			classElement.Access = CodeAccess.None;
			classElement.TypeModifiers = TypeModifiers.Partial;
			classElement.Type = TypeElementType.Class;
			classElement.Name = "TestClass";
			classElement.AddInterface(
			    new InterfaceReference("IDisposable", InterfaceReferenceType.Interface));

			StringWriter writer = new StringWriter();
			codeElements.Add(classElement);

			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "Partial Class TestClass\r\n" + 
			    "\tImplements IDisposable\r\n" +
			    "End Class",
			    text,
			    "Class element was not written correctly.");
		}

		/// <summary>
		/// Tests writing a constructor with a constructor reference.
		/// </summary>
		[Test]
		public void WriteConstructorReferenceTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			ConstructorElement constructorElement = new ConstructorElement();
			constructorElement.Access = CodeAccess.Public;
			constructorElement.Name = "New";
			constructorElement.Parameters = "ByVal value As Integer";
			constructorElement.Reference = "MyBase.New(value)";

			StringWriter writer = new StringWriter();
			codeElements.Add(constructorElement);

			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual("Public Sub New(ByVal value As Integer)\r\n\tMyBase.New(value)\r\n" +
			    "End Sub",
			    text,
			    "Constructor element was not written correctly.");
		}

		/// <summary>
		/// Tests writing a constructor.
		/// </summary>
		[Test]
		public void WriteConstructorTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			ConstructorElement constructorElement = new ConstructorElement();
			constructorElement.Access = CodeAccess.Public;
			constructorElement.Name = "New";
			constructorElement.Parameters = "ByVal value As Integer";

			StringWriter writer = new StringWriter();
			codeElements.Add(constructorElement);

			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual("Public Sub New(ByVal value As Integer)\r\nEnd Sub",
			    text,
			    "Constructor element was not written correctly.");
		}

		/// <summary>
		/// Tests writing a generic delegate.
		/// </summary>
		[Test]
		public void WriteDelegateGenericTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			DelegateElement delegateElement = new DelegateElement();
			delegateElement.Access = CodeAccess.Public;
			delegateElement.Type = "Integer";
			delegateElement.Name = "Compare";
			delegateElement.Parameters = "ByVal t1 As T, ByVal t2 As T";
			delegateElement.AddTypeParameter(
				new TypeParameter("T", "Class"));

			StringWriter writer = new StringWriter();
			codeElements.Add(delegateElement);

			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
				"Public Delegate Function Compare(Of T As Class)(ByVal t1 As T, ByVal t2 As T) As Integer",
				text,
				"Delegate element was not written correctly.");
		}

		/// <summary>
		/// Tests writing a delegate.
		/// </summary>
		[Test]
		public void WriteDelegateTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			DelegateElement delegateElement = new DelegateElement();
			delegateElement.Access = CodeAccess.Public;
			delegateElement.Type = "Integer";
			delegateElement.Name = "DoSomething";
			delegateElement.Parameters = "ByVal flag As Boolean";

			StringWriter writer = new StringWriter();
			codeElements.Add(delegateElement);

			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
				"Public Delegate Function DoSomething(ByVal flag As Boolean) As Integer",
				text,
				"Delegate element was not written correctly.");
		}

		/// <summary>
		/// Tests writing an event.
		/// </summary>
		[Test]
		public void WriteEventTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			EventElement eventElement = new EventElement();
			eventElement.Access = CodeAccess.Public;
			eventElement.Type = "EventHandler";
			eventElement.Name = "TestEvent";

			Assert.IsNull(eventElement.Parameters);

			StringWriter writer = new StringWriter();
			codeElements.Add(eventElement);

			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual("Public Event TestEvent As EventHandler",
				text,
				"Event element was not written correctly.");
		}

		/// <summary>
		/// Tests writing an event with parameters.
		/// </summary>
		[Test]
		public void WriteEventWithParametersTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			EventElement eventElement = new EventElement();
			eventElement.Access = CodeAccess.Public;
			eventElement.Type = null;
			eventElement.Name = "TestEvent";
			eventElement.Parameters = "ByVal args As EventArgs";

			StringWriter writer = new StringWriter();
			codeElements.Add(eventElement);

			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual("Public Event TestEvent(ByVal args As EventArgs)",
			    text,
			    "Event element was not written correctly.");
		}

		/// <summary>
		/// Tests writing a generic field.
		/// </summary>
		[Test]
		public void WriteFieldGenericTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			FieldElement fieldElement = new FieldElement();
			fieldElement.Access = CodeAccess.Private;
			fieldElement.MemberModifiers = MemberModifiers.Static;
			fieldElement.Type = "Dictionary(Of String, Integer)";
			fieldElement.Name = "_test";
			fieldElement.InitialValue = "New Dictionary(Of String, Integer)()";

			StringWriter writer = new StringWriter();
			codeElements.Add(fieldElement);

			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual("Private Shared _test As Dictionary(Of String, Integer) = New Dictionary(Of String, Integer)()",
			    text,
			    "FieldElement element was not written correctly.");
		}

		/// <summary>
		/// Tests writing a field.
		/// </summary>
		[Test]
		public void WriteFieldTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			FieldElement fieldElement = new FieldElement();
			fieldElement.Access = CodeAccess.Private;
			fieldElement.MemberModifiers = MemberModifiers.Static;
			fieldElement.Type = "Integer";
			fieldElement.Name = "_test";
			fieldElement.InitialValue = "1";

			StringWriter writer = new StringWriter();
			codeElements.Add(fieldElement);

			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual("Private Shared _test As Integer = 1",
			    text,
			    "FieldElement element was not written correctly.");
		}

		/// <summary>
		/// Tests writing an external function.
		/// </summary>
		[Test]
		public void WriteFunctionExternalTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			MethodElement methodElement = new MethodElement();
			methodElement.Access = CodeAccess.Public;
			methodElement.Type = "String";
			methodElement.Name = "ExternalFunction";
			methodElement.Parameters = "ByVal filename As String";
			methodElement.MemberModifiers = MemberModifiers.External;
			methodElement[VBExtendedProperties.ExternalLibrary] = "Some.dll";
			methodElement[VBExtendedProperties.ExternalAlias] = "doit";
			methodElement[VBExtendedProperties.ExternalModifier] = "Ansi";

			StringWriter writer = new StringWriter();
			codeElements.Add(methodElement);

			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "Public Declare Ansi Function ExternalFunction Lib \"Some.dll\" Alias \"doit\" (ByVal filename As String) As String",
			    text,
			    "Method element was not written correctly.");
		}

		/// <summary>
		/// Tests writing a group of elements.
		/// </summary>
		[Test]
		public void WriteGroupTest()
		{
			string[] nameSpaces = new string[]
			{
			    "System",
			    "System.IO",
			    "System.Text"
			};

			GroupElement group = new GroupElement();

			foreach (string nameSpace in nameSpaces)
			{
			    UsingElement usingElement = new UsingElement();
			    usingElement.Name = nameSpace;
			    group.AddChild(usingElement);
			}

			List<ICodeElement> codeElements = new List<ICodeElement>();
			codeElements.Add(group);

			StringWriter writer;
			VBWriter VBWriter = new VBWriter();

			writer = new StringWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "Imports System\r\n" +
			    "Imports System.IO\r\n" +
			    "Imports System.Text",
			    text,
			    "Group was not written correctly.");

			group.SeparatorType = GroupSeparatorType.Custom;
			group.CustomSeparator = "\r\n";

			writer = new StringWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			text = writer.ToString();
			Assert.AreEqual(
			    "Imports System\r\n\r\n" +
			    "Imports System.IO\r\n\r\n" +
			    "Imports System.Text",
			    text,
			    "Group was not written correctly.");
		}

		/// <summary>
		/// Tests writing an interface definition.
		/// </summary>
		[Test]
		public void WriteInterfaceDefinitionTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			TypeElement classElement = new TypeElement();
			classElement.Access = CodeAccess.Public;
			classElement.Type = TypeElementType.Interface;
			classElement.Name = "TestInterface";
			classElement.AddInterface(
			    new InterfaceReference("IDisposable", InterfaceReferenceType.Interface));

			MethodElement methodElement = new MethodElement();
			methodElement.Name = "DoSomething";
			methodElement.Access = CodeAccess.None;
			classElement.AddChild(methodElement);

			StringWriter writer = new StringWriter();
			codeElements.Add(classElement);

			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "Public Interface TestInterface\r\n" + 
			    "\tImplements IDisposable\r\n" +
			    "\r\n" + 
			    "\tSub DoSomething()\r\n" +
			    "\r\n" + 
			    "End Interface",
			    text,
			    "Interface element was not written correctly.");
		}

		/// <summary>
		/// Tests writing an abstract method.
		/// </summary>
		[Test]
		public void WriteMethodAbstractTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			MethodElement methodElement = new MethodElement();
			methodElement.Access = CodeAccess.Protected;
			methodElement.MemberModifiers = MemberModifiers.Abstract;
			methodElement.Type = null;
			methodElement.Name = "DoSomething";

			StringWriter writer = new StringWriter();
			codeElements.Add(methodElement);

			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual("Protected MustOverride Sub DoSomething()",
			    text,
			    "Method element was not written correctly.");
		}

		/// <summary>
		/// Tests writing a method with a generic return type.
		/// </summary>
		[Test]
		public void WriteMethodGenericReturnTypeTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			MethodElement methodElement = new MethodElement();
			methodElement.Access = CodeAccess.None;
			methodElement.Type = "IEnumerator(Of T)";
			methodElement.Name = "IEnumerable(Of T).GetEnumerator";
			methodElement.BodyText = "\tReturn Nothing";

			StringWriter writer = new StringWriter();
			codeElements.Add(methodElement);

			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "Function IEnumerable(Of T).GetEnumerator() As IEnumerator(Of T)\r\n" +
			    "\tReturn Nothing\r\n" +
			    "End Function"
			    ,
			    text,
			    "Method element was not written correctly.");
		}

		/// <summary>
		/// Tests writing a method that implements an interface.
		/// </summary>
		[Test]
		public void WriteMethodImplementsTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			MethodElement methodElement = new MethodElement();
			methodElement.Access = CodeAccess.Public;
			methodElement.MemberModifiers = MemberModifiers.Static;
			methodElement.Type = "Integer";
			methodElement.Name = "DoSomething";
			methodElement.Parameters = "ByVal flag As Boolean";
			methodElement.AddImplementation(new InterfaceReference("ISomething.DoSomething", InterfaceReferenceType.Interface));
			methodElement.AddImplementation(new InterfaceReference("ISomethingElse.DoSomething", InterfaceReferenceType.Interface));
			methodElement.BodyText = "\tReturn 0";

			StringWriter writer = new StringWriter();
			codeElements.Add(methodElement);

			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "Public Shared Function DoSomething(ByVal flag As Boolean) As Integer Implements ISomething.DoSomething, ISomethingElse.DoSomething\r\n" +
			    "\tReturn 0\r\n" +
			    "End Function",
			    text,
			    "Method element was not written correctly.");
		}

		/// <summary>
		/// Tests writing a partial method declaration.
		/// </summary>
		[Test]
		public void WriteMethodPartialDeclarationTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			MethodElement methodElement = new MethodElement();
			methodElement.Access = CodeAccess.Private;
			methodElement.MemberModifiers = MemberModifiers.Partial;
			methodElement.Type = null;
			methodElement.Name = "DoSomething";
			methodElement.Parameters = "ByVal flag As Boolean";
			methodElement.BodyText = null;

			StringWriter writer = new StringWriter();
			codeElements.Add(methodElement);

			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "Partial Private Sub DoSomething(ByVal flag As Boolean)\r\nEnd Sub",
			    text,
			    "Method element was not written correctly.");
		}

		/// <summary>
		/// Tests writing a partial method implementation.
		/// </summary>
		[Test]
		public void WriteMethodPartialImplementationTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			MethodElement methodElement = new MethodElement();
			methodElement.Access = CodeAccess.Private;
			methodElement.MemberModifiers = MemberModifiers.Partial;
			methodElement.Type = null;
			methodElement.Name = "DoSomething";
			methodElement.Parameters = "ByVal flag As Boolean";
			methodElement.BodyText = null;

			StringWriter writer = new StringWriter();
			codeElements.Add(methodElement);

			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "Partial Private Sub DoSomething(ByVal flag As Boolean)\r\n" +
			    "End Sub",
			    text,
			    "Method element was not written correctly.");
		}

		/// <summary>
		/// Tests writing a sealed method.
		/// </summary>
		[Test]
		public void WriteMethodSealedTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			MethodElement methodElement = new MethodElement();
			methodElement.Access = CodeAccess.Public;
			methodElement.MemberModifiers = MemberModifiers.Sealed | MemberModifiers.Override;
			methodElement.Type = string.Empty;
			methodElement.Name = "DoSomething";

			StringWriter writer = new StringWriter();
			codeElements.Add(methodElement);

			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual("Public Overrides NotOverridable Sub DoSomething()\r\nEnd Sub",
			    text,
			    "Method element was not written correctly.");
		}

		/// <summary>
		/// Tests writing a method.
		/// </summary>
		[Test]
		public void WriteMethodTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			MethodElement methodElement = new MethodElement();
			methodElement.Access = CodeAccess.Public;
			methodElement.MemberModifiers = MemberModifiers.Static;
			methodElement.Type = "Integer";
			methodElement.Name = "DoSomething";
			methodElement.Parameters = "ByVal flag As Boolean";
			methodElement.BodyText = "\tReturn 0";

			StringWriter writer = new StringWriter();
			codeElements.Add(methodElement);

			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "Public Shared Function DoSomething(ByVal flag As Boolean) As Integer\r\n" + 
			    "\tReturn 0\r\n" + 
			    "End Function",
			    text,
			    "Method element was not written correctly.");
		}

		/// <summary>
		/// Tests writing an explicit operator
		/// </summary>
		[Test]
		public void WriteOperatorExplicitTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			MethodElement operatorElement = new MethodElement();
			operatorElement.IsOperator = true;
			operatorElement.OperatorType = OperatorType.Explicit;
			operatorElement.Name = "CType";
			operatorElement.Access = CodeAccess.Public;
			operatorElement.MemberModifiers = MemberModifiers.Static;
			operatorElement.Type = "Decimal";
			operatorElement.Parameters = "ByVal f As Fraction";
			operatorElement.BodyText = "Return CDec(f.num) / f.den";

			StringWriter writer = new StringWriter();
			codeElements.Add(operatorElement);

			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "Public Shared Narrowing Operator CType(ByVal f As Fraction) As Decimal\r\n" + 
			    "\tReturn CDec(f.num) / f.den\r\n" + 
			    "End Operator",
			    text,
			    "Operator element was not written correctly.");
		}

		/// <summary>
		/// Tests writing an implicit operator
		/// </summary>
		[Test]
		public void WriteOperatorImplicitTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			MethodElement operatorElement = new MethodElement();
			operatorElement.IsOperator = true;
			operatorElement.OperatorType = OperatorType.Implicit;
			operatorElement.Name = "CType";
			operatorElement.Access = CodeAccess.Public;
			operatorElement.MemberModifiers = MemberModifiers.Static;
			operatorElement.Type = "Double";
			operatorElement.Parameters = "ByVal f As Fraction";
			operatorElement.BodyText = "Return CDbl(f.num) / f.den";

			StringWriter writer = new StringWriter();
			codeElements.Add(operatorElement);

			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "Public Shared Widening Operator CType(ByVal f As Fraction) As Double\r\n" +
			    "\tReturn CDbl(f.num) / f.den\r\n" +
			    "End Operator",
			    text,
			    "Operator element was not written correctly.");
		}

		/// <summary>
		/// Tests writing an operator
		/// </summary>
		[Test]
		public void WriteOperatorTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			MethodElement operatorElement = new MethodElement();
			operatorElement.IsOperator = true;
			operatorElement.Name = "+";
			operatorElement.Access = CodeAccess.Public;
			operatorElement.MemberModifiers = MemberModifiers.Static;
			operatorElement.Type = "Fraction";
			operatorElement.Parameters = "ByVal a As Fraction, ByVal b As Fraction";
			operatorElement.BodyText = "Return New Fraction(a.num * b.den + b.num * a.den, a.den * b.den)";

			StringWriter writer = new StringWriter();
			codeElements.Add(operatorElement);

			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "Public Shared Operator +(ByVal a As Fraction, ByVal b As Fraction) As Fraction\r\n" +
			    "\tReturn New Fraction(a.num * b.den + b.num * a.den, a.den * b.den)\r\n" +
			    "End Operator",
			    text,
			    "Operator element was not written correctly.");
		}

		/// <summary>
		/// Tests writing an external subroutine.
		/// </summary>
		[Test]
		public void WriteSubExternalTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			MethodElement methodElement = new MethodElement();
			methodElement.Access = CodeAccess.Public;
			methodElement.Type = null;
			methodElement.Name = "ExternalSub";
			methodElement.Parameters = "ByVal filename As String";
			methodElement.MemberModifiers = MemberModifiers.External;
			methodElement[VBExtendedProperties.ExternalLibrary] = "Some.dll";
			methodElement[VBExtendedProperties.ExternalAlias] = "doit";
			methodElement[VBExtendedProperties.ExternalModifier] = "Ansi";

			StringWriter writer = new StringWriter();
			codeElements.Add(methodElement);

			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "Public Declare Ansi Sub ExternalSub Lib \"Some.dll\" Alias \"doit\" (ByVal filename As String)",
			    text,
			    "Method element was not written correctly.");
		}

		/// <summary>
		/// Tests writing a using element with a redefine
		/// </summary>
		[Test]
		public void WriteUsingElementRedefineTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			UsingElement usingElement = new UsingElement();
			usingElement.Name = "System.Text";
			usingElement.Redefine = "SysText";

			StringWriter writer = new StringWriter();
			codeElements.Add(usingElement);

			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
				"Imports SysText = System.Text",
				text,
				"Using element was not written correctly.");
		}

		/// <summary>
		/// Tests writing a using element
		/// </summary>
		[Test]
		public void WriteUsingElementTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			UsingElement usingElement = new UsingElement();
			usingElement.Name = "System.Text";
			usingElement.AddHeaderCommentLine("We'll be doing several text operations.");

			StringWriter writer = new StringWriter();
			codeElements.Add(usingElement);

			VBWriter VBWriter = new VBWriter();
			VBWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "'We'll be doing several text operations.\r\n" + 
			    "Imports System.Text",
			    text,
			    "Using element was not written correctly.");
		}

		#endregion Public Methods
	}
}