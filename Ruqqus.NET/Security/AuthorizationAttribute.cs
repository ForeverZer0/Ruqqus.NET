using System;

namespace Ruqqus.Security
{
    /// <summary>
    /// Metadata attribute to indicate the the permission and scope required to perform an action. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class AuthorizationAttribute : Attribute
    {
        /// <summary>
        /// A flag indicating the authority level the API requires for this functionality.
        /// </summary>
        public readonly AuthorityKind Kind;

        /// <summary>
        /// Gets a flag indicating the required access level of the application for this functionality.
        /// </summary>
        public readonly OAuthScope Scope;

        /// <summary>
        /// A new instance of the <see cref="AuthorizationAttribute"/> class.
        /// </summary>
        /// <param name="kind">A flag indicating the authority level the API requires for this functionality.</param>
        /// <param name="scope">A flag indicating the required access level of the application for this functionality.</param>
        public AuthorizationAttribute(AuthorityKind kind, OAuthScope scope = OAuthScope.None)
        {
            Kind = kind;
            Scope = scope;
        }
    }
}