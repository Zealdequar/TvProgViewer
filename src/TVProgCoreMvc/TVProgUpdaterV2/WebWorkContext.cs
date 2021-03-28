﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Core.Domain.Localization;
using TVProgViewer.Core.Domain.Tax;
using TVProgViewer.Core.Domain.Vendors;
using TVProgViewer.Core.Http;
using TVProgViewer.Core.Security;
using TVProgViewer.Services.Authentication;
using TVProgViewer.Services.Common;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Directory;
using TVProgViewer.Services.Helpers;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Stores;
using TVProgViewer.Services.Vendors;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Services.TvProgMain;

namespace TVProgViewer.TVProgUpdaterV2
{
    /// <summary>
    /// Represents work context for web application
    /// </summary>
    public partial class WebWorkContext : IWorkContext
    {
        #region Поля

        private readonly CookieSettings _cookieSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICurrencyService _currencyService;
        private readonly IUserService _userService;
        private readonly IProgrammeService _programmeService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILanguageService _languageService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IVendorService _vendorService;
        private readonly IWebHelper _webHelper;
        private readonly LocalizationSettings _localizationSettings;
        private readonly TaxSettings _taxSettings;
        private readonly ProgrammeSettings _programmeSettings;

        private User _cachedUser;
        private User _originalUserIfImpersonated;
        private Vendor _cachedVendor;
        private Language _cachedLanguage;
        private Currency _cachedCurrency;
        private TvProgProviders _cachedProvider;
        private TypeProg _cachedTypeProg;
        private string _cachedCategory;
        private TaxDisplayType? _cachedTaxDisplayType;

        #endregion

        #region Ctor

