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
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using NArrange.Core;
using NArrange.Core.Configuration;
using NArrange.Tests.CSharp;
using NArrange.Tests.Core;
using NArrange.Tests.VisualBasic;

namespace NArrange.SourceTester
{
	/// <summary>
	/// Tests NArrange against a directory of source files that
	/// have no external dependencies (i.e. only mscorlib.dll and System.dll).
	/// </summary>
	public class Program
	{
		#region Constants

		private const int Fail = -1;

		#endregion Constants

		#region Private Methods

		/// <summary>
		/// Compares two assemblies
		/// </summary>
		/// <param name="assembly1"></param>
		/// <param name="assembly2"></param>
		/// <returns></returns>
		private static bool CompareAssemblies(Assembly assembly1, Assembly assembly2, ILogger logger)
		{
			Type[] assembly1Types = assembly1.GetTypes();
			Type[] assembly2Types = assembly2.GetTypes();

			bool areSame = true;

			if (assembly1Types.Length != assembly2Types.Length)
			{
			    logger.LogMessage(LogLevel.Warning, "Assemblies have a different number of types.");
			}

			if (areSame)
			{
			    foreach (Type type1 in assembly1Types)
			    {
			        if (!type1.FullName.StartsWith("<PrivateImplementationDetails>", StringComparison.Ordinal))
			        {
			            bool typeFound = false;
			            for (int typeIndex = 0; typeIndex < assembly2Types.Length; typeIndex++)
			            {
			                Type type2 = assembly2Types[typeIndex];
			                if (type2.FullName == type1.FullName)
			                {
			                    typeFound = true;

			                    areSame = CompareType(type1, type2, logger);
			                }
			            }

			            if (!typeFound)
			            {
			                logger.LogMessage(LogLevel.Warning, "Assembly is missing type {0}.", type1.FullName);
			                areSame = false;
			            }
			        }
			    }
			}

			return areSame;
		}

		private static bool CompareType(Type type1, Type type2, ILogger logger)
		{
			bool areSame = true;

			MemberInfo[] type1Members = type1.GetMembers();
			MemberInfo[] type2Members = type2.GetMembers();
			if (type1Members.Length == type2Members.Length)
			{
			    //TODO: Compare members
			}
			else
			{
			    logger.LogMessage(LogLevel.Warning,
			        "Type {0} has a different number of members.",
			        type1.FullName);
			    areSame = false;
			}

			return areSame;
		}

		private static CompilerResults CompileSourceFile(FileInfo sourceFile, string source)
		{
			CompilerResults results = null;

			string extension = sourceFile.Extension.TrimStart('.').ToUpperInvariant();

			switch (extension)
			{
			    case "CS":
			        results = CSharpTestFile.Compile(source, sourceFile.GetHashCode().ToString());
			        break;

			    case "VB":
			        results = VBTestFile.Compile(source, sourceFile.GetHashCode().ToString());
			        break;
			}

			return results;
		}

		private static FileInfo[] GetSourceFileNames(string path)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(path);

			List<FileInfo> sourceFiles = new List<FileInfo>();
			sourceFiles.AddRange(directoryInfo.GetFiles("*.cs", SearchOption.AllDirectories));
			sourceFiles.AddRange(directoryInfo.GetFiles("*.vb", SearchOption.AllDirectories));

			return sourceFiles.ToArray();
		}

		/// <summary>
		/// Parses the command line arguments
		/// </summary>
		/// <param name="args"></param>
		/// <param name="inputDirectory"></param>
		private static void ParseArguments(string[] args, ref string inputDirectory)
		{
			if (args.Length != 1)
			{
			    WriteUsage();
			    Environment.Exit(Fail);
			}

			inputDirectory = args[0];
		}

