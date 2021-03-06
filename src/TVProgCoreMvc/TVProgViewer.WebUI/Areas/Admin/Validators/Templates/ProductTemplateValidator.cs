﻿using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Templates;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.Data;
using TVProgViewer.Core.Domain.Catalog;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Templates
{
    public partial class ProductTemplateValidator : BaseTvProgValidator<ProductTemplateModel>
    {
        public ProductTemplateValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.Templates.Product.Name.Required"));
            RuleFor(x => x.ViewPath).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.Templates.Product.ViewPath.Required"));

            SetDatabaseValidationRules<ProductTemplate>(dataProvider);
        }
    }
}