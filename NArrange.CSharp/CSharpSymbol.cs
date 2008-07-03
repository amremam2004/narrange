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
 *		Justin Dearing
 *		- Removed unused using statements
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header


namespace NArrange.CSharp
{
	/// <summary>
	/// C# character constants
	/// </summary>
	public static class CSharpSymbol
	{
		#region Constants

		/// <summary>
		/// Alias qualifier
		/// </summary>
		public const char AliasQualifier = '.';

		/// <summary>
		/// Alias separator
		/// </summary>
		public const char AliasSeparator = ',';

		/// <summary>
		/// Assignment
		/// </summary>
		public const char Assignment = '=';

		/// <summary>
		/// Beginning of attribute
		/// </summary>
		public const char BeginAttribute = '[';

		/// <summary>
		/// Beginning of block
		/// </summary>
		public const char BeginBlock = '{';

		/// <summary>
		/// Beginning of character literal
		/// </summary>
		public const char BeginCharLiteral = '\'';

		/// <summary>
		/// Beginning of comment
		/// </summary>
		public const char BeginComment = '/';

		/// <summary>
		/// End of string
		/// </summary>
		public const char BeginFinalizer = '~';

		/// <summary>
		/// Beginning of generic parameter
		/// </summary>
		public const char BeginGeneric = '<';

		/// <summary>
		/// Beginning of parameter list
		/// </summary>
		public const char BeginParameterList = '(';

		/// <summary>
		/// Beginning of string
		/// </summary>
		public const char BeginString = '"';

		/// <summary>
		/// Begin verbatim string
		/// </summary>
		public const char BeginVerbatimString = '@';

		/// <summary>
		/// Beginning of block comment
		/// </summary>
		public const char BlockCommentModifier = '*';

		/// <summary>
		/// End of attribute
		/// </summary>
		public const char EndAttribute = ']';

		/// <summary>
		/// End of block
		/// </summary>
		public const char EndBlock = '}';

		/// <summary>
		/// End of generic parameter
		/// </summary>
		public const char EndGeneric = '>';

		/// <summary>
		/// End of statement
		/// </summary>
		public const char EndOfStatement = ';';

		/// <summary>
		/// End of parameter list
		/// </summary>
		public const char EndParameterList = ')';

		/// <summary>
		/// Negate
		/// </summary>
		public const char Negate = '!';

		/// <summary>
		/// Nullable
		/// </summary>
		public const char Nullable = '?';

		/// <summary>
		/// Preprocessor
		/// </summary>
		public const char Preprocessor = '#';

		/// <summary>
		/// Type inheritance 
		/// </summary>
		public const char TypeImplements = ':';

		#endregion Constants

		#region Public Methods

		/// <summary>
		/// Determines if the specified char is a Csharp symbol character
		/// </summary>
		/// <param name="character"></param>
		/// <returns></returns>
		public static bool IsCSharpSymbol(char character)
		{
			return
			    character == AliasQualifier ||
			    character == AliasSeparator ||
			    character == Assignment ||
			    character == BeginAttribute ||
			    character == BeginBlock ||
			    character == BeginCharLiteral ||
			    character == BeginComment ||
			    character == BeginFinalizer ||
			    character == BeginGeneric ||
			    character == BeginParameterList ||
			    character == BeginString ||
			    character == BlockCommentModifier ||
			    character == EndAttribute ||
			    character == EndBlock ||
			    character == EndGeneric ||
			    character == EndOfStatement ||
			    character == EndParameterList ||
			    character == Preprocessor ||
			    character == TypeImplements ||
				character == Negate ||
				character == Nullable ||
				character == BeginVerbatimString;
		}

		#endregion Public Methods
	}
}