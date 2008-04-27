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

namespace NArrange.VisualBasic
{
	/// <summary>
	/// Visual Basic keyword constants
	/// </summary>
	public static class VBKeyword
	{
		#region Constants

		/// <summary>
		/// Alias
		/// </summary>
		public const string Alias = "Alias";

		/// <summary>
		/// Ansi
		/// </summary>
		public const string Ansi = "Ansi";

		/// <summary>
		/// As
		/// </summary>
		public const string As = "As";

		/// <summary>
		/// Auto
		/// </summary>
		public const string Auto = "Auto";

		/// <summary>
		/// Begin
		/// </summary>
		public const string Begin = "Begin";

		/// <summary>
		/// Class
		/// </summary>
		public const string Class = "Class";

		/// <summary>
		/// Const
		/// </summary>
		public const string Constant = "Const";

		/// <summary>
		/// Custom
		/// </summary>
		public const string Custom = "Custom";

		/// <summary>
		/// Declare
		/// </summary>
		public const string Declare = "Declare";

		/// <summary>
		/// Default
		/// </summary>
		public const string Default = "Default";

		/// <summary>
		/// Delegate
		/// </summary>
		public const string Delegate = "Delegate";

		/// <summary>
		/// Dim
		/// </summary>
		public const string Dim = "Dim";

		/// <summary>
		/// End
		/// </summary>
		public const string End = "End";

		/// <summary>
		/// Enum
		/// </summary>
		public const string Enumeration = "Enum";

		/// <summary>
		/// Event
		/// </summary>
		public const string Event = "Event";

		/// <summary>
		/// Friend
		/// </summary>
		public const string Friend = "Friend";

		/// <summary>
		/// Function
		/// </summary>
		public const string Function = "Function";

		/// <summary>
		/// Handles
		/// </summary>
		public const string Handles = "Handles";

		/// <summary>
		/// Implements
		/// </summary>
		public const string Implements = "Implements";

		/// <summary>
		/// Imports
		/// </summary>
		public const string Imports = "Imports";

		/// <summary>
		/// Inherits
		/// </summary>
		public const string Inherits = "Inherits";

		/// <summary>
		/// Interface
		/// </summary>
		public const string Interface = "Interface";

		/// <summary>
		/// Lib
		/// </summary>
		public const string Lib = "Lib";

		/// <summary>
		/// Module
		/// </summary>
		public const string Module = "Module";

		/// <summary>
		/// MustInherit
		/// </summary>
		public const string MustInherit = "MustInherit";

		/// <summary>
		/// MustOverride
		/// </summary>
		public const string MustOverride = "MustOverride";

		/// <summary>
		/// Namespace
		/// </summary>
		public const string Namespace = "Namespace";

		/// <summary>
		/// Narrowing
		/// </summary>
		public const string Narrowing = "Narrowing";

		/// <summary>
		/// New
		/// </summary>
		public const string New = "New";

		/// <summary>
		/// NotInheritable
		/// </summary>
		public const string NotInheritable = "NotInheritable";

		/// <summary>
		/// NotOverridable
		/// </summary>
		public const string NotOverridable = "NotOverridable";

		/// <summary>
		/// Of
		/// </summary>
		public const string Of = "Of";

		/// <summary>
		/// Operator
		/// </summary>
		public const string Operator = "Operator";

		/// <summary>
		/// Option
		/// </summary>
		public const string Option = "Option";

		/// <summary>
		/// Overloads
		/// </summary>
		public const string Overloads = "Overloads";

		/// <summary>
		/// Overridable
		/// </summary>
		public const string Overridable = "Overridable";

		/// <summary>
		/// Overrides
		/// </summary>
		public const string Overrides = "Overrides";

		/// <summary>
		/// Partial
		/// </summary>
		public const string Partial = "Partial";

		/// <summary>
		/// Private
		/// </summary>
		public const string Private = "Private";

		/// <summary>
		/// Property
		/// </summary>
		public const string Property = "Property";

		/// <summary>
		/// Protected
		/// </summary>
		public const string Protected = "Protected";

		/// <summary>
		/// Public
		/// </summary>
		public const string Public = "Public";

		/// <summary>
		/// ReadOnly
		/// </summary>
		public const string ReadOnly = "ReadOnly";

		/// <summary>
		/// ReadWrite
		/// </summary>
		public const string ReadWrite = "ReadWrite";

		/// <summary>
		/// Region
		/// </summary>
		public const string Region = "Region";

		/// <summary>
		/// REM (line comment)
		/// </summary>
		public const string Rem = "REM";

		/// <summary>
		/// Shadows
		/// </summary>
		public const string Shadows = "Shadows";

		/// <summary>
		/// Shared
		/// </summary>
		public const string Shared = "Shared";

		/// <summary>
		/// Structure
		/// </summary>
		public const string Structure = "Structure";

		/// <summary>
		/// Sub
		/// </summary>
		public const string Sub = "Sub";

		/// <summary>
		/// Unicode
		/// </summary>
		public const string Unicode = "Unicode";

		/// <summary>
		/// Widening
		/// </summary>
		public const string Widening = "Widening";

		/// <summary>
		/// WithEvents
		/// </summary>
		public const string WithEvents = "WithEvents";

