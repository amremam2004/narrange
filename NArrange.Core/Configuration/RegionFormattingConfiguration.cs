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
 *		- Code cleanup via ReSharper 4.0 (http://www.jetbrains.com/resharper/)
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

using System;
using System.ComponentModel;
using System.Threading;
using System.Xml.Serialization;

namespace NArrange.Core.Configuration
{
	/// <summary>
	/// Specifies region style configuration.
	/// </summary>
	[XmlType("Regions")]
	public class RegionFormattingConfiguration : ICloneable
	{
		#region Fields

		private string _commentDirectiveBeginFormat = " $(Begin) {0}";
		private string _commentDirectiveBeginPattern = @"^\s?\$\(\s?Region\.Begin\s?\)\s?(?<Name>.*)$";
		private string _commentDirectiveEndFormat = " $(End) {0}";
		private string _commentDirectiveEndPattern = @"^\s?\$\(\s?Region\.End\s?\)\s?(?<Name>.*)?$";
		private bool _endRegionNameEnabled;
		private RegionStyle _style;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new RegionsConfiguration instance.
		/// </summary>
		public RegionFormattingConfiguration()
		{
			_endRegionNameEnabled = true;
		}

		#endregion Constructors

		#region Public Properties

		/// <summary>
		/// Format string when comment region directives are used.
		/// </summary>
		[XmlAttribute("CommentDirectiveBeginFormat")]
		[DisplayName("Comment directive begin format")]
		[Description("Format string when comment region directives are used.  Should include a {0} parameter for Name.")]
		public string CommentDirectiveBeginFormat
		{
			get
			{
				return _commentDirectiveBeginFormat;
			}
			set
			{
				_commentDirectiveBeginFormat = value;
			}
		}

		/// <summary>
		/// Regular expression for retrieving comment directive region names.
		/// </summary>
		[XmlAttribute("CommentDirectiveBeginPattern")]
		[DisplayName("Comment directive begin pattern")]
		[Description("Regular expression for retrieving comment directive region names.  Should include the <Name> group.")]
		public string CommentDirectiveBeginPattern
		{
			get
			{
				return _commentDirectiveBeginPattern;
			}
			set
			{
				_commentDirectiveBeginPattern = value;
			}
		}

		/// <summary>
		/// Format string when comment region directives are used.
		/// </summary>
		[XmlAttribute("CommentDirectiveEndFormat")]
		[DisplayName("Comment directive end format")]
		[Description("Format string when comment region directives are used.")]
		public string CommentDirectiveEndFormat
		{
			get
			{
				return _commentDirectiveEndFormat;
			}
			set
			{
				_commentDirectiveEndFormat = value;
			}
		}

		/// <summary>
		/// Regular expression for retrieving comment directive region names.
		/// </summary>
		[XmlAttribute("CommentDirectiveEndPattern")]
		[DisplayName("Comment directive end pattern")]
		[Description("Regular expression for retrieving comment directive region names.  Should include the <Name> group.")]
		public string CommentDirectiveEndPattern
		{
			get
			{
				return _commentDirectiveEndPattern;
			}
			set
			{
				_commentDirectiveEndPattern = value;
			}
		}

		/// <summary>
		/// Gets or sets the number of spaces per tab
		/// </summary>
		[XmlAttribute("EndRegionNameEnabled")]
		[DisplayName("End region name enabled")]
		[Description("Whether or not the region name or region name comment should be included in end region directives.")]
		public bool EndRegionNameEnabled
		{
			get
			{
			    return _endRegionNameEnabled;
			}
			set
			{
			    _endRegionNameEnabled = value;
			}
		}

		/// <summary>
		/// Gets or sets the number of spaces per tab
		/// </summary>
		[XmlAttribute("Style")]
		[DisplayName("Region style")]
		[Description("Controls how regions and their directives are written.")]
		public RegionStyle Style
		{
			get
			{
				return _style;
			}
			set
			{
				_style = value;
			}
		}

		#endregion Public Properties

		#region Public Methods

		/// <summary>
		/// Creates a clone of this instance
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			RegionFormattingConfiguration clone = new RegionFormattingConfiguration();

			clone._style = _style;
			clone._endRegionNameEnabled = _endRegionNameEnabled;
			clone._commentDirectiveBeginFormat = _commentDirectiveBeginFormat;
			clone._commentDirectiveBeginPattern = _commentDirectiveBeginPattern;
			clone._commentDirectiveEndFormat = _commentDirectiveEndFormat;
			clone._commentDirectiveEndPattern = _commentDirectiveEndPattern;

			return clone;
		}

		/// <summary>
		/// Gets the string representation
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(Thread.CurrentThread.CurrentCulture,
			    "Regions: EndRegionNameEnabled - {0}", EndRegionNameEnabled);
		}

		#endregion Public Methods
	}
}