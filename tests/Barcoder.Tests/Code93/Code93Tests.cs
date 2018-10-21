using FluentAssertions;
using Xunit;

namespace Barcoder.Tests
{
    public sealed class Code93Tests
    {
        private static void TestEncode(string data, string testResult)
        {
            IBarcode code = Code93.Encode(data, true, false);

            code.Bounds.X.Should().Be(testResult.Length);
            code.Bounds.Y.Should().Be(1);

            string encoded = string.Empty;
            int i = 0;
            foreach (var r in testResult)
                encoded += code.At(i++, 0) ? "1" : "0";
            encoded.Should().Be(testResult);
        }

        [Fact]
        public void Encode()
        {
            TestEncode("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
                "1010111101101010001101001001101000101100101001100100101100010101011010001011001" +
                "001011000101001101001000110101010110001010011001010001101001011001000101101101101001" +
                "101100101101011001101001101100101101100110101011011001011001101001101101001110101000" +
                "101001010010001010001001010000101001010001001001001001000101010100001000100101000010" +
                "101001110101010000101010111101");
        }
    }
}
