using System;
using System.Linq;
using Barcoder.Utils;

namespace Barcoder.Aztec
{
    public static class AztecEncoder
    {
        private const int MaximumNumberOfBits = 32;
        private const int MaximumNumberOfCompactBits = 4;

        private static readonly int[] WordSize = new[]
        {
            4, 6, 6, 8, 8, 8, 8, 8, 8, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10,
            12, 12, 12, 12, 12, 12, 12, 12, 12, 12,
        };

        public static IBarcode Encode(string content, int minimumEccPercentage = 23, int userSpecifiedLayers = 0)
        {
            BitList bits = HighLevelEncoding.Encode(content.Select(x => (byte)x).ToArray());
            int eccBits = ((bits.Length * minimumEccPercentage) / 100) + 11;

            int totalSizeBits = bits.Length + eccBits;
            int layers, totalBitsInLayer, wordSize;
            bool compact;
            BitList stuffedBits;

            if (userSpecifiedLayers != 0)
            {
                compact = userSpecifiedLayers < 0;
                layers = compact ? -userSpecifiedLayers : userSpecifiedLayers;

                if ((compact && layers > MaximumNumberOfCompactBits) || (!compact && layers > MaximumNumberOfBits))
                    throw new InvalidOperationException($"Illegal value {userSpecifiedLayers} for layers");

                totalBitsInLayer = GetTotalBitsInLayer(layers, compact);
                wordSize = WordSize[layers];
                int usableBitsInLayer = totalBitsInLayer - (totalBitsInLayer % wordSize);
                stuffedBits = StuffBits(bits, wordSize);

                if (stuffedBits.Length + eccBits > usableBitsInLayer)
                    throw new InvalidOperationException("Data too large for user specified layer");

                if (compact && stuffedBits.Length > wordSize * 64)
                    throw new InvalidOperationException("Data too large for user specified layer");
            }
            else
            {
                stuffedBits = new BitList();
                wordSize = 0;

                // We look at the possible table sizes in the order Compact1, Compact2, Compact3,
                // Compact4, Normal4,...  Normal(i) for i < 4 isn't typically used since Compact(i+1)
                // is the same size, but has more data.
                for (int i = 0; ; i++)
                {
                    if (i > MaximumNumberOfBits)
                        throw new InvalidOperationException("Data too large for an aztec code");

                    compact = i <= 3;
                    layers = compact ? i + 1 : i;

                    totalBitsInLayer = GetTotalBitsInLayer(layers, compact);
                    if (totalSizeBits > totalBitsInLayer)
                        continue;

                    // [Re]stuff the bits if this is the first opportunity, or if the
                    // wordSize has changed
                    if (wordSize != WordSize[layers])
                    {
                        wordSize = WordSize[layers];
                        stuffedBits = StuffBits(bits, wordSize);
                    }

                    int usableBitsInLayers = totalBitsInLayer - (totalBitsInLayer % wordSize);

                    if (compact && stuffedBits.Length > wordSize * 64)
                        continue; // Compact format only allows 64 data words, though C4 can hold more words than that

                    if (stuffedBits.Length + eccBits <= usableBitsInLayers)
                        break;
                }
            }
            BitList messageBits = ErrorCorrection.GenerateCheckWords(stuffedBits, totalBitsInLayer, wordSize);
            int messageSizeInWords = stuffedBits.Length / wordSize;
            BitList modeMessage = GenerateModeMessage(compact, layers, messageSizeInWords);

            // allocate symbol
            int baseMatrixSize = compact
                ? 11 + layers * 4
                : 14 + layers * 4;
            var alignmentMap = new int[baseMatrixSize];
            int matrixSize;

	        if (compact)
            {
		        // no alignment marks in compact mode, alignmentMap is a no-op
                matrixSize = baseMatrixSize;
		        for (int i = 0; i < alignmentMap.Length; i++)
                    alignmentMap[i] = i;
	        }
            else
            {
                matrixSize = baseMatrixSize + 1 + 2 * ((baseMatrixSize / 2 - 1) / 15);
		        int origCenter = baseMatrixSize / 2;
		        int center = matrixSize / 2;
		        for (int i = 0; i < origCenter; i++)
                {
                    int newOffset = i + i / 15;
                    alignmentMap[origCenter - i - 1] = center - newOffset - 1;
                    alignmentMap[origCenter + i] = center + newOffset + 1;
                }
	        }
            var code = new AztecCode(matrixSize);
            code.Content = content;

	        // draw data bits
	        for (int i = 0, rowOffset = 0; i < layers; i++)
            {
                int rowSize = (layers - i) * 4;
                if (compact)
                    rowSize += 9;
                else
                    rowSize += 12;

		        for (int j = 0; j < rowSize; j++)
                {
                    int columnOffset = j * 2;
			        for (int k = 0; k < 2; k++)
                    {
				        if (messageBits.GetBit(rowOffset + columnOffset + k))
                            code.Set(alignmentMap[i * 2 + k], alignmentMap[i * 2 + j]);

                        if (messageBits.GetBit(rowOffset + rowSize*2 + columnOffset + k))
                            code.Set(alignmentMap[i * 2 + j], alignmentMap[baseMatrixSize - 1 - i * 2 - k]);

                        if (messageBits.GetBit(rowOffset + rowSize*4 + columnOffset + k))
					        code.Set(alignmentMap[baseMatrixSize-1-i*2-k], alignmentMap[baseMatrixSize-1-i*2-j]);

                        if (messageBits.GetBit(rowOffset + rowSize * 6 + columnOffset + k))
                            code.Set(alignmentMap[baseMatrixSize - 1 - i * 2 - j], alignmentMap[i * 2 + k]);
                    }
		        }

                rowOffset += rowSize * 8;
            }

	        // draw mode message
            DrawModeMessage(code, compact, matrixSize, modeMessage);

	        // draw alignment marks
	        if (compact)
            {
                DrawBullsEye(code, matrixSize / 2, 5);
            }
            else
            {
                DrawBullsEye(code, matrixSize / 2, 7);
		        for (int i = 0, j = 0; i < baseMatrixSize / 2 - 1; i += 15, j += 16)
                {
			        for (int k = (matrixSize / 2) & 1; k < matrixSize; k += 2)
                    {
                        code.Set(matrixSize / 2 - j, k);
                        code.Set(matrixSize / 2 + j, k);
                        code.Set(k, matrixSize / 2 - j);
                        code.Set(k, matrixSize / 2 + j);
                    }
		        }
	        }

            return code;
        }

