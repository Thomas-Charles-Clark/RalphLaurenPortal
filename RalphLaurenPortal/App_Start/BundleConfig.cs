using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Optimization;

namespace RalphLaurenPortal
{
    public class BundleConfig
    {
        private static readonly CssSourceMapUrlRewrite cssSourceMapRewrite = new CssSourceMapUrlRewrite();

        private static readonly IItemTransform[] cssTransforms = { new CssRewriteUrlTransform(), new CssSourceMapUrlRewrite() };

        private static readonly IItemTransform[] altCssTransforms = { new CssSourceMapUrlRewrite(), new CustomCssRewriteUrlTransform() };

        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //
            // Styles
            //
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            bundles.Add(new StyleBundle("~/bundles/css/fonts")
                .Include("~/css/fonts/fonts.css", cssTransforms));

            bundles.Add(new StyleBundle("~/bundles/css/font-awesome")
                .Include("~/css/fonts/font-awesome.css", cssTransforms));

            bundles.Add(new StyleBundle("~/bundles/css/bootstrap-icons")
                .Include("~/css/fonts/bootstrap-icons.css", cssTransforms));

            bundles.Add(new StyleBundle("~/bundles/css/bootstrap")
                .Include("~/css/lib/bootstrap/dist/bootstrap.min.css", cssSourceMapRewrite));

            bundles.Add(new StyleBundle("~/bundles/css/kendo")
                .Include("~/css/lib/kendo/dist/2018.3.1017/kendo.common.min.css", altCssTransforms)
                .Include("~/css/lib/kendo/dist/2018.3.1017/kendo.default.min.css", altCssTransforms));

            bundles.Add(new StyleBundle("~/bundles/css/common")
                .Include("~/css/common/common.min.css", cssTransforms));

            bundles.Add(new StyleBundle("~/bundles/css/desktop")
                .Include("~/css/common/desktop.min.css", cssTransforms));

            bundles.Add(new StyleBundle("~/bundles/css/mobile")
                .Include("~/css/common/mobile.min.css", cssTransforms));

            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //
            // Scripts
            //
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            bundles.Add(new ScriptBundle("~/bundles/js/jquery").Include(
                        "~/js/lib/jquery/dist/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/jqueryval").Include(
                        "~/js/lib/jquery/dist/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/js/luxon").Include(
                        "~/js/lib/luxon/dist/luxon.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/home").Include(
                "~/js/models/home/page-view-model.js",
                "~/js/models/home/order-status-view-model.js",
                "~/js/models/home/stock-view-model.js",
                "~/js/models/home/tech-info-view-model.js",
                "~/js/models/home/sales-history-view-model.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/js/bootstrap").Include(
              "~/js/lib/bootstrap/dist/bootstrap.bundle.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/js/kendo").Include(
                "~/js/lib/kendo/dist/2018.3.1017/jszip.min.js",
                "~/js/lib/kendo/dist/2018.3.1017/kendo.all.js",
                "~/js/lib/kendo/dist/2018.3.1017/kendo.aspnetmvc.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/js/ko").Include(
                "~/js/lib/knockout/dist/knockout-latest.js",
                "~/js/lib/knockout/dist/knockout-validation.js",
                "~/js/lib/knockout/dist/knockout-kendo.js",
                "~/js/lib/knockout/dist/knockout-pre-rendered.js",
                "~/js/lib/knockout/custom/knockout-extensions.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/js/linq").Include(
                "~/js/lib/linq/dist/linq.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/js/error-modal").Include(
                "~/js/common/error-modal.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/js/trade-account-selector").Include(
                "~/js/common/trade-account-selector.js"
                ));

            foreach (var b in BundleTable.Bundles)
            {
                b.Transforms.Clear();
            }

#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
        }
    }


    // adapted from: https://stackoverflow.com/questions/12577108/asp-net-mvc4-bundling-with-twitter-bootstrap#answer-17702773
    // with thanks
    public class CssSourceMapUrlRewrite : IItemTransform
    {
        private static string ConvertUrlsToAbsolute(string baseUrl, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return content;
            }

            var regex = new Regex("\\/\\*\\# sourceMappingURL\\=(?<path>.+\\.map) \\*\\/");

            return regex.Replace(content, (match) => 
            {
                return $"/*# sourceMappingURL={HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Authority}{RebaseUrlToAbsolute(baseUrl, match.Groups["path"].Value)} */";
            });
        }

        public string Process(string includedVirtualPath, string input)
        {
            if (includedVirtualPath == null)
            {
                throw new ArgumentNullException("includedVirtualPath");
            }
            var directory = VirtualPathUtility.GetDirectory(includedVirtualPath);
            return ConvertUrlsToAbsolute(directory, input);
        }

        private static string RebaseUrlToAbsolute(string baseUrl, string url)
        {
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(baseUrl) || url.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                return url;
            }
            if (!baseUrl.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                baseUrl = string.Concat(baseUrl, "/");
            }
            return VirtualPathUtility.ToAbsolute(string.Concat(baseUrl, url));
        }
    }

    //https://github.com/aspnet/AspNetWebOptimization/blob/master/src/System.Web.Optimization/CssRewriteUrlTransform.cs
    public class CustomCssRewriteUrlTransform : IItemTransform
    {
        internal static string RebaseUrlToAbsolute(string baseUrl, string url)
        {
            // Don't do anything to invalid urls or absolute urls
            if (String.IsNullOrWhiteSpace(url) ||
                String.IsNullOrWhiteSpace(baseUrl) ||
                url.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                return url;
            }

            // NOTE: now we support for ~ app relative urls here
            if (!baseUrl.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                baseUrl += "/";
            }

            return VirtualPathUtility.ToAbsolute(baseUrl + url);
        }

        internal static string ConvertUrlsToAbsolute(string baseUrl, string content)
        {
            if (String.IsNullOrWhiteSpace(content))
            {
                return content;
            }

            // Replace all urls with absolute urls
            Regex url = new Regex(@"url\(['""]?(?<url>[^)]+?)['""]?\)");
            return url.Replace(content, ((match) => {
                return "url(" + RebaseUrlToAbsolute(baseUrl, match.Groups["url"].Value) + ")";
            }));
        }

        /// <summary>
        /// Converts any urls in the input to absolute using the base directory of the include virtual path.
        /// </summary>
        /// <param name="includedVirtualPath">The virtual path that was included in the bundle for this item that is being transformed</param>
        /// <param name="input"></param>
        /// <example>
        /// bundle.Include("~/content/some.css") will transform url(images/1.jpg) => url(/content/images/1.jpg)
        /// </example>
        public string Process(string includedVirtualPath, string input)
        {
            if (includedVirtualPath == null)
            {
                throw new ArgumentNullException("includedVirtualPath");
            }
            // Strip off the ~ that always occurs in app relative virtual paths
            string baseUrl = VirtualPathUtility.GetDirectory(includedVirtualPath.Substring(1));
            return ConvertUrlsToAbsolute(baseUrl, input);
        }
    }
}
