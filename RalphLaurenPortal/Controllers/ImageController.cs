using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RalphLaurenPortal.Controllers
{
    public class ImageController : Controller
    {
        [OutputCache(Duration = 31536000, Location = System.Web.UI.OutputCacheLocation.Downstream)] // Cached for 1 year
        public ImageResult CallImage(string width, string imageid)
        {
            imageid = Path.GetFileNameWithoutExtension(imageid);

            return new ImageResult(width,
                                   imageid,
                                   System.Web.Configuration.WebConfigurationManager.AppSettings["imagePath"]);
        }
    }


    public class ImageResult : ActionResult
    {

        private string _width;
        private string _imageId;
        private string _imagePath;

        public ImageResult(string width, string imageId, string imagePath)
        {
            _width = width;
            _imageId = imageId;
            _imagePath = imagePath;
        }


        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                return;
            }

            bool webp = false;

            //  Is image in cache? // - Tom, 23/10/2018 - removed; CloudFlare should be doing all of our caches.
            byte[] bytes = null;
            //ImageCacheProvider objImageCache = new ImageCacheProvider();
            //bytes = objImageCache.Get("IMAGE", _width, _imageId);

            if (bytes == null)
            {
                //  Read image from disk
#if DEBUG
                var jpegPath = $"{_imagePath}\\{_width}\\{_imageId}.jpeg";
                var webpPath = $"{_imagePath}\\{_width}\\{_imageId}.webp";
#else
                var jpegPath = $"{HttpContext.Current.Server.MapPath($"~/{_imagePath}")}/{_width}/{_imageId}.jpeg";
                var webpPath = $"{HttpContext.Current.Server.MapPath($"~/{_imagePath}")}/{_width}/{_imageId}.webp";
#endif

                bytes = File.ReadAllBytes(jpegPath);

                //if (File.Exists(webpPath))
                //{
                //    bytes = File.ReadAllBytes(webpPath);
                //    webp = true;
                //}
                //else
                //{
                //    if (File.Exists(jpegPath))
                //    {
                //        bytes = File.ReadAllBytes(jpegPath);
                //    }
                //    else
                //    {
                //        var response = context.HttpContext.Response;
                //        response.ContentType = "text/html";
                //        response.BufferOutput = true;
                //        response.Clear();

                //        byte[] buffer = System.Text.Encoding.UTF8.GetBytes($"{jpegPath}\r\n{webpPath}");

                //        response.OutputStream.Write(buffer, 0, buffer.Length);
                        
                //        response.OutputStream.Flush();
                //        response.End();

                //        return;
                //    }
                //}

                //objImageCache.Set("IMAGE", _width, _imageId, bytes, _cacheTimeMinutes);
            }

            
            if (webp)
            {
                var imageStream = new MemoryStream(bytes);

                //  Output image to browser
                var response = context.HttpContext.Response;
                response.ContentType = "image/webp";
                response.BufferOutput = true;
                response.Clear();
                byte[] buffer = new byte[4096];

                while (true)
                {
                    int read = imageStream.Read(buffer, 0, buffer.Length);

                    if (read == 0)
                    {
                        break;
                    }

                    response.OutputStream.Write(buffer, 0, read);
                }
                response.OutputStream.Flush();
                response.End();
                imageStream.Close();
            }
            else
            {
                var imageStream = new MemoryStream(bytes);

                //  Output image to browser
                var response = context.HttpContext.Response;
                response.ContentType = "image/jpeg";
                response.BufferOutput = true;
                response.Clear();
                byte[] buffer = new byte[4096];

                while (true)
                {
                    int read = imageStream.Read(buffer, 0, buffer.Length);

                    if (read == 0)
                    {
                        break;
                    }

                    response.OutputStream.Write(buffer, 0, read);
                }
                
                response.OutputStream.Flush();
                
                if (response.OutputStream.CanWrite)
                    response.End();
                
                imageStream.Close();
            }
           
        }
    }
}