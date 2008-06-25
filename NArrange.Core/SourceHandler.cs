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
 *      - Changed constructor to use a handler configuration and expose
 *        the configuration as a property
 *		- Obsoleted the project extensions
 *		Justin Dearing
 *		- Code cleanup via ReSharper 4.0 (http://www.jetbrains.com/resharper/)
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

using System;
using System.Reflection;

using NArrange.Core.Configuration;

namespace NArrange.Core
{
	/// <summary>
	/// This class provides instances for handling language specific requests 
	/// based on file extension.
	/// </summary>
	public sealed class SourceHandler
	{
		#region Fields

		private Assembly _assembly;
		private ICodeElementParser _codeParser;
		private ICodeElementWriter _codeWriter;
		private readonly SourceHandlerConfiguration _configuration;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new SourceHandler.
		/// </summary>
		/// <param name="configuration"></param>
		public SourceHandler(SourceHandlerConfiguration configuration)
		{
			if (configuration == null)
			{
			    throw new ArgumentNullException("configuration");
			}

			_configuration = configuration;

			Initialize();
		}

		#endregion Constructors

		#region Public Properties

		/// <summary>
		/// Gets the code parser associated with the extension
		/// </summary>
		public ICodeElementParser CodeParser
		{
			get
			{
			    return _codeParser;
			}
		}

		/// <summary>
		/// Gets the code writer associated with the extension.
		/// </summary>
		public ICodeElementWriter CodeWriter
		{
			get
			{
			    return _codeWriter;
			}
		}

		/// <summary>
		/// Gets the handler configuration used to create this SourceHandler.
		/// </summary>
		public SourceHandlerConfiguration Configuration
		{
			get
			{
			    return _configuration;
			}
		}

		#endregion Public Properties

		#region Private Methods

		/// <summary>
		/// Initializes the extension handler.
		/// </summary>
		private void Initialize()
		{
			_assembly = Assembly.Load(_configuration.AssemblyName);

			Type[] types = _assembly.GetTypes();
			foreach (Type type in types)
			{
			    if (_codeParser == null && type.GetInterface(typeof(ICodeElementParser).ToString()) != null)
			    {
			        _codeParser = Activator.CreateInstance(type) as ICodeElementParser;
			    }
			    else if (_codeWriter == null && type.GetInterface(typeof(ICodeElementWriter).ToString()) != null)
			    {
			        _codeWriter = Activator.CreateInstance(type) as ICodeElementWriter;
			    }
			}
		}

		#endregion Private Methods
	}
}