﻿using System;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents an online user model
    /// </summary>
    public partial record OnlineUserModel : BaseTvProgEntityModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Users.OnlineUsers.Fields.UserInfo")]
        public string UserInfo { get; set; }

        [TvProgResourceDisplayName("Admin.Users.OnlineUsers.Fields.IPAddress")]
        public string LastIpAddress { get; set; }

        [TvProgResourceDisplayName("Admin.Users.OnlineUsers.Fields.Location")]
        public string Location { get; set; }

        [TvProgResourceDisplayName("Admin.Users.OnlineUsers.Fields.LastActivityDate")]
        public DateTime LastActivityDate { get; set; }
        
        [TvProgResourceDisplayName("Admin.Users.OnlineUsers.Fields.LastVisitedPage")]
        public string LastVisitedPage { get; set; }

        #endregion
    }
}