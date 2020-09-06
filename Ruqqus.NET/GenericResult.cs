using System.Runtime.Serialization;

namespace Ruqqus
{
    /// <summary>
    /// Basic response that simply indicates if an error occured or not.
    /// </summary>
    [DataContract]
    public class GenericResult
    {
        /// <summary>
        /// Gets a message explaining the error.
        /// </summary>
        [DataMember(Name = "error")]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets a value indicating if an error was returned.
        /// </summary>
        public bool IsError => string.IsNullOrEmpty(ErrorMessage);
    }
}