﻿using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.Common
{
    /// <summary>
    /// Represents an address attribute value entity builder
    /// </summary>
    public partial class AddressAttributeValueBuilder : TvProgEntityBuilder<AddressAttributeValue>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(AddressAttributeValue.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(AddressAttributeValue.AddressAttributeId)).AsInt32().ForeignKey<AddressAttribute>();
        }

        #endregion
    }
}