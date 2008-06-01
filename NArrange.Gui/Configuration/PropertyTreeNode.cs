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
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace NArrange.Gui.Configuration
{
	/// <summary>
	/// Tree node that represents a component property.
	/// </summary>
	public class PropertyTreeNode : TreeNode
	{
		#region Fields

		private ToolStripMenuItem _addMenuItem;
		private object _component;
		private ContextMenuStrip _contextMenu;
		private PropertyDescriptor _property;
		private ToolStripMenuItem _removeMenuItem;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new PropertyTreeNode.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="component"></param>
		public PropertyTreeNode(PropertyDescriptor property, object component)
		{
			_property = property;
			this.Text = _property.DisplayName;

			_component = component;

			this.Initialize();
		}

		#endregion Constructors

		#region Public Properties

		/// <summary>
		/// Gets the component that the property is associated with.
		/// </summary>
		public object Component
		{
			get
			{
				return _component;
			}
		}

		/// <summary>
		/// Gets or sets the component property's value.
		/// </summary>
		public object PropertyValue
		{
			get
			{
				return  _property.GetValue(_component);
			}
			set
			{
				if (value != this.PropertyValue)
				{
					_property.SetValue(_component, value);
					if (!_property.IsReadOnly)
					{
						_addMenuItem.Enabled = value == null;
						_removeMenuItem.Enabled = value != null;
					}

					this.OnPropertyValueChanged();
				}
			}
		}

		#endregion Public Properties

		#region Private Methods

		/// <summary>
		/// Clears the property value.
		/// </summary>
		private void ClearProperty()
		{
			this.PropertyValue = null;
		}

		/// <summary>
		/// Initializes the property value with a new instance.
		/// </summary>
		private void CreateProperty()
		{
			this.PropertyValue = Activator.CreateInstance(_property.PropertyType);
		}

		/// <summary>
		/// Event handler for the Add menu item click event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void HandleAddMenuItemClick(object sender, EventArgs e)
		{
			this.CreateProperty();
		}

		/// <summary>
		/// Event handler for the Remove menu item click event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void HandleRemoveMenuItemClick(object sender, EventArgs e)
		{
			this.ClearProperty();
		}

		/// <summary>
		/// Initializes the node.
		/// </summary>
		private void Initialize()
		{
			_contextMenu = new ContextMenuStrip();

			if (!_property.IsReadOnly)
			{
				_removeMenuItem = new ToolStripMenuItem("&Remove");
				_removeMenuItem.Click += new EventHandler(HandleRemoveMenuItemClick);
				_removeMenuItem.Enabled = this.PropertyValue != null;
				_contextMenu.Items.Add(_removeMenuItem);

				_addMenuItem = new ToolStripMenuItem("&New");
				_addMenuItem.Click += new EventHandler(HandleAddMenuItemClick);
				_addMenuItem.Enabled = this.PropertyValue == null;
				_contextMenu.Items.Add(_addMenuItem);
			}

			this.ContextMenuStrip = _contextMenu;
		}

		/// <summary>
		/// Called when the property value changes.
		/// </summary>
		private void OnPropertyValueChanged()
		{
			EventHandler temp = PropertyValueChanged;
			if (temp != null)
			{
				temp(this, new EventArgs());
			}
		}

		#endregion Private Methods

		#region Public Methods

		/// <summary>
		/// Gets the string represenation of this object.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _property.DisplayName;
		}

		#endregion Public Methods

		#region Events

		/// <summary>
		/// Occurs when the PropertyValue changes.
		/// </summary>
		public event EventHandler PropertyValueChanged;

		#endregion Events
	}
}