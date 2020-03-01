using System;
using System.Linq;
using Barcoder.Utils;

namespace Barcoder.Aztec
{
    internal static class HighLevelEncoding
    {
        public static BitList Encode(byte[] data)
        {
            State[] states = { State.InitialState };

            for (var index = 0; index < data.Length; index++)
            {
                uint pairCode = 0;
                byte nextChar = index + 1 < data.Length ? data[index + 1] : (byte)0;

                var cur = data[index];
                if (cur == '\r' && nextChar == '\n')
                    pairCode = 2;
                else if (cur == '.' && nextChar == ' ')
                    pairCode = 3;
                else if (cur == ',' && nextChar == ' ')
                    pairCode = 4;
                else if (cur == ':' && nextChar == ' ')
                    pairCode = 5;

                if (pairCode > 0)
                {
                    // We have one of the four special PUNCT pairs.  Treat them specially.
                    // Get a new set of states for the two new characters.
                    states = UpdateStateListForPair(states, data, index, pairCode);
                    index++;
                }
                else
                {
                    // Get a new set of states for the new character.
                    states = UpdateStateListForChar(states, data, index);
                }
            }

            var minBitCnt = (int)(~0u >> 1);
            State? result = null;
            foreach (State state in states)
            {
                if (state.BitCount >= minBitCnt) continue;
                minBitCnt = state.BitCount;
                result = state;
            }

            return result?.ToBitList(data) ?? new BitList();
        }

        private static State[] SimplifyStates(State[] states)
        {
            State[] result = null;

            foreach (State newState in states ?? Array.Empty<State>())
            {
                bool add = true;
                State[] newResult = null;

                foreach (State oldState in result ?? Array.Empty<State>())
                {
                    if (add && oldState.IsBetterThanOrEqualTo(newState))
                        add = false;
                    if (!(add && newState.IsBetterThanOrEqualTo(oldState)))
                        newResult = (newResult ?? Array.Empty<State>()).Append(oldState).ToArray();
                }

                result = add ? (newResult ?? Array.Empty<State>()).Append(newState).ToArray() : newResult;
            }

            return result;
        }

        // We update a set of states for a new character by updating each state
        // for the new character, merging the results, and then removing the
        // non-optimal states.
        private static State[] UpdateStateListForChar(State[] states, byte[] data, int index)
        {
            State[] result = null;
            foreach (State state in states ?? Array.Empty<State>())
            {
                State[] r = UpdateStateForChar(state, data, index);
                if (r?.Length > 0)
                    result = (result ?? Array.Empty<State>()).Concat(r).ToArray();
            }
            return SimplifyStates(result);
        }

        // Return a set of states that represent the possible ways of updating this
        // state for the next character.  The resulting set of states are added to
        // the "result" list.
        private static State[] UpdateStateForChar(State state, byte[] data, int index)
        {
            State[] result = null;
            var ch = data[index];
            var charInCurrentTable = State.CharMap[state.Mode][ch] > 0;

            State? stateNoBinary = null;
            for (EncodingMode mode = EncodingMode.Upper; mode <= EncodingMode.Punct; mode++)
            {
                var charInMode = State.CharMap[mode][ch];
                if (charInMode > 0)
                {
                    if (!stateNoBinary.HasValue)
                    {
                        // Only create stateNoBinary the first time it's required.
                        stateNoBinary = state.EndBinaryShift(index);
                    }

                    // Try generating the character by latching to its mode
                    if (!charInCurrentTable || mode == state.Mode || mode == EncodingMode.Digit)
                    {
                        // If the character is in the current table, we don't want to latch to
                        // any other mode except possibly digit (which uses only 4 bits).  Any
                        // other latch would be equally successful *after* this character, and
                        // so wouldn't save any bits.
                        State res = stateNoBinary.Value.LatchAndAppend(mode, charInMode);
                        result = (result ?? Array.Empty<State>()).Append(res).ToArray();
                    }

                    // Try generating the character by switching to its mode.
                    if (!charInCurrentTable && State.ShiftTable.ContainsKey(state.Mode) && State.ShiftTable[state.Mode].ContainsKey(mode))
                    {
                        // It never makes sense to temporarily shift to another mode if the
                        // character exists in the current mode.  That can never save bits.
                        State res = stateNoBinary.Value.ShiftAndAppend(mode, charInMode);
                        result = (result ?? Array.Empty<State>()).Append(res).ToArray();
                    }
                }
            }

            if (state.ShiftByteCount > 0 || State.CharMap[state.Mode][ch] == 0)
            {
                // It's never worthwhile to go into binary shift mode if you're not already
                // in binary shift mode, and the character exists in your current mode.
                // That can never save bits over just outputting the char in the current mode.
                State res = state.AddBinaryShiftChar(index);
                result = (result ?? Array.Empty<State>()).Append(res).ToArray();
            }

            return result;
        }

        // We update a set of states for a new character by updating each state
        // for the new character, merging the results, and then removing the
        // non-optimal states.
        private static State[] UpdateStateListForPair(State[] states, byte[] data, int index, uint pairCode)
        {
            State[] result = null;
            foreach (State state in states ?? Array.Empty<State>())
            {
                State[] r = UpdateStateForPair(state, data, index, pairCode);
                result = (result ?? Array.Empty<State>()).Concat(r).ToArray();
            }
            return SimplifyStates(result);
        }

        private static State[] UpdateStateForPair(State state, byte[] data, int index, uint pairCode)
        {
            State[] result = Array.Empty<State>();
            State stateNoBinary = state.EndBinaryShift(index);
            // Possibility 1.  Latch to MODE_PUNCT, and then append this code
            result = result.Append(stateNoBinary.LatchAndAppend(EncodingMode.Punct, pairCode)).ToArray();
            if (state.Mode != EncodingMode.Punct)
            {
                // Possibility 2.  Shift to MODE_PUNCT, and then append this code.
                // Every state except MODE_PUNCT (handled above) can shift
                result = result.Append(stateNoBinary.ShiftAndAppend(EncodingMode.Punct, pairCode)).ToArray();
            }
            if (pairCode == 3 || pairCode == 4)
            {
                // both characters are in DIGITS.  Sometimes better to just add two digits
                State digitState = stateNoBinary
                    .LatchAndAppend(EncodingMode.Digit, 16 - pairCode) // period or comma in DIGIT
                    .LatchAndAppend(EncodingMode.Digit, 1);            // space in DIGIT
                result = result.Append(digitState).ToArray();
            }
            if (state.ShiftByteCount > 0)
            {
                // It only makes sense to do the characters as binary if we're already
                // in binary mode.
                result = result.Append(state.AddBinaryShiftChar(index).AddBinaryShiftChar(index + 1)).ToArray();
            }
            return result;
        }
    }
}
