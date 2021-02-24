﻿using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Directory;

namespace TVProgViewer.Data.Mapping.Builders.Directory
{
    /// <summary>
    /// Represents a currency entity builder
    /// </summary>
    public partial class CurrencyBuilder : TvProgEntityBuilder<Currency>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Currency.Name)).AsString(50).NotNullable()
                .WithColumn(nameof(Currency.CurrencyCode)).AsString(5).NotNullable()
                .WithColumn(nameof(Currency.DisplayLocale)).AsString(50).Nullable()
                .WithColumn(nameof(Currency.CustomFormatting)).AsString(50).Nullable();
        }

        #endregion
    }
}