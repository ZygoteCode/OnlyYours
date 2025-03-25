using System.Linq;
using System;
using Org.BouncyCastle.Crypto.Digests;
using System.IO.Compression;
using System.IO;

public class Utils
{
    public static byte[] Combine(params byte[][] arrays)
    {
        byte[] ret = new byte[arrays.Sum(x => x.Length)];
        int offset = 0;
        foreach (byte[] data in arrays)
        {
            Buffer.BlockCopy(data, 0, ret, offset, data.Length);
            offset += data.Length;
        }
        return ret;
    }

    public static byte[] GetKeccakHash(byte[] inputData)
    {
        KeccakDigest keccakDigest = new KeccakDigest(512);
        keccakDigest.BlockUpdate(inputData, 0, inputData.Length);
        byte[] inputDataHash = new byte[keccakDigest.GetDigestSize()];
        keccakDigest.DoFinal(inputDataHash, 0);
        return inputDataHash;
    }

    public static byte[] GetPasswordHash(byte[] inputData)
    {
        KeccakDigest keccakDigest = new KeccakDigest(256);
        keccakDigest.BlockUpdate(inputData, 0, inputData.Length);
        byte[] inputDataHash = new byte[keccakDigest.GetDigestSize()];
        keccakDigest.DoFinal(inputDataHash, 0);
        return inputDataHash;
    }

    public static bool CompareByteArrays(byte[] first, byte[] second)
    {
        if (first.Length != second.Length)
        {
            return false;
        }

        for (int i = 0; i < first.Length; i++)
        {
            if (first[i] != second[i])
            {
                return false;
            }
        }

        return true;
    }

    public static byte[] Compress(byte[] data)
    {
        using (var compressedStream = new MemoryStream())
        using (var gzipStream = new GZipStream(compressedStream, CompressionLevel.Optimal, leaveOpen: true))
        {
            gzipStream.Write(data, 0, data.Length);
            gzipStream.Close();
            return compressedStream.ToArray();
        }
    }

    public static byte[] Decompress(byte[] compressedData)
    {
        using (var compressedStream = new MemoryStream(compressedData))
        using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
        using (var resultStream = new MemoryStream())
        {
            gzipStream.CopyTo(resultStream);
            return resultStream.ToArray();
        }
    }
}