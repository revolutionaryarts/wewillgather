using System.Collections.Generic;

namespace Gather.Core.Domain.Security
{
    public class DefaultPermissionRecord
    {
        /// <summary>
        /// Gets or sets the user role system name
        /// </summary>
        public string UserRoleSystemName { get; set; }

        /// <summary>
        /// Gets or sets the user role permissions
        /// </summary>
        public IEnumerable<PermissionRecord> PermissionRecords { get; set; }
    }
}