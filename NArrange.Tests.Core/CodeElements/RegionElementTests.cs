using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NArrange.Core;
using NArrange.Core.CodeElements;

namespace NArrange.Tests.Core.CodeElements
{
	/// <summary>
	/// Test fixture for the RegionElement class
	/// </summary>
	[TestFixture]
	public class RegionElementTests : CodeElementTests<RegionElement>
	{
		#region Protected Methods

		/// <summary>
		/// Creates an instance for cloning
		/// </summary>
		/// <returns></returns>
		protected override RegionElement DoCreateClonePrototype()
		{
			RegionElement prototype = new RegionElement();
			prototype.Name = "Test Region";
			
			return prototype;
		}

		/// <summary>
		/// Test for ToString()
		/// </summary>
		protected override void DoToStringTest()
		{
			NamespaceElement element = new NamespaceElement();
			element.Name = "Test";
			
			string str = element.ToString();
			Assert.AreEqual("Test", str,
			    "Unexpected value returned for ToString.");
		}

		/// <summary>
		/// Verifies that a clone has the same state as the original
		/// </summary>
		/// <param name="original"></param>
		/// <param name="clone"></param>
		protected override void DoVerifyClone(RegionElement original, RegionElement clone)
		{
			Assert.AreEqual(original.Name, clone.Name);
		}

		#endregion Protected Methods

		#region Public Methods

		/// <summary>
		/// Tests constructing a new RegionElement
		/// </summary>
		[Test]
		public void CreateTest()
		{
			RegionElement element = new RegionElement();
			
			//
			// Verify default values
			//
			Assert.AreEqual(ElementType.Region, element.ElementType,
			    "Unexpected element type.");
			Assert.AreEqual(string.Empty, element.Name,
			    "Unexpected default value for Name.");
			Assert.IsNotNull(element.Children,
			    "Children collection should not be null.");
			Assert.AreEqual(0, element.Children.Count,
			    "Children collection should be empty.");
		}

		#endregion Public Methods
	}
}