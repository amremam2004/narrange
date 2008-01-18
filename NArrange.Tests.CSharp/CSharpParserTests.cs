using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

using NUnit.Framework;

using NArrange.CSharp;
using NArrange.Core;
using NArrange.Core.CodeElements;

namespace NArrange.Tests.CSharp
{
	/// <summary>
	/// Test fixture for the CSharpParser class.
	/// </summary>
	[TestFixture]
	public class CSharpParserTests	
	{
		#region Constants
		
		private const int NumConstructors = 4;		
		private const int NumDelegates = 1;		
		private const int NumEvents = 3;		
		private const int NumFields = 12;		
		private const int NumMethods = 7;		
		private const int NumNestedTypes = 4;		
		private const int NumProperties = 7;		
		
		#endregion Constants
		
		#region Public Methods
		
		/// <summary>
		/// Tests that when the end of a block is expected, the appropriate
		/// error is thrown.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException), ExpectedMessage = "Expected }: Line 4, Column 2")]
		public void ExpectedBlockCloseTest()		
		{
			using (TextReader reader = CSharpTestFile.GetTestFileReader("ExpectedBlockClose.cs"))
			{
			    CSharpParser parser = new CSharpParser();
			    parser.Parse(reader);
			}
		}		
		
		/// <summary>
		/// Tests that when an initial value is expected, the appropriate
		/// error is thrown.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException), ExpectedMessage = "Unexpected end of file. Expected ;: Line 9, Column 2")]
		public void ExpectedFieldEndOfStatementTest()		
		{
			using (TextReader reader = CSharpTestFile.GetTestFileReader("ExpectedFieldEndOfStatement.cs"))
			{
			    CSharpParser parser = new CSharpParser();
			    parser.Parse(reader);
			}
		}		
		
		/// <summary>
		/// Tests that when an initial value is expected, the appropriate
		/// error is thrown.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException), ExpectedMessage = "Expected an initial value: Line 7, Column 32")]
		public void ExpectedFieldInitialValueTest()		
		{
			using (TextReader reader = CSharpTestFile.GetTestFileReader("ExpectedFieldInitialValue.cs"))
			{
			    CSharpParser parser = new CSharpParser();
			    ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			}
		}		
		
		/// <summary>
		/// Tests the parsing of assembly attributes.
		/// </summary>
		[Test]
		public void ParseAssemblyAttributesTest()		
		{
			CSharpParser parser = new CSharpParser();
			
			CSharpTestFile testFile = CSharpTestUtilities.GetAssemblyAttributesFile();
			using (TextReader reader = testFile.GetReader())
			{
			    ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			    Assert.IsNotNull(elements, "Code element collection should not be null.");
			    Assert.AreEqual(16, elements.Count,
			        "An unexpected number of elements were parsed.");
			
			    //
			    // Using statements
			    //
			    UsingElement usingElement;
			
			    usingElement = elements[0] as UsingElement;
			    Assert.IsNotNull(usingElement,
			        "Element is not a UsingElement.");
			    Assert.AreEqual("System.Reflection", usingElement.Name,
			        "Unexpected name.");
			
			    usingElement = elements[1] as UsingElement;
			    Assert.IsNotNull(usingElement,
			        "Element is not a UsingElement.");
			    Assert.AreEqual("System.Runtime.CompilerServices", usingElement.Name,
			        "Unexpected name.");
			
			    usingElement = elements[2] as UsingElement;
			    Assert.IsNotNull(usingElement,
			        "Element is not a UsingElement.");
			    Assert.AreEqual("System.Runtime.InteropServices", usingElement.Name,
			        "Unexpected name.");
			
			    //
			    // Attributes
			    //
			    AttributeElement attributeElement;
			
			    attributeElement = elements[3] as AttributeElement;
			    Assert.IsNotNull(attributeElement,
			        "Element is not an AttributeElement.");
			    Assert.AreEqual("assembly: AssemblyTitle(\"NArrange.Core.Tests\")", attributeElement.BodyText,
			        "Unexpected attribute text.");
			    Assert.AreEqual(3, attributeElement.HeaderCommentLines.Count,
			        "An unexpected number of header comment lines were parsed.");
			
			    attributeElement = elements[4] as AttributeElement;
			    Assert.IsNotNull(attributeElement,
			        "Element is not an AttributeElement.");
			    Assert.AreEqual("assembly: AssemblyDescription(\"\")", attributeElement.BodyText,
			        "Unexpected attribute text.");
			    Assert.AreEqual(0, attributeElement.HeaderCommentLines.Count,
			        "An unexpected number of header comment lines were parsed.");
			
			    attributeElement = elements[5] as AttributeElement;
			    Assert.IsNotNull(attributeElement,
			        "Element is not an AttributeElement.");
			    Assert.AreEqual("assembly: AssemblyConfiguration(\"\")", attributeElement.BodyText,
			        "Unexpected attribute text.");
			    Assert.AreEqual(0, attributeElement.HeaderCommentLines.Count,
			        "An unexpected number of header comment lines were parsed.");
			
			    attributeElement = elements[6] as AttributeElement;
			    Assert.IsNotNull(attributeElement,
			        "Element is not an AttributeElement.");
			    Assert.AreEqual("assembly: AssemblyCompany(\"\")", attributeElement.BodyText,
			        "Unexpected attribute text.");
			    Assert.AreEqual(0, attributeElement.HeaderCommentLines.Count,
			        "An unexpected number of header comment lines were parsed.");
			
			    attributeElement = elements[7] as AttributeElement;
			    Assert.IsNotNull(attributeElement,
			        "Element is not an AttributeElement.");
			    Assert.AreEqual("assembly: AssemblyProduct(\"NArrange.Core.Tests\")", attributeElement.BodyText,
			        "Unexpected attribute text.");
			    Assert.AreEqual(0, attributeElement.HeaderCommentLines.Count,
			        "An unexpected number of header comment lines were parsed.");
			
			    attributeElement = elements[8] as AttributeElement;
			    Assert.IsNotNull(attributeElement,
			        "Element is not an AttributeElement.");
			    Assert.AreEqual("assembly: AssemblyCopyright(\"Copyright ©  2007\")", attributeElement.BodyText,
			        "Unexpected attribute text.");
			    Assert.AreEqual(0, attributeElement.HeaderCommentLines.Count,
			        "An unexpected number of header comment lines were parsed.");
			
			    attributeElement = elements[9] as AttributeElement;
			    Assert.IsNotNull(attributeElement,
			        "Element is not an AttributeElement.");
			    Assert.AreEqual("assembly: AssemblyTrademark(\"\")", attributeElement.BodyText,
			        "Unexpected attribute text.");
			    Assert.AreEqual(0, attributeElement.HeaderCommentLines.Count,
			        "An unexpected number of header comment lines were parsed.");
			
			    attributeElement = elements[10] as AttributeElement;
			    Assert.IsNotNull(attributeElement,
			        "Element is not an AttributeElement.");
			    Assert.AreEqual("assembly: AssemblyCulture(\"\")", attributeElement.BodyText,
			        "Unexpected attribute text.");
			    Assert.AreEqual(0, attributeElement.HeaderCommentLines.Count,
			        "An unexpected number of header comment lines were parsed.");
			
			    attributeElement = elements[11] as AttributeElement;
			    Assert.IsNotNull(attributeElement,
			        "Element is not an AttributeElement.");
			    Assert.AreEqual("assembly: ComVisible(false)", attributeElement.BodyText,
			        "Unexpected attribute text.");
			    Assert.AreEqual(3, attributeElement.HeaderCommentLines.Count,
			        "An unexpected number of header comment lines were parsed.");
			
			    attributeElement = elements[12] as AttributeElement;
			    Assert.IsNotNull(attributeElement,
			        "Element is not an AttributeElement.");
			    Assert.AreEqual("assembly: Guid(\"def01aba-79c5-4082-9522-e570c52a2df1\")", attributeElement.BodyText,
			        "Unexpected attribute text.");
			    Assert.AreEqual(1, attributeElement.HeaderCommentLines.Count,
			        "An unexpected number of header comment lines were parsed.");
			
			    attributeElement = elements[13] as AttributeElement;
			    Assert.IsNotNull(attributeElement,
			        "Element is not an AttributeElement.");
			    Assert.AreEqual("assembly: AssemblyVersion(\"1.0.0.0\")", attributeElement.BodyText,
			        "Unexpected attribute text.");
			    Assert.AreEqual(9, attributeElement.HeaderCommentLines.Count,
			        "An unexpected number of header comment lines were parsed.");
			
			    attributeElement = elements[14] as AttributeElement;
			    Assert.IsNotNull(attributeElement,
			        "Element is not an AttributeElement.");
			    Assert.AreEqual("assembly: AssemblyFileVersion(\"1.0.0.0\")", attributeElement.BodyText,
			        "Unexpected attribute text.");
			    Assert.AreEqual(0, attributeElement.HeaderCommentLines.Count,
			        "An unexpected number of header comment lines were parsed.");
			
			    //
			    // Namespace
			    //
			    NamespaceElement namespaceElement = elements[15] as NamespaceElement;
			    Assert.IsNotNull(namespaceElement, "Expected a NamespaceElement.");
			    Assert.AreEqual("SampleNamespace", namespaceElement.Name,
			        "Unexpected namespace name.");
			    Assert.IsNotNull(namespaceElement.Children,
			        "Children collection should not be null.");
			    Assert.AreEqual(0, namespaceElement.Children.Count,
			        "Children collection should not be null.");
			    Assert.AreEqual(3, namespaceElement.HeaderCommentLines.Count,
			        "An unexpected number of header comment lines were parsed.");
			}
		}		
		
