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
 *      - Added a TabStyle enumeration
 *		- Added an enumeration for whitespace characters
 *      - Added an enumeration for interface implementation types
 *      - Added FileAttributeType and UnaryExpressionOperator enumerations
 *      - Added a Matches binary operator for regular expression support in 
 *        condition expressions
 *		- Added a HandlerType enumeration
 *      Everton Elvio Koser
 *      - Added TypeModifiers.New (merged by James Nies)
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

using System;
using System.Collections.Generic;
using System.Text;

namespace NArrange.Core
{
	#region Enumerations

	/// <summary>
	/// Binary expression operator
	/// </summary>
	public enum BinaryExpressionOperator
	{
		/// <summary>
		/// Equality
		/// </summary>
		Equal,

		/// <summary>
		/// Not equal
		/// </summary>
		NotEqual,

		/// <summary>
		/// Contains
		/// </summary>
		Contains,

		/// <summary>
		/// Matches
		/// </summary>
		Matches,

		/// <summary>
		/// And
		/// </summary>
		And,

		/// <summary>
		/// Or
		/// </summary>
		Or
	}

	/// <summary>
	/// Code access level
	/// </summary>
	[Flags]
	public enum CodeAccess
	{
		/// <summary>
		/// None/Not specified
		/// </summary>
		None = 0,

		/// <summary>
		/// Private
		/// </summary>
		Private = 1,

		/// <summary>
		/// Protected/family
		/// </summary>
		Protected = 2,

		/// <summary>
		/// Internal/assembly
		/// </summary>
		Internal = 4,

		/// <summary>
		/// Public
		/// </summary>
		Public = 8
	}

	/// <summary>
	/// Comment type
	/// </summary>
	public enum CommentType
	{
		/// <summary>
		/// Single line comment
		/// </summary>
		Line,

		/// <summary>
		/// Single line XML comment
		/// </summary>
		XmlLine,

		/// <summary>
		/// Block comment
		/// </summary>
		Block
	}

	/// <summary>
	/// Element attribute scope.
	/// </summary>
	public enum ElementAttributeScope
	{
		/// <summary>
		/// Element
		/// </summary>
		Element,

		/// <summary>
		/// Parent
		/// </summary>
		Parent
	}

	/// <summary>
	/// Element attribute
	/// </summary>
	public enum ElementAttributeType
	{
		/// <summary>
		/// None
		/// </summary>
		None,

		/// <summary>
		/// Name
		/// </summary>
		Name,

		/// <summary>
		/// Access
		/// </summary>
		Access,

		/// <summary>
		/// Modifier
		/// </summary>
		Modifier,

		/// <summary>
		/// Element Type
		/// </summary>
		ElementType,

		/// <summary>
		/// Type
		/// </summary>
		Type,

		/// <summary>
		/// Attributes
		/// </summary>
		Attributes
	}

	/// <summary>
	/// Element type
	/// </summary>
	public enum ElementType
	{
		/// <summary>
		/// Not specified
		/// </summary>
		NotSpecified,

		/// <summary>
		/// Comment
		/// </summary>
		Comment,

		/// <summary>
		/// Attribute
		/// </summary>
		Attribute,

		/// <summary>
		/// Using statement
		/// </summary>
		Using,

		/// <summary>
		/// Namespace
		/// </summary>
		Namespace,

		/// <summary>
		/// Region
		/// </summary>
		Region,

		/// <summary>
		/// Field
		/// </summary>
		Field,

		/// <summary>
		/// Constructor
		/// </summary>
		Constructor,

		/// <summary>
		/// Property
		/// </summary>
		Property,

		/// <summary>
		/// Method
		/// </summary>
		Method,

		/// <summary>
		/// Event
		/// </summary>
		Event,

		/// <summary>
		/// Delegate
		/// </summary>
		Delegate,

		/// <summary>
		/// Type
		/// </summary>
		Type,
	}

	/// <summary>
	/// File attribute
	/// </summary>
	public enum FileAttributeType
	{
		/// <summary>
		/// None
		/// </summary>
		None,

		/// <summary>
		/// Name
		/// </summary>
		Name,

		/// <summary>
		/// Path
		/// </summary>
		Path,

		/// <summary>
		/// Attributes
		/// </summary>
		Attributes
	}

	/// <summary>
	/// Grouping separator type
	/// </summary>
	public enum GroupSeparatorType
	{
		/// <summary>
		/// New line
		/// </summary>
		NewLine,

		/// <summary>
		/// Custom separator string
		/// </summary>
		Custom
	}

	/// <summary>
	/// Handler type.
	/// </summary>
	public enum HandlerType
	{
		/// <summary>
		/// Source handler.
		/// </summary>
		Source,

		/// <summary>
		/// Project handler.
		/// </summary>
		Project
	}

	/// <summary>
	/// Enumeration for interface impelementation types.
	/// </summary>
	public enum InterfaceReferenceType
	{
		/// <summary>
		/// None/Unknown
		/// </summary>
		None,

