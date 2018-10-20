using FluentAssertions;
using Xunit;

namespace Barcoder.Tests
{
    public sealed class Code128Tests
    {
        private static void TestEncode(string txt, string testResult)
        {
            IBarcodeIntCS code = Barcoder.Code128.Encode(txt);

            code.Bounds.X.Should().Be(testResult.Length);
            code.Bounds.Y.Should().Be(1);

            string encoded = string.Empty;
            int i = 0;
            foreach (var r in testResult)
                encoded += code.At(i++, 0) ? "1" : "0";
            encoded.Should().Be(testResult);
        }

        [Fact]
        public void Code128_EncodeFunctionChars()
        {
            var encFNC1 = "11110101110";
            var encFNC2 = "11110101000";
            var encFNC3 = "10111100010";
            var encFNC4 = "10111101110";
            var encStartB = "11010010000";
            var encStop = "1100011101011";

            // Special Case FC1 can also be encoded to C Table therefor using 123 as suffix might have unexpected results.
            TestEncode(Code128Constants.FNC1 + "A23", encStartB + encFNC1 + "10100011000" + "11001110010" + "11001011100" + "10100011110" + encStop);
            TestEncode(Code128Constants.FNC2 + "123", encStartB + encFNC2 + "10011100110" + "11001110010" + "11001011100" + "11100010110" + encStop);
            TestEncode(Code128Constants.FNC3 + "123", encStartB + encFNC3 + "10011100110" + "11001110010" + "11001011100" + "11101000110" + encStop);
            TestEncode(Code128Constants.FNC4 + "123", encStartB + encFNC4 + "10011100110" + "11001110010" + "11001011100" + "11100011010" + encStop);
        }

        [Fact]
        public void Code128_ShouldUseCTable()
        {
            bool result;
            bool T(byte currentEncoding, params char[] nextChars)
                => Code128.ShouldUseCTable(nextChars, currentEncoding);

            result = T(Code128Constants.StartCSymbol, Code128Constants.FNC1, '1', '2');
            result.Should().BeTrue();

            result = T(Code128Constants.StartCSymbol, Code128Constants.FNC1, '1');
            result.Should().BeFalse();

            result = T(Code128Constants.StartCSymbol, '0', Code128Constants.FNC1, '1');
            result.Should().BeFalse();

            result = T(Code128Constants.StartBSymbol, '0', '1', Code128Constants.FNC1, '2', '3');
            result.Should().BeTrue();

            result = T(Code128Constants.StartBSymbol, '0', '1', Code128Constants.FNC1);
            result.Should().BeFalse();
        }

        [Fact]
        public void Code128_Issue16()
        {
            bool result;
            bool T(byte currentEncoding, params char[] nextChars)
                => Code128.ShouldUseATable(nextChars, currentEncoding);

            result = T(0, '\r', 'A');
            result.Should().BeTrue();

            result = T(0, Code128Constants.FNC1, '\r');
            result.Should().BeTrue();

            result = T(0, Code128Constants.FNC1, '1', '2', '3');
            result.Should().BeFalse();

            TestEncode(Code128Constants.FNC3 + "$P\rI", "110100001001011110001010010001100111011101101111011101011000100010110001010001100011101011");
        }

        [Fact]
        public void Code128_Datalogic()
        {
            // <Start A><FNC3>$P\r<checksum><STOP>
            TestEncode(Code128Constants.FNC3 + "$P\r",
                "11010000100" + // <Start A>
                "10111100010" + // <FNC3>
                "10010001100" + // $
                "11101110110" + // P
                "11110111010" + // CR
                "11000100010" + // checksum = 'I'
                "1100011101011"); // STOP

            // <Start B><FNC3>$P,Ae,P<CR><checksum><STOP>
            TestEncode(Code128Constants.FNC3 + "$P,Ae,P\r",
                "11010010000" + // <Start B>
                "10111100010" + // <FNC3>
                "10010001100" + // $
                "11101110110" + // P
                "10110011100" + // ,
                "10100011000" + // A
                "10110010000" + // e
                "10110011100" + // ,
                "11101110110" + // P
                "11101011110" + // <Code A>
                "11110111010" + // <CR>
                "10110001000" + // checksum = 'D'
                "1100011101011"); // STOP
        }
    }
}
