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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using NArrange.Core.Configuration;

namespace NArrange.Gui.Configuration
{
	/// <summary>
	/// Control for editing a code configuration.
	/// </summary>
	public partial class ConfigurationEditorControl : UserControl
	{
		#region Fields

		private CodeConfiguration _configuration;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Static constructor.
		/// </summary>
		static ConfigurationEditorControl()
		{
			//
			// Register the type descriptor provider for configuration elements.
			//
			TypeDescriptor.AddProvider(
				new ConfigurationElementTypeDescriptionProvider(typeof(ConfigurationElement)),
				typeof(ConfigurationElement));
		}

		/// <summary>
		/// Creates a new ConfigurationEditorControl.
		/// </summary>
		public ConfigurationEditorControl()
		{
			InitializeComponent();
		}

		#endregion Constructors

		#region Public Properties

		/// <summary>
		/// Gets or sets the CodeConfiguration to edit.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CodeConfiguration Configuration
		{
			get
			{
				return _configuration;
			}
			set
			{
				_configuration = value;
				this.RefreshConfiguration();
				if (this._configurationTreeView.TopNode != null)
				{
					this._configurationTreeView.TopNode.Expand();
				}
			}
		}

		#endregion Public Properties

		#region Private Methods

		/// <summary>
		/// Adds child tree view nodes under the specified root node using data
		/// from the configuration object.
		/// </summary>
		/// <param name="rootNode"></param>
		/// <param name="_configurationObject"></param>
		private void AddChildTreeNodes(TreeNode rootNode, object _configurationObject)
		{
			if (_configurationObject != null)
			{
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(_configurationObject);
				for(int propertyIndex = 0; propertyIndex < properties.Count; propertyIndex++)
				{
					PropertyDescriptor property = properties[propertyIndex];

					if (property.IsBrowsable &&
						!(property.PropertyType.IsValueType || property.PropertyType == typeof(string)))
					{
						object childPropertyValue = property.GetValue(_configurationObject);
						IList childList = childPropertyValue as IList;

						TreeNode childNode;

						if (childList != null)
						{
							childNode = CreateListPropertyNode(property, _configurationObject);
							IBindingList childBindingList = childList as IBindingList;
							if (childBindingList != null)
							{
								childBindingList.ListChanged += delegate(object sender, ListChangedEventArgs e)
								{
									RefreshListTreeNodes(childNode, property, _configurationObject, childBindingList);
								};
							}

							AddListTreeNodes(childNode, property, _configurationObject, childBindingList);
						}
						else
						{
							childNode = CreatePropertyNode(property, _configurationObject);
							AddChildTreeNodes(childNode, childPropertyValue);
						}

						rootNode.Nodes.Add(childNode);
					}
				}
			}
		}

		/// <summary>
		/// Adds nodes for a list.
		/// </summary>
		/// <param name="listNode"></param>
		/// <param name="listProperty"></param>
		/// <param name="component"></param>
		/// <param name="childList"></param>
		private void AddListTreeNodes(
			TreeNode listNode, PropertyDescriptor listProperty,
			object component, IList childList)
		{
			foreach (object listItem in childList)
			{
				TreeNode listItemNode = new ListItemTreeNode(listProperty, component, listItem);
				listNode.Nodes.Add(listItemNode);

				AddChildTreeNodes(listItemNode, listItem);
			}
		}

		/// <summary>
		/// Event handler for the tree view KeyDown event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ConfigurationTreeViewKeyDownHandler(object sender, KeyEventArgs e)
		{
			ListItemTreeNode listNode = this._configurationTreeView.SelectedNode as ListItemTreeNode;
			if (listNode != null)
			{
				if (e.Control)
				{
					if (e.KeyCode == Keys.Up)
					{
						//
						// Move the list item up
						//
						listNode.MoveUp();
						e.Handled = true;
					}
					else if (e.KeyCode == Keys.Down)
					{
						//
						// Move the list item down
						//
						listNode.MoveDown();
						e.Handled = true;
					}
				}
				else if (e.KeyCode == Keys.Delete)
				{
					//
					// Delete the list item
					//
					listNode.RemoveItem();
					e.Handled = true;
				}
			}
		}

		/// <summary>
		/// Creates a node for a list/collection property.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="component"></param>
		/// <returns></returns>
		private TreeNode CreateListPropertyNode(PropertyDescriptor property, object component)
		{
			Type[] newItemTypes = null;
			if (property.PropertyType == typeof(ConfigurationElementCollection))
			{
				newItemTypes = ConfigurationElementCollectionEditor.ItemTypes;
			}
			else if (property.PropertyType == typeof(HandlerConfigurationCollection))
			{
				newItemTypes = new Type[] { typeof(HandlerConfiguration) };
			}
			else if (property.PropertyType == typeof(ExtensionConfigurationCollection))
			{
				newItemTypes = new Type[] { typeof(ExtensionConfiguration) };
			}

			return new ListPropertyTreeNode(property, component, newItemTypes);
		}