		/// <summary>
		/// Base class implementation
		/// </summary>
		Class,

		/// <summary>
		/// Interface implementation
		/// </summary>
		Interface
	}

	/// <summary>
	/// Log level
	/// </summary>
	public enum LogLevel
	{
		/// <summary>
		/// Error message
		/// </summary>
		Error,

		/// <summary>
		/// Warning message
		/// </summary>
		Warning,

		/// <summary>
		/// Informational message
		/// </summary>
		Info,

		/// <summary>
		/// Verbose
		/// </summary>
		Verbose,

		/// <summary>
		/// Trace
		/// </summary>
		Trace
	}

	/// <summary>
	/// Member attributes
	/// </summary>
	[Flags]
	public enum MemberModifiers
	{
		/// <summary>
		/// None
		/// </summary>
		None = 0,

		/// <summary>
		/// Abstract
		/// </summary>
		Abstract = 1,

		/// <summary>
		/// Sealed
		/// </summary>
		Sealed = 2,

		/// <summary>
		/// Static
		/// </summary>
		Static = 4,

		/// <summary>
		/// Unsafe
		/// </summary>
		Unsafe = 8,

		/// <summary>
		/// Virtual
		/// </summary>
		Virtual = 16,

		/// <summary>
		/// Override
		/// </summary>
		Override = 32,

		/// <summary>
		/// New
		/// </summary>
		New = 64,

		/// <summary>
		/// ReadOnly
		/// </summary>
		ReadOnly = 128,

		/// <summary>
		/// Constant
		/// </summary>
		Constant = 256,

		/// <summary>
		/// External
		/// </summary>
		External = 512,

		/// <summary>
		/// Partial
		/// </summary>
		Partial = 1024
	}

	/// <summary>
	/// Operator type
	/// </summary>
	public enum OperatorType
	{
		/// <summary>
		/// None/Not specified
		/// </summary>
		None = 0,

		/// <summary>
		/// Explicit
		/// </summary>
		Explicit = 1,

		/// <summary>
		/// Implicit
		/// </summary>
		Implicit = 2
	}

	/// <summary>
	/// Tabbing style
	/// </summary>
	public enum TabStyle
	{
		/// <summary>
		/// Use tabs when writing elements
		/// </summary>
		Tabs,

		/// <summary>
		/// Uses spaces when writing elements
		/// </summary>
		Spaces
	}

	/// <summary>
	/// Type element type
	/// </summary>
	public enum TypeElementType
	{
		/// <summary>
		/// Class
		/// </summary>
		Class,

		/// <summary>
		/// Structure
		/// </summary>
		Structure,

		/// <summary>
		/// Interface
		/// </summary>
		Interface,

		/// <summary>
		/// Enumeration
		/// </summary>
		Enum,

		/// <summary>
		/// Module
		/// </summary>
		Module
	}

	/// <summary>
	/// Type attributes
	/// </summary>
	/// <remarks>This is a subset of member attributes that apply to types.</remarks>
	[Flags]
	public enum TypeModifiers
	{
		/// <summary>
		/// None
		/// </summary>
		None = MemberModifiers.None,

		/// <summary>
		/// Abstract
		/// </summary>
		Abstract = MemberModifiers.Abstract,

		/// <summary>
		/// Sealed
		/// </summary>
		Sealed = MemberModifiers.Sealed,

		/// <summary>
		/// Static
		/// </summary>
		Static = MemberModifiers.Static,

		/// <summary>
		/// Unsafe
		/// </summary>
		Unsafe = MemberModifiers.Unsafe,

		/// <summary>
		/// Partial
		/// </summary>
		Partial = MemberModifiers.Partial,

		/// <summary>
		/// New
		/// </summary>
		New = MemberModifiers.New
	}

	/// <summary>
	/// Unary expression operator
	/// </summary>
	public enum UnaryExpressionOperator
	{
		/// <summary>
		/// Negate
		/// </summary>
		Negate
	}

	/// <summary>
	/// Whitespace character types
	/// </summary>
	[Flags]
	public enum WhiteSpaceTypes
	{
		/// <summary>
		/// None
		/// </summary>
		None = 0,

		/// <summary>
		/// Spaces
		/// </summary>
		Space = 1,

		/// <summary>
		/// Tabs
		/// </summary>
		Tab = 2,

		/// <summary>
		/// Carriage returns
		/// </summary>
		CarriageReturn = 4,

		/// <summary>
		/// Line feeds
		/// </summary>
		Linefeed = 8,

		/// <summary>
		/// Spaces and tabs
		/// </summary>
		SpaceAndTab = Space | Tab,

		/// <summary>
		/// Carriage returns and line feeds
		/// </summary>
		CarriageReturnAndLinefeed = CarriageReturn | Linefeed,

		/// <summary>
		/// All whitespace characters
		/// </summary>
		All = SpaceAndTab | CarriageReturnAndLinefeed
	}

	#endregion Enumerations
}