#region
using System;

#endregion

namespace BlockCypher.Helpers {
    public static class Base58Helper {
        private const string Base58characters = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        public static byte[] Decode(string source) {
            int i = 0;
            int zeros = 0;

            while (i < source.Length) {
                if (source[i] == 0 || !Char.IsWhiteSpace(source[i]))
                    break;

                i++;
            }

            while (source[i] == '1') {
                zeros++;
                i++;
            }

            var b256 = new byte[(source.Length - i) * 733 / 1000 + 1];

            while (i < source.Length && !Char.IsWhiteSpace(source[i])) {
                int ch = Base58characters.IndexOf(source[i]);

                if (ch == -1)
                    return new byte[0];

                int carry = Base58characters.IndexOf(source[i]);

                for (int k = b256.Length - 1; k > 0; k--) {
                    carry += 58 * b256[k];
                    b256[k] = (byte) (carry % 256);
                    carry /= 256;
                }

                i++;
            }

            while (i < source.Length && Char.IsWhiteSpace(source[i]))
                i++;

            if (i != source.Length)
                return new byte[0];

            int j = 0;

            while (j < b256.Length && b256[j] == 0)
                j++;

            var destination = new byte[zeros + (b256.Length - j)];

            for (int kk = 0; kk < destination.Length; kk++) {
                if (kk < zeros)
                    destination[kk] = 0x00;
                else
                    destination[kk] = b256[j++];
            }

            return destination;
        }
    }
}