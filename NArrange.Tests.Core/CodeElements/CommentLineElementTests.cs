using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NArrange.Core;
using NArrange.Core.CodeElements;

namespace NArrange.Tests.Core.CodeElements
{
	/// <summary>
	/// Test fixture for the CommentLineElement class
	/// </summary>
	[TestFixture]
	public class CommentLineElementTests
	{
		#region Private Methods

		/// <summary>
		/// Creates an instance for testing Clone
		/// </summary>
		/// <returns></returns>
		private CommentElement DoCreateClonePrototype()
		{
			return new CommentElement("Some comment", CommentType.XmlLine);
		}

		/// <summary>
		/// Actual implementation of the clone test
		/// </summary>
		/// <param name="original"></param>
		/// <param name="clone"></param>
		private void DoVerifyClone(CommentElement original, CommentElement clone)
		{
			Assert.AreEqual(original.Text, clone.Text,
			    "Text property was not copied correctly.");
			Assert.AreEqual(original.Type, clone.Type,
			    "IsXmlComment was not copied correctly.");
		}

		#endregion Private Methods

		#region Public Methods

		/// <summary>
		/// Tests the clone method
		/// </summary>
		[Test]
		public void CloneTest()
		{
			CommentElement original = DoCreateClonePrototype();
			CommentElement clone = original.Clone() as CommentElement;
			Assert.IsNotNull(clone, "Clone did not create an instance of type {0}.",
			    typeof(CommentElement).Name);
			Assert.AreNotSame(original, clone, "Clone should be a different instance.");

			DoVerifyClone(original, clone);
		}

		/// <summary>
		/// Tests the construction of a CommentLine
		/// </summary>
		[Test]
		public void CreateTextAndXmlTest()
		{
			CommentElement commentLine = new CommentElement("Comment here", CommentType.XmlLine);

			//
			// Verify default property values
			//
			Assert.AreEqual(CommentType.XmlLine, commentLine.Type,
			    "Unexpected value for IsXmlComment.");
			Assert.AreEqual("Comment here", commentLine.Text,
			    "Unexpected value for Text.");
		}

		/// <summary>
		/// Tests the construction of a CommentLine
		/// </summary>
		[Test]
		public void CreateTextTest()
		{
			CommentElement commentLine = new CommentElement("Comment here");

			//
			// Verify default property values
			//
			Assert.AreEqual(CommentType.Line, commentLine.Type,
			    "Unexpected default value for IsXmlComment.");
			Assert.AreEqual("Comment here", commentLine.Text,
			    "Unexpected value for Text.");
			Assert.AreEqual(ElementType.CommentLine, commentLine.ElementType);
		}

		/// <summary>
		/// Tests for ToString
		/// </summary>
		[Test]
		public void ToStringTest()
		{
			CommentElement commentLine = new CommentElement("This is some text.");
			string str = commentLine.ToString();
			Assert.AreEqual("This is some text.", str,
			    "Unexpected string representation.");
		}

		#endregion Public Methods
	}
}