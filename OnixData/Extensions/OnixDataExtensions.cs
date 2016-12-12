﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OnixData.Legacy;
using OnixData.Version3;

namespace OnixData.Extensions
{
    public static class OnixDataExtensions
    {
        #region CONSTANTS

        public const int CONST_ISBN_LEN = 10;
        public const int CONST_EAN_LEN  = 13;

        #endregion

        public static bool HasValidEAN(this OnixLegacyProduct TargetProduct)
        {
            bool IsValid = false;
            string EAN = TargetProduct.EAN;

            if (!String.IsNullOrEmpty(EAN))
                IsValid = EAN.IsValidEAN();

            return IsValid;
        }

        public static bool IsValidEAN(this string TargetEAN)
        {
            bool IsValid = false;

            int[] EanWeights = 
			    { 1, 3, 1, 3, 1, 3, 1, 3, 1, 3, 1, 3, 1 };

            int sum, diff;
            int ZeroCharVal = (int)'0';
            long value;

            String TempEAN = TargetEAN;

            // Initialization
            sum = 0;
            value = 0;

            if (String.IsNullOrEmpty(TargetEAN))
                return false;

            if (TargetEAN.Length > CONST_EAN_LEN)
                return false;

            try
            {
                long nEAN = Convert.ToInt64(TargetEAN);
            }
            catch (Exception ex)
            { return false; }

            if (TargetEAN.Length < CONST_EAN_LEN)
                TempEAN = TargetEAN.PadLeft(CONST_EAN_LEN, '0');

            char[] EanCharArray = TempEAN.ToCharArray();
            for (int i = 0; i < EanCharArray.Length; ++i)
            {
                int TempCharVal = (int)EanCharArray[i];

                diff = (TempCharVal - ZeroCharVal);

                sum += (diff) * EanWeights[i];

                value = value * 10 + (diff);
            }

            IsValid = (sum % 10 == 0);

            return IsValid;
        }

        public static bool HasValidISBN(this OnixLegacyProduct TargetProduct)
        {
            bool IsValid = false;

            string ISBN = TargetProduct.ISBN;
            if (!String.IsNullOrEmpty(ISBN))
                IsValid = ISBN.IsISBNValid();

            return IsValid;
        }

        static bool IsISBNValid(this string TargetISBN)
        {
            bool    IsValid;
            int     TempVal;
            int     Counter;
            int     CheckSum;
            char    cCurrentCheckDigit;

            // Initialization
            IsValid             = true;
            cCurrentCheckDigit  = 'x';
            CheckSum            = Counter = 0;
            TempVal             = 0;

            if (TargetISBN.Length != CONST_ISBN_LEN)
                return false;

            string ISBNPrefix = TargetISBN.Substring(0, (CONST_ISBN_LEN - 1));

            try
            {
                long nISBNPrefix = Convert.ToInt64(ISBNPrefix);
            }
            catch (Exception ex) { return false; }

            try
            {
                cCurrentCheckDigit = TargetISBN.ToCharArray()[CONST_ISBN_LEN - 1];

                if( cCurrentCheckDigit == 'x' )
                    cCurrentCheckDigit = 'X';

                CheckSum = Counter = 0;
                foreach (char TempDigit in ISBNPrefix)
                {
                    int TempDigitVal = Convert.ToInt32(new String(new char[] { TempDigit }));

                    if (TempDigitVal > 0)
                        CheckSum += (CONST_ISBN_LEN - Counter) * TempDigitVal;

                    Counter++;
                }

                CheckSum = 11 - (CheckSum % 11);

                // sprintf(acTemp, "%c", cCurrentCheckDigit);

                TempVal = Convert.ToInt32(new String(new char[] { cCurrentCheckDigit } ));
                if( ((CheckSum == 10) && (cCurrentCheckDigit == 'X')) ||
                    ((CheckSum == 11) && (cCurrentCheckDigit == '0')) ||
                    (CheckSum == TempVal))
                {
                    IsValid = true;
                }
                else
                    IsValid = false;
            }
            catch(Exception ex)
            {
                IsValid = false;
            }

            return IsValid;
        }
    }
}
