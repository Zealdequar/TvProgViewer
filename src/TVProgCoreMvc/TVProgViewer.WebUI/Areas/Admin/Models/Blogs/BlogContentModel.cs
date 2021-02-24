﻿using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Blogs
{
    /// <summary>
    /// Represents a blog content model
    /// </summary>
    public partial record BlogContentModel : BaseTvProgModel
    {
        #region Ctor

        public BlogContentModel()
        {
            BlogPosts = new BlogPostSearchModel();
            BlogComments = new BlogCommentSearchModel();
            SearchTitle = new BlogPostSearchModel().SearchTitle;
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.List.SearchTitle")]
        public string SearchTitle { get; set; }

        public BlogPostSearchModel BlogPosts { get; set; }

        public BlogCommentSearchModel BlogComments { get; set; }

        #endregion
    }
}
