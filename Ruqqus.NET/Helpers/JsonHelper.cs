using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Ruqqus.Helpers
{
    /// <summary>
    /// A static helper class to facilitate serialization of of JSON streams and strings to/from objects.
    /// </summary>
    /// <remarks>All methods assume the objects are either primitives or implement a data contract.</remarks>
    public static class JsonHelper
    {
        /// <summary>
        /// Deserializes the JSON data specified in <paramref name="filename"/> into an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="filename">The path of a file containing JSON data.</param>
        /// <typeparam name="T">A primitive type or one that implements a data contract.</typeparam>
        /// <returns>The deserialized object instance.</returns>
        public static T Load<T>(string filename)
        {
            using var stream = File.OpenRead(filename);
            return Load<T>(stream);
        }

        /// <summary>
        /// Deserializes the JSON data in <paramref name="stream"/> into an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="stream">A stream open for reading that contains JSON data.</param>
        /// <typeparam name="T">A primitive type or one that implements a data contract.</typeparam>
        /// <returns>The deserialized object instance.</returns>
        public static T Load<T>(Stream stream)
        {
            var json = new DataContractJsonSerializer(typeof(T));
            return (T) json.ReadObject(stream);
        }

        /// <summary>
        /// Serializes an object instance into JSON and saves it the specified <paramref name="filename"/>.
        /// </summary>
        /// <param name="obj">An object to serialize into JSON.</param>
        /// <param name="filename">The path where the data will be written to.</param>
        /// <typeparam name="T">A primitive type or one that implements a data contract.</typeparam>
        public static void Save<T>(T obj, string filename)
        {
            using var stream = File.OpenWrite(filename);
            Save(obj, stream);
        }

        /// <summary>
        /// Serializes an object instance into JSON and saves it the specified <paramref name="stream"/>.
        /// </summary>
        /// <param name="obj">An object to serialize into JSON.</param>
        /// <param name="stream">The stream open for writing where the data will be written to.</param>
        /// <typeparam name="T">A primitive type or one that implements a data contract.</typeparam>
        public static void Save<T>(T obj, Stream stream)
        {
            var json = new DataContractJsonSerializer(typeof(T));
            json.WriteObject(stream, obj);
        }

        /// <summary>
        /// Converts an object to a JSON-formatted string.
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        /// <typeparam name="T">A primitive type or one that implements a data contract.</typeparam>
        /// <returns>The object represented as a JSON string.</returns>
        public static string ToString<T>(T obj)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using var stream = new MemoryStream();
            serializer.WriteObject(stream, obj);
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>
        /// Parses a JSON string into an object instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="jsonString">A valid JSON-formatted string.</param>
        /// <typeparam name="T">A primitive type or one that implements a data contract.</typeparam>
        /// <returns>The deserialized object instance.</returns>
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