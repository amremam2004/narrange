using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace NArrange.Core.Configuration
{
	/// <summary>
	/// Base configuration element class.
	/// </summary>
	public abstract class ConfigurationElement : ICloneable
	{
		#region Fields

		private List<ConfigurationElement> _elements;		
		
		#endregion Fields

		#region Public Properties

		/// <summary>
		/// Elements
		/// </summary>
		[XmlArrayItem(typeof(ElementConfiguration))]
		[XmlArrayItem(typeof(RegionConfiguration))]
		[Description("Element configurations")]
		public List<ConfigurationElement> Elements
		{
			get
			{
			    if (_elements == null)
			    {
			        lock (this)
			        {
			            if (_elements == null)
			            {
			                _elements = new List<ConfigurationElement>();
			            }
			        }
			    }
			
			    return _elements;
			}
		}

		#endregion Public Properties

		#region Protected Methods

		/// <summary>
		/// Creates a new instance and copies state
		/// </summary>
		/// <returns></returns>
		protected ConfigurationElement BaseClone()
		{
			ConfigurationElement clone = DoClone();
			
			foreach (ConfigurationElement child in this.Elements)
			{
			    ConfigurationElement childClone = child.Clone() as ConfigurationElement;
			    clone.Elements.Add(childClone);
			}
			
			return clone;
		}

		/// <summary>
		/// Creates a new instance of this type and copies state
		/// </summary>
		/// <returns></returns>
		protected abstract ConfigurationElement DoClone();

		#endregion Protected Methods

		#region Public Methods

		/// <summary>
		/// Creates a clone of this instance
		/// </summary>
		/// <returns></returns>
		public virtual object Clone()
		{
			ConfigurationElement configurationElement = this.BaseClone();
			
			return configurationElement;
		}

		#endregion Public Methods
	}
}