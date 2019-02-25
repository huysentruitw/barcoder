using System;
using System.Collections.Generic;

namespace Barcoder.Qr
{
    internal class BlockList
    {
        private readonly Block[] _blocks;

        public BlockList(Block[] blocks)
        {
            _blocks = blocks;
        }

        public byte[] Interleave(VersionInfo versionInfo)
        {
            int maxCodewordCount = Math.Max(versionInfo.DataCodewordsPerBlockInGroup1, versionInfo.DataCodewordsPerBlockInGroup2);
            var result = new List<byte>();
            for (int i = 0; i < maxCodewordCount; i++)
                for (int b = 0; b < _blocks.Length; b++)
                    if (_blocks[b].Data.Length > i)
                        result.Add(_blocks[b].Data[i]);
            for (int i = 0; i < versionInfo.ErrorCorrectionCodewordsPerBlock; i++)
                for (int b = 0; b < _blocks.Length; b++)
                    result.Add(_blocks[b].Ecc[i]);
            return result.ToArray();
        }

        public static BlockList SplitToBlocks(Queue<byte> data, VersionInfo versionInfo)
        {
            var blocks = new Block[versionInfo.NumberOfBlocksInGroup1 + versionInfo.NumberOfBlocksInGroup2];

            for (int b = 0; b < versionInfo.NumberOfBlocksInGroup1; b++)
            {
                var block = new Block();
                block.Data = new byte[versionInfo.DataCodewordsPerBlockInGroup1];
                for (int cw = 0; cw < versionInfo.DataCodewordsPerBlockInGroup1; cw++)
                    block.Data[cw] = data.Dequeue();
                block.Ecc = ErrorCorrection.CalculateEcc(block.Data, versionInfo.ErrorCorrectionCodewordsPerBlock);
                blocks[b] = block;
            }

            for (int b = 0; b < versionInfo.NumberOfBlocksInGroup2; b++)
            {
                var block = new Block();
                block.Data = new byte[versionInfo.DataCodewordsPerBlockInGroup2];
                for (int cw = 0; cw < versionInfo.DataCodewordsPerBlockInGroup2; cw++)
                    block.Data[cw] = data.Dequeue();
                block.Ecc = ErrorCorrection.CalculateEcc(block.Data, versionInfo.ErrorCorrectionCodewordsPerBlock);
                blocks[versionInfo.NumberOfBlocksInGroup1 + b] = block;
            }

            return new BlockList(blocks);
        }
    }

    internal class Block
    {
        public byte[] Data { get; set; }
        public byte[] Ecc { get; set; }
    }
}
