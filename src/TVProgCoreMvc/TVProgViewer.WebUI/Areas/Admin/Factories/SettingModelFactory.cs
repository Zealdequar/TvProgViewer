﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TVProgViewer.Core;
using TVProgViewer.Core.Configuration;
using TVProgViewer.Core.Domain;
using TVProgViewer.Core.Domain.Blogs;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Core.Domain.Gdpr;
using TVProgViewer.Core.Domain.Localization;
using TVProgViewer.Core.Domain.Media;
using TVProgViewer.Core.Domain.News;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Core.Domain.Security;
using TVProgViewer.Core.Domain.Seo;
using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Core.Domain.Tax;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Vendors;
using TVProgViewer.Data;
using TVProgViewer.Services;
using TVProgViewer.Services.Common;
using TVProgViewer.Services.Configuration;
using TVProgViewer.Services.Directory;
using TVProgViewer.Services.Gdpr;
using TVProgViewer.Services.Helpers;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Media;
using TVProgViewer.Services.Stores;
using TVProgViewer.Services.Themes;
using TVProgViewer.Web.Framework.Factories;
using TVProgViewer.Web.Framework.Models.Extensions;
using TVProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TVProgViewer.WebUI.Areas.Admin.Models.Common;
using TVProgViewer.WebUI.Areas.Admin.Models.Settings;
using TVProgViewer.WebUI.Areas.Admin.Models.Stores;

