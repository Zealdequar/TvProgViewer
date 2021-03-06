﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.WebUI.Areas.Admin.Factories;
using TVProgViewer.Web.Framework.Components;

namespace TVProgViewer.WebUI.Areas.Admin.Components
{
    /// <summary>
    /// Represents a view component that displays an admin widgets
    /// </summary>
    public class AdminWidgetViewComponent : TvProgViewComponent
    {
        #region Fields

        private readonly IWidgetModelFactory _widgetModelFactory;

        #endregion

        #region Ctor

        public AdminWidgetViewComponent(IWidgetModelFactory widgetModelFactory)
        {
            _widgetModelFactory = widgetModelFactory;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>View component result</returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
        {
            //prepare model
            var models = await _widgetModelFactory.PrepareRenderWidgetModelsAsync(widgetZone, additionalData);

            //no data?
            if (!models.Any())
                return Content(string.Empty);

            return View(models);
        }

        #endregion
    }
}