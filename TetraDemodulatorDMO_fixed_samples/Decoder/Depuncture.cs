namespace SDRSharp.Tetra
{
    unsafe class Depuncture
    {
        public enum PunctType
        {
            PUNCT_2_3,
            PUNCT_1_3,
            PUNCT_292_432,
            PUNCT_148_432,
            PUNCT_112_168,
            PUNCT_72_162,
            PUNCT_38_80
        }

        private enum FunctionType
        {
            Equals,
            Func_292,
            Func_148
        }

        /* Puncturing */
        static byte[] P_rate1_3 = new byte[] { 0, 1, 2, 3, 5, 6, 7 };
        static byte[] P_rate2_3 = new byte[] { 0, 1, 2, 5 };
        /* Voice */
        static byte[] P_rate8_12 = new byte[] { 0, 1, 2, 4 };
        static byte[] P_rate8_18 = new byte[] { 0, 1, 2, 3, 4, 5, 7, 8, 10, 11 };
        static byte[] P_rate8_17 = new byte[] { 0, 1, 2, 3, 4, 5, 7, 8, 10, 11, 13, 14, 16, 17, 19, 20, 22, 23 };

        /* De-Puncture the type-3 bits (source) and write mother code to dest
         * */
        public void Process(PunctType puncType, byte* source, sbyte* dest, int sourceLength)
        {
            uint i, j, k;
            byte t;
            byte* pPtr;
            byte period;
            FunctionType funcType;

            for (int ii = 0; ii < sourceLength * 4; ii++)
            {
                dest[ii] = 0;
            }

            fixed (byte* p1_3Ptr = P_rate1_3, p2_3Ptr = P_rate2_3, p8_12Ptr = P_rate8_12, p8_18Ptr = P_rate8_18, p8_17Ptr = P_rate8_17)
            {
                switch (puncType)
                {
                    case PunctType.PUNCT_2_3:
                        /* Section 8.2.3.1.3 */
                        pPtr = p2_3Ptr;
                        t = 3;
                        period = 8;
                        funcType = FunctionType.Equals;
                        break;

                    case PunctType.PUNCT_1_3:
                        /* Section 8.2.3.1.4 */
                        pPtr = p1_3Ptr;
                        t = 6;
                        period = 8;
                        funcType = FunctionType.Equals;
                        break;

                    case PunctType.PUNCT_292_432:
                        /* Section 8.2.3.1.5 */
                        pPtr = p2_3Ptr;
                        t = 3;
                        period = 8;
                        funcType = FunctionType.Func_292;
                        break;

                    case PunctType.PUNCT_148_432:
                        /* Section 8.2.3.1.6 */
                        pPtr = p1_3Ptr;
                        t = 6;
                        period = 8;
                        funcType = FunctionType.Func_148;
                        break;

                    case PunctType.PUNCT_112_168:
                        /* EN 300 395-2 Section 5.5.2.1 */
                        pPtr = p8_12Ptr;
                        t = 3;
                        period = 6;
                        funcType = FunctionType.Equals;
                        break;

                    case PunctType.PUNCT_72_162:
                        /* EN 300 395-2 Section 5.5.2.2 */
                        pPtr = p8_18Ptr;
                        t = 9;
                        period = 12;
                        funcType = FunctionType.Equals;
                        break;

                    case PunctType.PUNCT_38_80:
                        /* EN 300 395-2 Section 5.6.2.1 */
                        pPtr = p8_17Ptr;
                        t = 17;
                        period = 24;
                        funcType = FunctionType.Equals;
                        break;

                    default:
                        return;
                }

                /* Section 8.2.3.1.2 */
                for (j = 1; j <= sourceLength; j++)
                {
                    switch (funcType)
                    {
                        case FunctionType.Func_148:
                            i = (j + ((j - 1) / 35));
                            break;

                        case FunctionType.Func_292:
                            i = (j + ((j - 1) / 65));
                            break;

                        case FunctionType.Equals:
                        default:
                            i = j;
                            break;
                    }

                    k = period * ((i - 1) / t) + pPtr[i - t * ((i - 1) / t)];
                    dest[k - 1] = (sbyte)(source[j - 1] == 0 ? 1 : -1);
                }
            }
        }
    }
}
