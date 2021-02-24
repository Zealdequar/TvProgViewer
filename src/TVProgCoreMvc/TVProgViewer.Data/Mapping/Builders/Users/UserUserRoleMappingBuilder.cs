﻿using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.Users
{
    /// <summary>
    /// Represents a User User role mapping entity builder
    /// </summary>
    public partial class UserUserRoleMappingBuilder : TvProgEntityBuilder<UserUserRoleMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(UserUserRoleMapping), nameof(UserUserRoleMapping.UserId)))
                    .AsInt32().ForeignKey<User>().PrimaryKey()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(UserUserRoleMapping), nameof(UserUserRoleMapping.UserRoleId)))
                    .AsInt32().ForeignKey<UserRole>().PrimaryKey();
        }

        #endregion
    }
}