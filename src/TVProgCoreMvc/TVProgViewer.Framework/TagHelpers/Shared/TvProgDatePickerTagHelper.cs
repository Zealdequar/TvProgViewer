﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Extensions;

namespace TVProgViewer.Web.Framework.TagHelpers.Shared
{
    /// <summary>
    /// TVProgViewer-date-picker tag helper
    /// </summary>
    [HtmlTargetElement("tvprog-date-picker", 
        Attributes = DAY_NAME_ATTRIBUTE_NAME + "," + MONTH_NAME_ATTRIBUTE_NAME + "," + YEAR_NAME_ATTRIBUTE_NAME, 
        TagStructure = TagStructure.WithoutEndTag)]
    public class TvProgDatePickerTagHelper : TagHelper
    {
        #region Constants

        private const string DAY_NAME_ATTRIBUTE_NAME = "asp-day-name";
        private const string MONTH_NAME_ATTRIBUTE_NAME = "asp-month-name";
        private const string YEAR_NAME_ATTRIBUTE_NAME = "asp-year-name";

        private const string BEGIN_YEAR_ATTRIBUTE_NAME = "asp-begin-year";
        private const string END_YEAR_ATTRIBUTE_NAME = "asp-end-year";

        private const string SELECTED_DAY_ATTRIBUTE_NAME = "asp-selected-day";
        private const string SELECTED_MONTH_ATTRIBUTE_NAME = "asp-selected-month";
        private const string SELECTED_YEAR_ATTRIBUTE_NAME = "asp-selected-year";

        private const string WRAP_TAGS_ATTRIBUTE_NAME = "asp-wrap-tags";

        #endregion

        #region Properties

        protected IHtmlGenerator Generator { get; set; }

        /// <summary>
        /// Day name
        /// </summary>
        [HtmlAttributeName(DAY_NAME_ATTRIBUTE_NAME)]
        public string DayName { get; set; }

        /// <summary>
        /// Month name
        /// </summary>
        [HtmlAttributeName(MONTH_NAME_ATTRIBUTE_NAME)]
        public string MonthName { get; set; }

        /// <summary>
        /// Year name
        /// </summary>
        [HtmlAttributeName(YEAR_NAME_ATTRIBUTE_NAME)]
        public string YearName { get; set; }

        /// <summary>
        /// Begin year
        /// </summary>
        [HtmlAttributeName(BEGIN_YEAR_ATTRIBUTE_NAME)]
        public int? BeginYear { get; set; }

        /// <summary>
        /// End year
        /// </summary>
        [HtmlAttributeName(END_YEAR_ATTRIBUTE_NAME)]
        public int? EndYear { get; set; }

        /// <summary>
        /// Selected day
        /// </summary>
        [HtmlAttributeName(SELECTED_DAY_ATTRIBUTE_NAME)]
        public int? SelectedDay { get; set; }

        /// <summary>
        /// Selected month
        /// </summary>
        [HtmlAttributeName(SELECTED_MONTH_ATTRIBUTE_NAME)]
        public int? SelectedMonth { get; set; }

        /// <summary>
        /// Selected year
        /// </summary>
        [HtmlAttributeName(SELECTED_YEAR_ATTRIBUTE_NAME)]
        public int? SelectedYear { get; set; }

        /// <summary>
        /// Wrap tags
        /// </summary>
        [HtmlAttributeName(WRAP_TAGS_ATTRIBUTE_NAME)]
        public string WrapTags { get; set; }

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        #endregion

        #region Fields

        private readonly IHtmlHelper _htmlHelper;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public TvProgDatePickerTagHelper(IHtmlGenerator generator, IHtmlHelper htmlHelper, ILocalizationService localizationService)
        {
            Generator = generator;
            _htmlHelper = htmlHelper;
            _localizationService = localizationService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously executes the tag helper with the given context and output
        /// </summary>
        /// <param name="context">Contains information associated with the current HTML tag</param>
        /// <param name="output">A stateful HTML element used to generate an HTML tag</param>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            //contextualize IHtmlHelper
            var viewContextAware = _htmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);

            //tag details
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("class", "date-picker-wrapper");

            //build date selects
            var daysList = new TagBuilder("select");
            var monthsList = new TagBuilder("select");
            var yearsList = new TagBuilder("select");

