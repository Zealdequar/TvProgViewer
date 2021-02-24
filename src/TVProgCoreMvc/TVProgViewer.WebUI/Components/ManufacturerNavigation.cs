﻿using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Components;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Components
{
    public class ManufacturerNavigationViewComponent : TvProgViewComponent
    {
        private readonly CatalogSettings _catalogSettings;
        private readonly ICatalogModelFactory _catalogModelFactory;

        public ManufacturerNavigationViewComponent(CatalogSettings catalogSettings, ICatalogModelFactory catalogModelFactory)
        {
            _catalogSettings = catalogSettings;
            _catalogModelFactory = catalogModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(int currentManufacturerId)
        {
            if (_catalogSettings.ManufacturersBlockItemsToDisplay == 0)
                return Content("");

            var model = await _catalogModelFactory.PrepareManufacturerNavigationModelAsync(currentManufacturerId);
            if (!model.Manufacturers.Any())
                return Content("");

            return View(model);

        }
    }
}