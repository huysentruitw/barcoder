using System;
using Barcoder.Utils;

namespace Barcoder.DataMatrix
{
    internal static class ErrorCorrection
    {
        private static readonly ReedSolomonEncoder ReedSolomonEncoder = new ReedSolomonEncoder(new GaloisField(301, 256, 1));

        public static byte[] CalculateEcc(byte[] data, CodeSize size)
        {
            var dataSize = data.Length;
            var result = new byte[data.Length + size.EccCount];
            Array.Copy(data, result, data.Length);

            for (int block = 0; block < size.BlockCount; block++)
            {
                var dataCnt = size.DataCodewordsForBlock(block);
                var buff = new int[dataCnt];
                // copy the data for the current block to buff
                var j = 0;
                for (int i = block; i < dataSize; i += size.BlockCount)
                {
                    buff[j] = result[i];
                    j++;
                }
                // calc the error correction codes
                var ecc = ReedSolomonEncoder.Encode(buff, size.ErrorCorrectionCodewordsPerBlock);
                // and append them to the result
                j = 0;
                for (int i = block; i < size.ErrorCorrectionCodewordsPerBlock * size.BlockCount; i += size.BlockCount)
                {
                    result[dataSize + i] = (byte)ecc[j];
                    j++;
                }
            }

            return result;
        }
    }
}