        public WebWorkContext(CookieSettings cookieSettings,
            CurrencySettings currencySettings,
            IAuthenticationService authenticationService,
            ICurrencyService currencyService,
            IUserService userService,
            IProgrammeService programmeService,
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            ILanguageService languageService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IUserAgentHelper userAgentHelper,
            IVendorService vendorService,
            IWebHelper webHelper,
            LocalizationSettings localizationSettings,
            TaxSettings taxSettings,
            ProgrammeSettings programmeSettings)
        {
            _cookieSettings = cookieSettings;
            _currencySettings = currencySettings;
            _authenticationService = authenticationService;
            _currencyService = currencyService;
            _userService = userService;
            _programmeService = programmeService;
            _genericAttributeService = genericAttributeService;
            _httpContextAccessor = httpContextAccessor;
            _languageService = languageService;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _userAgentHelper = userAgentHelper;
            _vendorService = vendorService;
            _webHelper = webHelper;
            _localizationSettings = localizationSettings;
            _taxSettings = taxSettings;
            _programmeSettings = programmeSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get TVProgViewer User cookie
        /// </summary>
        /// <returns>String value of cookie</returns>
        protected virtual string GetUserCookie()
        {
            var cookieName = $"{TvProgCookieDefaults.Prefix}{TvProgCookieDefaults.UserCookie}";
            return _httpContextAccessor.HttpContext?.Request?.Cookies[cookieName];
        }

        /// <summary>
        /// Set TVProgViewer User cookie
        /// </summary>
        /// <param name="UserGuid">Guid of the User</param>
        protected virtual void SetUserCookie(Guid UserGuid)
        {
            if (_httpContextAccessor.HttpContext?.Response?.HasStarted ?? true)
                return;

            //delete current cookie value
            var cookieName = $"{TvProgCookieDefaults.Prefix}{TvProgCookieDefaults.UserCookie}";
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

            //get date of cookie expiration
            var cookieExpires = _cookieSettings.UserCookieExpires;
            var cookieExpiresDate = DateTime.Now.AddHours(cookieExpires);

            //if passed guid is empty set cookie as expired
            if (UserGuid == Guid.Empty)
                cookieExpiresDate = DateTime.Now.AddMonths(-1);

            //set new cookie value
            var options = new CookieOptions
            {
                HttpOnly = true,
                Expires = cookieExpiresDate,
                Secure = _webHelper.IsCurrentConnectionSecured()
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, UserGuid.ToString(), options);
        }

        /// <summary>
        /// Get language from the requested page URL
        /// </summary>
        /// <returns>The found language</returns>
        protected virtual async Task<Language> GetLanguageFromUrlAsync()
        {
            if (_httpContextAccessor.HttpContext?.Request == null)
                return null;

            //whether the requsted URL is localized
            var path = _httpContextAccessor.HttpContext.Request.Path.Value;
            
            var (isLocalized, language) = await path.IsLocalizedUrlAsync(_httpContextAccessor.HttpContext.Request.PathBase, false);

            if (!isLocalized)
                return null;
            //check language availability
            if (!await _storeMappingService.AuthorizeAsync(language))
                return null;

            return language;
        }

        /// <summary>
        /// Get language from the request
        /// </summary>
        /// <returns>The found language</returns>
        protected virtual async Task<Language> GetLanguageFromRequestAsync()
        {
            if (_httpContextAccessor.HttpContext?.Request == null)
                return null;

            //get request culture
            var requestCulture = _httpContextAccessor.HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture;
            if (requestCulture == null)
                return null;

            //try to get language by culture name
            var requestLanguage = (await _languageService.GetAllLanguagesAsync()).FirstOrDefault(language =>
                language.LanguageCulture.Equals(requestCulture.Culture.Name, StringComparison.InvariantCultureIgnoreCase));

            //check language availability
            if (requestLanguage == null || !requestLanguage.Published || !await _storeMappingService.AuthorizeAsync(requestLanguage))
                return null;

            return requestLanguage;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Текущий пользователь
        /// </summary>
        public virtual async Task<User> GetCurrentUserAsync()
        {
            // Закэширован ли он сейчас:
            if (_cachedUser != null)
                return _cachedUser;

            await SetCurrentUserAsync();

            return _cachedUser;
        }

        /// <summary>
        /// Sets the current user
        /// </summary>
        /// <param name="user">Current user</param>
        public virtual async Task SetCurrentUserAsync(User user = null)
        {
            if (user == null)
            {
                //check whether request is made by a background (schedule) task
                if (_httpContextAccessor.HttpContext?.Request
                    ?.Path.Equals(new PathString($"/{Services.Tasks.TvProgTaskDefaults.ScheduleTaskPath}"), StringComparison.InvariantCultureIgnoreCase)
                    ?? true)
                {
                    //in this case return built-in user record for background task
                    user = await _userService.GetOrCreateBackgroundTaskUserAsync();
                }

                if (user == null || user.Deleted || !user.Active || user.RequireReLogin)
                {
                    //check whether request is made by a search engine, in this case return built-in user record for search engines
                    if (_userAgentHelper.IsSearchEngine())
                        user = await _userService.GetOrCreateSearchEngineUserAsync();
                }

                if (user == null || user.Deleted || !user.Active || user.RequireReLogin)
                {
                    //try to get registered user
                    user = await _authenticationService.GetAuthenticatedUserAsync();
                }

                if (user != null && !user.Deleted && user.Active && !user.RequireReLogin)
                {
                    //get impersonate user if required
                    var impersonatedUserId = await _genericAttributeService
                        .GetAttributeAsync<int?>(user, TvProgUserDefaults.ImpersonatedUserIdAttribute);
                    if (impersonatedUserId.HasValue && impersonatedUserId.Value > 0)
                    {
                        var impersonatedUser = await _userService.GetUserByIdAsync(impersonatedUserId.Value);
                        if (impersonatedUser != null && !impersonatedUser.Deleted &&
                            impersonatedUser.Active &&
                            !impersonatedUser.RequireReLogin)
                        {
                            //set impersonated user
                            _originalUserIfImpersonated = user;
                            user = impersonatedUser;
                        }
                    }
                }

                if (user == null || user.Deleted || !user.Active || user.RequireReLogin)
                {
                    //get guest user
                    var userCookie = GetUserCookie();
                    if (Guid.TryParse(userCookie, out var userGuid))
                    {
                        //get user from cookie (should not be registered)
                        var userByCookie = await _userService.GetUserByGuidAsync(userGuid);
                        if (userByCookie != null && !await _userService.IsRegisteredAsync(userByCookie))
                            user = userByCookie;
                    }
                }

                if (user == null || user.Deleted || !user.Active || user.RequireReLogin)
                {
                    //create guest if not exists
                    user = await _userService.InsertGuestUserAsync();
                }
            }

            if (!user.Deleted && user.Active && !user.RequireReLogin)
            {
                //set user cookie
                SetUserCookie(user.UserGuid);

                //cache the found user
                _cachedUser = user;
            }
        }

        /// <summary>
        /// Gets the original User (in case the current one is impersonated)
        /// </summary>
        public virtual User OriginalUserIfImpersonated => _originalUserIfImpersonated;


        /// <summary>
        /// Gets the current vendor (logged-in manager)
        /// </summary>
        public virtual async Task<Vendor> GetCurrentVendorAsync()
        {
            //whether there is a cached value
            if (_cachedVendor != null)
                return _cachedVendor;
            var user = await GetCurrentUserAsync();
            if (user == null)
                return null;


            var vendor = await _vendorService.GetVendorByIdAsync(user.VendorId);
            if (vendor == null || vendor.Deleted || !vendor.Active)
                return null;

            _cachedVendor = vendor;
            return _cachedVendor;
        }

        /// <summary>
        /// Gets current user working language
        /// </summary>
        public virtual async Task<Language> GetWorkingLanguageAsync()
        {
            //whether there is a cached value
            if (_cachedLanguage != null)
                return _cachedLanguage;

            var user = await GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();

            Language detectedLanguage = null;

            //localized URLs are enabled, so try to get language from the requested page URL
            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                detectedLanguage = await GetLanguageFromUrlAsync();

            //whether we should detect the language from the request
            if (detectedLanguage == null && _localizationSettings.AutomaticallyDetectLanguage)
            {
                //whether language already detected by this way
                var alreadyDetected = await _genericAttributeService
                    .GetAttributeAsync<bool>(user, TvProgUserDefaults.LanguageAutomaticallyDetectedAttribute, store.Id);

                //if not, try to get language from the request
                if (!alreadyDetected)
                {
                    detectedLanguage = await GetLanguageFromRequestAsync();
                    if (detectedLanguage != null)
                    {
                        //language already detected
                        await _genericAttributeService
                            .SaveAttributeAsync(user, TvProgUserDefaults.LanguageAutomaticallyDetectedAttribute, true, store.Id);
                    }
                }
            }

            //if the language is detected we need to save it
            if (detectedLanguage != null)
            {
                //get current saved language identifier
                var currentLanguageId = await _genericAttributeService
                    .GetAttributeAsync<int>(user, TvProgUserDefaults.LanguageIdAttribute, store.Id);

                //save the detected language identifier if it differs from the current one
                if (detectedLanguage.Id != currentLanguageId)
                {
                    await _genericAttributeService
                        .SaveAttributeAsync(user, TvProgUserDefaults.LanguageIdAttribute, detectedLanguage.Id, store.Id);
                }
            }

            //get current user language identifier
            var userLanguageId = await _genericAttributeService
                .GetAttributeAsync<int>(user, TvProgUserDefaults.LanguageIdAttribute, store.Id);

            var allStoreLanguages = await _languageService.GetAllLanguagesAsync(storeId: store.Id);

            //check user language availability
            var userLanguage = allStoreLanguages.FirstOrDefault(language => language.Id == userLanguageId);
            if (userLanguage == null)
            {
                //it not found, then try to get the default language for the current store (if specified)
                userLanguage = allStoreLanguages.FirstOrDefault(language => language.Id == store.DefaultLanguageId);
            }

            //if the default language for the current store not found, then try to get the first one
            if (userLanguage == null)
                userLanguage = allStoreLanguages.FirstOrDefault();

            //if there are no languages for the current store try to get the first one regardless of the store
            if (userLanguage == null)
                userLanguage = (await _languageService.GetAllLanguagesAsync()).FirstOrDefault();

            //cache the found language
            _cachedLanguage = userLanguage;

            return _cachedLanguage;
        }

        /// <summary>
        /// Sets current user working language
        /// </summary>
        /// <param name="language">Language</param>
        public virtual async Task SetWorkingLanguageAsync(Language language)
        {
            //save passed language identifier
            var user = await GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            await _genericAttributeService.SaveAttributeAsync(user, TvProgUserDefaults.LanguageIdAttribute, language?.Id ?? 0, store.Id);

            //then reset the cached value
            _cachedLanguage = null;
        }

        /// <summary>
        /// Gets current user working currency
        /// </summary>
        public virtual async Task<Currency> GetWorkingCurrencyAsync()
        {
            //whether there is a cached value
            if (_cachedCurrency != null)
                return _cachedCurrency;

            var adminAreaUrl = $"{_webHelper.GetStoreLocation()}admin";

            //return primary store currency when we're in admin area/mode
            if (_webHelper.GetThisPageUrl(false).StartsWith(adminAreaUrl, StringComparison.InvariantCultureIgnoreCase))
            {
                var primaryStoreCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
                if (primaryStoreCurrency != null)
                {
                    _cachedCurrency = primaryStoreCurrency;
                    return primaryStoreCurrency;
                }
            }

            var user = await GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();

            //find a currency previously selected by a user
            var userCurrencyId = await _genericAttributeService
                .GetAttributeAsync<int>(user, TvProgUserDefaults.CurrencyIdAttribute, store.Id);

            var allStoreCurrencies = await _currencyService.GetAllCurrenciesAsync(storeId: store.Id);

            //check user currency availability
            var userCurrency = allStoreCurrencies.FirstOrDefault(currency => currency.Id == userCurrencyId);
            if (userCurrency == null)
            {
                //it not found, then try to get the default currency for the current language (if specified)
                var language = await GetWorkingLanguageAsync();
                userCurrency = allStoreCurrencies
                    .FirstOrDefault(currency => currency.Id == language.DefaultCurrencyId);
            }

            //if the default currency for the current store not found, then try to get the first one
            if (userCurrency == null)
                userCurrency = allStoreCurrencies.FirstOrDefault();

            //if there are no currencies for the current store try to get the first one regardless of the store
            if (userCurrency == null)
                userCurrency = (await _currencyService.GetAllCurrenciesAsync()).FirstOrDefault();

            //cache the found currency
            _cachedCurrency = userCurrency;

            return _cachedCurrency;
        }

        /// <summary>
        /// Sets current user working currency
        /// </summary>
        /// <param name="currency">Currency</param>
        public virtual async Task SetWorkingCurrencyAsync(Currency currency)
        {
            //save passed currency identifier
            var user = await GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            await _genericAttributeService.SaveAttributeAsync(user, TvProgUserDefaults.CurrencyIdAttribute, currency?.Id ?? 0, store.Id);

            //then reset the cached value
            _cachedCurrency = null;
        }

        /// <summary>
        /// Получение и установка ТВ-провайдера текущего пользователя
        /// </summary>
        public virtual async Task<TvProgProviders> GetWorkingProviderAsync()
        {
            // Убедиться, есть ли закешированное значение:
            if (_cachedProvider != null)
                return _cachedProvider;
            var adminAreaUrl = $"{_webHelper.GetStoreLocation()}admin";
            // Первичное значение провайдера, когда мы под пространством/режимом админа
            if (_webHelper.GetThisPageUrl(false).StartsWith(adminAreaUrl, StringComparison.InvariantCultureIgnoreCase))
            {
                var primarySystemProvider = await _programmeService.GetProviderByIdAsync(_programmeSettings.PrimarySystemProviderId);
                if (primarySystemProvider != null)
                {
                    _cachedProvider = primarySystemProvider;
                    return primarySystemProvider;
                }
            };
            var user = await GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();

            // Поиск ТВ-провайдера первоначально выбранного пользователем
            var userProviderId = await _genericAttributeService.GetAttributeAsync<int>(user,
                TvProgUserDefaults.ProviderIdAttribute, store.Id);

            var allStoreProviders = await _programmeService.GetAllProvidersAsync();

            // Проверка доступности ТВ-провайдера пользователю:
            var userProvider = allStoreProviders.FirstOrDefault(provider => provider.Id == userProviderId);
            if (userProvider == null)
            {
                // Если он не найден, тогда попытаться получить ТВ-провайдера по умолчанию:
                userProvider = allStoreProviders.FirstOrDefault(provider => provider.Id == _programmeSettings.PrimarySystemProviderId);
            }

            // Если по умолчанию ТВ-провайдер Не найден, тогда попытаться получить первый попавшийся:
            if (userProvider == null)
            {
                var providers = await _programmeService.GetAllProvidersAsync();
                userProvider = providers.FirstOrDefault();
            }

            // Кэширование найденного ТВ-провайдера:
            _cachedProvider = userProvider;

            return _cachedProvider;
        }
        public virtual async Task SetWorkingProviderAsync(TvProgProviders provider)
        {
            //save passed currency identifier
            var user = await GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            await _genericAttributeService.SaveAttributeAsync(user, TvProgUserDefaults.ProviderIdAttribute, provider?.Id ?? 0, store.Id);

            // Тогда сбросим закэшированное значение
            _cachedProvider = null;
        }
        /// <summary>
        /// Получение или установка типа ТВ-программы текущего пользователя
        /// </summary>
        public virtual async Task<TypeProg> GetWorkingTypeProgAsync()
        {
            // Убедиться, есть ли закешированное значение
            if (_cachedTypeProg != null)
                return _cachedTypeProg;
            var adminAreaUrl = $"{_webHelper.GetStoreLocation()}admin";

            // Первичное значение типа, когда мы под пространством/режимом админа
            if (_webHelper.GetThisPageUrl(false).StartsWith(adminAreaUrl, StringComparison.InvariantCultureIgnoreCase))
            {
                var primarySystemTypeProg = await _programmeService.GetTypeProgByIdAsync(_programmeSettings.PrimarySystemTypeProgId);
                if (primarySystemTypeProg != null)
                {
                    _cachedTypeProg = primarySystemTypeProg;
                    return primarySystemTypeProg;
                }
            }

            var user = await GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();

            // Поиск типа ТВ-программы, первоначально выбранного пользователем
            var userTypeProgId = await _genericAttributeService.GetAttributeAsync<int>(user, TvProgUserDefaults.TypeProgIdAttribute, store.Id);

            var allStoreTypeProgs = await _programmeService.GetAllTypeProgsAsync(false);

            // Проверка доступности типа ТВ-программы пользователю:
            var userTypeProg = allStoreTypeProgs.FirstOrDefault(typeProg => typeProg.Id == userTypeProgId);
            if (userTypeProg == null)
            {
                // Если он не найден, тогда попытаться получить тип ТВ-программы по умолчанию:
                userTypeProg = allStoreTypeProgs.FirstOrDefault(typeProg => typeProg.Id == _programmeSettings.PrimarySystemTypeProgId);
            }

            // Если по умолчанию тип ТВ-программы не найден, тогда попытаться получить первый попавшийся:
            if (userTypeProg == null)
            {
                var typeProgs = await _programmeService.GetAllTypeProgsAsync(false);
                userTypeProg = typeProgs.FirstOrDefault();
            }

            // Кэширование найденного типа ТВ-программы:
            _cachedTypeProg = userTypeProg;

            return _cachedTypeProg;
        }



        public virtual async Task SetWorkingTypeProgAsync(TypeProg typeProg)
        {
            //save passed currency identifier
            var user = await GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            await _genericAttributeService.SaveAttributeAsync(user, TvProgUserDefaults.ProviderIdAttribute, typeProg?.Id ?? 0, store.Id);

            // Тогда сбросим закэшированное значение
            _cachedTypeProg = null;
        }


        /// <summary>
        /// Получение или установка пользовательской категории ТВ-программы
        /// </summary>
        public virtual async Task<string> GetWorkingCategoryAsync()
        {
            // Убедиться, есть ли закешированное значение
            if (_cachedCategory != null)
                return _cachedCategory;
            var adminAreaUrl = $"{_webHelper.GetStoreLocation()}admin";

            // Первичное значение типа, когда мы под пространством/режимом админа
            if (_webHelper.GetThisPageUrl(false).StartsWith(adminAreaUrl, StringComparison.InvariantCultureIgnoreCase))
            {
                var primarySystemCategory = _programmeSettings.PrimarySystemCategory;
                if (primarySystemCategory != null)
                {
                    _cachedCategory = primarySystemCategory;
                    return primarySystemCategory;
                }
            }

            var user = await GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();

            // Поиск категории ТВ-программы, первоначально выбранного пользователем
            var userCategoryName = await _genericAttributeService.GetAttributeAsync<string>(user,
                    TvProgUserDefaults.CategoryAttribute, store.Id);

            var allStoreCategory = await _programmeService.GetCategoriesAsync();

            // Проверка доступности категории ТВ-программы пользователю:
            var userCategory = allStoreCategory.FirstOrDefault(category => category == userCategoryName);
            if (userCategory == null)
            {
                // Если он не найден, тогда попытаться получить категорию ТВ-программы по умолчанию:
                userCategory = allStoreCategory.FirstOrDefault(category => category == _programmeSettings.PrimarySystemCategory);
            }

            // Если по умолчанию категория ТВ-программы не найдена, тогда попытаться получить первую попавшуюся:
            if (userCategory == null)
            {
                var categories = await _programmeService.GetCategoriesAsync();
                userCategory = categories.FirstOrDefault();
            }

            // Кэширование найденной категории ТВ-программы:
            _cachedCategory = userCategory;

            return _cachedCategory;
        }

        public virtual async Task SetWorkingCategoryAsync(string category)
        {
            // Получение категории ТВ-программы
            var currCategory = category ?? "Все категории";
            var user = await GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            // И её сохранение
            await _genericAttributeService.SaveAttributeAsync(user, TvProgUserDefaults.CategoryAttribute, currCategory, store.Id);

            // Тогда сбросим закэшированное значение
            _cachedCategory = null;
        }
        /// <summary>
        /// Gets or sets current tax display type
        /// </summary>
        public virtual async Task<TaxDisplayType> GetTaxDisplayTypeAsync()
        {
            //whether there is a cached value
            if (_cachedTaxDisplayType.HasValue)
                return _cachedTaxDisplayType.Value;

            var taxDisplayType = TaxDisplayType.IncludingTax;
            var user = await GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();

            //whether users are allowed to select tax display type
            if (_taxSettings.AllowUsersToSelectTaxDisplayType && user != null)
            {
                //try to get previously saved tax display type
                var taxDisplayTypeId = await _genericAttributeService
                    .GetAttributeAsync<int?>(user, TvProgUserDefaults.TaxDisplayTypeIdAttribute, store.Id);
                if (taxDisplayTypeId.HasValue)
                    taxDisplayType = (TaxDisplayType)taxDisplayTypeId.Value;
                else
                {
                    //default tax type by user roles
                    var defaultRoleTaxDisplayType = await _userService.GetUserDefaultTaxDisplayTypeAsync(user);
                    if (defaultRoleTaxDisplayType != null)
                        taxDisplayType = defaultRoleTaxDisplayType.Value;
                }
            }
            else
            {
                //default tax type by user roles
                var defaultRoleTaxDisplayType = await _userService.GetUserDefaultTaxDisplayTypeAsync(user);
                if (defaultRoleTaxDisplayType != null)
                    taxDisplayType = defaultRoleTaxDisplayType.Value;
                else
                {
                    //or get the default tax display type
                    taxDisplayType = _taxSettings.TaxDisplayType;
                }
            }

            //cache the value
            _cachedTaxDisplayType = taxDisplayType;

            return _cachedTaxDisplayType.Value;
        }

        public virtual async Task SetTaxDisplayTypeAsync(TaxDisplayType taxDisplayType)
        {
            //whether users are allowed to select tax display type
            if (!_taxSettings.AllowUsersToSelectTaxDisplayType)
                return;

            //save passed value
            var user = await GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            await _genericAttributeService
                .SaveAttributeAsync(user, TvProgUserDefaults.TaxDisplayTypeIdAttribute, (int)taxDisplayType, store.Id);

            //then reset the cached value
            _cachedTaxDisplayType = null;
        }

        /// <summary>
        /// Gets or sets value indicating whether we're in admin area
        /// </summary>
        public virtual bool IsAdmin { get; set; }

        #endregion
    }
}