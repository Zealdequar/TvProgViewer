﻿using System.Threading.Tasks;
using TVProgViewer.Services.Plugins;

namespace TVProgViewer.Services.Tax
{
    /// <summary>
    /// Provides an interface for creating tax providers
    /// </summary>
    public partial interface ITaxProvider : IPlugin
    {
        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="taxRateRequest">Tax rate request</param>
        /// <returns>Tax</returns>
        Task<TaxRateResult> GetTaxRateAsync(TaxRateRequest taxRateRequest);

        /// <summary>
        /// Gets tax total
        /// </summary>
        /// <param name="taxTotalRequest">Tax total request</param>
        /// <returns>Tax total</returns>
        Task<TaxTotalResult> GetTaxTotalAsync(TaxTotalRequest taxTotalRequest);
    }
}
