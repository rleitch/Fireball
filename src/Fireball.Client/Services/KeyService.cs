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
            var totalLength = encodedValues.Sum(v => v.Length);
            Span<byte> combinedArray = new byte[totalLength];

            int offset = 0;
            foreach (var byteArray in encodedValues)
            {
                byteArray.CopyTo(combinedArray[offset..]);
                offset += byteArray.Length;
            }
            
            return ToBaseAlphabet(SHA256.HashData(combinedArray));
        }

        private static string ToBaseAlphabet(byte[] byteArray)
        {
            var sb = new StringBuilder(byteArray.Length);
            foreach (var b in byteArray)
            {
                sb.Append(BaseAlphabet[b % BaseAlphabet.Length]);
            }
            return sb.ToString();
        }
    }
}