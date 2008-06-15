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
 *		- Refactoring of SolutionParser to allow for additional solution
 *		  types.
 *		- Support parsing of MonoDevelop solutions.
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;

namespace NArrange.Core
{
	/// <summary>
	/// Parses a solution for individual project file names
	/// </summary>
	public sealed class SolutionParser : ISolutionParser
	{
		#region Static Fields

		private static SolutionParser _instance;
		private static object _instanceLock = new object();

		#endregion Static Fields

		#region Fields

		private List<string> _extensions = new List<string>();
		private Dictionary<string, ISolutionParser> _parserMap = 
			new Dictionary<string, ISolutionParser>(StringComparer.OrdinalIgnoreCase);

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new solution parser.
		/// </summary>
		private SolutionParser()
		{
			List<ISolutionParser> parsers = new List<ISolutionParser>();
			parsers.Add(new MSBuildSolutionParser());
			parsers.Add(new MonoDevelopSolutionParser());

			foreach (ISolutionParser parser in parsers)
			{
				foreach (string extension in parser.Extensions)
				{
					_parserMap.Add(extension, parser);
					_extensions.Add(extension);
				}
			}
		}

		#endregion Constructors

		#region Public Properties

		/// <summary>
		/// Gets a list of extensions supported by this solution parser.
		/// </summary>
		public ReadOnlyCollection<string> Extensions
		{
			get
			{
				return _extensions.AsReadOnly();
			}
		}

		/// <summary>
		/// Gets a solution parser for all solution types.
		/// </summary>
		public static SolutionParser Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_instanceLock)
					{
						if (_instance == null)
						{
							_instance = new SolutionParser();
						}
					}
				}

				return _instance;
			}
		}

		#endregion Public Properties

		#region Public Methods

		/// <summary>
		/// Gets a value indicating whether or not the specified file
		/// is a recognized solution file.
		/// </summary>
		/// <param name="inputFile"></param>
		/// <returns></returns>
		public bool IsSolution(string inputFile)
		{
			bool isSolution = false;

			if (!string.IsNullOrEmpty(inputFile))
			{
				string extension = Path.GetExtension(inputFile).TrimStart('.');
				isSolution = _parserMap.ContainsKey(extension);
			}

			return isSolution;
		}

		/// <summary>
		/// Parses project file names from a solution file.
		/// </summary>
		/// <param name="solutionFile"></param>
		/// <returns>A list of project file names</returns>
		public ReadOnlyCollection<string> Parse(string solutionFile)
		{
			if (solutionFile == null)
			{
			    throw new ArgumentNullException("solutionFile");
			}

			List<string> projectFiles = new List<string>();

			string extension = Path.GetExtension(solutionFile).TrimStart('.');
			ISolutionParser parser = null;
			_parserMap.TryGetValue(extension, out parser);

			if (parser != null)
			{
				projectFiles.AddRange(parser.Parse(solutionFile));
			}

			return projectFiles.AsReadOnly();
		}

		#endregion Public Methods
	}
}