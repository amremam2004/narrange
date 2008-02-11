using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

using NUnit.Framework;

using NArrange.CSharp;
using NArrange.Core;
using NArrange.Core.CodeElements;
using NArrange.Core.Configuration;
using NArrange.Tests.CSharp;

namespace NArrange.Tests.Core
{
	/// <summary>
	/// Test fixture for the CodeArranger class
	/// </summary>
	[TestFixture]
	public class CodeArrangerTests
	{
		#region Fields

		private ReadOnlyCollection<ICodeElement> TestElements;		
		
		#endregion Fields

		#region Public Methods

		/// <summary>
		/// Test the construction with a null configuration
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CreateWithNullTest()
		{
			CodeArranger arranger = new CodeArranger(null);
		}

		/// <summary>
		/// Tests arranging a simple
		/// </summary>
		[Test]
		public void DefaultArrangeSimpleClassTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();
			
			TypeElement classElement = new TypeElement();
			classElement.Type = TypeElementType.Class;
			classElement.Access = CodeAccess.Public;
			classElement.Name = "TestClass";
			
			NamespaceElement namesspaceElement = new NamespaceElement();
			namesspaceElement.Name = "TestNamespace";
			namesspaceElement.AddChild(classElement);
			
			MethodElement methodElement = new MethodElement();
			methodElement.Type = "void";
			methodElement.Access = CodeAccess.Public;
			methodElement.Name = "DoSomething";
			classElement.AddChild(methodElement);
			
			FieldElement fieldElement = new FieldElement();
			fieldElement.Type = "bool";
			fieldElement.Access = CodeAccess.Private;
			fieldElement.Name = "_val";
			classElement.AddChild(fieldElement);
			
			PropertyElement propertyElement = new PropertyElement();
			propertyElement.Type = "bool";
			propertyElement.Access = CodeAccess.Public;
			propertyElement.Name = "Value";
			propertyElement.BodyText = "return _val";
			classElement.AddChild(propertyElement);
			
			codeElements.Add(namesspaceElement);
			
			CodeArranger arranger = new CodeArranger(CodeConfiguration.Default);
			
			ReadOnlyCollection<ICodeElement> arranged = 
			    arranger.Arrange(codeElements.AsReadOnly());
			
			Assert.AreEqual(1, arranged.Count,
			    "After arranging, an unexpected number of elements were returned.");
			NamespaceElement namespaceElementTest = arranged[0] as NamespaceElement;
			Assert.IsNotNull(namespaceElementTest, "Expected a namespace element.");
			
			Assert.AreEqual(1, namespaceElementTest.Children.Count,
			    "After arranging, an unexpected number of namespace elements were returned.");
			TypeElement typeElement = namespaceElementTest.Children[0] as TypeElement;
			Assert.IsNotNull(typeElement, "Expected a type element.");
			
			Assert.AreEqual(TypeElementType.Class, typeElement.Type,
			    "Unexpected type element type.");
			Assert.AreEqual(classElement.Name, typeElement.Name,
			    "Unexpected type element name.");
			
			Assert.AreEqual(3, typeElement.Children.Count,
			    "An unexpected number of class child elements were returned.");
			List<RegionElement> regionElements = new List<RegionElement>();
			foreach (ICodeElement classChildElement in typeElement.Children)
			{
			    RegionElement regionElement = classChildElement as RegionElement;
			    Assert.IsNotNull(regionElement, "Expected a region element but was {0}.", 
			        classChildElement.ElementType);
			    regionElements.Add(regionElement);
			}
			
			Assert.AreEqual("Fields", regionElements[0].Name,
			    "Unexpected region element name.");
			Assert.AreEqual("Public Properties", regionElements[1].Name,
			    "Unexpected region element name.");
			Assert.AreEqual("Public Methods", regionElements[2].Name,
			    "Unexpected region element name.");
			
			GroupElement fieldGroupElement = regionElements[0].Children[0] as GroupElement;
			Assert.IsNotNull(fieldGroupElement, "Expected a group element for fields.");
			
			foreach (ICodeElement codeElement in fieldGroupElement.Children)
			{
			    FieldElement fieldElementTest = codeElement as FieldElement;
			    Assert.IsNotNull(fieldElementTest, 
			        "Expected a field element but was type {0}: {1}", 
			        codeElement.ElementType, codeElement);
			}
			
