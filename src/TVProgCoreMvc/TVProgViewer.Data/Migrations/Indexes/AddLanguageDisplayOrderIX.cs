﻿using FluentMigrator;
using TVProgViewer.Core.Domain.Localization;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2020/03/13 09:36:08:9037689")]
    public class AddLanguageDisplayOrderIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.Index("IX_Language_DisplayOrder").OnTable(nameof(Language))
                .OnColumn(nameof(Language.DisplayOrder)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}