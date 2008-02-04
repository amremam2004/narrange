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
 *		- Added a backup and restore feature
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using NArrange.Core.CodeElements;
using NArrange.Core.Configuration;

namespace NArrange.Core
{
	/// <summary>
	/// Class for arranging source code files
	/// </summary>
	public sealed class FileArranger
	{
		#region Fields

		private string _configFile;		
		private CodeConfiguration _configuration;		
		private int _filesProcessed;		
		private ILogger _logger;		
		private ProjectManager _projectManager;		
		
		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new file arranger
		/// </summary>
		/// <param name="configFile"></param>
		/// <param name="logger"></param>
		public FileArranger(string configFile, ILogger logger)
		{
			_configFile = configFile;
			_logger = logger;
		}

		#endregion Constructors

		#region Private Methods

		/// <summary>
		/// Arranges code elements
		/// </summary>
		/// <param name="codeConfiguration"></param>
		/// <param name="elements"></param>
		/// <returns></returns>
		private static ReadOnlyCollection<ICodeElement> ArrangeElements(
			CodeConfiguration codeConfiguration, ReadOnlyCollection<ICodeElement> elements)
		{
			CodeArranger arranger = new CodeArranger(codeConfiguration);
			elements = arranger.Arrange(elements);
			return elements;
		}

		/// <summary>
		/// Arranges an individual source file
		/// </summary>
		/// <param name="inputFile"></param>
		/// <param name="outputFile"></param>
		/// <returns></returns>
		private bool ArrangeSourceFile(string inputFile, string outputFile)
		{
			bool success = true;
			
			ReadOnlyCollection<ICodeElement> elements = null;
			
			    try
			    {
			        elements = ParseElements(inputFile);
			    }
			    catch (DirectoryNotFoundException)
			    {
			        LogMessage(LogLevel.Warning, "File {0} does not exist.",
			            inputFile);
			        success = false;
			    }
			    catch (FileNotFoundException)
			    {
			        LogMessage(LogLevel.Warning, "File {0} does not exist.",
			            inputFile);
			        success = false;
			    }
			    catch (IOException ioEx)
			    {
			        LogMessage(LogLevel.Warning, "Unable to read file {0}: {1}",
			            inputFile, ioEx.ToString());
			        success = false;
			    }
			    catch (UnauthorizedAccessException ioEx)
			    {
			        LogMessage(LogLevel.Warning, "Unable to read file {0}: {1}",
			            inputFile, ioEx.Message);
			        success = false;
			    }
			    catch (ParseException parseEx)
			    {
			        LogMessage(LogLevel.Warning, "Unable to parse file {0}: {1}",
			            inputFile, parseEx.Message);
			        success = false;
			    }
			
			if (success)
			{
			    try
			    {
			        elements = ArrangeElements(_configuration, elements);
			    }
			    catch (InvalidOperationException invalidEx)
			    {
			        LogMessage(LogLevel.Warning, "Unable to arrange file {0}: {1}",
			           inputFile, invalidEx.ToString());
			        success = false;
			    }
			}
			
			if (success)
			{
				try
				{
					success = WriteElements(outputFile, elements);
					if (!success)
					{
						LogMessage(LogLevel.Warning, "Unable to write file {0}",
						inputFile);
					}
				}
				catch (IOException ioEx)
				{
					LogMessage(LogLevel.Warning, "Unable to write file {0}: {1}",
						inputFile, ioEx.Message);
					success = false;
				}
				catch (UnauthorizedAccessException ioEx)
				{
					LogMessage(LogLevel.Warning, "Unable to write file {0}: {1}",
						inputFile, ioEx.Message);
					success = false;
				}
			}
			
			return success;
		}

		private bool InitializeConfiguration()
		{
			bool success = true;
			
			try
			{
			    LoadConfiguration(_configFile);
			    _projectManager = new ProjectManager(_configuration);
			}
			catch (InvalidOperationException xmlEx)
			{
			    LogMessage(LogLevel.Error, "Unable to load configuration file {0}: {1}",
			        _configFile, xmlEx.Message);
			    success = false;
			}
			catch (IOException ioEx)
			{
			    LogMessage(LogLevel.Error, "Unable to load configuration file {0}: {1}",
			        _configFile, ioEx.Message);
			    success = false;
			}
			catch (UnauthorizedAccessException authEx)
			{
			    LogMessage(LogLevel.Error, "Unable to load configuration file {0}: {1}",
			        _configFile, authEx.Message);
			    success = false;
			}
			catch (TargetInvocationException invEx)
			{
			    LogMessage(LogLevel.Error, "Unable to load extension assembly from configuration file {0}: {1}",
			        _configFile, invEx.Message);
			    success = false;
			}
			
			return success;
		}

		/// <summary>
		/// Loads the configuration file that specifies how elements will be arranged.
		/// </summary>
		/// <param name="configFile"></param>
		/// <returns></returns>
		private void LoadConfiguration(string configFile)
		{
			if (_configuration == null)
			{
			    if (configFile != null)
			    {
			        _configuration = CodeConfiguration.Load(configFile);
			    }
			    else
			    {
			        _configuration = CodeConfiguration.Default;
			    }
			}
		}

