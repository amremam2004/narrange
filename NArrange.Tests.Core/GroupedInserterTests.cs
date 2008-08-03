namespace NArrange.Tests.Core
{
    using System;

    using NArrange.Core;
    using NArrange.Core.CodeElements;
    using NArrange.Core.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the GroupedInserter class.
    /// </summary>
    [TestFixture]
    public class GroupedInserterTests
    {
        #region Methods

        /// <summary>
        /// Tests the creation of a GroupedInserter instance.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            GroupBy groupBy = new GroupBy();

            GroupedInserter regionedInserter = new GroupedInserter(groupBy);
        }

        /// <summary>
        /// Test construction with a null configuration.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateWithNullTest()
        {
            GroupedInserter regionedInserter = new GroupedInserter(null);
        }

        /// <summary>
        /// Tests inserting groups in a sorted manner.
        /// </summary>
        [Test]
        public void GroupSortTest()
        {
            GroupBy groupBy = new GroupBy();
            groupBy.By = ElementAttributeType.Name;
            groupBy.AttributeCapture = "^(.*?)(\\.|$)";
            groupBy.Direction = SortDirection.Ascending;

            GroupedInserter groupedInserter = new GroupedInserter(groupBy);

            //
            // Create a parent element
            //
            GroupElement groupElement = new GroupElement();
            Assert.AreEqual(0, groupElement.Children.Count, "Parent element should not have any children.");

            // Insert elements
            groupedInserter.InsertElement(groupElement, new UsingElement("NUnit.Framework"));
            groupedInserter.InsertElement(groupElement, new UsingElement("NArrange.Core"));
            groupedInserter.InsertElement(groupElement, new UsingElement("NArrange.Core.Configuration"));
            groupedInserter.InsertElement(groupElement, new UsingElement("System"));
            groupedInserter.InsertElement(groupElement, new UsingElement("System.IO"));

            Assert.AreEqual(3, groupElement.Children.Count, "Unexpected number of child groups.");

            GroupElement childGroup;

            // System usings should always come first
            childGroup = groupElement.Children[0] as GroupElement;
            Assert.IsNotNull(childGroup, "Expected a child group.");
            Assert.AreEqual(2, childGroup.Children.Count, "Unexpected number of group children.");
            Assert.AreEqual("System", childGroup.Children[0].Name);
            Assert.AreEqual("System.IO", childGroup.Children[1].Name);

            childGroup = groupElement.Children[1] as GroupElement;
            Assert.IsNotNull(childGroup, "Expected a child group.");
            Assert.AreEqual(2, childGroup.Children.Count, "Unexpected number of group children.");
            Assert.AreEqual("NArrange.Core", childGroup.Children[0].Name);
            Assert.AreEqual("NArrange.Core.Configuration", childGroup.Children[1].Name);

            childGroup = groupElement.Children[2] as GroupElement;
            Assert.IsNotNull(childGroup, "Expected a child group.");
            Assert.AreEqual(1, childGroup.Children.Count, "Unexpected number of group children.");
            Assert.AreEqual("NUnit.Framework", childGroup.Children[0].Name);
        }

        /// <summary>
        /// Tests inserting elements.
        /// </summary>
        [Test]
        public void InsertSortedTest()
        {
            GroupBy groupBy = new GroupBy();
            groupBy.By = ElementAttributeType.Name;
            groupBy.AttributeCapture = "^(.*?)(\\.|$)";

            SortBy sortBy = new SortBy();
            sortBy.By = ElementAttributeType.Name;
            SortedInserter sortedInserter = new SortedInserter(ElementType.Using, sortBy);

            GroupedInserter groupedInserter = new GroupedInserter(groupBy, sortedInserter);

            //
            // Create a parent element
            //
            GroupElement groupElement = new GroupElement();
            Assert.AreEqual(0, groupElement.Children.Count, "Parent element should not have any children.");

            //
            // With no criteria specified, elements should just be inserted
            // at the end of the collection.
            //
            UsingElement using1 = new UsingElement();
            using1.Name = "System.IO";
            groupedInserter.InsertElement(groupElement, using1);
            Assert.AreEqual(1, groupElement.Children.Count, "Group element was not inserted into the parent.");
            Assert.IsTrue(groupElement.Children[0] is GroupElement, "Group element was not inserted into the parent.");
            Assert.AreEqual(1, groupElement.Children[0].Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, groupElement.Children[0].Children.IndexOf(using1), "Element was not inserted at the correct index.");

            UsingElement using2 = new UsingElement();
            using2.Name = "System";
            groupedInserter.InsertElement(groupElement, using2);
            Assert.AreEqual(1, groupElement.Children.Count, "Group element was not inserted into the parent.");
            Assert.IsTrue(groupElement.Children[0] is GroupElement, "Group element was not inserted into the parent.");
            Assert.AreEqual(2, groupElement.Children[0].Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, groupElement.Children[0].Children.IndexOf(using2), "Element is not at the correct index.");
            Assert.AreEqual(1, groupElement.Children[0].Children.IndexOf(using1), "Element is not at the correct index.");

            UsingElement using3 = new UsingElement();
            using3.Name = "System.Text";
            groupedInserter.InsertElement(groupElement, using3);
            Assert.AreEqual(1, groupElement.Children.Count, "Group element was not inserted into the parent.");
            Assert.IsTrue(groupElement.Children[0] is GroupElement, "Group element was not inserted into the parent.");
            Assert.AreEqual(3, groupElement.Children[0].Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, groupElement.Children[0].Children.IndexOf(using2), "Element is not at the correct index[0].Children.");
            Assert.AreEqual(1, groupElement.Children[0].Children.IndexOf(using1), "Element is not at the correct index.");
            Assert.AreEqual(2, groupElement.Children[0].Children.IndexOf(using3), "Element is not at the correct index.");
        }

        /// <summary>
        /// Tests inserting elements.
        /// </summary>
        [Test]
        public void InsertTest()
        {
            GroupBy groupBy = new GroupBy();
            groupBy.By = ElementAttributeType.Name;
            groupBy.AttributeCapture = "^(.*?)(\\.|$)";

            GroupedInserter groupedInserter = new GroupedInserter(groupBy);

            //
            // Create a parent element
            //
            GroupElement groupElement = new GroupElement();
            Assert.AreEqual(0, groupElement.Children.Count, "Parent element should not have any children.");

            //
            // With no criteria specified, elements should just be inserted
            // at the end of the collection.
            //
            UsingElement using1 = new UsingElement();
            using1.Name = "System.IO";
            groupedInserter.InsertElement(groupElement, using1);
            Assert.AreEqual(1, groupElement.Children.Count, "Group element was not inserted into the parent.");
            Assert.IsTrue(groupElement.Children[0] is GroupElement, "Group element was not inserted into the parent.");
            Assert.AreEqual(1, groupElement.Children[0].Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, groupElement.Children[0].Children.IndexOf(using1), "Element was not inserted at the correct index.");

            UsingElement using2 = new UsingElement();
            using2.Name = "System";
            groupedInserter.InsertElement(groupElement, using2);
            Assert.AreEqual(1, groupElement.Children.Count, "Group element was not inserted into the parent.");
            Assert.IsTrue(groupElement.Children[0] is GroupElement, "Group element was not inserted into the parent.");
            Assert.AreEqual(2, groupElement.Children[0].Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, groupElement.Children[0].Children.IndexOf(using1), "Element is not at the correct index.");
            Assert.AreEqual(1, groupElement.Children[0].Children.IndexOf(using2), "Element is not at the correct index.");

            UsingElement using3 = new UsingElement();
            using3.Name = "System.Text";
            groupedInserter.InsertElement(groupElement, using3);
            Assert.AreEqual(1, groupElement.Children.Count, "Group element was not inserted into the parent.");
            Assert.IsTrue(groupElement.Children[0] is GroupElement, "Group element was not inserted into the parent.");
            Assert.AreEqual(3, groupElement.Children[0].Children.Count, "Element was not inserted into the parent.");
            Assert.AreEqual(0, groupElement.Children[0].Children.IndexOf(using1), "Element is not at the correct index[0].Children.");
            Assert.AreEqual(1, groupElement.Children[0].Children.IndexOf(using2), "Element is not at the correct index.");
            Assert.AreEqual(2, groupElement.Children[0].Children.IndexOf(using3), "Element is not at the correct index.");
        }

        #endregion Methods
    }
}