namespace TVProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the setting model factory implementation
    /// </summary>
    public partial class SettingModelFactory : ISettingModelFactory
    {
        #region Fields

        private readonly AppSettings _appSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly IAddressAttributeModelFactory _addressAttributeModelFactory;
        private readonly IAddressService _addressService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICurrencyService _currencyService;
        private readonly IUserAttributeModelFactory _userAttributeModelFactory;
        private readonly ITvProgDataProvider _dataProvider;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGdprService _gdprService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IReturnRequestModelFactory _returnRequestModelFactory;
        private readonly IReviewTypeModelFactory _reviewTypeModelFactory;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IThemeProvider _themeProvider;
        private readonly IVendorAttributeModelFactory _vendorAttributeModelFactory;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public SettingModelFactory(AppSettings appSettings,
            CurrencySettings currencySettings,
            IAddressAttributeModelFactory addressAttributeModelFactory,
            IAddressService addressService,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICurrencyService currencyService,
            IUserAttributeModelFactory userAttributeModelFactory,
            ITvProgDataProvider dataProvider,
            IDateTimeHelper dateTimeHelper,
            IGdprService gdprService,
            ILocalizedModelFactory localizedModelFactory,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IReturnRequestModelFactory returnRequestModelFactory,
            ISettingService settingService,
            IStoreContext storeContext,
            IStoreService storeService,
            IThemeProvider themeProvider,
            IVendorAttributeModelFactory vendorAttributeModelFactory,
            IReviewTypeModelFactory reviewTypeModelFactory,
            IWorkContext workContext)
        {
            _appSettings = appSettings;
            _currencySettings = currencySettings;
            _addressAttributeModelFactory = addressAttributeModelFactory;
            _addressService = addressService;
            _baseAdminModelFactory = baseAdminModelFactory;
            _currencyService = currencyService;
            _userAttributeModelFactory = userAttributeModelFactory;
            _dataProvider = dataProvider;
            _dateTimeHelper = dateTimeHelper;
            _gdprService = gdprService;
            _localizedModelFactory = localizedModelFactory;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _pictureService = pictureService;
            _returnRequestModelFactory = returnRequestModelFactory;
            _settingService = settingService;
            _storeContext = storeContext;
            _storeService = storeService;
            _themeProvider = themeProvider;
            _vendorAttributeModelFactory = vendorAttributeModelFactory;
            _reviewTypeModelFactory = reviewTypeModelFactory;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare address model
        /// </summary>
        /// <param name="model">Address model</param>
        /// <param name="address">Address</param>
        protected virtual async Task PrepareAddressModelAsync(AddressModel model, Address address)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //set some of address fields as enabled and required
            model.CountryEnabled = true;
            model.StateProvinceEnabled = true;
            model.CountyEnabled = true;
            model.CityEnabled = true;
            model.StreetAddressEnabled = true;
            model.ZipPostalCodeEnabled = true;
            model.ZipPostalCodeRequired = true;

            //prepare available countries
            await _baseAdminModelFactory.PrepareCountriesAsync(model.AvailableCountries);

            //prepare available states
            await _baseAdminModelFactory.PrepareStatesAndProvincesAsync(model.AvailableStates, model.CountryId);
        }

        /// <summary>
        /// Prepare store theme models
        /// </summary>
        /// <param name="models">List of store theme models</param>
        protected virtual async Task PrepareStoreThemeModelsAsync(IList<StoreInformationSettingsModel.ThemeModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var storeInformationSettings = await _settingService.LoadSettingAsync<StoreInformationSettings>(storeId);

            //get available themes
            var availableThemes = await _themeProvider.GetThemesAsync();
            foreach (var theme in availableThemes)
            {
                models.Add(new StoreInformationSettingsModel.ThemeModel
                {
                    FriendlyName = theme.FriendlyName,
                    SystemName = theme.SystemName,
                    PreviewImageUrl = theme.PreviewImageUrl,
                    PreviewText = theme.PreviewText,
                    SupportRtl = theme.SupportRtl,
                    Selected = theme.SystemName.Equals(storeInformationSettings.DefaultStoreTheme, StringComparison.InvariantCultureIgnoreCase)
                });
            }
        }

        /// <summary>
        /// Prepare sort option search model
        /// </summary>
        /// <param name="searchModel">Sort option search model</param>
        /// <returns>Sort option search model</returns>
        protected virtual Task<SortOptionSearchModel> PrepareSortOptionSearchModelAsync(SortOptionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare GDPR consent search model
        /// </summary>
        /// <param name="searchModel">GDPR consent search model</param>
        /// <returns>GDPR consent search model</returns>
        protected virtual Task<GdprConsentSearchModel> PrepareGdprConsentSearchModelAsync(GdprConsentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare address settings model
        /// </summary>
        /// <returns>Address settings model</returns>
        protected virtual async Task<AddressSettingsModel> PrepareAddressSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var addressSettings = await _settingService.LoadSettingAsync<AddressSettings>(storeId);

            //fill in model values from the entity
            var model = addressSettings.ToSettingsModel<AddressSettingsModel>();

            return model;
        }

        /// <summary>
        /// Prepare user settings model
        /// </summary>
        /// <returns>User settings model</returns>
        protected virtual async Task<UserSettingsModel> PrepareUserSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var userSettings = await _settingService.LoadSettingAsync<UserSettings>(storeId);

            //fill in model values from the entity
            var model = userSettings.ToSettingsModel<UserSettingsModel>();

            return model;
        }

        /// <summary>
        /// Prepare multi-factor authentication settings model
        /// </summary>
        /// <returns>MultiFactorAuthenticationSettingsModel</returns>
        protected virtual async Task<MultiFactorAuthenticationSettingsModel> PrepareMultiFactorAuthenticationSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var multiFactorAuthenticationSettings = await _settingService.LoadSettingAsync<MultiFactorAuthenticationSettings>(storeId);

            //fill in model values from the entity
            var model = multiFactorAuthenticationSettings.ToSettingsModel<MultiFactorAuthenticationSettingsModel>();

            return model;

        }

        /// <summary>
        /// Prepare date time settings model
        /// </summary>
        /// <returns>Date time settings model</returns>
        protected virtual async Task<DateTimeSettingsModel> PrepareDateTimeSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var dateTimeSettings = await _settingService.LoadSettingAsync<DateTimeSettings>(storeId);

            //fill in model values from the entity
            var model = new DateTimeSettingsModel
            {
                AllowUsersToSetTimeZone = dateTimeSettings.AllowUsersToSetTimeZone
            };

            //fill in additional values (not existing in the entity)
            model.DefaultStoreTimeZoneId = _dateTimeHelper.DefaultStoreTimeZone.Id;

            //prepare available time zones
            await _baseAdminModelFactory.PrepareTimeZonesAsync(model.AvailableTimeZones, false);

            return model;
        }

        /// <summary>
        /// Prepare external authentication settings model
        /// </summary>
        /// <returns>External authentication settings model</returns>
        protected virtual async Task<ExternalAuthenticationSettingsModel> PrepareExternalAuthenticationSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var externalAuthenticationSettings = await _settingService.LoadSettingAsync<ExternalAuthenticationSettings>(storeId);

            //fill in model values from the entity
            var model = new ExternalAuthenticationSettingsModel
            {
                AllowUsersToRemoveAssociations = externalAuthenticationSettings.AllowUsersToRemoveAssociations
            };

            return model;
        }

        /// <summary>
        /// Prepare store information settings model
        /// </summary>
        /// <returns>Store information settings model</returns>
        protected virtual async Task<StoreInformationSettingsModel> PrepareStoreInformationSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var storeInformationSettings = await _settingService.LoadSettingAsync<StoreInformationSettings>(storeId);
            var commonSettings = await _settingService.LoadSettingAsync<CommonSettings>(storeId);

            //fill in model values from the entity
            var model = new StoreInformationSettingsModel
            {
                StoreClosed = storeInformationSettings.StoreClosed,
                DefaultStoreTheme = storeInformationSettings.DefaultStoreTheme,
                AllowUserToSelectTheme = storeInformationSettings.AllowUserToSelectTheme,
                LogoPictureId = storeInformationSettings.LogoPictureId,
                DisplayEuCookieLawWarning = storeInformationSettings.DisplayEuCookieLawWarning,
                FacebookLink = storeInformationSettings.FacebookLink,
                TwitterLink = storeInformationSettings.TwitterLink,
                YoutubeLink = storeInformationSettings.YoutubeLink,
                SubjectFieldOnContactUsForm = commonSettings.SubjectFieldOnContactUsForm,
                UseSystemEmailForContactUsForm = commonSettings.UseSystemEmailForContactUsForm,
                PopupForTermsOfServiceLinks = commonSettings.PopupForTermsOfServiceLinks
            };

            //prepare available themes
            await PrepareStoreThemeModelsAsync(model.AvailableStoreThemes);

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.StoreClosed_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.StoreClosed, storeId);
            model.DefaultStoreTheme_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.DefaultStoreTheme, storeId);
            model.AllowUserToSelectTheme_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.AllowUserToSelectTheme, storeId);
            model.LogoPictureId_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.LogoPictureId, storeId);
            model.DisplayEuCookieLawWarning_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.DisplayEuCookieLawWarning, storeId);
            model.FacebookLink_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.FacebookLink, storeId);
            model.TwitterLink_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.TwitterLink, storeId);
            model.YoutubeLink_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.YoutubeLink, storeId);
            model.SubjectFieldOnContactUsForm_OverrideForStore = await _settingService.SettingExistsAsync(commonSettings, x => x.SubjectFieldOnContactUsForm, storeId);
            model.UseSystemEmailForContactUsForm_OverrideForStore = await _settingService.SettingExistsAsync(commonSettings, x => x.UseSystemEmailForContactUsForm, storeId);
            model.PopupForTermsOfServiceLinks_OverrideForStore = await _settingService.SettingExistsAsync(commonSettings, x => x.PopupForTermsOfServiceLinks, storeId);

            return model;
        }

        /// <summary>
        /// Prepare Sitemap settings model
        /// </summary>
        /// <returns>Sitemap settings model</returns>
        protected virtual async Task<SitemapSettingsModel> PrepareSitemapSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var sitemapSettings = await _settingService.LoadSettingAsync<SitemapSettings>(storeId);

            //fill in model values from the entity
            var model = new SitemapSettingsModel
            {
                SitemapEnabled = sitemapSettings.SitemapEnabled,
                SitemapPageSize = sitemapSettings.SitemapPageSize,
                SitemapIncludeCategories = sitemapSettings.SitemapIncludeCategories,
                SitemapIncludeManufacturers = sitemapSettings.SitemapIncludeManufacturers,
                SitemapIncludeProducts = sitemapSettings.SitemapIncludeProducts,
                SitemapIncludeProductTags = sitemapSettings.SitemapIncludeProductTags,
                SitemapIncludeBlogPosts = sitemapSettings.SitemapIncludeBlogPosts,
                SitemapIncludeNews = sitemapSettings.SitemapIncludeNews,
                SitemapIncludeTopics = sitemapSettings.SitemapIncludeTopics
            };

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.SitemapEnabled_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapEnabled, storeId);
            model.SitemapPageSize_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapPageSize, storeId);
            model.SitemapIncludeCategories_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeCategories, storeId);
            model.SitemapIncludeManufacturers_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeManufacturers, storeId);
            model.SitemapIncludeProducts_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeProducts, storeId);
            model.SitemapIncludeProductTags_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeProductTags, storeId);
            model.SitemapIncludeBlogPosts_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeBlogPosts, storeId);
            model.SitemapIncludeNews_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeNews, storeId);
            model.SitemapIncludeTopics_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeTopics, storeId);

            return model;
        }

        /// <summary>
        /// Prepare minification settings model
        /// </summary>
        /// <returns>Minification settings model</returns>
        protected virtual async Task<MinificationSettingsModel> PrepareMinificationSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var minificationSettings = await _settingService.LoadSettingAsync<CommonSettings>(storeId);

            //fill in model values from the entity
            var model = new MinificationSettingsModel
            {
                EnableHtmlMinification = minificationSettings.EnableHtmlMinification,
                EnableJsBundling = minificationSettings.EnableJsBundling,
                EnableCssBundling = minificationSettings.EnableCssBundling,
                UseResponseCompression = minificationSettings.UseResponseCompression
            };

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.EnableHtmlMinification_OverrideForStore = await _settingService.SettingExistsAsync(minificationSettings, x => x.EnableHtmlMinification, storeId);
            model.EnableJsBundling_OverrideForStore = await _settingService.SettingExistsAsync(minificationSettings, x => x.EnableJsBundling, storeId);
            model.EnableCssBundling_OverrideForStore = await _settingService.SettingExistsAsync(minificationSettings, x => x.EnableCssBundling, storeId);
            model.UseResponseCompression_OverrideForStore = await _settingService.SettingExistsAsync(minificationSettings, x => x.UseResponseCompression, storeId);

            return model;
        }

        /// <summary>
        /// Prepare SEO settings model
        /// </summary>
        /// <returns>SEO settings model</returns>
        protected virtual async Task<SeoSettingsModel> PrepareSeoSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var seoSettings = await _settingService.LoadSettingAsync<SeoSettings>(storeId);

            //fill in model values from the entity
            var model = new SeoSettingsModel
            {
                PageTitleSeparator = seoSettings.PageTitleSeparator,
                PageTitleSeoAdjustment = (int)seoSettings.PageTitleSeoAdjustment,
                PageTitleSeoAdjustmentValues = await seoSettings.PageTitleSeoAdjustment.ToSelectListAsync(),
                DefaultTitle = seoSettings.DefaultTitle,
                DefaultMetaKeywords = seoSettings.DefaultMetaKeywords,
                DefaultMetaDescription = seoSettings.DefaultMetaDescription,
                GenerateProductMetaDescription = seoSettings.GenerateProductMetaDescription,
                ConvertNonWesternChars = seoSettings.ConvertNonWesternChars,
                CanonicalUrlsEnabled = seoSettings.CanonicalUrlsEnabled,
                WwwRequirement = (int)seoSettings.WwwRequirement,
                WwwRequirementValues = await seoSettings.WwwRequirement.ToSelectListAsync(),

                TwitterMetaTags = seoSettings.TwitterMetaTags,
                OpenGraphMetaTags = seoSettings.OpenGraphMetaTags,
                CustomHeadTags = seoSettings.CustomHeadTags,
                MicrodataEnabled = seoSettings.MicrodataEnabled
            };

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.PageTitleSeparator_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.PageTitleSeparator, storeId);
            model.PageTitleSeoAdjustment_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.PageTitleSeoAdjustment, storeId);
            model.DefaultTitle_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.DefaultTitle, storeId);
            model.DefaultMetaKeywords_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.DefaultMetaKeywords, storeId);
            model.DefaultMetaDescription_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.DefaultMetaDescription, storeId);
            model.GenerateProductMetaDescription_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.GenerateProductMetaDescription, storeId);
            model.ConvertNonWesternChars_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.ConvertNonWesternChars, storeId);
            model.CanonicalUrlsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.CanonicalUrlsEnabled, storeId);
            model.WwwRequirement_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.WwwRequirement, storeId);
            model.TwitterMetaTags_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.TwitterMetaTags, storeId);
            model.OpenGraphMetaTags_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.OpenGraphMetaTags, storeId);
            model.CustomHeadTags_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.CustomHeadTags, storeId);
            model.MicrodataEnabled_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.MicrodataEnabled, storeId);

            return model;
        }

        /// <summary>
        /// Prepare security settings model
        /// </summary>
        /// <returns>Security settings model</returns>
        protected virtual async Task<SecuritySettingsModel> PrepareSecuritySettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var securitySettings = await _settingService.LoadSettingAsync<SecuritySettings>(storeId);

            //fill in model values from the entity
            var model = new SecuritySettingsModel
            {
                EncryptionKey = securitySettings.EncryptionKey,
                HoneypotEnabled = securitySettings.HoneypotEnabled
            };

            //fill in additional values (not existing in the entity)
            if (securitySettings.AdminAreaAllowedIpAddresses != null)
                model.AdminAreaAllowedIpAddresses = string.Join(",", securitySettings.AdminAreaAllowedIpAddresses);

            return model;
        }

        /// <summary>
        /// Prepare captcha settings model
        /// </summary>
        /// <returns>Captcha settings model</returns>
        protected virtual async Task<CaptchaSettingsModel> PrepareCaptchaSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var captchaSettings = await _settingService.LoadSettingAsync<CaptchaSettings>(storeId);

            //fill in model values from the entity
            var model = captchaSettings.ToSettingsModel<CaptchaSettingsModel>();

            model.CaptchaTypeValues = await captchaSettings.CaptchaType.ToSelectListAsync();

            if (storeId <= 0)
                return model;

            model.Enabled_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.Enabled, storeId);
            model.ShowOnLoginPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnLoginPage, storeId);
            model.ShowOnRegistrationPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnRegistrationPage, storeId);
            model.ShowOnContactUsPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnContactUsPage, storeId);
            model.ShowOnEmailWishlistToFriendPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnEmailWishlistToFriendPage, storeId);
            model.ShowOnEmailProductToFriendPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnEmailProductToFriendPage, storeId);
            model.ShowOnBlogCommentPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnBlogCommentPage, storeId);
            model.ShowOnNewsCommentPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnNewsCommentPage, storeId);
            model.ShowOnProductReviewPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnProductReviewPage, storeId);
            model.ShowOnApplyVendorPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnApplyVendorPage, storeId);
            model.ShowOnForgotPasswordPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnForgotPasswordPage, storeId);
            model.ShowOnForum_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnForum, storeId);
            model.ReCaptchaPublicKey_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ReCaptchaPublicKey, storeId);
            model.ReCaptchaPrivateKey_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ReCaptchaPrivateKey, storeId);
            model.CaptchaType_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.CaptchaType, storeId);
            model.ReCaptchaV3ScoreThreshold_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ReCaptchaV3ScoreThreshold, storeId);

            return model;
        }

        /// <summary>
        /// Prepare PDF settings model
        /// </summary>
        /// <returns>PDF settings model</returns>
        protected virtual async Task<PdfSettingsModel> PreparePdfSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var pdfSettings = await _settingService.LoadSettingAsync<PdfSettings>(storeId);

            //fill in model values from the entity
            var model = new PdfSettingsModel
            {
                LetterPageSizeEnabled = pdfSettings.LetterPageSizeEnabled,
                LogoPictureId = pdfSettings.LogoPictureId,
                DisablePdfInvoicesForPendingOrders = pdfSettings.DisablePdfInvoicesForPendingOrders,
                InvoiceFooterTextColumn1 = pdfSettings.InvoiceFooterTextColumn1,
                InvoiceFooterTextColumn2 = pdfSettings.InvoiceFooterTextColumn2
            };

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.LetterPageSizeEnabled_OverrideForStore = await _settingService.SettingExistsAsync(pdfSettings, x => x.LetterPageSizeEnabled, storeId);
            model.LogoPictureId_OverrideForStore = await _settingService.SettingExistsAsync(pdfSettings, x => x.LogoPictureId, storeId);
            model.DisablePdfInvoicesForPendingOrders_OverrideForStore = await _settingService.SettingExistsAsync(pdfSettings, x => x.DisablePdfInvoicesForPendingOrders, storeId);
            model.InvoiceFooterTextColumn1_OverrideForStore = await _settingService.SettingExistsAsync(pdfSettings, x => x.InvoiceFooterTextColumn1, storeId);
            model.InvoiceFooterTextColumn2_OverrideForStore = await _settingService.SettingExistsAsync(pdfSettings, x => x.InvoiceFooterTextColumn2, storeId);

            return model;
        }

        /// <summary>
        /// Prepare localization settings model
        /// </summary>
        /// <returns>Localization settings model</returns>
        protected virtual async Task<LocalizationSettingsModel> PrepareLocalizationSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var localizationSettings = await _settingService.LoadSettingAsync<LocalizationSettings>(storeId);

            //fill in model values from the entity
            var model = new LocalizationSettingsModel
            {
                UseImagesForLanguageSelection = localizationSettings.UseImagesForLanguageSelection,
                SeoFriendlyUrlsForLanguagesEnabled = localizationSettings.SeoFriendlyUrlsForLanguagesEnabled,
                AutomaticallyDetectLanguage = localizationSettings.AutomaticallyDetectLanguage,
                LoadAllLocaleRecordsOnStartup = localizationSettings.LoadAllLocaleRecordsOnStartup,
                LoadAllLocalizedPropertiesOnStartup = localizationSettings.LoadAllLocalizedPropertiesOnStartup,
                LoadAllUrlRecordsOnStartup = localizationSettings.LoadAllUrlRecordsOnStartup
            };

            return model;
        }

        /// <summary>
        /// Prepare admin area settings model
        /// </summary>
        /// <returns>Admin area settings model</returns>
        protected virtual async Task<AdminAreaSettingsModel> PrepareAdminAreaSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var adminAreaSettings = await _settingService.LoadSettingAsync<AdminAreaSettings>(storeId);

            //fill in model values from the entity
            var model = new AdminAreaSettingsModel
            {
                UseRichEditorInMessageTemplates = adminAreaSettings.UseRichEditorInMessageTemplates
            };

            //fill in overridden values
            if (storeId > 0)
            {
                model.UseRichEditorInMessageTemplates_OverrideForStore = await _settingService.SettingExistsAsync(adminAreaSettings, x => x.UseRichEditorInMessageTemplates, storeId);
            }

            return model;
        }

        /// <summary>
        /// Prepare display default menu item settings model
        /// </summary>
        /// <returns>Display default menu item settings model</returns>
        protected virtual async Task<DisplayDefaultMenuItemSettingsModel> PrepareDisplayDefaultMenuItemSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var displayDefaultMenuItemSettings = await _settingService.LoadSettingAsync<DisplayDefaultMenuItemSettings>(storeId);

            //fill in model values from the entity
            var model = new DisplayDefaultMenuItemSettingsModel
            {
                DisplayHomepageMenuItem = displayDefaultMenuItemSettings.DisplayHomepageMenuItem,
                DisplayNewProductsMenuItem = displayDefaultMenuItemSettings.DisplayNewProductsMenuItem,
                DisplayProductSearchMenuItem = displayDefaultMenuItemSettings.DisplayProductSearchMenuItem,
                DisplayUserInfoMenuItem = displayDefaultMenuItemSettings.DisplayUserInfoMenuItem,
                DisplayBlogMenuItem = displayDefaultMenuItemSettings.DisplayBlogMenuItem,
                DisplayForumsMenuItem = displayDefaultMenuItemSettings.DisplayForumsMenuItem,
                DisplayContactUsMenuItem = displayDefaultMenuItemSettings.DisplayContactUsMenuItem
            };

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.DisplayHomepageMenuItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultMenuItemSettings, x => x.DisplayHomepageMenuItem, storeId);
            model.DisplayNewProductsMenuItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultMenuItemSettings, x => x.DisplayNewProductsMenuItem, storeId);
            model.DisplayProductSearchMenuItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultMenuItemSettings, x => x.DisplayProductSearchMenuItem, storeId);
            model.DisplayUserInfoMenuItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultMenuItemSettings, x => x.DisplayUserInfoMenuItem, storeId);
            model.DisplayBlogMenuItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultMenuItemSettings, x => x.DisplayBlogMenuItem, storeId);
            model.DisplayForumsMenuItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultMenuItemSettings, x => x.DisplayForumsMenuItem, storeId);
            model.DisplayContactUsMenuItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultMenuItemSettings, x => x.DisplayContactUsMenuItem, storeId);

            return model;
        }

        /// <summary>
        /// Prepare display default footer item settings model
        /// </summary>
        /// <returns>Display default footer item settings model</returns>
        protected virtual async Task<DisplayDefaultFooterItemSettingsModel> PrepareDisplayDefaultFooterItemSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var displayDefaultFooterItemSettings = await _settingService.LoadSettingAsync<DisplayDefaultFooterItemSettings>(storeId);

            //fill in model values from the entity
            var model = new DisplayDefaultFooterItemSettingsModel
            {
                DisplaySitemapFooterItem = displayDefaultFooterItemSettings.DisplaySitemapFooterItem,
                DisplayContactUsFooterItem = displayDefaultFooterItemSettings.DisplayContactUsFooterItem,
                DisplayProductSearchFooterItem = displayDefaultFooterItemSettings.DisplayProductSearchFooterItem,
                DisplayNewsFooterItem = displayDefaultFooterItemSettings.DisplayNewsFooterItem,
                DisplayBlogFooterItem = displayDefaultFooterItemSettings.DisplayBlogFooterItem,
                DisplayForumsFooterItem = displayDefaultFooterItemSettings.DisplayForumsFooterItem,
                DisplayRecentlyViewedProductsFooterItem = displayDefaultFooterItemSettings.DisplayRecentlyViewedProductsFooterItem,
                DisplayCompareProductsFooterItem = displayDefaultFooterItemSettings.DisplayCompareProductsFooterItem,
                DisplayNewProductsFooterItem = displayDefaultFooterItemSettings.DisplayNewProductsFooterItem,
                DisplayUserInfoFooterItem = displayDefaultFooterItemSettings.DisplayUserInfoFooterItem,
                DisplayUserOrdersFooterItem = displayDefaultFooterItemSettings.DisplayUserOrdersFooterItem,
                DisplayUserAddressesFooterItem = displayDefaultFooterItemSettings.DisplayUserAddressesFooterItem,
                DisplayShoppingCartFooterItem = displayDefaultFooterItemSettings.DisplayShoppingCartFooterItem,
                DisplayWishlistFooterItem = displayDefaultFooterItemSettings.DisplayWishlistFooterItem,
                DisplayApplyVendorAccountFooterItem = displayDefaultFooterItemSettings.DisplayApplyVendorAccountFooterItem
            };

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.DisplaySitemapFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplaySitemapFooterItem, storeId);
            model.DisplayContactUsFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayContactUsFooterItem, storeId);
            model.DisplayProductSearchFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayProductSearchFooterItem, storeId);
            model.DisplayNewsFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayNewsFooterItem, storeId);
            model.DisplayBlogFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayBlogFooterItem, storeId);
            model.DisplayForumsFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayForumsFooterItem, storeId);
            model.DisplayRecentlyViewedProductsFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayRecentlyViewedProductsFooterItem, storeId);
            model.DisplayCompareProductsFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayCompareProductsFooterItem, storeId);
            model.DisplayNewProductsFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayNewProductsFooterItem, storeId);
            model.DisplayUserInfoFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayUserInfoFooterItem, storeId);
            model.DisplayUserOrdersFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayUserOrdersFooterItem, storeId);
            model.DisplayUserAddressesFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayUserAddressesFooterItem, storeId);
            model.DisplayShoppingCartFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayShoppingCartFooterItem, storeId);
            model.DisplayWishlistFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayWishlistFooterItem, storeId);
            model.DisplayApplyVendorAccountFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayApplyVendorAccountFooterItem, storeId);

            return model;
        }

        /// <summary>
        /// Prepare setting model to add
        /// </summary>
        /// <param name="model">Setting model to add</param>
        protected virtual async Task PrepareAddSettingModelAsync(SettingModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare app settings model
        /// </summary>
        /// <returns>App settings model</returns>
        public virtual AppSettingsModel PrepareAppSettingsModel()
        {
            var model = new AppSettingsModel
            {
                CacheConfigModel = _appSettings.CacheConfig.ToConfigModel<CacheConfigModel>(),
                HostingConfigModel = _appSettings.HostingConfig.ToConfigModel<HostingConfigModel>(),
                RedisConfigModel = _appSettings.RedisConfig.ToConfigModel<RedisConfigModel>(),
                AzureBlobConfigModel = _appSettings.AzureBlobConfig.ToConfigModel<AzureBlobConfigModel>(),
                InstallationConfigModel = _appSettings.InstallationConfig.ToConfigModel<InstallationConfigModel>(),
                PluginConfigModel = _appSettings.PluginConfig.ToConfigModel<PluginConfigModel>(),
                CommonConfigModel = _appSettings.CommonConfig.ToConfigModel<CommonConfigModel>()
            };

            return model;
        }

        /// <summary>
        /// Prepare blog settings model
        /// </summary>
        /// <returns>Blog settings model</returns>
        public virtual async Task<BlogSettingsModel> PrepareBlogSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var blogSettings = await _settingService.LoadSettingAsync<BlogSettings>(storeId);

            //fill in model values from the entity
            var model = blogSettings.ToSettingsModel<BlogSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.Enabled_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.Enabled, storeId);
            model.PostsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.PostsPageSize, storeId);
            model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.AllowNotRegisteredUsersToLeaveComments, storeId);
            model.NotifyAboutNewBlogComments_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.NotifyAboutNewBlogComments, storeId);
            model.NumberOfTags_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.NumberOfTags, storeId);
            model.ShowHeaderRssUrl_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.ShowHeaderRssUrl, storeId);
            model.BlogCommentsMustBeApproved_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.BlogCommentsMustBeApproved, storeId);

            return model;
        }

        /// <summary>
        /// Prepare vendor settings model
        /// </summary>
        /// <returns>Vendor settings model</returns>
        public virtual async Task<VendorSettingsModel> PrepareVendorSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var vendorSettings = await _settingService.LoadSettingAsync<VendorSettings>(storeId);

            //fill in model values from the entity
            var model = vendorSettings.ToSettingsModel<VendorSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;

            //fill in overridden values
            if (storeId > 0)
            {
                model.VendorsBlockItemsToDisplay_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.VendorsBlockItemsToDisplay, storeId);
                model.ShowVendorOnProductDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.ShowVendorOnProductDetailsPage, storeId);
                model.ShowVendorOnOrderDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.ShowVendorOnOrderDetailsPage, storeId);
                model.AllowUsersToContactVendors_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.AllowUsersToContactVendors, storeId);
                model.AllowUsersToApplyForVendorAccount_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.AllowUsersToApplyForVendorAccount, storeId);
                model.TermsOfServiceEnabled_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.TermsOfServiceEnabled, storeId);
                model.AllowSearchByVendor_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.AllowSearchByVendor, storeId);
                model.AllowVendorsToEditInfo_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.AllowVendorsToEditInfo, storeId);
                model.NotifyStoreOwnerAboutVendorInformationChange_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.NotifyStoreOwnerAboutVendorInformationChange, storeId);
                model.MaximumProductNumber_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.MaximumProductNumber, storeId);
                model.AllowVendorsToImportProducts_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.AllowVendorsToImportProducts, storeId);
            }

            //prepare nested search model
            await _vendorAttributeModelFactory.PrepareVendorAttributeSearchModelAsync(model.VendorAttributeSearchModel);

            return model;
        }

        /// <summary>
        /// Prepare forum settings model
        /// </summary>
        /// <returns>Forum settings model</returns>
        public virtual async Task<ForumSettingsModel> PrepareForumSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var forumSettings = await _settingService.LoadSettingAsync<ForumSettings>(storeId);

            //fill in model values from the entity
            var model = forumSettings.ToSettingsModel<ForumSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;
            model.ForumEditorValues = await forumSettings.ForumEditor.ToSelectListAsync();

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.ForumsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ForumsEnabled, storeId);
            model.RelativeDateTimeFormattingEnabled_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.RelativeDateTimeFormattingEnabled, storeId);
            model.ShowUsersPostCount_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ShowUsersPostCount, storeId);
            model.AllowGuestsToCreatePosts_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowGuestsToCreatePosts, storeId);
            model.AllowGuestsToCreateTopics_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowGuestsToCreateTopics, storeId);
            model.AllowUsersToEditPosts_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowUsersToEditPosts, storeId);
            model.AllowUsersToDeletePosts_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowUsersToDeletePosts, storeId);
            model.AllowPostVoting_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowPostVoting, storeId);
            model.MaxVotesPerDay_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.MaxVotesPerDay, storeId);
            model.AllowUsersToManageSubscriptions_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowUsersToManageSubscriptions, storeId);
            model.TopicsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.TopicsPageSize, storeId);
            model.PostsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.PostsPageSize, storeId);
            model.ForumEditor_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ForumEditor, storeId);
            model.SignaturesEnabled_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.SignaturesEnabled, storeId);
            model.AllowPrivateMessages_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowPrivateMessages, storeId);
            model.ShowAlertForPM_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ShowAlertForPM, storeId);
            model.NotifyAboutPrivateMessages_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.NotifyAboutPrivateMessages, storeId);
            model.ActiveDiscussionsFeedEnabled_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ActiveDiscussionsFeedEnabled, storeId);
            model.ActiveDiscussionsFeedCount_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ActiveDiscussionsFeedCount, storeId);
            model.ForumFeedsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ForumFeedsEnabled, storeId);
            model.ForumFeedCount_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ForumFeedCount, storeId);
            model.SearchResultsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.SearchResultsPageSize, storeId);
            model.ActiveDiscussionsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ActiveDiscussionsPageSize, storeId);

            return model;
        }

        /// <summary>
        /// Prepare news settings model
        /// </summary>
        /// <returns>News settings model</returns>
        public virtual async Task<NewsSettingsModel> PrepareNewsSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var newsSettings = await _settingService.LoadSettingAsync<NewsSettings>(storeId);

            //fill in model values from the entity
            var model = newsSettings.ToSettingsModel<NewsSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.Enabled_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.Enabled, storeId);
            model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.AllowNotRegisteredUsersToLeaveComments, storeId);
            model.NotifyAboutNewNewsComments_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.NotifyAboutNewNewsComments, storeId);
            model.ShowNewsOnMainPage_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.ShowNewsOnMainPage, storeId);
            model.MainPageNewsCount_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.MainPageNewsCount, storeId);
            model.NewsArchivePageSize_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.NewsArchivePageSize, storeId);
            model.ShowHeaderRssUrl_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.ShowHeaderRssUrl, storeId);
            model.NewsCommentsMustBeApproved_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.NewsCommentsMustBeApproved, storeId);

            return model;
        }

        /// <summary>
        /// Prepare shipping settings model
        /// </summary>
        /// <returns>Shipping settings model</returns>
        public virtual async Task<ShippingSettingsModel> PrepareShippingSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var shippingSettings = await _settingService.LoadSettingAsync<ShippingSettings>(storeId);

            //fill in model values from the entity
            var model = shippingSettings.ToSettingsModel<ShippingSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;
            model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;

            //fill in overridden values
            if (storeId > 0)
            {
                model.ShipToSameAddress_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.ShipToSameAddress, storeId);
                model.AllowPickupInStore_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.AllowPickupInStore, storeId);
                model.DisplayPickupPointsOnMap_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.DisplayPickupPointsOnMap, storeId);
                model.IgnoreAdditionalShippingChargeForPickupInStore_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.IgnoreAdditionalShippingChargeForPickupInStore, storeId);
                model.GoogleMapsApiKey_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.GoogleMapsApiKey, storeId);
                model.UseWarehouseLocation_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.UseWarehouseLocation, storeId);
                model.NotifyUserAboutShippingFromMultipleLocations_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.NotifyUserAboutShippingFromMultipleLocations, storeId);
                model.FreeShippingOverXEnabled_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.FreeShippingOverXEnabled, storeId);
                model.FreeShippingOverXValue_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.FreeShippingOverXValue, storeId);
                model.FreeShippingOverXIncludingTax_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.FreeShippingOverXIncludingTax, storeId);
                model.EstimateShippingCartPageEnabled_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.EstimateShippingCartPageEnabled, storeId);
                model.EstimateShippingProductPageEnabled_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.EstimateShippingProductPageEnabled, storeId);
                model.DisplayShipmentEventsToUsers_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.DisplayShipmentEventsToUsers, storeId);
                model.DisplayShipmentEventsToStoreOwner_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.DisplayShipmentEventsToStoreOwner, storeId);
                model.HideShippingTotal_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.HideShippingTotal, storeId);
                model.BypassShippingMethodSelectionIfOnlyOne_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.BypassShippingMethodSelectionIfOnlyOne, storeId);
                model.ConsiderAssociatedProductsDimensions_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.ConsiderAssociatedProductsDimensions, storeId);
                model.ShippingOriginAddress_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.ShippingOriginAddressId, storeId);
            }

            //prepare shipping origin address
            var originAddress = await _addressService.GetAddressByIdAsync(shippingSettings.ShippingOriginAddressId);
            if (originAddress != null)
                model.ShippingOriginAddress = originAddress.ToModel(model.ShippingOriginAddress);
            await PrepareAddressModelAsync(model.ShippingOriginAddress, originAddress);

            return model;
        }

        /// <summary>
        /// Prepare tax settings model
        /// </summary>
        /// <returns>Tax settings model</returns>
        public virtual async Task<TaxSettingsModel> PrepareTaxSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var taxSettings = await _settingService.LoadSettingAsync<TaxSettings>(storeId);

            //fill in model values from the entity
            var model = taxSettings.ToSettingsModel<TaxSettingsModel>();
            model.TaxBasedOnValues = await taxSettings.TaxBasedOn.ToSelectListAsync();
            model.TaxDisplayTypeValues = await taxSettings.TaxDisplayType.ToSelectListAsync();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;

            //fill in overridden values
            if (storeId > 0)
            {
                model.PricesIncludeTax_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.PricesIncludeTax, storeId);
                model.AllowUsersToSelectTaxDisplayType_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.AllowUsersToSelectTaxDisplayType, storeId);
                model.TaxDisplayType_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.TaxDisplayType, storeId);
                model.DisplayTaxSuffix_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.DisplayTaxSuffix, storeId);
                model.DisplayTaxRates_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.DisplayTaxRates, storeId);
                model.HideZeroTax_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.HideZeroTax, storeId);
                model.HideTaxInOrderSummary_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.HideTaxInOrderSummary, storeId);
                model.ForceTaxExclusionFromOrderSubtotal_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.ForceTaxExclusionFromOrderSubtotal, storeId);
                model.DefaultTaxCategoryId_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.DefaultTaxCategoryId, storeId);
                model.TaxBasedOn_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.TaxBasedOn, storeId);
                model.TaxBasedOnPickupPointAddress_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.TaxBasedOnPickupPointAddress, storeId);
                model.DefaultTaxAddress_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.DefaultTaxAddressId, storeId);
                model.ShippingIsTaxable_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.ShippingIsTaxable, storeId);
                model.ShippingPriceIncludesTax_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.ShippingPriceIncludesTax, storeId);
                model.ShippingTaxClassId_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.ShippingTaxClassId, storeId);
                model.PaymentMethodAdditionalFeeIsTaxable_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.PaymentMethodAdditionalFeeIsTaxable, storeId);
                model.PaymentMethodAdditionalFeeIncludesTax_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.PaymentMethodAdditionalFeeIncludesTax, storeId);
                model.PaymentMethodAdditionalFeeTaxClassId_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.PaymentMethodAdditionalFeeTaxClassId, storeId);
                model.EuVatEnabled_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.EuVatEnabled, storeId);
                model.EuVatShopCountryId_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.EuVatShopCountryId, storeId);
                model.EuVatAllowVatExemption_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.EuVatAllowVatExemption, storeId);
                model.EuVatUseWebService_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.EuVatUseWebService, storeId);
                model.EuVatAssumeValid_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.EuVatAssumeValid, storeId);
                model.EuVatEmailAdminWhenNewVatSubmitted_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.EuVatEmailAdminWhenNewVatSubmitted, storeId);
            }

            //prepare available tax categories
            await _baseAdminModelFactory.PrepareTaxCategoriesAsync(model.TaxCategories);

            //prepare available EU VAT countries
            await _baseAdminModelFactory.PrepareCountriesAsync(model.EuVatShopCountries);

            //prepare default tax address
            var defaultAddress = await _addressService.GetAddressByIdAsync(taxSettings.DefaultTaxAddressId);
            if (defaultAddress != null)
                model.DefaultTaxAddress = defaultAddress.ToModel(model.DefaultTaxAddress);
            await PrepareAddressModelAsync(model.DefaultTaxAddress, defaultAddress);

            return model;
        }

        /// <summary>
        /// Prepare catalog settings model
        /// </summary>
        /// <returns>Catalog settings model</returns>
        public virtual async Task<CatalogSettingsModel> PrepareCatalogSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var catalogSettings = await _settingService.LoadSettingAsync<CatalogSettings>(storeId);

            //fill in model values from the entity
            var model = catalogSettings.ToSettingsModel<CatalogSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;
            model.AvailableViewModes.Add(new SelectListItem
            {
                Text = await _localizationService.GetResourceAsync("Admin.Catalog.ViewMode.Grid"),
                Value = "grid"
            });
            model.AvailableViewModes.Add(new SelectListItem
            {
                Text = await _localizationService.GetResourceAsync("Admin.Catalog.ViewMode.List"),
                Value = "list"
            });

            //fill in overridden values
            if (storeId > 0)
            {
                model.AllowViewUnpublishedProductPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AllowViewUnpublishedProductPage, storeId);
                model.DisplayDiscontinuedMessageForUnpublishedProducts_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayDiscontinuedMessageForUnpublishedProducts, storeId);
                model.ShowSkuOnProductDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowSkuOnProductDetailsPage, storeId);
                model.ShowSkuOnCatalogPages_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowSkuOnCatalogPages, storeId);
                model.ShowManufacturerPartNumber_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowManufacturerPartNumber, storeId);
                model.ShowGtin_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowGtin, storeId);
                model.ShowFreeShippingNotification_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowFreeShippingNotification, storeId);
                model.AllowProductSorting_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AllowProductSorting, storeId);
                model.AllowProductViewModeChanging_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AllowProductViewModeChanging, storeId);
                model.DefaultViewMode_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DefaultViewMode, storeId);
                model.ShowProductsFromSubcategories_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowProductsFromSubcategories, storeId);
                model.ShowCategoryProductNumber_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowCategoryProductNumber, storeId);
                model.ShowCategoryProductNumberIncludingSubcategories_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowCategoryProductNumberIncludingSubcategories, storeId);
                model.CategoryBreadcrumbEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.CategoryBreadcrumbEnabled, storeId);
                model.ShowShareButton_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowShareButton, storeId);
                model.PageShareCode_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.PageShareCode, storeId);
                model.ProductReviewsMustBeApproved_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductReviewsMustBeApproved, storeId);
                model.OneReviewPerProductFromUser_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.OneReviewPerProductFromUser, storeId);
                model.AllowAnonymousUsersToReviewProduct_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AllowAnonymousUsersToReviewProduct, storeId);
                model.ProductReviewPossibleOnlyAfterPurchasing_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductReviewPossibleOnlyAfterPurchasing, storeId);
                model.NotifyStoreOwnerAboutNewProductReviews_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NotifyStoreOwnerAboutNewProductReviews, storeId);
                model.NotifyUserAboutProductReviewReply_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NotifyUserAboutProductReviewReply, storeId);
                model.EmailAFriendEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.EmailAFriendEnabled, storeId);
                model.AllowAnonymousUsersToEmailAFriend_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AllowAnonymousUsersToEmailAFriend, storeId);
                model.RecentlyViewedProductsNumber_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.RecentlyViewedProductsNumber, storeId);
                model.RecentlyViewedProductsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.RecentlyViewedProductsEnabled, storeId);
                model.NewProductsNumber_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NewProductsNumber, storeId);
                model.NewProductsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NewProductsEnabled, storeId);
                model.CompareProductsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.CompareProductsEnabled, storeId);
                model.ShowBestsellersOnHomepage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowBestsellersOnHomepage, storeId);
                model.NumberOfBestsellersOnHomepage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NumberOfBestsellersOnHomepage, storeId);
                model.SearchPageProductsPerPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.SearchPageProductsPerPage, storeId);
                model.SearchPageAllowUsersToSelectPageSize_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.SearchPageAllowUsersToSelectPageSize, storeId);
                model.SearchPagePageSizeOptions_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.SearchPagePageSizeOptions, storeId);
                model.ProductSearchAutoCompleteEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductSearchAutoCompleteEnabled, storeId);
                model.ProductSearchAutoCompleteNumberOfProducts_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductSearchAutoCompleteNumberOfProducts, storeId);
                model.ShowProductImagesInSearchAutoComplete_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowProductImagesInSearchAutoComplete, storeId);
                model.ShowLinkToAllResultInSearchAutoComplete_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowLinkToAllResultInSearchAutoComplete, storeId);
                model.ProductSearchTermMinimumLength_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductSearchTermMinimumLength, storeId);
                model.ProductsAlsoPurchasedEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductsAlsoPurchasedEnabled, storeId);
                model.ProductsAlsoPurchasedNumber_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductsAlsoPurchasedNumber, storeId);
                model.NumberOfProductTags_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NumberOfProductTags, storeId);
                model.ProductsByTagPageSize_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductsByTagPageSize, storeId);
                model.ProductsByTagAllowUsersToSelectPageSize_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductsByTagAllowUsersToSelectPageSize, storeId);
                model.ProductsByTagPageSizeOptions_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductsByTagPageSizeOptions, storeId);
                model.IncludeShortDescriptionInCompareProducts_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.IncludeShortDescriptionInCompareProducts, storeId);
                model.IncludeFullDescriptionInCompareProducts_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.IncludeFullDescriptionInCompareProducts, storeId);
                model.ManufacturersBlockItemsToDisplay_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ManufacturersBlockItemsToDisplay, storeId);
                model.DisplayTaxShippingInfoFooter_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoFooter, storeId);
                model.DisplayTaxShippingInfoProductDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoProductDetailsPage, storeId);
                model.DisplayTaxShippingInfoProductBoxes_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoProductBoxes, storeId);
                model.DisplayTaxShippingInfoShoppingCart_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoShoppingCart, storeId);
                model.DisplayTaxShippingInfoWishlist_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoWishlist, storeId);
                model.DisplayTaxShippingInfoOrderDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoOrderDetailsPage, storeId);
                model.ShowProductReviewsPerStore_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowProductReviewsPerStore, storeId);
                model.ShowProductReviewsOnAccountPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowProductReviewsTabOnAccountPage, storeId);
                model.ProductReviewsPageSizeOnAccountPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductReviewsPageSizeOnAccountPage, storeId);
                model.ProductReviewsSortByCreatedDateAscending_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductReviewsSortByCreatedDateAscending, storeId);
                model.ExportImportProductAttributes_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportProductAttributes, storeId);
                model.ExportImportProductSpecificationAttributes_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportProductSpecificationAttributes, storeId);
                model.ExportImportProductCategoryBreadcrumb_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportProductCategoryBreadcrumb, storeId);
                model.ExportImportCategoriesUsingCategoryName_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportCategoriesUsingCategoryName, storeId);
                model.ExportImportAllowDownloadImages_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportAllowDownloadImages, storeId);
                model.ExportImportSplitProductsFile_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportSplitProductsFile, storeId);
                model.RemoveRequiredProducts_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.RemoveRequiredProducts, storeId);
                model.ExportImportRelatedEntitiesByName_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportRelatedEntitiesByName, storeId);
                model.ExportImportProductUseLimitedToStores_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportProductUseLimitedToStores, storeId);
                model.DisplayDatePreOrderAvailability_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayDatePreOrderAvailability, storeId);
            }

            //prepare nested search model
            await PrepareSortOptionSearchModelAsync(model.SortOptionSearchModel);
            await _reviewTypeModelFactory.PrepareReviewTypeSearchModelAsync(model.ReviewTypeSearchModel);

            return model;
        }

        /// <summary>
        /// Prepare paged sort option list model
        /// </summary>
        /// <param name="searchModel">Sort option search model</param>
        /// <returns>Sort option list model</returns>
        public virtual async Task<SortOptionListModel> PrepareSortOptionListModelAsync(SortOptionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var catalogSettings = await _settingService.LoadSettingAsync<CatalogSettings>(storeId);

            //get sort options
            var sortOptions = Enum.GetValues(typeof(ProductSortingEnum)).OfType<ProductSortingEnum>().ToList().ToPagedList(searchModel);

            //prepare list model
            var model = await new SortOptionListModel().PrepareToGridAsync(searchModel, sortOptions, () =>
            {
                return sortOptions.SelectAwait(async option =>
                {
                    //fill in model values from the entity
                    var sortOptionModel = new SortOptionModel { Id = (int)option };

                    //fill in additional values (not existing in the entity)
                    sortOptionModel.Name = await _localizationService.GetLocalizedEnumAsync(option);
                    sortOptionModel.IsActive = !catalogSettings.ProductSortingEnumDisabled.Contains((int)option);
                    sortOptionModel.DisplayOrder = catalogSettings
                        .ProductSortingEnumDisplayOrder.TryGetValue((int)option, out var value) ? value : (int)option;

                    return sortOptionModel;
                }).OrderBy(option => option.DisplayOrder);
            });

            return model;
        }

        /// <summary>
        /// Prepare reward points settings model
        /// </summary>
        /// <returns>Reward points settings model</returns>
        public virtual async Task<RewardPointsSettingsModel> PrepareRewardPointsSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var rewardPointsSettings = await _settingService.LoadSettingAsync<RewardPointsSettings>(storeId);

            //fill in model values from the entity
            var model = rewardPointsSettings.ToSettingsModel<RewardPointsSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;
            model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
            model.ActivatePointsImmediately = model.ActivationDelay <= 0;

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.Enabled_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.Enabled, storeId);
            model.ExchangeRate_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.ExchangeRate, storeId);
            model.MinimumRewardPointsToUse_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.MinimumRewardPointsToUse, storeId);
            model.MaximumRewardPointsToUsePerOrder_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.MaximumRewardPointsToUsePerOrder, storeId);
            model.PointsForRegistration_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.PointsForRegistration, storeId);
            model.RegistrationPointsValidity_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.RegistrationPointsValidity, storeId);
            model.PointsForPurchases_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.PointsForPurchases_Amount, storeId) || await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.PointsForPurchases_Points, storeId);
            model.MinOrderTotalToAwardPoints_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.MinOrderTotalToAwardPoints, storeId);
            model.PurchasesPointsValidity_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.PurchasesPointsValidity, storeId);
            model.ActivationDelay_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.ActivationDelay, storeId);
            model.DisplayHowMuchWillBeEarned_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.DisplayHowMuchWillBeEarned, storeId);
            model.PointsForRegistration_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.PointsForRegistration, storeId);
            model.PageSize_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.PageSize, storeId);

            return model;
        }

        /// <summary>
        /// Prepare order settings model
        /// </summary>
        /// <returns>Order settings model</returns>
        public virtual async Task<OrderSettingsModel> PrepareOrderSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var orderSettings = await _settingService.LoadSettingAsync<OrderSettings>(storeId);

            //fill in model values from the entity
            var model = orderSettings.ToSettingsModel<OrderSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;
            model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
            model.OrderIdent = await _dataProvider.GetTableIdentAsync<Order>();

            //fill in overridden values
            if (storeId > 0)
            {
                model.IsReOrderAllowed_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.IsReOrderAllowed, storeId);
                model.MinOrderSubtotalAmount_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.MinOrderSubtotalAmount, storeId);
                model.MinOrderSubtotalAmountIncludingTax_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.MinOrderSubtotalAmountIncludingTax, storeId);
                model.MinOrderTotalAmount_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.MinOrderTotalAmount, storeId);
                model.AutoUpdateOrderTotalsOnEditingOrder_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.AutoUpdateOrderTotalsOnEditingOrder, storeId);
                model.AnonymousCheckoutAllowed_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.AnonymousCheckoutAllowed, storeId);
                model.CheckoutDisabled_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.CheckoutDisabled, storeId);
                model.TermsOfServiceOnShoppingCartPage_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.TermsOfServiceOnShoppingCartPage, storeId);
                model.TermsOfServiceOnOrderConfirmPage_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.TermsOfServiceOnOrderConfirmPage, storeId);
                model.OnePageCheckoutEnabled_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.OnePageCheckoutEnabled, storeId);
                model.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab, storeId);
                model.DisableBillingAddressCheckoutStep_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.DisableBillingAddressCheckoutStep, storeId);
                model.DisableOrderCompletedPage_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.DisableOrderCompletedPage, storeId);
                model.DisplayPickupInStoreOnShippingMethodPage_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.DisplayPickupInStoreOnShippingMethodPage, storeId);
                model.AttachPdfInvoiceToOrderPlacedEmail_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.AttachPdfInvoiceToOrderPlacedEmail, storeId);
                model.AttachPdfInvoiceToOrderPaidEmail_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.AttachPdfInvoiceToOrderPaidEmail, storeId);
                model.AttachPdfInvoiceToOrderCompletedEmail_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.AttachPdfInvoiceToOrderCompletedEmail, storeId);
                model.ReturnRequestsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.ReturnRequestsEnabled, storeId);
                model.ReturnRequestsAllowFiles_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.ReturnRequestsAllowFiles, storeId);
                model.ReturnRequestNumberMask_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.ReturnRequestNumberMask, storeId);
                model.NumberOfDaysReturnRequestAvailable_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.NumberOfDaysReturnRequestAvailable, storeId);
                model.CustomOrderNumberMask_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.CustomOrderNumberMask, storeId);
                model.ExportWithProducts_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.ExportWithProducts, storeId);
                model.AllowAdminsToBuyCallForPriceProducts_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.AllowAdminsToBuyCallForPriceProducts, storeId);
                model.DeleteGiftCardUsageHistory_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.DeleteGiftCardUsageHistory, storeId);
            }

            //prepare nested search models
            await _returnRequestModelFactory.PrepareReturnRequestReasonSearchModelAsync(model.ReturnRequestReasonSearchModel);
            await _returnRequestModelFactory.PrepareReturnRequestActionSearchModelAsync(model.ReturnRequestActionSearchModel);

            return model;
        }

        /// <summary>
        /// Prepare shopping cart settings model
        /// </summary>
        /// <returns>Shopping cart settings model</returns>
        public virtual async Task<ShoppingCartSettingsModel> PrepareShoppingCartSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var shoppingCartSettings = await _settingService.LoadSettingAsync<ShoppingCartSettings>(storeId);

            //fill in model values from the entity
            var model = shoppingCartSettings.ToSettingsModel<ShoppingCartSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.DisplayCartAfterAddingProduct_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.DisplayCartAfterAddingProduct, storeId);
            model.DisplayWishlistAfterAddingProduct_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.DisplayWishlistAfterAddingProduct, storeId);
            model.MaximumShoppingCartItems_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.MaximumShoppingCartItems, storeId);
            model.MaximumWishlistItems_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.MaximumWishlistItems, storeId);
            model.AllowOutOfStockItemsToBeAddedToWishlist_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.AllowOutOfStockItemsToBeAddedToWishlist, storeId);
            model.MoveItemsFromWishlistToCart_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.MoveItemsFromWishlistToCart, storeId);
            model.CartsSharedBetweenStores_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.CartsSharedBetweenStores, storeId);
            model.ShowProductImagesOnShoppingCart_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.ShowProductImagesOnShoppingCart, storeId);
            model.ShowProductImagesOnWishList_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.ShowProductImagesOnWishList, storeId);
            model.ShowDiscountBox_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.ShowDiscountBox, storeId);
            model.ShowGiftCardBox_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.ShowGiftCardBox, storeId);
            model.CrossSellsNumber_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.CrossSellsNumber, storeId);
            model.EmailWishlistEnabled_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.EmailWishlistEnabled, storeId);
            model.AllowAnonymousUsersToEmailWishlist_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.AllowAnonymousUsersToEmailWishlist, storeId);
            model.MiniShoppingCartEnabled_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.MiniShoppingCartEnabled, storeId);
            model.ShowProductImagesInMiniShoppingCart_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.ShowProductImagesInMiniShoppingCart, storeId);
            model.MiniShoppingCartProductNumber_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.MiniShoppingCartProductNumber, storeId);
            model.AllowCartItemEditing_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.AllowCartItemEditing, storeId);
            model.GroupTierPricesForDistinctShoppingCartItems_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.GroupTierPricesForDistinctShoppingCartItems, storeId);

            return model;
        }

        /// <summary>
        /// Prepare media settings model
        /// </summary>
        /// <returns>Media settings model</returns>
        public virtual async Task<MediaSettingsModel> PrepareMediaSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var mediaSettings = await _settingService.LoadSettingAsync<MediaSettings>(storeId);

            //fill in model values from the entity
            var model = mediaSettings.ToSettingsModel<MediaSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;
            model.PicturesStoredIntoDatabase = await _pictureService.IsStoreInDbAsync();

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.AvatarPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.AvatarPictureSize, storeId);
            model.ProductThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.ProductThumbPictureSize, storeId);
            model.ProductDetailsPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.ProductDetailsPictureSize, storeId);
            model.ProductThumbPictureSizeOnProductDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.ProductThumbPictureSizeOnProductDetailsPage, storeId);
            model.AssociatedProductPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.AssociatedProductPictureSize, storeId);
            model.CategoryThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.CategoryThumbPictureSize, storeId);
            model.ManufacturerThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.ManufacturerThumbPictureSize, storeId);
            model.VendorThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.VendorThumbPictureSize, storeId);
            model.CartThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.CartThumbPictureSize, storeId);
            model.MiniCartThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.MiniCartThumbPictureSize, storeId);
            model.MaximumImageSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.MaximumImageSize, storeId);
            model.MultipleThumbDirectories_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.MultipleThumbDirectories, storeId);
            model.DefaultImageQuality_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.DefaultImageQuality, storeId);
            model.ImportProductImagesUsingHash_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.ImportProductImagesUsingHash, storeId);
            model.DefaultPictureZoomEnabled_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.DefaultPictureZoomEnabled, storeId);

            return model;
        }

        /// <summary>
        /// Prepare user user settings model
        /// </summary>
        /// <returns>User user settings model</returns>
        public virtual async Task<UserUserSettingsModel> PrepareUserUserSettingsModelAsync()
        {
            var model = new UserUserSettingsModel
            {
                ActiveStoreScopeConfiguration = await _storeContext.GetActiveStoreScopeConfigurationAsync()
            };

            //prepare user settings model
            model.UserSettings = await PrepareUserSettingsModelAsync();

            //prepare multi-factor authentication settings model
            model.MultiFactorAuthenticationSettings = await PrepareMultiFactorAuthenticationSettingsModelAsync();

            //prepare address settings model
            model.AddressSettings = await PrepareAddressSettingsModelAsync();

            //prepare date time settings model
            model.DateTimeSettings = await PrepareDateTimeSettingsModelAsync();

            //prepare external authentication settings model
            model.ExternalAuthenticationSettings = await PrepareExternalAuthenticationSettingsModelAsync();

            //prepare nested search models
            await _userAttributeModelFactory.PrepareUserAttributeSearchModelAsync(model.UserAttributeSearchModel);
            await _addressAttributeModelFactory.PrepareAddressAttributeSearchModelAsync(model.AddressAttributeSearchModel);

            return model;
        }

        /// <summary>
        /// Prepare GDPR settings model
        /// </summary>
        /// <returns>GDPR settings model</returns>
        public virtual async Task<GdprSettingsModel> PrepareGdprSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var gdprSettings = await _settingService.LoadSettingAsync<GdprSettings>(storeId);

            //fill in model values from the entity
            var model = gdprSettings.ToSettingsModel<GdprSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;

            //prepare nested search model
            await PrepareGdprConsentSearchModelAsync(model.GdprConsentSearchModel);

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.GdprEnabled_OverrideForStore = await _settingService.SettingExistsAsync(gdprSettings, x => x.GdprEnabled, storeId);
            model.LogPrivacyPolicyConsent_OverrideForStore = await _settingService.SettingExistsAsync(gdprSettings, x => x.LogPrivacyPolicyConsent, storeId);
            model.LogNewsletterConsent_OverrideForStore = await _settingService.SettingExistsAsync(gdprSettings, x => x.LogNewsletterConsent, storeId);
            model.LogUserProfileChanges_OverrideForStore = await _settingService.SettingExistsAsync(gdprSettings, x => x.LogUserProfileChanges, storeId);

            return model;
        }

        /// <summary>
        /// Prepare paged GDPR consent list model
        /// </summary>
        /// <param name="searchModel">GDPR search model</param>
        /// <returns>GDPR consent list model</returns>
        public virtual async Task<GdprConsentListModel> PrepareGdprConsentListModelAsync(GdprConsentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get sort options
            var consentList = (await _gdprService.GetAllConsentsAsync()).ToPagedList(searchModel);

            //prepare list model
            var model = await new GdprConsentListModel().PrepareToGridAsync(searchModel, consentList, () =>
            {
                return consentList.SelectAwait(async consent =>
                {
                    var gdprConsentModel = consent.ToModel<GdprConsentModel>();

                    var gdprConsent = await _gdprService.GetConsentByIdAsync(gdprConsentModel.Id);
                    gdprConsentModel.Message = await _localizationService.GetLocalizedAsync(gdprConsent, entity => entity.Message);
                    gdprConsentModel.RequiredMessage = await _localizationService.GetLocalizedAsync(gdprConsent, entity => entity.RequiredMessage);

                    return gdprConsentModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare GDPR consent model
        /// </summary>
        /// <param name="model">GDPR consent model</param>
        /// <param name="gdprConsent">GDPR consent</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>GDPR consent model</returns>
        public virtual async Task<GdprConsentModel> PrepareGdprConsentModelAsync(GdprConsentModel model, GdprConsent gdprConsent, bool excludeProperties = false)
        {
            Action<GdprConsentLocalizedModel, int> localizedModelConfiguration = null;

            //fill in model values from the entity
            if (gdprConsent != null)
            {
                model ??= gdprConsent.ToModel<GdprConsentModel>();

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Message = await _localizationService.GetLocalizedAsync(gdprConsent, entity => entity.Message, languageId, false, false);
                    locale.RequiredMessage = await _localizationService.GetLocalizedAsync(gdprConsent, entity => entity.RequiredMessage, languageId, false, false);
                };
            }

            //set default values for the new model
            if (gdprConsent == null)
                model.DisplayOrder = 1;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare general and common settings model
        /// </summary>
        /// <returns>General and common settings model</returns>
        public virtual async Task<GeneralCommonSettingsModel> PrepareGeneralCommonSettingsModelAsync()
        {
            var model = new GeneralCommonSettingsModel
            {
                ActiveStoreScopeConfiguration = await _storeContext.GetActiveStoreScopeConfigurationAsync()
            };

            //prepare store information settings model
            model.StoreInformationSettings = await PrepareStoreInformationSettingsModelAsync();

            //prepare Sitemap settings model
            model.SitemapSettings = await PrepareSitemapSettingsModelAsync();

            //prepare Minification settings model
            model.MinificationSettings = await PrepareMinificationSettingsModelAsync();

            //prepare SEO settings model
            model.SeoSettings = await PrepareSeoSettingsModelAsync();

            //prepare security settings model
            model.SecuritySettings = await PrepareSecuritySettingsModelAsync();

            //prepare captcha settings model
            model.CaptchaSettings = await PrepareCaptchaSettingsModelAsync();

            //prepare PDF settings model
            model.PdfSettings = await PreparePdfSettingsModelAsync();

            //prepare PDF settings model
            model.LocalizationSettings = await PrepareLocalizationSettingsModelAsync();

            //prepare admin area settings model
            model.AdminAreaSettings = await PrepareAdminAreaSettingsModelAsync();

            //prepare display default menu item settings model
            model.DisplayDefaultMenuItemSettings = await PrepareDisplayDefaultMenuItemSettingsModelAsync();

            //prepare display default footer item settings model
            model.DisplayDefaultFooterItemSettings = await PrepareDisplayDefaultFooterItemSettingsModelAsync();

            return model;
        }

        /// <summary>
        /// Prepare product editor settings model
        /// </summary>
        /// <returns>Product editor settings model</returns>
        public virtual async Task<ProductEditorSettingsModel> PrepareProductEditorSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var productEditorSettings = await _settingService.LoadSettingAsync<ProductEditorSettings>(storeId);

            //fill in model values from the entity
            var model = productEditorSettings.ToSettingsModel<ProductEditorSettingsModel>();

            return model;
        }

        /// <summary>
        /// Prepare setting search model
        /// </summary>
        /// <param name="searchModel">Setting search model</param>
        /// <returns>Setting search model</returns>
        public virtual async Task<SettingSearchModel> PrepareSettingSearchModelAsync(SettingSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare model to add
            await PrepareAddSettingModelAsync(searchModel.AddSetting);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged setting list model
        /// </summary>
        /// <param name="searchModel">Setting search model</param>
        /// <returns>Setting list model</returns>
        public virtual async Task<SettingListModel> PrepareSettingListModelAsync(SettingSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get settings
            var settings = (await _settingService.GetAllSettingsAsync()).AsQueryable();

            //filter settings
            if (!string.IsNullOrEmpty(searchModel.SearchSettingName))
                settings = settings.Where(setting => setting.Name.ToLowerInvariant().Contains(searchModel.SearchSettingName.ToLowerInvariant()));
            if (!string.IsNullOrEmpty(searchModel.SearchSettingValue))
                settings = settings.Where(setting => setting.Value.ToLowerInvariant().Contains(searchModel.SearchSettingValue.ToLowerInvariant()));

            var pagedSettings = settings.ToList().ToPagedList(searchModel);

            //prepare list model
            var model = await new SettingListModel().PrepareToGridAsync(searchModel, pagedSettings, () =>
            {
                return pagedSettings.SelectAwait(async setting =>
                {
                    //fill in model values from the entity
                    var settingModel = setting.ToModel<SettingModel>();

                    //fill in additional values (not existing in the entity)
                    settingModel.Store = setting.StoreId > 0
                        ? (await _storeService.GetStoreByIdAsync(setting.StoreId))?.Name ?? "Deleted"
                        : await _localizationService.GetResourceAsync("Admin.Configuration.Settings.AllSettings.Fields.StoreName.AllStores");

                    return settingModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare setting mode model
        /// </summary>
        /// <param name="modeName">Mode name</param>
        /// <returns>Setting mode model</returns>
        public virtual async Task<SettingModeModel> PrepareSettingModeModelAsync(string modeName)
        {
            var model = new SettingModeModel
            {
                ModeName = modeName,
                Enabled = await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentUserAsync(), modeName)
            };

            return model;
        }

        /// <summary>
        /// Prepare store scope configuration model
        /// </summary>
        /// <returns>Store scope configuration model</returns>
        public virtual async Task<StoreScopeConfigurationModel> PrepareStoreScopeConfigurationModelAsync()
        {
            var model = new StoreScopeConfigurationModel
            {
                Stores = (await _storeService.GetAllStoresAsync()).Select(store => store.ToModel<StoreModel>()).ToList(),
                StoreId = await _storeContext.GetActiveStoreScopeConfigurationAsync()
            };

            return model;
        }

        #endregion
    }
}