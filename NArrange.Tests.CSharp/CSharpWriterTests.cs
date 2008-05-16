using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;

using NUnit.Framework;

using NArrange.CSharp;
using NArrange.Core;
using NArrange.Core.CodeElements;
using NArrange.Core.Configuration;
using NArrange.Tests.Core;

namespace NArrange.Tests.CSharp
{
	/// <summary>
	/// Test fixture for the CSharpWriter class
	/// </summary>
	[TestFixture]
	public class CSharpWriterTests : CodeWriterTests<CSharpWriter>
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
			methodElement.Type = "bool";
			methodElement.BodyText = "\treturn false;";

			classElement.AddChild(methodElement);

			List<ICodeElement> codeElements = new List<ICodeElement>();

			StringWriter writer;
			codeElements.Add(classElement);

			CodeConfiguration configuration = new CodeConfiguration();
			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Configuration = configuration;

			configuration.ClosingComments.Enabled = true;
			configuration.ClosingComments.Format = "End $(ElementType) $(Name)";

			writer = new StringWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "public class TestClass\r\n" +
			    "{\r\n" +
			    "\tpublic bool DoSomething()\r\n" +
			    "\t{\r\n" +
			    "\t\treturn false;\r\n" +
			    "\t} // End Method DoSomething\r\n" +
			    "} // End Type TestClass", text,
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
			CSharpWriter codeWriter = new CSharpWriter();
			codeWriter.Configuration = configuration;

			configuration.Regions.EndRegionNameEnabled = true;

