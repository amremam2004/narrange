using System;

using NArrange.Core;
using NArrange.Core.CodeElements;

using NUnit.Framework;

namespace NArrange.Tests.Core
{
	/// <summary>
	/// Test fixture for the ChainElementArranger class.
	/// </summary>
	[TestFixture]
	public class ChainElementArrangerTests
	{
		#region Public Methods

		/// <summary>
		/// Tests the AddArranger method with a null arranger
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddArrangerNullTest()
		{
			ChainElementArranger chainArranger = new ChainElementArranger();
			chainArranger.AddArranger(null);
		}

		/// <summary>
		/// Tests the CanArrange method
		/// </summary>
		[Test]
		public void CanArrangeTest()
		{
			ChainElementArranger chain = new ChainElementArranger();
			FieldElement fieldElement = new FieldElement();

			//
			// No arrangers in chain
			//
			Assert.IsFalse(chain.CanArrange(fieldElement), 
			    "Empty chain element arranger should not be able to arrange an element.");

			//
			// Add an arranger that can't arrange the element
			//
			TestElementArranger disabledArranger = new TestElementArranger(false);
			chain.AddArranger(disabledArranger);
			Assert.IsFalse(chain.CanArrange(fieldElement),
			    "Unexpected return value from CanArrange.");

			//
			// Add an arranger that can arrange the element
			//
			TestElementArranger enabledArranger = new TestElementArranger(true);
			chain.AddArranger(enabledArranger);
			Assert.IsTrue(chain.CanArrange(fieldElement),
			    "Unexpected return value from CanArrange.");

			//
			// Null
			//
			Assert.IsFalse(chain.CanArrange(null),
			    "Unexpected return value from CanArrange.");
		}

		/// <summary>
		/// Tests the Arrange method with an element that cannot be handled.
		/// </summary>
		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void UnsupportedArrangeNoParentTest()
		{
			ChainElementArranger chain = new ChainElementArranger();
			FieldElement fieldElement = new FieldElement();

			//
			// Add an arranger that can't arrange the element
			//
			TestElementArranger disabledArranger = new TestElementArranger(false);
			chain.AddArranger(disabledArranger);
			Assert.IsFalse(chain.CanArrange(fieldElement),
			    "Unexpected return value from CanArrange.");

			chain.ArrangeElement(null, fieldElement);
		}

		/// <summary>
		/// Tests the Arrange method with an element that cannot be handled.
		/// </summary>
		[Test]
		public void UnsupportedArrangeWithParentTest()
		{
			GroupElement parentElement = new GroupElement();
			ChainElementArranger chain = new ChainElementArranger();
			FieldElement fieldElement = new FieldElement();

			//
			// Add an arranger that can't arrange the element
			//
			TestElementArranger disabledArranger = new TestElementArranger(false);
			chain.AddArranger(disabledArranger);
			Assert.IsFalse(chain.CanArrange(fieldElement),
			    "Unexpected return value from CanArrange.");

			chain.ArrangeElement(parentElement, fieldElement);
			Assert.IsTrue(parentElement.Children.Contains(fieldElement));
		}

		#endregion Public Methods

		#region Other

		private class TestElementArranger : IElementArranger
		{
			#region Fields

			private bool _arrangeCalled;
			private bool _canArrange;

			#endregion Fields

			#region Constructors

			public TestElementArranger(bool canArrange)
			{
				_canArrange = canArrange;
			}

			#endregion Constructors

			#region Public Properties

			public bool ArrangeCalled
			{
				get
				{
				    return _arrangeCalled;
				}
			}

			#endregion Public Properties

			#region Public Methods

			public void ArrangeElement(ICodeElement parentElement, ICodeElement codeElement)
			{
				_arrangeCalled = true;
			}

			public bool CanArrange(ICodeElement codeElement)
			{
				return _canArrange;
			}

			public bool CanArrange(ICodeElement parentElement, ICodeElement codeElement)
			{
				return _canArrange;
			}

			#endregion Public Methods
		}

		#endregion Other
	}
}