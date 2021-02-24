﻿using TVProgViewer.Web.Framework.Mvc.ModelBinding;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a sort option model
    /// </summary>
    public partial record SortOptionModel : BaseTvProgEntityModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.SortOptions.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.SortOptions.IsActive")]
        public bool IsActive { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Catalog.SortOptions.DisplayOrder")]
        public int DisplayOrder { get; set; }

        #endregion
    }
}