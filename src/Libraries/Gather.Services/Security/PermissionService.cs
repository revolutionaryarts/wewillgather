using System;
using System.Collections.Generic;
using System.Linq;
using Gather.Core;
using Gather.Core.Data;
using Gather.Core.Domain.Security;
using Gather.Core.Domain.Users;
using Gather.Services.Users;

namespace Gather.Services.Security
{
    public class PermissionService : IPermissionService
    {

        #region Fields

        private readonly IRepository<PermissionRecord> _permissionRecordRepository;
        private readonly IUserService _userService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructors

        public PermissionService(IRepository<PermissionRecord> permissionPecordRepository, IUserService userService, IWorkContext workContext)
        {
            _permissionRecordRepository = permissionPecordRepository;
            _userService = userService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public virtual void DeletePermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                throw new ArgumentNullException("permission");

            _permissionRecordRepository.Delete(permission);
        }

        /// <summary>
        /// Gets a permission
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        /// <returns>Permission</returns>
        public virtual PermissionRecord GetPermissionRecordById(int permissionId)
        {
            if (permissionId == 0)
                return null;

            return _permissionRecordRepository.GetById(permissionId);
        }

        /// <summary>
        /// Gets a permission
        /// </summary>
        /// <param name="systemName">Permission system name</param>
        /// <returns>Permission</returns>
        public virtual PermissionRecord GetPermissionRecordBySystemName(string systemName)
        {
            if (String.IsNullOrWhiteSpace(systemName))
                return null;

            return GetAllPermissionRecords().FirstOrDefault(p => systemName.Equals(p.SystemName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Gets all permissions
        /// </summary>
        /// <returns>Permissions</returns>
        public virtual IList<PermissionRecord> GetAllPermissionRecords()
        {
            var query = from cr in _permissionRecordRepository.Table
                        orderby cr.Name
                        select cr;
            var permissions = query.ToList();
            return permissions;
        }

        /// <summary>
        /// Inserts a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public virtual void InsertPermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                throw new ArgumentNullException("permission");

            _permissionRecordRepository.Insert(permission);
        }

        /// <summary>
        /// Updates the permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public virtual void UpdatePermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                throw new ArgumentNullException("permission");

            _permissionRecordRepository.Update(permission);
        }

        /// <summary>
        /// Install permissions
        /// </summary>
        public virtual void InstallPermissions()
        {
            var permissionProvider = new PermissionProvider();
            var permissions = permissionProvider.GetPermissions();
            foreach (var permission in permissions)
            {
                var permission1 = GetPermissionRecordBySystemName(permission.SystemName);
                if (permission1 == null)
                {
                    // Map the permissions to the default roles
                    var defaultPermissions = permissionProvider.GetDefaultPermissions();
                    foreach (var defaultPermission in defaultPermissions)
                    {
                        var userRole = _userService.GetUserRoleBySystemName(defaultPermission.UserRoleSystemName);

                        var defaultMappingProvided = (from p in defaultPermission.PermissionRecords
                                                      where p.SystemName == permission.SystemName
                                                      select p).Any();

                        var mappingExists = (from p in userRole.PermissionRecords
                                             where p.SystemName == permission.SystemName
                                             select p).Any();

                        if (defaultMappingProvided && !mappingExists)
                            permission.UserRoles.Add(userRole);
                    }

                    InsertPermissionRecord(permission);
                }
            }
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize(string permissionRecordSystemName)
        {
            if (String.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            var permission = GetPermissionRecordBySystemName(permissionRecordSystemName);
            return Authorize(permission);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize(PermissionRecord permission)
        {
            return Authorize(permission, _workContext.CurrentUser);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <param name="user">User</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize(PermissionRecord permission, User user)
        {
            if (permission == null)
                return false;

            if (user == null)
                return false;

            var userRoles = user.UserRoles.Where(ur => ur.Active);
            foreach (var role in userRoles)
                foreach (var userPermission in role.PermissionRecords)
                    if (userPermission.SystemName.Equals(permission.SystemName, StringComparison.InvariantCultureIgnoreCase))
                        return true;

            return false;
        }

        #endregion

    }
}