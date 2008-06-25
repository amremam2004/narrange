using System.IO;
using System.Reflection;

namespace NArrange.Tests.Core
{
	/// <summary>
	/// Interface for a source code test file.
	/// </summary>
	public interface ISourceCodeTestFile
	{
		#region Properties

		/// <summary>
		/// Gets the assembly for the test file.
		/// </summary>
		Assembly Assembly
		{
			get;
		}

		/// <summary>
		/// Gets the name of the test file.
		/// </summary>
		string Name
		{
			get;
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Gets a TextReader for this test file.
		/// </summary>
		/// <returns></returns>
		TextReader GetReader();

		#endregion Methods
	}
}