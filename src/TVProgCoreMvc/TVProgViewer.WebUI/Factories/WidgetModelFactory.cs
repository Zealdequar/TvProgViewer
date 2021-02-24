﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using TVProgViewer.Core;
using TVProgViewer.Core.Caching;
using TVProgViewer.Services.Cms;
using TVProgViewer.Services.Users;
using TVProgViewer.Web.Framework.Themes;
using TVProgViewer.WebUI.Infrastructure.Cache;
using TVProgViewer.WebUI.Models.Cms;

namespace TVProgViewer.WebUI.Factories
{
    /// <summary>
    /// Represents the widget model factory
    /// </summary>
    public partial class WidgetModelFactory : IWidgetModelFactory
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IThemeContext _themeContext;
        private readonly IWidgetPluginManager _widgetPluginManager;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public WidgetModelFactory(IUserService userService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IThemeContext themeContext,
            IWidgetPluginManager widgetPluginManager,
            IWorkContext workContext)
        {
            _userService = userService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _themeContext = themeContext;
            _widgetPluginManager = widgetPluginManager;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the render widget models
        /// </summary>
        /// <param name="widgetZone">Name of widget zone</param>
        /// <param name="additionalData">Additional data object</param>
        /// <returns>List of the render widget models</returns>
        public virtual async Task<List<RenderWidgetModel>> PrepareRenderWidgetModelAsync(string widgetZone, object additionalData = null)
        {
            var roles = await _userService.GetUserRoleIdsAsync(await _workContext.GetCurrentUserAsync());

            var cacheKey = _staticCacheManager.PrepareKeyForShortTermCache(TvProgModelCacheDefaults.WidgetModelKey,
                roles, await _storeContext.GetCurrentStoreAsync(), widgetZone, await _themeContext.GetWorkingThemeNameAsync());

            var cachedModels = await _staticCacheManager.GetAsync(cacheKey, async () =>
                (await _widgetPluginManager.LoadActivePluginsAsync(await _workContext.GetCurrentUserAsync(), (await _storeContext.GetCurrentStoreAsync()).Id, widgetZone))
                .Select(widget => new RenderWidgetModel
                {
                    WidgetViewComponentName = widget.GetWidgetViewComponentName(widgetZone),
                    WidgetViewComponentArguments = new RouteValueDictionary { ["widgetZone"] = widgetZone }
                }));

            //"WidgetViewComponentArguments" property of widget models depends on "additionalData".
            //We need to clone the cached model before modifications (the updated one should not be cached)
            var models = cachedModels.Select(renderModel => new RenderWidgetModel
            {
                WidgetViewComponentName = renderModel.WidgetViewComponentName,
                WidgetViewComponentArguments = new RouteValueDictionary { ["widgetZone"] = widgetZone, ["additionalData"] = additionalData }
            }).ToList();

            return models;
        }

        #endregion
    }
}