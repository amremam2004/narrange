using System;

namespace SampleNamespace
{
    /// <summary>
    /// Class with operators
    /// </summary>
    public class Fraction
    {
        int num, den;
        public Fraction(int num, int den)
        {
            this.num = num;
            this.den = den;
        }

        // overload operator +
        public static Fraction operator +(Fraction a, Fraction b)
        {
            return new Fraction(a.num * b.den + b.num * a.den, a.den * b.den);
        }

        // overload operator *
        public static Fraction operator *(Fraction a, Fraction b)
        {
            return new Fraction(a.num * b.num, a.den * b.den);
        }

		// overload operator ==
		public static bool operator ==(Fraction a, Fraction b)
		{
			return a.num == b.num && a.den == b.den;
		}

		// overload operator !=
		public static bool operator !=(Fraction a, Fraction b)
		{
			return !(a == b);
		}

        // define operator double
        public static implicit operator double(Fraction f)
        {
            return (double)f.num / f.den;
        }

        // define operator decimal
        public static explicit operator decimal(Fraction f)
        {
            return (decimal)f.num / f.den;
        }
    }
}
