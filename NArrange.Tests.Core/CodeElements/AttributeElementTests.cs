using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NArrange.Core;
using NArrange.Core.CodeElements;

namespace NArrange.Tests.Core.CodeElements
{
	/// <summary>
	/// Test fixture for the AttributeElement class.
	/// </summary>
	[TestFixture]
	public class AttributeElementTests : CommentedElementTests<AttributeElement>
	{
		#region Protected Methods

		/// <summary>
		/// Creates an instance for cloning.
		/// </summary>
		/// <returns></returns>
		protected override AttributeElement DoCreateClonePrototype()
		{
			AttributeElement prototype = new AttributeElement();
			prototype.Name = "SampleAttribute";
			prototype.Target = "class";
			prototype.BodyText = "\"Test\"";

			AttributeElement child1 = new AttributeElement();
			AttributeElement child2 = new AttributeElement();

			prototype.AddChild(child1);
			prototype.AddChild(child2);

			return prototype;
		}

		/// <summary>
		/// Verifies that a clone has the same state as the original.
		/// </summary>
		/// <param name="original"></param>
		/// <param name="clone"></param>
		protected override void DoVerifyClone(AttributeElement original, AttributeElement clone)
		{
			Assert.AreEqual(original.Name, clone.Name);
			Assert.AreEqual(original.Target, clone.Target);
			Assert.AreEqual(original.BodyText, clone.BodyText);

			Assert.AreEqual(original.Children.Count, clone.Children.Count);
		}

		#endregion Protected Methods

		#region Public Methods

		/// <summary>
		/// Tests constructing a new UsingElement.
		/// </summary>
		[Test]
		public void CreateTest()
		{
			AttributeElement element = new AttributeElement();

			//
			// Verify default values
			//
			Assert.IsNull(element.Target,
			    "Unexpected defaulf value for Target.");

			Assert.IsNotNull(element.Children,
			    "Children collection should not be null.");
			Assert.AreEqual(0, element.Children.Count,
			    "Children collection should be empty.");
			Assert.IsNotNull(element.HeaderComments,
			    "HeaderCommentLines collection should not be null.");
			Assert.AreEqual(0, element.HeaderComments.Count,
			    "HeaderCommentLines collection should be empty.");
		}

		#endregion Public Methods
	}
}