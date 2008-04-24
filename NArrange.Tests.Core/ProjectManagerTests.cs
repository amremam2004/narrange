using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NArrange.Core;

namespace NArrange.Tests.Core
{
	/// <summary>
	/// Test fixture for the ProjectManager class.
	/// </summary>
	[TestFixture]
	public class ProjectManagerTests
	{
		#region Public Methods

		/// <summary>
		/// Tests creating a new ProjectManager with a null configuration.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullConfigurationTest()
		{
			ProjectManager projectManager = new ProjectManager(null);
		}

		#endregion Public Methods
	}
}