            daysList.Attributes.Add("name", DayName);
            monthsList.Attributes.Add("name", MonthName);
            yearsList.Attributes.Add("name", YearName);

            var tagHelperAttributes = new List<string>
            {
                DAY_NAME_ATTRIBUTE_NAME,
                MONTH_NAME_ATTRIBUTE_NAME,
                YEAR_NAME_ATTRIBUTE_NAME,
                BEGIN_YEAR_ATTRIBUTE_NAME,
                END_YEAR_ATTRIBUTE_NAME,
                SELECTED_DAY_ATTRIBUTE_NAME,
                SELECTED_MONTH_ATTRIBUTE_NAME,
                SELECTED_YEAR_ATTRIBUTE_NAME,
                WRAP_TAGS_ATTRIBUTE_NAME
            };
            var userAttributes = new Dictionary<string, object>();
            foreach (var attribute in context.AllAttributes)
            {
                if (!tagHelperAttributes.Contains(attribute.Name))
                    userAttributes.Add(attribute.Name, attribute.Value);
            }
            var htmlAttributesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(userAttributes);
            daysList.MergeAttributes(htmlAttributesDictionary, true);
            monthsList.MergeAttributes(htmlAttributesDictionary, true);
            yearsList.MergeAttributes(htmlAttributesDictionary, true);

            var days = new StringBuilder();
            var months = new StringBuilder();
            var years = new StringBuilder();

            days.AppendFormat("<option value='{0}'>{1}</option>", "0", await _localizationService.GetResourceAsync("Common.Day"));

            for (var i = 1; i <= 31; i++)
                days.AppendFormat("<option value='{0}'{1}>{0}</option>", i,
                    (SelectedDay.HasValue && SelectedDay.Value == i) ? " selected=\"selected\"" : null);

            months.AppendFormat("<option value='{0}'>{1}</option>", "0", await _localizationService.GetResourceAsync("Common.Month"));

            for (var i = 1; i <= 12; i++)
            {
                months.AppendFormat("<option value='{0}'{1}>{2}</option>",
                    i,
                    (SelectedMonth.HasValue && SelectedMonth.Value == i) ? " selected=\"selected\"" : null,
                    CultureInfo.CurrentUICulture.DateTimeFormat.GetMonthName(i));
            }

            years.AppendFormat("<option value='{0}'>{1}</option>", "0", await _localizationService.GetResourceAsync("Common.Year"));

            if (BeginYear == null)
                BeginYear = DateTime.UtcNow.Year - 100;
            if (EndYear == null)
                EndYear = DateTime.UtcNow.Year;

            if (EndYear > BeginYear)
            {
                for (var i = BeginYear.Value; i <= EndYear.Value; i++)
                    years.AppendFormat("<option value='{0}'{1}>{0}</option>", i,
                        (SelectedYear.HasValue && SelectedYear.Value == i) ? " selected=\"selected\"" : null);
            }
            else
            {
                for (var i = BeginYear.Value; i >= EndYear.Value; i--)
                    years.AppendFormat("<option value='{0}'{1}>{0}</option>", i,
                        (SelectedYear.HasValue && SelectedYear.Value == i) ? " selected=\"selected\"" : null);
            }

            daysList.InnerHtml.AppendHtml(days.ToString());
            monthsList.InnerHtml.AppendHtml(months.ToString());
            yearsList.InnerHtml.AppendHtml(years.ToString());

            if (bool.TryParse(WrapTags, out var wrapTags) && wrapTags)
            {
                var wrapDaysList = "<span class=\"days-list select-wrapper\">" + await daysList.RenderHtmlContentAsync() + "</span>";
                var wrapMonthsList = "<span class=\"months-list select-wrapper\">" + await monthsList.RenderHtmlContentAsync() + "</span>";
                var wrapYearsList = "<span class=\"years-list select-wrapper\">" + await yearsList.RenderHtmlContentAsync() + "</span>";

                output.Content.AppendHtml(wrapDaysList);
                output.Content.AppendHtml(wrapMonthsList);
                output.Content.AppendHtml(wrapYearsList);
            }
            else
            {
                output.Content.AppendHtml(daysList);
                output.Content.AppendHtml(monthsList);
                output.Content.AppendHtml(yearsList);
            }
        }

        #endregion
    }
}