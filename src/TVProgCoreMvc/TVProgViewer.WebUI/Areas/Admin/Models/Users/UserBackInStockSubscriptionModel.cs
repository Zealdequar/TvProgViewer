﻿using System;
using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user back in stock subscription model
    /// </summary>
    public partial record UserBackInStockSubscriptionModel : BaseTvProgEntityModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Users.Users.BackInStockSubscriptions.Store")]
        public string StoreName { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.BackInStockSubscriptions.Product")]
        public int ProductId { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.BackInStockSubscriptions.Product")]
        public string ProductName { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.BackInStockSubscriptions.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        #endregion
    }
}