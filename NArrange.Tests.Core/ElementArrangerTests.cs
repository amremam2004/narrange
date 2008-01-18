using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NArrange.Core;

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
			public TestNullInserterArranger()			
				: base(ElementType.Constructor, null,
                new ElementFilter("$(Access) == 'Public'"),
                null)
			{
			}		}		
		
		#endregion Other
	}
}