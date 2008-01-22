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
	/// Code element base class.
	/// </summary>
	public abstract class CodeElement : ICodeElement
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		protected CodeElement()
		{
			//
			// Default property values
			//
			_name = string.Empty;
		}

		#endregion Constructors

		#region Fields

		private List<ICodeElement> _children;		
		private object _childrenLock = new object();		
		private string _name;		
		private ICodeElement _parent;		
		
		#endregion Fields

		#region Public Properties

		/// <summary>
		/// Gets the collection of children for this element
		/// </summary>
		public ReadOnlyCollection<ICodeElement> Children
		{
			get
			{
			    return BaseChildren.AsReadOnly();
			}
		}

		/// <summary>
		/// Gets the element type
		/// </summary>
		public abstract ElementType ElementType
		{
			get;
		}

		/// <summary>
		/// Gets or sets the code element name.
		/// </summary>
		public string Name
		{
			get
			{
			    return _name;
			}
			set
			{
			    _name = value;
			}
		}

		/// <summary>
		/// Gets or sets the parent element
		/// </summary>
		public ICodeElement Parent
		{
			get
			{
			    return _parent;
			}
			set
			{
			    if (value != _parent)
			    {
			        if (_parent != null)
			        {
			            _parent.RemoveChild(this);
			        }
			
			        _parent = value;
			        if (_parent != null && !_parent.Children.Contains(this))
			        {
			            _parent.AddChild(this);
			        }
			    }
			}
		}

		#endregion Public Properties

		#region Protected Properties

		/// <summary>
		/// Gets the base child collection
		/// </summary>
		protected List<ICodeElement> BaseChildren
		{
			get
			{
			    if (_children == null)
			    {
			        lock (_childrenLock)
			        {
			            if (_children == null)
			            {
			                _children = new List<ICodeElement>();
			            }
			        }
			    }
			
			    return _children;
			}
		}

		#endregion Protected Properties

		#region Public Methods

		/// <summary>
		/// Allows an ICodeElementVisitor to process (or visit) this element.
		/// </summary>
		/// <remarks>See the Gang of Four Visitor design pattern.</remarks>
		/// <param name="visitor"></param>
		public abstract void Accept(ICodeElementVisitor visitor);

		/// <summary>
		/// Adds a child to this element
		/// </summary>
		/// <param name="childElement"></param>
		public void AddChild(ICodeElement childElement)
		{
			if (childElement != null && !BaseChildren.Contains(childElement))
			{
			    BaseChildren.Add(childElement);
			    childElement.Parent = this;
			}
		}

		/// <summary>
		/// Removes all child elements
		/// </summary>
		public void ClearChildren()
		{
			for (int childIndex = 0; childIndex < Children.Count; childIndex++)
			{
			    ICodeElement child = Children[childIndex];
			    if (child != null && child.Parent != null)
			    {
			        child.Parent = null;
			    }
			}
			
			BaseChildren.Clear();
		}

		/// <summary>
		/// Clones the 
		/// </summary>
		/// <returns></returns>
		public virtual object Clone()
		{
			CodeElement clone = DoClone();
			
			//
			// Copy base state
			//
			clone._name = _name;
			for(int childIndex = 0; childIndex < Children.Count; childIndex++)
			{
			    ICodeElement child = Children[childIndex];
			    ICodeElement childClone = child.Clone() as ICodeElement;
			
			    clone.AddChild(childClone);
			}
			
			return clone;
		}

		/// <summary>
		/// Inserts a child element at the specified index
		/// </summary>
		/// <param name="index"></param>
		/// <param name="codeElement"></param>
		public void InsertChild(int index, ICodeElement codeElement)
		{
			BaseChildren.Insert(index, codeElement);
		}

		/// <summary>
		/// Removes a child from this element
		/// </summary>
		/// <param name="childElement"></param>
		public void RemoveChild(ICodeElement childElement)
		{
			if (childElement != null && BaseChildren.Contains(childElement))
			{
			    BaseChildren.Remove(childElement);
			    childElement.Parent = null;
			}
		}

		/// <summary>
		/// Gets the string representation of this object.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _name;
		}

		#endregion Public Methods

		#region Protected Methods

		/// <summary>
		/// Creates a clone of the instance and assigns any state
		/// </summary>
		/// <returns></returns>
		protected abstract CodeElement DoClone();

		#endregion Protected Methods
	}
}