﻿using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2020/03/13 09:36:08:9037692")]
    public class AddRelatedProductProductId1IX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.Index("IX_RelatedProduct_ProductId1").OnTable(nameof(RelatedProduct))
                .OnColumn(nameof(RelatedProduct.ProductId1)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}