		/// <summary>
		/// Tests parsing an attribute where the closing is expected.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException),
           MatchType = MessageMatch.Contains,
          ExpectedMessage = "Unexpected end of file. Expected ]")]
		public void ParseAttributeCloseExpectedTest()		
		{
			StringReader reader = new StringReader(
			    "[assembly: AssemblyDescription(\"SampleAssembly\")");
			
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
		}		
		
		/// <summary>
		/// Tests parsing an attribute.
		/// </summary>
		[Test]
		public void ParseAttributeTest()		
		{
			StringReader reader = new StringReader(
			    "[assembly: AssemblyDescription(\"SampleAssembly\")]");
			
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			Assert.AreEqual(1, elements.Count,
			    "An unexpected number of elements were parsed.");
			AttributeElement attributeElement = elements[0] as AttributeElement;
			Assert.IsNotNull(attributeElement,
			    "Element is not a AttributeElement.");
			Assert.AreEqual(
			    "assembly: AssemblyDescription(\"SampleAssembly\")", 
			    attributeElement.BodyText,
			    "Unexpected attribute text.");
		}		
		
		/// <summary>
		/// Tests parsing an attribute that contains an attribute
		/// character in a string.
		/// </summary>
		[Test]
		public void ParseAttributeWithAttributeCharacterTest()		
		{
			StringReader reader = new StringReader(
			    "[assembly: AssemblyDescription(\"SampleAssembly]\")]");
			
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			Assert.AreEqual(1, elements.Count,
			    "An unexpected number of elements were parsed.");
			AttributeElement attributeElement = elements[0] as AttributeElement;
			Assert.IsNotNull(attributeElement,
			    "Element is not a AttributeElement.");
			Assert.AreEqual(
			    "assembly: AssemblyDescription(\"SampleAssembly]\")",
			    attributeElement.BodyText,
			    "Unexpected attribute text.");
		}		
		
		/// <summary>
		/// Tests the parsing of a single namespace with a single class
		/// definition.
		/// </summary>
		[Test]
		public void ParseClassDefinitionTest()		
		{
			CSharpParser parser = new CSharpParser();
			
			CSharpTestFile testFile = CSharpTestUtilities.GetClassDefinitionFile();
			using (TextReader reader = testFile.GetReader())
			{
			    ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			    Assert.IsNotNull(elements, "Code element collection should not be null.");
			    Assert.AreEqual(4, elements.Count,
			        "An unexpected number of elements were parsed.");
			
			    UsingElement using1 = elements[0] as UsingElement;
			    Assert.IsNotNull(using1, "Expected a UsingElement.");
			    Assert.AreEqual("System", using1.Name,
			        "Unexpected using name.");
			
			    UsingElement using2 = elements[1] as UsingElement;
			    Assert.IsNotNull(using2, "Expected a UsingElement.");
			    Assert.AreEqual("System.Collections.Generic", using2.Name,
			        "Unexpected using name.");
			
			    UsingElement using3 = elements[2] as UsingElement;
			    Assert.IsNotNull(using3, "Expected a UsingElement.");
			    Assert.AreEqual("System.Text", using3.Name,
			        "Unexpected using name.");
			
			    NamespaceElement namespaceElement = elements[3] as NamespaceElement;
			    Assert.IsNotNull(namespaceElement, "Expected a NamespaceElement.");
			    Assert.AreEqual("SampleNamespace", namespaceElement.Name,
			        "Unexpected namespace name.");
			
			    Assert.IsNotNull(namespaceElement.Children,
			        "Namespace Children collection should not be null.");
			    Assert.AreEqual(1, namespaceElement.Children.Count,
			        "An unexpected number of namespace child elements were parsed.");
			
			    TypeElement classElement = namespaceElement.Children[0] as TypeElement;
			    Assert.IsNotNull(classElement, "Expected a TypeElement.");
			    Assert.AreEqual("SampleClass", classElement.Name,
			        "Unexpected class name.");
			    Assert.AreEqual(3, classElement.HeaderCommentLines.Count,
			        "An unexpected number of class header comment lines were parsed.");
			    foreach (ICommentLine commentLine in
			        classElement.HeaderCommentLines)
			    {
			        Assert.IsTrue(commentLine.IsXmlComment,
			            "Class header comment should be an XML comment.");
			    }
			    Assert.AreEqual(CodeAccess.Public, classElement.Access,
			        "Unexpected class code access level.");
			}
		}		
		
		/// <summary>
		/// Tests parsing a class with unspecified access.
		/// </summary>
		[Test]
		public void ParseClassPartialUnspecifiedAccessTest()		
		{
			StringReader reader = new StringReader(
			    "partial class Test{}");
			
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			Assert.AreEqual(1, elements.Count,
			    "An unexpected number of elements were parsed.");
			TypeElement typeElement = elements[0] as TypeElement;
			Assert.IsNotNull(typeElement,
			    "Element is not a TypeElement.");
			Assert.AreEqual("Test", typeElement.Name,
			    "Unexpected name.");
			Assert.AreEqual(CodeAccess.NotSpecified, typeElement.Access,
			    "Unexpected code access.");
			Assert.IsTrue(typeElement.IsPartial,
			    "Expected a partial class.");
			Assert.AreEqual(TypeElementType.Class, typeElement.Type,
			    "Unexpected type element type.");
		}		
		
		/// <summary>
		/// Tests parsing a constructor with parameters.
		/// </summary>
		[Test]
		public void ParseConstructorParametersTest()		
		{
			StringReader reader = new StringReader(
			    "public TestClass(int value, int max){}");
			
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			Assert.AreEqual(1, elements.Count,
			    "An unexpected number of elements were parsed.");
			ConstructorElement constructorElement = elements[0] as ConstructorElement;
			Assert.IsNotNull(constructorElement,
			    "Element is not a ConstructorElement.");
			Assert.AreEqual("TestClass", constructorElement.Name,
			    "Unexpected name.");
			Assert.AreEqual(CodeAccess.Public, constructorElement.Access,
			    "Unexpected code access.");
			Assert.AreEqual("int value, int max", constructorElement.Params,
			    "Unexpected parameter string.");
		}		
		
		/// <summary>
		/// Tests parsing a constructor with constructor reference.
		/// </summary>
		[Test]
		public void ParseConstructorReferenceTest()		
		{
			StringReader reader = new StringReader(
			    "public TestClass(int value, int max) : base(value){}");
			
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			Assert.AreEqual(1, elements.Count,
			    "An unexpected number of elements were parsed.");
			ConstructorElement constructorElement = elements[0] as ConstructorElement;
			Assert.IsNotNull(constructorElement,
			    "Element is not a ConstructorElement.");
			Assert.AreEqual("TestClass", constructorElement.Name,
			    "Unexpected name.");
			Assert.AreEqual(CodeAccess.Public, constructorElement.Access,
			    "Unexpected code access.");
			Assert.AreEqual("int value, int max", constructorElement.Params,
			    "Unexpected parameter string.");
			Assert.AreEqual("base(value)", constructorElement.Reference,
			    "Unexpected constructor reference.");
		}		
		
		/// <summary>
		/// Tests parsing a constructor with constructor reference.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException), ExpectedMessage="Unexpected end of file",
            MatchType=MessageMatch.Contains)]
		public void ParseConstructorReferenceUnexpectedEofTest()		
		{
			StringReader reader = new StringReader(
			    "public TestClass(int value, int max) : base");
			
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
		}		
		
		/// <summary>
		/// Tests parsing a constructor.
		/// </summary>
		[Test]
		public void ParseConstructorTest()		
		{
			StringReader reader = new StringReader(
			    "public TestClass(){}");
			
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			Assert.AreEqual(1, elements.Count,
			    "An unexpected number of elements were parsed.");
			ConstructorElement constructorElement = elements[0] as ConstructorElement;
			Assert.IsNotNull(constructorElement,
			    "Element is not a ConstructorElement.");
			Assert.AreEqual("TestClass", constructorElement.Name,
			    "Unexpected name.");
			Assert.AreEqual(CodeAccess.Public, constructorElement.Access,
			    "Unexpected code access.");
			Assert.AreEqual(string.Empty, constructorElement.Params,
			    "Unexpected parameter string.");
		}		
		
		/// <summary>
		/// Verifies the parsing of constructor members from the 
		/// sample class.
		/// </summary>
		[Test]
		public void ParseConstructorsTest()		
		{
			CSharpTestFile testFile = CSharpTestUtilities.GetClassMembersFile();
			using (TextReader reader = testFile.GetReader())
			{
			    TypeElement classElement = GetMembersTestClass(reader);
			
			    ConstructorElement constructor;
			
			    int constructorOffset = NumFields;
			
			    constructor = classElement.Children[constructorOffset + 0] as ConstructorElement;
			    Assert.IsNotNull(constructor, "Expected a constructor.");
			    Assert.AreEqual("SampleClass", constructor.Name,
			        "Unexpected constructor name.");
			    Assert.AreEqual(CodeAccess.Public, constructor.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(3, constructor.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(constructor.IsStatic,
			        "Constructor should not be static.");
			    Assert.IsEmpty(constructor.Params,
			        "Parameter string should be empty.");
			
			    constructor = classElement.Children[constructorOffset + 1] as ConstructorElement;
			    Assert.IsNotNull(constructor, "Expected a constructor.");
			    Assert.AreEqual("SampleClass", constructor.Name,
			        "Unexpected constructor name.");
			    Assert.AreEqual(CodeAccess.Internal, constructor.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(4, constructor.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(constructor.IsStatic,
			        "Constructor should not be static.");
			    Assert.AreEqual("string[] arrayParam", constructor.Params,
			        "Unexpected parameters string.");
			
			    constructor = classElement.Children[constructorOffset + 2] as ConstructorElement;
			    Assert.IsNotNull(constructor, "Expected a constructor.");
			    Assert.AreEqual("SampleClass", constructor.Name,
			        "Unexpected constructor name.");
			    Assert.AreEqual(CodeAccess.NotSpecified, constructor.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(1, constructor.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsTrue(constructor.IsStatic,
			        "Constructor should be static.");
			    Assert.IsEmpty(constructor.Params,
			        "Parameter string should be empty.");
			
			    constructor = classElement.Children[constructorOffset + 3] as ConstructorElement;
			    Assert.IsNotNull(constructor, "Expected a constructor.");
			    Assert.AreEqual("~SampleClass", constructor.Name,
			        "Unexpected constructor name.");
			    Assert.AreEqual(CodeAccess.NotSpecified, constructor.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(4, constructor.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(constructor.IsStatic,
			        "Constructor should not be static.");
			    Assert.IsEmpty(constructor.Params,
			        "Parameter string should be empty.");
			}
		}		
		
		/// <summary>
		/// Verifies the parsing of delegates from the 
		/// sample class.
		/// </summary>
		[Test]
		public void ParseDelegatesTest()		
		{
			CSharpTestFile testFile = CSharpTestUtilities.GetClassMembersFile();
			using (TextReader reader = testFile.GetReader())
			{
			    TypeElement classElement = GetMembersTestClass(reader);
			
			    DelegateElement delegateElement;
			
			    int delegateOffset = NumFields + NumConstructors + NumProperties + NumMethods;
			
			    delegateElement = classElement.Children[delegateOffset + 0] as DelegateElement;
			    Assert.IsNotNull(delegateElement, "Expected a delegate.");
			    Assert.AreEqual("SampleEventHandler", delegateElement.Name,
			        "Unexpected delegate name.");
			    Assert.AreEqual(CodeAccess.Public, delegateElement.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(5, delegateElement.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(delegateElement.IsStatic,
			        "Delegate should not be static.");
			    Assert.AreEqual("void", delegateElement.Type,
			        "Unexpected return type.");
			    Assert.IsFalse(delegateElement.IsAbstract,
			        "Delegate should not be abstract.");
			    Assert.IsFalse(delegateElement.IsOverride,
			        "Delegate should not be an override.");
			    Assert.IsFalse(delegateElement.IsSealed,
			        "Delegate should not be sealed.");
			    Assert.IsFalse(delegateElement.IsVirtual,
			        "Delegate should not be virtual.");
			    Assert.IsFalse(delegateElement.IsNew,
			        "Delegate should not be new.");
			    Assert.AreEqual(0, delegateElement.Attributes.Count,
			        "Unexpected number of attributes.");
			    Assert.AreEqual("object sender, bool boolParam", delegateElement.Params,
			        "Unexpected parameter string.");
			}
		}		
		
		/// <summary>
		/// Verifies the parsing of events from the 
		/// sample class.
		/// </summary>
		[Test]
		public void ParseEventsTest()		
		{
			CSharpTestFile testFile = CSharpTestUtilities.GetClassMembersFile();
			using (TextReader reader = testFile.GetReader())
			{
			    TypeElement classElement = GetMembersTestClass(reader);
			
			    EventElement eventElement;
			
			    int eventOffset = NumFields + NumConstructors + 
			        NumProperties + NumMethods + NumDelegates;
			
			    eventElement = classElement.Children[eventOffset + 0] as EventElement;
			    Assert.IsNotNull(eventElement, "Expected an event.");
			    Assert.AreEqual("SimpleEvent", eventElement.Name,
			        "Unexpected event name.");
			    Assert.AreEqual(CodeAccess.Public, eventElement.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(3, eventElement.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(eventElement.IsStatic,
			        "Delegate should not be static.");
			    Assert.AreEqual("SampleEventHandler", eventElement.Type,
			        "Unexpected return type.");
			    Assert.IsFalse(eventElement.IsAbstract,
			        "Event should not be abstract.");
			    Assert.IsFalse(eventElement.IsOverride,
			        "Event should not be an override.");
			    Assert.IsFalse(eventElement.IsSealed,
			        "Event should not be sealed.");
			    Assert.IsFalse(eventElement.IsVirtual,
			        "Event should not be virtual.");
			    Assert.IsFalse(eventElement.IsNew,
			        "Event should not be new.");
			    Assert.AreEqual(0, eventElement.Attributes.Count,
			        "Unexpected number of attributes.");
			    Assert.IsNull(eventElement.BodyText,
			        "Unexpected body text.");
			
			    eventElement = classElement.Children[eventOffset + 1] as EventElement;
			    Assert.IsNotNull(eventElement, "Expected an event.");
			    Assert.AreEqual("GenericEvent", eventElement.Name,
			        "Unexpected event name.");
			    Assert.AreEqual(CodeAccess.Public, eventElement.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(3, eventElement.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(eventElement.IsStatic,
			        "Delegate should not be static.");
			    Assert.AreEqual("EventHandler<EventArgs>", eventElement.Type,
			        "Unexpected return type.");
			    Assert.IsFalse(eventElement.IsAbstract,
			        "Event should not be abstract.");
			    Assert.IsFalse(eventElement.IsOverride,
			        "Event should not be an override.");
			    Assert.IsFalse(eventElement.IsSealed,
			        "Event should not be sealed.");
			    Assert.IsFalse(eventElement.IsVirtual,
			        "Event should not be virtual.");
			    Assert.IsFalse(eventElement.IsNew,
			        "Event should not be new.");
			    Assert.AreEqual(0, eventElement.Attributes.Count,
			        "Unexpected number of attributes.");
			    Assert.IsNull(eventElement.BodyText,
			        "Unexpected body text.");
			
			    eventElement = classElement.Children[eventOffset + 2] as EventElement;
			    Assert.IsNotNull(eventElement, "Expected an event.");
			    Assert.AreEqual("ExplicitEvent", eventElement.Name,
			        "Unexpected event name.");
			    Assert.AreEqual(CodeAccess.Public, eventElement.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(3, eventElement.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(eventElement.IsStatic,
			        "Delegate should not be static.");
			    Assert.AreEqual("SampleEventHandler", eventElement.Type,
			        "Unexpected return type.");
			    Assert.IsFalse(eventElement.IsAbstract,
			        "Event should not be abstract.");
			    Assert.IsFalse(eventElement.IsOverride,
			        "Event should not be an override.");
			    Assert.IsFalse(eventElement.IsSealed,
			        "Event should not be sealed.");
			    Assert.IsFalse(eventElement.IsVirtual,
			        "Event should not be virtual.");
			    Assert.IsFalse(eventElement.IsNew,
			        "Event should not be new.");
			    Assert.AreEqual(0, eventElement.Attributes.Count,
			        "Unexpected number of attributes.");
			    Assert.IsTrue(eventElement.BodyText.Contains("add"),
			        "Unexpected body text.");
			    Assert.IsTrue(eventElement.BodyText.Contains("remove"),
			        "Unexpected body text.");
			}
		}		
		
		/// <summary>
		/// Tests parsing a generic field.
		/// </summary>
		[Test]
		public void ParseFieldGenericTest()		
		{
			StringReader reader = new StringReader(
			    "private static Dictionary<string, int> _dictionary = new Dictionary<string, int>();");
			
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			Assert.AreEqual(1, elements.Count,
			    "An unexpected number of elements were parsed.");
			FieldElement fieldElement = elements[0] as FieldElement;
			Assert.IsNotNull(fieldElement,
			    "Element is not a FieldElement.");
			Assert.AreEqual("_dictionary", fieldElement.Name,
			    "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, fieldElement.Access,
			    "Unexpected code access.");
			Assert.IsTrue(fieldElement.IsStatic,
			    "Unexpected value for IsStatic.");
			Assert.AreEqual("Dictionary<string, int>", fieldElement.Type,
			    "Unexpected member type.");
			Assert.AreEqual("new Dictionary<string, int>()", fieldElement.InitialValue,
			    "Unexpected initial value.");
		}		
		
		/// <summary>
		/// Tests parsing a field with an initial value that contains
		/// character symbols.
		/// </summary>
		[Test]
		public void ParseFieldWithCharInitialValueTest()		
		{
			StringReader reader = new StringReader(
			    "private char val = 'X';");
			
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			Assert.AreEqual(1, elements.Count,
			    "An unexpected number of elements were parsed.");
			FieldElement fieldElement = elements[0] as FieldElement;
			Assert.IsNotNull(fieldElement,
			    "Element is not a FieldElement.");
			Assert.AreEqual("val", fieldElement.Name,
			    "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, fieldElement.Access,
			    "Unexpected code access.");
			Assert.AreEqual("char", fieldElement.Type,
			    "Unexpected member type.");
			Assert.AreEqual("'X'", fieldElement.InitialValue,
			    "Unexpected initial value.");
		}		
		
		/// <summary>
		/// Tests parsing a field with an initial value.
		/// </summary>
		[Test]
		public void ParseFieldWithInitialValueTest()		
		{
			StringReader reader = new StringReader(
			    "private int val = 17;");
			
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			Assert.AreEqual(1, elements.Count,
			    "An unexpected number of elements were parsed.");
			FieldElement fieldElement = elements[0] as FieldElement;
			Assert.IsNotNull(fieldElement,
			    "Element is not a FieldElement.");
			Assert.AreEqual("val", fieldElement.Name,
			    "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, fieldElement.Access,
			    "Unexpected code access.");
			Assert.AreEqual("int", fieldElement.Type,
			    "Unexpected member type.");
			Assert.AreEqual("17", fieldElement.InitialValue,
			    "Unexpected initial value.");
		}		
		
		/// <summary>
		/// Tests parsing a field with an initial value that contains
		/// character symbols.
		/// </summary>
		[Test]
		public void ParseFieldWithNestedCharInitialValueTest()		
		{
			string fieldText = "private char val = '\\'';";
			StringReader reader = new StringReader(fieldText);
			
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			Assert.AreEqual(1, elements.Count,
			    "An unexpected number of elements were parsed.");
			FieldElement fieldElement = elements[0] as FieldElement;
			Assert.IsNotNull(fieldElement,
			    "Element is not a FieldElement.");
			Assert.AreEqual("val", fieldElement.Name,
			    "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, fieldElement.Access,
			    "Unexpected code access.");
			Assert.AreEqual("char", fieldElement.Type,
			    "Unexpected member type.");
			Assert.AreEqual("'\\''", fieldElement.InitialValue,
			    "Unexpected initial value.");
		}		
		
		/// <summary>
		/// Verifies the parsing of field members from the 
		/// sample class.
		/// </summary>
		[Test]
		public void ParseFieldsTest()		
		{
			CSharpTestFile testFile = CSharpTestUtilities.GetClassMembersFile();
			using (TextReader reader = testFile.GetReader())
			{
			    TypeElement classElement = GetMembersTestClass(reader);
			
			    FieldElement field;
			
			    field = classElement.Children[0] as FieldElement;
			    Assert.IsNotNull(field, "Expected a field.");
			    Assert.AreEqual("_simpleField", field.Name,
			        "Unexpected field name.");
			    Assert.AreEqual("bool", field.Type,
			        "Unexpected field type.");
			    Assert.AreEqual(CodeAccess.Private, field.Access,
			        "Unexpected field access level.");
			    Assert.IsNull(field.InitialValue,
			        "Unexpected field initial value.");
			    Assert.AreEqual(1, field.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(field.IsStatic,
			        "Field should not be static.");
			    Assert.IsFalse(field.IsReadOnly,
			        "Field should not be readonly.");
			    Assert.IsFalse(field.IsConstant,
			       "Field should not be a constant.");
			
			    field = classElement.Children[1] as FieldElement;
			    Assert.IsNotNull(field, "Expected a field.");
			    Assert.AreEqual("_fieldWithInitialVal", field.Name,
			        "Unexpected field name.");
			    Assert.AreEqual("int", field.Type,
			        "Unexpected field type.");
			    Assert.AreEqual(CodeAccess.Private, field.Access,
			        "Unexpected field access level.");
			    Assert.AreEqual("1", field.InitialValue,
			        "Unexpected field initial value.");
			    Assert.AreEqual(0, field.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(field.IsStatic,
			        "Field should not be static.");
			
			    field = classElement.Children[2] as FieldElement;
			    Assert.IsNotNull(field, "Expected a field.");
			    Assert.AreEqual("StaticStr", field.Name,
			        "Unexpected field name.");
			    Assert.AreEqual("string", field.Type,
			        "Unexpected field type.");
			    Assert.AreEqual(CodeAccess.Protected, field.Access,
			        "Unexpected field access level.");
			    Assert.AreEqual("\"static; string;\"", field.InitialValue,
			        "Unexpected field initial value.");
			    Assert.AreEqual(3, field.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsTrue(field.IsStatic,
			        "Field should be static.");
			    Assert.IsTrue(field.IsReadOnly,
			        "Field should be readonly.");
			
			    field = classElement.Children[3] as FieldElement;
			    Assert.IsNotNull(field, "Expected a field.");
			    Assert.AreEqual("_genericField", field.Name,
			        "Unexpected field name.");
			    Assert.AreEqual("Nullable<int>", field.Type,
			        "Unexpected field type.");
			    Assert.AreEqual(CodeAccess.Private, field.Access,
			        "Unexpected field access level.");
			    Assert.IsNull(field.InitialValue,
			        "Unexpected field initial value.");
			    Assert.AreEqual(0, field.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(field.IsStatic,
			        "Field should not be static.");
			
			    field = classElement.Children[4] as FieldElement;
			    Assert.IsNotNull(field, "Expected a field.");
			    Assert.AreEqual("_arrayField", field.Name,
			        "Unexpected field name.");
			    Assert.AreEqual("string[]", field.Type,
			        "Unexpected field type.");
			    Assert.AreEqual(CodeAccess.Protected | CodeAccess.Internal, field.Access,
			        "Unexpected field access level.");
			    Assert.AreEqual("{ }", field.InitialValue,
			        "Unexpected field initial value.");
			    Assert.AreEqual(0, field.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(field.IsStatic,
			        "Field should not be static.");
			
			    field = classElement.Children[5] as FieldElement;
			    Assert.IsNotNull(field, "Expected a field.");
			    Assert.AreEqual("@internal", field.Name,
			        "Unexpected field name.");
			    Assert.AreEqual("bool", field.Type,
			        "Unexpected field type.");
			    Assert.AreEqual(CodeAccess.Internal, field.Access,
			        "Unexpected field access level.");
			    Assert.IsNull(field.InitialValue,
			        "Unexpected field initial value.");
			    Assert.AreEqual(0, field.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(field.IsStatic,
			        "Field should not be static.");
			
			    field = classElement.Children[6] as FieldElement;
			    Assert.IsNotNull(field, "Expected a field.");
			    Assert.AreEqual("_globalNamespaceTypeField", field.Name,
			        "Unexpected field name.");
			    Assert.AreEqual("global::System.Boolean", field.Type,
			        "Unexpected field type.");
			    Assert.AreEqual(CodeAccess.Private, field.Access,
			        "Unexpected field access level.");
			    Assert.IsNull(field.InitialValue,
			        "Unexpected field initial value.");
			    Assert.AreEqual(0, field.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(field.IsStatic,
			        "Field should not be static.");
			
			    field = classElement.Children[7] as FieldElement;
			    Assert.IsNotNull(field, "Expected a field.");
			    Assert.AreEqual("_attributedField", field.Name,
			        "Unexpected field name.");
			    Assert.AreEqual("string", field.Type,
			        "Unexpected field type.");
			    Assert.AreEqual(CodeAccess.Private, field.Access,
			        "Unexpected field access level.");
			    Assert.IsTrue(field.InitialValue.Contains("null"),
			        "Unexpected field initial value.");
			    Assert.AreEqual(3, field.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsTrue(field.IsStatic,
			        "Field should be static.");
			    Assert.AreEqual(1, field.Attributes.Count,
			        "Unexpected number of attributes.");
			
			    field = classElement.Children[8] as FieldElement;
			    Assert.IsNotNull(field, "Expected a field.");
			    Assert.AreEqual("ConstantStr", field.Name,
			        "Unexpected field name.");
			    Assert.AreEqual("string", field.Type,
			        "Unexpected field type.");
			    Assert.AreEqual(CodeAccess.Public, field.Access,
			        "Unexpected field access level.");
			    Assert.IsTrue(field.InitialValue.Contains("\"constant string\""), 
			        "Unexpected field initial value.");
			    Assert.AreEqual(0, field.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(field.IsStatic,
			        "Field should not be static.");
			    Assert.AreEqual(0, field.Attributes.Count,
			        "Unexpected number of attributes.");
			    Assert.IsTrue(field.IsConstant,
			       "Field should be a constant.");
			    Assert.IsFalse(field.IsReadOnly,
			       "Field should not be readonly.");
			
			    field = classElement.Children[9] as FieldElement;
			    Assert.IsNotNull(field, "Expected a field.");
			    Assert.AreEqual("_volatileField", field.Name,
			        "Unexpected field name.");
			    Assert.AreEqual("int", field.Type,
			        "Unexpected field type.");
			    Assert.AreEqual(CodeAccess.Private, field.Access,
			        "Unexpected field access level.");
			    Assert.IsNull(field.InitialValue,
			        "Unexpected field initial value.");
			    Assert.AreEqual(0, field.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsTrue(field.IsVolatile,
			        "Field should be volatile.");
			    Assert.IsFalse(field.IsStatic,
			        "Field should not be static.");
			    Assert.AreEqual(0, field.Attributes.Count,
			        "Unexpected number of attributes.");
			    Assert.IsFalse(field.IsConstant,
			       "Field should not be a constant.");
			    Assert.IsFalse(field.IsReadOnly,
			       "Field should not be a readonly.");
			
			    field = classElement.Children[10] as FieldElement;
			    Assert.IsNotNull(field, "Expected a field.");
			    Assert.AreEqual("_val1, _val2", field.Name,
			        "Unexpected field name.");
			    Assert.AreEqual("int", field.Type,
			        "Unexpected field type.");
			    Assert.AreEqual(CodeAccess.Private, field.Access,
			        "Unexpected field access level.");
			    Assert.IsNull(field.InitialValue,
			        "Unexpected field initial value.");
			    Assert.AreEqual(0, field.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(field.IsVolatile,
			        "Field should not be volatile.");
			    Assert.IsFalse(field.IsStatic,
			        "Field should not be static.");
			    Assert.AreEqual(0, field.Attributes.Count,
			        "Unexpected number of attributes.");
			    Assert.IsFalse(field.IsConstant,
			       "Field should not be a constant.");
			    Assert.IsFalse(field.IsReadOnly,
			       "Field should not be a readonly.");
			
			    field = classElement.Children[11] as FieldElement;
			    Assert.IsNotNull(field, "Expected a field.");
			    Assert.AreEqual("_val3, _val4, _val5, _val6", field.Name,
			        "Unexpected field name.");
			    Assert.AreEqual("int", field.Type,
			        "Unexpected field type.");
			    Assert.AreEqual(CodeAccess.Private, field.Access,
			        "Unexpected field access level.");
			    Assert.AreEqual("10", field.InitialValue,
			        "Unexpected field initial value.");
			    Assert.AreEqual(0, field.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(field.IsVolatile,
			        "Field should not be volatile.");
			    Assert.IsFalse(field.IsStatic,
			        "Field should not be static.");
			    Assert.AreEqual(0, field.Attributes.Count,
			        "Unexpected number of attributes.");
			    Assert.IsFalse(field.IsConstant,
			       "Field should not be a constant.");
			    Assert.IsFalse(field.IsReadOnly,
			       "Field should not be a readonly.");
			}
		}		
		
		/// <summary>
		/// Tests parsing header comments.
		/// </summary>
		[Test]
		public void ParseHeaderCommentsBlockTest()		
		{
			StringReader reader = new StringReader(
			    "/*Comment1\r\nComment2*/</summary>\r\npublic TestClass(){}");
			
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			Assert.AreEqual(1, elements.Count,
			    "An unexpected number of elements were parsed.");
			ConstructorElement constructorElement = elements[0] as ConstructorElement;
			Assert.IsNotNull(constructorElement,
			    "Element is not a ConstructorElement.");
			Assert.AreEqual("TestClass", constructorElement.Name,
			    "Unexpected name.");
			Assert.AreEqual(CodeAccess.Public, constructorElement.Access,
			    "Unexpected code access.");
			Assert.AreEqual(string.Empty, constructorElement.Params,
			    "Unexpected parameter string.");
			Assert.AreEqual(2, constructorElement.HeaderCommentLines.Count,
			    "Unexpected number of header comment lines.");
			Assert.AreEqual("Comment1", constructorElement.HeaderCommentLines[0].Text);
			Assert.IsFalse(constructorElement.HeaderCommentLines[0].IsXmlComment);
			Assert.AreEqual("Comment2", constructorElement.HeaderCommentLines[1].Text);
			Assert.IsFalse(constructorElement.HeaderCommentLines[1].IsXmlComment);
		}		
		
		/// <summary>
		/// Tests parsing header comments.
		/// </summary>
		[Test]
		public void ParseHeaderCommentsTest()		
		{
			StringReader reader = new StringReader(
			    "//Comment1\r\n//Comment2\r\npublic TestClass(){}");
			
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			Assert.AreEqual(1, elements.Count,
			    "An unexpected number of elements were parsed.");
			ConstructorElement constructorElement = elements[0] as ConstructorElement;
			Assert.IsNotNull(constructorElement,
			    "Element is not a ConstructorElement.");
			Assert.AreEqual("TestClass", constructorElement.Name,
			    "Unexpected name.");
			Assert.AreEqual(CodeAccess.Public, constructorElement.Access,
			    "Unexpected code access.");
			Assert.AreEqual(string.Empty, constructorElement.Params,
			    "Unexpected parameter string.");
			Assert.AreEqual(2, constructorElement.HeaderCommentLines.Count,
			    "Unexpected number of header comment lines.");
			Assert.AreEqual("Comment1", constructorElement.HeaderCommentLines[0].Text);
			Assert.IsFalse(constructorElement.HeaderCommentLines[0].IsXmlComment);
			Assert.AreEqual("Comment2", constructorElement.HeaderCommentLines[1].Text);
			Assert.IsFalse(constructorElement.HeaderCommentLines[1].IsXmlComment);
		}		
		
		/// <summary>
		/// Tests parsing header comments.
		/// </summary>
		[Test]
		public void ParseHeaderCommentsXmlTest()		
		{
			StringReader reader = new StringReader(
			    "///<summary>Comment1\r\n///Comment2</summary>\r\npublic TestClass(){}");
			
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			Assert.AreEqual(1, elements.Count,
			    "An unexpected number of elements were parsed.");
			ConstructorElement constructorElement = elements[0] as ConstructorElement;
			Assert.IsNotNull(constructorElement,
			    "Element is not a ConstructorElement.");
			Assert.AreEqual("TestClass", constructorElement.Name,
			    "Unexpected name.");
			Assert.AreEqual(CodeAccess.Public, constructorElement.Access,
			    "Unexpected code access.");
			Assert.AreEqual(string.Empty, constructorElement.Params,
			    "Unexpected parameter string.");
			Assert.AreEqual(2, constructorElement.HeaderCommentLines.Count,
			    "Unexpected number of header comment lines.");
			Assert.AreEqual("<summary>Comment1", constructorElement.HeaderCommentLines[0].Text);
			Assert.IsTrue(constructorElement.HeaderCommentLines[0].IsXmlComment);
			Assert.AreEqual("Comment2</summary>", constructorElement.HeaderCommentLines[1].Text);
			Assert.IsTrue(constructorElement.HeaderCommentLines[1].IsXmlComment);
		}		
		
		/// <summary>
		/// Tests the parsing of a single namespace with a single interface
		/// definition.
		/// </summary>
		[Test]
		public void ParseInterfaceDefinitionTest()		
		{
			CSharpParser parser = new CSharpParser();
			
			CSharpTestFile testFile = CSharpTestUtilities.GetInterfaceDefinitionFile();
			using (TextReader reader = testFile.GetReader())
			{
			    ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			    Assert.IsNotNull(elements, "Code element collection should not be null.");
			    Assert.AreEqual(2, elements.Count,
			        "An unexpected number of elements were parsed.");
			
			    UsingElement using1 = elements[0] as UsingElement;
			    Assert.IsNotNull(using1, "Expected a UsingElement.");
			    Assert.AreEqual("System", using1.Name,
			        "Unexpected using name.");
			
			    NamespaceElement namespaceElement = elements[1] as NamespaceElement;
			    Assert.IsNotNull(namespaceElement, "Expected a NamespaceElement.");
			    Assert.AreEqual("SampleNamespace", namespaceElement.Name,
			        "Unexpected namespace name.");
			
			    Assert.IsNotNull(namespaceElement.Children,
			        "Namespace Children collection should not be null.");
			    Assert.AreEqual(1, namespaceElement.Children.Count,
			        "An unexpected number of namespace child elements were parsed.");
			
			    TypeElement interfaceElement = namespaceElement.Children[0] as TypeElement;
			    Assert.IsNotNull(interfaceElement, "Expected a TypeElement.");
			    Assert.AreEqual(TypeElementType.Interface, interfaceElement.Type,
			        "Expected type to be an interface.");
			    Assert.IsFalse(interfaceElement.IsStatic,
			        "Structure should not be static.");
			    Assert.IsFalse(interfaceElement.IsSealed,
			        "Interfaces should not be considered sealed.");
			    Assert.AreEqual("SampleInterface", interfaceElement.Name,
			        "Unexpected interface name.");
			    Assert.AreEqual(3, interfaceElement.HeaderCommentLines.Count,
			        "An unexpected number of class header comment lines were parsed.");
			    foreach (ICommentLine commentLine in
			        interfaceElement.HeaderCommentLines)
			    {
			        Assert.IsTrue(commentLine.IsXmlComment,
			            "Interface header comment should be an XML comment.");
			    }
			    Assert.AreEqual(CodeAccess.Public, interfaceElement.Access,
			        "Unexpected code access level.");
			}
		}		
		
		/// <summary>
		/// Verifies that the correct number of members are parsed from the 
		/// sample class.
		/// </summary>
		[Test]
		public void ParseMembersCountTest()		
		{
			CSharpTestFile testFile = CSharpTestUtilities.GetClassMembersFile();
			using (TextReader reader = testFile.GetReader())
			{
			    TypeElement classElement = GetMembersTestClass(reader);
			
			    Assert.AreEqual(
			        NumFields + NumConstructors + NumProperties + 
			        NumMethods + NumNestedTypes + NumDelegates + NumEvents, 
			        classElement.Children.Count,
			        "Unexpected number of class members.");
			}
		}		
		
		/// <summary>
		/// Tests parsing a method.
		/// </summary>
		[Test]
		public void ParseMethodTest()		
		{
			StringReader reader = new StringReader(
			    "private void DoSomething(){}");
			
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			Assert.AreEqual(1, elements.Count,
			    "An unexpected number of elements were parsed.");
			MethodElement methodElement = elements[0] as MethodElement;
			Assert.IsNotNull(methodElement,
			    "Element is not a MethodElement.");
			Assert.AreEqual("DoSomething", methodElement.Name,
			    "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, methodElement.Access,
			    "Unexpected code access.");
			Assert.AreEqual("void", methodElement.Type,
			    "Unexpected member type.");
		}		
		
		/// <summary>
		/// Tests parsing a method with a block character in a comment of the 
		/// body text.
		/// </summary>
		[Test]
		public void ParseMethodWithBlockCharBlockCommentTest()		
		{
			StringReader reader = new StringReader(
			    "private void DoSomething(){/*Test }*/}");
			
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			Assert.AreEqual(1, elements.Count,
			    "An unexpected number of elements were parsed.");
			MethodElement methodElement = elements[0] as MethodElement;
			Assert.IsNotNull(methodElement,
			    "Element is not a MethodElement.");
			Assert.AreEqual("DoSomething", methodElement.Name,
			    "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, methodElement.Access,
			    "Unexpected code access.");
			Assert.AreEqual("void", methodElement.Type,
			    "Unexpected member type.");
			Assert.AreEqual("/*Test }*/", methodElement.BodyText,
			    "Unexpected body text.");
		}		
		
		/// <summary>
		/// Tests parsing a method with a block character in a comment of the 
		/// body text.
		/// </summary>
		[Test]
		public void ParseMethodWithBlockCharLineCommentTest()		
		{
			StringReader reader = new StringReader(
			    "private void DoSomething(){\r\n//Test }\r\n}");
			
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			Assert.AreEqual(1, elements.Count,
			    "An unexpected number of elements were parsed.");
			MethodElement methodElement = elements[0] as MethodElement;
			Assert.IsNotNull(methodElement,
			    "Element is not a MethodElement.");
			Assert.AreEqual("DoSomething", methodElement.Name,
			    "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, methodElement.Access,
			    "Unexpected code access.");
			Assert.AreEqual("void", methodElement.Type,
			    "Unexpected member type.");
			Assert.AreEqual("//Test }", methodElement.BodyText,
			    "Unexpected body text.");
		}		
		
		/// <summary>
		/// Tests parsing a method with a block character in the body text.
		/// </summary>
		[Test]
		public void ParseMethodWithBlockCharTest()		
		{
			StringReader reader = new StringReader(
			    "private void DoSomething(){Console.WriteLine(\"}\";Console.WriteLine();}");
			
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			Assert.AreEqual(1, elements.Count,
			    "An unexpected number of elements were parsed.");
			MethodElement methodElement = elements[0] as MethodElement;
			Assert.IsNotNull(methodElement,
			    "Element is not a MethodElement.");
			Assert.AreEqual("DoSomething", methodElement.Name,
			    "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, methodElement.Access,
			    "Unexpected code access.");
			Assert.AreEqual("void", methodElement.Type,
			    "Unexpected member type.");
			Assert.AreEqual("Console.WriteLine(\"}\";Console.WriteLine();", methodElement.BodyText,
			    "Unexpected body text.");
		}		
		
		/// <summary>
		/// Verifies the parsing of methods from the 
		/// sample class.
		/// </summary>
		[Test]
		public void ParseMethodsTest()		
		{
			CSharpTestFile testFile = CSharpTestUtilities.GetClassMembersFile();
			using (TextReader reader = testFile.GetReader())
			{
			    TypeElement classElement = GetMembersTestClass(reader);
			
			    MethodElement method;
			
			    int methodOffset = NumFields + NumConstructors + NumProperties;
			
			    method = classElement.Children[methodOffset + 0] as MethodElement;
			    Assert.IsNotNull(method, "Expected a method.");
			    Assert.AreEqual("DoSomething", method.Name,
			        "Unexpected method name.");
			    Assert.AreEqual(CodeAccess.Public, method.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(1, method.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(method.IsStatic,
			        "Method should not be static.");
			    Assert.AreEqual("void", method.Type,
			        "Unexpected return type.");
			    Assert.IsFalse(method.IsAbstract,
			        "Method should not be abstract.");
			    Assert.IsFalse(method.IsOverride,
			        "Method should not be an override.");
			    Assert.IsFalse(method.IsSealed,
			        "Method should not be sealed.");
			    Assert.IsFalse(method.IsVirtual,
			        "Method should not be virtual.");
			    Assert.IsFalse(method.IsNew,
			        "Method should not be new.");
			    Assert.AreEqual(0, method.Attributes.Count,
			        "Unexpected number of attributes.");
			    Assert.AreEqual(string.Empty, method.Params,
			        "Unexpected parameter string.");
			
			    method = classElement.Children[methodOffset + 1] as MethodElement;
			    Assert.IsNotNull(method, "Expected a method.");
			    Assert.AreEqual("ToString", method.Name,
			        "Unexpected method name.");
			    Assert.AreEqual(CodeAccess.Public, method.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(3, method.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(method.IsStatic,
			        "Method should not be static.");
			    Assert.AreEqual("string", method.Type,
			        "Unexpected return type.");
			    Assert.IsFalse(method.IsAbstract,
			        "Method should not be abstract.");
			    Assert.IsTrue(method.IsOverride,
			        "Method should be an override.");
			    Assert.IsFalse(method.IsSealed,
			        "Method should not be sealed.");
			    Assert.IsFalse(method.IsVirtual,
			        "Method should not be virtual.");
			    Assert.IsFalse(method.IsNew,
			        "Method should not be new.");
			    Assert.AreEqual(0, method.Attributes.Count,
			        "Unexpected number of attributes.");
			    Assert.AreEqual(string.Empty, method.Params,
			        "Unexpected parameter string.");
			
			    method = classElement.Children[methodOffset + 2] as MethodElement;
			    Assert.IsNotNull(method, "Expected a method.");
			    Assert.AreEqual("GetBoolValue", method.Name,
			        "Unexpected method name.");
			    Assert.AreEqual(CodeAccess.Private, method.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(6, method.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(method.IsStatic,
			        "Method should not be static.");
			    Assert.AreEqual("bool", method.Type,
			        "Unexpected return type.");
			    Assert.IsFalse(method.IsAbstract,
			        "Method should not be abstract.");
			    Assert.IsFalse(method.IsOverride,
			        "Method should not be an override.");
			    Assert.IsFalse(method.IsSealed,
			        "Method should not be sealed.");
			    Assert.IsFalse(method.IsVirtual,
			        "Method should not be virtual.");
			    Assert.IsFalse(method.IsNew,
			        "Method should not be new.");
			    Assert.AreEqual(0, method.Attributes.Count,
			        "Unexpected number of attributes.");
			    Assert.AreEqual("int intParam, string stringParam", method.Params,
			        "Unexpected parameter string.");
			    Assert.IsTrue(method.BodyText.Contains("return true;"),
			        "Unexpected body text.");
			
			    method = classElement.Children[methodOffset + 3] as MethodElement;
			    Assert.IsNotNull(method, "Expected a method.");
			    Assert.AreEqual("GetWithParamAttributes", method.Name,
			        "Unexpected method name.");
			    Assert.AreEqual(CodeAccess.Internal, method.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(5, method.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsTrue(method.IsStatic,
			        "Method should be static.");
			    Assert.AreEqual("int?", method.Type,
			        "Unexpected return type.");
			    Assert.IsFalse(method.IsAbstract,
			        "Method should not be abstract.");
			    Assert.IsFalse(method.IsOverride,
			        "Method should not be an override.");
			    Assert.IsFalse(method.IsSealed,
			        "Method should not be sealed.");
			    Assert.IsFalse(method.IsVirtual,
			        "Method should not be virtual.");
			    Assert.IsFalse(method.IsNew,
			        "Method should not be new.");
			    Assert.AreEqual(1, method.Attributes.Count,
			        "Unexpected number of attributes.");
			    Assert.IsTrue(
			        method.Params.Contains(
			        "[Description(\"Int parameter\")] int intParam"),
			        "Unexpected params string.");
			    Assert.IsTrue(
			        method.Params.Contains(
			        "[Description(\"String parameter\")] string stringParam"),
			        "Unexpected params string.");
			
			    method = classElement.Children[methodOffset + 4] as MethodElement;
			    Assert.IsNotNull(method, "Expected a method.");
			    Assert.AreEqual("GetWithTypeParameters", method.Name,
			        "Unexpected method name.");
			    Assert.AreEqual(CodeAccess.Public, method.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(3, method.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(method.IsStatic,
			        "Method should not be static.");
			    Assert.AreEqual("bool", method.Type,
			        "Unexpected return type.");
			    Assert.IsFalse(method.IsAbstract,
			        "Method should not be abstract.");
			    Assert.IsFalse(method.IsOverride,
			        "Method should not be an override.");
			    Assert.IsFalse(method.IsSealed,
			        "Method should not be sealed.");
			    Assert.IsFalse(method.IsVirtual,
			        "Method should not be virtual.");
			    Assert.IsFalse(method.IsNew,
			        "Method should not be new.");
			    Assert.AreEqual(0, method.Attributes.Count,
			        "Unexpected number of attributes.");
			    Assert.AreEqual("Action<T1> typeParam1, Action<T2> typeParam2",
			        method.Params.Trim(),
			        "Unexpected params string.");
			    Assert.AreEqual(2, method.TypeParameters.Count,
			        "Unexpected number of type parameters.");
			    Assert.AreEqual("T1", method.TypeParameters[0].Name,
			        "Unexpected type parameter name.");
			    Assert.AreEqual(2, method.TypeParameters[0].Constraints.Count,
			        "Unexpected type parameter constraints.");
			    Assert.AreEqual("T2", method.TypeParameters[1].Name,
			        "Unexpected type parameter name.");
			    Assert.AreEqual(2, method.TypeParameters[1].Constraints.Count,
			        "Unexpected type parameter constraints.");
			
			    //
			    // External method
			    //
			    method = classElement.Children[methodOffset + 5] as MethodElement;
			    Assert.IsNotNull(method, "Expected a method.");
			    Assert.AreEqual("MessageBox", method.Name,
			        "Unexpected method name.");
			    Assert.AreEqual(CodeAccess.Public, method.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(0, method.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsTrue(method.IsStatic,
			        "Method should be static.");
			    Assert.AreEqual("int", method.Type,
			        "Unexpected return type.");
			    Assert.IsFalse(method.IsAbstract,
			        "Method should not be abstract.");
			    Assert.IsTrue(method.IsExternal,
			        "Method should be external.");
			    Assert.IsFalse(method.IsOverride,
			        "Method should not be an override.");
			    Assert.IsFalse(method.IsSealed,
			        "Method should not be sealed.");
			    Assert.IsFalse(method.IsVirtual,
			        "Method should not be virtual.");
			    Assert.IsFalse(method.IsNew,
			        "Method should not be new.");
			    Assert.AreEqual(1, method.Attributes.Count,
			        "Unexpected number of attributes.");
			    Assert.AreEqual("int h, string m, string c, int type",
			        method.Params.Trim(),
			        "Unexpected params string.");
			    Assert.AreEqual(0, method.TypeParameters.Count,
			        "Unexpected number of type parameters.");
			
			    //
			    // Unsafe method
			    //
			    method = classElement.Children[methodOffset + 6] as MethodElement;
			    Assert.IsNotNull(method, "Expected a method.");
			    Assert.AreEqual("UnsafeSqrPtrParam", method.Name,
			        "Unexpected method name.");
			    Assert.AreEqual(CodeAccess.NotSpecified, method.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(4, method.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsTrue(method.IsStatic,
			        "Method should be static.");
			    Assert.AreEqual("void", method.Type,
			        "Unexpected return type.");
			    Assert.IsFalse(method.IsAbstract,
			        "Method should not be abstract.");
			    Assert.IsTrue(method.IsUnsafe,
			        "Method should be unsafe.");
			    Assert.IsFalse(method.IsExternal,
			        "Method should not be external.");
			    Assert.IsFalse(method.IsOverride,
			        "Method should not be an override.");
			    Assert.IsFalse(method.IsSealed,
			        "Method should not be sealed.");
			    Assert.IsFalse(method.IsVirtual,
			        "Method should not be virtual.");
			    Assert.IsFalse(method.IsNew,
			        "Method should not be new.");
			    Assert.AreEqual(0, method.Attributes.Count,
			        "Unexpected number of attributes.");
			    Assert.AreEqual("int* p",
			        method.Params.Trim(),
			        "Unexpected params string.");
			    Assert.AreEqual(0, method.TypeParameters.Count,
			        "Unexpected number of type parameters.");
			}
		}		
		
		/// <summary>
		/// Tests the parsing of a single namespace with a multiple class
		/// definitions.
		/// </summary>
		[Test]
		public void ParseMultiClassDefinitionTest()		
		{
			CSharpParser parser = new CSharpParser();
			
			CSharpTestFile testFile = CSharpTestUtilities.GetMultiClassDefinitionFile();
			using (TextReader reader = testFile.GetReader())
			{
			    ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			    Assert.IsNotNull(elements, "Code element collection should not be null.");
			    Assert.AreEqual(5, elements.Count,
			        "An unexpected number of elements were parsed.");
			
			    UsingElement using1 = elements[0] as UsingElement;
			    Assert.IsNotNull(using1, "Expected a UsingElement.");
			    Assert.AreEqual("System", using1.Name,
			        "Unexpected using name.");
			
			    UsingElement using2 = elements[1] as UsingElement;
			    Assert.IsNotNull(using2, "Expected a UsingElement.");
			    Assert.AreEqual("System.Collections.Generic", using2.Name,
			        "Unexpected using name.");
			
			    UsingElement using3 = elements[2] as UsingElement;
			    Assert.IsNotNull(using3, "Expected a UsingElement.");
			    Assert.AreEqual("System.Text", using3.Name,
			        "Unexpected using name.");
			
			    UsingElement using4 = elements[3] as UsingElement;
			    Assert.IsNotNull(using4, "Expected a UsingElement.");
			    Assert.AreEqual("System.ComponentModel", using4.Name,
			        "Unexpected using name.");
			
			    NamespaceElement namespaceElement = elements[4] as NamespaceElement;
			    Assert.IsNotNull(namespaceElement, "Expected a NamespaceElement.");
			    Assert.AreEqual("SampleNamespace", namespaceElement.Name,
			        "Unexpected namespace name.");
			
			    Assert.IsNotNull(namespaceElement.Children,
			        "Namespace Children collection should not be null.");
			    Assert.AreEqual(8, namespaceElement.Children.Count,
			        "An unexpected number of namespace child elements were parsed.");
			
			    //
			    // Sample class 1
			    //
			    TypeElement classElement1 = namespaceElement.Children[0] as TypeElement;
			    Assert.IsNotNull(classElement1, "Expected a TypeElement.");
			    Assert.AreEqual("SampleClass1", classElement1.Name,
			        "Unexpected class name.");
			    Assert.AreEqual(CodeAccess.Public, classElement1.Access,
			        "Unexpected class code access level.");
			    Assert.IsFalse(classElement1.IsStatic,
			        "Class should not be static.");
			    Assert.IsFalse(classElement1.IsSealed,
			       "Class should not be sealed.");
			
			    //
			    // Sample class 2
			    //
			    TypeElement classElement2 = namespaceElement.Children[1] as TypeElement;
			    Assert.IsNotNull(classElement2, "Expected a TypeElement.");
			    Assert.AreEqual("SampleClass2", classElement2.Name,
			        "Unexpected class name.");
			    Assert.AreEqual(CodeAccess.Internal, classElement2.Access,
			        "Unexpected class code access level.");
			    Assert.AreEqual(1, classElement2.Interfaces.Count,
			        "Unexpected number of implemented interfaces.");
			    Assert.AreEqual("Exception", classElement2.Interfaces[0],
			        "Unexpected implemented interface name.");
			    Assert.IsFalse(classElement2.IsStatic,
			        "Class should not be static.");
			    Assert.IsFalse(classElement2.IsSealed,
			       "Class should not be sealed.");
			
			    //
			    // Sample class 3
			    //
			    TypeElement classElement3 = namespaceElement.Children[2] as TypeElement;
			    Assert.IsNotNull(classElement3, "Expected a TypeElement.");
			    Assert.AreEqual("SampleClass3", classElement3.Name,
			        "Unexpected class name.");
			    Assert.AreEqual(CodeAccess.Internal, classElement3.Access,
			        "Unexpected class code access level.");
			    Assert.AreEqual(3, classElement3.Interfaces.Count,
			        "Unexpected base class name.");
			    Assert.AreEqual("List<int>", classElement3.Interfaces[0],
			        "Unexpected interface name.");
			    Assert.AreEqual("IDisposable", classElement3.Interfaces[1],
			        "Unexpected interface name.");
			    Assert.AreEqual("IComparable", classElement3.Interfaces[2],
			        "Unexpected interface name.");
			    Assert.IsFalse(classElement3.IsStatic,
			        "Class should not be static.");
			    Assert.IsFalse(classElement3.IsSealed,
			       "Class should not be sealed.");
			
			    //
			    // Sample class 4
			    //
			    TypeElement classElement4 = namespaceElement.Children[3] as TypeElement;
			    Assert.IsNotNull(classElement4, "Expected a TypeElement.");
			    Assert.AreEqual("SampleClass4", classElement4.Name,
			        "Unexpected class name.");
			    Assert.AreEqual(CodeAccess.Internal, classElement4.Access,
			        "Unexpected class code access level.");
			    Assert.IsFalse(classElement4.IsStatic,
			        "Class should not be static.");
			    Assert.IsTrue(classElement4.IsSealed,
			       "Class should be sealed.");
			    Assert.AreEqual(2, classElement4.Interfaces.Count,
			        "Unexpected base class name.");
			    Assert.AreEqual("IComparable", classElement4.Interfaces[0],
			        "Unexpected interface name.");
			    Assert.AreEqual("IDisposable", classElement4.Interfaces[1],
			        "Unexpected interface name.");
			
			    Assert.AreEqual(2, classElement4.TypeParameters.Count,
			        "Unexpected number of type parameters.");
			
			    TypeParameter parameter1 = classElement4.TypeParameters[0];
			    Assert.AreEqual("T1", parameter1.Name,
			        "Unexpected type parameter name.");
			    // where T1 : IComparable, IConvertible, new()
			    Assert.AreEqual(3, parameter1.Constraints.Count,
			        "Unexpected number of type parameter constraints.");
			    Assert.AreEqual("IComparable", parameter1.Constraints[0],
			        "Unexpected type parameter contraint.");
			    Assert.AreEqual("IConvertible", parameter1.Constraints[1],
			        "Unexpected type parameter contraint.");
			    Assert.AreEqual("new()", parameter1.Constraints[2],
			        "Unexpected type parameter contraint.");
			
			    TypeParameter parameter2 = classElement4.TypeParameters[1];
			    Assert.AreEqual("T2", parameter2.Name,
			        "Unexpected type parameter name.");
			    // where T2 : class, IComparable, IConvertible, new()
			    Assert.AreEqual(4, parameter2.Constraints.Count,
			        "Unexpected number of type parameter constraints.");
			    Assert.AreEqual("class", parameter2.Constraints[0],
			        "Unexpected type parameter contraint.");
			    Assert.AreEqual("IComparable<T2>", parameter2.Constraints[1],
			        "Unexpected type parameter contraint.");
			    Assert.AreEqual("global::System.IConvertible", parameter2.Constraints[2],
			        "Unexpected type parameter contraint.");
			    Assert.AreEqual("new()", parameter2.Constraints[3],
			        "Unexpected type parameter contraint.");
			
			    //
			    // Sample class 5
			    //
			    TypeElement classElement5 = namespaceElement.Children[4] as TypeElement;
			    Assert.IsNotNull(classElement5, "Expected a TypeElement.");
			    Assert.AreEqual("SampleClass5", classElement5.Name,
			        "Unexpected class name.");
			    Assert.AreEqual(CodeAccess.Internal, classElement5.Access,
			        "Unexpected class code access level.");
			    Assert.IsTrue(classElement5.IsStatic,
			        "Class should be static.");
			    Assert.IsFalse(classElement5.IsSealed,
			       "Class should not be sealed.");
			
			    //
			    // Sample class 6
			    //
			    TypeElement classElement6 = namespaceElement.Children[5] as TypeElement;
			    Assert.IsNotNull(classElement6, "Expected a TypeElement.");
			    Assert.AreEqual("SampleClass6", classElement6.Name,
			        "Unexpected class name.");
			    Assert.AreEqual(CodeAccess.Public, classElement6.Access,
			        "Unexpected class code access level.");
			    Assert.IsTrue(classElement6.IsStatic,
			        "Class should be static.");
			    Assert.IsFalse(classElement6.IsSealed,
			       "Class should not be sealed.");
			
			    //
			    // Sample class 7
			    //
			    TypeElement classElement7 = namespaceElement.Children[6] as TypeElement;
			    Assert.IsNotNull(classElement7, "Expected a TypeElement.");
			    Assert.AreEqual("SampleClass7", classElement7.Name,
			        "Unexpected class name.");
			    Assert.AreEqual(CodeAccess.Public, classElement7.Access,
			        "Unexpected class code access level.");
			    Assert.IsFalse(classElement7.IsStatic,
			        "Class should be static.");
			    Assert.IsTrue(classElement7.IsSealed,
			       "Class should be sealed.");
			    Assert.AreEqual(2, classElement7.Interfaces.Count,
			        "Unexpected number of interfaces.");
			    Assert.AreEqual("global::System.IDisposable", classElement7.Interfaces[0],
			        "Unexpected interface name.");
			    Assert.AreEqual("IComparable<int>", classElement7.Interfaces[1],
			        "Unexpected interface name.");
			
			    //
			    // Sample class 8
			    //
			    TypeElement classElement8 = namespaceElement.Children[7] as TypeElement;
			    Assert.IsNotNull(classElement8, "Expected a TypeElement.");
			    Assert.AreEqual("SampleClass8", classElement8.Name,
			        "Unexpected class name.");
			    Assert.AreEqual(CodeAccess.Public, classElement8.Access,
			        "Unexpected class code access level.");
			    Assert.IsFalse(classElement8.IsStatic,
			        "Class should be static.");
			    Assert.IsFalse(classElement8.IsSealed,
			       "Class should not be sealed.");
			    Assert.IsTrue(classElement8.IsAbstract,
			       "Class should be abstract.");
			}
		}		
		
		/// <summary>
		/// Tests parsing multiple fields with an initial value from a single statement.
		/// </summary>
		[Test]
		public void ParseMultiFieldInitialValueTest()		
		{
			StringReader reader = new StringReader(
			    "private int val1, val2 = 1;");
			
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			Assert.AreEqual(1, elements.Count,
			    "An unexpected number of elements were parsed.");
			FieldElement fieldElement = elements[0] as FieldElement;
			Assert.IsNotNull(fieldElement,
			    "Element is not a FieldElement.");
			Assert.AreEqual("val1, val2", fieldElement.Name,
			    "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, fieldElement.Access,
			    "Unexpected code access.");
			Assert.AreEqual("int", fieldElement.Type,
			    "Unexpected member type.");
			Assert.AreEqual("1", fieldElement.InitialValue,
			    "Unexpected initial value.");
		}		
		
		/// <summary>
		/// Tests parsing multiple fields from a single statement.
		/// </summary>
		[Test]
		public void ParseMultiFieldTest()		
		{
			StringReader reader = new StringReader(
			    "private int val1, val2;");
			
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			Assert.AreEqual(1, elements.Count,
			    "An unexpected number of elements were parsed.");
			FieldElement fieldElement = elements[0] as FieldElement;
			Assert.IsNotNull(fieldElement,
			    "Element is not a FieldElement.");
			Assert.AreEqual("val1, val2", fieldElement.Name,
			    "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, fieldElement.Access,
			    "Unexpected code access.");
			Assert.AreEqual("int", fieldElement.Type,
			    "Unexpected member type.");
			Assert.IsNull(fieldElement.InitialValue,
			    "Unexpected initial value.");
		}		
		
		/// <summary>
		/// Tests the parsing of multiple namespaces
		/// </summary>
		[Test]
		public void ParseMultipleNamespaceTest()		
		{
			CSharpParser parser = new CSharpParser();
			
			CSharpTestFile testFile = CSharpTestUtilities.GetMultipleNamespaceFile();
			using (TextReader reader = testFile.GetReader())
			{
			    ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			    Assert.IsNotNull(elements, "Code element collection should not be null.");
			    Assert.AreEqual(2, elements.Count,
			        "An unexpected number of elements were parsed.");
			
			    NamespaceElement namespaceElement1 = elements[0] as NamespaceElement;
			    Assert.IsNotNull(namespaceElement1, "Expected a NamespaceElement.");
			    Assert.AreEqual("SampleNamespace1", namespaceElement1.Name,
			        "Unexpected namespace name.");
			
			    NamespaceElement namespaceElement2 = elements[1] as NamespaceElement;
			    Assert.IsNotNull(namespaceElement2, "Expected a NamespaceElement.");
			    Assert.AreEqual("SampleNamespace2", namespaceElement2.Name,
			        "Unexpected namespace name.");
			}
		}		
		
		/// <summary>
		/// Tests parsing of a namepsace where a closing 
		/// brace is expected.";
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException), 
            MatchType=MessageMatch.Contains, 
            ExpectedMessage="Expected }")]
		public void ParseNamespaceExpectedBlockEnd()		
		{
			StringReader reader = new StringReader(
			    "namespace TestNamespace{");
			
			CSharpParser parser = new CSharpParser();
			parser.Parse(reader);
		}		
		
		/// <summary>
		/// Tests parsing of a namepsace where an opening 
		/// brace is expected.";
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Expected {")]
		public void ParseNamespaceExpectedBlockStart()		
		{
			StringReader reader = new StringReader(
			    "namespace TestNamespace");
			
			CSharpParser parser = new CSharpParser();
			parser.Parse(reader);
		}		
		
		/// <summary>
		/// Verifies the parsing of nested types from the 
		/// sample class.
		/// </summary>
		[Test]
		public void ParseNestedTypesTest()		
		{
			CSharpTestFile testFile = CSharpTestUtilities.GetClassMembersFile();
			using (TextReader reader = testFile.GetReader())
			{
			    TypeElement classElement = GetMembersTestClass(reader);
			
			    TypeElement type;
			
			    int nestedTypeOffset = NumFields + NumConstructors + NumProperties + 
			        NumMethods + NumDelegates + NumEvents;
			
			    type = classElement.Children[nestedTypeOffset + 0] as TypeElement;
			    Assert.IsNotNull(type, "Expected a type.");
			    Assert.AreEqual(TypeElementType.Enum, type.Type,
			        "Unexpected type element type.");
			    Assert.AreEqual("SampleEnum", type.Name,
			        "Unexpected type name.");
			    Assert.AreEqual(CodeAccess.Private, type.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(3, type.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(type.IsStatic,
			        "Type should not be static.");
			    Assert.IsFalse(type.IsAbstract,
			        "Type should not be abstract.");
			    Assert.IsFalse(type.IsSealed,
			        "Type should not be sealed.");
			    Assert.AreEqual(1, type.Attributes.Count,
			        "Unexpected number of attributes.");
			
			    type = classElement.Children[nestedTypeOffset + 1] as TypeElement;
			    Assert.IsNotNull(type, "Expected a type.");
			    Assert.AreEqual(TypeElementType.Structure, type.Type,
			        "Unexpected type element type.");
			    Assert.AreEqual("SampleStructure", type.Name,
			        "Unexpected type name.");
			    Assert.AreEqual(CodeAccess.Public, type.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(3, type.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(type.IsStatic,
			        "Type should not be static.");
			    Assert.IsFalse(type.IsAbstract,
			        "Type should not be abstract.");
			    Assert.IsFalse(type.IsSealed,
			        "Type should not be sealed.");
			    Assert.AreEqual(0, type.Attributes.Count,
			        "Unexpected number of attributes.");
			    Assert.AreEqual(2, type.Children.Count,
			        "Unexpected number of child elements.");
			
			    type = classElement.Children[nestedTypeOffset + 2] as TypeElement;
			    Assert.IsNotNull(type, "Expected a type.");
			    Assert.AreEqual(TypeElementType.Class, type.Type,
			        "Unexpected type element type.");
			    Assert.AreEqual("SampleNestedClass", type.Name,
			        "Unexpected type name.");
			    Assert.AreEqual(CodeAccess.Private, type.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(3, type.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(type.IsStatic,
			        "Type should not be static.");
			    Assert.IsFalse(type.IsAbstract,
			        "Type should not be abstract.");
			    Assert.IsFalse(type.IsSealed,
			        "Type should not be sealed.");
			    Assert.AreEqual(0, type.Attributes.Count,
			        "Unexpected number of attributes.");
			    Assert.AreEqual(1, type.Children.Count,
			        "Unexpected number of child elements.");
			
			    type = classElement.Children[nestedTypeOffset + 3] as TypeElement;
			    Assert.IsNotNull(type, "Expected a type.");
			    Assert.AreEqual(TypeElementType.Class, type.Type,
			        "Unexpected type element type.");
			    Assert.AreEqual("SampleNestedStaticClass", type.Name,
			        "Unexpected type name.");
			    Assert.AreEqual(CodeAccess.Internal, type.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(3, type.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsTrue(type.IsStatic,
			        "Type should be static.");
			    Assert.IsFalse(type.IsAbstract,
			        "Type should not be abstract.");
			    Assert.IsFalse(type.IsSealed,
			        "Type should not be sealed.");
			    Assert.AreEqual(0, type.Attributes.Count,
			        "Unexpected number of attributes.");
			    Assert.AreEqual(1, type.Children.Count,
			        "Unexpected number of child elements.");
			}
		}		
		
		/// <summary>
		/// Tests parsing a null stream.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ParseNullStreamTest()		
		{
			CSharpParser parser = new CSharpParser();
			parser.Parse(null);
		}		
		
		/// <summary>
		/// Verifies the parsing of operators.
		/// </summary>
		[Test]
		public void ParseOperatorsTest()		
		{
			CSharpTestFile testFile = CSharpTestUtilities.GetOperatorsFile();
			using (TextReader reader = testFile.GetReader())
			{
			    CSharpParser parser = new CSharpParser();
			    ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			    Assert.AreEqual(2, elements.Count,
			        "Unexpected number of top-level elements.");
			
			    NamespaceElement namespaceElement = elements[1] as NamespaceElement;
			    Assert.IsNotNull(namespaceElement, "Expected a namespace element.");
			
			    Assert.AreEqual(1, namespaceElement.Children.Count,
			        "Unexpected number of namespace elements.");
			
			    TypeElement classElement = namespaceElement.Children[0] as TypeElement;
			    Assert.IsNotNull(classElement, "Expected a type element.");
			    Assert.AreEqual(TypeElementType.Class, classElement.Type, "Expected a class type.");
			    Assert.AreEqual("Fraction", classElement.Name,
			        "Unexpected type name.");
			    Assert.AreEqual(CodeAccess.Public, classElement.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(3, classElement.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(classElement.IsStatic,
			        "Type should not be static.");
			    Assert.IsFalse(classElement.IsAbstract,
			        "Type should not be abstract.");
			    Assert.IsFalse(classElement.IsSealed,
			        "Type should not be sealed.");
			    Assert.AreEqual(0, classElement.Attributes.Count,
			        "Unexpected number of attributes.");
			    
			    Assert.AreEqual(6, classElement.Children.Count,
			        "Unexpected number of child elements.");
			
			    FieldElement fieldElement = classElement.Children[0] as FieldElement;
			    Assert.IsNotNull(fieldElement, "Expected a field element.");
			    Assert.AreEqual("num, den", fieldElement.Name,
			        "Unexpected field name.");
			    Assert.AreEqual("int", fieldElement.Type,
			        "Unexpected field type.");
			
			    ConstructorElement constructorElement = classElement.Children[1] as ConstructorElement;
			    Assert.IsNotNull(constructorElement, "Expected a constructor element.");
			    Assert.AreEqual("Fraction", constructorElement.Name,
			        "Unexpected constructor name.");
			    Assert.AreEqual("int num, int den", constructorElement.Params,
			        "Unexpected constructor parameters.");
			
			    MethodElement operatorElement = classElement.Children[2] as MethodElement;
			    Assert.IsNotNull(operatorElement, "Expected a method element.");
			    Assert.IsTrue(operatorElement.IsOperator,
			        "Expected method to be an operator."); 
			    Assert.AreEqual("+", operatorElement.Name,
			        "Unexpected operator name.");
			    Assert.AreEqual(OperatorType.NotSpecified, operatorElement.OperatorType,
			        "Unexpected operator attributes.");
			    Assert.AreEqual(CodeAccess.Public, operatorElement.Access,
			        "Unexpected operator access.");
			    Assert.IsTrue(operatorElement.IsStatic,
			        "Expected operator to be static.");
			    Assert.AreEqual("Fraction", operatorElement.Type,
			        "Unexpected operator return type.");
			    Assert.AreEqual("Fraction a, Fraction b", operatorElement.Params,
			        "Unexpected operator parameters.");
			    Assert.IsTrue(operatorElement.BodyText.Contains("return"),
			        "Unexpected operator body text.");
			
			    operatorElement = classElement.Children[3] as MethodElement;
			    Assert.IsNotNull(operatorElement, "Expected a method element.");
			    Assert.IsTrue(operatorElement.IsOperator,
			        "Expected method to be an operator.");
			    Assert.AreEqual("*", operatorElement.Name,
			        "Unexpected operator name.");
			    Assert.AreEqual(OperatorType.NotSpecified, operatorElement.OperatorType,
			        "Unexpected operator attributes.");
			    Assert.AreEqual(CodeAccess.Public, operatorElement.Access,
			        "Unexpected operator access.");
			    Assert.IsTrue(operatorElement.IsStatic,
			        "Expected operator to be static.");
			    Assert.AreEqual("Fraction", operatorElement.Type,
			        "Unexpected operator return type.");
			    Assert.AreEqual("Fraction a, Fraction b", operatorElement.Params,
			        "Unexpected operator parameters.");
			    Assert.IsTrue(operatorElement.BodyText.Contains("return"),
			        "Unexpected operator body text.");
			
			    operatorElement = classElement.Children[4] as MethodElement;
			    Assert.IsNotNull(operatorElement, "Expected a method element.");
			    Assert.IsTrue(operatorElement.IsOperator,
			        "Expected method to be an operator.");
			    Assert.IsNull(operatorElement.Name,
			        "Unexpected operator name.");
			    Assert.AreEqual(OperatorType.Implicit, operatorElement.OperatorType,
			        "Unexpected operator attributes.");
			    Assert.AreEqual(CodeAccess.Public, operatorElement.Access,
			        "Unexpected operator access.");
			    Assert.IsTrue(operatorElement.IsStatic,
			        "Expected operator to be static.");
			    Assert.AreEqual("double", operatorElement.Type,
			        "Unexpected operator return type.");
			    Assert.AreEqual("Fraction f", operatorElement.Params,
			        "Unexpected operator parameters.");
			    Assert.IsTrue(operatorElement.BodyText.Contains("return"),
			        "Unexpected operator body text.");
			
			    operatorElement = classElement.Children[5] as MethodElement;
			    Assert.IsNotNull(operatorElement, "Expected a method element.");
			    Assert.IsTrue(operatorElement.IsOperator,
			        "Expected method to be an operator.");
			    Assert.IsNull(operatorElement.Name,
			        "Unexpected operator name.");
			    Assert.AreEqual(OperatorType.Explicit, operatorElement.OperatorType,
			        "Unexpected operator attributes.");
			    Assert.AreEqual(CodeAccess.Public, operatorElement.Access,
			        "Unexpected operator access.");
			    Assert.IsTrue(operatorElement.IsStatic,
			        "Expected operator to be static.");
			    Assert.AreEqual("decimal", operatorElement.Type,
			        "Unexpected operator return type.");
			    Assert.AreEqual("Fraction f", operatorElement.Params,
			        "Unexpected operator parameters.");
			    Assert.IsTrue(operatorElement.BodyText.Contains("return"),
			        "Unexpected operator body text.");
			}
		}		
		
		/// <summary>
		/// Verifies the parsing of properties members from the 
		/// sample class.
		/// </summary>
		[Test]
		public void ParsePropertiesTest()		
		{
			CSharpTestFile testFile = CSharpTestUtilities.GetClassMembersFile();
			using (TextReader reader = testFile.GetReader())
			{
			    TypeElement classElement = GetMembersTestClass(reader);
			
			    PropertyElement property;
			
			    int propertyOffset = NumFields + NumConstructors;
			
			    property = classElement.Children[propertyOffset + 0] as PropertyElement;
			    Assert.IsNotNull(property, "Expected a property.");
			    Assert.AreEqual("SimpleProperty", property.Name,
			        "Unexpected property name.");
			    Assert.AreEqual(CodeAccess.Public, property.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(3, property.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(property.IsStatic,
			        "Property should not be static.");
			    Assert.AreEqual("bool", property.Type,
			        "Unexpected property type.");
			    Assert.IsFalse(property.IsAbstract,
			        "Property should not be abstract.");
			    Assert.IsFalse(property.IsOverride,
			        "Property should not be an override.");
			    Assert.IsFalse(property.IsSealed,
			        "Property should not be sealed.");
			    Assert.IsFalse(property.IsVirtual,
			        "Property should not be virtual.");
			    Assert.IsTrue(
			        property.BodyText.Contains("get"), "Unexpected body text.");
			    Assert.IsTrue(
			        property.BodyText.Contains("set"), "Unexpeced body text.");
			
			    property = classElement.Children[propertyOffset + 1] as PropertyElement;
			    Assert.IsNotNull(property, "Expected a property.");
			    Assert.AreEqual("ProtectedProperty", property.Name,
			        "Unexpected property name.");
			    Assert.AreEqual(CodeAccess.Protected, property.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(3, property.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(property.IsStatic,
			        "Property should not be static.");
			    Assert.AreEqual("int", property.Type,
			       "Unexpected property type.");
			    Assert.IsFalse(property.IsAbstract,
			        "Property should not be abstract.");
			    Assert.IsFalse(property.IsOverride,
			        "Property should not be an override.");
			    Assert.IsFalse(property.IsSealed,
			        "Property should not be sealed.");
			    Assert.IsTrue(property.IsVirtual,
			        "Property should be virtual.");
			    Assert.IsTrue(
			        property.BodyText.Contains("get"), "Unexpected body text.");
			    Assert.IsFalse(
			        property.BodyText.Contains("set"), "Unexpeced body text.");
			
			    property = classElement.Children[propertyOffset + 2] as PropertyElement;
			    Assert.IsNotNull(property, "Expected a property.");
			    Assert.AreEqual("StaticProperty", property.Name,
			        "Unexpected property name.");
			    Assert.AreEqual(CodeAccess.Public, property.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(4, property.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsTrue(property.IsStatic,
			        "Property should be static.");
			    Assert.AreEqual("string", property.Type,
			       "Unexpected property type.");
			    Assert.IsFalse(property.IsAbstract,
			        "Property should not be abstract.");
			    Assert.IsFalse(property.IsOverride,
			        "Property should not be an override.");
			    Assert.IsFalse(property.IsSealed,
			        "Property should not be sealed.");
			    Assert.IsFalse(property.IsVirtual,
			        "Property should not be virtual.");
			    Assert.IsTrue(
			        property.BodyText.Contains("get"), "Unexpected body text.");
			    Assert.IsFalse(
			        property.BodyText.Contains("set"), "Unexpeced body text.");
			    Assert.AreEqual(0, property.Attributes.Count,
			        "Unexpected number of attributes.");
			
			    property = classElement.Children[propertyOffset + 3] as PropertyElement;
			    Assert.IsNotNull(property, "Expected a property.");
			    Assert.AreEqual("AttributedProperty", property.Name,
			        "Unexpected property name.");
			    Assert.AreEqual(CodeAccess.Public, property.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(2, property.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsTrue(property.IsStatic,
			        "Property should be static.");
			    Assert.AreEqual("string", property.Type,
			       "Unexpected property type.");
			    Assert.IsFalse(property.IsAbstract,
			        "Property should not be abstract.");
			    Assert.IsFalse(property.IsOverride,
			        "Property should not be an override.");
			    Assert.IsFalse(property.IsSealed,
			        "Property should not be sealed.");
			    Assert.IsFalse(property.IsVirtual,
			        "Property should not be virtual.");
			    Assert.IsTrue(
			        property.BodyText.Contains("get"), "Unexpected body text.");
			    Assert.IsFalse(
			        property.BodyText.Contains("set"), "Unexpeced body text.");
			    Assert.AreEqual(2, property.Attributes.Count,
			        "Unexpected number of attributes.");
			
			    property = classElement.Children[propertyOffset + 4] as PropertyElement;
			    Assert.IsNotNull(property, "Expected a property.");
			    Assert.AreEqual("GenericProperty", property.Name,
			        "Unexpected property name.");
			    Assert.AreEqual(CodeAccess.Internal, property.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(6, property.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(property.IsStatic,
			        "Property should not be static.");
			    Assert.AreEqual("Nullable<int>", property.Type,
			       "Unexpected property type.");
			    Assert.IsFalse(property.IsAbstract,
			        "Property should not be abstract.");
			    Assert.IsFalse(property.IsOverride,
			        "Property should not be an override.");
			    Assert.IsFalse(property.IsSealed,
			        "Property should not be sealed.");
			    Assert.IsFalse(property.IsVirtual,
			        "Property should not be virtual.");
			    Assert.IsFalse(property.IsNew,
			       "Property should not be new.");
			    Assert.IsTrue(
			        property.BodyText.Contains("get"), "Unexpected body text.");
			    Assert.IsTrue(
			        property.BodyText.Contains("set"), "Unexpeced body text.");
			    Assert.AreEqual(1, property.Attributes.Count,
			        "Unexpected number of attributes.");
			    Assert.AreEqual("Obsolete", property.Attributes[0].BodyText,
			        "Unexpected attribute text.");
			
			    property = classElement.Children[propertyOffset + 5] as PropertyElement;
			    Assert.IsNotNull(property, "Expected a property.");
			    Assert.AreEqual("ArrayProperty", property.Name,
			        "Unexpected property name.");
			    Assert.AreEqual(CodeAccess.Public, property.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(3, property.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(property.IsStatic,
			        "Property should not be static.");
			    Assert.AreEqual("string[]", property.Type,
			       "Unexpected property type.");
			    Assert.IsFalse(property.IsAbstract,
			        "Property should not be abstract.");
			    Assert.IsFalse(property.IsOverride,
			        "Property should not be an override.");
			    Assert.IsFalse(property.IsSealed,
			        "Property should not be sealed.");
			    Assert.IsFalse(property.IsVirtual,
			        "Property should not be virtual.");
			    Assert.IsTrue(property.IsNew,
			        "Property should be new.");
			    Assert.IsTrue(
			        property.BodyText.Contains("get"), "Unexpected body text.");
			    Assert.IsFalse(
			        property.BodyText.Contains("set"), "Unexpeced body text.");
			    Assert.AreEqual(0, property.Attributes.Count,
			        "Unexpected number of attributes.");
			
			    //
			    // Indexer property
			    //
			    property = classElement.Children[propertyOffset + 6] as PropertyElement;
			    Assert.IsNotNull(property, "Expected a property.");
			    Assert.AreEqual("this[int index]", property.Name,
			        "Unexpected property name.");
			    Assert.AreEqual(CodeAccess.Public, property.Access,
			        "Unexpected access level.");
			    Assert.AreEqual(5, property.HeaderCommentLines.Count,
			        "Unexpected number of header comment lines.");
			    Assert.IsFalse(property.IsStatic,
			        "Property should not be static.");
			    Assert.AreEqual("string", property.Type,
			       "Unexpected property type.");
			    Assert.IsFalse(property.IsAbstract,
			        "Property should not be abstract.");
			    Assert.IsFalse(property.IsOverride,
			        "Property should not be an override.");
			    Assert.IsFalse(property.IsSealed,
			        "Property should not be sealed.");
			    Assert.IsFalse(property.IsVirtual,
			        "Property should not be virtual.");
			    Assert.IsFalse(property.IsNew,
			        "Property should not be new.");
			    Assert.IsTrue(
			        property.BodyText.Contains("get"), "Unexpected body text.");
			    Assert.IsTrue(
			        property.BodyText.Contains("set"), "Unexpeced body text.");
			    Assert.AreEqual(0, property.Attributes.Count,
			        "Unexpected number of attributes.");
			}
		}		
		
		/// <summary>
		/// Tests parsing a simple class.
		/// </summary>
		[Test]
		public void ParseSimpleClassTest()		
		{
			StringReader reader = new StringReader(
			    "public class Test{}");
			
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			Assert.AreEqual(1, elements.Count,
			    "An unexpected number of elements were parsed.");
			TypeElement typeElement = elements[0] as TypeElement;
			Assert.IsNotNull(typeElement,
			    "Element is not a TypeElement.");
			Assert.AreEqual("Test", typeElement.Name,
			    "Unexpected name.");
			Assert.AreEqual(CodeAccess.Public, typeElement.Access,
			    "Unexpected code access.");
			Assert.AreEqual(TypeElementType.Class, typeElement.Type,
			    "Unexpected type element type.");
		}		
		
		/// <summary>
		/// Tests parsing a simple field.
		/// </summary>
		[Test]
		public void ParseSimpleFieldTest()		
		{
			StringReader reader = new StringReader(
			    "private int val;");
			
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			Assert.AreEqual(1, elements.Count,
			    "An unexpected number of elements were parsed.");
			FieldElement fieldElement = elements[0] as FieldElement;
			Assert.IsNotNull(fieldElement,
			    "Element is not a FieldElement.");
			Assert.AreEqual("val", fieldElement.Name,
			    "Unexpected name.");
			Assert.AreEqual(CodeAccess.Private, fieldElement.Access,
			    "Unexpected code access.");
			Assert.AreEqual("int", fieldElement.Type,
			    "Unexpected member type.");
			Assert.IsNull(fieldElement.InitialValue,
			    "Unexpected initial value.");
		}		
		
		/// <summary>
		/// Tests the parsing of a single namespace.
		/// </summary>
		[Test]
		public void ParseSingleNamespaceTest()		
		{
			CSharpParser parser = new CSharpParser();
			
			CSharpTestFile testFile = CSharpTestUtilities.GetSingleNamespaceFile();
			using (TextReader reader = testFile.GetReader())
			{
			    ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			    Assert.IsNotNull(elements, "Code element collection should not be null.");
			    Assert.AreEqual(1, elements.Count,
			        "An unexpected number of elements were parsed.");
			
			    NamespaceElement namespaceElement = elements[0] as NamespaceElement;
			    Assert.IsNotNull(namespaceElement, "Expected a NamespaceElement.");
			    Assert.AreEqual("SampleNamespace", namespaceElement.Name,
			        "Unexpected namespace name.");
			    Assert.IsNotNull(namespaceElement.Children,
			        "Children collection should not be null.");
			    Assert.AreEqual(0, namespaceElement.Children.Count,
			        "Children collection should not be null.");
			}
		}		
		
		/// <summary>
		/// Tests the parsing of a single namespace with a single structure
		/// definition.
		/// </summary>
		[Test]
		public void ParseStructDefinitionTest()		
		{
			CSharpParser parser = new CSharpParser();
			
			CSharpTestFile testFile = CSharpTestUtilities.GetStructDefinitionFile();
			using (TextReader reader = testFile.GetReader())
			{
			    ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			    Assert.IsNotNull(elements, "Code element collection should not be null.");
			    Assert.AreEqual(2, elements.Count,
			        "An unexpected number of elements were parsed.");
			
			    UsingElement using1 = elements[0] as UsingElement;
			    Assert.IsNotNull(using1, "Expected a UsingElement.");
			    Assert.AreEqual("System", using1.Name,
			        "Unexpected using name.");
			
			    NamespaceElement namespaceElement = elements[1] as NamespaceElement;
			    Assert.IsNotNull(namespaceElement, "Expected a NamespaceElement.");
			    Assert.AreEqual("SampleNamespace", namespaceElement.Name,
			        "Unexpected namespace name.");
			
			    Assert.IsNotNull(namespaceElement.Children,
			        "Namespace Children collection should not be null.");
			    Assert.AreEqual(1, namespaceElement.Children.Count,
			        "An unexpected number of namespace child elements were parsed.");
			
			    TypeElement structElement = namespaceElement.Children[0] as TypeElement;
			    Assert.IsNotNull(structElement, "Expected a TypeElement.");
			    Assert.AreEqual(TypeElementType.Structure, structElement.Type,
			        "Expected type to be a structure.");
			    Assert.IsFalse(structElement.IsStatic,
			        "Structure should not be static.");
			    Assert.IsFalse(structElement.IsSealed,
			        "Structures should not be sealed.");
			    Assert.AreEqual("SampleStruct", structElement.Name,
			        "Unexpected structure name.");
			    Assert.AreEqual(3, structElement.HeaderCommentLines.Count,
			        "An unexpected number of class header comment lines were parsed.");
			    foreach (ICommentLine commentLine in
			        structElement.HeaderCommentLines)
			    {
			        Assert.IsTrue(commentLine.IsXmlComment,
			            "Structure header comment should be an XML comment.");
			    }
			    Assert.AreEqual(CodeAccess.Public, structElement.Access,
			        "Unexpected code access level.");
			}
		}		
		
		/// <summary>
		/// Tests parsing a using statement where an end 
		/// of statement is expected.";
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Expected = or ;")]
		public void ParseUsingExpectedStatementEnd()		
		{
			StringReader reader = new StringReader(
			    "using System.Text");
			
			CSharpParser parser = new CSharpParser();
			parser.Parse(reader);
		}		
		
		/// <summary>
		/// Tests parsing a using statement that redefines a class/namespace.
		/// </summary>
		[Test]
		public void ParseUsingRedefineTest()		
		{
			StringReader reader = new StringReader(
			    "using Redefined = System.Text.Encoder;");
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			Assert.AreEqual(1, elements.Count,
			    "An unexpected number of elements were parsed.");
			UsingElement usingElement = elements[0] as UsingElement;
			Assert.IsNotNull(usingElement,
			    "Element is not a UsingElement.");
			Assert.AreEqual("System.Text.Encoder", usingElement.Name,
			    "Unexpected name.");
			Assert.AreEqual("Redefined", usingElement.Redefine);
		}		
		
		/// <summary>
		/// Tests parsing a simple using statement.";
		/// </summary>
		[Test]
		public void ParseUsingTest()		
		{
			StringReader reader = new StringReader(
			    "using System.Text;");
			
			CSharpParser parser = new CSharpParser();
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			Assert.AreEqual(1, elements.Count,
			    "An unexpected number of elements were parsed.");
			UsingElement usingElement = elements[0] as UsingElement;
			Assert.IsNotNull(usingElement,
			    "Element is not a UsingElement.");
			Assert.AreEqual("System.Text", usingElement.Name,
			    "Unexpected name.");
		}		
		
		/// <summary>
		/// Tests that when the end of file is not expected, the appropriate
		/// error is thrown.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ParseException), ExpectedMessage = "Unexpected end of file: Line 3, Column 11")]
		public void UnexpectedEndOfFileTest()		
		{
			using (TextReader reader = CSharpTestFile.GetTestFileReader("UnexpectedEndOfFile.cs"))
			{
			    CSharpParser parser = new CSharpParser();
			    parser.Parse(reader);
			}
		}		
		
		#endregion Public Methods
		
		#region Private Methods
		
		/// <summary>
		/// Gets the ClassMembers test class.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		private static TypeElement GetMembersTestClass(TextReader reader)		
		{
			TypeElement classElement;
			CSharpParser parser = new CSharpParser();
			
			ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
			
			Assert.AreEqual(9, elements.Count,
			    "Unexpected number of top-level elements.");
			
			NamespaceElement namespaceElement = elements[8] as NamespaceElement;
			Assert.IsNotNull(namespaceElement, "Expected a namespace element.");
			
			Assert.AreEqual(2, namespaceElement.Children.Count,
			    "Unexpected number of namespace elements.");
			
			UsingElement usingElement = namespaceElement.Children[0] as UsingElement;
			Assert.IsNotNull(usingElement, "Expected a using element.");
			Assert.AreEqual("System.ComponentModel", usingElement.Name,
			    "Unexpected using element name.");
			
			classElement = namespaceElement.Children[1] as TypeElement;
			Assert.IsNotNull(classElement, "Expected a type element.");
			Assert.AreEqual(TypeElementType.Class, classElement.Type, "Expected a class type.");
			Assert.AreEqual("SampleClass", classElement.Name,
			    "Unexpected class name.");
			
			return classElement;
		}		
		
		#endregion Private Methods
	}
}