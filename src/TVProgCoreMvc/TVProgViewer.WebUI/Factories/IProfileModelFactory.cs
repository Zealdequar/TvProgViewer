﻿using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.WebUI.Models.Profile;

namespace TVProgViewer.WebUI.Factories
{
    /// <summary>
    /// Represents the interface of the profile model factory
    /// </summary>
    public partial interface IProfileModelFactory
    {
        /// <summary>
        /// Prepare the profile index model
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="page">Number of posts page; pass null to disable paging</param>
        /// <returns>Profile index model</returns>
        Task<ProfileIndexModel> PrepareProfileIndexModelAsync(User user, int? page);

        /// <summary>
        /// Prepare the profile info model
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Profile info model</returns>
        Task<ProfileInfoModel> PrepareProfileInfoModelAsync(User user);

        /// <summary>
        /// Prepare the profile posts model
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="page">Number of posts page</param>
        /// <returns>Profile posts model</returns>  
        Task<ProfilePostsModel> PrepareProfilePostsModelAsync(User user, int page);
    }
}
