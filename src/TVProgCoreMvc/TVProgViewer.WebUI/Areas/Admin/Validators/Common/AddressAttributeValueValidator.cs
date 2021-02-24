﻿using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Common;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Common
{
    public partial class AddressAttributeValueValidator : BaseTvProgValidator<AddressAttributeValueModel>
    {
        public AddressAttributeValueValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.AddressAttributes.Values.Fields.Name.Required"));

            SetDatabaseValidationRules<AddressAttributeValue>(dataProvider);
        }
    }
}