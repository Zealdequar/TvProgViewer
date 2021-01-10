﻿using System;
using System.Linq;
using FluentValidation;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Services.Directory;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.WebUI.Models.User;

namespace TVProgViewer.WebUI.Validators.User
{
    public partial class RegisterValidator : BaseTvProgValidator<RegisterModel>
    {
        public RegisterValidator(ILocalizationService localizationService, 
            IStateProvinceService stateProvinceService,
            UserSettings userSettings)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
            
            if (userSettings.EnteringEmailTwice)
            {
                RuleFor(x => x.ConfirmEmail).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.ConfirmEmail.Required"));
                RuleFor(x => x.ConfirmEmail).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
                RuleFor(x => x.ConfirmEmail).Equal(x => x.Email).WithMessage(localizationService.GetResource("Account.Fields.Email.EnteredEmailsDoNotMatch"));
            }
            
            if (userSettings.UsernamesEnabled)
            {
                RuleFor(x => x.Username).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.Username.Required"));
                RuleFor(x => x.Username).IsUsername(userSettings).WithMessage(localizationService.GetResource("Account.Fields.Username.NotValid"));
            }
            
            if (userSettings.FirstNameEnabled && userSettings.FirstNameRequired)
            {
                RuleFor(x => x.FirstName).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.FirstName.Required"));
            }
            if (userSettings.LastNameEnabled && userSettings.LastNameRequired)
            {
                RuleFor(x => x.LastName).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.LastName.Required"));
            }

            //Password rule
            RuleFor(x => x.Password).IsPassword(localizationService, userSettings);           

            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.ConfirmPassword.Required"));
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage(localizationService.GetResource("Account.Fields.Password.EnteredPasswordsDoNotMatch"));

            //form fields
            if (userSettings.CountryEnabled && userSettings.CountryRequired)
            {
                RuleFor(x => x.CountryId)
                    .NotEqual(0)
                    .WithMessage(localizationService.GetResource("Account.Fields.Country.Required"));
            }
            if (userSettings.CountryEnabled &&
                userSettings.StateProvinceEnabled &&
                userSettings.StateProvinceRequired)
            {
                RuleFor(x => x.StateProvinceId).Must((x, context) =>
                {
                    //does selected country have states?
                    var hasStates = stateProvinceService.GetStateProvincesByCountryId(x.CountryId).Any();
                    if (hasStates)
                    {
                        //if yes, then ensure that a state is selected
                        if (x.StateProvinceId == 0)
                            return false;
                    }

                    return true;
                }).WithMessage(localizationService.GetResource("Account.Fields.StateProvince.Required"));
            }
            if (userSettings.DateOfBirthEnabled && userSettings.DateOfBirthRequired)
            {
                //entered?
                RuleFor(x => x.DateOfBirthDay).Must((x, context) =>
                {
                    var dateOfBirth = x.ParseDateOfBirth();
                    if (!dateOfBirth.HasValue)
                        return false;

                    return true;
                }).WithMessage(localizationService.GetResource("Account.Fields.DateOfBirth.Required"));

                //minimum age
                RuleFor(x => x.DateOfBirthDay).Must((x, context) =>
                {
                    var dateOfBirth = x.ParseDateOfBirth();
                    if (dateOfBirth.HasValue && userSettings.DateOfBirthMinimumAge.HasValue &&
                        CommonHelper.GetDifferenceInYears(dateOfBirth.Value, DateTime.Today) <
                        userSettings.DateOfBirthMinimumAge.Value)
                        return false;

                    return true;
                }).WithMessage(string.Format(localizationService.GetResource("Account.Fields.DateOfBirth.MinimumAge"), userSettings.DateOfBirthMinimumAge));
            }
            if (userSettings.CompanyRequired && userSettings.CompanyEnabled)
            {
                RuleFor(x => x.Company).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.Company.Required"));
            }
            if (userSettings.StreetAddressRequired && userSettings.StreetAddressEnabled)
            {
                RuleFor(x => x.StreetAddress).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.StreetAddress.Required"));
            }
            if (userSettings.StreetAddress2Required && userSettings.StreetAddress2Enabled)
            {
                RuleFor(x => x.StreetAddress2).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.StreetAddress2.Required"));
            }
            if (userSettings.ZipPostalCodeRequired && userSettings.ZipPostalCodeEnabled)
            {
                RuleFor(x => x.ZipPostalCode).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.ZipPostalCode.Required"));
            }
            if (userSettings.CountyRequired && userSettings.CountyEnabled)
            {
                RuleFor(x => x.County).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.County.Required"));
            }
            if (userSettings.CityRequired && userSettings.CityEnabled)
            {
                RuleFor(x => x.City).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.City.Required"));
            }
            if (userSettings.PhoneRequired && userSettings.PhoneEnabled)
            {
                RuleFor(x => x.Phone).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.Phone.Required"));
            }
            if (userSettings.FaxRequired && userSettings.FaxEnabled)
            {
                RuleFor(x => x.Fax).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.Fax.Required"));
            }
        }
    }
}