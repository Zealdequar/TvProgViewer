﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user search model
    /// </summary>
    public partial record UserSearchModel : BaseSearchModel, IAclSupportedModel
    {
        #region Ctor

        public UserSearchModel()
        {
            SelecteduserRoleIds = new List<int>();
            AvailableUserRoles = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.Users.Users.List.UserRoles")]
        public IList<int> SelecteduserRoleIds { get; set; }
        public IList<int> SelectedUserRoleIds { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public IList<SelectListItem> AvailableUserRoles { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.List.SearchEmail")]
        public string SearchEmail { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.List.SearchUsername")]
        public string SearchUsername { get; set; }

        public bool UsernamesEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.List.SearchFirstName")]
        public string SearchFirstName { get; set; }
        public bool FirstNameEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.List.SearchLastName")]
        public string SearchLastName { get; set; }
        public bool LastNameEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.List.SearchDateOfBirth")]
        public string SearchDayOfBirth { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.List.SearchDateOfBirth")]
        public string SearchMonthOfBirth { get; set; }

        public bool DateOfBirthEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.List.SearchCompany")]
        public string SearchCompany { get; set; }

        public bool CompanyEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.List.SearchPhone")]
        public string SearchPhone { get; set; }

        public bool PhoneEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.List.SearchZipCode")]
        public string SearchZipPostalCode { get; set; }

        public bool ZipPostalCodeEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.List.SearchIpAddress")]
        public string SearchIpAddress { get; set; }

        public bool AvatarEnabled { get; internal set; }

        #endregion
    }
}