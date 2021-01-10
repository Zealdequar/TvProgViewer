﻿using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product specification attribute list model
    /// </summary>
    public partial record ProductSpecificationAttributeListModel : BasePagedListModel<ProductSpecificationAttributeModel>
    {
    }
}