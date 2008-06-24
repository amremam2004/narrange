using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace NArrange.Core.Configuration
{
	/// <summary>
	/// Base class for handler configuration information.
	/// </summary>
	public abstract class HandlerConfiguration : ICloneable
	{
		#region Fields

		private string _assembly;

		#endregion Fields

		#region Public Properties

		/// <summary>
		/// Gets or sets the extension handler assembly
		/// </summary>
		[XmlAttribute("Assembly")]
		[Description("The full assembly name used for assembly loading.")]
		[DisplayName("Assembly name")]
		public string AssemblyName
		{
			get
			{
				return _assembly;
			}
			set
			{
				_assembly = value;
			}
		}

		/// <summary>
		/// Gets the handler type.
		/// </summary>
		public abstract HandlerType HandlerType
		{
			get;
		}

		#endregion Public Properties

		#region Protected Methods

		/// <summary>
		/// Creates a clone of this instance.
		/// </summary>
		/// <returns></returns>
		protected abstract HandlerConfiguration DoClone();

		#endregion Protected Methods

		#region Public Methods

		/// <summary>
		/// Creates a clone of this instance.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			HandlerConfiguration clone = DoClone();

			clone._assembly = _assembly;

			return clone;
		}

		#endregion Public Methods
	}
}