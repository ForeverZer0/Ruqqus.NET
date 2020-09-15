using System;

namespace Ruqqus.Security
{
    /// <summary>
    /// Metadata attribute to indicate the the permission and scope required to perform an action. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class ScopeAttribute : Attribute
    {
        /// <summary>
        /// Gets a flag indicating the required access level of the application for this functionality.
        /// </summary>
        public readonly OAuthScope Scope;

        /// <summary>
        /// A new instance of the <see cref="ScopeAttribute"/> class.
        /// </summary>
        /// <param name="kind">A flag indicating the authority level (login) required by the application for this functionality.</param>
        /// <param name="scope">A flag indicating the required scope (OAuth) required by the application for this functionality.</param>
        public ScopeAttribute(OAuthScope scope)
        {
            Scope = scope;
        }
    }
}