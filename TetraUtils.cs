using System;

namespace SDRSharp.Tetra
{
    unsafe class TetraUtils
    {
        public static byte BitsToByte(byte* bitsBuffer, int offset, int length)
        {
            var result = (byte)0;

            for (int i = 0; i < length; i++)
            {
                result <<= 1;
                result |= (byte)(bitsBuffer[i + offset] & 1);
            }

            return result;
        }

        public static byte BitsToChar(byte* bitsBuffer, int offset, int length)
        {
            var result = (byte)0;

            for (int i = 0; i < length; i++)
            {
                result >>= 1;
                result |= (byte)((bitsBuffer[i + offset] & 1) << 7);
            }

            return result;
        }

        public static uint BitsToUInt32(byte* bitsBuffer, int offset, int length)
        {
            var result = (uint)0;

            for (int i = 0; i < length; i++)
            {
                result <<= 1;
                result |= (bitsBuffer[i + offset] & (uint)1);
            }

            return result;
        }

        public static int BitsToInt32(byte* bitsBuffer, int offset, int length)
        {
            var result = (int)0;

            for (int i = 0; i < length; i++)
            {
                result <<= 1;
                result |= (bitsBuffer[i + offset] & (int)1);
            }

            return result;
        }

        public static ulong BitsToULong(byte* bitsBuffer, int offset, int length)
        {
            var result = (ulong)0;

            for (int i = 0; i < length; i++)
            {
                result <<= 1;
                result |= (bitsBuffer[i + offset] & (ulong)1);
            }

            return result;
        }

        public static long BitsToLong(byte* bitsBuffer, int offset, int length)
        {
            var result = (long)0;

            for (int i = 0; i < length; i++)
            {
                result <<= 1;
                result |= (bitsBuffer[i + offset] & (long)1);
            }

            return result;
        }

        public static string BitsToString(byte* bitsBuffer, int offset, int length)
        {
            var result = String.Empty;

            for (int i = 0; i < length; i++)
            {
                result += ((bitsBuffer[i + offset]) == 0) ? "0" : "1";
            }

            return result;
        }

        public static string BitsToString(sbyte* bitsBuffer, int offset, int length)
        {
            var result = String.Empty;

            for (int i = 0; i < length; i++)
            {
                result += ((bitsBuffer[i + offset]) == 0) ? "-" : ((bitsBuffer[i + offset]) == -1) ? "0" : "1";
            }

            return result;
        }

        public static void ByteToBits(byte data, byte* bitsBuffer, int bufferOffset)
        {
            var length = 8 * sizeof(byte);

            for (int i = 0; i < length; i++)
            {
                bitsBuffer[bufferOffset + i] = (byte)((data & 0x80) == 0 ? 0 : 1);
                data <<= 1;
            }
        }

        public static void IntToBits(int data, byte* bitsBuffer, int bufferOffset)
        {
            var length = 8 * sizeof(int);

            for (int i = 0; i < length; i++)
            {
                bitsBuffer[bufferOffset + i] = (byte)((data & 0x80000000) == 0 ? 0 : 1);
                data <<= 1;
            }
        }

        public static void UIntToBits(UInt32 data, byte* bitsBuffer, int firstBitOffset)
        {
            var length = 8 * sizeof(int) - firstBitOffset;
            var msb = 0x80000000 >> firstBitOffset;

            for (int i = 0; i < length; i++)
            {
                bitsBuffer[i] = (byte)((data & msb) == 0 ? 0 : 1);
                data <<= 1;
            }
        }

        public static uint CreateScramblerCode(int mcc, int mnc, int colour)
        {
            mcc &= 0x3ff;
            mnc &= 0x3fff;
            colour &= 0x3f;

            return (((uint)mcc << 22) | ((uint)mnc << 8) | ((uint)colour << 2) | 3);
        }

        public static uint CreateScramblerCode(int mnc, int sourceAddress)
        {
            mnc &= 0x3f;
            sourceAddress &= 0xffffff;

            return (((uint)mnc << 26) | ((uint)sourceAddress << 2) | 3);
        }
    }
}
