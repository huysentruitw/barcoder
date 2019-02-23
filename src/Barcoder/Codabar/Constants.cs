using System.Collections.Generic;

namespace Barcoder.Codebar
{
    internal static class Constants
    {
        public static readonly IReadOnlyDictionary<char, bool[]> EncodingTable = new Dictionary<char, bool[]>
        {
            { '0', new[] {true, false, true, false, true, false, false, true, true} },
            { '1', new[] {true, false, true, false, true, true, false, false, true} },
            { '2', new[] {true, false, true, false, false, true, false, true, true} },
            { '3', new[] {true, true, false, false, true, false, true, false, true} },
            { '4', new[] {true, false, true, true, false, true, false, false, true} },
            { '5', new[] {true, true, false, true, false, true, false, false, true} },
            { '6', new[] {true, false, false, true, false, true, false, true, true} },
            { '7', new[] {true, false, false, true, false, true, true, false, true} },
            { '8', new[] {true, false, false, true, true, false, true, false, true} },
            { '9', new[] {true, true, false, true, false, false, true, false, true} },
            { '-', new[] {true, false, true, false, false, true, true, false, true} },
            { '$', new[] {true, false, true, true, false, false, true, false, true} },
            { ':', new[] {true, true, false, true, false, true, true, false, true, true} },
            { '/', new[] {true, true, false, true, true, false, true, false, true, true} },
            { '.', new[] {true, true, false, true, true, false, true, true, false, true} },
            { '+', new[] {true, false, true, true, false, true, true, false, true, true} },
            { 'A', new[] {true, false, true, true, false, false, true, false, false, true} },
            { 'B', new[] {true, false, false, true, false, false, true, false, true, true} },
            { 'C', new[] {true, false, true, false, false, true, false, false, true, true} },
            { 'D', new[] {true, false, true, false, false, true, true, false, false, true} },
        };

        public const int Margin = 10;
    }
}
