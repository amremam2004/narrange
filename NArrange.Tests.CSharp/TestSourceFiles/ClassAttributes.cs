using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SampleNamespace
{
    [Obsolete("This attribute is above the header comments.")]

    /// <summary>
    /// This is a sample class definition.
    /// </summary>
    [Serializable]
    [ComVisible(true)]
    public class SampleClass
    {
    }
}