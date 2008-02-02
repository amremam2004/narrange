using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

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