﻿using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Catalog;

namespace TVProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a category entity builder
    /// </summary>
    public partial class CategoryBuilder : TvProgEntityBuilder<Category>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Category.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(Category.MetaKeywords)).AsString(400).Nullable()
                .WithColumn(nameof(Category.MetaTitle)).AsString(400).Nullable()
                .WithColumn(nameof(Category.PriceRanges)).AsString(400).Nullable()
                .WithColumn(nameof(Category.PageSizeOptions)).AsString(200).Nullable();
        }

        #endregion
    }
}