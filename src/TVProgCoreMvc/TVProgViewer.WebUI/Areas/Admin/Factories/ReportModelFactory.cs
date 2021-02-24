﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Core.Domain.Payments;
using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Services.Catalog;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Directory;
using TVProgViewer.Services.Helpers;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Orders;
using TVProgViewer.WebUI.Areas.Admin.Models.Reports;
using TVProgViewer.Web.Framework.Models.DataTables;
using TVProgViewer.Web.Framework.Models.Extensions;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the report model factory implementation
    /// </summary>
    public partial class ReportModelFactory : IReportModelFactory
    {
        #region Fields

        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICountryService _countryService;
        private readonly IUserReportService _userReportService;
        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderReportService _orderReportService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ReportModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            ICountryService countryService,
            IUserReportService userReportService,
            IUserService userService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IOrderReportService orderReportService,
            IPriceFormatter priceFormatter,
            IProductAttributeFormatter productAttributeFormatter,
            IProductService productService,
            IWorkContext workContext)
        {
            _baseAdminModelFactory = baseAdminModelFactory;
            _countryService = countryService;
            _userReportService = userReportService;
            _userService = userService;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _orderReportService = orderReportService;
            _priceFormatter = priceFormatter;
            _productAttributeFormatter = productAttributeFormatter;
            _productService = productService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        protected virtual async Task<IPagedList<BestsellersReportLine>> GetBestsellersReportAsync(BestsellerSearchModel searchModel)
        {
            //get parameters to filter bestsellers
            var orderStatus = searchModel.OrderStatusId > 0 ? (OrderStatus?)searchModel.OrderStatusId : null;
            var paymentStatus = searchModel.PaymentStatusId > 0 ? (PaymentStatus?)searchModel.PaymentStatusId : null;
            if (await _workContext.GetCurrentVendorAsync() != null)
                searchModel.VendorId = (await _workContext.GetCurrentVendorAsync()).Id;
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

            //get bestsellers
            var bestsellers = await _orderReportService.BestSellersReportAsync(showHidden: true,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                os: orderStatus,
                ps: paymentStatus,
                billingCountryId: searchModel.BillingCountryId,
                orderBy: OrderByEnum.OrderByTotalAmount,
                vendorId: searchModel.VendorId,
                categoryId: searchModel.CategoryId,
                manufacturerId: searchModel.ManufacturerId,
                storeId: searchModel.StoreId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            return bestsellers;
        }

        #endregion

        #region Methods

        #region LowStock

        /// <summary>
        /// Prepare low stock product search model
        /// </summary>
        /// <param name="searchModel">Low stock product search model</param>
        /// <returns>Low stock product search model</returns>
        public virtual async Task<LowStockProductSearchModel> PrepareLowStockProductSearchModelAsync(LowStockProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare "published" filter (0 - all; 1 - published only; 2 - unpublished only)
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "0",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.LowStock.SearchPublished.All")
            });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "1",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.LowStock.SearchPublished.PublishedOnly")
            });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "2",
                Text = await _localizationService.GetResourceAsync("Admin.Reports.LowStock.SearchPublished.UnpublishedOnly")
            });

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged low stock product list model
        /// </summary>
        /// <param name="searchModel">Low stock product search model</param>
        /// <returns>Low stock product list model</returns>
        public virtual async Task<LowStockProductListModel> PrepareLowStockProductListModelAsync(LowStockProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter comments
            var publishedOnly = searchModel.SearchPublishedId == 0 ? null : searchModel.SearchPublishedId == 1 ? true : (bool?)false;
            var vendorId = (await _workContext.GetCurrentVendorAsync())?.Id ?? 0;

            //get low stock product and product combinations
            var products = await _productService.GetLowStockProductsAsync(vendorId: vendorId, loadPublishedOnly: publishedOnly);
            var combinations = await _productService.GetLowStockProductCombinationsAsync(vendorId: vendorId, loadPublishedOnly: publishedOnly);

            //prepare low stock product models
            var lowStockProductModels = new List<LowStockProductModel>();
            lowStockProductModels.AddRange(await products.SelectAwait(async product => new LowStockProductModel
            {
                Id = product.Id,
                Name = product.Name,

                ManageInventoryMethod = await _localizationService.GetLocalizedEnumAsync(product.ManageInventoryMethod),
                StockQuantity = await _productService.GetTotalStockQuantityAsync(product),
                Published = product.Published
            }).ToListAsync());

            lowStockProductModels.AddRange(await combinations.SelectAwait(async combination =>
            {
                var product = await _productService.GetProductByIdAsync(combination.ProductId);
                return new LowStockProductModel
                {
                    Id = combination.ProductId,
                    Name = product.Name,

                    Attributes = await _productAttributeFormatter
                        .FormatAttributesAsync(product, combination.AttributesXml, await _workContext.GetCurrentUserAsync(), "<br />", true, true, true, false),
                    ManageInventoryMethod = await _localizationService.GetLocalizedEnumAsync(product.ManageInventoryMethod),

                    StockQuantity = combination.StockQuantity,
                    Published = product.Published
                };
            }).ToListAsync());

            var pagesList = lowStockProductModels.ToPagedList(searchModel);

            //prepare list model
            var model = new LowStockProductListModel().PrepareToGrid(searchModel, pagesList, () => pagesList);

            return model;
        }

        #endregion

        #region Bestsellers

        /// <summary>
        /// Prepare bestseller search model
        /// </summary>
        /// <param name="searchModel">Bestseller search model</param>
        /// <returns>Bestseller search model</returns>
        public virtual async Task<BestsellerSearchModel> PrepareBestsellerSearchModelAsync(BestsellerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            //prepare available order statuses
            await _baseAdminModelFactory.PrepareOrderStatusesAsync(searchModel.AvailableOrderStatuses);

            //prepare available payment statuses
            await _baseAdminModelFactory.PreparePaymentStatusesAsync(searchModel.AvailablePaymentStatuses);

            //prepare available categories
            await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

            //prepare available manufacturers
            await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

            //prepare available billing countries
            searchModel.AvailableCountries = (await _countryService.GetAllCountriesForBillingAsync(showHidden: true))
                .Select(country => new SelectListItem { Text = country.Name, Value = country.Id.ToString() }).ToList();
            searchModel.AvailableCountries.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = "0" });

            //prepare available vendors
            await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged bestseller list model
        /// </summary>
        /// <param name="searchModel">Bestseller search model</param>
        /// <returns>Bestseller list model</returns>
        public virtual async Task<BestsellerListModel> PrepareBestsellerListModelAsync(BestsellerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var bestsellers = await GetBestsellersReportAsync(searchModel);

            //prepare list model
            var model = await new BestsellerListModel().PrepareToGridAsync(searchModel, bestsellers, () =>
            {
                return bestsellers.SelectAwait(async bestseller =>
                {
                    //fill in model values from the entity
                    var bestsellerModel = new BestsellerModel
                    {
                        ProductId = bestseller.ProductId,
                        TotalQuantity = bestseller.TotalQuantity
                    };

                    //fill in additional values (not existing in the entity)
                    bestsellerModel.ProductName = (await _productService.GetProductByIdAsync(bestseller.ProductId))?.Name;
                    bestsellerModel.TotalAmount = await _priceFormatter.FormatPriceAsync(bestseller.TotalAmount, true, false);

                    return bestsellerModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Get a formatted bestsellers total amount
        /// </summary>
        /// <param name="searchModel">Bestseller search model</param>
        /// <returns>Bestseller total amount</returns>
        public virtual async Task<string> GetBestsellerTotalAmountAsync(BestsellerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter bestsellers
            var orderStatus = searchModel.OrderStatusId > 0 ? (OrderStatus?)searchModel.OrderStatusId : null;
            var paymentStatus = searchModel.PaymentStatusId > 0 ? (PaymentStatus?)searchModel.PaymentStatusId : null;
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
                searchModel.VendorId = currentVendor.Id;
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

            //get a total amount
            var totalAmount = await _orderReportService.BestSellersReportTotalAmountAsync(
                showHidden: true,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                os: orderStatus,
                ps: paymentStatus,
                billingCountryId: searchModel.BillingCountryId,
                vendorId: searchModel.VendorId,
                categoryId: searchModel.CategoryId,
                manufacturerId: searchModel.ManufacturerId,
                storeId: searchModel.StoreId);

            return await _priceFormatter.FormatPriceAsync(totalAmount, true, false);
        }

        #endregion

        #region NeverSold

        /// <summary>
        /// Prepare never sold report search model
        /// </summary>
        /// <param name="searchModel">Never sold report search model</param>
        /// <returns>Never sold report search model</returns>
        public virtual async Task<NeverSoldReportSearchModel> PrepareNeverSoldSearchModelAsync(NeverSoldReportSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            //prepare available categories
            await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

            //prepare available manufacturers
            await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

            //prepare available vendors
            await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged never sold report list model
        /// </summary>
        /// <param name="searchModel">Never sold report search model</param>
        /// <returns>Never sold report list model</returns>
        public virtual async Task<NeverSoldReportListModel> PrepareNeverSoldListModelAsync(NeverSoldReportSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter neverSoldReports
            if (await _workContext.GetCurrentVendorAsync() != null)
                searchModel.SearchVendorId = (await _workContext.GetCurrentVendorAsync()).Id;
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

            //get report items
            var items = await _orderReportService.ProductsNeverSoldAsync(showHidden: true,
                vendorId: searchModel.SearchVendorId,
                storeId: searchModel.SearchStoreId,
                categoryId: searchModel.SearchCategoryId,
                manufacturerId: searchModel.SearchManufacturerId,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new NeverSoldReportListModel().PrepareToGrid(searchModel, items, () =>
            {
                //fill in model values from the entity
                return items.Select(item => new NeverSoldReportModel
                {
                    ProductId = item.Id,
                    ProductName = item.Name
                });
            });

            return model;
        }

        #endregion

        #region Country sales

        /// <summary>
        /// Prepare country report search model
        /// </summary>
        /// <param name="searchModel">Country report search model</param>
        /// <returns>Country report search model</returns>
        public virtual async Task<CountryReportSearchModel> PrepareCountrySalesSearchModelAsync(CountryReportSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available order statuses
            await _baseAdminModelFactory.PrepareOrderStatusesAsync(searchModel.AvailableOrderStatuses);

            //prepare available payment statuses
            await _baseAdminModelFactory.PreparePaymentStatusesAsync(searchModel.AvailablePaymentStatuses);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged country report list model
        /// </summary>
        /// <param name="searchModel">Country report search model</param>
        /// <returns>Country report list model</returns>
        public virtual async Task<CountryReportListModel> PrepareCountrySalesListModelAsync(CountryReportSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter countryReports
            var orderStatus = searchModel.OrderStatusId > 0 ? (OrderStatus?)searchModel.OrderStatusId : null;
            var paymentStatus = searchModel.PaymentStatusId > 0 ? (PaymentStatus?)searchModel.PaymentStatusId : null;
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

            //get items
            var items = (await _orderReportService.GetCountryReportAsync(os: orderStatus,
                ps: paymentStatus,
                startTimeUtc: startDateValue,
                endTimeUtc: endDateValue)).ToPagedList(searchModel);

            //prepare list model
            var model = await new CountryReportListModel().PrepareToGridAsync(searchModel, items, () =>
            {
                return items.SelectAwait(async item =>
                {
                    //fill in model values from the entity
                    var countryReportModel = new CountryReportModel
                    {
                        TotalOrders = item.TotalOrders
                    };

                    //fill in additional values (not existing in the entity)
                    countryReportModel.SumOrders = await _priceFormatter.FormatPriceAsync(item.SumOrders, true, false);
                    countryReportModel.CountryName = (await _countryService.GetCountryByIdAsync(item.CountryId ?? 0))?.Name;

                    return countryReportModel;
                });
            });

            return model;
        }

        #endregion

        #region User reports

        /// <summary>
        /// Prepare user reports search model
        /// </summary>
        /// <param name="searchModel">User reports search model</param>
        /// <returns>User reports search model</returns>
        public virtual async Task<UserReportsSearchModel> PrepareUserReportsSearchModelAsync(UserReportsSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare nested search models
            await PrepareBestUsersReportByOrderTotalSearchModelAsync(searchModel.BestUsersByOrderTotal);
            await PrepareBestUsersReportSearchModelAsync(searchModel.BestUsersByNumberOfOrders);
            await PrepareRegisteredUsersReportSearchModelAsync(searchModel.RegisteredUsers);

            return searchModel;
        }

        /// <summary>
        /// Prepare best users by number of orders report search model
        /// </summary>
        /// <param name="searchModel">Best users report search model</param>
        /// <returns>Best users report search model</returns>
        protected virtual async Task<BestUsersReportSearchModel> PrepareBestUsersReportSearchModelAsync(BestUsersReportSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available order, payment and shipping statuses
            await _baseAdminModelFactory.PrepareOrderStatusesAsync(searchModel.AvailableOrderStatuses);
            await _baseAdminModelFactory.PreparePaymentStatusesAsync(searchModel.AvailablePaymentStatuses);
            await _baseAdminModelFactory.PrepareShippingStatusesAsync(searchModel.AvailableShippingStatuses);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare best users by order total report search model
        /// </summary>
        /// <param name="searchModel">Best users report search model</param>
        /// <returns>Best users report search model</returns>
        protected virtual async Task<BestUsersReportSearchModel> PrepareBestUsersReportByOrderTotalSearchModelAsync(BestUsersReportSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available order, payment and shipping statuses
            await _baseAdminModelFactory.PrepareOrderStatusesAsync(searchModel.AvailableOrderStatuses);
            await _baseAdminModelFactory.PreparePaymentStatusesAsync(searchModel.AvailablePaymentStatuses);
            await _baseAdminModelFactory.PrepareShippingStatusesAsync(searchModel.AvailableShippingStatuses);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }


        /// <summary>
        /// Prepare registered users report search model
        /// </summary>
        /// <param name="searchModel">Registered users report search model</param>
        /// <returns>Registered users report search model</returns>
        protected virtual Task<RegisteredUsersReportSearchModel> PrepareRegisteredUsersReportSearchModelAsync(RegisteredUsersReportSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged best users report list model
        /// </summary>
        /// <param name="searchModel">Best users report search model</param>
        /// <returns>Best users report list model</returns>
        public virtual async Task<BestUsersReportListModel> PrepareBestUsersReportListModelAsync(BestUsersReportSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
            var orderStatus = searchModel.OrderStatusId > 0 ? (OrderStatus?)searchModel.OrderStatusId : null;
            var paymentStatus = searchModel.PaymentStatusId > 0 ? (PaymentStatus?)searchModel.PaymentStatusId : null;
            var shippingStatus = searchModel.ShippingStatusId > 0 ? (ShippingStatus?)searchModel.ShippingStatusId : null;

            //get report items
            var reportItems = await _userReportService.GetBestUsersReportAsync(createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                os: orderStatus,
                ps: paymentStatus,
                ss: shippingStatus,
                orderBy: searchModel.OrderBy,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new BestUsersReportListModel().PrepareToGridAsync(searchModel, reportItems, () =>
            {
                return reportItems.SelectAwait(async item =>
                {
                    //fill in model values from the entity
                    var bestUsersReportModel = new BestUsersReportModel
                    {
                        UserId = item.UserId,

                        OrderTotal = await _priceFormatter.FormatPriceAsync(item.OrderTotal, true, false),
                        OrderCount = item.OrderCount
                    };

                    //fill in additional values (not existing in the entity)
                    var user = await _userService.GetUserByIdAsync(item.UserId);
                    if (user != null)
                    {
                        bestUsersReportModel.UserName = (await _userService.IsRegisteredAsync(user))
                             ? user.Email
                             : await _localizationService.GetResourceAsync("Admin.Users.Guest");
                    }

                    return bestUsersReportModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged registered users report list model
        /// </summary>
        /// <param name="searchModel">Registered users report search model</param>
        /// <returns>Registered users report list model</returns>
        public virtual async Task<RegisteredUsersReportListModel> PrepareRegisteredUsersReportListModelAsync(RegisteredUsersReportSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get report items
            var reportItems = new List<RegisteredUsersReportModel>
            {
                new RegisteredUsersReportModel
                {
                    Period = await _localizationService.GetResourceAsync("Admin.Reports.Users.RegisteredUsers.Fields.Period.7days"),
                    Users = await _userReportService.GetRegisteredUsersReportAsync(7)
                },
                new RegisteredUsersReportModel
                {
                    Period = await _localizationService.GetResourceAsync("Admin.Reports.Users.RegisteredUsers.Fields.Period.14days"),
                    Users = await _userReportService.GetRegisteredUsersReportAsync(14)
                },
                new RegisteredUsersReportModel
                {
                    Period = await _localizationService.GetResourceAsync("Admin.Reports.Users.RegisteredUsers.Fields.Period.month"),
                    Users = await _userReportService.GetRegisteredUsersReportAsync(30)
                },
                new RegisteredUsersReportModel
                {
                    Period = await _localizationService.GetResourceAsync("Admin.Reports.Users.RegisteredUsers.Fields.Period.year"),
                    Users = await _userReportService.GetRegisteredUsersReportAsync(365)
                }
            };

            var pagedList = reportItems.ToPagedList(searchModel);

            //prepare list model
            var model = new RegisteredUsersReportListModel().PrepareToGrid(searchModel, pagedList, () => pagedList);

            return model;
        }

        #endregion

        #endregion
    }
}