﻿using TVProgViewer.Core.Domain.Localization;
using TVProgViewer.Services.Caching;
using System.Threading.Tasks;

namespace TVProgViewer.Services.Localization.Caching
{
    /// <summary>
    /// Represents a language cache event consumer
    /// </summary>
    public partial class LanguageCacheEventConsumer : CacheEventConsumer<Language>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(Language entity)
        {
            await RemoveAsync(TvProgLocalizationDefaults.LocaleStringResourcesAllPublicCacheKey, entity);
            await RemoveAsync(TvProgLocalizationDefaults.LocaleStringResourcesAllAdminCacheKey, entity);
            await RemoveAsync(TvProgLocalizationDefaults.LocaleStringResourcesAllCacheKey, entity);
            await RemoveByPrefixAsync(TvProgLocalizationDefaults.LocaleStringResourcesByNamePrefix, entity);
        }
    }
}