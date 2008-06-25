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


namespace NArrange.VisualBasic
{
	/// <summary>
	/// Visual Basic character constants
	/// </summary>
	public static class VBSymbol
	{
		#region Constants

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
		public const char BeginAttribute = '<';

		/// <summary>
		/// Beginning of comment
		/// </summary>
		public const char BeginComment = '\'';

		/// <summary>
		/// Beginning of parameter list
		/// </summary>
		public const char BeginParameterList = '(';

		/// <summary>
		/// Beginning of string
		/// </summary>
		public const char BeginString = '"';

		/// <summary>
		/// Begin type constraint list
		/// </summary>
		public const char BeginTypeConstraintList = '{';

		/// <summary>
		/// End of attribute
		/// </summary>
		public const char EndAttribute = '>';

		/// <summary>
		/// End of parameter list
		/// </summary>
		public const char EndParameterList = ')';

		/// <summary>
		/// End type constraint list
		/// </summary>
		public const char EndTypeConstraintList = '}';

		/// <summary>
		/// Line continuation.
		/// </summary>
		public const char LineContinuation = '_';

		/// <summary>
		/// Line delimiter.
		/// </summary>
		public const char LineDelimiter = ':';

		/// <summary>
		/// Preprocessor
		/// </summary>
		public const char Preprocessor = '#';

		#endregion Constants

		#region Public Methods

		/// <summary>
		/// Determines if the specified char is a Visual Basic symbol character.
		/// </summary>
		/// <param name="character"></param>
		/// <returns></returns>
		public static bool IsVBSymbol(char character)
		{
			return character == AliasSeparator ||
			    character == Assignment ||
			    character == BeginAttribute ||
			    character == BeginComment ||
			    character == BeginTypeConstraintList ||
			    character == EndTypeConstraintList ||
			    character == BeginParameterList ||
			    character == BeginString ||
			    character == EndAttribute ||
			    character == EndParameterList ||
			    character == Preprocessor ||
			    character == LineContinuation ||
			    character == LineDelimiter;
		}

		#endregion Public Methods
	}
}