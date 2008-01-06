using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel;

namespace SampleNamespace
{
    public class SampleClass1
    {
    }

    class SampleClass2 : Exception
    {
    }

    internal class SampleClass3 : List<int>, IDisposable, IComparable
    {
        #region IDisposable Members

        public void Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }

    internal sealed class SampleClass4<T1, T2> : IComparable, IDisposable
        where T1 : IComparable, IConvertible, new()
        where T2 : class, IComparable, IConvertible, new()
    {
        #region IComparable Members

        public int CompareTo(object obj)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }

    internal static class SampleClass5
    {
    }

    static public class SampleClass6
    {
    }

    sealed public class SampleClass7
    {
    }

    public abstract class SampleClass8
    {
    }
}