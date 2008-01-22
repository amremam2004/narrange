using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NArrange.Core;
using NArrange.Core.CodeElements;

namespace NArrange.Tests.Core.CodeElements
{
	/// <summary>
	/// Test fixture for the MethodElement class.
	/// </summary>
	[TestFixture]
	public class MethodElementTests : CommentedElementTests<MethodElement>
	{
		#region Public Methods

		/// <summary>
		/// Tests the creation of a new instance.
		/// </summary>
		[Test]
		public void CreateTest()
		{
			MethodElement methodElement = new MethodElement();
			
			//
			// Verify default property values
			//
			Assert.AreEqual(ElementType.Method, methodElement.ElementType,
			    "Unexpected element type.");
			Assert.AreEqual(CodeAccess.Public, methodElement.Access,
			    "Unexpected default value for Access.");
			Assert.AreEqual(string.Empty, methodElement.Params,
			    "Unexpected default value for Params.");
			Assert.IsNotNull(methodElement.Attributes,
			    "Attributes collection should be instantiated.");
			Assert.AreEqual(0, methodElement.Attributes.Count,
			    "Attributes collection should be empty.");
			Assert.IsNull(methodElement.BodyText,
			    "Unexpected default value for BodyText.");
			Assert.IsNotNull(methodElement.Children,
			    "Children collection should be instantiated.");
			Assert.AreEqual(0, methodElement.Children.Count,
			    "Children collection should be empty.");
			Assert.IsNotNull(methodElement.HeaderCommentLines,
			    "HeaderCommentLines collection should not be null.");
			Assert.AreEqual(0, methodElement.HeaderCommentLines.Count,
			    "HeaderCommentLines collection should be empty.");
			Assert.IsFalse(methodElement.IsAbstract,
			    "Unexpected default value for IsAbstract.");
			Assert.IsFalse(methodElement.IsSealed,
			    "Unexpected default value for IsSealed.");
			Assert.IsFalse(methodElement.IsStatic,
			    "Unexpected default value for IsStatic.");
			Assert.AreEqual(string.Empty, methodElement.Name,
			    "Unexpected default value for Name.");
		}

		#endregion Public Methods

		#region Protected Methods

		/// <summary>
		/// Creates an instance for cloning
		/// </summary>
		/// <returns></returns>
		protected override MethodElement DoCreateClonePrototype()
		{
			MethodElement prototype = new MethodElement();
			prototype.Name = "SomeMethod";
			prototype.Access = CodeAccess.Internal;
			prototype.AddAttribute(new AttributeElement("Obsolete"));
			prototype.Params = "T val";
			prototype.Type = "bool";
			prototype.TypeParameters.Add(
			    new TypeParameter("T", "class", "new()"));
			
			prototype.AddHeaderCommentLine("/// <summary>");
			prototype.AddHeaderCommentLine("/// This is a method.");
			prototype.AddHeaderCommentLine("/// </summary>");
			
			prototype.BodyText = "{return T != null;}";
			
			prototype.MemberModifiers = MemberModifier.Abstract;
			
			return prototype;
		}

		/// <summary>
		/// Verifies the clone was succesful
		/// </summary>
		/// <param name="original"></param>
		/// <param name="clone"></param>
		protected override void DoVerifyClone(MethodElement original, MethodElement clone)
		{
			Assert.AreEqual(original.Name, clone.Name,
			    "Name was not copied correctly.");
			Assert.AreEqual(original.Params, clone.Params,
			    "Params was not copied correctly.");
			Assert.AreEqual(original.Access, clone.Access,
			    "Access was not copied correctly.");
			Assert.AreEqual(original.Attributes.Count, clone.Attributes.Count,
			    "Attributes were not copied correctly.");
			Assert.AreEqual(original.BodyText, clone.BodyText,
			    "BodyText was not copied correctly.");
			Assert.AreEqual(original.Children.Count, clone.Children.Count,
			    "Children were not copied correctly.");
			Assert.AreEqual(original.HeaderCommentLines.Count, clone.HeaderCommentLines.Count,
			    "HeaderCommentLines were not copied correctly.");
			Assert.AreEqual(original.TypeParameters.Count, clone.TypeParameters.Count,
			    "TypeParameters were not copied correctly.");
			Assert.AreEqual(original.IsAbstract, clone.IsAbstract,
			    "IsAbstract was not copied correctly.");
			Assert.AreEqual(original.IsSealed, clone.IsSealed,
			    "IsSealed was not copied correctly.");
			Assert.AreEqual(original.IsStatic, clone.IsStatic,
			    "IsStatic was not copied correctly.");
			Assert.AreEqual(original.Type, clone.Type,
			    "Type was not copied correctly.");
			Assert.AreEqual(original.IsOperator, clone.IsOperator,
			    "IsOperator was not copied correctly.");
			Assert.AreEqual(original.OperatorType, clone.OperatorType,
			    "OperatorType was not copied correctly.");
		}

		#endregion Protected Methods
	}
}