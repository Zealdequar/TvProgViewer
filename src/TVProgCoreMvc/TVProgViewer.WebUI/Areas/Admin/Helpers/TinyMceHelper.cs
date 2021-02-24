﻿using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using TVProgViewer.Core;
using TVProgViewer.Core.Infrastructure;

namespace TVProgViewer.WebUI.Areas.Admin.Helpers
{
    /// <summary>
    /// TinyMCE helper
    /// </summary>
    public static class TinyMceHelper
    {
        /// <summary>
        /// Get tinyMCE language name for current language 
        /// </summary>
        /// <returns>tinyMCE language name</returns>
        public static async Task<string> GetTinyMceLanguageAsync()
        {
            //nopCommerce supports TinyMCE's localization for 10 languages:
            //Chinese, Spanish, Arabic, Portuguese, Russian, German, French, Italian, Dutch and English out-of-the-box.
            //Additional languages can be downloaded from the website TinyMCE(https://www.tinymce.com/download/language-packages/)

            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var webHostEnvironment = EngineContext.Current.Resolve<IWebHostEnvironment>();
            var fileProvider = EngineContext.Current.Resolve<ITvProgFileProvider>();

            var languageCulture = (await workContext.GetWorkingLanguageAsync()).LanguageCulture;

            var langFile = $"{languageCulture}.js";
            var directoryPath = fileProvider.Combine(webHostEnvironment.WebRootPath, @"lib\tinymce\langs");
            var fileExists = fileProvider.FileExists($"{directoryPath}\\{langFile}");

            if (!fileExists)
            {
                languageCulture = languageCulture.Replace('-', '_');
                langFile = $"{languageCulture}.js";
                fileExists = fileProvider.FileExists($"{directoryPath}\\{langFile}");
            }

            if (!fileExists)
            {
                languageCulture = languageCulture.Split('_', '-')[0];
                langFile = $"{languageCulture}.js";
                fileExists = fileProvider.FileExists($"{directoryPath}\\{langFile}");
            }

            return fileExists ? languageCulture : string.Empty;
        }
    }
}