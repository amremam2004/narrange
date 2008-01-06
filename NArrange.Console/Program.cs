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
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using NArrange.Core;
using NArrange.Core.CodeElements;
using NArrange.Core.Configuration;

namespace NArrange.ConsoleApplication
{
	/// <summary>
	/// NArrange console application
	/// </summary>
	public class Program	
	{
		#region Constants
		
		private const int Fail = -1;		
		
		#endregion Constants
		
		#region Fields
		
		private static CodeConfiguration _configuration;		
		private static Dictionary<string, SourceHandler> _projectExtensionHandlers;		
		private static Dictionary<string, SourceHandler> _sourceExtensionHandlers;		
		
		#endregion Fields
		
		#region Public Methods
		
		/// <summary>
		/// Application entry point
		/// </summary>
		/// <param name="args"></param>
		public static void Main(string[] args)		
		{
			Version version = Assembly.GetExecutingAssembly().GetName().Version;
			Console.WriteLine();
			Console.WriteLine("NArrange {0}", version);
			Console.WriteLine(new string('_', 60));
			
			if (args.Length < 1 || args[0] == "?" || args[0] == "/?" || args[0] == "help")
			{
			    WriteUsage();
			    Environment.Exit(Fail);
			}
			
			string configFile = null;
			string inputFile = null;
			string outputFile = null;
			
			ParseArguments(args, ref configFile, ref inputFile, ref outputFile);
			
			//
			// Arrange the source code file
			//
			string message = null;
			bool success = Arrange(configFile, inputFile, outputFile, ref message);
			
			if (!success)
			{
			    Console.WriteLine("Unable to arrange {0}. {1}", inputFile, message);
			    Environment.Exit(Fail);
			}
			else
			{
			    Console.WriteLine("Arrange successful.");
			}
		}		
		
		#endregion Public Methods
		
		#region Private Methods
		
		/// <summary>
		/// Arranges an individual source code file
		/// </summary>
		/// <param name="configFile"></param>
		/// <param name="inputFile"></param>
		/// <param name="outputFile"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		private static bool Arrange(string configFile, string inputFile, string outputFile,
			ref string message)		
		{
			bool success = true;
			
			try
			{
			    LoadConfiguration(configFile);
			}
			catch (XmlException xmlEx)
			{
			    message = string.Format("Unable to load configuration file {0}: {1}",
			        configFile, xmlEx.Message);
			    success = false;
			}
			catch (IOException ioEx)
			{
			    message = string.Format("Unable to load configuration file {0}: {1}",
			        configFile, ioEx.Message);
			    success = false;
			}
			catch (UnauthorizedAccessException authEx)
			{
			    message = string.Format("Unable to load configuration file {0}: {1}",
			        configFile, authEx.Message);
			    success = false;
			}
			catch (TargetInvocationException invEx)
			{
			    message = string.Format("Unable to load extension assembly from configuration file {0}: {1}",
			        configFile, invEx.Message);
			    success = false;
			}
			
			bool isProject = IsProject(inputFile);
			if (isProject)
			{
			    success = ArrangeProject(inputFile);
			}
			else if (GetExtension(inputFile) == "sln")
			{
			    SolutionParser solutionParser = new SolutionParser();
			    ReadOnlyCollection<string> projectFiles = solutionParser.Parse(inputFile);
			    if (projectFiles.Count > 0)
			    {
			        foreach (string projectFile in projectFiles)
			        {
			            ArrangeProject(projectFile);
			        }
			    }
			    else
			    {
			        Console.WriteLine("Solution {0} does not contain any project files.",
			            inputFile);
			    }
			}
			else
			{
			    if (outputFile == null)
			    {
			        outputFile = new FileInfo(inputFile).FullName;
			    }
			
			    success = ArrangeFile(inputFile, outputFile, out message);
			}
			
			return success;
		}		
		
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
		
