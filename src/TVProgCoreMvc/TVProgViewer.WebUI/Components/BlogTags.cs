﻿using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core.Domain.Blogs;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Components;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Components
{
    public class BlogTagsViewComponent : TvProgViewComponent
    {
        private readonly BlogSettings _blogSettings;
        private readonly IBlogModelFactory _blogModelFactory;

        public BlogTagsViewComponent(BlogSettings blogSettings, IBlogModelFactory blogModelFactory)
        {
            _blogSettings = blogSettings;
            _blogModelFactory = blogModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(int currentCategoryId, int currentProductId)
        {
            if (!_blogSettings.Enabled)
                return Content("");

            var model = await _blogModelFactory.PrepareBlogPostTagListModelAsync();
            return View(model);
        }
    }
}
