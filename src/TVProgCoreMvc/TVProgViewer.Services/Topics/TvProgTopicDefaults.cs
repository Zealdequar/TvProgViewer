﻿using TVProgViewer.Core.Caching;
using TVProgViewer.Core.Domain.Topics;

namespace TVProgViewer.Services.Topics
{
    public static partial class TvProgTopicDefaults
    {
        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : show hidden?
        /// {2} : include in top menu?
        /// </remarks>
        public static CacheKey TopicsAllCacheKey => new CacheKey("TvProg.topic.all.{0}-{1}-{2}", TvProgEntityCacheDefaults<Topic>.AllPrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : show hidden?
        /// {2} : include in top menu?
        /// {3} : user role IDs hash
        /// </remarks>
        public static CacheKey TopicsAllWithACLCacheKey => new CacheKey("TvProg.topic.all.withacl.{0}-{1}-{2}-{3}", TvProgEntityCacheDefaults<Topic>.AllPrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : topic system name
        /// {1} : store id
        /// {2} : user roles Ids hash
        /// </remarks>
        public static CacheKey TopicBySystemNameCacheKey => new CacheKey("TvProg.topic.bysystemname.{0}-{1}-{2}", TopicBySystemNamePrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : topic system name
        /// </remarks>
        public static string TopicBySystemNamePrefix => "TvProg.topic.bysystemname.{0}";

        #endregion
    }
}