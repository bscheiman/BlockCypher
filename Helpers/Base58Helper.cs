#region
using System;
using System.Linq;
using System.Numerics;
using BlockCypher.Pcl;

#endregion

namespace BlockCypher.Helpers {
    public static class Base58Helper {
        public const int CheckSumSizeInBytes = 4;

        private const string Digits = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        public static byte[] AddCheckSum(byte[] data) {
            var checkSum = GetCheckSum(data);
            var dataWithCheckSum = ArrayHelpers.ConcatArrays(data, checkSum);

            return dataWithCheckSum;
        }

        public static byte[] Decode(string s) {
            // Decode Base58 string to BigInteger 
            BigInteger intData = 0;

            for (int i = 0; i < s.Length; i++) {
                int digit = Digits.IndexOf(s[i]); //Slow

                if (digit < 0)
                    throw new FormatException(string.Format("Invalid Base58 character `{0}` at position {1}", s[i], i));

                intData = intData * 58 + digit;
            }

            // Encode BigInteger to byte[]
            // Leading zero bytes get encoded as leading `1` characters
            int leadingZeroCount = s.ToCharArray().TakeWhile(c => c == '1').Count();
            var leadingZeros = Enumerable.Repeat((byte) 0, leadingZeroCount);
            var bytesWithoutLeadingZeros = intData.ToByteArray().Reverse() // to big endian
                                                  .SkipWhile(b => b == 0); //strip sign byte
            var result = leadingZeros.Concat(bytesWithoutLeadingZeros).ToArray();
            return result;
        }

        // Throws `FormatException` if s is not a valid Base58 string, or the checksum is invalid
        public static byte[] DecodeWithCheckSum(string s) {
            var dataWithCheckSum = Decode(s);
            var dataWithoutCheckSum = VerifyAndRemoveCheckSum(dataWithCheckSum);

            if (dataWithoutCheckSum == null)
                throw new FormatException("Base58 checksum is invalid");

            return dataWithoutCheckSum;
        }

        public static string Encode(byte[] data) {
            // Decode byte[] to BigInteger
            var intData = data.Aggregate<byte, BigInteger>(0, (current, t) => current * 256 + t);

            // Encode BigInteger to Base58 string
            string result = "";

            while (intData > 0) {
                int remainder = (int) (intData % 58);
                intData /= 58;
                result = Digits[remainder] + result;
            }

            // Append `1` for each leading 0 byte
            for (int i = 0; i < data.Length && data[i] == 0; i++)
                result = '1' + result;

            return result;
        }

        public static string EncodeWithCheckSum(byte[] data) {
            return Encode(AddCheckSum(data));
        }

        private static byte[] GetCheckSum(byte[] data) {
            var hash1 = data.ToSHA256();
            var hash2 = hash1.ToSHA256();

            var result = new byte[CheckSumSizeInBytes];
            Buffer.BlockCopy(hash2, 0, result, 0, result.Length);

            return result;
        }

        public static byte[] VerifyAndRemoveCheckSum(byte[] data) {
            var result = ArrayHelpers.SubArray(data, 0, data.Length - CheckSumSizeInBytes);
            var givenCheckSum = ArrayHelpers.SubArray(data, data.Length - CheckSumSizeInBytes);
            var correctCheckSum = GetCheckSum(result);

            return givenCheckSum.SequenceEqual(correctCheckSum) ? result : null;
        }
    }
}
