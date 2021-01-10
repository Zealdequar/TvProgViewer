﻿using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents an incomplete order report list model
    /// </summary>
    public partial record OrderIncompleteReportListModel : BasePagedListModel<OrderIncompleteReportModel>
    {
    }
}