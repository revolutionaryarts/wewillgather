using System.ComponentModel;

namespace Gather.Core.Domain.Common
{
    public enum ModerationRequestType
    {
        /// <summary>
        /// Project Approval
        /// </summary>
        [Description("Action Approval Request")]
        ProjectApproval = 10,
        /// <summary>
        /// Project Change Request
        /// </summary>
        [Description("Action Content Change Request")]
        ProjectChange = 15,
        /// <summary>
        /// Project Comments
        /// </summary>
        [Description("Action Comment Moderation Request")]
        ProjectComment = 20,
        /// <summary>
        /// Project Moderation
        /// </summary>
        [Description("Action Moderation Request")]
        ProjectModeration = 25,
        /// <summary>
        /// Project Withdrawal
        /// </summary>
        [Description("Action Withdrawal Request")]
        ProjectWithdrawal = 30

    }
}
