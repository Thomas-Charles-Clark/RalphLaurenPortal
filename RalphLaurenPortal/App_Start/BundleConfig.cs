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

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/js/lib/jquery/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/js/lib/jquery/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/js/common").Include(
                "~/js/common/common.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/js/bootstrap").Include(
              "~/js/lib/bootstrap/dist/bootstrap.bundle.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/js/ko").Include(
                "~/js/lib/knockout/dist/knockout-latest.js",
                "~/js/lib/knockout/dist/knockout-validation.js",
                "~/js/lib/knockout/dist/knockout-pre-rendered.js",
                "~/js/lib/knockout/custom/knockout-extensions.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/js/linq").Include(
                "~/js/lib/linq/linq.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/js/error-modal").Include(
                "~/js/common/error-modal.js"
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
}
