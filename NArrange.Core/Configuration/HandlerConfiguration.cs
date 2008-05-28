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
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace NArrange.Core.Configuration
{
	/// <summary>
	/// Specifies source code extension handler assembly
	/// </summary>
	[XmlType("SourceHandler")]
	public class HandlerConfiguration : ICloneable
	{
		#region Fields

		private string _assembly;
		private string _language;
		private List<ExtensionConfiguration> _projectExtensions;
		private List<ExtensionConfiguration> _sourceExtensions;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new ExtensionConfiguration instance
		/// </summary>
		public HandlerConfiguration()
		{
		}

		#endregion Constructors

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
		/// Gets or sets the language name.
		/// </summary>
		[XmlAttribute("Language")]
		[Description("The key for the language.")]
		public string Language
		{
			get
			{
			    return _language;
			}
			set
			{
			    _language = value;
			}
		}

		/// <summary>
		/// Extensions
		/// </summary>
		[XmlArrayItem(typeof(ExtensionConfiguration))]
		[Description("The list of project file extensions recognized for the language.")]
		[DisplayName("Project extensions")]
		public List<ExtensionConfiguration> ProjectExtensions
		{
			get
			{
			    if (_projectExtensions == null)
			    {
			        lock (this)
			        {
			            if (_projectExtensions == null)
			            {
			                _projectExtensions = new List<ExtensionConfiguration>();
			            }
			        }
			    }

			    return _projectExtensions;
			}
		}

		/// <summary>
		/// Extensions
		/// </summary>
		[XmlArrayItem(typeof(ExtensionConfiguration))]
		[Description("The list of source code file extensions recognized for the language.")]
		[DisplayName("Source extensions")]
		public List<ExtensionConfiguration> SourceExtensions
		{
			get
			{
			    if (_sourceExtensions == null)
			    {
			        lock (this)
			        {
			            if (_sourceExtensions == null)
			            {
			                _sourceExtensions = new List<ExtensionConfiguration>();
			            }
			        }
			    }

			    return _sourceExtensions;
			}
		}

		#endregion Public Properties

		#region Public Methods

		/// <summary>
		/// Creates a clone of this instance
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			HandlerConfiguration clone = new HandlerConfiguration();

			clone._assembly = _assembly;

			foreach (ExtensionConfiguration extension in this.ProjectExtensions)
			{
			    ExtensionConfiguration extensionClone = extension.Clone() as ExtensionConfiguration;
			    clone.ProjectExtensions.Add(extensionClone);
			}

			foreach (ExtensionConfiguration extension in this.SourceExtensions)
			{
			    ExtensionConfiguration extensionClone = extension.Clone() as ExtensionConfiguration;
			    clone.SourceExtensions.Add(extensionClone);
			}

			return clone;
		}

		/// <summary>
		/// Gets the string representation
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(Thread.CurrentThread.CurrentCulture,
			    "Handler: {0}", this._assembly);
		}

		#endregion Public Methods
	}
}