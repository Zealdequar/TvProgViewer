﻿using Newtonsoft.Json;
using TVProgViewer.Core.Configuration;
using TVProgViewer.Core.Infrastructure;
using TVProgViewer.Core.Redis;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace TVProgViewer.Services.Plugins
{
    /// <summary>
    /// Represents an information about plugins
    /// </summary>
    public partial class RedisPluginsInfo : PluginsInfo
    {
        #region Fields

        private readonly IDatabase _db;

        #endregion

        #region Ctor

        public RedisPluginsInfo(AppSettings appSettings, ITvProgFileProvider fileProvider, IRedisConnectionWrapper connectionWrapper)
            : base(fileProvider)
        {
            _db = connectionWrapper.GetDatabase(appSettings.RedisConfig.DatabaseId ?? (int)RedisDatabaseNumber.Plugin);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Save plugins info to the redis
        /// </summary>
        public override async Task SaveAsync()
        {
            var text = JsonConvert.SerializeObject(this, Formatting.Indented);
            await _db.StringSetAsync(nameof(RedisPluginsInfo), text);
        }

        /// <summary>
        /// Save plugins info to the redis
        /// </summary>
        public override void Save()
        {
            var text = JsonConvert.SerializeObject(this, Formatting.Indented);
            _db.StringSet(nameof(RedisPluginsInfo), text);
        }

        /// <summary>
        /// Get plugins info
        /// </summary>
        /// <returns>True if data are loaded, otherwise False</returns>
        public override async Task<bool> LoadPluginInfoAsync()
        {
            //try to get plugin info from the JSON file
            var serializedItem = await _db.StringGetAsync(nameof(RedisPluginsInfo));

            var loaded = false;

            if (serializedItem.HasValue)
                loaded = DeserializePluginInfo(serializedItem);

            if (loaded)
                return true;

            if (await base.LoadPluginInfoAsync())
            {
                await SaveAsync();
                loaded = true;
            }

            //delete the plugins info file
            var filePath = _fileProvider.MapPath(TvProgPluginDefaults.PluginsInfoFilePath);
            _fileProvider.DeleteFile(filePath);

            return loaded;
        }

        #endregion
    }
}
