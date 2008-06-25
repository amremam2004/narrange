using System;

using NArrange.Core;

using NUnit.Framework;

namespace NArrange.Tests.Core
{
	/// <summary>
	/// Test fixture for the ElementArranger class.
	/// </summary>
	[TestFixture]
	public class ElementArrangerTests
	{
		#region Public Methods

		/// <summary>
		/// Test the construction with a null inserter
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CreateWithNullInserterTest()
		{
			TestNullInserterArranger arranger = new TestNullInserterArranger();
		}

		#endregion Public Methods

		#region Other

		private class TestNullInserterArranger : ElementArranger
		{
			#region Constructors

			public TestNullInserterArranger()
				: base(ElementType.Constructor, null,
                new ElementFilter("$(Access) == 'Public'"),
                null)
			{
			}

			#endregion Constructors
		}

		#endregion Other
	}
}