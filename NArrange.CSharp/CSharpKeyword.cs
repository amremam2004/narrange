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
	/// C# keyword constants
	/// </summary>
	public static class CSharpKeyword
	{
		#region Constants

		/// <summary>
		/// abstract
		/// </summary>
		public const string Abstract = "abstract";

		/// <summary>
		/// as
		/// </summary>
		public const string As = "as";

		/// <summary>
		/// class
		/// </summary>
		public const string Class = "class";

		/// <summary>
		/// const
		/// </summary>
		public const string Constant = "const";

		/// <summary>
		/// delegate
		/// </summary>
		public const string Delegate = "delegate";

		/// <summary>
		/// endregion
		/// </summary>
		public const string EndRegion = "endregion";

		/// <summary>
		/// enum
		/// </summary>
		public const string Enumeration = "enum";

		/// <summary>
		/// event
		/// </summary>
		public const string Event = "event";

		/// <summary>
		/// explicit
		/// </summary>
		public const string Explicit = "explicit";

		/// <summary>
		/// extern
		/// </summary>
		public const string External = "extern";

		/// <summary>
		/// fixed
		/// </summary>
		public const string Fixed = "fixed";

		/// <summary>
		/// global
		/// </summary>
		public const string Global = "global";

		/// <summary>
		/// implicit
		/// </summary>
		public const string Implicit = "implicit";

		/// <summary>
		/// interface
		/// </summary>
		public const string Interface = "interface";

		/// <summary>
		/// internal
		/// </summary>
		public const string Internal = "internal";

		/// <summary>
		/// namespace
		/// </summary>
		public const string Namespace = "namespace";

		/// <summary>
		/// new
		/// </summary>
		public const string New = "new";

		/// <summary>
		/// operator
		/// </summary>
		public const string Operator = "operator";

		/// <summary>
		/// override
		/// </summary>
		public const string Override = "override";

		/// <summary>
		/// partial
		/// </summary>
		public const string Partial = "partial";

		/// <summary>
		/// private
		/// </summary>
		public const string Private = "private";

		/// <summary>
		/// protected
		/// </summary>
		public const string Protected = "protected";

		/// <summary>
		/// public
		/// </summary>
		public const string Public = "public";

		/// <summary>
		/// readonly
		/// </summary>
		public const string ReadOnly = "readonly";

		/// <summary>
		/// region
		/// </summary>
		public const string Region = "region";

		/// <summary>
		/// sealed
		/// </summary>
		public const string Sealed = "sealed";

		/// <summary>
		/// static
		/// </summary>
		public const string Static = "static";

		/// <summary>
		/// struct
		/// </summary>
		public const string Structure = "struct";

		/// <summary>
		/// unsafe
		/// </summary>
		public const string Unsafe = "unsafe";

		/// <summary>
		/// using
		/// </summary>
		public const string Using = "using";

		/// <summary>
		/// virtual
		/// </summary>
		public const string Virtual = "virtual";

		/// <summary>
		/// void
		/// </summary>
		public const string Void = "void";

		/// <summary>
		/// volatile
		/// </summary>
		public const string Volatile = "volatile";

		/// <summary>
		/// where
		/// </summary>
		public const string Where = "where";

		#endregion Constants

		#region Static Fields

		/// <summary>
		/// new()
		/// </summary>
		public static readonly string NewConstraint = New + 
		    CSharpSymbol.BeginParameterList + CSharpSymbol.EndParameterList;

		#endregion Static Fields
	}
}