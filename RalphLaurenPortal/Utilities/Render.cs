using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Web;
using System.Web.Optimization;

namespace RalphLaurenPortal.Utilities
{
    public static class Render
    {
        private static readonly char[] newline = { '\r', '\n' };

        private static readonly char[] tilde = { '~' };

        public static IHtmlString Styles(string path)
        {
            return new HtmlString(System.Web.Optimization.Styles.RenderFormat(@"<link href=""{0}"" rel=""stylesheet"" />", path).ToString().TrimEnd(newline));
        }

        public static IHtmlString ScriptAsync(string path)
        {
#if DEBUG
            var files = new BundleResolver().GetBundleContents(path);

            var bundleBytes = new List<byte>();

            string b64encodedHash = null;

            using (var sha512 = SHA512.Create())
            {
                foreach (var f in files)
                {
                    bundleBytes.AddRange(System.IO.File.ReadAllBytes(HttpContext.Current.Server.MapPath(f)));
                }

                var hashedBytes = sha512.ComputeHash(bundleBytes.ToArray());

                b64encodedHash = Convert.ToBase64String(hashedBytes);
            }

            return new HtmlString(Scripts.RenderFormat($"<script src='{{0}}' integrity='sha512-{b64encodedHash}' async></script>", path).ToString().TrimEnd(newline));

#else
            var bundleContext = new BundleContext(new HttpContextWrapper(HttpContext.Current), BundleTable.Bundles, path);

            var bundle = BundleTable.Bundles.GetBundleFor(path);

            var bundleResponse = bundle.GenerateBundleResponse(bundleContext);

            var bundleBytes = System.Text.Encoding.UTF8.GetBytes(bundleResponse.Content);

            var b64encodedHash = (string)null;

            using (var sha512 = SHA512.Create())
            {
                var hashedBytes = sha512.ComputeHash(bundleBytes);

                b64encodedHash = Convert.ToBase64String(hashedBytes);
            }

            return new HtmlString(Scripts.RenderFormat($"<script src='{{0}}' integrity='sha512-{b64encodedHash}' async></script>", path).ToString().TrimEnd(newline));
#endif
        }

        public static IHtmlString ScriptDefer(string path)
        {
            // Not entirely sure this is worth all the bother - it will add yae much overhead to
            // every single page view...

            return new HtmlString(Scripts.RenderFormat($"<script src='{{0}}' defer></script>", path).ToString().TrimEnd(newline));

#if DEBUG

            //var result = "";

            //var files = new BundleResolver().GetBundleContents(path);

            //string b64encodedHash = null;

            //using (var sha512 = SHA512.Create())
            //{
            //    foreach (var f in files)
            //    {
            //        var filePath = HttpContext.Current.Server.MapPath(f);

            //        var hashedBytes = sha512.ComputeHash(System.IO.File.ReadAllBytes(filePath));

            //        b64encodedHash = Convert.ToBase64String(hashedBytes);

            //        result += $"<script src='{f.TrimStart(tilde)}' integrity='sha512-{b64encodedHash}' defer></script>{Environment.NewLine}";
            //    }
            //}

            //return new HtmlString(result.TrimEnd(newline));

#else

            var bundleContext = new BundleContext(new HttpContextWrapper(HttpContext.Current), BundleTable.Bundles, path);

            var bundle = BundleTable.Bundles.GetBundleFor(path);

            var bundleResponse = bundle.GenerateBundleResponse(bundleContext);

            var bundleBytes = System.Text.Encoding.UTF8.GetBytes(bundleResponse.Content);

            var b64encodedHash = (string)null;

            using (var sha512 = SHA512.Create())
            {
                var hashedBytes = sha512.ComputeHash(bundleBytes);

                b64encodedHash = Convert.ToBase64String(hashedBytes);
            }

            return new HtmlString(Scripts.RenderFormat($"<script src='{{0}}' integrity='sha512-{b64encodedHash}' defer></script>", path).ToString().TrimEnd(newline));

#endif
        }
    }
}