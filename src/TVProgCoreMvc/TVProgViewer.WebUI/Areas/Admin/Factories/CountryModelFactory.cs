﻿using System;
using System.Linq;
using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Services.Directory;
using TVProgViewer.Services.Localization;
using TVProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TVProgViewer.WebUI.Areas.Admin.Models.Directory;
using TVProgViewer.Web.Framework.Factories;
using TVProgViewer.Web.Framework.Models.Extensions;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the country model factory implementation
    /// </summary>
    public partial class CountryModelFactory : ICountryModelFactory
    {
        #region Fields

        private readonly ICountryService _countryService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly IStateProvinceService _stateProvinceService;

        #endregion

        #region Ctor

        public CountryModelFactory(ICountryService countryService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IStateProvinceService stateProvinceService)
        {
            _countryService = countryService;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            _stateProvinceService = stateProvinceService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare state and province search model
        /// </summary>
        /// <param name="searchModel">State and province search model</param>
        /// <param name="country">Country</param>
        /// <returns>State and province search model</returns>
        protected virtual StateProvinceSearchModel PrepareStateProvinceSearchModel(StateProvinceSearchModel searchModel, Country country)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (country == null)
                throw new ArgumentNullException(nameof(country));

            searchModel.CountryId = country.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare country search model
        /// </summary>
        /// <param name="searchModel">Country search model</param>
        /// <returns>Country search model</returns>
        public virtual Task<CountrySearchModel> PrepareCountrySearchModelAsync(CountrySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged country list model
        /// </summary>
        /// <param name="searchModel">Country search model</param>
        /// <returns>Country list model</returns>
        public virtual async Task<CountryListModel> PrepareCountryListModelAsync(CountrySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get countries
            var countries = (await _countryService.GetAllCountriesAsync(showHidden: true)).ToPagedList(searchModel);

            //prepare list model
            var model = await new CountryListModel().PrepareToGridAsync(searchModel, countries, () =>
            {
                //fill in model values from the entity
                return countries.SelectAwait(async country =>
                {
                    var countryModel = country.ToModel<CountryModel>();

                    countryModel.NumberOfStates = (await _stateProvinceService.GetStateProvincesByCountryIdAsync(country.Id))?.Count ?? 0;

                    return countryModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare country model
        /// </summary>
        /// <param name="model">Country model</param>
        /// <param name="country">Country</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Country model</returns>
        public virtual async Task<CountryModel> PrepareCountryModelAsync(CountryModel model, Country country, bool excludeProperties = false)
        {
            Action<CountryLocalizedModel, int> localizedModelConfiguration = null;

            if (country != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = country.ToModel<CountryModel>();
                    model.NumberOfStates = (await _stateProvinceService.GetStateProvincesByCountryIdAsync(country.Id))?.Count ?? 0;
                }

                //prepare nested search model
                PrepareStateProvinceSearchModel(model.StateProvinceSearchModel, country);

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(country, entity => entity.Name, languageId, false, false);
                };
            }

            //set default values for the new model
            if (country == null)
            {
                model.Published = true;
                model.AllowsBilling = true;
                model.AllowsShipping = true;
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            //prepare available stores
            await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, country, excludeProperties);

            return model;
        }

        /// <summary>
        /// Prepare paged state and province list model
        /// </summary>
        /// <param name="searchModel">State and province search model</param>
        /// <param name="country">Country</param>
        /// <returns>State and province list model</returns>
        public virtual async Task<StateProvinceListModel> PrepareStateProvinceListModelAsync(StateProvinceSearchModel searchModel, Country country)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (country == null)
                throw new ArgumentNullException(nameof(country));

            //get comments
            var states = (await _stateProvinceService.GetStateProvincesByCountryIdAsync(country.Id, showHidden: true)).ToPagedList(searchModel);

            //prepare list model
            var model = new StateProvinceListModel().PrepareToGrid(searchModel, states, () =>
            {
                //fill in model values from the entity
                return states.Select(state => state.ToModel<StateProvinceModel>());
            });

            return model;
        }

        /// <summary>
        /// Prepare state and province model
        /// </summary>
        /// <param name="model">State and province model</param>
        /// <param name="country">Country</param>
        /// <param name="state">State or province</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>State and province model</returns>
        public virtual async Task<StateProvinceModel> PrepareStateProvinceModelAsync(StateProvinceModel model,
            Country country, StateProvince state, bool excludeProperties = false)
        {
            Action<StateProvinceLocalizedModel, int> localizedModelConfiguration = null;

            if (state != null)
            {
                //fill in model values from the entity
                model ??= state.ToModel<StateProvinceModel>();

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(state, entity => entity.Name, languageId, false, false);
                };
            }

            model.CountryId = country.Id;

            //set default values for the new model
            if (state == null)
                model.Published = true;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        #endregion
    }
}