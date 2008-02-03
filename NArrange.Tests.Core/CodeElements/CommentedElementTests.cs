using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NArrange.Core;
using NArrange.Core.CodeElements;

namespace NArrange.Tests.Core.CodeElements
{
	/// <summary>
	/// Base test class for commented code elements
	/// </summary>
	/// <typeparam name="TCodeElement"></typeparam>
	public abstract class CommentedElementTests<TCodeElement> : CodeElementTests<TCodeElement>
		where TCodeElement : CommentedElement, new()
	{
		#region Public Methods

		/// <summary>
		/// Tests the AddHeaderCommentLine method
		/// </summary>
		[Test]
		public void AddHeaderCommentLineTest()
		{
			TCodeElement codeElement = new TCodeElement();
			
			codeElement.AddHeaderCommentLine("Regular comment line", false);
			codeElement.AddHeaderCommentLine("<summary>XML comment line</summary>", true);
			
			Assert.AreEqual(2, codeElement.HeaderComments.Count,
			    "AddHeaderCommentLine did not add a comment to the collection.");
			Assert.AreEqual(CommentType.Line, codeElement.HeaderComments[0].Type,
			    "Unexpected comment type.");
			Assert.AreEqual("Regular comment line", codeElement.HeaderComments[0].Text,
			    "Unexpected comment text.");
			Assert.AreEqual(CommentType.XmlLine, codeElement.HeaderComments[1].Type,
			    "Unexpected comment type.");
			Assert.AreEqual("<summary>XML comment line</summary>", codeElement.HeaderComments[1].Text,
			    "Unexpected comment text.");
		}

		/// <summary>
		/// Tests the ClearHeaderCommentLines method
		/// </summary>
		[Test]
		public void ClearHeaderCommentLinesTest()
		{
			TCodeElement codeElement = new TCodeElement();
			codeElement.AddHeaderComment(
			    new CommentElement("Test 1"));
			codeElement.AddHeaderComment(
			   new CommentElement("Test 2"));
			
			Assert.AreEqual(2, codeElement.HeaderComments.Count,
			    "Unexpected number of header comment lines.");
			
			codeElement.ClearHeaderCommentLines();
			
			Assert.AreEqual(0, codeElement.HeaderComments.Count,
			    "Header comment lines was not cleared.");
		}

		#endregion Public Methods
	}
}