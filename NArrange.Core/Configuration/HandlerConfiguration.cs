namespace NArrange.Core.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    /// <summary>
    /// Base class for handler configuration information.
    /// </summary>
    public abstract class HandlerConfiguration : ICloneable
    {
        #region Fields

        /// <summary>
        /// Assembly name.
        /// </summary>
        private string _assembly;

        #endregion Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets the extension handler assembly.
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

        #region Public Methods

        /// <summary>
        /// Creates a clone of this instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            HandlerConfiguration clone = DoClone();

            clone._assembly = _assembly;

            return clone;
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Creates a clone of this instance.
        /// </summary>
        /// <returns>Clone of the instance.</returns>
        protected abstract HandlerConfiguration DoClone();

        #endregion Protected Methods
    }
}