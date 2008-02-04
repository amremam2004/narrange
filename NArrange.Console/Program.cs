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
 *      - Moved logging into a ConsoleLogger class
 *		- Added a backup and restore feature
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
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

		#region Private Methods

		/// <summary>
		/// Parses the command line arguments
		/// </summary>
		/// <param name="args"></param>
		/// <param name="configFile"></param>
		/// <param name="inputFile"></param>
		/// <param name="outputFile"></param>
		/// <param name="backup"></param>
		/// <param name="restore"></param>
		private static void ParseArguments(string[] args,
			ref string configFile, ref string inputFile, ref string outputFile,
			ref bool backup, ref bool restore)
		{
			List<string> argList = new List<string>(args);
			for (int argIndex = 0; argIndex < argList.Count; argIndex++)
			{
			    string arg = argList[argIndex];
			
			    if (arg.StartsWith("/"))
			    {
			        string argLower = arg.ToLower();
			        if (arg.Length == 2)
			        {
			            char flag = argLower[1];
			
			            switch (flag)
			            {
			                case 'c':
			                    configFile = arg.Substring(3);
			                    argList.RemoveAt(argIndex);
			                    argIndex--;
			                    break;
			
							case 'b':
								backup = true;
								argList.RemoveAt(argIndex);
								argIndex--;
								break;
			
							case 'r':
								restore = true;
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
		/// Writes usage information to the console
		/// </summary>
		private static void WriteUsage()
		{
			Console.WriteLine("Usage:");
			Console.WriteLine("narrange.console <input> [output] [/c:configuration]");
			Console.WriteLine("\t[/b:backup] [/r:restore]");
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("input\tSpecifies the source code file, project or solution to arrange.");
			Console.WriteLine();
			Console.WriteLine("output\tFor a single source file, specifies the output file ");
			Console.WriteLine("\tto write arranged code to.");
			Console.WriteLine("\t[Optional] If not specified the input source");
			Console.WriteLine("\tfile will be overwritten.");
			Console.WriteLine();
			Console.WriteLine("/c\tConfiguration - Specifies the XML configuration file to use.");
			Console.WriteLine("\t[Optional] If not specified the default ");
			Console.WriteLine("\tconfiguration will be used.");
			Console.WriteLine();
			Console.WriteLine("/b\tBackup - Specifies to create a backup before arranging");
			Console.WriteLine("\t[Optional] If not specified, no backup will be created ");
			Console.WriteLine();
			Console.WriteLine("/r\tRestore - Restores arranged files from the latest backup");
			Console.WriteLine("\t[Optional] When this flag is provided, no files will be arranged ");
			Console.WriteLine();
		}

		#endregion Private Methods

		#region Public Methods

		/// <summary>
		/// Application entry point
		/// </summary>
		/// <param name="args"></param>
		public static void Main(string[] args)
		{
			ConsoleLogger logger = new ConsoleLogger();
			
			Assembly assembly = Assembly.GetExecutingAssembly();
			Version version = assembly.GetName().Version;
			Console.WriteLine();
			logger.WriteMessage(ConsoleColor.Cyan, "NArrange {0}", version);
			Console.WriteLine(new string('_', 60));
			
			object[] copyrightAttributes = assembly.GetCustomAttributes(
			    typeof(AssemblyCopyrightAttribute), false);
			if(copyrightAttributes.Length > 0)
			{
			    AssemblyCopyrightAttribute copyRight = copyrightAttributes[0] as AssemblyCopyrightAttribute;
			    Console.WriteLine(copyRight.Copyright.Replace("©", "(C)"));
			}
			Console.WriteLine();
			
			if (args.Length < 1 || args[0] == "?" || args[0] == "/?" || args[0] == "help")
			{
			    WriteUsage();
			    Environment.Exit(Fail);
			}
			
			string configFile = null;
			string inputFile = null;
			string outputFile = null;
			bool backup = false;
			bool restore = false;
			
			ParseArguments(args, ref configFile, ref inputFile,
					ref outputFile, ref backup, ref restore);
			
			if (restore)
			{
				logger.LogMessage(LogLevel.Verbose, "Restoring {0}...", inputFile);
				string key = BackupUtilities.CreateFileNameKey(inputFile);
				bool success = BackupUtilities.RestoreFiles(BackupUtilities.BackupRoot, key);
				if (success)
				{
					logger.LogMessage(LogLevel.Info, "Restored");
				}
				else
				{
					logger.LogMessage(LogLevel.Error, "Restore failed.");
				}
			}
			else
			{
				//
				// Arrange the source code file
				//
				FileArranger fileArranger = new FileArranger(configFile, logger);
				bool success = fileArranger.Arrange(inputFile, outputFile, backup);
			
				if (!success)
				{
					logger.LogMessage(LogLevel.Error, "Unable to arrange {0}.", inputFile);
					Environment.Exit(Fail);
				}
				else
				{
					logger.LogMessage(LogLevel.Info, "Arrange successful.");
				}
			}
		}

		#endregion Public Methods
	}
}