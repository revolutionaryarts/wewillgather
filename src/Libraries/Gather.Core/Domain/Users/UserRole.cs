using System;
using System.Collections.Generic;
using Gather.Core.Domain.Security;

namespace Gather.Core.Domain.Users
{
    public class UserRole : BaseEntity
    {

        private ICollection<PermissionRecord> _permissionRecords;

        /// <summary>
        /// Gets or sets the created by user id
        /// </summary>
        public virtual int? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the created date
        /// </summary>
        public virtual DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if the role is a core role
        /// </summary>
        public virtual bool IsSystemRole { get; set; }

        /// <summary>
        /// Gets or sets the last modified user id
        /// </summary>
        public virtual int? LastModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the last modified date
        /// </summary>
        public virtual DateTime? LastModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the role name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the role permission records
        /// </summary>
        public virtual ICollection<PermissionRecord> PermissionRecords
        {
            get { return _permissionRecords ?? (_permissionRecords = new List<PermissionRecord>()); }
            set { _permissionRecords = value; }
        }

        /// <summary>
        /// Gets or sets the role system name
        /// </summary>
        public virtual string SystemName { get; set; }

    }
}