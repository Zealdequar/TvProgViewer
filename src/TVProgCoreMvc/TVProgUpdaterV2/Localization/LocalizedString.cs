﻿using Microsoft.AspNetCore.Html;

namespace TVProgViewer.TVProgUpdaterV2.Localization
{
    /// <summary>
    /// Localized string
    /// </summary>
    public class LocalizedString : HtmlString
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="localized">Localized value</param>
        public LocalizedString(string localized): base (localized)
        {
            Text = localized;
        }
        
        /// <summary>
        /// Text
        /// </summary>
        public string Text { get; }
    }
}