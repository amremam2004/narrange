using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NArrange.Core;
using NArrange.Core.CodeElements;

namespace NArrange.Tests.Core.CodeElements
{
	/// <summary>
	/// Test fixture base for the CodeElement class
	/// </summary>
	public abstract class CodeElementTests<TCodeElement>
		where TCodeElement : CodeElement, new()
	{
		#region Protected Methods

		/// <summary>
		/// Creates an instance to be cloned
		/// </summary>
		/// <returns></returns>
		protected abstract TCodeElement DoCreateClonePrototype();
		/// <summary>
		/// Performs the ToString test
		/// </summary>
		protected virtual void DoToStringTest()
		{
			TCodeElement codeElement = new TCodeElement();
			codeElement.Name = "Element";
			string str = codeElement.ToString();
			Assert.AreEqual("Element", str,
			    "Unexpected string representation.");
		}


		/// <summary>
		/// Verifies that a clone has the same state as the original
		/// </summary>
		/// <param name="original"></param>
		/// <param name="clone"></param>
		protected abstract void DoVerifyClone(TCodeElement original, TCodeElement clone);

		#endregion Protected Methods

		#region Public Methods

		/// <summary>
		/// Tests the clone method
		/// </summary>
		[Test]
		public void CloneTest()
		{
            const string Key1 = "Test1";
            const string Key2 = "Test2";

			TCodeElement original = DoCreateClonePrototype();
            original[Key1] = "SomeValue";
            original[Key2] = false;

			TCodeElement clone = original.Clone() as TCodeElement;
			Assert.IsNotNull(clone, "Clone did not create an instance of type {0}.",
			    typeof(TCodeElement).Name);
			Assert.AreNotSame(original, clone, "Clone should be a different instance.");

            Assert.AreEqual(original[Key1], clone[Key1], 
                "Extended properties were not cloned correctly.");
            Assert.AreEqual(original[Key2], clone[Key2],
                "Extended properties were not cloned correctly.");

			DoVerifyClone(original, clone);
		}

		/// <summary>
		/// Tests getting and setting the parent property
		/// </summary>
		[Test]
		public virtual void ParentTest()
		{
			TCodeElement parentElement = new TCodeElement();
			TCodeElement childElement = new TCodeElement();
			Assert.IsNull(childElement.Parent, "Parent should not be set.");
			
			childElement.Parent = parentElement;
			Assert.AreSame(parentElement, childElement.Parent,
			    "Parent was not set correctly.");
			
			Assert.IsTrue(parentElement.Children.Contains(childElement),
			    "Parent Children collection does not contain the child element.");
			
			childElement.Parent = null;
			Assert.IsNull(childElement.Parent, "Parent should not be set.");
			
			Assert.IsFalse(parentElement.Children.Contains(childElement),
			    "Parent Children collection should not contain the child element.");
			
			parentElement.AddChild(childElement);
			Assert.AreSame(parentElement, childElement.Parent,
			     "Parent was not set correctly.");
			
			parentElement.RemoveChild(childElement);
			Assert.IsNull(childElement.Parent, "Parent should not be set.");
		}

		/// <summary>
		/// Tests the ToString method
		/// </summary>
		[Test]
		public virtual void ToStringTest()
		{
			DoToStringTest();
		}

		#endregion Public Methods
	}
}