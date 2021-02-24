﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core.Domain.Blogs;
using TVProgViewer.Services.Blogs;
using TVProgViewer.Services.Events;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Logging;
using TVProgViewer.Services.Messages;
using TVProgViewer.Services.Security;
using TVProgViewer.Services.Seo;
using TVProgViewer.Services.Stores;
using TVProgViewer.WebUI.Areas.Admin.Factories;
using TVProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TVProgViewer.WebUI.Areas.Admin.Models.Blogs;
using TVProgViewer.Web.Framework.Mvc;
using TVProgViewer.Web.Framework.Mvc.Filters;
using TVProgViewer.Core.Events;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Areas.Admin.Controllers
{
    public partial class BlogController : BaseAdminController
    {
        #region Fields

        private readonly IBlogModelFactory _blogModelFactory;
        private readonly IBlogService _blogService;
        private readonly IUserActivityService _userActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;

        #endregion

        #region Ctor

        public BlogController(IBlogModelFactory blogModelFactory,
            IBlogService blogService,
            IUserActivityService userActivityService,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IUrlRecordService urlRecordService)
        {
            _blogModelFactory = blogModelFactory;
            _blogService = blogService;
            _userActivityService = userActivityService;
            _eventPublisher = eventPublisher;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _urlRecordService = urlRecordService;
        }

        #endregion

        #region Utilities

        protected virtual async Task SaveStoreMappingsAsync(BlogPost blogPost, BlogPostModel model)
        {
            blogPost.LimitedToStores = model.SelectedStoreIds.Any();
            await _blogService.UpdateBlogPostAsync(blogPost);

            var existingStoreMappings = await _storeMappingService.GetStoreMappingsAsync(blogPost);
            var allStores = await _storeService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        await _storeMappingService.InsertStoreMappingAsync(blogPost, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        await _storeMappingService.DeleteStoreMappingAsync(storeMappingToDelete);
                }
            }
        }

        #endregion

        #region Methods        

        #region Blog posts

        public virtual IActionResult Index()
        {
            return RedirectToAction("BlogPosts");
        }

        public virtual async Task<IActionResult> BlogPosts(int? filterByBlogPostId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            //prepare model
            var model = await _blogModelFactory.PrepareBlogContentModelAsync(new BlogContentModel(), filterByBlogPostId);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(BlogPostSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _blogModelFactory.PrepareBlogPostListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> BlogPostCreate()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            //prepare model
            var model = await _blogModelFactory.PrepareBlogPostModelAsync(new BlogPostModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> BlogPostCreate(BlogPostModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var blogPost = model.ToEntity<BlogPost>();
                blogPost.CreatedOnUtc = DateTime.UtcNow;
                await _blogService.InsertBlogPostAsync(blogPost);

                //activity log
                await _userActivityService.InsertActivityAsync("AddNewBlogPost",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewBlogPost"), blogPost.Id), blogPost);

                //search engine name
                var seName = await _urlRecordService.ValidateSeNameAsync(blogPost, model.SeName, model.Title, true);
                await _urlRecordService.SaveSlugAsync(blogPost, seName, blogPost.LanguageId);

                //Stores
                await SaveStoreMappingsAsync(blogPost, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Blog.BlogPosts.Added"));

                if (!continueEditing)
                    return RedirectToAction("BlogPosts");

                return RedirectToAction("BlogPostEdit", new { id = blogPost.Id });
            }

            //prepare model
            model = await _blogModelFactory.PrepareBlogPostModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> BlogPostEdit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            //try to get a blog post with the specified id
            var blogPost = await _blogService.GetBlogPostByIdAsync(id);
            if (blogPost == null)
                return RedirectToAction("BlogPosts");

            //prepare model
            var model = await _blogModelFactory.PrepareBlogPostModelAsync(null, blogPost);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> BlogPostEdit(BlogPostModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            //try to get a blog post with the specified id
            var blogPost = await _blogService.GetBlogPostByIdAsync(model.Id);
            if (blogPost == null)
                return RedirectToAction("BlogPosts");

            if (ModelState.IsValid)
            {
                blogPost = model.ToEntity(blogPost);
                await _blogService.UpdateBlogPostAsync(blogPost);

                //activity log
                await _userActivityService.InsertActivityAsync("EditBlogPost",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditBlogPost"), blogPost.Id), blogPost);

                //search engine name
                var seName = await _urlRecordService.ValidateSeNameAsync(blogPost, model.SeName, model.Title, true);
                await _urlRecordService.SaveSlugAsync(blogPost, seName, blogPost.LanguageId);

                //Stores
                await SaveStoreMappingsAsync(blogPost, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Blog.BlogPosts.Updated"));

                if (!continueEditing)
                    return RedirectToAction("BlogPosts");

                return RedirectToAction("BlogPostEdit", new { id = blogPost.Id });
            }

            //prepare model
            model = await _blogModelFactory.PrepareBlogPostModelAsync(model, blogPost, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            //try to get a blog post with the specified id
            var blogPost = await _blogService.GetBlogPostByIdAsync(id);
            if (blogPost == null)
                return RedirectToAction("BlogPosts");

            await _blogService.DeleteBlogPostAsync(blogPost);

            //activity log
            await _userActivityService.InsertActivityAsync("DeleteBlogPost",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteBlogPost"), blogPost.Id), blogPost);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Blog.BlogPosts.Deleted"));

            return RedirectToAction("BlogPosts");
        }

        #endregion

        #region Comments

        public virtual async Task<IActionResult> BlogComments(int? filterByBlogPostId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            //try to get a blog post with the specified id
            var blogPost = await _blogService.GetBlogPostByIdAsync(filterByBlogPostId ?? 0);
            if (blogPost == null && filterByBlogPostId.HasValue)
                return RedirectToAction("BlogComments");

            //prepare model
            var model = await _blogModelFactory.PrepareBlogCommentSearchModelAsync(new BlogCommentSearchModel(), blogPost);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Comments(BlogCommentSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _blogModelFactory.PrepareBlogCommentListModelAsync(searchModel, searchModel.BlogPostId);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CommentUpdate(BlogCommentModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            //try to get a blog comment with the specified id
            var comment = await _blogService.GetBlogCommentByIdAsync(model.Id)
                ?? throw new ArgumentException("No comment found with the specified id");

            var previousIsApproved = comment.IsApproved;

            //fill entity from model
            comment = model.ToEntity(comment);

            await _blogService.UpdateBlogCommentAsync(comment);

            //raise event (only if it wasn't approved before and is approved now)
            if (!previousIsApproved && comment.IsApproved)
                await _eventPublisher.PublishAsync(new BlogCommentApprovedEvent(comment));

            //activity log
            await _userActivityService.InsertActivityAsync("EditBlogComment",
               string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditBlogComment"), comment.Id), comment);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> CommentDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            //try to get a blog comment with the specified id
            var comment = await _blogService.GetBlogCommentByIdAsync(id)
                ?? throw new ArgumentException("No comment found with the specified id", nameof(id));

            await _blogService.DeleteBlogCommentAsync(comment);

            //activity log
            await _userActivityService.InsertActivityAsync("DeleteBlogPostComment",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteBlogPostComment"), comment.Id), comment);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelectedComments(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            if (selectedIds == null)
                return Json(new { Result = true });

            var comments = await _blogService.GetBlogCommentsByIdsAsync(selectedIds.ToArray());

            await _blogService.DeleteBlogCommentsAsync(comments);
            //activity log
            foreach (var blogComment in comments)
            {
                await _userActivityService.InsertActivityAsync("DeleteBlogPostComment",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteBlogPostComment"), blogComment.Id), blogComment);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ApproveSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            if (selectedIds == null)
                return Json(new { Result = true });

            //filter not approved comments
            var blogComments = (await _blogService.GetBlogCommentsByIdsAsync(selectedIds.ToArray())).Where(comment => !comment.IsApproved);

            foreach (var blogComment in blogComments)
            {
                blogComment.IsApproved = true;

                await _blogService.UpdateBlogCommentAsync(blogComment);

                //raise event 
                await _eventPublisher.PublishAsync(new BlogCommentApprovedEvent(blogComment));

                //activity log
                await _userActivityService.InsertActivityAsync("EditBlogComment",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditBlogComment"), blogComment.Id), blogComment);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> DisapproveSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            if (selectedIds == null)
                return Json(new { Result = true });

            //filter approved comments
            var blogComments = (await _blogService.GetBlogCommentsByIdsAsync(selectedIds.ToArray())).Where(comment => comment.IsApproved);

            foreach (var blogComment in blogComments)
            {
                blogComment.IsApproved = false;

                await _blogService.UpdateBlogCommentAsync(blogComment);

                //activity log
                await _userActivityService.InsertActivityAsync("EditBlogComment",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditBlogComment"), blogComment.Id), blogComment);
            }

            return Json(new { Result = true });
        }

        #endregion

        #endregion
    }
}