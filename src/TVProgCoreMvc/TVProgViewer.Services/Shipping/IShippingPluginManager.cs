﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Services.Plugins;

namespace TVProgViewer.Services.Shipping
{
    /// <summary>
    /// Represents a shipping plugin manager
    /// </summary>
    public partial interface IShippingPluginManager : IPluginManager<IShippingRateComputationMethod>
    {
        /// <summary>
        /// Load active shipping providers
        /// </summary>
        /// <param name="user">Filter by user; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <param name="systemName">Filter by shipping provider system name; pass null to load all plugins</param>
        /// <returns>List of active shipping providers</returns>
        Task<IList<IShippingRateComputationMethod>> LoadActivePluginsAsync(User user = null, int storeId = 0, string systemName = null);

        /// <summary>
        /// Check whether the passed shipping provider is active
        /// </summary>
        /// <param name="shippingProvider">Shipping provider to check</param>
        /// <returns>Result</returns>
        bool IsPluginActive(IShippingRateComputationMethod shippingProvider);

        /// <summary>
        /// Check whether the shipping provider with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of shipping provider to check</param>
        /// <param name="user">Filter by user; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>Result</returns>
        Task<bool> IsPluginActiveAsync(string systemName, User user = null, int storeId = 0);
    }
}