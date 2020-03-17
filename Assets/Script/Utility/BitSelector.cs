using System;
using System.Collections.Generic;
using System.Text;

class BitSelector
{
    public const UInt32 Sequence16 = (1 << 16) - 1;
    public const UInt32 Sequence12 = (1 << 12) - 1;
    public const UInt32 Sequence3 = (1 << 3) - 1;

    public static UInt64 GetSequence(int count, int offset)
    {
        return ((UInt64)(1 << count) - 1) << offset;
    }

    public static int GetFirstBitPosition(int number)
    {
        if (number == 0)
            return -1;

        int count = 0;
        while (number != 1)
        {
            number = number >> 1;
            count++;
        }

        return count;
    }
}