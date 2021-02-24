﻿using FluentMigrator;
using TVProgViewer.Core.Domain.Stores;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2020/03/13 11:35:09:1647930")]
    public class AddStoreMappingEntityIdEntityNameIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.Index("IX_StoreMapping_EntityId_EntityName").OnTable(nameof(StoreMapping))
                .OnColumn(nameof(StoreMapping.EntityId)).Ascending()
                .OnColumn(nameof(StoreMapping.EntityName)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}