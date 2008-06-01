#region Header

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

#endregion Header

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

using NArrange.Core.Configuration;

namespace NArrange.Gui.Configuration
{
	/// <summary>
	/// Collection editor for configuration element collections.
	/// </summary>
	public sealed class ConfigurationElementCollectionEditor : CollectionEditor
	{
		#region Static Fields

		/// <summary>
		/// Item types supported by the editor.
		/// </summary>
		public static Type[] ItemTypes = new Type[]
			{
				typeof(ElementConfiguration),
				typeof(ElementReferenceConfiguration),
				typeof(RegionConfiguration)
			};

		#endregion Static Fields

		#region Constructors

		/// <summary>
		/// Creates a new ConfigurationElementCollectionEditor.
		/// </summary>
		/// <param name="type"></param>
		public ConfigurationElementCollectionEditor(Type type)
			: base(type)
		{
		}

		#endregion Constructors

		#region Protected Methods

		/// <summary>
		/// Creates a new instance of the specified type.
		/// </summary>
		/// <param name="itemType"></param>
		/// <returns></returns>
		protected override object CreateInstance(Type itemType)
		{
			//
			// Create a new instance of the specified type.
			//
			if (itemType == typeof(ElementConfiguration))
			{
				return new ElementConfiguration();
			}
			else if (itemType == typeof(RegionConfiguration))
			{
				return new RegionConfiguration();
			}
			else if (itemType == typeof(ElementReferenceConfiguration))
			{
				return new ElementReferenceConfiguration();
			}
			else
			{
				return base.CreateInstance(itemType);
			}
		}

		/// <summary>
		/// Gets the list of types that can be created for the collection.
		/// </summary>
		/// <returns></returns>
		protected override Type[] CreateNewItemTypes()
		{
			return ItemTypes;
		}

		#endregion Protected Methods
	}
}