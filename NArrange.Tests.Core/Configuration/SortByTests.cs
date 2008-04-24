using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using NUnit.Framework;

using NArrange.Core;
using NArrange.Core.Configuration;

namespace NArrange.Tests.Core.Configuration
{
	/// <summary>
	/// Test fixture for the SortBy class
	/// </summary>
	[TestFixture]
	public class SortByTests
	{
		#region Public Methods

		/// <summary>
		/// Tests the creation of a new SortBy
		/// </summary>
		[Test]
		public void CreateTest()
		{
			SortBy sortBy = new SortBy();

			//
			// Verify default state
			//
			Assert.AreEqual(ElementAttributeType.None, sortBy.By,
			    "Unexpected default value for By.");
			Assert.AreEqual(ListSortDirection.Ascending, sortBy.Direction,
			    "Unexpected default value for Direction.");
			Assert.IsNull(sortBy.InnerSortBy,
			    "Unexpected default value for InnerSortBy.");
		}

		/// <summary>
		/// Tests the ToString method
		/// </summary>
		[Test]
		public void ToStringTest()
		{
			SortBy sortBy = new SortBy();
			sortBy.By = ElementAttributeType.Name;

			string str = sortBy.ToString();

			Assert.AreEqual("Sort by: Name", str,
			    "Unexpected string representation.");
		}

		#endregion Public Methods
	}
}