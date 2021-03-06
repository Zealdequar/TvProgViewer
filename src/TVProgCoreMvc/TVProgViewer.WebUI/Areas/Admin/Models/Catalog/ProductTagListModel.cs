﻿using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product tag list model
    /// </summary>
    public partial record ProductTagListModel : BasePagedListModel<ProductTagModel>
    {
    }
}