		private static bool ArrangeFile(
			string inputFile, string outputFile, out string message)		
		{
			bool success = true;
			message = null;
			
			ReadOnlyCollection<ICodeElement> elements = null;
			bool canParse = CanParse(inputFile);
			if (!canParse)
			{
			    message = string.Format(
			        "No assembly is registered to handle file {0}.  Please update the configuration.",
			        inputFile);
			    success = false;
			}
			else
			{
			    try
			    {
			        elements = ParseElements(inputFile);
			    }
			    catch (IOException ioEx)
			    {
			        message = string.Format("Unable to parse file {0}: {1}",
			            inputFile, ioEx.ToString());
			        success = false;
			    }
			    catch (UnauthorizedAccessException ioEx)
			    {
			        message = string.Format("Unable to read file {0}: {1}",
			            inputFile, ioEx.Message);
			        success = false;
			    }
			    catch (ParseException parseEx)
			    {
			        message = string.Format("Unable to parse file {0}: {1}",
			            inputFile, parseEx.Message);
			        success = false;
			    }
			}
			
			if (success)
			{
			    try
			    {
			        elements = ArrangeElements(_configuration, elements);
			    }
			    catch (InvalidOperationException invalidEx)
			    {
			        message = string.Format("Unable to arrange file {0}: {1}",
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
			            message = string.Format("Unable to write file {0}",
			            inputFile);
			        }
			    }
			    catch (IOException ioEx)
			    {
			        message = string.Format("Unable to write file {0}: {1}",
			            inputFile, ioEx.Message);
			        success = false;
			    }
			    catch (UnauthorizedAccessException ioEx)
			    {
			        message = string.Format("Unable to write file {0}: {1}",
			            inputFile, ioEx.Message);
			        success = false;
			    }
			}
			
			return success;
		}		
		
		private static bool ArrangeProject(string inputFile)		
		{
			bool success = true;
			
			SourceHandler handler = GetProjectHandler(inputFile);
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
			
			ReadOnlyCollection<string> filenames = projectParser.Parse(inputFile);
			if (filenames.Count > 0)
			{
			    foreach (string sourceFile in filenames)
			    {
			        string extension = GetExtension(sourceFile);
			        if (extensions.Contains(extension))
			        {
			            string message;
			            ArrangeFile(sourceFile, sourceFile, out message);
			            if (message != null)
			            {
			                Console.WriteLine(message);
			            }
			        }
			    }
			}
			else
			{
			    Console.WriteLine("Project {0} does not contain any source files that can be parsed.",
			        inputFile);
			}
			
			return success;
		}		
		
		/// <summary>
		/// Determines whether or not the specified file can be parsed
		/// </summary>
		/// <param name="inputFile"></param>
		/// <returns></returns>
		private static bool CanParse(string inputFile)		
		{
			return _sourceExtensionHandlers.ContainsKey(GetExtension(inputFile));
		}		
		
		/// <summary>
		/// Gets the file extension
		/// </summary>
		/// <param name="inputFile"></param>
		/// <returns></returns>
		private static string GetExtension(string inputFile)		
		{
			return Path.GetExtension(inputFile).TrimStart('.');
		}		
		
		/// <summary>
		/// Retrieves an extension handler for a project file
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		private static SourceHandler GetProjectHandler(string filename)		
		{
			string extension = GetExtension(filename);
			return _projectExtensionHandlers[extension];
		}		
		
		/// <summary>
		/// Retrieves an extension handler
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		private static SourceHandler GetSourceHandler(string filename)		
		{
			string extension = GetExtension(filename);
			return _sourceExtensionHandlers[extension];
		}		
		
		/// <summary>
		/// Determines whether or not the specified file is a project
		/// </summary>
		/// <param name="inputFile"></param>
		/// <returns></returns>
		private static bool IsProject(string inputFile)		
		{
			return _projectExtensionHandlers.ContainsKey(GetExtension(inputFile));
		}		
		
