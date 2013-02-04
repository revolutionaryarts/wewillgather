using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gather.Core.Domain.Security;

namespace Gather.Services.Security
{
    public interface IPermissionProvider
    {
        /// <summary>
        /// Get a list of all permissions
        /// </summary>
        /// <returns>List of PermissionRecord</returns>
        IEnumerable<PermissionRecord> GetPermissions();

        /// <summary>
        /// Get a list of the default user role permissions
        /// </summary>
        /// <returns>List of DefaultPermissionRecord</returns>
        IEnumerable<DefaultPermissionRecord> GetDefaultPermissions();
    }
}