using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using NUnit.Framework;

using NArrange.Core;
using NArrange.Core.CodeElements;
using NArrange.Core.Configuration;

namespace NArrange.Tests.Core
{
	/// <summary>
	/// Test fixture for the SortedInserter class.
	/// </summary>
	[TestFixture]
	public class SortedInserterTests
	{
		#region Public Methods

		/// <summary>
		/// Tests the creation of a SortedInserter instance.
		/// </summary>
		[Test]
		public void CreateTest()
		{
			SortBy sortBy = new SortBy();

			SortedInserter sortedInserter = new SortedInserter(ElementType.NotSpecified, sortBy);
		}

		/// <summary>
		/// Test construction with a null configuration
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CreateWithNullTest()
		{
			SortedInserter sortedInserter = new SortedInserter(ElementType.NotSpecified, null);
		}

		/// <summary>
		/// Tests inserting elements by access and name.
		/// </summary>
		[Test]
		public void InsertByAccessAndNameTest()
		{
			SortBy sortBy = new SortBy();
			sortBy.By = ElementAttribute.Access;
			sortBy.Direction = ListSortDirection.Ascending;

			SortBy innerSortBy = new SortBy();
			innerSortBy.By = ElementAttribute.Name;
			innerSortBy.Direction = ListSortDirection.Ascending;

			sortBy.InnerSortBy = innerSortBy;

			SortedInserter sortedInserter = new SortedInserter(ElementType.Field, sortBy);

			//
			// Create a parent element
			//
			RegionElement regionElement = new RegionElement();
			Assert.AreEqual(0, regionElement.Children.Count,
			    "Parent element should not have any children.");

			//
			// Insert elements with middle access.
			//
			FieldElement field1 = new FieldElement();
			field1.Access = CodeAccess.Protected | CodeAccess.Internal;
			field1.Name = "newField";
			sortedInserter.InsertElement(regionElement, field1);
			Assert.AreEqual(1, regionElement.Children.Count,
			    "Element was not inserted into the parent.");
			Assert.AreEqual(0, regionElement.Children.IndexOf(field1),
			    "Element was not inserted at the correct index.");

			FieldElement field2 = new FieldElement();
			field2.Access = CodeAccess.Protected | CodeAccess.Internal;
			field2.Name = "gooField";
			sortedInserter.InsertElement(regionElement, field2);
			Assert.AreEqual(2, regionElement.Children.Count,
			    "Element was not inserted into the parent.");
			Assert.AreEqual(0, regionElement.Children.IndexOf(field2),
			    "Element was not inserted at the correct index.");
			Assert.AreEqual(1, regionElement.Children.IndexOf(field1),
			    "Element was not inserted at the correct index.");

			//
			// Insert an element that should be sorted toward the end
			//
			FieldElement field3 = new FieldElement();
			field3.Access = CodeAccess.Public;
			field3.Name = "zooField";
			sortedInserter.InsertElement(regionElement, field3);
			Assert.AreEqual(3, regionElement.Children.Count,
			    "Element was not inserted into the parent.");
			Assert.AreEqual(0, regionElement.Children.IndexOf(field2),
			    "Element was not inserted at the correct index.");
			Assert.AreEqual(1, regionElement.Children.IndexOf(field1),
			    "Element was not inserted at the correct index.");
			Assert.AreEqual(2, regionElement.Children.IndexOf(field3),
			    "Element is not at the correct index.");

			FieldElement field4 = new FieldElement();
			field4.Access = CodeAccess.Public;
			field4.Name = "tooField";
			sortedInserter.InsertElement(regionElement, field4);
			Assert.AreEqual(4, regionElement.Children.Count,
			    "Element was not inserted into the parent.");
			Assert.AreEqual(0, regionElement.Children.IndexOf(field2),
			    "Element was not inserted at the correct index.");
			Assert.AreEqual(1, regionElement.Children.IndexOf(field1),
			    "Element was not inserted at the correct index.");
			Assert.AreEqual(2, regionElement.Children.IndexOf(field4),
			    "Element is not at the correct index.");
			Assert.AreEqual(3, regionElement.Children.IndexOf(field3),
			    "Element is not at the correct index.");

			//
			// Insert an element that should be sorted toward the beginning
			//
			FieldElement field5 = new FieldElement();
			field5.Access = CodeAccess.Private;
			field5.Name = "booField";
			sortedInserter.InsertElement(regionElement, field5);
			Assert.AreEqual(5, regionElement.Children.Count,
			    "Element was not inserted into the parent.");
			Assert.AreEqual(0, regionElement.Children.IndexOf(field5),
			    "Element was not inserted at the correct index.");
			Assert.AreEqual(1, regionElement.Children.IndexOf(field2),
			    "Element was not inserted at the correct index.");
			Assert.AreEqual(2, regionElement.Children.IndexOf(field1),
			    "Element was not inserted at the correct index.");
			Assert.AreEqual(3, regionElement.Children.IndexOf(field4),
			    "Element is not at the correct index.");
			Assert.AreEqual(4, regionElement.Children.IndexOf(field3),
			    "Element is not at the correct index.");

			FieldElement field6 = new FieldElement();
			field6.Access = CodeAccess.Private;
			field6.Name = "fooField";
			sortedInserter.InsertElement(regionElement, field6);
			Assert.AreEqual(6, regionElement.Children.Count,
			    "Element was not inserted into the parent.");
			Assert.AreEqual(0, regionElement.Children.IndexOf(field5),
			    "Element was not inserted at the correct index.");
			Assert.AreEqual(1, regionElement.Children.IndexOf(field6),
			    "Element was not inserted at the correct index.");
			Assert.AreEqual(2, regionElement.Children.IndexOf(field2),
			    "Element was not inserted at the correct index.");
			Assert.AreEqual(3, regionElement.Children.IndexOf(field1),
			    "Element was not inserted at the correct index.");
			Assert.AreEqual(4, regionElement.Children.IndexOf(field4),
			    "Element is not at the correct index.");
			Assert.AreEqual(5, regionElement.Children.IndexOf(field3),
			    "Element is not at the correct index.");
		}

