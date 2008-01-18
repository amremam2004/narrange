//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~                                                                        
// Copyright (c) 2007-2008 James Nies and NArrange contributors. 	      
// 	    All rights reserved.                   				      
//                                                                             
// This program and the accompanying materials are made available under       
// the terms of the Common Public License v1.0 which accompanies this         
// distribution.							      
//                                                                             
// Redistribution and use in source and binary forms, with or                 
// without modification, are permitted provided that the following            
// conditions are met:                                                        
//                                                                             
// Redistributions of source code must retain the above copyright             
// notice, this list of conditions and the following disclaimer.              
// Redistributions in binary form must reproduce the above copyright          
// notice, this list of conditions and the following disclaimer in            
// the documentation and/or other materials provided with the distribution.   
//                                                                             
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS        
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT          
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS          
// FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT   
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,      
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED   
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,        
// OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY     
// OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING    
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS         
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.               
//                                                                             
// Contributors:
//      James Nies
//      - Initial creation
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
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
	/// Code arranger configuration information
	/// </summary>
	public class CodeConfiguration : ConfigurationElement	
	{
		#region Fields
		
		private static CodeConfiguration _default;		
		private static object _defaultLock = new object();		
		private List<HandlerConfiguration> _handlers;		
		
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
		
		#region Public Methods
		
		/// <summary>
		/// Loads a configuration from file
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static CodeConfiguration Load(string filename)		
		{
			using (FileStream fileStream = new FileStream(filename, FileMode.Open))
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
			XmlSerializer serializer = new XmlSerializer(typeof(CodeConfiguration));
			CodeConfiguration configuration = 
			    serializer.Deserialize(stream) as CodeConfiguration;
			
			return configuration;
		}		
		
		/// <summary>
		/// Saves the configuration to a file.
		/// </summary>
		/// <param name="filename"></param>
		public void Save(string filename)		
		{
			XmlSerializer serializer = new XmlSerializer(this.GetType());
			using (FileStream stream = new FileStream(filename, FileMode.Create))
			{
			    serializer.Serialize(stream, this);
			}
		}		
		
		#endregion Public Methods
		
		#region Public Properties
		
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
		
		#endregion Public Properties
		
		#region Protected Methods
		
		/// <summary>
		/// Creates a clone of this instance.
		/// </summary>
		/// <returns></returns>
		protected override ConfigurationElement DoClone()		
		{
			CodeConfiguration clone = new CodeConfiguration();
			
			foreach (HandlerConfiguration handler in this.Handlers)
			{
			    HandlerConfiguration handlerClone = handler.Clone() as HandlerConfiguration;
			    clone.Handlers.Add(handlerClone);
			}
			
			return clone;
		}		
		
		#endregion Protected Methods
	}
}