		/// <summary>
		/// Creates a node for a regular property.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="component"></param>
		/// <returns></returns>
		private TreeNode CreatePropertyNode(PropertyDescriptor property, object component)
		{
			PropertyTreeNode propertyTreeNode = new PropertyTreeNode(property, component);
			propertyTreeNode.PropertyValueChanged += delegate(object sender, EventArgs e)
			{
				this._propertyGrid.SelectedObject = propertyTreeNode.PropertyValue;
			};

			return propertyTreeNode;
		}

		/// <summary>
		/// Event handler for the property grid PropertyValueChanged event.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="e"></param>
		private void PropertyGridPropertyValueChangedHandler(object s, PropertyValueChangedEventArgs e)
		{
			ListItemTreeNode listItemTreeNode = this._configurationTreeView.SelectedNode as ListItemTreeNode;
			if (listItemTreeNode != null)
			{
				listItemTreeNode.UpdateText();
			}
		}

		/// <summary>
		/// Refreshes the UI based on the current configuration instance.
		/// </summary>
		private void RefreshConfiguration()
		{
			this.RefreshTree();
			this._propertyGrid.SelectedObject = _configuration;
		}

		/// <summary>
		/// Refreshes nodes within a list property.
		/// </summary>
		/// <param name="listNode"></param>
		/// <param name="listProperty"></param>
		/// <param name="component"></param>
		/// <param name="childList"></param>
		private void RefreshListTreeNodes(TreeNode listNode, PropertyDescriptor listProperty, object component, IList childList)
		{
			for(int itemIndex = 0; itemIndex < childList.Count; itemIndex++)
			{
				object listItem = childList[itemIndex];

				ListItemTreeNode listItemNode = null;

				//
				// Look for an existing node for the list item
				//
				for (int nodeIndex = 0; nodeIndex < listNode.Nodes.Count; nodeIndex++)
				{
					ListItemTreeNode listItemNodeCandidate = listNode.Nodes[nodeIndex] as ListItemTreeNode;
					object listItemNodeValue = listItemNodeCandidate.ListItem;

					if (listItemNodeValue.Equals(listItem))
					{
						listItemNode = listItemNodeCandidate;
						break;
					}
				}

				if (listItemNode == null)
				{
					// Create a new node
					listItemNode = new ListItemTreeNode(listProperty, component, listItem);
					listNode.Nodes.Add(listItemNode);
					AddChildTreeNodes(listItemNode, listItem);
				}
				else if(listItemNode.Index != itemIndex)
				{
					// Update the node position
					listNode.Nodes.Remove(listItemNode);
					listNode.Nodes.Insert(itemIndex, listItemNode);
					itemIndex = 0;
				}

				listItemNode.UpdateMenu();
			}

			// Remove nodes that are no longer present in the list
			for (int nodeIndex = childList.Count; nodeIndex < listNode.Nodes.Count; nodeIndex++)
			{
				listNode.Nodes.RemoveAt(nodeIndex);
				nodeIndex--;
			}
		}

		/// <summary>
		/// Refreshes the tree view for the current configuration instance.
		/// </summary>
		private void RefreshTree()
		{
			this._configurationTreeView.Nodes.Clear();

			if (_configuration != null)
			{
				TreeNode rootNode = new TreeNode("Code Configuration");
				rootNode.Tag = _configuration;
				this.AddChildTreeNodes(rootNode, _configuration);

				this._configurationTreeView.Nodes.Add(rootNode);
			}
		}

		/// <summary>
		/// Event handler for the tree view NodeSelect event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TreeNodeSelectHandler(object sender, TreeViewEventArgs e)
		{
			TreeNode selectedNode = _configurationTreeView.SelectedNode;
			_propertyGrid.SelectedObject = null;

			if (selectedNode != null)
			{
				PropertyTreeNode propertyNode = selectedNode as PropertyTreeNode;
				if(propertyNode != null && !(propertyNode is ListPropertyTreeNode))
				{
					_propertyGrid.SelectedObject = propertyNode.PropertyValue;
				}
				else if(selectedNode.Tag != null && !(selectedNode.Tag is IList))
				{
					_propertyGrid.SelectedObject = selectedNode.Tag;
				}
			}
		}

		#endregion Private Methods
	}
}