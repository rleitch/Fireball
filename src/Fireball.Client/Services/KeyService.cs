using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Fireball.Client.Services
{
    public static class KeyService
    {
        private const string BaseAlphabet = "123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";

        public static string BuildKey(params string[] keyParts)
        {
            var encodedValues = keyParts.Select(Encoding.UTF8.GetBytes).ToArray();
            var combinedArray = new byte[encodedValues.Sum(v => v.Length)];

            int offset = 0;
            foreach (var byteArray in encodedValues)
            {
                Buffer.BlockCopy(byteArray, 0, combinedArray, offset, byteArray.Length);
                offset += byteArray.Length;
            }

            return ToBaseAlphabet(Shake256.HashData(combinedArray, 32));
        }

        public static string ToBaseAlphabet(byte[] byteArray)
        {
            var sb = new StringBuilder();
            foreach (var b in byteArray)
            {
                sb.Append(BaseAlphabet[b % BaseAlphabet.Length]);
            }
            return sb.ToString();
        }
    }
}