			foreach (ICodeElement codeElement in regionElements[1].Children)
			{
			    PropertyElement propertyElementTest = codeElement as PropertyElement;
			    Assert.IsNotNull(propertyElementTest,
			        "Expected a property element but type {0}: {1}",
			         codeElement.ElementType, codeElement);
			}
			
			foreach (ICodeElement codeElement in regionElements[2].Children)
			{
			    MethodElement methodElementTest = codeElement as MethodElement;
			    Assert.IsNotNull(methodElementTest,
			        "Expected a method element but type {0}: {1}",
			         codeElement.ElementType, codeElement);
			}
		}

		/// <summary>
		/// Tests arranging with the default configuration
		/// </summary>
		[Test]
		public void DefualtArrangeTest()
		{
			CodeArranger arranger = new CodeArranger(CodeConfiguration.Default);
			
			ReadOnlyCollection<ICodeElement> arranged = arranger.Arrange(TestElements);
			
			//
			// Verify using statements were grouped and sorted correctly
			//
			Assert.AreEqual(3, arranged.Count,
			    "An unexpected number of root elements were returned from Arrange.");
			
			GroupElement groupElement = arranged[0] as GroupElement;
			Assert.IsNotNull(groupElement, "Expected a group element.");
			Assert.AreEqual(7, groupElement.Children.Count,
			    "Group contains an unexpected number of child elements.");
			Assert.AreEqual("System", groupElement.Name,
			    "Unexpected group name.");
			
			string lastUsingName = null;
			foreach (CodeElement groupedElement in groupElement.Children)
			{
			    UsingElement usingElement = groupedElement as UsingElement;
			    Assert.IsNotNull(usingElement, "Expected a using element.");
			
			    string usingName = usingElement.Name;
			    if (lastUsingName != null)
			    {
			        Assert.AreEqual(-1, lastUsingName.CompareTo(usingName),
			            "Expected using statements to be sorted by name.");
			    }
			}
			
			//
			// Verify the attribute
			//
			AttributeElement attributeElement = arranged[1] as AttributeElement;
			Assert.IsNotNull(attributeElement, "Expected an attribute.");
			
			//
			// Verify the namespace arrangement
			//
			NamespaceElement namespaceElement = arranged[2] as NamespaceElement;
			Assert.IsNotNull(namespaceElement, "Expected a namespace element.");
		}

		/// <summary>
		/// Tests arranging using statements in a region with the default configuration
		/// </summary>
		[Test]
		public void DefualtArrangeUsingsInRegionTest()
		{
			CodeArranger arranger = new CodeArranger(CodeConfiguration.Default);
			
			List<ICodeElement> codeElements = new List<ICodeElement>();
			
			RegionElement regionElement = new RegionElement();
			regionElement.Name = "Using Directives";
			
			UsingElement usingElement1 = new UsingElement();
			usingElement1.Name = "System";
			regionElement.AddChild(usingElement1);
			
			UsingElement usingElement2 = new UsingElement();
			usingElement2.Name = "System.Text";
			regionElement.AddChild(usingElement2);
			
			codeElements.Add(regionElement);
			
			ReadOnlyCollection<ICodeElement> arranged = arranger.Arrange(codeElements.AsReadOnly());
			
			//
			// Verify using statements were stripped from the region
			//
			Assert.AreEqual(1, arranged.Count,
				"An unexpected number of root elements were returned from Arrange.");
			GroupElement groupElement = arranged[0] as GroupElement;
			Assert.IsNotNull(groupElement,
				"Expected a group element.");
			Assert.AreEqual("System", groupElement.Name);
			foreach (ICodeElement arrangedElement in groupElement.Children)
			{
				Assert.IsTrue(arrangedElement is UsingElement,
					"Expected a using element.");
			}
		}

		/// <summary>
		/// Performs setup for this test fixture
		/// </summary>
		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			CSharpTestFile testFile = CSharpTestUtilities.GetClassMembersFile();
			using (TextReader reader = testFile.GetReader())
			{
			    CSharpParser parser = new CSharpParser();
			    TestElements = parser.Parse(reader);
			
			    Assert.IsTrue(TestElements.Count > 0,
			        "Test file does not contain any elements.");
			}
		}

		#endregion Public Methods
	}
}