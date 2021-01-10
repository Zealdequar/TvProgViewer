﻿namespace TVProgViewer.Core.Domain.Security
{
    /// <summary>
    /// Represents a permission record-User role mapping class
    /// </summary>
    public partial class PermissionRecordUserRoleMapping : BaseEntity
    {
        /// <summary>
        /// Gets or sets the permission record identifier
        /// </summary>
        public int PermissionRecordId { get; set; }

        /// <summary>
        /// Gets or sets the User role identifier
        /// </summary>
        public int UserRoleId { get; set; }
    }
}