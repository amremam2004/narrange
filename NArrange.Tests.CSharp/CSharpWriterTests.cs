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

namespace NArrange.Tests.CSharp
{
	/// <summary>
	/// Test fixture for the CSharpWriter class
	/// </summary>
	[TestFixture]
	public class CSharpWriterTests
	{
		#region Public Methods

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
			methodElement.BodyText = "\treturn false;";
			
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
			methodElement.BodyText = "\treturn false;";
			
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
		/// Tests writing a tree of arranged elements
		/// </summary>
		[Test]
		public void WriteArrangedElementTest()
		{
			CodeArranger arranger = new CodeArranger(CodeConfiguration.Default);
			
			ReadOnlyCollection<ICodeElement> testElements;
			
			CSharpTestFile testFile = CSharpTestUtilities.GetClassMembersFile();
			using (TextReader reader = testFile.GetReader())
			{
			    CSharpParser parser = new CSharpParser();
			    testElements = parser.Parse(reader);
			
			    Assert.IsTrue(testElements.Count > 0,
			        "Test file does not contain any elements.");
			}
			
			ReadOnlyCollection<ICodeElement> arranged = arranger.Arrange(testElements);
			
			//
			// Write the arranged elements
			//
			StringWriter writer = new StringWriter();
			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(arranged, writer);
			
			string text = writer.ToString();
			
			//
			// Verify that the arranged file still compiles sucessfully.
			//
			CompilerResults results = CSharpTestFile.Compile(text, "ArrangedClassMembers.cs");
			CompilerError error = CSharpTestFile.GetCompilerError(results);
			if (error != null)
			{
			    Assert.Fail("Arranged source code should not produce compiler errors. " +
			        "Error: {0} - {1}, line {2}, column {3} ",
			        error.ErrorText, "ArrangedClassMembers.cs",
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
			
			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);
			
			string text = writer.ToString();
			Assert.AreEqual(
			    "///<summary>We no longer need this...</summary>\r\n" +
			    "[Obsolete(\"This is obsolete\")]\r\n",
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
			classElement.TypeModifiers = TypeModifier.Static;
			classElement.Type = TypeElementType.Class;
			classElement.Name = "TestClass";
			classElement.TypeParameters.Add(
			    new TypeParameter("T", "class", "IDisposable", "new()"));
			classElement.AddInterface("IDisposable");
			
			StringWriter writer = new StringWriter();
			codeElements.Add(classElement);
			
			CSharpWriter csharpWriter = new CSharpWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);
			
			string text = writer.ToString();
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
			classElement.TypeModifiers = TypeModifier.Static | TypeModifier.Partial;
			classElement.Type = TypeElementType.Class;
			classElement.Name = "TestClass";
			classElement.TypeParameters.Add(
			    new TypeParameter("T", "class", "IDisposable", "new()"));
			classElement.AddInterface("IDisposable");
			
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
			classElement.TypeModifiers = TypeModifier.Sealed;
			classElement.Type = TypeElementType.Class;
			classElement.Name = "TestClass";
			classElement.AddInterface("IDisposable");
			classElement.AddInterface("IEnumerable");
			
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
			
			classElement.TypeModifiers = TypeModifier.Abstract;
			csharpWriter = new CSharpWriter();
			writer = new StringWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);
			
			text = writer.ToString();
			Assert.AreEqual(
			    "public abstract class TestClass : IDisposable, IEnumerable\r\n{\r\n}",
			    text,
			    "Class element was not written correctly.");
			
			classElement.TypeModifiers = TypeModifier.Static;
			csharpWriter = new CSharpWriter();
			writer = new StringWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);
			
			text = writer.ToString();
			Assert.AreEqual(
			    "public static class TestClass : IDisposable, IEnumerable\r\n{\r\n}",
			    text,
			    "Class element was not written correctly.");
			
			classElement.TypeModifiers = TypeModifier.Unsafe;
			csharpWriter = new CSharpWriter();
			writer = new StringWriter();
			csharpWriter.Write(codeElements.AsReadOnly(), writer);
			
			text = writer.ToString();
			Assert.AreEqual(
			    "public unsafe class TestClass : IDisposable, IEnumerable\r\n{\r\n}",
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
			
			FieldElement field = new FieldElement();
			field.Name = "_val";
			field.Access = CodeAccess.Private;
			field.Type = "int";
			
			fieldsRegion.AddChild(field);
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
			    "\tprivate int _val;\r\n\r\n" + 
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
			classElement.Access = CodeAccess.NotSpecified;
			classElement.TypeModifiers = TypeModifier.Partial;
			classElement.Type = TypeElementType.Class;
			classElement.Name = "TestClass";
			classElement.AddInterface("IDisposable");
			
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
			constructorElement.Params = "int value";
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
			constructorElement.Params = "int value";
			
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
		/// Tests writing an explicit operator
		/// </summary>
		[Test]
		public void WriteExplicitOperatorTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();
			
			MethodElement operatorElement = new MethodElement();
			operatorElement.IsOperator = true;
			operatorElement.OperatorType = OperatorType.Explicit;
			operatorElement.Name = null;
			operatorElement.Access = CodeAccess.Public;
			operatorElement.MemberModifiers = MemberModifier.Static;
			operatorElement.Type = "decimal";
			operatorElement.Params = "Fraction f";
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
		/// Tests writing a generic field.
		/// </summary>
		[Test]
		public void WriteFieldGenericTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();
			
			FieldElement fieldElement = new FieldElement();
			fieldElement.Access = CodeAccess.Private;
			fieldElement.MemberModifiers = MemberModifier.Static;
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
			    "FielElement element was not written correctly.");
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
			fieldElement.MemberModifiers = MemberModifier.Static;
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
			    "FielElement element was not written correctly.");
		}

		/// <summary>
		/// Tests writing an implicit operator
		/// </summary>
		[Test]
		public void WriteImplicitOperatorTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();
			
			MethodElement operatorElement = new MethodElement();
			operatorElement.IsOperator = true;
			operatorElement.OperatorType = OperatorType.Implicit;
			operatorElement.Name = null;
			operatorElement.Access = CodeAccess.Public;
			operatorElement.MemberModifiers = MemberModifier.Static;
			operatorElement.Type = "double";
			operatorElement.Params = "Fraction f";
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
			classElement.AddInterface("IDisposable");
			
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
		/// Tests calling Write with a null element collection.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void WriteNullElementsTest()
		{
			CSharpWriter writer = new CSharpWriter();
			writer.Write(null, new StringWriter());
		}

		/// <summary>
		/// Tests calling Write with a null writer.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void WriteNullWriterTest()
		{
			CSharpWriter writer = new CSharpWriter();
			writer.Write(new List<ICodeElement>().AsReadOnly(), null);
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
			operatorElement.MemberModifiers = MemberModifier.Static;
			operatorElement.Type = "Fraction";
			operatorElement.Params = "Fraction a, Fraction b";
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