		/// <summary>
		/// Tests inserting elements by access.
		/// </summary>
		[Test]
		public void InsertByAccessTest()
		{
			SortBy sortBy = new SortBy();
			sortBy.By = ElementAttribute.Access;
			sortBy.Direction = ListSortDirection.Ascending;

			SortedInserter sortedInserter = new SortedInserter(ElementType.Field, sortBy);

			//
			// Create a parent element
			//
			RegionElement regionElement = new RegionElement();
			Assert.AreEqual(0, regionElement.Children.Count,
			    "Parent element should not have any children.");

			//
			// Insert an element with a middle access.
			//
			FieldElement field1 = new FieldElement();
			field1.Access = CodeAccess.Protected | CodeAccess.Internal;
			sortedInserter.InsertElement(regionElement, field1);
			Assert.AreEqual(1, regionElement.Children.Count,
			    "Element was not inserted into the parent.");
			Assert.AreEqual(0, regionElement.Children.IndexOf(field1),
			    "Element was not inserted at the correct index.");

			//
			// Insert an element that should be sorted toward the end
			//
			FieldElement field2 = new FieldElement();
			field2.Access = CodeAccess.Public;
			sortedInserter.InsertElement(regionElement, field2);
			Assert.AreEqual(2, regionElement.Children.Count,
			    "Element was not inserted into the parent.");
			Assert.AreEqual(0, regionElement.Children.IndexOf(field1),
			    "Element is not at the correct index.");
			Assert.AreEqual(1, regionElement.Children.IndexOf(field2),
			    "Element is not at the correct index.");

			//
			// Insert an element that should be sorted toward the beginning
			//
			FieldElement field3 = new FieldElement();
			field3.Access = CodeAccess.Private;
			sortedInserter.InsertElement(regionElement, field3);
			Assert.AreEqual(3, regionElement.Children.Count,
			    "Element was not inserted into the parent.");
			Assert.AreEqual(0, regionElement.Children.IndexOf(field3),
			   "Element is not at the correct index.");
			Assert.AreEqual(1, regionElement.Children.IndexOf(field1),
			    "Element is not at the correct index.");
			Assert.AreEqual(2, regionElement.Children.IndexOf(field2),
			  "Element is not at the correct index.");
		}

