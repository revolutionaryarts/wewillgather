using System.ComponentModel;

namespace Gather.Core.Domain.Common
{
    public enum MessageType
    {

        /// <summary>
        /// Project approval message
        /// </summary>
        [Description("Project Approved")]
        ProjectApproved = 10,

        /// <summary>
        /// Project rejection message
        /// </summary>
        [Description("Project Rejected")]
        ProjectRejected = 15,

        /// <summary>
        /// Project change request approval message
        /// </summary>
        [Description("Project Change Request Approved")]
        ProjectChangeApproved = 20,

        /// <summary>
        /// Project change request approval message
        /// </summary>
        [Description("Project Change Request Rejected")]
        ProjectChangeRejected = 25,

        /// <summary>
        /// Project moderation approval message
        /// </summary>
        [Description("Project Moderation Approved")]
        ProjectModerationApproved = 30,

        /// <summary>
        /// Project change request approval message
        /// </summary>
        [Description("Project Moderation Rejected")]
        ProjectModerationRejected = 35,

        /// <summary>
        /// Project withdrawal request approval message
        /// </summary>
        [Description("Project Withdrawal Approved")]
        ProjectWithdrawalApproved = 40,

        /// <summary>
        /// Project withdrawal request approval message
        /// </summary>
        [Description("Project Withdrawal Rejected")]
        ProjectWithdrawalRejected = 45,

        /// <summary>
        /// Project dispute request approval message
        /// </summary>
        [Description("Project Dispute Approved")]
        ProjectDisputeApproved = 50,

        /// <summary>
        /// Project dispute request rejected message
        /// </summary>
        [Description("Project Dispute Rejected")]
        ProjectDisputeRejected = 55,

        /// <summary>
        /// Project dispute remove owner
        /// </summary>
        [Description("Project Dispute Removed Owner")]
        ProjectDisputeOwnerRemoved = 60,

        /// <summary>
        /// Project recurrence scheduled
        /// </summary>
        [Description("Project Recurrence Scheduled")]
        ProjectRecurrenceScheduled = 65,

        /// <summary>
        /// Comment Removal request approval message
        /// </summary>
        [Description("Comment Removal Approved")]
        CommentRemovalApproved = 70,

        /// <summary>
        /// Comment Removal request approval message
        /// </summary>
        [Description("Comment Removal Rejected")]
        CommentRemovalRejected = 75,

        /// <summary>
        /// Comment Removal request approval message
        /// </summary>
        [Description("New Project Tweet")]
        TweetProjectApproved = 80,

        /// <summary>
        /// Comment Removal request approval message
        /// </summary>
        [Description("More Volunteers Required Project Tweet")]
        TweetProjectMoreVolunteers = 85

    }
}