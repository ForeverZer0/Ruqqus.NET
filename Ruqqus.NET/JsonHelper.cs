using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Ruqqus
{
    /// <summary>
    /// A static helper class to facilitate serialization of of JSON streams and strings to/from objects.
    /// </summary>
    /// <remarks>All methods assume the objects are either primitives or implement a data contract.</remarks>
    public static class JsonHelper
    {
        public static T Load<T>(string filename)
        {
            using var stream = File.OpenRead(filename);
            return Load<T>(stream);
        }
        
        public static T Load<T>(Stream stream)
        {
            var json = new DataContractJsonSerializer(typeof(T));
            return (T) json.ReadObject(stream);
        }

        public static void Save<T>(T obj, string filename)
        {
            using var stream = File.OpenWrite(filename);
            Save(obj, stream);
        }

        public static void Save<T>(T obj, Stream stream)
        {
            var json = new DataContractJsonSerializer(typeof(T));
            json.WriteObject(stream, obj);
        }

        public static string ToString<T>(T obj)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using var stream = new MemoryStream();
            serializer.WriteObject(stream, obj);
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        // TODO: Remove?
        public static async Task<string> ToStringAsync<T>(T obj)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            await using var stream = new MemoryStream();
            serializer.WriteObject(stream, obj);
            await stream.FlushAsync();
            stream.Seek(0, SeekOrigin.Begin);
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        public static T Parse<T>(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
                throw new ArgumentNullException(nameof(jsonString));
            
            var serializer = new DataContractJsonSerializer(typeof(T));
            var utf8 = Encoding.UTF8.GetBytes(jsonString);
            using var stream = new MemoryStream(utf8);
            return (T) serializer.ReadObject(stream);
        }
    }
}