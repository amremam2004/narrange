using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NArrange.Core;
using NArrange.Core.Configuration;

namespace NArrange.Tests.Core.Configuration
{
	/// <summary>
	/// Test fixture for the FilterBy class
	/// </summary>
	[TestFixture]
	public class FilterByTests
	{
		#region Public Methods

		/// <summary>
		/// Tests the creation of a new FilterBy
		/// </summary>
		[Test]
		public void CreateTest()
		{
			FilterBy filterBy = new FilterBy();
			
			//
			// Verify default state
			//
			Assert.IsNull(filterBy.Condition,
			    "Unexpected default value for Condition.");
		}

		/// <summary>
		/// Tests the ToString method
		/// </summary>
		[Test]
		public void ToStringTest()
		{
			FilterBy filterBy = new FilterBy();
			filterBy.Condition = "$(Access) == 'Protected'";
			
			string str = filterBy.ToString();
			
			Assert.AreEqual("Filter by: $(Access) == 'Protected'", str,
			    "Unexpected string representation.");
		}

		#endregion Public Methods
	}
}