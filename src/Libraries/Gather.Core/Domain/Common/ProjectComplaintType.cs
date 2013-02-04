using System.ComponentModel;

namespace Gather.Core.Domain.Common
{
    public enum ProjectComplaintType
    {
        /// <summary>
        /// Comment is abusive
        /// </summary>
        Abusive = 10,

        /// <summary>
        /// Comment is irrelevant
        /// </summary>
        Irrelevant = 15,

        /// <summary>
        /// Comment is inappropriate
        /// </summary>
        Inappropriate = 20,

        /// <summary>
        /// Comment is spam
        /// </summary>
        Spam = 25,

        /// <summary>
        /// Comment violates T&Cs
        /// </summary>
        [Description("Violates T&Cs")]
        ViolatesTerms = 30,

        /// <summary>
        /// Dispute Ownership
        /// </summary>
        [Description("Dispute Ownership")]
        DisputeOwnership = 35
    }
}