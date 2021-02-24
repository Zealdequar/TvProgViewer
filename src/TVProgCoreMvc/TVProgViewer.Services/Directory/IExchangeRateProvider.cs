﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Services.Plugins;

namespace TVProgViewer.Services.Directory
{
    /// <summary>
    /// Exchange rate provider interface
    /// </summary>
    public partial interface IExchangeRateProvider : IPlugin
    {
        /// <summary>
        /// Gets currency live rates
        /// </summary>
        /// <param name="exchangeRateCurrencyCode">Exchange rate currency code</param>
        /// <returns>Exchange rates</returns>
        Task<IList<ExchangeRate>> GetCurrencyLiveRatesAsync(string exchangeRateCurrencyCode);
    }
}