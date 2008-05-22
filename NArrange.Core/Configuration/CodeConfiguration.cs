#region Header

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 * Copyright (c) 2007-2008 James Nies and NArrange contributors. 	      
 * 	    All rights reserved.                   				      
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
 *		- Added configuration for encoding
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace NArrange.Core.Configuration
{
	/// <summary>
	/// Code arranger configuration information.
	/// </summary>
	public class CodeConfiguration : ConfigurationElement
	{
		#region Static Fields

		private static XmlSerializer _serializer = new XmlSerializer(typeof(CodeConfiguration));
		private static CodeConfiguration _default;
		private static object _defaultLock = new object();

		#endregion Static Fields

		#region Fields

		private ClosingCommentConfiguration _closingComments;
		private EncodingConfiguration _encoding;
		private List<HandlerConfiguration> _handlers;
		private RegionsConfiguration _regions;
		private TabConfiguration _tabs;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new CodeConfiguration.
		/// </summary>
		/// <remarks>Required for XML serialization</remarks>
		public CodeConfiguration()
		{
		}

		#endregion Constructors

		#region Public Properties

		/// <summary>
		/// Closing comment configuration
		/// </summary>
		[Description("Closing comment configuration")]
		public ClosingCommentConfiguration ClosingComments
		{
			get
			{
			    if (_closingComments == null)
			    {
			        lock (this)
			        {
			            if (_closingComments == null)
			            {
			                //
			                // Default closing comment configuration
			                //
			                _closingComments = new ClosingCommentConfiguration();
			            }
			        }
			    }

			    return _closingComments;
			}
			set
			{
			    _closingComments = value;
			}
		}

		/// <summary>
		/// Gets the default configuration
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
			                //
			                // Load the default configuration from the embedded resource file.
			                //
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

		/// <summary>
		/// Encoding configuration.
		/// </summary>
		[Description("Encoding configuration")]
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
			                //
			                // Default encoding configuration
			                //
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
		/// Source code handlers
		/// </summary>
		[XmlArrayItem(typeof(HandlerConfiguration))]
		[Description("Handler configurations")]
		public List<HandlerConfiguration> Handlers
		{
			get
			{
			    if (_handlers == null)
			    {
			        lock (this)
			        {
			            if (_handlers == null)
			            {
			                _handlers = new List<HandlerConfiguration>();
			            }
			        }
			    }

			    return _handlers;
			}
		}

		/// <summary>
		/// Regions configuration.
		/// </summary>
		[Description("Regions configuration")]
		public RegionsConfiguration Regions
		{
			get
			{
			    if (_regions == null)
			    {
			        lock (this)
			        {
			            if (_regions == null)
			            {
			                //
			                // Default regions configuration
			                //
			                _regions = new RegionsConfiguration();
			            }
			        }
			    }

			    return _regions;
			}
			set
			{
			    _regions = value;
			}
		}

		/// <summary>
		/// Tab configuration
		/// </summary>
		[Description("Tab configuration")]
		public TabConfiguration Tabs
		{
			get
			{
			    if (_tabs == null)
			    {
			        lock (this)
			        {
			            if (_tabs == null)
			            {
			                //
			                // Default tab configuration
			                //
			                _tabs = new TabConfiguration();
			            }
			        }
			    }

			    return _tabs;
			}
			set
			{
			    _tabs = value;
			}
		}

		#endregion Public Properties

		#region Private Methods

		/// <summary>
		/// Recurses through the configuration tree and executes actions against 
		/// each configuration element.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="actions"></param>
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

		#endregion Private Methods

		#region Protected Methods

		/// <summary>
		/// Creates a clone of this instance.
		/// </summary>
		/// <returns></returns>
		protected override ConfigurationElement DoClone()
		{
			CodeConfiguration clone = new CodeConfiguration();

			clone._tabs = Tabs.Clone() as TabConfiguration;
			clone._closingComments = ClosingComments.Clone() as ClosingCommentConfiguration;

			foreach (HandlerConfiguration handler in this.Handlers)
			{
			    HandlerConfiguration handlerClone = handler.Clone() as HandlerConfiguration;
			    clone.Handlers.Add(handlerClone);
			}

			return clone;
		}

		#endregion Protected Methods

		#region Public Methods

		/// <summary>
		/// Override Clone so that we can force resolution of element references.
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			CodeConfiguration clone = base.Clone() as CodeConfiguration;
			clone.ResolveReferences();

			return clone;
		}

		/// <summary>
		/// Loads a configuration from file
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static CodeConfiguration Load(string fileName)
		{
			using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
			{
			    return Load(fileStream);
			}
		}

		/// <summary>
		/// Loads a configuration from a stream
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static CodeConfiguration Load(Stream stream)
		{
			CodeConfiguration configuration = 
			    _serializer.Deserialize(stream) as CodeConfiguration;

			configuration.ResolveReferences();

			return configuration;
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

			TreeProcess(this, 
			    new Action<ConfigurationElement>[] 
			    { 
			        populateElementMap,
			        populateElementReferenceList
			    });

			//
			// Resolve element references
			//
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
			            string.Format("Unable to resolve element reference for Id={0}.",
			            reference.Id));
			    }
			}
		}

		/// <summary>
		/// Saves the configuration to a file.
		/// </summary>
		/// <param name="fileName"></param>
		public void Save(string fileName)
		{
			using (FileStream stream = new FileStream(fileName, FileMode.Create))
			{
			    _serializer.Serialize(stream, this);
			}
		}

		#endregion Public Methods
	}
}