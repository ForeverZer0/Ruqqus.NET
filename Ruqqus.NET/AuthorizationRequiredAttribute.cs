using System;

namespace Ruqqus.NET
{

    /// <summary>
    /// Metadata attribute to indicate the the permission and scope required to perform an action. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class AuthorityAttribute : Attribute
    {
        /// <summary>
        /// Gets a flag indicating the authority level the API requires for this functionality.
        /// </summary>
        public AuthorityKind Kind { get; }
        
        /// <summary>
        /// Gets a flag indicating the required access level of the application for this functionality.
        /// </summary>
        public OAuthScope Scope { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="AuthorityAttribute"/> class.
        /// </summary>
        /// <param name="kind">A flag indicating the authority level the API requires for this functionality.</param>
        /// <param name="scope">A flag indicating the required access level of the application for this functionality.</param>
        public AuthorityAttribute(AuthorityKind kind, OAuthScope scope = OAuthScope.None)
        {
            Kind = kind;
            Scope = scope;
        }
    }
}