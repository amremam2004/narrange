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
 *      - Fixed a bug where the output file override was not being 
 *        acknowledged
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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

		private Dictionary<string, ArrangeResult> _arrangeResults;
		private CodeArranger _codeArranger;
		private string _configFile;
		private CodeConfiguration _configuration;
		private int _filesParsed;
		private int _filesWritten;
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
		/// <param name="elements"></param>
		/// <returns></returns>
		private ReadOnlyCollection<ICodeElement> ArrangeElements(ReadOnlyCollection<ICodeElement> elements)
		{
			if (_codeArranger == null)
			{
				_codeArranger = new CodeArranger(_configuration);
			}

			elements = _codeArranger.Arrange(elements);

			return elements;
		}

		/// <summary>
		/// Arranges an individual source file
		/// </summary>
		/// <param name="inputFile"></param>
		/// <param name="outputFile"></param>
		/// <returns></returns>
		private void ArrangeSourceFile(string inputFile, string outputFile)
		{
			ReadOnlyCollection<ICodeElement> elements = null;
			string inputFileText = null;

			try
			{
				FileAttributes fileAttributes = File.GetAttributes(inputFile);
				if (inputFile == outputFile &&
			        ((fileAttributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly))
				{
					LogMessage(LogLevel.Warning, "File {0} is read-only", inputFile);
				}
				else
				{
					inputFileText = File.ReadAllText(inputFile, Encoding.Default);
					elements = _projectManager.ParseElements(inputFile, inputFileText);
					LogMessage(LogLevel.Trace, "Parsed {0}", inputFile);
				}
			}
			catch (DirectoryNotFoundException)
			{
				LogMessage(LogLevel.Warning, "File {0} does not exist.",
					inputFile);
			}
			catch (FileNotFoundException)
			{
				LogMessage(LogLevel.Warning, "File {0} does not exist.",
					inputFile);
			}
			catch (IOException ioEx)
			{
				LogMessage(LogLevel.Warning, "Unable to read file {0}: {1}",
					inputFile, ioEx.ToString());
			}
			catch (UnauthorizedAccessException ioEx)
			{
				LogMessage(LogLevel.Warning, "Unable to read file {0}: {1}",
					inputFile, ioEx.Message);
			}
			catch (ParseException parseEx)
			{
				LogMessage(LogLevel.Warning, "Unable to parse file {0}: {1}",
					inputFile, parseEx.Message);
			}

			if (elements != null)
			{
				try
				{
					elements = ArrangeElements(elements);
				}
				catch (InvalidOperationException invalidEx)
				{
					LogMessage(LogLevel.Warning, "Unable to arrange file {0}: {1}",
					   inputFile, invalidEx.ToString());
					elements = null;
				}
			}

			string outputFileText = null;
			if (elements != null)
			{
				ICodeElementWriter codeWriter = _projectManager.GetSourceHandler(inputFile).Writer;
				codeWriter.Configuration = _configuration;

				StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
				try
				{
					codeWriter.Write(elements, writer);
				}
				catch (Exception ex)
				{
					LogMessage(LogLevel.Error, ex.ToString());
					throw;
				}

				outputFileText = writer.ToString();
			}

			if (outputFileText != null)
			{
				//
				// Store the arranged elements so that we can create a backup before writing
				//
				_arrangeResults.Add(outputFile, new ArrangeResult(
					inputFile, inputFileText, outputFile, outputFileText));
			}
		}

		private bool Initialize()
		{
			bool success = true;

			_filesParsed = 0;
			_filesWritten = 0;
			_arrangeResults = new Dictionary<string, ArrangeResult>();

			try
			{
			    LoadConfiguration(_configFile);
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

				_projectManager = new ProjectManager(_configuration);
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

		private bool WriteFile(ArrangeResult arrangeResult)
		{
			bool success = true;

			try
			{
				File.WriteAllText(arrangeResult.OutputFile, arrangeResult.OutputFileText,
					Encoding.Default);
				LogMessage(LogLevel.Trace, "Wrote {0}", arrangeResult.OutputFile); 
			}
			catch (IOException ioEx)
			{
				LogMessage(LogLevel.Warning, "Unable to write file {0}: {1}",
					arrangeResult.OutputFile, ioEx.Message);
				success = false;
			}
			catch (UnauthorizedAccessException ioEx)
			{
				LogMessage(LogLevel.Warning, "Unable to write file {0}: {1}",
					arrangeResult.OutputFile, ioEx.Message);
				success = false;
			}

			if (success)
			{
				_filesWritten++;
			}

			return success;
		}

		private bool WriteFiles(string inputFile, bool backup)
		{
			bool success = true;

			List<string> filesToModify = new List<string>();
			_filesParsed = _arrangeResults.Count;
			LogMessage(LogLevel.Verbose, "{0} files parsed.", _filesParsed);

			Dictionary<string, ArrangeResult>.Enumerator enumerator = _arrangeResults.GetEnumerator();
			while(enumerator.MoveNext())
			{
				ArrangeResult fileResult = enumerator.Current.Value;
				if (fileResult.Modified)
				{
					filesToModify.Add(fileResult.OutputFile);
				}
				else
				{
					LogMessage(LogLevel.Trace, "File {0} will not be modified",
						fileResult.OutputFile);
				}
			}

			if (backup && filesToModify.Count > 0)
			{
				try
				{
					LogMessage(LogLevel.Verbose, "Creating backup for {0}...", inputFile);
					string key = BackupUtilities.CreateFileNameKey(inputFile);
					string backupLocation = BackupUtilities.BackupFiles(
						BackupUtilities.BackupRoot, key, filesToModify);
					LogMessage(LogLevel.Info, "Backup created at {0}", backupLocation);
				}
				catch(Exception ex)
				{
					LogMessage(LogLevel.Error, 
						"Unable to create backup for {0} - {1}", inputFile, ex.Message);
					success = false;
					_filesParsed = 0;
				}
			}

			if (success)
			{
				LogMessage(LogLevel.Verbose, "Writing files...");
				foreach (string fileToModify in filesToModify)
				{
					WriteFile(_arrangeResults[fileToModify]);
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

			success = Initialize();

			if (success)
			{
				bool isProject = _projectManager.IsProject(inputFile);
				bool isSolution = !isProject && ProjectManager.IsSolution(inputFile);

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
						LogMessage(LogLevel.Verbose, "Parsing files...");

						foreach (string sourceFile in sourceFiles)
						{
			                if (string.IsNullOrEmpty(outputFile))
			                {
			                    ArrangeSourceFile(sourceFile, sourceFile);
			                }
			                else
			                {
			                    ArrangeSourceFile(sourceFile, outputFile);
			                }
						}

						if (success && _arrangeResults.Count > 0)
						{
							success = WriteFiles(inputFile, backup);
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

					if (_filesParsed == 0 && (sourceFiles.Count <= 1 && !(isProject || isSolution)))
					{
						success = false;
					}
				}
			}

			LogMessage(LogLevel.Verbose, "{0} files written.", _filesWritten);

			return success;
		}

		#endregion Public Methods

		#region Other

		/// <summary>
		/// Arrange result
		/// </summary>
		private class ArrangeResult
		{
			private readonly string _inputFile;
			private readonly string _outputFile;
			private readonly string _outputFileText;
			private readonly bool _modified;
			/// <summary>
			/// Creates a new ArrangeResult
			/// </summary>
			/// <param name="inputFile"></param>
			/// <param name="inputFileText"></param>
			/// <param name="outputFile"></param>
			/// <param name="outputFileText"></param>
			public ArrangeResult(string inputFile, string inputFileText, 
				string outputFile, string outputFileText)
			{
				_inputFile = inputFile;
				_outputFile = outputFile;
				_outputFileText = outputFileText;
				_modified = _inputFile != _outputFile ||
					inputFileText != outputFileText;
			}

			public string OutputFile
			{
				get
				{
					return _outputFile;
				}
			}

			public string OutputFileText
			{
				get
				{
					return _outputFileText;
				}
			}

			public bool Modified
			{
				get
				{
					return _modified;
				}
			}
		}

		#endregion Other
	}
}