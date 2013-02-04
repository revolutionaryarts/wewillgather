using System.ComponentModel;

namespace Gather.Core.Domain.Common
{
    public enum ProjectStatus
    {
        /// <summary>
        /// Draft
        /// </summary>
        [Description("Draft")]
        Draft = 10,
        /// <summary>
        /// Pending Approval
        /// </summary>
        [Description("Pending Approval")]
        PendingApproval = 15,
        /// <summary>
        /// Open
        /// </summary>
        [Description("Open")]
        Open = 20,
        /// <summary>
        /// Closed
        /// </summary>
        [Description("Closed")]
        Closed = 25,
        /// <summary>
        /// In Progress
        /// </summary>
        [Description("In Progress")]
        InProgress = 30,
        /// <summary>
        /// Rejected
        /// </summary>
        [Description("Rejected")]
        Rejected = 35,
        /// <summary>
        /// Deleted
        /// </summary>
        [Description("Deleted")]
        Deleted = 40,
        /// <summary>
        /// Banned
        /// </summary>
        [Description("Banned")]
        Banned = 45,
        /// <summary>
        /// Withdrawn
        /// </summary>
        [Description("Withdrawn")]
        Withdrawn = 50,
        /// <summary>
        /// Pending Change Approval
        /// </summary>
        [Description("Pending Change Approval")]
        PendingChangeApproval = 55
    }
}