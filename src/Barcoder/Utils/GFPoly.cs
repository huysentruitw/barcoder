using System;
using System.Linq;

namespace Barcoder.Utils
{
    internal struct GFPoly
    {
        public GaloisField GaloisField;
        public int[] Coefficients;
        
        public GFPoly(GaloisField galoisField, int[] coefficients)
        {
            GaloisField = galoisField;
            Coefficients = coefficients.Length > 1 ? coefficients.SkipWhile(x => x == 0).ToArray() : coefficients;
        }

        public static GFPoly Zero(GaloisField galoisField)
            => new GFPoly(galoisField, new int[] { 0 });

        public static GFPoly MonominalPoly(GaloisField galoisField, int degree, int coefficient)
        {
            if (coefficient == 0) return Zero(galoisField);
            int[] coefficients = new int[degree + 1];
            coefficients[0] = coefficient;
            return new GFPoly(galoisField, coefficients);
        }

        public int Degree() => Coefficients.Length - 1;

        public bool IsZero() => Coefficients[0] == 0;

        public int GetCoefficient(int degree) => Coefficients[Degree() - degree];

        public GFPoly AddOrSubtract(GFPoly other)
        {
            if (IsZero()) return other;
            if (other.IsZero()) return this;
            int[] smallCoefficients = Coefficients;
            int[] largeCoefficients = other.Coefficients;
            if (smallCoefficients.Length > largeCoefficients.Length)
                (largeCoefficients, smallCoefficients) = (smallCoefficients, largeCoefficients);
            int[] sumDiff = new int[largeCoefficients.Length];
            int lenDiff = largeCoefficients.Length - smallCoefficients.Length;
            Array.Copy(largeCoefficients, sumDiff, lenDiff);
            for (int i = lenDiff; i < largeCoefficients.Length; i++)
                sumDiff[i] = GaloisField.AddOrSubtract(smallCoefficients[i - lenDiff], largeCoefficients[i]);
            return new GFPoly(GaloisField, sumDiff);
        }

        public GFPoly MultiplyByMonominal(int degree, int coefficient)
        {
            if (coefficient == 0) return Zero(GaloisField);
            int size = Coefficients.Length;
            int[] coefficients = new int[size + degree];
            for (int i = 0; i < size; i++)
                coefficients[i] = GaloisField.Multiply(Coefficients[i], coefficient);
            return new GFPoly(GaloisField, coefficients);
        }

        public GFPoly Multiply(GFPoly other)
        {
            if (IsZero() || other.IsZero()) return Zero(GaloisField);
            int[] product = new int[Coefficients.Length + other.Coefficients.Length - 1];
            for (int i = 0; i < Coefficients.Length; i++)
            {
                int ac = Coefficients[i];
                for (int j = 0; j < other.Coefficients.Length; j++)
                {
                    int bc = other.Coefficients[j];
                    product[i + j] = GaloisField.AddOrSubtract(product[i + j], GaloisField.Multiply(ac, bc));
                }
            }
            return new GFPoly(GaloisField, product);
        }

        public (GFPoly Quotient, GFPoly Remainder) Divide(GFPoly other)
        {
            GFPoly quotient = Zero(GaloisField);
            GFPoly remainder = this;
            int denomLeadTerm = other.GetCoefficient(other.Degree());
            int inversDenomLeadTerm = GaloisField.Inverse(denomLeadTerm);
            while (remainder.Degree() >= other.Degree() && !remainder.IsZero())
            {
                int degreeDiff = remainder.Degree() - other.Degree();
                int scale = GaloisField.Multiply(remainder.GetCoefficient(remainder.Degree()), inversDenomLeadTerm);
                GFPoly term = other.MultiplyByMonominal(degreeDiff, scale);
                GFPoly itQuot = MonominalPoly(GaloisField, degreeDiff, scale);
                quotient = quotient.AddOrSubtract(itQuot);
                remainder = remainder.AddOrSubtract(term);
            }
            return (quotient, remainder);
        }
    }
}