        private static int GetTotalBitsInLayer(int layers, bool compact)
        {
            int alpha = compact ? 88 : 112;
            return (alpha + 16 * layers) * layers;
        }

        private static BitList StuffBits(BitList bits, int wordSize)
        {
            var result = new BitList();
            int n = bits.Length;
            uint mask = (uint)((1 << wordSize) - 2);
            for (int i = 0; i < n; i += wordSize)
            {
                uint word = 0;
                for (int j = 0; j < wordSize; j++)
                    if (i + j >= n || bits.GetBit(i + j))
                        word |= (uint)(1 << (wordSize - 1 - j));

                if ((word & mask) == mask)
                {
                    result.AddBits(word & mask, (byte)wordSize);
                    i--;
                }
                else if ((word & mask) == 0)
                {
                    result.AddBits(word | 1, (byte)wordSize);
                    i--;
                }
                else
                {
                    result.AddBits(word, (byte)wordSize);
                }
            }

            return result;
        }

        private static BitList GenerateModeMessage(bool compact, int layers, int messageSizeInWords)
        {
            var modeMessage = new BitList();
            if (compact)
            {
                modeMessage.AddBits((uint)layers - 1, 2);
                modeMessage.AddBits((uint)messageSizeInWords - 1, 6);
                modeMessage = ErrorCorrection.GenerateCheckWords(modeMessage, 28, 4);
            }
            else
            {
                modeMessage.AddBits((uint)layers - 1, 5);
                modeMessage.AddBits((uint)messageSizeInWords - 1, 11);
                modeMessage = ErrorCorrection.GenerateCheckWords(modeMessage, 40, 4);
            }

            return modeMessage;
        }

        private static void DrawModeMessage(AztecCode matrix, bool compact, int matrixSize, BitList modeMessage)
        {
            int center = matrixSize / 2;
            if (compact)
            {
                for (int i = 0; i < 7; i++)
                {
                    int offset = center - 3 + i;
                    if (modeMessage.GetBit(i))
                        matrix.Set(offset, center - 5);

                    if (modeMessage.GetBit(i + 7))
                        matrix.Set(center + 5, offset);

                    if (modeMessage.GetBit(20 - i))
                        matrix.Set(offset, center + 5);

                    if (modeMessage.GetBit(27 - i))
                        matrix.Set(center - 5, offset);
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    int offset = center - 5 + i + i / 5;
                    if (modeMessage.GetBit(i))
                        matrix.Set(offset, center - 7);

                    if (modeMessage.GetBit(i + 10))
                        matrix.Set(center + 7, offset);

                    if (modeMessage.GetBit(29 - i))
                        matrix.Set(offset, center + 7);

                    if (modeMessage.GetBit(39 - i))
                        matrix.Set(center - 7, offset);
                }
            }
        }

        private static void DrawBullsEye(AztecCode matrix, int center, int size)
        {
            for (int i = 0; i < size; i += 2)
            {
                for (int j = center - i; j <= center+i; j++)
                {
                    matrix.Set(j, center - i);
                    matrix.Set(j, center + i);
                    matrix.Set(center - i, j);
                    matrix.Set(center + i, j);
                }
            }

            matrix.Set(center - size, center - size);
            matrix.Set(center - size + 1, center - size);
            matrix.Set(center - size, center - size + 1);
            matrix.Set(center + size, center - size);
            matrix.Set(center + size, center - size + 1);
            matrix.Set(center + size, center + size - 1);
        }
    }
}
