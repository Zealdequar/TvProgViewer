﻿using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Forums;

namespace TVProgViewer.Data.Mapping.Builders.Forums
{
    /// <summary>
    /// Represents a forum group entity builder
    /// </summary>
    public partial class ForumGroupBuilder : TvProgEntityBuilder<ForumGroup>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(ForumGroup.Name)).AsString(200).NotNullable();
        }

        #endregion
    }
}