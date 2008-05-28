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
 *      - Allow filtering of source and project files
 *		- Allow arranging of an entire directory
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

using NArrange.Core.CodeElements;
using NArrange.Core.Configuration;

namespace NArrange.Core
{
	/// <summary>
	/// Provides project/file retrieval and storage functionality.
	/// </summary>
	public class ProjectManager
	{
		#region Fields

		private CodeConfiguration _configuration;
		private Dictionary<string, SourceHandler> _projectExtensionHandlers;
		private Dictionary<string, SourceHandler> _sourceExtensionHandlers;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new FileManager
		/// </summary>
		/// <param name="configuration"></param>
		public ProjectManager(CodeConfiguration configuration)
		{
			if (configuration == null)
			{
			    throw new ArgumentNullException("configuration");
			}

			_configuration = configuration;

			this.Initialize();
		}

		#endregion Constructors

		#region Private Methods

		private ReadOnlyCollection<string> GetDirectorySourceFiles(string fileName)
		{
			List<string> sourceFiles = new List<string>();

			DirectoryInfo directoryInfo = new DirectoryInfo(fileName);

			FileInfo[] files = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
			foreach (FileInfo file in files)
			{
				if (IsRecognizedSourceFile(file.FullName))
				{
					sourceFiles.Add(file.FullName);
				}
			}

			return sourceFiles.AsReadOnly();
		}

		/// <summary>
		/// Retrieves an extension handler for a project file
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		private SourceHandler GetProjectHandler(string fileName)
		{
			string extension = GetExtension(fileName);

			SourceHandler sourceHandler = null;
			_projectExtensionHandlers.TryGetValue(extension, out sourceHandler);

			return sourceHandler;
		}

		private ReadOnlyCollection<string> GetProjectSourceFiles(string fileName)
		{
			List<string> sourceFiles = new List<string>();

			SourceHandler handler = GetProjectHandler(fileName);
			if (handler != null)
			{
				bool isRecognizedProject = IsRecognizedFile(fileName, handler.Configuration.ProjectExtensions);

				if (isRecognizedProject)
				{
					IProjectParser projectParser = handler.ProjectParser;

					List<string> extensions = new List<string>();
					foreach (string key in _sourceExtensionHandlers.Keys)
					{
						SourceHandler sourceHandler = _sourceExtensionHandlers[key];
						if (sourceHandler == handler)
						{
							extensions.Add(key);
						}
					}

					ReadOnlyCollection<string> fileNames = projectParser.Parse(fileName);
					foreach (string sourceFile in fileNames)
					{
						if (IsRecognizedSourceFile(sourceFile))
						{
							sourceFiles.Add(sourceFile);
						}
					}
				}
			}

			return sourceFiles.AsReadOnly();
		}

		private ReadOnlyCollection<string> GetSolutionSourceFiles(string fileName)
		{
			List<string> sourceFiles = new List<string>();

			ReadOnlyCollection<string> projectFiles = SolutionParser.Parse(fileName);

			foreach (string projectFile in projectFiles)
			{
			    sourceFiles.AddRange(GetProjectSourceFiles(projectFile));
			}

			return sourceFiles.AsReadOnly();
		}

		/// <summary>
		/// Initializes the manager from the configuration supplied 
		/// during constuction.
		/// </summary>
		private void Initialize()
		{
			//
			// Load extension handlers
			//
			_projectExtensionHandlers = new Dictionary<string, SourceHandler>(
			    StringComparer.OrdinalIgnoreCase);
			_sourceExtensionHandlers = new Dictionary<string, SourceHandler>(
			    StringComparer.OrdinalIgnoreCase);
			foreach (HandlerConfiguration handlerConfiguration in _configuration.Handlers)
			{
			    SourceHandler handler = new SourceHandler(handlerConfiguration);

			    foreach (ExtensionConfiguration extension in handlerConfiguration.ProjectExtensions)
			    {
			        _projectExtensionHandlers.Add(extension.Name, handler);
			    }

			    foreach (ExtensionConfiguration extension in handlerConfiguration.SourceExtensions)
			    {
			        _sourceExtensionHandlers.Add(extension.Name, handler);
			    }
			}
		}

