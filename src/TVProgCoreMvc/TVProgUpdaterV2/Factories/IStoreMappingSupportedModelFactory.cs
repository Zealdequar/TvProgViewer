﻿using System.Threading.Tasks;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Stores;
using TVProgViewer.TVProgUpdaterV2.Models;

namespace TVProgViewer.TVProgUpdaterV2.Factories
{
    /// <summary>
    /// Represents the store mapping supported model factory
    /// </summary>
    public partial interface IStoreMappingSupportedModelFactory
    {
        /// <summary>
        /// Prepare selected and all available stores for the passed model
        /// </summary>
        /// <typeparam name="TModel">Store mapping supported model type</typeparam>
        /// <param name="model">Model</param>
        Task PrepareModelStoresAsync<TModel>(TModel model) where TModel : IStoreMappingSupportedModel;

        /// <summary>
        /// Prepare selected and all available stores for the passed model by store mappings
        /// </summary>
        /// <typeparam name="TModel">Store mapping supported model type</typeparam>
        /// <typeparam name="TEntity">Store mapping supported entity type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="entity">Entity</param>
        /// <param name="ignoreStoreMappings">Whether to ignore existing store mappings</param>
        Task PrepareModelStoresAsync<TModel, TEntity>(TModel model, TEntity entity, bool ignoreStoreMappings)
            where TModel : IStoreMappingSupportedModel where TEntity : BaseEntity, IStoreMappingSupported;
    }
}