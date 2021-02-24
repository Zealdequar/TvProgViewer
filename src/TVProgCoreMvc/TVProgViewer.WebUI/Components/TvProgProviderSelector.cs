﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TVProgViewer.Web.Framework.Components;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.WebUI.Models.Common;

namespace TVProgViewer.WebUI.Components
{
    public class TvProgProviderSelectorViewComponent : TvProgViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;

        public TvProgProviderSelectorViewComponent(ICommonModelFactory commonModelFactory)
        {
            _commonModelFactory = commonModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _commonModelFactory.PrepareTvProgProviderSelectorModelAsync();
            return View(model);
        }
    }
}
