﻿using TVProgViewer.Core.Caching;
using TVProgViewer.Core.Domain.Messages;

namespace TVProgViewer.Services.Messages
{
    /// <summary>
    /// Represents default values related to messages services
    /// </summary>
    public static partial class TvProgMessageDefaults
    {
        /// <summary>
        /// Gets a key for notifications list from TempDataDictionary
        /// </summary>
        public static string NotificationListKey => "NotificationList";

        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// </remarks>
        public static CacheKey MessageTemplatesAllCacheKey => new CacheKey("TvProg.messagetemplate.all.{0}", TvProgEntityCacheDefaults<MessageTemplate>.AllPrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : template name
        /// {1} : store ID
        /// </remarks>
        public static CacheKey MessageTemplatesByNameCacheKey => new CacheKey("TvProg.messagetemplate.byname.{0}-{1}", MessageTemplatesByNamePrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : template name
        /// </remarks>
        public static string MessageTemplatesByNamePrefix => "TvProg.messagetemplate.byname.{0}";

        #endregion
    }
}