		/// <summary>
		/// Loads the configuration file that specifies how elements will be arranged.
		/// </summary>
		/// <param name="configFile"></param>
		/// <returns></returns>
		private static void LoadConfiguration(string configFile)		
		{
			if (configFile != null)
			{
			    _configuration = CodeConfiguration.Load(configFile);
			}
			else
			{
			    _configuration = CodeConfiguration.Default;
			}
			
			//
			// Load extension handlers
			//
			_projectExtensionHandlers = new Dictionary<string, SourceHandler>(
			    StringComparer.InvariantCultureIgnoreCase);
			_sourceExtensionHandlers = new Dictionary<string, SourceHandler>(
			    StringComparer.InvariantCultureIgnoreCase);
			foreach (HandlerConfiguration handlerConfiguration in _configuration.Handlers)
			{
			    SourceHandler handler = new SourceHandler(handlerConfiguration.AssemblyName);
			
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
		
		/// <summary>
		/// Parses the command line arguments
		/// </summary>
		/// <param name="args"></param>
		/// <param name="configFile"></param>
		/// <param name="inputFile"></param>
		/// <param name="outputFile"></param>
		private static void ParseArguments(string[] args,
			ref string configFile, ref string inputFile, ref string outputFile)		
		{
			List<string> argList = new List<string>(args);
			for (int argIndex = 0; argIndex < argList.Count; argIndex++)
			{
			    string arg = argList[argIndex];
			
			    if (arg.StartsWith("/"))
			    {
			        string argLower = arg.ToLower();
			        if (arg.Length > 3)
			        {
			            char flag = argLower[1];
			
			            switch (flag)
			            {
			                case 'c':
			                    configFile = arg.Substring(3);
			                    argList.RemoveAt(argIndex);
			                    argIndex--;
			                    break;
			
			                default:
			                    WriteUsage();
			                    Environment.Exit(Fail);
			                    break;
			            }
			        }
			        else
			        {
			            WriteUsage();
			            Environment.Exit(Fail);
			        }
			    }
			    else
			    {
			        if (inputFile == null)
			        {
			            inputFile = arg;
			        }
			        else if (outputFile == null)
			        {
			            outputFile = arg;
			        }
			
			        argList.RemoveAt(argIndex);
			        argIndex--;
			    }
			}
			
			if (argList.Count > 0)
			{
			    WriteUsage();
			    Environment.Exit(Fail);
			}
		}		
		
		/// <summary>
		/// Parses code elements from the input file
		/// </summary>
		/// <param name="inputFile"></param>
		/// <returns></returns>
		private static ReadOnlyCollection<ICodeElement> ParseElements(string inputFile)		
		{
			ReadOnlyCollection<ICodeElement> elements;
			ICodeParser parser = GetSourceHandler(inputFile).CodeParser;
			using (StreamReader reader = new StreamReader(inputFile, Encoding.Default))
			{
			    elements = parser.Parse(reader);
			}
			return elements;
		}		
		
		/// <summary>
		/// Writes arranged elements to the output file
		/// </summary>
		/// <param name="outputFile"></param>
		/// <param name="elements"></param>
		/// <returns></returns>
		private static bool WriteElements(
			string outputFile, ReadOnlyCollection<ICodeElement> elements)		
		{
			bool success = true;
			ICodeWriter codeWriter = GetSourceHandler(outputFile).Writer;
			
			StringWriter writer = new StringWriter();
			try
			{
			    codeWriter.Write(elements, writer);
			}
			catch (Exception ex)
			{
			    Console.WriteLine(ex.ToString());
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
		
		/// <summary>
		/// Writes usage information to the console
		/// </summary>
		private static void WriteUsage()		
		{
			Console.WriteLine("Usage:");
			Console.WriteLine("narrange.console [/c:configuration] <input> [output]");
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("/c\tConfiguration - Specifies the XML configuration file to use.");
			Console.WriteLine("\t\t\t(Optional) If not specified the default ");
			Console.WriteLine("\t\t\tconfiguration will be used.");
			Console.WriteLine();
			Console.WriteLine("input\tSpecifies the source code file to arrange.");
			Console.WriteLine();
			Console.WriteLine("output\tSpecifies the file to write arranged code to.");
			Console.WriteLine("\t\t\t(Optional) If not specified the input ");
			Console.WriteLine("\t\t\tfile will be overwritten.");
			Console.WriteLine();
		}		
		
		#endregion Private Methods

	}
}