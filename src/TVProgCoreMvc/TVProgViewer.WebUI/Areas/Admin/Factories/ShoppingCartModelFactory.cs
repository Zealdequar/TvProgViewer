﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Services.Catalog;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Directory;
using TVProgViewer.Services.Helpers;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Orders;
using TVProgViewer.Services.Stores;
using TVProgViewer.Services.Tax;
using TVProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TVProgViewer.WebUI.Areas.Admin.Models.ShoppingCart;
using TVProgViewer.Web.Framework.Extensions;
using TVProgViewer.Web.Framework.Models.Extensions;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the shopping cart model factory implementation
    /// </summary>
    public partial class ShoppingCartModelFactory : IShoppingCartModelFactory
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICountryService _countryService;
        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreService _storeService;
        private readonly ITaxService _taxService;

        #endregion

        #region Ctor

        public ShoppingCartModelFactory(CatalogSettings catalogSettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICountryService countryService,
            IUserService userService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IPriceFormatter priceFormatter,
            IProductAttributeFormatter productAttributeFormatter,
            IProductService productService,
            IShoppingCartService shoppingCartService,
            IStoreService storeService,
            ITaxService taxService)
        {
            _catalogSettings = catalogSettings;
            _baseAdminModelFactory = baseAdminModelFactory;
            _countryService = countryService;
            _userService = userService;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _priceFormatter = priceFormatter;
            _productAttributeFormatter = productAttributeFormatter;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _storeService = storeService;
            _taxService = taxService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare shopping cart item search model
        /// </summary>
        /// <param name="searchModel">Shopping cart item search model</param>
        /// <returns>Shopping cart item search model</returns>
        protected virtual ShoppingCartItemSearchModel PrepareShoppingCartItemSearchModel(ShoppingCartItemSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare shopping cart search model
        /// </summary>
        /// <param name="searchModel">Shopping cart search model</param>
        /// <returns>Shopping cart search model</returns>
        public virtual async Task<ShoppingCartSearchModel> PrepareShoppingCartSearchModelAsync(ShoppingCartSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available shopping cart types
            await _baseAdminModelFactory.PrepareShoppingCartTypesAsync(searchModel.AvailableShoppingCartTypes, false);

            //set default search values
            searchModel.ShoppingCartType = ShoppingCartType.ShoppingCart;

            //prepare available billing countries
            searchModel.AvailableCountries = (await _countryService.GetAllCountriesForBillingAsync(showHidden: true))
                .Select(country => new SelectListItem { Text = country.Name, Value = country.Id.ToString() }).ToList();
            searchModel.AvailableCountries.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = "0" });

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare nested search model
            PrepareShoppingCartItemSearchModel(searchModel.ShoppingCartItemSearchModel);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged shopping cart list model
        /// </summary>
        /// <param name="searchModel">Shopping cart search model</param>
        /// <returns>Shopping cart list model</returns>
        public virtual async Task<ShoppingCartListModel> PrepareShoppingCartListModelAsync(ShoppingCartSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get users with shopping carts
            var users = await _userService.GetUsersWithShoppingCartsAsync(searchModel.ShoppingCartType,
                storeId: searchModel.StoreId,
                productId: searchModel.ProductId,
                createdFromUtc: searchModel.StartDate,
                createdToUtc: searchModel.EndDate,
                countryId: searchModel.BillingCountryId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new ShoppingCartListModel().PrepareToGridAsync(searchModel, users, () =>
            {
                return users.SelectAwait(async user =>
                {
                    //fill in model values from the entity
                    var shoppingCartModel = new ShoppingCartModel
                    {
                        UserId = user.Id
                    };

                    //fill in additional values (not existing in the entity)
                    shoppingCartModel.UserEmail = (await _userService.IsRegisteredAsync(user))
                        ? user.Email
                        : await _localizationService.GetResourceAsync("Admin.Users.Guest");
                    shoppingCartModel.TotalItems = (await _shoppingCartService
                        .GetShoppingCartAsync(user, searchModel.ShoppingCartType,
                            searchModel.StoreId, searchModel.ProductId, searchModel.StartDate, searchModel.EndDate))
                        .Sum(item => item.Quantity);

                    return shoppingCartModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged shopping cart item list model
        /// </summary>
        /// <param name="searchModel">Shopping cart item search model</param>
        /// <param name="user">User</param>
        /// <returns>Shopping cart item list model</returns>
        public virtual async Task<ShoppingCartItemListModel> PrepareShoppingCartItemListModelAsync(ShoppingCartItemSearchModel searchModel, User user)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            //get shopping cart items
            var items = (await _shoppingCartService.GetShoppingCartAsync(user, searchModel.ShoppingCartType,
                searchModel.StoreId, searchModel.ProductId, searchModel.StartDate, searchModel.EndDate)).ToPagedList(searchModel);

            var isSearchProduct = searchModel.ProductId > 0;

            Product product = null;

            if (isSearchProduct)
            {
                product = await _productService.GetProductByIdAsync(searchModel.ProductId) ?? throw new Exception("Product is not found");
            }

            //prepare list model
            var model = await new ShoppingCartItemListModel().PrepareToGridAsync(searchModel, items, () =>
            {
                return items.SelectAwait(async item =>
                {
                    //fill in model values from the entity
                    var itemModel = item.ToModel<ShoppingCartItemModel>();

                    if (!isSearchProduct)
                        product = await _productService.GetProductByIdAsync(item.ProductId);

                    //convert dates to the user time
                    itemModel.UpdatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(item.UpdatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    itemModel.Store = (await _storeService.GetStoreByIdAsync(item.StoreId))?.Name ?? "Deleted";
                    itemModel.AttributeInfo = await _productAttributeFormatter.FormatAttributesAsync(product, item.AttributesXml, user);
                    var (unitPrice, _, _) = await _shoppingCartService.GetUnitPriceAsync(item, true);
                    itemModel.UnitPrice = await _priceFormatter.FormatPriceAsync((await _taxService.GetProductPriceAsync(product, unitPrice)).price);
                    var (subTotal, _, _, _) = await _shoppingCartService.GetSubTotalAsync(item, true);
                    itemModel.Total = await _priceFormatter.FormatPriceAsync((await _taxService.GetProductPriceAsync(product, subTotal)).price);

                    //set product name since it does not survive mapping
                    itemModel.ProductName = product.Name;

                    return itemModel;
                });
            });

            return model;
        }

        #endregion
    }
}