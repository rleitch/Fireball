using System.IO.Compression;
using System.IO;
using System.Text;

namespace Fireball.Common.Extensions
{
    public static class CompressionExtensions
    {
        public static byte[] CompressBytes(this byte[] uncompressedBytes)
        {
            using var memoryStream = new MemoryStream();
            using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
            {
                gzipStream.Write(uncompressedBytes, 0, uncompressedBytes.Length);
            }

            return memoryStream.ToArray();
        }

        public static byte[] DecompressBytes(this byte[] compressedBytes)
        {
            using var compressedStream = new MemoryStream(compressedBytes);
            using var decompressedStream = new MemoryStream();
            using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            {
                gzipStream.CopyTo(decompressedStream);
            }

            return decompressedStream.ToArray();
        }

        public static string DecompressString(this byte[] compressedBytes)
        {
            return Encoding.UTF8.GetString(DecompressBytes(compressedBytes));
        }
    }
}