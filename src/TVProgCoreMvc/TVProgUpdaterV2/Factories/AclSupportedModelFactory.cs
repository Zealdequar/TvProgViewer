﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Security;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Security;
using TVProgViewer.TVProgUpdaterV2.Models;
using System.Threading.Tasks;

namespace TVProgViewer.TVProgUpdaterV2.Factories
{
    /// <summary>
    /// Represents the base implementation of the factory of model which supports access control list (ACL)
    /// </summary>
    public partial class AclSupportedModelFactory : IAclSupportedModelFactory
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly IUserService _userService;

        #endregion

        #region Ctor

        public AclSupportedModelFactory(IAclService aclService,
            IUserService userService)
        {
            _aclService = aclService;
            _userService = userService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare selected and all available user roles for the passed model
        /// </summary>
        /// <typeparam name="TModel">ACL supported model type</typeparam>
        /// <param name="model">Model</param>
        public virtual async Task PrepareModelUserRolesAsync<TModel>(TModel model) where TModel : IAclSupportedModel
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //prepare available user roles
            var availableRoles = await _userService.GetAllUserRolesAsync(showHidden: true);
            model.AvailableUserRoles = availableRoles.Select(role => new SelectListItem
            {
                Text = role.Name,
                Value = role.Id.ToString(),
                Selected = model.SelectedUserRoleIds.Contains(role.Id)
            }).ToList();
        }

        /// <summary>
        /// Prepare selected and all available user roles for the passed model by ACL mappings
        /// </summary>
        /// <typeparam name="TModel">ACL supported model type</typeparam>
        /// <typeparam name="TEntity">ACL supported entity type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="entity">Entity</param>
        /// <param name="ignoreAclMappings">Whether to ignore existing ACL mappings</param>
        public virtual async Task PrepareModelUserRolesAsync<TModel, TEntity>(TModel model, TEntity entity, bool ignoreAclMappings)
            where TModel : IAclSupportedModel where TEntity : BaseEntity, IAclSupported
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //prepare user roles with granted access
            if (!ignoreAclMappings && entity != null)
                model.SelectedUserRoleIds = (await _aclService.GetUserRoleIdsWithAccessAsync(entity)).ToList();

            await PrepareModelUserRolesAsync(model);
        }

        #endregion
    }
}