		/// <summary>
		/// Tests inserting elements by name in descending order.
		/// </summary>
		[Test]
		public void InsertByNameDescendingTest()
		{
			SortBy sortBy = new SortBy();
			sortBy.By = ElementAttribute.Name;
			sortBy.Direction = ListSortDirection.Descending;

			SortedInserter sortedInserter = new SortedInserter(ElementType.Field, sortBy);

			//
			// Create a parent element
			//
			RegionElement regionElement = new RegionElement();
			Assert.AreEqual(0, regionElement.Children.Count,
			    "Parent element should not have any children.");

			//
			// Insert an element with a mid alphabet name.
			//
			FieldElement field1 = new FieldElement();
			field1.Name = "newField";
			sortedInserter.InsertElement(regionElement, field1);
			Assert.AreEqual(1, regionElement.Children.Count,
			    "Element was not inserted into the parent.");
			Assert.AreEqual(0, regionElement.Children.IndexOf(field1),
			    "Element was not inserted at the correct index.");

			//
			// Insert an element that should be sorted toward the beginning
			//
			FieldElement field2 = new FieldElement();
			field2.Name = "zooField";
			sortedInserter.InsertElement(regionElement, field2);
			Assert.AreEqual(2, regionElement.Children.Count,
			    "Element was not inserted into the parent.");
			Assert.AreEqual(0, regionElement.Children.IndexOf(field2),
			    "Element is not at the correct index.");
			Assert.AreEqual(1, regionElement.Children.IndexOf(field1),
			    "Element is not at the correct index.");

			//
			// Insert an element that should be sorted toward the end
			//
			FieldElement field3 = new FieldElement();
			field3.Name = "booField";
			sortedInserter.InsertElement(regionElement, field3);
			Assert.AreEqual(3, regionElement.Children.Count,
			    "Element was not inserted into the parent.");
			Assert.AreEqual(0, regionElement.Children.IndexOf(field2),
			   "Element is not at the correct index.");
			Assert.AreEqual(1, regionElement.Children.IndexOf(field1),
			    "Element is not at the correct index.");
			Assert.AreEqual(2, regionElement.Children.IndexOf(field3),
			  "Element is not at the correct index.");
		}

		/// <summary>
		/// Tests inserting elements by name.
		/// </summary>
		[Test]
		public void InsertByNameTest()
		{
			SortBy sortBy = new SortBy();
			sortBy.By = ElementAttribute.Name;
			sortBy.Direction = ListSortDirection.Ascending;

			SortedInserter sortedInserter = new SortedInserter(ElementType.Field, sortBy);

			//
			// Create a parent element
			//
			RegionElement regionElement = new RegionElement();
			Assert.AreEqual(0, regionElement.Children.Count,
			    "Parent element should not have any children.");

			//
			// Insert an element with a mid alphabet name.
			//
			FieldElement field1 = new FieldElement();
			field1.Name = "newField";
			sortedInserter.InsertElement(regionElement, field1);
			Assert.AreEqual(1, regionElement.Children.Count,
			    "Element was not inserted into the parent.");
			Assert.AreEqual(0, regionElement.Children.IndexOf(field1),
			    "Element was not inserted at the correct index.");

			//
			// Insert an element that should be sorted toward the end
			//
			FieldElement field2 = new FieldElement();
			field2.Name = "zooField";
			sortedInserter.InsertElement(regionElement, field2);
			Assert.AreEqual(2, regionElement.Children.Count,
			    "Element was not inserted into the parent.");
			Assert.AreEqual(0, regionElement.Children.IndexOf(field1),
			    "Element is not at the correct index.");
			Assert.AreEqual(1, regionElement.Children.IndexOf(field2),
			    "Element is not at the correct index.");

			//
			// Insert an element that should be sorted toward the beginning
			//
			FieldElement field3 = new FieldElement();
			field3.Name = "booField";
			sortedInserter.InsertElement(regionElement, field3);
			Assert.AreEqual(3, regionElement.Children.Count,
			    "Element was not inserted into the parent.");
			Assert.AreEqual(0, regionElement.Children.IndexOf(field3),
			   "Element is not at the correct index.");
			Assert.AreEqual(1, regionElement.Children.IndexOf(field1),
			    "Element is not at the correct index.");
			Assert.AreEqual(2, regionElement.Children.IndexOf(field2),
			  "Element is not at the correct index.");
		}

