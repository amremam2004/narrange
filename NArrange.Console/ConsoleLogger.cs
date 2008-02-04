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
using System.Collections.Generic;
using System.Text;

using NArrange.Core;

namespace NArrange.ConsoleApplication
{
	/// <summary>
	/// Console logger
	/// </summary>
	public class ConsoleLogger : ILogger
	{
		#region Constants

		private const ConsoleColor ErrorColor = ConsoleColor.Red;
		private const ConsoleColor InfoColor = ConsoleColor.Cyan;
		private const ConsoleColor TraceColor = ConsoleColor.Gray;
		private const ConsoleColor WarningColor = ConsoleColor.Yellow;

		#endregion Constants

		#region Public Methods

		/// <summary>
		/// Logs a message to the console
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		/// <param name="args"></param>
		public void LogMessage(LogLevel level, string message, params object[] args)
		{
			switch (level)
			{
			    case LogLevel.Error:
			        WriteMessage(ErrorColor, message, args);
			        break;
			
			    case LogLevel.Warning:
			        WriteMessage(WarningColor, message, args);
			        break;
			
			    case LogLevel.Info:
			        WriteMessage(InfoColor, message, args);
			        break;
			
				case LogLevel.Trace:
					#if TRACE
					WriteMessage(TraceColor, message, args);
					#endif
					break;
			
			    default:
			        WriteMessage(Console.ForegroundColor, message, args);
			        break;
			}
		}

		/// <summary>
		/// Writes a message to the console using the specified color.
		/// </summary>
		/// <param name="color"></param>
		/// <param name="message"></param>
		/// <param name="args"></param>
		public void WriteMessage(ConsoleColor color, string message, params object[] args)
		{
			ConsoleColor origColor = Console.ForegroundColor;
			
			try
			{
			    Console.ForegroundColor = color;
			    if (args != null && args.Length > 0)
			    {
			        Console.WriteLine(message, args);
			    }
			    else
			    {
			        Console.WriteLine(message);
			    }
			}
			finally
			{
			    Console.ForegroundColor = origColor;
			}
		}

		#endregion Public Methods
	}
}