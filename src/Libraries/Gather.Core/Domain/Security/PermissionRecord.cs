using System.Collections.Generic;
using Gather.Core.Domain.Users;

namespace Gather.Core.Domain.Security
{
    public class PermissionRecord : BaseEntity
    {
        private ICollection<UserRole> _userRoles;

        /// <summary>
        /// Gets or sets the permission name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the permission system name
        /// </summary>
        public virtual string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the permission roles
        /// </summary>
        public virtual ICollection<UserRole> UserRoles
        {
            get { return _userRoles ?? (_userRoles = new List<UserRole>()); }
            set { _userRoles = value; }
        }
    }
}