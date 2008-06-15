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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace NArrange.Gui.Configuration
{
	/// <summary>
	/// Tree node for a list item.
	/// </summary>
	public sealed class ListItemTreeNode : TreeNode
	{
		#region Fields

		private object _component;
		private ContextMenuStrip _contextMenu;
		private object _listItem;
		private PropertyDescriptor _listProperty;
		private ToolStripMenuItem _moveDownMenuItem;
		private ToolStripMenuItem _moveUpMenuItem;
		private ToolStripMenuItem _removeMenuItem;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new ListItemTreeNode.
		/// </summary>
		/// <param name="listProperty"></param>
		/// <param name="component"></param>
		/// <param name="listItem"></param>
		public ListItemTreeNode(PropertyDescriptor listProperty, object component, object listItem)
		{
			_listProperty = listProperty;
			_component = component;
			_listItem = listItem;

			this.Tag = _listItem;

			Initialize();
		}

		#endregion Constructors

		#region Public Properties

		/// <summary>
		/// Gets the list item associated with this node.
		/// </summary>
		public object ListItem
		{
			get
			{
				return _listItem;
			}
		}

		#endregion Public Properties

		#region Private Methods

		/// <summary>
		/// Event handler for the Move Down menu item click event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleMoveDownMenuItemClick(object sender, EventArgs e)
		{
			MoveDown();
		}

		/// <summary>
		/// Event handler for the Move Up menu item click event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleMoveUpMenuItemClick(object sender, EventArgs e)
		{
			MoveUp();
		}

		/// <summary>
		/// Event handler for the Remove menu item click event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleRemoveMenuItemClick(object sender, EventArgs e)
		{
			this.RemoveItem();
		}

		/// <summary>
		/// Initializes this tree node.
		/// </summary>
		private void Initialize()
		{
			this.UpdateText();

			_contextMenu = new ContextMenuStrip();

			_removeMenuItem = new ToolStripMenuItem("&Remove");
			_removeMenuItem.Click += new EventHandler(HandleRemoveMenuItemClick);
			_removeMenuItem.ShortcutKeys = Keys.Delete;
			_contextMenu.Items.Add(_removeMenuItem);

			_moveUpMenuItem = new ToolStripMenuItem("Move &Up");
			_moveUpMenuItem.Click += new EventHandler(HandleMoveUpMenuItemClick);
			_moveUpMenuItem.ShortcutKeys = Keys.Control | Keys.Up;
			_contextMenu.Items.Add(_moveUpMenuItem);

			_moveDownMenuItem = new ToolStripMenuItem("Move &Down");
			_moveDownMenuItem.Click += new EventHandler(HandleMoveDownMenuItemClick);
			_moveDownMenuItem.ShortcutKeys = Keys.Control | Keys.Down;
			_contextMenu.Items.Add(_moveDownMenuItem);

			this.UpdateMenu();

			this.ContextMenuStrip = _contextMenu;
		}

		/// <summary>
		/// Sets this node as the selected node in the tree view.
		/// </summary>
		private void Select()
		{
			if (this.TreeView != null)
			{
				this.TreeView.SelectedNode = this;
			}
		}

		#endregion Private Methods

		#region Public Methods

		/// <summary>
		/// Moves this list item node down in the collection.
		/// </summary>
		public void MoveDown()
		{
			IList list = this._listProperty.GetValue(_component) as IList;
			if (list != null && list.Contains(_listItem))
			{
				int index = list.IndexOf(_listItem);
				if (index < list.Count - 1)
				{
					TreeNode parent = this.Parent;
					if (parent != null)
					{
						parent.Nodes.Remove(this);
					}
					list.Remove(_listItem);

					int newIndex = ++index;
					if (parent != null)
					{
						parent.Nodes.Insert(newIndex, this);
					}
					list.Insert(newIndex, _listItem);
				}

				this.Select();
				this.UpdateMenu();
			}
		}

		/// <summary>
		/// Moves this list item node down in the collection.
		/// </summary>
		public void MoveUp()
		{
			IList list = this._listProperty.GetValue(_component) as IList;
			if (list != null && list.Contains(_listItem))
			{
				int index = list.IndexOf(_listItem);
				if (index > 0)
				{
					TreeNode parent = this.Parent;
					if (parent != null)
					{
						parent.Nodes.Remove(this);
					}
					list.Remove(_listItem);

					int newIndex = --index;
					if (parent != null)
					{
						parent.Nodes.Insert(newIndex, this);
					}
					list.Insert(newIndex, _listItem);
				}

				this.Select();
				this.UpdateMenu();
			}
		}

		/// <summary>
		/// Removes the item from the collection.
		/// </summary>
		public void RemoveItem()
		{
			IList list = this._listProperty.GetValue(_component) as IList;
			if (list != null && list.Contains(_listItem))
			{
				if (this.Parent != null)
				{
					this.Parent.TreeView.SelectedNode = this.Parent;
				}

				list.Remove(_listItem);
			}
		}

		/// <summary>
		/// Updates the context menu for the tree node.
		/// </summary>
		public void UpdateMenu()
		{
			IList list = this._listProperty.GetValue(_component) as IList;
			if (list != null && list.Contains(_listItem))
			{
				int index = list.IndexOf(_listItem);
				this._moveUpMenuItem.Enabled = index > 0;
				this._moveDownMenuItem.Enabled = index < list.Count - 1;
			}
		}

		/// <summary>
		/// Updates the display text.
		/// </summary>
		public void UpdateText()
		{
			this.Text = _listItem.ToString();
		}

		#endregion Public Methods
	}
}