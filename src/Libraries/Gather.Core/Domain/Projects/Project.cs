using System;
using System.Collections.Generic;
using Gather.Core.Domain.Comments;
using Gather.Core.Domain.Common;

namespace Gather.Core.Domain.Projects
{
    public class Project : BaseProject
    {
        private ICollection<Comment> _comments;

        /// <summary>
        /// Gets or sets the comments
        /// </summary>
        public virtual ICollection<Comment> Comments
        {
            get { return _comments ?? (_comments = new List<Comment>()); }
            set { _comments = value; }
        } 

        /// <summary>
        /// Gets or sets the Projects email address
        /// </summary>
        public virtual string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the email disclosure identifier
        /// </summary>
        public virtual int EmailDisclosureId { get; set; }

        /// <summary>
        /// Gets or sets the email disclosure level
        /// </summary>
        public virtual DisclosureLevel EmailDisclosureLevel
        {
            get { return (DisclosureLevel) EmailDisclosureId; }
            set { EmailDisclosureId = (int) value; }
        }

        /// <summary>
        /// Gets or sets the recurring flag value
        /// </summary>
        public virtual bool IsRecurring { get; set; }

        /// <summary>
        /// Last moderator approval by user id
        /// </summary>
        public virtual int? LastModeratorApprovalBy { get; set; }

        /// <summary>
        /// Gets or sets the last moderator approval date
        /// </summary>
        public virtual DateTime? LastModeratorApprovalDate { get; set; }

        /// <summary>
        /// Last modified by using user id
        /// </summary>
        public virtual int? LastModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the last modified date
        /// </summary>
        public virtual DateTime? LastModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the number of recurrences
        /// </summary>
        public virtual int Recurrence { get; set; }

        /// <summary>
        /// Gets or sets the interval of recurrence
        /// </summary>
        public virtual int RecurrenceIntervalId { get; set; }

        /// <summary>
        /// Gets or sets the interval of recurrence
        /// </summary>
        public virtual RecurrenceInterval RecurrenceInterval
        {
            get { return (RecurrenceInterval) RecurrenceIntervalId; }
            set { RecurrenceIntervalId = (int) value; }
        }

        /// <summary>
        /// Gets or sets the Status
        /// </summary>
        public virtual int StatusId { get; set; }

        /// <summary>
        /// Gets or sets the Status
        /// </summary>
        public virtual ProjectStatus Status
        {
            get { return (ProjectStatus) StatusId; }
            set { StatusId = (int) value; }
        }

        /// <summary>
        /// Gets or sets the projects telephone
        /// </summary>
        public virtual string Telephone { get; set; }

        /// <summary>
        /// Gets or sets the telephone disclosure identifier
        /// </summary>
        public virtual int TelephoneDisclosureId { get; set; }

        /// <summary>
        /// Gets or sets the telephone disclosure level
        /// </summary>
        public virtual DisclosureLevel TelephoneDisclosureLevel
        {
            get { return (DisclosureLevel) TelephoneDisclosureId; }
            set { TelephoneDisclosureId = (int) value; }
        }

        /// <summary>
        /// Gets or sets the user Twitter profile Id
        /// </summary>
        public virtual string TwitterProfile { get; set; }

        /// <summary>
        /// Gets or sets the Website
        /// </summary>
        public virtual string Website { get; set; }

        /// <summary>
        /// Gets or sets the website disclosure identifier
        /// </summary>
        public virtual int WebsiteDisclosureId { get; set; }

        /// <summary>
        /// Gets or sets the website disclosure level
        /// </summary>
        public virtual DisclosureLevel WebsiteDisclosureLevel
        {
            get { return (DisclosureLevel) WebsiteDisclosureId; }
            set { WebsiteDisclosureId = (int) value; }
        }

        /// <summary>
        /// Moderator notes
        /// </summary>
        public virtual string ModeratorNotes { get; set; }

        /// <summary>
        /// Gets or sets whether the reminder message has been sent
        /// </summary>
        public virtual Boolean ReminderMessageSent { get; set; }

        /// <summary>
        /// Gets or sets whether a requirement for more volunteers alert tweet has been sent
        /// </summary>
        public virtual Boolean AlertMessageSent { get; set; }
    }
}