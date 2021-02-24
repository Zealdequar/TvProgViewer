﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Services.Catalog;
using TVProgViewer.Services.Common;
using TVProgViewer.Services.Events;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Logging;
using TVProgViewer.Services.Messages;
using TVProgViewer.Services.Security;
using TVProgViewer.WebUI.Areas.Admin.Factories;
using TVProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TVProgViewer.Web.Framework.Mvc.Filters;
using TVProgViewer.Core.Events;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Areas.Admin.Controllers
{
    public partial class ProductReviewController : BaseAdminController
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IUserActivityService _userActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IProductReviewModelFactory _productReviewModelFactory;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;

        #endregion Fields

        #region Ctor

        public ProductReviewController(CatalogSettings catalogSettings,
            IUserActivityService userActivityService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IProductReviewModelFactory productReviewModelFactory,
            IProductService productService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService)
        {
            _catalogSettings = catalogSettings;
            _userActivityService = userActivityService;
            _eventPublisher = eventPublisher;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _productReviewModelFactory = productReviewModelFactory;
            _productService = productService;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            //prepare model
            var model = await _productReviewModelFactory.PrepareProductReviewSearchModelAsync(new ProductReviewSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(ProductReviewSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProductReviews))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _productReviewModelFactory.PrepareProductReviewListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            //try to get a product review with the specified id
            var productReview = await _productService.GetProductReviewByIdAsync(id);
            if (productReview == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendorAsync() != null && (await _productService.GetProductByIdAsync(productReview.ProductId)).VendorId != (await _workContext.GetCurrentVendorAsync()).Id)
                return RedirectToAction("List");

            //prepare model
            var model = await _productReviewModelFactory.PrepareProductReviewModelAsync(null, productReview);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(ProductReviewModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            //try to get a product review with the specified id
            var productReview = await _productService.GetProductReviewByIdAsync(model.Id);
            if (productReview == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendorAsync() != null && (await _productService.GetProductByIdAsync(productReview.ProductId)).VendorId != (await _workContext.GetCurrentVendorAsync()).Id)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var previousIsApproved = productReview.IsApproved;

                //vendor can edit "Reply text" only
                var isLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;
                if (!isLoggedInAsVendor)
                {
                    productReview.Title = model.Title;
                    productReview.ReviewText = model.ReviewText;
                    productReview.IsApproved = model.IsApproved;
                }

                productReview.ReplyText = model.ReplyText;

                //notify user about reply
                if (productReview.IsApproved && !string.IsNullOrEmpty(productReview.ReplyText)
                    && _catalogSettings.NotifyUserAboutProductReviewReply && !productReview.UserNotifiedOfReply)
                {
                    var userLanguageId = await _genericAttributeService.GetAttributeAsync<User, int>(productReview.UserId,
                        TvProgUserDefaults.LanguageIdAttribute, productReview.StoreId);

                    var queuedEmailIds = await _workflowMessageService.SendProductReviewReplyUserNotificationMessageAsync(productReview, userLanguageId);
                    if (queuedEmailIds.Any())
                        productReview.UserNotifiedOfReply = true;
                }

                await _productService.UpdateProductReviewAsync(productReview);

                //activity log
                await _userActivityService.InsertActivityAsync("EditProductReview",
                   string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditProductReview"), productReview.Id), productReview);

                //vendor can edit "Reply text" only
                if (!isLoggedInAsVendor)
                {
                    var product = await _productService.GetProductByIdAsync(productReview.ProductId);
                    //update product totals
                    await _productService.UpdateProductReviewTotalsAsync(product);

                    //raise event (only if it wasn't approved before and is approved now)
                    if (!previousIsApproved && productReview.IsApproved)
                        await _eventPublisher.PublishAsync(new ProductReviewApprovedEvent(productReview));
                }

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.ProductReviews.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = productReview.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await _productReviewModelFactory.PrepareProductReviewModelAsync(model, productReview, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            //try to get a product review with the specified id
            var productReview = await _productService.GetProductReviewByIdAsync(id);
            if (productReview == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("List");

            await _productService.DeleteProductReviewAsync(productReview);

            //activity log
            await _userActivityService.InsertActivityAsync("DeleteProductReview",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteProductReview"), productReview.Id), productReview);

            var product = await _productService.GetProductByIdAsync(productReview.ProductId);

            //update product totals
            await _productService.UpdateProductReviewTotalsAsync(product);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.ProductReviews.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> ApproveSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("List");

            if (selectedIds == null)
                return Json(new { Result = true });

            //filter not approved reviews
            var productReviews = (await _productService.GetProductReviewsByIdsAsync(selectedIds.ToArray())).Where(review => !review.IsApproved);

            foreach (var productReview in productReviews)
            {
                productReview.IsApproved = true;
                await _productService.UpdateProductReviewAsync(productReview);

                var product = await _productService.GetProductByIdAsync(productReview.ProductId);

                //update product totals
                await _productService.UpdateProductReviewTotalsAsync(product);

                //raise event 
                await _eventPublisher.PublishAsync(new ProductReviewApprovedEvent(productReview));
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> DisapproveSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("List");

            if (selectedIds == null)
                return Json(new { Result = true });

            //filter approved reviews
            var productReviews = (await _productService.GetProductReviewsByIdsAsync(selectedIds.ToArray())).Where(review => review.IsApproved);

            foreach (var productReview in productReviews)
            {
                productReview.IsApproved = false;
                await _productService.UpdateProductReviewAsync(productReview);

                var product = await _productService.GetProductByIdAsync(productReview.ProductId);

                //update product totals
                await _productService.UpdateProductReviewTotalsAsync(product);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("List");

            if (selectedIds == null)
                return Json(new { Result = true });

            var productReviews = await _productService.GetProductReviewsByIdsAsync(selectedIds.ToArray());
            var products = await _productService.GetProductsByIdsAsync(productReviews.Select(p => p.ProductId).Distinct().ToArray());

            await _productService.DeleteProductReviewsAsync(productReviews);

            //update product totals
            foreach (var product in products)
            {
                await _productService.UpdateProductReviewTotalsAsync(product);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductReviewReviewTypeMappingList(ProductReviewReviewTypeMappingSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProductReviews))
                return await AccessDeniedDataTablesJson();
            var productReview = await _productService.GetProductReviewByIdAsync(searchModel.ProductReviewId)
                ?? throw new ArgumentException("No product review found with the specified id");

            //prepare model
            var model = await _productReviewModelFactory.PrepareProductReviewReviewTypeMappingListModelAsync(searchModel, productReview);

            return Json(model);
        }

        #endregion
    }
}