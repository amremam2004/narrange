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
using System.Text;

namespace NArrange.Core.CodeElements
{
	/// <summary>
	/// Class/struct code element
	/// </summary>
	public class TypeElement : AttributedElement
	{
		#region Fields

		private object _interacesLock = new object();		
		private List<string> _interfaces;		
		private TypeElementType _type;		
		private TypeModifier _typeModifiers;		
		private List<TypeParameter> _typeParameters;		
		private object _typeParametersLock = new object();		
		
		#endregion Fields

		#region Protected Properties

		/// <summary>
		/// List of implemented interface names
		/// </summary>
		protected List<string> BaseInterfaces
		{
			get
			{
			    if (_interfaces == null)
			    {
			        lock (_interacesLock)
			        {
			            if (_interfaces == null)
			            {
			                _interfaces = new List<string>();
			            }
			        }
			    }
			
			    return _interfaces;
			}
		}

		#endregion Protected Properties

		#region Public Properties

		/// <summary>
		/// Gets the element type
		/// </summary>
		public override ElementType ElementType
		{
			get
			{
			    return ElementType.Type;
			}
		}

		/// <summary>
		/// Gets the collection of implemented interface names for the type
		/// definition.
		/// </summary>
		public ReadOnlyCollection<string> Interfaces
		{
			get
			{
			    return BaseInterfaces.AsReadOnly();
			}
		}

		/// <summary>
		/// Gets whether or not the type is abstract
		/// </summary>
		public bool IsAbstract
		{
			get
			{
			    return (_typeModifiers & TypeModifier.Abstract) == TypeModifier.Abstract;
			}
		}

		/// <summary>
		/// Gets whether or not the type is a partial class
		/// </summary>
		public bool IsPartial
		{
			get
			{
			    return (_typeModifiers & TypeModifier.Partial) == TypeModifier.Partial;
			}
		}

		/// <summary>
		/// Gets whether or not the type is sealed
		/// </summary>
		public bool IsSealed
		{
			get
			{
			    return (_typeModifiers & TypeModifier.Sealed) == TypeModifier.Sealed;
			}
		}

		/// <summary>
		/// Gets whether or not the type is static
		/// </summary>
		public bool IsStatic
		{
			get
			{
			    return (_typeModifiers & TypeModifier.Static) == TypeModifier.Static;
			}
		}

		/// <summary>
		/// Gets whether or not the type is unsafe
		/// </summary>
		public bool IsUnsafe
		{
			get
			{
			    return (_typeModifiers & TypeModifier.Unsafe) == TypeModifier.Unsafe;
			}
		}

		/// <summary>
		/// Gets or sets the type of the type element
		/// </summary>
		public TypeElementType Type
		{
			get
			{
			    return _type;
			}
			set
			{
			    _type = value;
			}
		}

		/// <summary>
		/// Gets or sets the type attributes
		/// </summary>
		public TypeModifier TypeModifiers
		{
			get
			{
			    return _typeModifiers;
			}
			set
			{
			    _typeModifiers = value;
			}
		}

		/// <summary>
		/// List of type parameters
		/// </summary>
		public List<TypeParameter> TypeParameters
		{
			get
			{
			    if (_typeParameters == null)
			    {
			        lock (_typeParametersLock)
			        {
			            if (_typeParameters == null)
			            {
			                _typeParameters = new List<TypeParameter>();
			            }
			        }
			    }
			
			    return _typeParameters;
			}
		}

		#endregion Public Properties

		#region Protected Methods

		/// <summary>
		/// Clones an attributed element
		/// </summary>
		/// <returns></returns>
		protected override AttributedElement DoAttributedClone()
		{
			TypeElement clone = new TypeElement();
			
			//
			// Copy state
			//
			clone._typeModifiers = _typeModifiers;
			clone._type = _type;
			foreach (string interfaceName in Interfaces)
			{
			    clone.AddInterface(interfaceName);
			}
			foreach (TypeParameter typeParam in TypeParameters)
			{
			    TypeParameter typeParamClone = typeParam.Clone() as TypeParameter;
			    clone.TypeParameters.Add(typeParamClone);
			}
			
			return clone;
		}

		#endregion Protected Methods

		#region Public Methods

		/// <summary>
		/// Allows an ICodeElementVisitor to process (or visit) this element.
		/// </summary>
		/// <remarks>See the Gang of Four Visitor design pattern.</remarks>
		/// <param name="visitor"></param>
		public override void Accept(ICodeElementVisitor visitor)
		{
			visitor.VisitTypeElement(this);
		}

		/// <summary>
		/// Adds an interface implementation to the type definition.
		/// </summary>
		/// <param name="interfaceName"></param>
		public void AddInterface(string interfaceName)
		{
			BaseInterfaces.Add(interfaceName);
		}

		#endregion Public Methods
	}
}