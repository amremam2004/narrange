using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using NUnit.Framework;

using NArrange.VisualBasic;

namespace NArrange.Tests.VisualBasic
{
	/// <summary>
	/// Test fixture for the VBSymbol class
	/// </summary>
	[TestFixture]
	public class VBSymbolTests
	{
		#region Public Methods

		/// <summary>
		/// Tests the IsVBSymbol method
		/// </summary>
		[Test]
		public void IsVBSymbolTest()
		{
			// 
			// Using reflection, verify that all symbol constants are determined
			// to be symbols.
			//
			FieldInfo[] symbolFields = typeof(VBSymbol).GetFields(
			    BindingFlags.Static | BindingFlags.Public);
			foreach (FieldInfo symbolField in symbolFields)
			{
			    if (symbolField.FieldType == typeof(char))
			    {
			        char fieldValue = (char)symbolField.GetValue(null);
			        Assert.IsTrue(VBSymbol.IsVBSymbol(fieldValue),
			            "Field value should be considered a VB symbol.");
			    }
			}
			
			Assert.IsFalse(VBSymbol.IsVBSymbol('1'));
			Assert.IsFalse(VBSymbol.IsVBSymbol('A'));
			Assert.IsFalse(VBSymbol.IsVBSymbol('$'));
			Assert.IsFalse(VBSymbol.IsVBSymbol('_'));
		}

		#endregion Public Methods
	}
}