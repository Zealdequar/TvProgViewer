﻿using System;
using System.Collections.Generic;
using System.Linq;
using TVProgViewer.Core;
using TVProgViewer.Core.Caching;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Security;
using TVProgViewer.Data;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Localization;
using System.Threading.Tasks;

namespace TVProgViewer.Services.Security
{
    /// <summary>
    /// Permission service
    /// </summary>
    public partial class PermissionService : IPermissionService
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<PermissionRecord> _permissionRecordRepository;
        private readonly IRepository<PermissionRecordUserRoleMapping> _permissionRecordUserRoleMappingRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PermissionService(IUserService userService,
            ILocalizationService localizationService,
            IRepository<PermissionRecord> permissionRecordRepository,
            IRepository<PermissionRecordUserRoleMapping> permissionRecordUserRoleMappingRepository,
            IStaticCacheManager staticCacheManager,
            IWorkContext workContext)
        {
            _userService = userService;
            _localizationService = localizationService;
            _permissionRecordRepository = permissionRecordRepository;
            _permissionRecordUserRoleMappingRepository = permissionRecordUserRoleMappingRepository;
            _staticCacheManager = staticCacheManager;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get permission records by user role identifier
        /// </summary>
        /// <param name="userRoleId">User role identifier</param>
        /// <returns>Permissions</returns>
        protected virtual async Task<IList<PermissionRecord>> GetPermissionRecordsByUserRoleIdAsync(int userRoleId)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(TvProgSecurityDefaults.PermissionRecordsAllCacheKey, userRoleId);

            var query = from pr in _permissionRecordRepository.Table
                        join prcrm in _permissionRecordUserRoleMappingRepository.Table on pr.Id equals prcrm
                            .PermissionRecordId
                        where prcrm.UserRoleId == userRoleId
                        orderby pr.Id
                        select pr;

            return await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());
        }

        /// <summary>
        /// Delete a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        protected virtual async Task DeletePermissionRecordAsync(PermissionRecord permission)
        {
            await _permissionRecordRepository.DeleteAsync(permission);
        }

        /// <summary>
        /// Gets a permission
        /// </summary>
        /// <param name="systemName">Permission system name</param>
        /// <returns>Permission</returns>
        protected virtual async Task<PermissionRecord> GetPermissionRecordBySystemNameAsync(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            var query = from pr in _permissionRecordRepository.Table
                        where pr.SystemName == systemName
                        orderby pr.Id
                        select pr;

            var permissionRecord = await query.FirstOrDefaultAsync();
            return permissionRecord;
        }

        /// <summary>
        /// Inserts a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        protected virtual async Task InsertPermissionRecordAsync(PermissionRecord permission)
        {
            await _permissionRecordRepository.InsertAsync(permission);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all permissions
        /// </summary>
        /// <returns>Permissions</returns>
        public virtual async Task<IList<PermissionRecord>> GetAllPermissionRecordsAsync()
        {
            var permissions = await _permissionRecordRepository.GetAllAsync(query =>
            {
                return from pr in query
                       orderby pr.Name
                       select pr;
            });

            return permissions;
        }

        /// <summary>
        /// Updates the permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public virtual async Task UpdatePermissionRecordAsync(PermissionRecord permission)
        {
            await _permissionRecordRepository.UpdateAsync(permission);
        }

        /// <summary>
        /// Install permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        public virtual async Task InstallPermissionsAsync(IPermissionProvider permissionProvider)
        {
            //install new permissions
            var permissions = permissionProvider.GetPermissions();
            //default user role mappings
            var defaultPermissions = permissionProvider.GetDefaultPermissions().ToList();

            foreach (var permission in permissions)
            {
                var permission1 = await GetPermissionRecordBySystemNameAsync(permission.SystemName);
                if (permission1 != null)
                    continue;

                //new permission (install it)
                permission1 = new PermissionRecord
                {
                    Name = permission.Name,
                    SystemName = permission.SystemName,
                    Category = permission.Category
                };

                //save new permission
                await InsertPermissionRecordAsync(permission1);

                foreach (var defaultPermission in defaultPermissions)
                {
                    var userRole = await _userService.GetUserRoleBySystemNameAsync(defaultPermission.systemRoleName);
                    if (userRole == null)
                    {
                        //new role (save it)
                        userRole = new UserRole
                        {
                            Name = defaultPermission.systemRoleName,
                            Active = true,
                            SystemName = defaultPermission.systemRoleName
                        };
                        await _userService.InsertUserRoleAsync(userRole);
                    }

                    var defaultMappingProvided = defaultPermission.permissions.Any(p => p.SystemName == permission1.SystemName);

                    if (!defaultMappingProvided)
                        continue;

                    await InsertPermissionRecordUserRoleMappingAsync(new PermissionRecordUserRoleMapping { UserRoleId = userRole.Id, PermissionRecordId = permission1.Id });
                }

                //save localization
                await _localizationService.SaveLocalizedPermissionNameAsync(permission1);
            }
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual async Task<bool> AuthorizeAsync(PermissionRecord permission)
        {
            return await AuthorizeAsync(permission, await _workContext.GetCurrentUserAsync());
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <param name="user">User</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual async Task<bool> AuthorizeAsync(PermissionRecord permission, User user)
        {
            if (permission == null)
                return false;

            if (user == null)
                return false;

            return await AuthorizeAsync(permission.SystemName, user);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual async Task<bool> AuthorizeAsync(string permissionRecordSystemName)
        {
            return await AuthorizeAsync(permissionRecordSystemName, await _workContext.GetCurrentUserAsync());
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="user">User</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual async Task<bool> AuthorizeAsync(string permissionRecordSystemName, User user)
        {
            if (string.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            var userRoles = await _userService.GetUserRolesAsync(user);
            foreach (var role in userRoles)
                if (await AuthorizeAsync(permissionRecordSystemName, role.Id))
                    //yes, we have such permission
                    return true;

            //no permission found
            return false;
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="userRoleId">User role identifier</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual async Task<bool> AuthorizeAsync(string permissionRecordSystemName, int userRoleId)
        {
            if (string.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            var key = _staticCacheManager.PrepareKeyForDefaultCache(TvProgSecurityDefaults.PermissionAllowedCacheKey, permissionRecordSystemName, userRoleId);

            return await _staticCacheManager.GetAsync(key, async () =>
            {
                var permissions = await GetPermissionRecordsByUserRoleIdAsync(userRoleId);
                foreach (var permission in permissions)
                    if (permission.SystemName.Equals(permissionRecordSystemName, StringComparison.InvariantCultureIgnoreCase))
                        return true;

                return false;
            });
        }

        /// <summary>
        /// Gets a permission record-user role mapping
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        public virtual async Task<IList<PermissionRecordUserRoleMapping>> GetMappingByPermissionRecordIdAsync(int permissionId)
        {
            var query = _permissionRecordUserRoleMappingRepository.Table;

            query = query.Where(x => x.PermissionRecordId == permissionId);

            return await query.ToListAsync();
        }

        /// <summary>
        /// Delete a permission record-user role mapping
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        /// <param name="userRoleId">User role identifier</param>
        public virtual async Task DeletePermissionRecordUserRoleMappingAsync(int permissionId, int userRoleId)
        {
            var mapping = _permissionRecordUserRoleMappingRepository.Table
                .FirstOrDefault(prcm => prcm.UserRoleId == userRoleId && prcm.PermissionRecordId == permissionId);
            if (mapping is null)
                return;

            await _permissionRecordUserRoleMappingRepository.DeleteAsync(mapping);
        }

        /// <summary>
        /// Inserts a permission record-user role mapping
        /// </summary>
        /// <param name="permissionRecordUserRoleMapping">Permission record-user role mapping</param>
        public virtual async Task InsertPermissionRecordUserRoleMappingAsync(PermissionRecordUserRoleMapping permissionRecordUserRoleMapping)
        {
            await _permissionRecordUserRoleMappingRepository.InsertAsync(permissionRecordUserRoleMapping);
        }

        #endregion
    }
}