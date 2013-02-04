using System;
using Gather.Core.Domain.Common;

namespace Gather.Core.Domain.ModerationQueue
{
    public class ModerationQueue : BaseEntity
    {
        /// <summary>
        /// Created by using user id
        /// </summary>
        public virtual int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the user creation date
        /// </summary>
        public virtual DateTime CreatedDate { get; set; }

        /// <summary>
        /// Last modified by using user id
        /// </summary>
        public virtual int? ModeratedBy { get; set; }

        /// <summary>
        /// Gets or sets the last modified date
        /// </summary>
        public virtual DateTime? ModeratedDate { get; set; }

        /// <summary>
        /// Gets or sets the request type identifier
        /// </summary>
        public virtual int RequestTypeId { get; set; }

        /// <summary>
        /// Gets or sets the request type
        /// </summary>
        public virtual ModerationRequestType RequestType
        {
            get
            {
                return (ModerationRequestType)RequestTypeId;
            }
            set
            {
                RequestTypeId = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the request type identifier
        /// </summary>
        public virtual int StatusId { get; set; }

        /// <summary>
        /// Gets or sets the request type
        /// </summary>
        public virtual ModerationStatusType StatusType
        {
            get
            {
                return (ModerationStatusType)StatusId;
            }
            set
            {
                StatusId = (int)value;
            }
        }

        public virtual string Notes { get; set; }
    }
}