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
using System.Text;
using System.Xml;

namespace NArrange.Core
{
	/// <summary>
	/// Parses a solution for individual project file names
	/// </summary>
	public class SolutionParser	
	{
		#region Public Methods
		
		/// <summary>
		/// Parses project file names from a solution file.
		/// </summary>
		/// <param name="solutionFile"></param>
		/// <returns>A list of project filenames</returns>
		public ReadOnlyCollection<string> Parse(string solutionFile)		
		{
			if (solutionFile == null)
			{
			    throw new ArgumentNullException("solutionFile");
			}
			
			string solutionPath = Path.GetDirectoryName(solutionFile);
			
			List<string> projectFiles = new List<string>();
			
			using (StreamReader reader = new StreamReader(solutionFile, Encoding.Default))
			{
			    while (!reader.EndOfStream)
			    {
			        //
			        // Find lines like the following:
			        // Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "NArrange.Core", "NArrange.Core\NArrange.Core.csproj", "{CD74EA33-223D-4CD9-9028-AADD4E929613}"
			
			        string line = reader.ReadLine().TrimStart();
			        if (line.StartsWith("Project"))
			        {
			            string[] projectData = line.Split(',');
			            string projectFile = projectData[1].Trim().Trim('"');
			            string projectPath = Path.Combine(solutionPath, projectFile);
			            projectFiles.Add(projectPath);
			        }
			    }
			}
			
			return projectFiles.AsReadOnly();
		}		
		
		#endregion Public Methods

	}
}