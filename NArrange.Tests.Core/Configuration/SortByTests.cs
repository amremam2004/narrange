namespace NArrange.Tests.Core.Configuration
{
    using System.ComponentModel;

    using NArrange.Core;
    using NArrange.Core.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the SortBy class.
    /// </summary>
    [TestFixture]
    public class SortByTests
    {
        #region Public Methods

        /// <summary>
        /// Tests the creation of a new SortBy.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            SortBy sortBy = new SortBy();

            //
            // Verify default state
            //
            Assert.AreEqual(ElementAttributeType.None, sortBy.By, "Unexpected default value for By.");
            Assert.AreEqual(SortDirection.Ascending, sortBy.Direction, "Unexpected default value for Direction.");
            Assert.IsNull(sortBy.InnerSortBy, "Unexpected default value for InnerSortBy.");
        }

        /// <summary>
        /// Tests the ToString method.
        /// </summary>
        [Test]
        public void ToStringTest()
        {
            SortBy sortBy = new SortBy();
            sortBy.By = ElementAttributeType.Name;

            string str = sortBy.ToString();

            Assert.AreEqual("Sort by: Name", str, "Unexpected string representation.");
        }

        #endregion Public Methods
    }
}