﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TVProgViewer.TVProgUpdaterV2.Models;

namespace TVProgViewer.TVProgUpdaterV2.Factories
{
    /// <summary>
    /// Represents the localized model factory
    /// </summary>
    public partial interface ILocalizedModelFactory
    {
        /// <summary>
        /// Prepare localized model for localizable entities
        /// </summary>
        /// <typeparam name="T">Localized model type</typeparam>
        /// <param name="configure">Model configuration action</param>
        /// <returns>List of localized model</returns>
        Task<IList<T>> PrepareLocalizedModelsAsync<T>(Action<T, int> configure = null) where T : ILocalizedLocaleModel;
    }
}