		/// <summary>
		/// Tests inserting elements without any criteria.
		/// </summary>
		[Test]
		public void InsertByNoneTest()
		{
			SortBy sortBy = new SortBy();
			sortBy.By = ElementAttribute.None;
			sortBy.Direction = ListSortDirection.Ascending;

			SortedInserter sortedInserter = new SortedInserter(ElementType.Field, sortBy);

			//
			// Create a parent element
			//
			RegionElement regionElement = new RegionElement();
			Assert.AreEqual(0, regionElement.Children.Count,
			    "Parent element should not have any children.");

			//
			// With no criteria specified, elements should just be inserted 
			// at the end of the collection.
			//

			FieldElement field1 = new FieldElement();
			field1.Name = "zooField";
			sortedInserter.InsertElement(regionElement, field1);
			Assert.AreEqual(1, regionElement.Children.Count,
			    "Element was not inserted into the parent.");
			Assert.AreEqual(0, regionElement.Children.IndexOf(field1),
			    "Element was not inserted at the correct index.");

			FieldElement field2 = new FieldElement();
			field1.Name = "newField";
			sortedInserter.InsertElement(regionElement, field2);
			Assert.AreEqual(2, regionElement.Children.Count,
			    "Element was not inserted into the parent.");
			Assert.AreEqual(0, regionElement.Children.IndexOf(field1),
			    "Element is not at the correct index.");
			Assert.AreEqual(1, regionElement.Children.IndexOf(field2),
			    "Element is not at the correct index.");

			FieldElement field3 = new FieldElement();
			field1.Name = "booField";
			sortedInserter.InsertElement(regionElement, field3);
			Assert.AreEqual(3, regionElement.Children.Count,
			    "Element was not inserted into the parent.");
			Assert.AreEqual(0, regionElement.Children.IndexOf(field1),
			    "Element is not at the correct index.");
			Assert.AreEqual(1, regionElement.Children.IndexOf(field2),
			    "Element is not at the correct index.");
			Assert.AreEqual(2, regionElement.Children.IndexOf(field3),
			    "Element is not at the correct index.");
		}

		/// <summary>
		/// Tests inserting a null element.
		/// </summary>
		[Test]
		public void InsertNullTest()
		{
			SortBy sortBy = new SortBy();
			sortBy.By = ElementAttribute.Name;
			sortBy.Direction = ListSortDirection.Ascending;

			SortedInserter sortedInserter = new SortedInserter(ElementType.Field, sortBy);

			//
			// Create a parent element
			//
			RegionElement regionElement = new RegionElement();
			Assert.AreEqual(0, regionElement.Children.Count,
			    "Parent element should not have any children.");

			//
			// Insert a non-null element
			//
			FieldElement field1 = new FieldElement();
			field1.Name = "newField";
			sortedInserter.InsertElement(regionElement, field1);
			Assert.AreEqual(1, regionElement.Children.Count,
			    "Element was not inserted into the parent.");
			Assert.AreEqual(0, regionElement.Children.IndexOf(field1),
			    "Element was not inserted at the correct index.");

			//
			// Insert a null element
			//
			FieldElement field2 = null;
			sortedInserter.InsertElement(regionElement, field2);
			Assert.AreEqual(2, regionElement.Children.Count,
			    "Element was not inserted into the parent.");
			Assert.AreEqual(0, regionElement.Children.IndexOf(field2),
			    "Element is not at the correct index.");
			Assert.AreEqual(1, regionElement.Children.IndexOf(field1),
			    "Element is not at the correct index.");


			//
			// Inser the null element first
			//
			regionElement.ClearChildren();
			sortedInserter.InsertElement(regionElement, field2);
			sortedInserter.InsertElement(regionElement, field1);
			Assert.AreEqual(2, regionElement.Children.Count,
			    "Element was not inserted into the parent.");
			Assert.AreEqual(0, regionElement.Children.IndexOf(field2),
			    "Element is not at the correct index.");
			Assert.AreEqual(1, regionElement.Children.IndexOf(field1),
			    "Element is not at the correct index.");
		}

		#endregion Public Methods
	}
}