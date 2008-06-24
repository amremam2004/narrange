using NArrange.Core;
using NArrange.Core.Configuration;
using NUnit.Framework;

namespace NArrange.Tests.Core.Configuration
{
	/// <summary>
	/// Test fixture for the GroupBy class
	/// </summary>
	[TestFixture]
	public class GroupByTests
	{
		#region Public Methods

		/// <summary>
		/// Tests the creation of a new GroupBy
		/// </summary>
		[Test]
		public void CreateTest()
		{
			GroupBy groupBy = new GroupBy();

			//
			// Verify default state
			//
			Assert.IsNull(groupBy.AttributeCapture,
			    "Unexpected default value AttributeCapture.");
			Assert.AreEqual(ElementAttributeType.None, groupBy.By,
			    "Unexpected default value for By.");
			Assert.AreEqual(GroupSeparatorType.NewLine, groupBy.SeparatorType,
			    "Unexpected default value for SeparateType.");
			Assert.IsNull(groupBy.CustomSeparator,
			    "Unexpected default value for CustomSeparator.");
		}

		/// <summary>
		/// Tests the CustomSeparator property
		/// </summary>
		[Test]
		public void CustomSeparatorTest()
		{
			GroupBy groupBy = new GroupBy();
			Assert.IsNull(groupBy.CustomSeparator,
			    "Unexpected default value for CustomSeparator.");

			groupBy.CustomSeparator = "// This is a group\r\n";
			Assert.AreEqual("// This is a group\r\n", groupBy.CustomSeparator,
			    "CustomSeparator was not set correctly.");
		}

		/// <summary>
		/// Tests the SeparateType property
		/// </summary>
		[Test]
		public void SeparatorTypeTest()
		{
			GroupBy groupBy = new GroupBy();
			Assert.AreEqual(GroupSeparatorType.NewLine, groupBy.SeparatorType,
			    "Unexpected default value for SeparateType.");

			groupBy.SeparatorType = GroupSeparatorType.Custom;
			Assert.AreEqual(GroupSeparatorType.Custom, groupBy.SeparatorType,
			    "SeparateType was not set correctly.");
		}

		/// <summary>
		/// Tests the ToString method
		/// </summary>
		[Test]
		public void ToStringTest()
		{
			GroupBy groupBy = new GroupBy();
			groupBy.By = ElementAttributeType.Access;

			string str = groupBy.ToString();

			Assert.AreEqual("Group by: Access", str,
			    "Unexpected string representation.");
		}

		#endregion Public Methods
	}
}