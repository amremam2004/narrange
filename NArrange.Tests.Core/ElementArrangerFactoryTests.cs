using System;

using NArrange.Core;
using NArrange.Core.Configuration;

using NUnit.Framework;

namespace NArrange.Tests.Core
{
	/// <summary>
	/// Test fixture for the ElementArrangerFactory class
	/// </summary>
	[TestFixture]
	public class ElementArrangerFactoryTests
	{
		#region Public Methods

		/// <summary>
		/// Tests calling CreateElementArranger with a null configuration.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CreateElementArrangerNullConfigurationTest()
		{
			ElementArrangerFactory.CreateElementArranger(null, new ElementConfiguration(),
			    new RegionConfiguration());
		}

		#endregion Public Methods
	}
}