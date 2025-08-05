using HomeEstate.Services.Core.Dtos;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace HomeEstate.Web.Extensions
{
    // Extensions/HtmlExtensions.cs
    public static class HtmlExtensions
    {
        public static IHtmlContent GenericPagination<T>(this IHtmlHelper htmlHelper,
            IPagination model,
            string loadFunction = "loadData",
            bool showTooltips = false)
        {
            var viewData = new ViewDataDictionary(htmlHelper.ViewData)
            {
                ["LoadFunction"] = loadFunction,
                ["ShowTooltips"] = showTooltips.ToString().ToLower()
            };
                
            return htmlHelper.Partial("_GenericPagination", model, viewData);
        }
    }
}