		/// <summary>
		/// Log a message
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		/// <param name="args"></param>
		private void LogMessage(LogLevel level, string message, params object[] args)
		{
			if (_logger != null)
			{
			    _logger.LogMessage(level, message, args);
			}
		}

		/// <summary>
		/// Writes arranged elements to the output file
		/// </summary>
		/// <param name="outputFile"></param>
		/// <param name="elements"></param>
		/// <returns></returns>
		private bool WriteElements(
			string outputFile, ReadOnlyCollection<ICodeElement> elements)
		{
			bool success = true;
			ICodeWriter codeWriter = _projectManager.GetSourceHandler(outputFile).Writer;
			codeWriter.Configuration = _configuration;
			
			StringWriter writer = new StringWriter();
			try
			{
			    codeWriter.Write(elements, writer);
			}
			catch (Exception ex)
			{
			    LogMessage(LogLevel.Error, ex.ToString());
			    throw;
			}
			
			if (success)
			{
			    string content = writer.ToString();
			
			    bool sameContents = false;
			    if (File.Exists(outputFile))
			    {
			        string existingContent = File.ReadAllText(outputFile, Encoding.Default);
			        sameContents = existingContent == content;
			    }
			
			    if (!sameContents)
			    {
			        File.WriteAllText(outputFile, content, Encoding.Default);
			    }
			}
			
			return success;
		}

		#endregion Private Methods

		#region Public Methods

		/// <summary>
		/// Arranges a file, project or solution
		/// </summary>
		/// <param name="inputFile"></param>
		/// <param name="outputFile"></param>
		/// <returns></returns>
		public bool Arrange(string inputFile, string outputFile)
		{
			return Arrange(inputFile, outputFile, false);
		}

		/// <summary>
		/// Arranges a file, project or solution
		/// </summary>
		/// <param name="inputFile"></param>
		/// <param name="outputFile"></param>
		/// <param name="backup"></param>
		/// <returns></returns>
		public bool Arrange(string inputFile, string outputFile, bool backup)
		{
			bool success = true;
			_filesProcessed = 0;
			
			success = InitializeConfiguration();
			
			if (success)
			{
				bool isProject = _projectManager.IsProject(inputFile);
				bool isSolution = !isProject && _projectManager.IsSolution(inputFile);
			
				if (!(isProject || isSolution))
				{
					if (outputFile == null)
					{
						outputFile = new FileInfo(inputFile).FullName;
					}
			
					bool canParse = _projectManager.CanParse(inputFile);
					if (!canParse)
					{
						LogMessage(LogLevel.Warning,
							"No assembly is registered to handle file {0}.  Please update the configuration.",
							inputFile);
						success = false;
					}
				}
			
				if (success)
				{
					ReadOnlyCollection<string> sourceFiles = _projectManager.GetSourceFiles(inputFile);
					if (sourceFiles.Count > 0)
					{
						if (backup)
						{
							LogMessage(LogLevel.Info, "Creating backup for {0}...", inputFile);
							string key = BackupUtilities.CreateFileNameKey(inputFile);
							string backupLocation = BackupUtilities.BackupFiles(
								BackupUtilities.BackupRoot, key, sourceFiles);
							LogMessage(LogLevel.Verbose, "Backup created at {0}", backupLocation);
						}
			
						foreach (string sourceFile in sourceFiles)
						{
							if (outputFile == null)
							{
								outputFile = sourceFile;
							}
			
							bool fileSuccess = ArrangeSourceFile(sourceFile, sourceFile);
							if (fileSuccess)
							{
								_filesProcessed++;
							}
							else if (sourceFiles.Count <= 1 && !(isProject || isSolution))
							{
								success = false;
							}
						}
					}
					else
					{
						if (isSolution)
						{
							LogMessage(LogLevel.Warning, "Solution {0} does not contain any supported source files.",
								inputFile);
						}
						else if (isProject)
						{
							LogMessage(LogLevel.Warning, "Project {0} does not contain any supported source files.",
							   inputFile);
						}
					}
				}
			}
			
			LogMessage(LogLevel.Verbose, "{0} files processed.", _filesProcessed);
			
			return success;
		}

		/// <summary>
		/// Parses code elements from the input file
		/// </summary>
		/// <param name="inputFile"></param>
		/// <returns></returns>
		public ReadOnlyCollection<ICodeElement> ParseElements(string inputFile)
		{
			InitializeConfiguration();
			
			ReadOnlyCollection<ICodeElement> elements = null;
			SourceHandler sourceHandler = _projectManager.GetSourceHandler(inputFile);
			if (sourceHandler != null)
			{
			    ICodeParser parser = sourceHandler.CodeParser;
			    if (parser != null)
			    {
			        using (StreamReader reader = new StreamReader(inputFile, Encoding.Default))
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