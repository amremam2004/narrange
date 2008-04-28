using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NArrange.Core;
using NArrange.Core.CodeElements;

namespace NArrange.Tests.Core.CodeElements
{
	/// <summary>
	/// Test fixture for the PropertyElement class.
	/// </summary>
	[TestFixture]
	public class PropertyElementTests : AttributedElementTests<PropertyElement>
	{
		#region Protected Methods

		/// <summary>
		/// Creates an instance for cloning
		/// </summary>
		/// <returns></returns>
		protected override PropertyElement DoCreateClonePrototype()
		{
			PropertyElement prototype = new PropertyElement();
			prototype.Name = "SomeProperty";
			prototype.Access = CodeAccess.Internal;
			prototype.AddAttribute(new AttributeElement("Obsolete"));
			prototype.Type = "string";

			prototype.AddHeaderCommentLine("/// <summary>");
			prototype.AddHeaderCommentLine("/// This is a field.");
			prototype.AddHeaderCommentLine("/// </summary>");

			prototype.BodyText = "get{return string.empty}";

			prototype.MemberModifiers = MemberModifiers.Abstract; 

			return prototype;
		}

		/// <summary>
		/// Verifies the clone was succesful
		/// </summary>
		/// <param name="original"></param>
		/// <param name="clone"></param>
		protected override void DoVerifyClone(PropertyElement original, PropertyElement clone)
		{
			Assert.AreEqual(original.Name, clone.Name,
			    "Name was not copied correctly.");
			Assert.AreEqual(original.Access, clone.Access,
			    "Access was not copied correctly.");
			Assert.AreEqual(original.Attributes.Count, clone.Attributes.Count,
			    "Attributes were not copied correctly.");
			Assert.AreEqual(original.BodyText, clone.BodyText,
			    "BodyText was not copied correctly.");
			Assert.AreEqual(original.Children.Count, clone.Children.Count,
			    "Children were not copied correctly.");
			Assert.AreEqual(original.HeaderComments.Count, clone.HeaderComments.Count,
			    "HeaderCommentLines were not copied correctly.");
			Assert.AreEqual(original.IsAbstract, clone.IsAbstract,
			    "IsAbstract was not copied correctly.");
			Assert.AreEqual(original.IsSealed, clone.IsSealed,
			    "IsSealed was not copied correctly.");
			Assert.AreEqual(original.IsStatic, clone.IsStatic,
			    "IsStatic was not copied correctly.");
			Assert.AreEqual(original.Type, clone.Type,
			    "Type was not copied correctly.");
		}

		#endregion Protected Methods

		#region Public Methods

		/// <summary>
		/// Tests the creation of a new instance.
		/// </summary>
		[Test]
		public void CreateTest()
		{
			PropertyElement propertyElement = new PropertyElement();

			//
			// Verify default property values
			//
			Assert.AreEqual(ElementType.Property, propertyElement.ElementType,
			    "Unexpected element type.");
			Assert.AreEqual(CodeAccess.Public, propertyElement.Access,
			    "Unexpected default value for Access.");
			Assert.IsNotNull(propertyElement.Attributes,
			    "Attributes collection should be instantiated.");
			Assert.AreEqual(0, propertyElement.Attributes.Count,
			    "Attributes collection should be empty.");
			Assert.IsNull(propertyElement.BodyText,
			    "Unexpected default value for BodyText.");
			Assert.IsNotNull(propertyElement.Children,
			    "Children collection should be instantiated.");
			Assert.AreEqual(0, propertyElement.Children.Count,
			    "Children collection should be empty.");
			Assert.IsNotNull(propertyElement.HeaderComments,
			    "HeaderCommentLines collection should not be null.");
			Assert.AreEqual(0, propertyElement.HeaderComments.Count,
			    "HeaderCommentLines collection should be empty.");
			Assert.IsFalse(propertyElement.IsAbstract,
			    "Unexpected default value for IsAbstract.");
			Assert.IsFalse(propertyElement.IsSealed,
			    "Unexpected default value for IsSealed.");
			Assert.IsFalse(propertyElement.IsStatic,
			    "Unexpected default value for IsStatic.");
			Assert.AreEqual(string.Empty, propertyElement.Name,
			    "Unexpected default value for Name.");
		}

		#endregion Public Methods
	}
}