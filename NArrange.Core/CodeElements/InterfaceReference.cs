using System;

namespace NArrange.Core.CodeElements
{
	/// <summary>
	/// Interface implementation definition for type and member references to interfaces and 
	/// interface members.
	/// </summary>
	public sealed class InterfaceReference : ICloneable
	{
		#region Fields

		private string _name;
		private InterfaceReferenceType _referenceType;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new InterfaceImplementation.
		/// </summary>
		public InterfaceReference()
		{
		}

		/// <summary>
		/// Creates a new InterfaceImplementation with the specified name and reference type.
		/// </summary>
		public InterfaceReference(string name, InterfaceReferenceType referenceType)
		{
			_name = name;
			_referenceType = referenceType;
		}

		#endregion Constructors

		#region Public Properties

		/// <summary>
		/// Gets or sets the referenced interface name.
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
		/// Gets or sets the interface reference type.
		/// </summary>
		public InterfaceReferenceType ReferenceType
		{
			get
			{
			    return _referenceType;
			}
			set
			{
			    _referenceType = value;
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
			InterfaceReference clone = new InterfaceReference();

			//
			// Copy state
			//
			clone._name = _name;
			clone._referenceType = _referenceType;

			return clone;
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
	}
}