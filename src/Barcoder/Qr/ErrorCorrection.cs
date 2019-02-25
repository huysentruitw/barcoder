using System;
using System.Linq;
using Barcoder.Utils;

namespace Barcoder.Qr
{
    internal static class ErrorCorrection
    {
        private static readonly GaloisField GaloisField = new GaloisField(285, 256, 0);
        private static readonly ReedSolomonEncoder ReedSolomonEncoder = new ReedSolomonEncoder(GaloisField);

        public static byte[] CalculateEcc(byte[] data, byte eccCount)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            var dataInts = data.Select(x => (int)x).ToArray();
            int[] res = ReedSolomonEncoder.Encode(dataInts, eccCount);
            return res.Select(x => (byte)x).ToArray();
        }
    }
}
