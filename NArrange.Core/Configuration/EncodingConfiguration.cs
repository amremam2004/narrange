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
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace NArrange.Core.Configuration
{
	/// <summary>
	/// Specifies encoding configuration.
	/// </summary>
	[XmlType("Encoding")]
	public class EncodingConfiguration : ICloneable
	{
		#region Static Fields

		/// <summary>
		/// CodePage value to indicate the system ANSI default.
		/// </summary>
		public static string DefaultCodePage = "Default";

		/// <summary>
		/// CodePage value to indicate auto-detection.
		/// </summary>
		public static string DetectCodePage = "Detect";

		#endregion Static Fields

		#region Fields

		private string _codePage;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new EncodingConfiguration instance.
		/// </summary>
		public EncodingConfiguration()
		{
			_codePage = DetectCodePage;
		}

		#endregion Constructors

		#region Public Properties

		/// <summary>
		/// Gets or sets the CodePage.
		/// </summary>
		[XmlAttribute("CodePage")]
		public string CodePage
		{
			get
			{
			    return _codePage;
			}
			set
			{
			    _codePage = value;
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
			EncodingConfiguration clone = new EncodingConfiguration();

			clone._codePage = _codePage;

			return clone;
		}

		/// <summary>
		/// Gets the encoding (null if Detect) specified by the configuration.
		/// </summary>
		/// <returns></returns>
		public Encoding GetEncoding()
		{
			Encoding encoding = null;
			string codePage = this.CodePage;
			if (!(string.IsNullOrEmpty(codePage) || codePage.Trim().Length == 0 ||
				codePage.ToUpperInvariant() == DetectCodePage.ToUpperInvariant()))
			{
				if (codePage.ToUpperInvariant() == DefaultCodePage.ToUpperInvariant())
				{
					encoding = Encoding.Default;
				}
				else
				{
					int codePageInt;
					if (int.TryParse(codePage, out codePageInt))
					{
						encoding = Encoding.GetEncoding(codePageInt);
					}
					else
					{
						throw new FormatException(
							string.Format(CultureInfo.CurrentCulture,
							"Invalid code page '{0}'.", codePage));
					}
				}
			}

			return encoding;
		}

		/// <summary>
		/// Gets the string representation of this instance.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(Thread.CurrentThread.CurrentCulture,
			    "Encoding: CodePage - {0}", this.CodePage);
		}

		#endregion Public Methods
	}
}