		private static void TestFiles(string inputDir, ILogger logger)
		{
			if (!Directory.Exists(inputDir))
			{
			    logger.LogMessage(LogLevel.Error, "Test directory {0} does not exist", inputDir);
			    Environment.Exit(Fail);
			}

			string arrangedDir = Path.Combine(inputDir, "Arranged");
			if (Directory.Exists(arrangedDir))
			{
			    Directory.Delete(arrangedDir, true);
			}
			Directory.CreateDirectory(arrangedDir);

			FileInfo[] allSourceFiles = GetSourceFileNames(inputDir);
			logger.LogMessage(LogLevel.Info, "Testing with {0} source files...",
			    allSourceFiles.Length);

			int noNamespaceCount = 0;
			int preprocessorCount = 0;
			int uncompiledCount = 0;
			int successCount = 0;
			int failedCount = 0;

			foreach (FileInfo sourceFile in allSourceFiles)
			{
			    string initialSource = File.ReadAllText(sourceFile.FullName, Encoding.Default);

			    if (initialSource.ToLower().Contains("namespace"))
			    {
			        CompilerResults initialResults = CompileSourceFile(sourceFile, initialSource);

			        CompilerError error = TestUtilities.GetCompilerError(initialResults);
			        if (error == null)
			        {
			            logger.LogMessage(LogLevel.Trace, "Succesfully compiled {0}", sourceFile.FullName);


			            //
			            // Arrange the source code file
			            //
			            TestLogger testLogger = new TestLogger();
			            FileArranger fileArranger = new FileArranger(null, testLogger);
			            string outputFile = Path.Combine(arrangedDir, sourceFile.Name);
			            bool success = false;
			            try
			            {
			                success = fileArranger.Arrange(sourceFile.FullName, outputFile);
			            }
			            catch (Exception ex)
			            {
			                logger.LogMessage(LogLevel.Error, "Unable to arrange {0}.  {1}",
			                    sourceFile.Name, ex.Message);
			                failedCount++;
			            }

			            if (success)
			            {
			                logger.LogMessage(LogLevel.Info, "Arrange successful.");
			            }
			            else if (testLogger.HasPartialMessage(LogLevel.Warning,
			                "preprocessor"))
			            {
			                logger.LogMessage(LogLevel.Trace, "File is unhandled.");
			                preprocessorCount++;
			            }
			            else
			            {
			                foreach (TestLogger.TestLogEvent logEvent in testLogger.Events)
			                {
			                    logger.LogMessage(logEvent.Level, logEvent.Message);
			                }

			                logger.LogMessage(LogLevel.Error, "Unable to arrange {0}.", sourceFile.Name);
			                failedCount++;
			            }

			            if (success)
			            {
			                string arrangedSource = File.ReadAllText(outputFile, Encoding.Default);
			                CompilerResults arrangedResults = CompileSourceFile(
			                    new FileInfo(outputFile), arrangedSource);

			                CompilerError arrangedError = TestUtilities.GetCompilerError(arrangedResults);
			                if (arrangedError == null)
			                {
			                    logger.LogMessage(LogLevel.Trace, "Succesfully compiled arranged file {0}", outputFile);
			                    bool assembliesMatch = CompareAssemblies(
			                        initialResults.CompiledAssembly, arrangedResults.CompiledAssembly,
			                        logger);
			                    if (assembliesMatch)
			                    {
			                        successCount++;
			                    }
			                    else
			                    {
			                        logger.LogMessage(LogLevel.Error, "Arranged assembly differs.");
			                        failedCount++;
			                    }
			                }
			                else
			                {
			                    logger.LogMessage(LogLevel.Error, "Failed to compile arranged file {0}, {1}",
			                        outputFile, arrangedError.ToString());
			                    failedCount++;
			                }
			            }
			        }
			        else
			        {
			            logger.LogMessage(LogLevel.Error, "Failed to compile {0}", sourceFile.FullName);
			            uncompiledCount++;
			        }
			    }
			    else
			    {
			        noNamespaceCount++;
			    }
			}

			logger.LogMessage(LogLevel.Info, "Unsupported - preprocessor: " + preprocessorCount.ToString());
			logger.LogMessage(LogLevel.Info, "Unsupported - no namespace: " + noNamespaceCount.ToString());
			logger.LogMessage(LogLevel.Info, "Uncompiled: " + uncompiledCount.ToString());
			logger.LogMessage(LogLevel.Info, "Success: " + successCount.ToString());
			logger.LogMessage(LogLevel.Info, "Failed: " + failedCount.ToString());
		}

		/// <summary>
		/// Writes usage information to the console
		/// </summary>
		private static void WriteUsage()
		{
			Console.WriteLine("Usage:");
			Console.WriteLine("narrange-test <input dir>");
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("input dir\tSpecifies the test source file directory.");
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
			ConsoleLogger.WriteMessage(ConsoleColor.Cyan, "NArrange Test {0}", version);
			Console.WriteLine(new string('_', 60));

			object[] copyrightAttributes = assembly.GetCustomAttributes(
			    typeof(AssemblyCopyrightAttribute), false);
			if (copyrightAttributes.Length > 0)
			{
			    AssemblyCopyrightAttribute copyRight = copyrightAttributes[0] as AssemblyCopyrightAttribute;
			    Console.WriteLine(copyRight.Copyright.Replace("�", "(C)"));
			}
			Console.WriteLine();

			if (args.Length < 1 || args[0] == "?" || args[0] == "/?" || args[0] == "help")
			{
			    WriteUsage();
			    Environment.Exit(Fail);
			}

			string inputDir = null;

			ParseArguments(args, ref inputDir);
			logger.Trace = true;

			try
			{
			    TestFiles(inputDir, logger);
			}
			catch (Exception ex)
			{
			    logger.LogMessage(LogLevel.Error, "Failure: {0}", ex.ToString());
			    Environment.Exit(Fail);
			}
		}

		#endregion Public Methods
	}
}