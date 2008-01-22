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