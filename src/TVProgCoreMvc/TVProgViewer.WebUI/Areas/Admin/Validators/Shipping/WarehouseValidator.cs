﻿using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Shipping;
using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Shipping
{
    public partial class WarehouseValidator : BaseTvProgValidator<WarehouseModel>
    {
        public WarehouseValidator(IDataProvider dataProvider, ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Shipping.Warehouses.Fields.Name.Required"));

            SetDatabaseValidationRules<Warehouse>(dataProvider);
        }
    }
}