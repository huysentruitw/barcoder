using System;
using System.Collections.Generic;

namespace Barcoder.Utils
{
    internal class ReedSolomonEncoder
    {
        private readonly object _syncRoot = new object();
        private readonly List<GFPoly> _polynomes = new List<GFPoly>();

        public ReedSolomonEncoder(GaloisField galoisField)
        {
            GaloisField = galoisField;
            _polynomes = new List<GFPoly> { new GFPoly(GaloisField, new[] { 1 }) };
        }

        public GaloisField GaloisField { get; }

        public GFPoly[] Polynomes => _polynomes.ToArray();

        public int[] Encode(int[] data, int eccCount)
        {
            GFPoly generator = GetPolynomial(eccCount);
            GFPoly info = new GFPoly(GaloisField, data);
            info = info.MultiplyByMonominal(eccCount, 1);
            (_, GFPoly remainder) = info.Divide(generator);
            int[] result = new int[eccCount];
            int numZero = eccCount - remainder.Coefficients.Length;
            Array.Copy(remainder.Coefficients, 0, result, numZero, remainder.Coefficients.Length);
            return result;
        }

        private GFPoly GetPolynomial(int degree)
        {
            lock (_syncRoot)
            {
                if (degree >= _polynomes.Count)
                {
                    GFPoly last = _polynomes[_polynomes.Count - 1];
                    for (int d = _polynomes.Count; d <= degree; d++)
                    {
                        GFPoly next = last.Multiply(new GFPoly(GaloisField, new int[] { 1, GaloisField.ALogTable[d - 1 + GaloisField.Base] }));
                        _polynomes.Add(next);
                        last = next;
                    }
                }
                return _polynomes[degree];
            }
        }
    }
}
