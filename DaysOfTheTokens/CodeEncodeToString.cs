using System;
using UnityEngine;

public class CodeEncodeToString
{
    /**
     * Encode 
     */
    static public void encodeIntToString(int value)
    {
    }

    static public void decodeFromString(int value)
    {
    }

    // All normal letters expected the '.'. We will use it to separate fields

    // From http://codeofthedamned.com/index.php/number-base-conversion

    // The set of possible symbols for representing other bases.
    readonly char[] symbols =  {'0', '1', '2', '3', '4', '5',
                            '6', '7', '8', '9', 'A', 'B',
                            'C', 'D', 'E', 'F', 'G', 'H',
                            'I', 'J', 'K', 'L', 'M', 'N',
                            'P', 'Q', 'R', 'S', 'T',
                            'U', 'V', 'W', 'X', 'Y', 'Z',
                            'a', 'b', 'c', 'd', 'e', 'f',
                            'g', 'h', 'i', 'j', 'k', 'l',
                            'm', 'n', 'o', 'p', 'q', 'r',
                            's', 't', 'u', 'v', 'w', 'x',
                            'y', 'z'};

    public String ConvertToBase(long inDecimal)
    {
        int radix = symbols.Length;

        String valueInNewBase = "";

        long[] remainder = new long[radix]; // 32
        long quotient = inDecimal;
        int place = 0;

        if (inDecimal <= 0)
        {
            return ""+symbols[0];
        }

        while (0 != quotient)
        {
            long value = quotient;
            remainder[place] = value % radix;
            //Debug.Log("Remainder: "+place + " : " + remainder[place]);
            quotient = (value - remainder[place]) / radix;

            place++;
        }

        //Debug.Log(inDecimal + " in base " + radix + " is ");

        for (int index = 1; index <= place; index++)
        {
            valueInNewBase += symbols[remainder[place - index]];
        }

        return valueInNewBase;
    }

    public int ConvertFromBase(String from)
    {
        int radix = symbols.Length;
        int result = 0;

        char[] arrayFrom = from.ToCharArray();
        int rank = from.Length-1;

        for (int iFrom = 0; iFrom < from.Length; iFrom++)
        {
            char oneDigit = arrayFrom[iFrom];

            // Now find which number is that
            int digitFromArray = findNumberFromArray(oneDigit);
            // Then multiply it by the radix at the power of the rank

            //Debug.Log("Character at rank " + iFrom + " is " + oneDigit + " with result " + digitFromArray);

            result += digitFromArray * (IntPow(radix , rank));

            //Debug.Log("=== " + digitFromArray + " * ( " + radix + " ^ " + rank + " ) "+ (radix ^ rank));

            rank--;
        }

        return result;
    }

    int findNumberFromArray(char forChar)
    {
        for (int iFrom = 0; iFrom < symbols.Length; iFrom++)
        {
            if (forChar == symbols[iFrom])
            {
                //Debug.Log("Character found is " + forChar + " with matching " + symbols[iFrom]);
                return iFrom;
            }
        }
        return -1;
    }

    int IntPow(int x, int pow)
    {
        int ret = 1;
        while (pow != 0)
        {
            if ((pow & 1) == 1)
                ret *= x;
            x *= x;
            pow >>= 1;
        }
        return ret;
    }
}
