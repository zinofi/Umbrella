using Umbrella.WebUtilities.DynamicImage;
using Umbrella.WebUtilities.DynamicImage.Enumerations;
using System;
using System.Linq;
using System.Web;
using Umbrella.Utilities.Extensions;
using System.IO;
using log4net;
using System.Threading;
using System.Web.Configuration;
using Umbrella.Utilities.log4net;
using Umbrella.WebUtilities.DynamicImage.Interfaces;
using Umbrella.Legacy.Utilities;
using Ninject;
using Umbrella.Legacy.WebUtilities.DynamicImage.Configuration;

namespace Umbrella.Legacy.WebUtilities.DynamicImage.Modules
{
    public class DynamicImageModule : IHttpModule
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DynamicImageModule));

        public void Init(HttpApplication context)
        {
            context.BeginRequest += (sender, e) =>
                {
                    try
                    {
                        DynamicImageMappingsConfig mappingsConfig = new DynamicImageMappingsConfig(WebConfigurationManager.OpenWebConfiguration("~/web.config"));

                        string requestedPath = VirtualPathUtility.ToAppRelative(context.Request.Path).ToLower();

                        if (requestedPath.StartsWith("~/dynamicimage"))
                        {
                            //Break down the url into segments
                            string[] segments = requestedPath.Split('/');

                            //Check there are at least 7 segments
                            if (segments.Length < 7)
                            {
                                context.Response.StatusCode = 404;
                                context.Response.End();
                            }
                            else
                            {
                                IDynamicImageUtility dynamicImageUtility = LibraryBindings.DependencyResolver.Get<IDynamicImageUtility>();

                                //Ignore the first 2 segments
                                int width = int.Parse(segments[2]);
                                int height = int.Parse(segments[3]);
                                DynamicResizeMode mode = segments[4].ToEnum<DynamicResizeMode>();
                                string originalExtension = segments[5];
                                string path = "/" + string.Join("/", segments.Skip(6));

								//Before doing anything, we need to validate that the image parameters we have
								//are permitted by the DynamicImageMappings config settings. These exist to prevent
								//malicious users from requesting images in sizes and formats with the intent of
								//overloading the server with bogus image requests
								if (mappingsConfig.Enabled)
								{
									DynamicImageMapping mapping = new DynamicImageMapping
									{
										Width = width,
										Height = height,
										ResizeMode = mode,
										Format = dynamicImageUtility.ParseImageFormat(originalExtension.ToLower())
									};

									//If the mapping is invalid, return a 404
									if (!mappingsConfig.Settings.Any(x => x == mapping))
									{
										context.Response.StatusCode = 404;
										context.Response.End();
									}
								}

                                Umbrella.WebUtilities.DynamicImage.DynamicImage image = dynamicImageUtility.GetImage(width, height, mode, originalExtension, path);

                                if (!string.IsNullOrEmpty(image.CachedVirtualPath))
                                {
                                    HttpContext.Current.RewritePath(image.CachedVirtualPath);
                                }
                                else if (image.Content != null && image.Content.Length > 0)
                                {
                                    //Get the If-Modified-Since header
                                    string ifModifiedSince = context.Request.Headers["If-Modified-Since"];
                                    if (!string.IsNullOrEmpty(ifModifiedSince))
                                    {
                                        DateTime lastModified = DateTime.Parse(ifModifiedSince);

                                        if (lastModified.ToString() == image.LastModified.ToString())
                                        {
                                            context.Response.StatusCode = 304;
                                            context.Response.End();
                                        }
                                    }

                                    context.Response.Headers.Add("Content-Type", "image/" + image.ImageOptions.Format.ToString().ToLower());
                                    context.Response.Headers.Add("Last-Modified", image.LastModified.ToUniversalTime().ToString());

                                    //Write the file to the output stream
                                    using (MemoryStream ms = new MemoryStream(image.Content))
                                    {
                                        ms.WriteTo(context.Response.OutputStream);
                                        context.Response.StatusCode = 200;
                                        context.Response.End();
                                    }
                                }
                            }
                        }
                    }
                    catch(Exception exc) when (!(exc is ThreadAbortException) && Log.LogError(exc, message: "Error in DynamicImageModule for path: " + context.Request.Path, returnValue: true))
                    {
                        context.Response.StatusCode = 404;
                        context.Response.End();
                    }
                };
        }

        public void Dispose()
        {
            //nothing to explicity dispose
        }
    }
}
