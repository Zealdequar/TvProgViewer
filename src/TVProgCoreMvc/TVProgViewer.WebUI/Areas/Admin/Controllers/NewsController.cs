﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core.Domain.News;
using TVProgViewer.Services.Events;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Logging;
using TVProgViewer.Services.Messages;
using TVProgViewer.Services.News;
using TVProgViewer.Services.Security;
using TVProgViewer.Services.Seo;
using TVProgViewer.Services.Stores;
using TVProgViewer.WebUI.Areas.Admin.Factories;
using TVProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TVProgViewer.WebUI.Areas.Admin.Models.News;
using TVProgViewer.Web.Framework.Mvc;
using TVProgViewer.Web.Framework.Mvc.Filters;
using TVProgViewer.Core.Events;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Areas.Admin.Controllers
{
    public partial class NewsController : BaseAdminController
    {
        #region Fields

        private readonly IUserActivityService _userActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly INewsModelFactory _newsModelFactory;
        private readonly INewsService _newsService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;

        #endregion

        #region Ctor

        public NewsController(IUserActivityService userActivityService,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            INewsModelFactory newsModelFactory,
            INewsService newsService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IUrlRecordService urlRecordService)
        {
            _userActivityService = userActivityService;
            _eventPublisher = eventPublisher;
            _localizationService = localizationService;
            _newsModelFactory = newsModelFactory;
            _newsService = newsService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _urlRecordService = urlRecordService;
        }

        #endregion

        #region Utilities

        protected virtual async Task SaveStoreMappingsAsync(NewsItem newsItem, NewsItemModel model)
        {
            newsItem.LimitedToStores = model.SelectedStoreIds.Any();
            await _newsService.UpdateNewsAsync(newsItem);

            var existingStoreMappings = await _storeMappingService.GetStoreMappingsAsync(newsItem);
            var allStores = await _storeService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        await _storeMappingService.InsertStoreMappingAsync(newsItem, store.Id);
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

        #region News items

        public virtual IActionResult Index()
        {
            return RedirectToAction("NewsItems");
        }

        public virtual async Task<IActionResult> NewsItems(int? filterByNewsItemId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //prepare model
            var model = await _newsModelFactory.PrepareNewsContentModelAsync(new NewsContentModel(), filterByNewsItemId);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(NewsItemSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _newsModelFactory.PrepareNewsItemListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> NewsItemCreate()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //prepare model
            var model = await _newsModelFactory.PrepareNewsItemModelAsync(new NewsItemModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> NewsItemCreate(NewsItemModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var newsItem = model.ToEntity<NewsItem>();
                newsItem.CreatedOnUtc = DateTime.UtcNow;
                await _newsService.InsertNewsAsync(newsItem);

                //activity log
                await _userActivityService.InsertActivityAsync("AddNewNews",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewNews"), newsItem.Id), newsItem);

                //search engine name
                var seName = await _urlRecordService.ValidateSeNameAsync(newsItem, model.SeName, model.Title, true);
                await _urlRecordService.SaveSlugAsync(newsItem, seName, newsItem.LanguageId);

                //Stores
                await SaveStoreMappingsAsync(newsItem, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.News.NewsItems.Added"));

                if (!continueEditing)
                    return RedirectToAction("NewsItems");

                return RedirectToAction("NewsItemEdit", new { id = newsItem.Id });
            }

            //prepare model
            model = await _newsModelFactory.PrepareNewsItemModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> NewsItemEdit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news item with the specified id
            var newsItem = await _newsService.GetNewsByIdAsync(id);
            if (newsItem == null)
                return RedirectToAction("NewsItems");

            //prepare model
            var model = await _newsModelFactory.PrepareNewsItemModelAsync(null, newsItem);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> NewsItemEdit(NewsItemModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news item with the specified id
            var newsItem = await _newsService.GetNewsByIdAsync(model.Id);
            if (newsItem == null)
                return RedirectToAction("NewsItems");

            if (ModelState.IsValid)
            {
                newsItem = model.ToEntity(newsItem);
                await _newsService.UpdateNewsAsync(newsItem);

                //activity log
                await _userActivityService.InsertActivityAsync("EditNews",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditNews"), newsItem.Id), newsItem);

                //search engine name
                var seName = await _urlRecordService.ValidateSeNameAsync(newsItem, model.SeName, model.Title, true);
                await _urlRecordService.SaveSlugAsync(newsItem, seName, newsItem.LanguageId);

                //stores
                await SaveStoreMappingsAsync(newsItem, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.News.NewsItems.Updated"));

                if (!continueEditing)
                    return RedirectToAction("NewsItems");

                return RedirectToAction("NewsItemEdit", new { id = newsItem.Id });
            }

            //prepare model
            model = await _newsModelFactory.PrepareNewsItemModelAsync(model, newsItem, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news item with the specified id
            var newsItem = await _newsService.GetNewsByIdAsync(id);
            if (newsItem == null)
                return RedirectToAction("NewsItems");

            await _newsService.DeleteNewsAsync(newsItem);

            //activity log
            await _userActivityService.InsertActivityAsync("DeleteNews",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteNews"), newsItem.Id), newsItem);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.News.NewsItems.Deleted"));

            return RedirectToAction("NewsItems");
        }

        #endregion

        #region Comments

        public virtual async Task<IActionResult> NewsComments(int? filterByNewsItemId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news item with the specified id
            var newsItem = await _newsService.GetNewsByIdAsync(filterByNewsItemId ?? 0);
            if (newsItem == null && filterByNewsItemId.HasValue)
                return RedirectToAction("NewsComments");

            //prepare model
            var model = await _newsModelFactory.PrepareNewsCommentSearchModelAsync(new NewsCommentSearchModel(), newsItem);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Comments(NewsCommentSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _newsModelFactory.PrepareNewsCommentListModelAsync(searchModel, searchModel.NewsItemId);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CommentUpdate(NewsCommentModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news comment with the specified id
            var comment = await _newsService.GetNewsCommentByIdAsync(model.Id)
                ?? throw new ArgumentException("No comment found with the specified id");

            var previousIsApproved = comment.IsApproved;

            //fill entity from model
            comment = model.ToEntity(comment);

            await _newsService.UpdateNewsCommentAsync(comment);

            //activity log
            await _userActivityService.InsertActivityAsync("EditNewsComment",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditNewsComment"), comment.Id), comment);

            //raise event (only if it wasn't approved before and is approved now)
            if (!previousIsApproved && comment.IsApproved)
                await _eventPublisher.PublishAsync(new NewsCommentApprovedEvent(comment));

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> CommentDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news comment with the specified id
            var comment = await _newsService.GetNewsCommentByIdAsync(id)
                ?? throw new ArgumentException("No comment found with the specified id", nameof(id));

            await _newsService.DeleteNewsCommentAsync(comment);

            //activity log
            await _userActivityService.InsertActivityAsync("DeleteNewsComment",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteNewsComment"), comment.Id), comment);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelectedComments(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (selectedIds == null)
                return Json(new { Result = true });

            var comments = await _newsService.GetNewsCommentsByIdsAsync(selectedIds.ToArray());

            await _newsService.DeleteNewsCommentsAsync(comments);

            //activity log
            foreach (var newsComment in comments)
            {
                await _userActivityService.InsertActivityAsync("DeleteNewsComment",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteNewsComment"), newsComment.Id), newsComment);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ApproveSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (selectedIds == null)
                return Json(new { Result = true });

            //filter not approved comments
            var newsComments = (await _newsService.GetNewsCommentsByIdsAsync(selectedIds.ToArray())).Where(comment => !comment.IsApproved);

            foreach (var newsComment in newsComments)
            {
                newsComment.IsApproved = true;

                await _newsService.UpdateNewsCommentAsync(newsComment);

                //raise event 
                await _eventPublisher.PublishAsync(new NewsCommentApprovedEvent(newsComment));

                //activity log
                await _userActivityService.InsertActivityAsync("EditNewsComment",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditNewsComment"), newsComment.Id), newsComment);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> DisapproveSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (selectedIds == null)
                return Json(new { Result = true });

            //filter approved comments
            var newsComments = (await _newsService.GetNewsCommentsByIdsAsync(selectedIds.ToArray())).Where(comment => comment.IsApproved);

            foreach (var newsComment in newsComments)
            {
                newsComment.IsApproved = false;

                await _newsService.UpdateNewsCommentAsync(newsComment);

                //activity log
                await _userActivityService.InsertActivityAsync("EditNewsComment",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditNewsComment"), newsComment.Id), newsComment);
            }

            return Json(new { Result = true });
        }

        #endregion

        #endregion
    }
}