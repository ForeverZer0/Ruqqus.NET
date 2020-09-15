using System;
using System.IO;
using System.Runtime.Serialization;
using Ruqqus.Helpers;
using Ruqqus.Security;

namespace Ruqqus
{
    /// <summary>
    /// Abstract base class for objects that can be saved/loaded in JSON format.
    /// </summary>
    /// <typeparam name="T">A type that is decorated with a <see cref="DataContractAttribute"/>.</typeparam>
    [DataContract][KnownType("GetKnownTypes")]
    public abstract class JsonObject<T>
    {
        /// <summary>
        /// Loads a token stored in JSON format from the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The stream to the file to load.</param>
        /// <returns>A new instance of the object created from the JSON data.</returns>
        public static T LoadJson(string path) => JsonHelper.Load<T>(path);
        
        /// <summary>
        /// Loads a token stored in JSON format from the specified <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">A stream that is open for reading to load from.</param>
        /// <returns>A new instance of the object created from the JSON data.</returns>
        public static T LoadJson(Stream stream) => JsonHelper.Load<T>(stream);

        /// <summary>
        /// Serializes the object as JSON format into a file at the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">A stream where the object will be written to.</param>
        public void SaveJson([NotNull] string path) => JsonHelper.Save(this, path);
        
        /// <summary>
        /// Serializes the object as JSON format into the specified <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">A stream open for writing that the object will be written to.</param>
        public void SaveJson([NotNull] Stream stream) => JsonHelper.Save(this, stream);

        /// <summary>
        /// Returns the the object represented as a JSON string.
        /// </summary>
        /// <returns>The object represented as a JSON string.</returns>
        public string ToJson() => JsonHelper.ToString(this);

        /// <summary>
        /// Parses a JSON string into an object instance.
        /// </summary>
        /// <param name="jsonString">A valid JSON-formatted string.</param>
        /// <returns>The deserialized object instance.</returns>
        public static T FromJson([NotNull] string jsonString) => JsonHelper.Parse<T>(jsonString);

        /// <summary>
        /// Returns array of types as a hints for the DataContract serializer.
        /// </summary>
        /// <returns>An array of types that implement data contracts..</returns>
        private static Type[] GetKnownTypes()
        {
            return new[]
            {
                typeof(Token), 
                typeof(ClientInfo), 
                typeof(ItemBase), 
                typeof(Submission), 
                typeof(Post), 
                typeof(Comment),
                typeof(Guild),
                typeof(Title),
                typeof(Badge),
                typeof(User)
            };
        }
    }
}