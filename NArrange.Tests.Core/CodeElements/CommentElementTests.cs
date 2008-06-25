using NArrange.Core;
using NArrange.Core.CodeElements;

using NUnit.Framework;

namespace NArrange.Tests.Core.CodeElements
{
	/// <summary>
	/// Test fixture for the CommentElement class.
	/// </summary>
	[TestFixture]
	public class CommentElementTests : CodeElementTests<CommentElement>
	{
		#region Protected Methods

		/// <summary>
		/// Creates an instance for cloning.
		/// </summary>
		/// <returns></returns>
		protected override CommentElement DoCreateClonePrototype()
		{
			CommentElement prototype = new CommentElement("Block comment here...", CommentType.Block);

			return prototype;
		}

		/// <summary>
		/// Performs the ToString test.
		/// </summary>
		protected override void DoToStringTest()
		{
			CommentElement commentElement = new CommentElement(
			    "This is a block comment...", CommentType.Block);
			Assert.AreEqual(commentElement.Text, commentElement.ToString(),
			    "Unexpected string representation.");
		}

		/// <summary>
		/// Verifies that a clone has the same state as the original.
		/// </summary>
		/// <param name="original"></param>
		/// <param name="clone"></param>
		protected override void DoVerifyClone(CommentElement original, CommentElement clone)
		{
			Assert.AreEqual(original.Name, clone.Name);
			Assert.AreEqual(original.Text, clone.Text);
			Assert.AreEqual(original.Type, clone.Type);
		}

		#endregion Protected Methods

		#region Public Methods

		/// <summary>
		/// Tests constructing a new CommentElement
		/// </summary>
		[Test]
		public void CreateDefaultTest()
		{
			CommentElement element = new CommentElement();

			//
			// Verify default values
			//
			Assert.AreEqual(string.Empty, element.Name,
			    "Unexpected default value for Name.");
			Assert.AreEqual(CommentType.Line, element.Type,
			    "Unexpected default value for Type.");
			Assert.IsNull(element.Text,
			    "Unexpected default value for Text.");
		}

		/// <summary>
		/// Tests constructing a new CommentElement
		/// </summary>
		[Test]
		public void CreateTextAndTypeTest()
		{
			CommentElement element = new CommentElement("Test", CommentType.Block);

			//
			// Verify default values
			//
			Assert.AreEqual(string.Empty, element.Name,
			    "Unexpected default value for Name.");
			Assert.AreEqual(CommentType.Block, element.Type,
			    "Unexpected value for Type.");
			Assert.AreEqual("Test", element.Text,
			    "Unexpected value for Text.");
		}

		/// <summary>
		/// Tests constructing a new CommentElement
		/// </summary>
		[Test]
		public void CreateTypeTest()
		{
			CommentElement element = new CommentElement(CommentType.XmlLine);

			//
			// Verify default values
			//
			Assert.AreEqual(string.Empty, element.Name,
			    "Unexpected default value for Name.");
			Assert.AreEqual(CommentType.XmlLine, element.Type,
			    "Unexpected value for Type.");
			Assert.IsNull(element.Text,
			    "Unexpected default value for Text.");
		}

		#endregion Public Methods
	}
}