		/// <summary>
		/// WriteOnly
		/// </summary>
		public const string WriteOnly = "WriteOnly";

		#endregion Constants

		#region Static Fields

		private static Dictionary<string, string> _specialNormalizedKeywords = 
		    CreateNormalizedKeywordDictionary();

		#endregion Static Fields

		#region Private Methods

		/// <summary>
		/// Creates a dictionary of special cases when normalizing keywords.
		/// </summary>
		/// <returns></returns>
		private static Dictionary<string, string> CreateNormalizedKeywordDictionary()
		{
			Dictionary<string, string> _specialNormalizedKeywords;
			_specialNormalizedKeywords = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

			_specialNormalizedKeywords[VBKeyword.ReadOnly] = VBKeyword.ReadOnly;
			_specialNormalizedKeywords[VBKeyword.ReadWrite] = VBKeyword.ReadWrite;
			_specialNormalizedKeywords[VBKeyword.WriteOnly] = VBKeyword.WriteOnly;
			_specialNormalizedKeywords[VBKeyword.MustOverride] = VBKeyword.MustOverride;
			_specialNormalizedKeywords[VBKeyword.MustInherit] = VBKeyword.MustInherit;
			_specialNormalizedKeywords[VBKeyword.NotInheritable] = VBKeyword.NotInheritable;
			_specialNormalizedKeywords[VBKeyword.NotOverridable] = VBKeyword.NotOverridable;
			_specialNormalizedKeywords[VBKeyword.WithEvents] = VBKeyword.WithEvents;
			_specialNormalizedKeywords[VBKeyword.Rem] = VBKeyword.Rem.ToUpper();
			return _specialNormalizedKeywords;
		}

		#endregion Private Methods

		#region Public Methods

		/// <summary>
		/// Determines whether or not the specifed word is a VB keyword
		/// </summary>
		/// <param name="trimmedWord"></param>
		/// <returns></returns>
		public static bool IsVBKeyword(string trimmedWord)
		{
			bool isKeyword = false;

			if (!string.IsNullOrEmpty(trimmedWord))
			{
				string normalized = Normalize(trimmedWord.Trim());

				isKeyword =
			        normalized == VBKeyword.Alias ||
			        normalized == VBKeyword.Ansi ||
			        normalized == VBKeyword.Auto ||
					normalized == VBKeyword.As ||
					normalized == VBKeyword.Begin ||
					normalized == VBKeyword.Class ||
					normalized == VBKeyword.Constant ||
					normalized == VBKeyword.Custom ||
					normalized == VBKeyword.Default ||
					normalized == VBKeyword.Delegate ||
			        normalized == VBKeyword.Dim ||
					normalized == VBKeyword.End ||
					normalized == VBKeyword.Enumeration ||
					normalized == VBKeyword.Event ||
					normalized == VBKeyword.Friend ||
					normalized == VBKeyword.Function ||
			        normalized == VBKeyword.Handles ||
					normalized == VBKeyword.Imports ||
					normalized == VBKeyword.Implements ||
					normalized == VBKeyword.Inherits ||
					normalized == VBKeyword.Interface ||
			        normalized == VBKeyword.Lib ||
			        normalized == VBKeyword.Module ||
					normalized == VBKeyword.MustInherit ||
					normalized == VBKeyword.MustOverride ||
					normalized == VBKeyword.Namespace ||
					normalized == VBKeyword.Narrowing ||
					normalized == VBKeyword.New ||
					normalized == VBKeyword.NotInheritable ||
					normalized == VBKeyword.NotOverridable ||
					normalized == VBKeyword.Of ||
					normalized == VBKeyword.Operator ||
			        normalized == VBKeyword.Option ||
					normalized == VBKeyword.Overloads ||
					normalized == VBKeyword.Overridable ||
					normalized == VBKeyword.Overrides ||
					normalized == VBKeyword.Partial ||
					normalized == VBKeyword.Private ||
					normalized == VBKeyword.Property ||
					normalized == VBKeyword.Protected ||
					normalized == VBKeyword.Public ||
					normalized == VBKeyword.ReadOnly ||
			        normalized == VBKeyword.ReadWrite ||
					normalized == VBKeyword.Region ||
			        normalized == VBKeyword.Rem ||
					normalized == VBKeyword.Shadows ||
					normalized == VBKeyword.Shared ||
					normalized == VBKeyword.Structure ||
					normalized == VBKeyword.Sub ||
			        normalized == VBKeyword.Unicode ||
					normalized == VBKeyword.Widening ||
			        normalized == VBKeyword.WithEvents ||
			        normalized == VBKeyword.WriteOnly;
			}

			return isKeyword;
		}

		/// <summary>
		/// Normalizes a keyword for standard casing
		/// </summary>
		/// <param name="keyword"></param>
		/// <returns></returns>
		public static string Normalize(string keyword)
		{
			string normalized = null;

			if (keyword != null)
			{
				if (keyword.Length > 0)
				{
					if (!(_specialNormalizedKeywords.TryGetValue(keyword, out normalized)))
					{
			            normalized = char.ToUpper(keyword[0]).ToString();
			            if (keyword.Length > 1)
			            {
			                normalized += keyword.Substring(1);
			            }
			        }
				}
			}

			return normalized;
		}

		#endregion Public Methods
	}
}