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
 *      - Allow ElementReferenceConfiguration elements to be deserialized
 *        into the Elements collection.
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

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

		private ConfigurationElementCollection _elements;

		#endregion Fields

		#region Public Properties

		/// <summary>
		/// Elements
		/// </summary>
		[XmlArrayItem(typeof(ElementConfiguration))]
		[XmlArrayItem(typeof(RegionConfiguration))]
		[XmlArrayItem(typeof(ElementReferenceConfiguration))]
		[Description("The list of child element configurations.")]
		public virtual ConfigurationElementCollection Elements
		{
			get
			{
			    if (_elements == null)
			    {
			        lock (this)
			        {
			            if (_elements == null)
			            {
							_elements = new ConfigurationElementCollection();
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