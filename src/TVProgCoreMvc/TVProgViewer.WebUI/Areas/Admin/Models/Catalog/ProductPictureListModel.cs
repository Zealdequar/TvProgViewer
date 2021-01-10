﻿using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product picture list model
    /// </summary>
    public partial record ProductPictureListModel : BasePagedListModel<ProductPictureModel>
    {
    }
}