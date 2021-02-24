﻿using System.Threading.Tasks;

namespace TVProgViewer.Services.Plugins
{
    /// <summary>
    /// Interface denoting plug-in attributes that are displayed throughout 
    /// the editing interface.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        string GetConfigurationPageUrl();

        /// <summary>
        /// Gets or sets the plugin descriptor
        /// </summary>
        PluginDescriptor PluginDescriptor { get; set; }

        /// <summary>
        /// Install plugin
        /// </summary>
        Task InstallAsync();

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        Task UninstallAsync();

        /// <summary>
        /// Update plugin
        /// </summary>
        /// <param name="currentVersion">Current version of plugin</param>
        /// <param name="targetVersion">New version of plugin</param>
        Task UpdateAsync(string currentVersion, string targetVersion);

        /// <summary>
        /// Prepare plugin to the uninstallation
        /// </summary>
        Task PreparePluginToUninstallAsync();
    }
}
