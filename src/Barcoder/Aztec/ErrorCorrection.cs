using System;
using Barcoder.Utils;

namespace Barcoder.Aztec
{
    internal static class ErrorCorrection
    {
        public static BitList GenerateCheckWords(BitList bits, int totalBits, int wordSize)
        {
            var rs = new ReedSolomonEncoder(GetGaloisField(wordSize));

            // bits is guaranteed to be a multiple of the wordSize, so no padding needed
            int messageWordCount = bits.Length / wordSize;
            int totalWordCount = totalBits / wordSize;
            int eccWordCount = totalWordCount - messageWordCount;

            int[] messageWords = BitsToWords(bits, wordSize, messageWordCount);
            int[] eccWords = rs.Encode(messageWords, eccWordCount);
            int startPad = totalBits % wordSize;

            var messageBits = new BitList();
            messageBits.AddBits(0, (byte)startPad);

            foreach (var messageWord in messageWords)
                messageBits.AddBits((uint)messageWord, (byte)wordSize);

            foreach (var eccWord in eccWords)
                messageBits.AddBits((uint)eccWord, (byte)wordSize);

            return messageBits;
        }

        private static int[] BitsToWords(BitList stuffedBits, int wordSize, int wordCount)
        {
            var message = new int[wordCount];

            for (int i = 0; i < wordCount; i++)
            {
                int value = 0;
                for (int j = 0; j < wordSize; j++)
                    if (stuffedBits.GetBit(i*wordSize + j))
                        value |= (1 << (wordSize - j - 1));

                message[i] = value;
            }

            return message;
        }

        private static GaloisField GetGaloisField(int wordSize)
        {
            switch (wordSize)
            {
                case 4: return new GaloisField(0x13, 16, 1);
                case 6: return new GaloisField(0x43, 64, 1);
                case 8: return new GaloisField(0x012D, 256, 1);
                case 10: return new GaloisField(0x409, 1024, 1);
                case 12: return new GaloisField(0x1069, 4096, 1);
                default: throw new InvalidOperationException($"Invalid word size {wordSize}");
            }
        }
    }
}
