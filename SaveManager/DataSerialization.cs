using UnityEngine;

using System;
using System.IO;
using System.Text;
using System.IO.Compression;

public static class DataSerializer{
    public static string SerializeData(object data){
        string json = JsonUtility.ToJson(data);
        return json;
    }

    public static T DeserializeData<T>(string serializedData){
        if(serializedData == null) return default(T);
        return JsonUtility.FromJson<T>(serializedData);
    }

    public static string Encrypt(this string s){
        byte[] buffer = Encoding.UTF8.GetBytes(s);
        var memoryStream = new MemoryStream();
        using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
        {
            gZipStream.Write(buffer, 0, buffer.Length);
        }

        memoryStream.Position = 0;

        var compressedData = new byte[memoryStream.Length];
        memoryStream.Read(compressedData, 0, compressedData.Length);

        var gZipBuffer = new byte[compressedData.Length + 4];
        Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
        Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
        return Convert.ToBase64String(gZipBuffer);
    }

    public static string Decrypt(this string s){
        byte[] gZipBuffer = Convert.FromBase64String(s);
        using (var memoryStream = new MemoryStream())
        {
            int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
            memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

            var buffer = new byte[dataLength];

            memoryStream.Position = 0;
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                gZipStream.Read(buffer, 0, buffer.Length);
            }

            return Encoding.UTF8.GetString(buffer);
        }
    }
}