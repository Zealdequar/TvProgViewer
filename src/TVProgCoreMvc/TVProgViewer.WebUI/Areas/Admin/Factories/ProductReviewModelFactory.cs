﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Html;
using TVProgViewer.Services.Catalog;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Helpers;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Stores;
using TVProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TVProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TVProgViewer.Web.Framework.Extensions;
using TVProgViewer.Web.Framework.Models.Extensions;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the product review model factory implementation
    /// </summary>
    public partial class ProductReviewModelFactory : IProductReviewModelFactory
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IProductService _productService;
        private readonly IReviewTypeService _reviewTypeService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ProductReviewModelFactory(CatalogSettings catalogSettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            IUserService userService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IProductService productService,
            IReviewTypeService reviewTypeService,
            IStoreService storeService,
            IWorkContext workContext)
        {
            _catalogSettings = catalogSettings;
            _baseAdminModelFactory = baseAdminModelFactory;
            _userService = userService;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _productService = productService;
            _reviewTypeService = reviewTypeService;
            _storeService = storeService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare product review search model
        /// </summary>
        /// <param name="searchModel">Product review search model</param>
        /// <returns>Product review search model</returns>
        public virtual async Task<ProductReviewSearchModel> PrepareProductReviewSearchModelAsync(ProductReviewSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            //prepare "approved" property (0 - all; 1 - approved only; 2 - disapproved only)
            searchModel.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = await _localizationService.GetResourceAsync("Admin.Catalog.ProductReviews.List.SearchApproved.All"),
                Value = "0"
            });
            searchModel.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = await _localizationService.GetResourceAsync("Admin.Catalog.ProductReviews.List.SearchApproved.ApprovedOnly"),
                Value = "1"
            });
            searchModel.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = await _localizationService.GetResourceAsync("Admin.Catalog.ProductReviews.List.SearchApproved.DisapprovedOnly"),
                Value = "2"
            });

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged product review list model
        /// </summary>
        /// <param name="searchModel">Product review search model</param>
        /// <returns>Product review list model</returns>
        public virtual async Task<ProductReviewListModel> PrepareProductReviewListModelAsync(ProductReviewSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter reviews
            var createdOnFromValue = !searchModel.CreatedOnFrom.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
            var createdToFromValue = !searchModel.CreatedOnTo.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
            var isApprovedOnly = searchModel.SearchApprovedId == 0 ? null : searchModel.SearchApprovedId == 1 ? true : (bool?)false;
            var vendorId = (await _workContext.GetCurrentVendorAsync())?.Id ?? 0;

            //get product reviews
            var productReviews = await _productService.GetAllProductReviewsAsync(showHidden: true,
                userId: 0,
                approved: isApprovedOnly,
                fromUtc: createdOnFromValue,
                toUtc: createdToFromValue,
                message: searchModel.SearchText,
                storeId: searchModel.SearchStoreId,
                productId: searchModel.SearchProductId,
                vendorId: vendorId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new ProductReviewListModel().PrepareToGridAsync(searchModel, productReviews, () =>
            {
                return productReviews.SelectAwait(async productReview =>
                {
                    //fill in model values from the entity
                    var productReviewModel = productReview.ToModel<ProductReviewModel>();

                    //convert dates to the user time
                    productReviewModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(productReview.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    productReviewModel.StoreName = (await _storeService.GetStoreByIdAsync(productReview.StoreId))?.Name;
                    productReviewModel.ProductName = (await _productService.GetProductByIdAsync(productReview.ProductId))?.Name;
                    productReviewModel.UserInfo = (await _userService.GetUserByIdAsync(productReview.UserId)) is User user && (await _userService.IsRegisteredAsync(user))
                        ? user.Email
                        : await _localizationService.GetResourceAsync("Admin.Users.Guest");

                    productReviewModel.ReviewText = HtmlHelper.FormatText(productReview.ReviewText, false, true, false, false, false, false);
                    productReviewModel.ReplyText = HtmlHelper.FormatText(productReview.ReplyText, false, true, false, false, false, false);

                    return productReviewModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare product review model
        /// </summary>
        /// <param name="model">Product review model</param>
        /// <param name="productReview">Product review</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Product review model</returns>
        public virtual async Task<ProductReviewModel> PrepareProductReviewModelAsync(ProductReviewModel model,
            ProductReview productReview, bool excludeProperties = false)
        {
            if (productReview != null)
            {
                //fill in model values from the entity
                model ??= new ProductReviewModel
                {
                    Id = productReview.Id,
                    StoreName = (await _storeService.GetStoreByIdAsync(productReview.StoreId))?.Name,
                    ProductId = productReview.ProductId,
                    ProductName = (await _productService.GetProductByIdAsync(productReview.ProductId))?.Name,
                    UserId = productReview.UserId,
                    Rating = productReview.Rating
                };

                model.UserInfo = await _userService.GetUserByIdAsync(productReview.UserId) is User user && await _userService.IsRegisteredAsync(user)
                    ? user.Email : await _localizationService.GetResourceAsync("Admin.Users.Guest");

                model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(productReview.CreatedOnUtc, DateTimeKind.Utc);

                if (!excludeProperties)
                {
                    model.Title = productReview.Title;
                    model.ReviewText = productReview.ReviewText;
                    model.ReplyText = productReview.ReplyText;
                    model.IsApproved = productReview.IsApproved;
                }

                //prepare nested search model
                await PrepareProductReviewReviewTypeMappingSearchModelAsync(model.ProductReviewReviewTypeMappingSearchModel, productReview);
            }

            model.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

            return model;
        }

        /// <summary>
        /// Prepare product review mapping search model
        /// </summary>
        /// <param name="searchModel">Product review mapping search model</param>
        /// <param name="productReview">Product</param>
        /// <returns>Product review mapping search model</returns>
        public virtual async Task<ProductReviewReviewTypeMappingSearchModel> PrepareProductReviewReviewTypeMappingSearchModelAsync(ProductReviewReviewTypeMappingSearchModel searchModel,
            ProductReview productReview)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (productReview == null)
                throw new ArgumentNullException(nameof(productReview));

            searchModel.ProductReviewId = productReview.Id;

            searchModel.IsAnyReviewTypes = (await _reviewTypeService.GetProductReviewReviewTypeMappingsByProductReviewIdAsync(productReview.Id)).Any();

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged product reviews mapping list model
        /// </summary>
        /// <param name="searchModel">Product review and review type mapping search model</param>
        /// <param name="productReview">Product review</param>
        /// <returns>Product review and review type mapping list model</returns>
        public virtual async Task<ProductReviewReviewTypeMappingListModel> PrepareProductReviewReviewTypeMappingListModelAsync(ProductReviewReviewTypeMappingSearchModel searchModel, ProductReview productReview)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (productReview == null)
                throw new ArgumentNullException(nameof(productReview));

            //get product review and review type mappings
            var productReviewReviewTypeMappings = (await _reviewTypeService
                .GetProductReviewReviewTypeMappingsByProductReviewIdAsync(productReview.Id)).ToPagedList(searchModel);

            //prepare grid model
            var model = await new ProductReviewReviewTypeMappingListModel().PrepareToGridAsync(searchModel, productReviewReviewTypeMappings, () =>
            {
                return productReviewReviewTypeMappings.SelectAwait(async productReviewReviewTypeMapping =>
                {
                    //fill in model values from the entity
                    var productReviewReviewTypeMappingModel = productReviewReviewTypeMapping
                        .ToModel<ProductReviewReviewTypeMappingModel>();

                    //fill in additional values (not existing in the entity)
                    var reviewType = await _reviewTypeService.GetReviewTypeByIdAsync(productReviewReviewTypeMapping.ReviewTypeId);

                    productReviewReviewTypeMappingModel.Name = await _localizationService.GetLocalizedAsync(reviewType, entity => entity.Name);
                    productReviewReviewTypeMappingModel.Description = await _localizationService.GetLocalizedAsync(reviewType, entity => entity.Description);
                    productReviewReviewTypeMappingModel.VisibleToAllUsers = reviewType.VisibleToAllUsers;

                    return productReviewReviewTypeMappingModel;
                });
            });

            return model;
        }

        #endregion
    }
}