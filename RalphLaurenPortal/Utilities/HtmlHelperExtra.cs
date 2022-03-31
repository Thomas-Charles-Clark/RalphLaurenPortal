using RalphLaurenPortal.Models.Resource;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace RalphLaurenPortal.Utilities
{
    public class HtmlHelperExtra
    {
        public static IHtmlString EscapeForJS(string value)
        {
            return new HtmlString(HttpUtility.JavaScriptStringEncode(value));
        }

        public static IHtmlString EscapeForJS(object value)
        {
            return new HtmlString(value == null ? null : HttpUtility.JavaScriptStringEncode(value.ToString()));
        }

        public static IHtmlString Resource(string name, byte languageId, short countryId, DateTime useDate)
        {
            var resourceDefinition = new Resource().FetchResources(countryId, languageId, useDate);

            var value = resourceDefinition.Result.Resources.Where(p => p.Name == name).Select(p => p.Value).SingleOrDefault();

            return new HtmlString(value);
        }

        public static IHtmlString ResourceTitleCase(string name, byte languageId, short countryId, DateTime useDate)
        {
            var ti = new CultureInfo("en-GB", false).TextInfo;

            var resourceDefinition = new Resource().FetchResources(countryId, languageId, useDate);

            var value = resourceDefinition.Result.Resources.Where(p => p.Name == name).Select(p => p.Value).SingleOrDefault();

            return new HtmlString(ti.ToTitleCase(value));
        }
    }
}