﻿using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.WebUI.Areas.Admin.Models.Users;

namespace TVProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the user role model factory
    /// </summary>
    public partial interface IUserRoleModelFactory
    {
        /// <summary>
        /// Prepare user role search model
        /// </summary>
        /// <param name="searchModel">User role search model</param>
        /// <returns>User role search model</returns>
        Task<UserRoleSearchModel> PrepareUserRoleSearchModelAsync(UserRoleSearchModel searchModel);

        /// <summary>
        /// Prepare paged user role list model
        /// </summary>
        /// <param name="searchModel">User role search model</param>
        /// <returns>User role list model</returns>
        Task<UserRoleListModel> PrepareUserRoleListModelAsync(UserRoleSearchModel searchModel);

        /// <summary>
        /// Prepare user role model
        /// </summary>
        /// <param name="model">User role model</param>
        /// <param name="userRole">User role</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>User role model</returns>
        Task<UserRoleModel> PrepareUserRoleModelAsync(UserRoleModel model, UserRole userRole, bool excludeProperties = false);

        /// <summary>
        /// Prepare user role product search model
        /// </summary>
        /// <param name="searchModel">User role product search model</param>
        /// <returns>User role product search model</returns>
        Task<UserRoleProductSearchModel> PrepareUserRoleProductSearchModelAsync(UserRoleProductSearchModel searchModel);

        /// <summary>
        /// Prepare paged user role product list model
        /// </summary>
        /// <param name="searchModel">User role product search model</param>
        /// <returns>User role product list model</returns>
        Task<UserRoleProductListModel> PrepareUserRoleProductListModelAsync(UserRoleProductSearchModel searchModel);
    }
}