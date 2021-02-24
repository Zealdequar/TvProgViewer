﻿using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2020/03/13 09:36:08:9037693")]
    public class AddProductAttributeValueProductAttributeMappingIdDisplayOrderIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.Index("IX_ProductAttributeValue_ProductAttributeMappingId_DisplayOrder").OnTable(nameof(ProductAttributeValue))
                .OnColumn(nameof(ProductAttributeValue.ProductAttributeMappingId)).Ascending()
                .OnColumn(nameof(ProductAttributeValue.DisplayOrder)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}