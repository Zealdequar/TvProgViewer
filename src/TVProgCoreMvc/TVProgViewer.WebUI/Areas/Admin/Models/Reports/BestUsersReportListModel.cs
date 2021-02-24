﻿using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a best users report list model
    /// </summary>
    public partial record BestUsersReportListModel : BasePagedListModel<BestUsersReportModel>
    {
    }
}