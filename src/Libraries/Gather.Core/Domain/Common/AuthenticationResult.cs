namespace Gather.Core.Domain.Common
{
    public enum AuthenticationResult
    {
        /// <summary>
        /// Link a new social media account to an existing profile
        /// </summary>
        LinkAccount,
        /// <summary>
        /// Login/register
        /// </summary>
        LoginOrRegister
    }
}