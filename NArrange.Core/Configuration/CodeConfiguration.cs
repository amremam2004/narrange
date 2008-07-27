#region Header

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 * Copyright (c) 2007-2008 James Nies and NArrange contributors.
 *    All rights reserved.
 *
 * This program and the accompanying materials are made available under
 * the terms of the Common Public License v1.0 which accompanies this
 * distribution.
 *
 * Redistribution and use in source and binary forms, with or
 * without modification, are permitted provided that the following
 * conditions are met:
 *
 * Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright
 * notice, this list of conditions and the following disclaimer in
 * the documentation and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
 * TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
 * OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY
 * OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * Contributors:
 *      James Nies
 *      - Initial creation
 *      - Added configuration for closing comments
 *      - Added configuration for region options
 *      - Added a ResolveReferences method that gets called whenever a
 *        configuration is loaded or cloned.  This resolves references
 *        in ElementReferenceConfiguration config elements by locating
 *        the referenced element and attaching a clone to the reference.
 *      - Added configuration for encoding
 *      - Allow the configuration to be loaded without resolving
 *        references (needed for configuration editor)
 *      - Upgrade configurations to the new project extension format when
 *        loading.
 *      - Moved formatting configurations to a new config element and
 *        upgrade when loading.
 *      Justin Dearing
 *      - Code cleanup via ReSharper 4.0 (http://www.jetbrains.com/resharper/)
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

namespace NArrange.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// Code arranger configuration information.
    /// </summary>
    public class CodeConfiguration : ConfigurationElement
    {
        #region Static Fields

        /// <summary>
        /// Seriarializer for serializing and deserializing the configuration 
        /// to and from XML.
        /// </summary>
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(CodeConfiguration));

        /// <summary>
        /// Synchronization lock for the default configuration instance.
        /// </summary>
        private static readonly object _defaultLock = new object();

        /// <summary>
        /// The default configuration instance.
        /// </summary>
        private static CodeConfiguration _default;

        #endregion Static Fields

        #region Fields

        /// <summary>
        /// Encoding configuration.
        /// </summary>
        private EncodingConfiguration _encoding;

        /// <summary>
        /// Formatting configuration.
        /// </summary>
        private FormattingConfiguration _formatting;

        /// <summary>
        /// Collection of source/project handler configurations.
        /// </summary>
        private HandlerConfigurationCollection _handlers;

        #endregion Fields

        #region Public Static Properties

        /// <summary>
        /// Gets the default configuration.
        /// </summary>
        public static CodeConfiguration Default
        {
            get
            {
                if (_default == null)
                {
                    lock (_defaultLock)
                    {
                        if (_default == null)
                        {
                            // Load the default configuration from the embedded resource file.
                            using (Stream resourceStream =
                                typeof(CodeConfiguration).Assembly.GetManifestResourceStream(
                                typeof(CodeConfiguration).Assembly.GetName().Name + ".DefaultConfig.xml"))
                            {
                                _default = Load(resourceStream);
                            }
                        }
                    }
                }

                return _default;
            }
        }

        #endregion Public Static Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the closing comment configuration.
        /// </summary>
        [Description("The settings for closing comments (Obsolete - Use Formatting.ClosingComments instead).")]
        [DisplayName("Closing comments (Obsolete)")]
        [ReadOnly(true)]
        [Browsable(false)]
        public ClosingCommentConfiguration ClosingComments
        {
            get
            {
                return null;
            }
            set
            {
                Formatting.ClosingComments = value;
            }
        }

        /// <summary>
        /// Gets or sets the encoding configuration.
        /// </summary>
        [Description("The encoding settings used for reading and writing source code files.")]
        [ReadOnly(true)]
        public EncodingConfiguration Encoding
        {
            get
            {
                if (_encoding == null)
                {
                    lock (this)
                    {
                        if (_encoding == null)
                        {
                            // Default encoding configuration
                            _encoding = new EncodingConfiguration();
                        }
                    }
                }

                return _encoding;
            }
            set
            {
                _encoding = value;
            }
        }

        /// <summary>
        /// Gets or sets the formatting configuration.
        /// </summary>
        [Description("Formatting settings.")]
        [DisplayName("Formatting")]
        [ReadOnly(true)]
        public FormattingConfiguration Formatting
        {
            get
            {
                if (_formatting == null)
                {
                    lock (this)
                    {
                        if (_formatting == null)
                        {
                            // Default style configuration
                            _formatting = new FormattingConfiguration();
                        }
                    }
                }

                return _formatting;
            }
            set
            {
                _formatting = value;
            }
        }

        /// <summary>
        /// Gets the collection of source code/project handlers.
        /// </summary>
        [XmlArrayItem(typeof(SourceHandlerConfiguration))]
        [XmlArrayItem(typeof(ProjectHandlerConfiguration))]
        [Description("The list of project/language handlers and their settings.")]
        public HandlerConfigurationCollection Handlers
        {
            get
            {
                if (_handlers == null)
                {
                    lock (this)
                    {
                        if (_handlers == null)
                        {
                            _handlers = new HandlerConfigurationCollection();
                        }
                    }
                }

                return _handlers;
            }
        }

        /// <summary>
        /// Gets or sets the regions configuration.
        /// </summary>
        [Description("The settings for all regions (Obsolete - Use Formatting.Regions instead).")]
        [ReadOnly(true)]
        [Browsable(false)]
        public RegionFormatConfiguration Regions
        {
            get
            {
                return null;
            }
            set
            {
                Formatting.Regions = value;
            }
        }

        /// <summary>
        /// Gets or sets the tab configuration.
        /// </summary>
        [Description("The settings for indentation (Obsolete - Use Formatting.Tabs instead).")]
        [ReadOnly(true)]
        [Browsable(false)]
        public TabConfiguration Tabs
        {
            get
            {
                return null;
            }
            set
            {
                Formatting.Tabs = value;
            }
        }

        #endregion Public Properties

        #region Public Static Methods

        /// <summary>
        /// Loads a configuration from the specified file.
        /// </summary>
        /// <param name="fileName">Configuration file name.</param>
        /// <returns>The loaded code configuration if succesful, otherwise null.</returns>
        public static CodeConfiguration Load(string fileName)
        {
            return Load(fileName, true);
        }

        /// <summary>
        /// Loads a configuration from the specified file.
        /// </summary>
        /// <param name="fileName">Configuration file name.</param>
        /// <param name="resolveReferences">Resolve element references.</param>
        /// <returns>The code configuration if succesful, otherwise null.</returns>
        public static CodeConfiguration Load(string fileName, bool resolveReferences)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
            {
                return Load(fileStream, resolveReferences);
            }
        }

        /// <summary>
        /// Loads a configuration from a stream.
        /// </summary>
        /// <param name="stream">The stream to load the configuration from.</param>
        /// <returns>The code configuration if succesful, otherwise null.</returns>
        public static CodeConfiguration Load(Stream stream)
        {
            return Load(stream, true);
        }

        /// <summary>
        /// Loads a configuration from a stream.
        /// </summary>
        /// <param name="stream">The sream to load the configuration from.</param>
        /// <param name="resolveReferences">
        /// Whether or not element references should be resolved.
        /// </param>
        /// <returns>The code configuration if succesful, otherwise null.</returns>
        public static CodeConfiguration Load(Stream stream, bool resolveReferences)
        {
            CodeConfiguration configuration =
                _serializer.Deserialize(stream) as CodeConfiguration;

            if (resolveReferences)
            {
                configuration.ResolveReferences();
            }

            configuration.Upgrade();

            return configuration;
        }

        #endregion Public Static Methods

        #region Public Methods

        /// <summary>
        /// Override Clone so that we can force resolution of element references.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            CodeConfiguration clone = base.Clone() as CodeConfiguration;
            clone.ResolveReferences();

            return clone;
        }

        /// <summary>
        /// Resolves any reference elements in the configuration.
        /// </summary>
        public void ResolveReferences()
        {
            Dictionary<string, ElementConfiguration> elementMap = new Dictionary<string, ElementConfiguration>();
            List<ElementReferenceConfiguration> elementReferences = new List<ElementReferenceConfiguration>();

            Action<ConfigurationElement> populateElementMap = delegate(ConfigurationElement element)
            {
                ElementConfiguration elementConfiguration = element as ElementConfiguration;
                if (elementConfiguration != null && elementConfiguration.Id != null)
                {
                    elementMap.Add(elementConfiguration.Id, elementConfiguration);
                }
            };

            Action<ConfigurationElement> populateElementReferenceList = delegate(ConfigurationElement element)
            {
                ElementReferenceConfiguration elementReference = element as ElementReferenceConfiguration;
                if (elementReference != null && elementReference.Id != null)
                {
                    elementReferences.Add(elementReference);
                }
            };

            Action<ConfigurationElement>[] actions = new Action<ConfigurationElement>[]
                {
                    populateElementMap,
                    populateElementReferenceList
                };

            TreeProcess(this, actions);

            // Resolve element references
            foreach (ElementReferenceConfiguration reference in elementReferences)
            {
                ElementConfiguration referencedElement = null;
                elementMap.TryGetValue(reference.Id, out referencedElement);
                if (referencedElement != null)
                {
                    reference.ReferencedElement = referencedElement;
                }
                else
                {
                    throw new InvalidOperationException(
                        string.Format(
                        "Unable to resolve element reference for Id={0}.",
                        reference.Id));
                }
            }
        }

        /// <summary>
        /// Saves the configuration to a file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void Save(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Create))
            {
                _serializer.Serialize(stream, this);
            }
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Creates a clone of this instance.
        /// </summary>
        /// <returns>A clone of the instance.</returns>
        protected override ConfigurationElement DoClone()
        {
            CodeConfiguration clone = new CodeConfiguration();

            clone._encoding = Encoding.Clone() as EncodingConfiguration;
            clone._formatting = Formatting.Clone() as FormattingConfiguration;

            foreach (HandlerConfiguration handler in Handlers)
            {
                HandlerConfiguration handlerClone = handler.Clone() as HandlerConfiguration;
                clone.Handlers.Add(handlerClone);
            }

            return clone;
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Recurses through the configuration tree and executes actions against 
        /// each configuration element.
        /// </summary>
        /// <param name="element">Element to process.</param>
        /// <param name="actions">Actions to perform.</param>
        private void TreeProcess(ConfigurationElement element, Action<ConfigurationElement>[] actions)
        {
            if (element != null)
            {
                foreach (ConfigurationElement childElement in element.Elements)
                {
                    foreach (Action<ConfigurationElement> action in actions)
                    {
                        action(childElement);
                    }

                    TreeProcess(childElement, actions);
                }
            }
        }

        /// <summary>
        /// Upgrades the configuration.
        /// </summary>
        private void Upgrade()
        {
            UpgradeProjectExtensions();
        }

        /// <summary>
        /// Moves project extensions to the new format.
        /// </summary>
        private void UpgradeProjectExtensions()
        {
            // Migrate project handler configurations
            string parserType = typeof(MSBuildProjectParser).FullName;
            ProjectHandlerConfiguration projectHandlerConfiguration = null;
            foreach (HandlerConfiguration handlerConfiguration in Handlers)
            {
                if (handlerConfiguration.HandlerType == HandlerType.Project)
                {
                    ProjectHandlerConfiguration candidateConfiguration = handlerConfiguration as ProjectHandlerConfiguration;
                    if (candidateConfiguration.ParserType != null &&
                        candidateConfiguration.ParserType.ToUpperInvariant() == parserType.ToUpperInvariant())
                    {
                        projectHandlerConfiguration = candidateConfiguration;
                        break;
                    }
                }
            }

            //
            // Create the new project configuration if necessary
            //
            if (projectHandlerConfiguration == null)
            {
                projectHandlerConfiguration = new ProjectHandlerConfiguration();
                projectHandlerConfiguration.ParserType = parserType;
                Handlers.Insert(0, projectHandlerConfiguration);
            }

            foreach (HandlerConfiguration handlerConfiguration in Handlers)
            {
                if (handlerConfiguration.HandlerType == HandlerType.Source)
                {
                    SourceHandlerConfiguration sourceHandlerConfiguration = handlerConfiguration as SourceHandlerConfiguration;
                    foreach (ExtensionConfiguration projectExtension in sourceHandlerConfiguration.ProjectExtensions)
                    {
                        bool upgraded = false;
                        foreach (ExtensionConfiguration upgradedExtension in projectHandlerConfiguration.ProjectExtensions)
                        {
                            if (string.Compare(upgradedExtension.Name, projectExtension.Name, true) == 0)
                            {
                                upgraded = true;
                                break;
                            }
                        }

                        if (!upgraded)
                        {
                            projectHandlerConfiguration.ProjectExtensions.Add(projectExtension);
                        }
                    }

                    sourceHandlerConfiguration.ProjectExtensions.Clear();
                }
            }
        }

        #endregion Private Methods
    }
}