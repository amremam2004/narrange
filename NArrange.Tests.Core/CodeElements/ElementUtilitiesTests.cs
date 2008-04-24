using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NArrange.Core;
using NArrange.Core.CodeElements;
using NArrange.Core.Configuration;

namespace NArrange.Tests.Core.CodeElements
{
	/// <summary>
	/// Test fixture for the ElementUtilities class.
	/// </summary>
	[TestFixture]
	public class ElementUtilitiesTests
	{
		#region Public Methods

		/// <summary>
		/// Tests Format with a null element
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FormatNullElementTest()
		{
			ElementUtilities.Format("$(Name)", null);
		}

		/// <summary>
		/// Tests Format with a null format string
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FormatNullStringTest()
		{
			ElementUtilities.Format(null, new MethodElement());
		}

		/// <summary>
		/// Tests the Format method with a valid format string and element.
		/// </summary>
		[Test]
		public void FormatTest()
		{
			MethodElement methodElement = new MethodElement();
			methodElement.Name = "Test";

			string formatted = ElementUtilities.Format("End $(ElementType) $(Name)",
			    methodElement);

			Assert.AreEqual("End Method Test", formatted,
			    "Unexpected formatted result.");
		}

		/// <summary>
		/// Tests the GetAttribute method for Access
		/// </summary>
		[Test]
		public void GetAttributeAccessTest()
		{
			FieldElement fieldElement = new FieldElement();
			fieldElement.Name = "TestField";
			fieldElement.Access = CodeAccess.Protected;

			string attribute = ElementUtilities.GetAttribute(ElementAttributeType.Access, fieldElement);
			Assert.AreEqual("Protected", attribute, "Unexpected attribute.");
		}

		/// <summary>
		/// Tests the GetAttribute method for ElementType
		/// </summary>
		[Test]
		public void GetAttributeElementTypeTest()
		{
			FieldElement fieldElement = new FieldElement();
			fieldElement.Name = "TestField";
			fieldElement.Access = CodeAccess.Protected;

			string attribute = ElementUtilities.GetAttribute(ElementAttributeType.ElementType, fieldElement);
			Assert.AreEqual("Field", attribute, "Unexpected attribute.");
		}

		/// <summary>
		/// Tests the GetAttribute method for Modifier
		/// </summary>
		[Test]
		public void GetAttributeModifierTest()
		{
			FieldElement fieldElement = new FieldElement();
			fieldElement.Name = "TestField";
			fieldElement.Access = CodeAccess.Protected;
			fieldElement.ReturnType = "int";
			fieldElement.MemberModifiers = MemberModifiers.Static;

			string attribute = ElementUtilities.GetAttribute(ElementAttributeType.Modifier, fieldElement);
			Assert.AreEqual("Static", attribute, "Unexpected attribute.");

			TypeElement typeElement = new TypeElement();
			typeElement.TypeModifiers = TypeModifiers.Sealed;

			attribute = ElementUtilities.GetAttribute(ElementAttributeType.Modifier, typeElement);
			Assert.AreEqual("Sealed", attribute, "Unexpected attribute.");

			UsingElement usingElement = new UsingElement();
			usingElement.Name = "System";

			attribute = ElementUtilities.GetAttribute(ElementAttributeType.Modifier, usingElement);
			Assert.AreEqual(string.Empty, attribute, "Unexpected attribute.");
		}

		/// <summary>
		/// Tests the GetAttribute method for Name
		/// </summary>
		[Test]
		public void GetAttributeNameTest()
		{
			FieldElement fieldElement = new FieldElement();
			fieldElement.Name = "TestField";

			string attribute = ElementUtilities.GetAttribute(ElementAttributeType.Name, fieldElement);
			Assert.AreEqual("TestField", attribute, "Unexpected attribute.");
		}

		/// <summary>
		/// Tests the GetAttribute method for None
		/// </summary>
		[Test]
		public void GetAttributeNoneTest()
		{
			FieldElement fieldElement = new FieldElement();
			fieldElement.Name = "TestField";

			string attribute = ElementUtilities.GetAttribute(ElementAttributeType.None, fieldElement);
			Assert.AreEqual(string.Empty, attribute, "Unexpected attribute.");
		}

		/// <summary>
		/// Tests the GetAttribute method for Type
		/// </summary>
		[Test]
		public void GetAttributeTypeTest()
		{
			FieldElement fieldElement = new FieldElement();
			fieldElement.Name = "TestField";
			fieldElement.Access = CodeAccess.Protected;
			fieldElement.ReturnType = "int";

			string attribute = ElementUtilities.GetAttribute(ElementAttributeType.Type, fieldElement);
			Assert.AreEqual("int", attribute, "Unexpected attribute.");

			TypeElement typeElement = new TypeElement();
			typeElement.TypeElementType = TypeElementType.Interface;

			attribute = ElementUtilities.GetAttribute(ElementAttributeType.Type, typeElement);
			Assert.AreEqual("Interface", attribute, "Unexpected attribute.");

			UsingElement usingElement = new UsingElement();
			usingElement.Name = "System";

			attribute = ElementUtilities.GetAttribute(ElementAttributeType.Type, usingElement);
			Assert.AreEqual(string.Empty, attribute, "Unexpected attribute.");
		}

		#endregion Public Methods
	}
}