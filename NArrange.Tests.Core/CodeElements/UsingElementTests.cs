namespace NArrange.Tests.Core.CodeElements
{
    using NArrange.Core.CodeElements;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the UsingElement class.
    /// </summary>
    [TestFixture]
    public class UsingElementTests : CommentedElementTests<UsingElement>
    {
        #region Methods

        /// <summary>
        /// Tests constructing a new UsingElement.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            UsingElement element = new UsingElement();

            //
            // Verify default values
            //
            Assert.AreEqual(string.Empty, element.Name, "Unexpected default value for Name.");
            Assert.IsNull(element.Redefine, "Unexpected default value for Redefine.");

            Assert.IsNotNull(element.Children, "Children collection should not be null.");
            Assert.AreEqual(0, element.Children.Count, "Children collection should be empty.");
            Assert.IsNotNull(element.HeaderComments, "HeaderCommentLines collection should not be null.");
            Assert.AreEqual(0, element.HeaderComments.Count, "HeaderCommentLines collection should be empty.");
            Assert.IsFalse(element.IsMovable, "By default using/import directives should not be movable.");
        }

        /// <summary>
        /// Creates an instance for cloning.
        /// </summary>
        /// <returns>Clone prototype.</returns>
        protected override UsingElement DoCreateClonePrototype()
        {
            UsingElement prototype = new UsingElement();
            prototype.Name = "SampleNamespace";
            prototype.Redefine = "MySampleNamespace";
            prototype.IsMovable = true;

            return prototype;
        }

        /// <summary>
        /// Verifies that a clone has the same state as the original.
        /// </summary>
        /// <param name="original">Original element.</param>
        /// <param name="clone">Clone element.</param>
        protected override void DoVerifyClone(UsingElement original, UsingElement clone)
        {
            Assert.AreEqual(original.Name, clone.Name);
            Assert.AreEqual(original.Redefine, clone.Redefine);
            Assert.AreEqual(original.IsMovable, clone.IsMovable);
        }

        #endregion Methods
    }
}