			writer = new StringWriter();
			codeWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "#region TestRegion\r\n\r\n" +
			    "#endregion TestRegion", text,
			    "Unexpected element text.");

			configuration.Regions.EndRegionNameEnabled = false;

			writer = new StringWriter();
			codeWriter.Write(codeElements.AsReadOnly(), writer);

			text = writer.ToString();
			Assert.AreEqual(
			    "#region TestRegion\r\n\r\n" +
			    "#endregion", text,
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
			methodElement.Type = "bool";
			methodElement.BodyText = "\treturn false;";

			classElement.AddChild(methodElement);

			List<ICodeElement> codeElements = new List<ICodeElement>();

			StringWriter writer; 
			codeElements.Add(classElement);

			CodeConfiguration configuration = new CodeConfiguration();
			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Configuration = configuration;

			//
			// Tabs
			//
			configuration.Tabs.SpacesPerTab = 4;
			configuration.Tabs.Style = TabStyle.Tabs;

			writer = new StringWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "public class TestClass\r\n" + 
			    "{\r\n" + 
			    "\tpublic bool DoSomething()\r\n" +
			    "\t{\r\n" +
			    "\t\treturn false;\r\n" +
			    "\t}\r\n" + 
			    "}", text,
			    "Unexpected element text.");

			//
			// Spaces(4)
			//
			configuration.Tabs.SpacesPerTab = 4;
			configuration.Tabs.Style = TabStyle.Spaces;

			writer = new StringWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			text = writer.ToString();
			Assert.AreEqual(
			    "public class TestClass\r\n" +
			    "{\r\n" +
			    "    public bool DoSomething()\r\n" +
			    "    {\r\n" +
			    "        return false;\r\n" +
			    "    }\r\n" +
			    "}", text,
			    "Unexpected element text.");

			//
			// Spaces(8)
			//
			configuration.Tabs.SpacesPerTab = 8;
			configuration.Tabs.Style = TabStyle.Spaces;

			writer = new StringWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			text = writer.ToString();
			Assert.AreEqual(
			    "public class TestClass\r\n" +
			    "{\r\n" +
			    "        public bool DoSomething()\r\n" +
			    "        {\r\n" +
			    "                return false;\r\n" +
			    "        }\r\n" +
			    "}", text,
			    "Unexpected element text.");

			//
			// Parse spaces
			//
			configuration.Tabs.SpacesPerTab = 4;
			configuration.Tabs.Style = TabStyle.Tabs;
			methodElement.BodyText = "    return false;";

			writer = new StringWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			text = writer.ToString();
			Assert.AreEqual(
			    "public class TestClass\r\n" +
			    "{\r\n" +
			    "\tpublic bool DoSomething()\r\n" +
			    "\t{\r\n" +
			    "\t\treturn false;\r\n" +
			    "\t}\r\n" +
			    "}", text,
			    "Unexpected element text.");
		}

		/// <summary>
		/// Tests writing an attribute element with a list of children.
		/// </summary>
		[Test]
		public void WriteAttributeElementListTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			AttributeElement attributeElement = new AttributeElement();
			attributeElement.Name = "Obsolete";
			attributeElement.Target = "property";
			attributeElement.BodyText = "\"This is obsolete\"";
			attributeElement.AddHeaderCommentLine(
			    "<summary>We no longer need this...</summary>", true);

			AttributeElement childAttributeElement = new AttributeElement();
			childAttributeElement.Name = "Description";
			childAttributeElement.BodyText = "\"This is a description.\"";
			attributeElement.AddChild(childAttributeElement);

			StringWriter writer = new StringWriter();
			codeElements.Add(attributeElement);

			CSharpWriter codeWriter = new CSharpWriter();
			codeWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "///<summary>We no longer need this...</summary>\r\n" +
			    "[property: Obsolete(\"This is obsolete\"),\r\nDescription(\"This is a description.\")]",
			    text,
			    "Attribute element was not written correctly.");
		}

		/// <summary>
		/// Tests writing an attribute element
		/// </summary>
		[Test]
		public void WriteAttributeElementTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			AttributeElement attributeElement = new AttributeElement();
			attributeElement.Name = "Obsolete";
			attributeElement.BodyText = "\"This is obsolete\"";
			attributeElement.AddHeaderCommentLine(
			    "<summary>We no longer need this...</summary>", true);

			StringWriter writer = new StringWriter();
			codeElements.Add(attributeElement);

			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "///<summary>We no longer need this...</summary>\r\n" +
			    "[Obsolete(\"This is obsolete\")]",
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
			classElement.TypeModifiers = TypeModifiers.Static;
			classElement.Type = TypeElementType.Class;
			classElement.Name = "TestClass";
			classElement.AddInterface(
			    new InterfaceReference("IDisposable", InterfaceReferenceType.Interface));
			classElement.AddTypeParameter(new TypeParameter("T"));

			codeElements.Add(classElement);

			StringWriter writer = new StringWriter();
			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "public static class TestClass<T> : IDisposable\r\n" + 
			    "{\r\n}",
			    text,
			    "Class element was not written correctly.");

			classElement = new TypeElement();
			classElement.Access = CodeAccess.Public;
			classElement.TypeModifiers = TypeModifiers.Static;
			classElement.Type = TypeElementType.Class;
			classElement.Name = "TestClass";
			classElement.AddInterface(
			    new InterfaceReference("IDisposable", InterfaceReferenceType.Interface));
			classElement.AddTypeParameter(
			    new TypeParameter("T", "class", "IDisposable", "new()"));

			codeElements[0] = classElement;

			writer = new StringWriter();
			csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			text = writer.ToString();
			Assert.AreEqual(
			    "public static class TestClass<T> : IDisposable\r\n" +
			    "\twhere T : class, IDisposable, new()\r\n" +
			    "{\r\n}",
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
			classElement.TypeModifiers = TypeModifiers.Static | TypeModifiers.Partial;
			classElement.Type = TypeElementType.Class;
			classElement.Name = "TestClass";
			classElement.AddTypeParameter(
			    new TypeParameter("T", "class", "IDisposable", "new()"));
			classElement.AddInterface(
			    new InterfaceReference("IDisposable", InterfaceReferenceType.Interface));

			StringWriter writer = new StringWriter();
			codeElements.Add(classElement);

			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "public static partial class TestClass<T> : IDisposable\r\n" +
			    "\twhere T : class, IDisposable, new()\r\n" +
			    "{\r\n}",
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

			CSharpWriter csharpWriter = new CSharpWriter();
			writer = new StringWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "public sealed class TestClass : IDisposable, IEnumerable\r\n{\r\n}",
			    text,
			    "Class element was not written correctly.");

			classElement.TypeModifiers = TypeModifiers.Abstract;
			csharpWriter = new CSharpWriter();
			writer = new StringWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			text = writer.ToString();
			Assert.AreEqual(
			    "public abstract class TestClass : IDisposable, IEnumerable\r\n{\r\n}",
			    text,
			    "Class element was not written correctly.");

			classElement.TypeModifiers = TypeModifiers.Static;
			csharpWriter = new CSharpWriter();
			writer = new StringWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			text = writer.ToString();
			Assert.AreEqual(
			    "public static class TestClass : IDisposable, IEnumerable\r\n{\r\n}",
			    text,
			    "Class element was not written correctly.");

			classElement.TypeModifiers = TypeModifiers.Unsafe;
			csharpWriter = new CSharpWriter();
			writer = new StringWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			text = writer.ToString();
			Assert.AreEqual(
			    "public unsafe class TestClass : IDisposable, IEnumerable\r\n{\r\n}",
			    text,
			    "Class element was not written correctly.");

			classElement.TypeModifiers = TypeModifiers.New;
			classElement.Access = CodeAccess.Private;
			csharpWriter = new CSharpWriter();
			writer = new StringWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			text = writer.ToString();
			Assert.AreEqual(
			    "private new class TestClass : IDisposable, IEnumerable\r\n{\r\n}",
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
			field1.Type = "int";

			FieldElement field2 = new FieldElement();
			field2.Name = "_val2";
			field2.Access = CodeAccess.Private;
			field2.Type = "int";

			fieldsRegion.AddChild(field1);
			fieldsRegion.AddChild(field2);
			classElement.AddChild(fieldsRegion);

			RegionElement methodsRegion = new RegionElement();
			methodsRegion.Name = "Methods";

			MethodElement method = new MethodElement();
			method.Name = "DoSomething";
			method.Access = CodeAccess.Public;
			method.Type = "void";
			method.BodyText = string.Empty;

			methodsRegion.AddChild(method);
			classElement.AddChild(methodsRegion);

			StringWriter writer;
			codeElements.Add(classElement);

			CSharpWriter csharpWriter = new CSharpWriter();
			writer = new StringWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "public class TestClass\r\n" + 
			    "{\r\n" +
			    "\t#region Fields\r\n\r\n" + 
			    "\tprivate int _val1;\r\n" +
			    "\tprivate int _val2;\r\n\r\n" + 
			    "\t#endregion Fields\r\n\r\n" + 
			    "\t#region Methods\r\n\r\n" + 
			    "\tpublic void DoSomething()\r\n" + 
			    "\t{\r\n" + 
			    "\t}\r\n\r\n" + 
			    "\t#endregion Methods\r\n" +
			    "}",
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

			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "partial class TestClass : IDisposable\r\n" +
			    "{\r\n}",
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
			constructorElement.Name = "TestClass";
			constructorElement.Parameters = "int value";
			constructorElement.Reference = "base(value)";

			StringWriter writer = new StringWriter();
			codeElements.Add(constructorElement);

			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual("public TestClass(int value)\r\n\t: base(value)\r\n{\r\n}",
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
			constructorElement.Name = "TestClass";
			constructorElement.Parameters = "int value";

			StringWriter writer = new StringWriter();
			codeElements.Add(constructorElement);

			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual("public TestClass(int value)\r\n{\r\n}",
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
			delegateElement.Type = "int";
			delegateElement.Name = "Compare";
			delegateElement.Parameters = "T t1, T t2";
			delegateElement.AddTypeParameter(
				new TypeParameter("T", "class"));

			StringWriter writer = new StringWriter();
			codeElements.Add(delegateElement);

			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
				"public delegate int Compare<T>(T t1, T t2)\r\n\twhere T : class;",
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
			delegateElement.Type = "int";
			delegateElement.Name = "DoSomething";
			delegateElement.Parameters = "bool flag";

			StringWriter writer = new StringWriter();
			codeElements.Add(delegateElement);

			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
				"public delegate int DoSomething(bool flag);",
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

			StringWriter writer = new StringWriter();
			codeElements.Add(eventElement);

			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual("public event EventHandler TestEvent;",
				text,
				"Event element was not written correctly.");
		}

		/// <summary>
		/// Tests writing a fixed field.
		/// </summary>
		[Test]
		public void WriteFieldFixedTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			FieldElement fieldElement = new FieldElement();
			fieldElement.Access = CodeAccess.Public;
			fieldElement.Type = "char";
			fieldElement.Name = "pathName[128]";
			fieldElement[CSharpExtendedProperties.Fixed] = true;

			StringWriter writer = new StringWriter();
			codeElements.Add(fieldElement);

			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual("public fixed char pathName[128];",
			    text,
			    "FieldElement element was not written correctly.");
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
			fieldElement.Type = "Dictionary<string, int>";
			fieldElement.Name = "_test";
			fieldElement.InitialValue = "new Dictionary<string, int>()";

			StringWriter writer = new StringWriter();
			codeElements.Add(fieldElement);

			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual("private static Dictionary<string, int> _test = new Dictionary<string, int>();",
			    text,
			    "FieldElement element was not written correctly.");
		}

		/// <summary>
		/// Tests writing a new constant field.
		/// </summary>
		[Test]
		public void WriteFieldNewConstantTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			FieldElement fieldElement = new FieldElement();
			fieldElement.Access = CodeAccess.Public;
			fieldElement.MemberModifiers = MemberModifiers.Constant | MemberModifiers.New;
			fieldElement.Type = "string";
			fieldElement.Name = "Test";
			fieldElement.InitialValue = "\"Test\"";

			StringWriter writer = new StringWriter();
			codeElements.Add(fieldElement);

			CSharpWriter codeWriter = new CSharpWriter();
			codeWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual("public new const string Test = \"Test\";",
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
			fieldElement.Type = "int";
			fieldElement.Name = "_test";
			fieldElement.InitialValue = "1";

			StringWriter writer = new StringWriter();
			codeElements.Add(fieldElement);

			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual("private static int _test = 1;",
			    text,
			    "FieldElement element was not written correctly.");
		}

		/// <summary>
		/// Tests writing a volatile field.
		/// </summary>
		[Test]
		public void WriteFieldVolatileTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			FieldElement fieldElement = new FieldElement();
			fieldElement.Access = CodeAccess.Private;
			fieldElement.MemberModifiers = MemberModifiers.Static;
			fieldElement.IsVolatile = true;
			fieldElement.Type = "int";
			fieldElement.Name = "_test";
			fieldElement.InitialValue = "1";

			StringWriter writer = new StringWriter();
			codeElements.Add(fieldElement);

			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual("private static volatile int _test = 1;",
			    text,
			    "FieldElement element was not written correctly.");
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
			CSharpWriter csharpWriter = new CSharpWriter();

			writer = new StringWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "using System;\r\n" +
			    "using System.IO;\r\n" +
			    "using System.Text;",
			    text,
			    "Group was not written correctly.");

			group.SeparatorType = GroupSeparatorType.Custom;
			group.CustomSeparator = "\r\n";

			writer = new StringWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			text = writer.ToString();
			Assert.AreEqual(
			    "using System;\r\n\r\n" +
			    "using System.IO;\r\n\r\n" +
			    "using System.Text;",
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

			StringWriter writer = new StringWriter();
			codeElements.Add(classElement);

			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "public interface TestInterface : IDisposable\r\n" +
			    "{\r\n}",
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
			methodElement.Type = "void";
			methodElement.Name = "DoSomething";

			StringWriter writer = new StringWriter();
			codeElements.Add(methodElement);

			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual("protected abstract void DoSomething();",
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
			methodElement.Type = "IEnumerator<T>";
			methodElement.Name = "IEnumerable<T>.GetEnumerator";
			methodElement.BodyText = "\treturn null;";

			StringWriter writer = new StringWriter();
			codeElements.Add(methodElement);

			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "IEnumerator<T> IEnumerable<T>.GetEnumerator()\r\n" +
			    "{\r\n" +
			    "\treturn null;\r\n" +
			    "}"
			    ,
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
			methodElement.Type = "void";
			methodElement.Name = "DoSomething";
			methodElement.Parameters = "bool flag";
			methodElement.BodyText = null;

			StringWriter writer = new StringWriter();
			codeElements.Add(methodElement);

			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "partial private void DoSomething(bool flag);",
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
			methodElement.Type = "void";
			methodElement.Name = "DoSomething";
			methodElement.Parameters = "bool flag";
			methodElement.BodyText = "\treturn;";

			StringWriter writer = new StringWriter();
			codeElements.Add(methodElement);

			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "partial private void DoSomething(bool flag)\r\n" +
			    "{\r\n" +
			    "\treturn;\r\n" +
			    "}"
			    ,
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
			methodElement.Type = "void";
			methodElement.Name = "DoSomething";

			StringWriter writer = new StringWriter();
			codeElements.Add(methodElement);

			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual("public override sealed void DoSomething();",
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
			methodElement.Type = "int";
			methodElement.Name = "DoSomething";
			methodElement.Parameters = "bool flag";
			methodElement.BodyText = "\treturn 0;";

			StringWriter writer = new StringWriter();
			codeElements.Add(methodElement);

			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "public static int DoSomething(bool flag)\r\n" + 
			    "{\r\n" + 
			    "\treturn 0;\r\n" + 
			    "}"
			    ,
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
			operatorElement.Name = null;
			operatorElement.Access = CodeAccess.Public;
			operatorElement.MemberModifiers = MemberModifiers.Static;
			operatorElement.Type = "decimal";
			operatorElement.Parameters = "Fraction f";
			operatorElement.BodyText = "return (decimal)f.num / f.den;";

			StringWriter writer = new StringWriter();
			codeElements.Add(operatorElement);

			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "public static explicit operator decimal(Fraction f)\r\n" +
			    "{\r\n" +
			    "\treturn (decimal)f.num / f.den;\r\n" +
			    "}",
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
			operatorElement.Name = null;
			operatorElement.Access = CodeAccess.Public;
			operatorElement.MemberModifiers = MemberModifiers.Static;
			operatorElement.Type = "double";
			operatorElement.Parameters = "Fraction f";
			operatorElement.BodyText = "return (double)f.num / f.den;";

			StringWriter writer = new StringWriter();
			codeElements.Add(operatorElement);

			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "public static implicit operator double(Fraction f)\r\n" +
			    "{\r\n" +
			    "\treturn (double)f.num / f.den;\r\n" +
			    "}",
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
			operatorElement.Parameters = "Fraction a, Fraction b";
			operatorElement.BodyText = "return new Fraction(a.num * b.den + b.num * a.den, a.den * b.den);";

			StringWriter writer = new StringWriter();
			codeElements.Add(operatorElement);

			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "public static Fraction operator +(Fraction a, Fraction b)\r\n" +
			    "{\r\n" +
			    "\treturn new Fraction(a.num * b.den + b.num * a.den, a.den * b.den);\r\n" +
			    "}",
			    text,
			    "Operator element was not written correctly.");
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

			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
				"using SysText = System.Text;",
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

			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);

			string text = writer.ToString();
			Assert.AreEqual(
			    "//We'll be doing several text operations.\r\n" + 
			    "using System.Text;",
			    text,
			    "Using element was not written correctly.");
		}

		#endregion Public Methods
	}
}