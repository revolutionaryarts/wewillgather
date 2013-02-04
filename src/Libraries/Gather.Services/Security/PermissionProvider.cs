using System.Collections.Generic;
using Gather.Core.Domain.Security;
using Gather.Core.Domain.Users;

namespace Gather.Services.Security
{
    public class PermissionProvider : IPermissionProvider
    {

        #region Fields

        public static readonly PermissionRecord AccessAdminArea = new PermissionRecord { Name = "Access admin area", SystemName = "AccessAdminArea" };
        public static readonly PermissionRecord AddProjects = new PermissionRecord { Name = "Add projects", SystemName = "AddProjects" };
        public static readonly PermissionRecord DeleteModerationRequests = new PermissionRecord { Name = "Delete Moderation Request", SystemName = "DeleteModetationRequests" };
        public static readonly PermissionRecord EditAdmins = new PermissionRecord { Name = "Edit Admins", SystemName = "EditAdmins" };
        public static readonly PermissionRecord ManageCategories = new PermissionRecord { Name = "Manage categories", SystemName = "ManageCategories" };
        public static readonly PermissionRecord ManageComments = new PermissionRecord { Name = "Manage comments", SystemName = "ManageComments" };
        public static readonly PermissionRecord ManageLocations = new PermissionRecord { Name = "Manage locations", SystemName = "ManageLocations" };
        public static readonly PermissionRecord ManageModerationQueue = new PermissionRecord { Name = "Manage moderation queue", SystemName = "ManageModerationQueue" };
        public static readonly PermissionRecord ManagePages = new PermissionRecord { Name = "Manage pages", SystemName = "ManagePages" };
        public static readonly PermissionRecord ManageProjects = new PermissionRecord { Name = "Manage projects", SystemName = "ManageProjects" };
        public static readonly PermissionRecord ManageSettings = new PermissionRecord { Name = "Manage settings", SystemName = "ManageSettings" };
        public static readonly PermissionRecord ManageSiteOwner = new PermissionRecord { Name = "Manage site owner", SystemName = "ManageSiteOwner" };
        public static readonly PermissionRecord ManageSuccessStories = new PermissionRecord { Name = "Manage success stories", SystemName = "ManageSuccessStories" };
        public static readonly PermissionRecord ManageUserRoles = new PermissionRecord { Name = "Manage user roles", SystemName = "ManageUserRoles" };
        public static readonly PermissionRecord ManageUsers = new PermissionRecord { Name = "Manage users", SystemName = "ManageUsers" };
        public static readonly PermissionRecord PromoteUsers = new PermissionRecord { Name = "Promote users", SystemName = "PromoteUsers" };
        
        #endregion

        #region Methods

        /// <summary>
        /// Get a list of all permissions
        /// </summary>
        /// <returns>List of PermissionRecord</returns>
        public IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                AccessAdminArea,
                AddProjects,
                EditAdmins,
                ManageCategories,
                ManageComments,
                ManageLocations,
                ManageModerationQueue,
                ManagePages,
                ManageProjects,
                ManageSettings,
                ManageSiteOwner,
                ManageSuccessStories,
                ManageUserRoles,
                ManageUsers,
                PromoteUsers
            };
        }

        /// <summary>
        /// Get a list of the default user role permissions
        /// </summary>
        /// <returns>List of DefaultPermissionRecord</returns>
        public IEnumerable<DefaultPermissionRecord> GetDefaultPermissions()
        {
            return new[]
            {
                new DefaultPermissionRecord
                {
                    UserRoleSystemName = SystemUserRoleNames.SiteOwner,
                    PermissionRecords = new[]
                    {
                        AccessAdminArea,
                        AddProjects,
                        EditAdmins,
                        ManageCategories,
                        ManageComments,
                        ManageLocations,
                        ManageModerationQueue,
                        ManagePages,
                        ManageProjects,
                        ManageSettings,
                        ManageSiteOwner,
                        ManageSuccessStories,
                        ManageUserRoles,
                        ManageUsers,
                        PromoteUsers,
                        DeleteModerationRequests
                    }
                },
                new DefaultPermissionRecord
                {
                    UserRoleSystemName = SystemUserRoleNames.Administrators,
                    PermissionRecords = new[]
                    {
                        AccessAdminArea,
                        AddProjects,
                        EditAdmins,
                        ManageCategories,
                        ManageComments,
                        ManageLocations,
                        ManageModerationQueue,
                        ManagePages,
                        ManageProjects,
                        ManageSettings,
                        ManageSuccessStories,
                        ManageUsers,
                        PromoteUsers
                    }
                },
                new DefaultPermissionRecord
                {
                    UserRoleSystemName = SystemUserRoleNames.Moderators,
                    PermissionRecords = new[]
                    {
                        AccessAdminArea,
                        AddProjects,
                        ManageComments,
                        ManageModerationQueue
                    }
                },
                new DefaultPermissionRecord
                {
                    UserRoleSystemName = SystemUserRoleNames.Members,
                    PermissionRecords = new[]
                    {
                        AddProjects
                    }
                }
            };
        }

        #endregion

    }
}