		private static bool IsRecognizedFile(string fileName, List<ExtensionConfiguration> extensions)
		{
			bool isRecognizedFile = true;

			string extension = GetExtension(fileName);
			ExtensionConfiguration extensionConfiguration = null;
			foreach (ExtensionConfiguration extensionEntry in extensions)
			{
			    if (extensionEntry.Name == extension)
			    {
			        extensionConfiguration = extensionEntry;
			        break;
			    }
			}

			if (extensionConfiguration != null && extensionConfiguration.FilterBy != null)
			{
			    FilterBy filterBy = extensionConfiguration.FilterBy;
			    FileFilter fileFilter = new FileFilter(filterBy.Condition);
			    if (File.Exists(fileName))
			    {
			        isRecognizedFile = fileFilter.IsMatch(new FileInfo(fileName));
			    }
			}
			return isRecognizedFile;
		}

		private bool IsRecognizedSourceFile(string fileName)
		{
			bool recognized = false;

			SourceHandler handler = GetSourceHandler(fileName);
			if (handler != null)
			{
			    recognized = IsRecognizedFile(fileName, handler.Configuration.SourceExtensions);
			}

			return recognized;
		}

		#endregion Private Methods

		#region Public Methods

		/// <summary>
		/// Determines whether or not the specified file can be parsed
		/// </summary>
		/// <param name="inputFile"></param>
		/// <returns></returns>
		public bool CanParse(string inputFile)
		{
			return GetSourceHandler(inputFile) != null;
		}

		/// <summary>
		/// Gets the file extension
		/// </summary>
		/// <param name="inputFile"></param>
		/// <returns></returns>
		public static string GetExtension(string inputFile)
		{
			return Path.GetExtension(inputFile).TrimStart('.');
		}

		/// <summary>
		/// Gets all parse-able source files that are children of the specified 
		/// solution or project.  If an individual source file is provided, the
		/// same file name is returned.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public ReadOnlyCollection<string> GetSourceFiles(string fileName)
		{
			List<string> sourceFiles = new List<string>();

			if (IsSolution(fileName))
			{
			    sourceFiles.AddRange(GetSolutionSourceFiles(fileName));
			}
			else if (IsProject(fileName))
			{
			    sourceFiles.AddRange(GetProjectSourceFiles(fileName));
			}
			else if (IsRecognizedSourceFile(fileName))
			{
			    sourceFiles.Add(fileName);
			}
			else if (Directory.Exists(fileName))
			{
				sourceFiles.AddRange(GetDirectorySourceFiles(fileName));
			}

			return sourceFiles.AsReadOnly();
		}

		/// <summary>
		/// Retrieves an extension handler
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public SourceHandler GetSourceHandler(string fileName)
		{
			string extension = GetExtension(fileName);

			SourceHandler handler = null;
			_sourceExtensionHandlers.TryGetValue(extension, out handler);

			return handler;
		}

		/// <summary>
		/// Determines whether or not the specified file is a project
		/// </summary>
		/// <param name="inputFile"></param>
		/// <returns></returns>
		public bool IsProject(string inputFile)
		{
			return this._projectExtensionHandlers.ContainsKey(GetExtension(inputFile));
		}

		/// <summary>
		/// Determines whether or not the specified file is a solution
		/// </summary>
		/// <param name="inputFile"></param>
		/// <returns></returns>
		public static bool IsSolution(string inputFile)
		{
			return GetExtension(inputFile) == "sln";
		}

		/// <summary>
		/// Parses code elements from the input file
		/// </summary>
		/// <param name="inputFile"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		public ReadOnlyCollection<ICodeElement> ParseElements(string inputFile, string text)
		{
			ReadOnlyCollection<ICodeElement> elements = null;
			SourceHandler sourceHandler = this.GetSourceHandler(inputFile);
			if (sourceHandler != null)
			{
			    ICodeElementParser parser = sourceHandler.CodeParser;
			    if (parser != null)
			    {
			        using (StringReader reader = new StringReader(text))
			        {
			            elements = parser.Parse(reader);
			        }
			    }
			}

			return elements;
		}

		#endregion Public Methods
	}
}