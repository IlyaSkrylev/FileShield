using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileShield
{
    public class Cryptography
    {
        public static string EncodeString(string input, string key)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(key))
                return string.Empty;

            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] encodedBytes = new byte[inputBytes.Length];

            for (int i = 0; i < inputBytes.Length; i++)
            {
                encodedBytes[i] = (byte)(inputBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return Convert.ToBase64String(encodedBytes);
        }

        public static string DecodeString(string input, string key)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(key))
                return string.Empty;

            byte[] inputBytes = Convert.FromBase64String(input);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] decodedBytes = new byte[inputBytes.Length];

            for (int i = 0; i < inputBytes.Length; i++)
            {
                decodedBytes[i] = (byte)(inputBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return Encoding.UTF8.GetString(decodedBytes);
        }

        public static byte[]? EncodeFile(byte[] input, string key)
        {
            if (input.Length == 0 || string.IsNullOrEmpty(key))
                return null;

            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] encodedBytes = new byte[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                encodedBytes[i] = (byte)(input[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return encodedBytes;
        }

        public static byte[]? DecodeFile(byte[] input, string key)
        {
            if (input.Length == 0 || string.IsNullOrEmpty(key))
                return null;

            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] decodedBytes = new byte[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                decodedBytes[i] = (byte)(input[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return decodedBytes;
        }
    }
}
