using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NArrange.Core.CodeElements;

namespace NArrange.Tests.Core.CodeElements
{
	/// <summary>
	/// Test fixture for the CommentLine class
	/// </summary>
	[TestFixture]
	public class CommentLineTests	
	{
		#region Public Methods
		
		/// <summary>
		/// Tests the construction of a CommentLine
		/// </summary>
		[Test]
		public void CreateTextAndXmlTest()		
		{
			CommentLine commentLine = new CommentLine("Comment here", true);
			
			//
			// Verify default property values
			//
			Assert.IsTrue(commentLine.IsXmlComment,
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
			CommentLine commentLine = new CommentLine("Comment here");
			
			//
			// Verify default property values
			//
			Assert.IsFalse(commentLine.IsXmlComment,
			    "Unexpected default value for IsXmlComment.");
			Assert.AreEqual("Comment here", commentLine.Text,
			    "Unexpected value for Text.");
		}		
		
		/// <summary>
		/// Tests for ToString
		/// </summary>
		[Test]
		public void ToStringTest()		
		{
			CommentLine commentLine = new CommentLine("This is some text.", true);
			string str = commentLine.ToString();
			Assert.AreEqual("This is some text.", str,
			    "Unexpected string representation.");
		}		
		
		#endregion Public Methods

	}
}