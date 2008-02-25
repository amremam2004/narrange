using System.Text;
using System;
using System.Runtime;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using System.Reflection;

[assembly: AssemblyDescription("Sample assembly for C# parsing and writing.")]

namespace SampleNamespace
{
    // Using statement within the namespace definition
    using System.ComponentModel;

    /// <summary>
    /// This is a class definition.
    /// </summary>
    public class SampleClass	// Extra comment here
    {
        #region Fields

        //This field has a regular comment
        private bool _simpleField;
        private int _fieldWithInitialVal = 1;
        
        /// <summary>
        /// This is a static readonly string
        /// </summary>
        protected static readonly string StaticStr = "static; string;";

        private Nullable<int> _genericField;
        protected internal string[] _arrayField = { };
        internal bool @internal;
        private global::System.Boolean _globalNamespaceTypeField;

        /// <summary>
        /// This field has an attribute
        /// </summary>
        [ThreadStatic]
        private static string _attributedField = 
            // Semi here...;
            null;
        
        public const string ConstantStr = /*semi here...;*/ "constant string" ;

        private volatile int _volatileField;

        private int _val1, _val2;
        private int _val3 ,  _val4,_val5,
           _val6 = 10;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Instance constructor
        /// </summary>
        public SampleClass()
        {
        }

        /// <summary>
        /// Internal constructor with params
        /// </summary>
        /// <param name="arrayParam"></param>
        internal SampleClass(string[] arrayParam)
        {
        }

        // Static constructor
        static SampleClass()
        {
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        /// <remarks></remarks>
        ~SampleClass()
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Simple property
        /// </summary>
        public bool SimpleProperty
        {
            get
            {
                return _simpleField;
            }
            set
            {
                _simpleField = value;
            }
        }

        //
        // This is a protected property.  Also virtual.
        //
        protected virtual int ProtectedProperty
        {
            get
            {
                return _fieldWithInitialVal;
            }
        }

        /// <summary>
        /// This property is static
        /// </summary>
        // Mixed comment style here
        public static string StaticProperty		// Extra comment here
        {
            get
            {
                return StaticStr + " is returned.";
            }
        }

        // This property has multiple attributes and different
        // ordering for the static specification.
        [Obsolete("This property has attributes.")]
        [Description("Multiple attribute property.")]
        static public string AttributedProperty
        {
            get
            {
                return _attributedField;
            }
        }

        [Obsolete]  
        /// <summary>
        /// Generic property.  This comment has extra whitespace
        /// before the member, but should still be considered header comments.
        /// This member also has an attribute before the comment
        /// that should still be matched to the property.
        /// </summary>
        
        internal Nullable<int> GenericProperty
        {
            get
            {
                return _genericField;
            }
            set
            {
                _genericField = value;
                if (_genericField.HasValue)
                {
                    @internal = true;
                }
            }
        }

        /// <summary>
        /// This property returns an array
        /// </summary>
        public new string[] ArrayProperty
        {
            get
            {
                return _arrayField;
            }
        }

        /// <summary>
        /// Indexer property
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string this[int index]
        {
            get
            {
                return _arrayField[index];
            }
            set
            {
                _arrayField[index] = value;
            }
        }

        #endregion Properties

        #region Methods

        // Simple method
        public void DoSomething()	// Extra comment here
        {
            // 
            // Make sure we detect that we're in a string while 
            // parsing the following line.
            //
            Console.WriteLine("}");
        }

        /* 
         * Block comment here
         */
        public override string ToString()
        {
            /* This comment has a block end character in it. } */
            return "SampleClass";
        }

        /// <summary>
        /// Simple method with params and a return value
        /// </summary>
        /// <param name="intParam"></param>
        /// <param name="stringParam"></param>
        /// <returns></returns>
        private bool GetBoolValue(int intParam, string stringParam)
		/*
		 * Extra block comment here
		 */
        {
            return true;
        }

        /// <summary>
        /// This method has parameter attributes
        /// </summary>
        /// <param name="intParam">Int parameter</param>
        /// <returns></returns>
        [Description("Method with parameter attributes")]
		internal static int? GetWithParamAttributes(			// Extra comment 1 here
            [Description("Int parameter")] int intParam,		// Extra comment 2 here
            [Description("String parameter")] string stringParam)
        {
            if (intParam == 0)
            {
                return null;
            }
            else
            {
                return intParam;
            }
        }

        /// <summary>
        /// This method has parameter attributes
        /// </summary>
        public bool GetWithTypeParameters<T1,T2>(
            Action<T1> typeParam1, Action<T2> typeParam2) 
            where T1 : IDisposable, new()		// Extra comment here
            where T2 : IDisposable, new()
        {
            try
            {
                typeParam1(new T1());
                typeParam2(new T2());
                return true;
            }
            catch(InvalidOperationException)
            {
                return false;
            }
        }

        [DllImport("User32.dll")]
        public static extern int MessageBox(int h, string m, string c, int type);

        /// <summary>
        /// Unsafe method
        /// </summary>
        /// <param name="p"></param>
        unsafe static void UnsafeSqrPtrParam(int* p)
        {
            *p *= *p;
        }

        #endregion Methods

        #region Delegates

        /// <summary>
        /// Sample delegate definition
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="boolParam"></param>
        public delegate void SampleEventHandler(object sender, bool boolParam);

		/// <summary>
		/// Generic delegate
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		/// <returns></returns>
		private delegate int Compare<T>(T t1, T t2) where T : class;

        #endregion Delegates

        #region Events

        /// <summary>
        /// Simple event
        /// </summary>
        public event SampleEventHandler SimpleEvent;

        /// <summary>
        /// Generic event
        /// </summary>
        public event EventHandler<EventArgs> GenericEvent;

        /// <summary>
        /// Explicit event
        /// </summary>
        public event SampleEventHandler ExplicitEvent
        {
            add
            {
            }
            remove
            {
            }
        }

        #endregion Events

        #region Nested Types

        /// <summary>
        /// Sample enumeration
        /// </summary>
        [Flags]
        private enum SampleEnum
        {
            None = 0,
            Some = 1,
            More = 2
        }

        /// <summary>
        /// Nested structure
        /// </summary>
        public struct SampleStructure
        {
            public readonly string Name;

            /// <summary>
            /// Creates a new SampleStructure
            /// </summary>
            public SampleStructure(string name)
            {
                Name = name;
            }
        }

        /// <summary>
        /// Nested class
        /// </summary>
        private class SampleNestedClass<T> where T : new()
        {
            /// <summary>
            /// Creates a new SampleNestedClass
            /// </summary>
            public SampleNestedClass()
            {
            }
        }

        /// <summary>
        /// Sample nested, static class
        /// </summary>
        internal static class SampleNestedStaticClass
        {
            public static bool DoSomething(string stringParam)
            {
                return true;
            }
        }

        #endregion Nested Types
    }
}