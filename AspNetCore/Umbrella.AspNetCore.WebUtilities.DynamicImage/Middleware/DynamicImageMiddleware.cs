using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Umbrella.AspNetCore.WebUtilities.DynamicImage.Configuration;
using Umbrella.AspNetCore.WebUtilities.DynamicImage.Middleware.Options;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Hosting;
using Umbrella.WebUtilities.DynamicImage.Configuration;
using Umbrella.WebUtilities.DynamicImage.Enumerations;
using Umbrella.WebUtilities.DynamicImage.Interfaces;

namespace Umbrella.AspNetCore.WebUtilities.DynamicImage.Middleware
{
    public class DynamicImageMiddleware
    {
        #region Private Members
        private readonly RequestDelegate m_Next;
        private readonly ILogger<DynamicImageMiddleware> m_Logger;
        private readonly DynamicImageOptions m_DynamicImageOptions;
        private readonly IDynamicImageUtility m_DynamicImageUtility;
        private readonly IUmbrellaHostingEnvironment m_UmbrellaHostingEnvironment;
        private readonly DynamicImageMiddlewareOptions m_MiddlewareOptions = new DynamicImageMiddlewareOptions();
        #endregion

        #region Constructors
        public DynamicImageMiddleware(RequestDelegate next,
            ILogger<DynamicImageMiddleware> logger,
            IOptions<DynamicImageOptions> options,
            IDynamicImageUtility dynamicImageUtility,
            IUmbrellaHostingEnvironment umbrellaHostingEnvironment,
            Action<DynamicImageMiddlewareOptions> optionsBuilder)
        {
            m_Next = next;
            m_Logger = logger;
            m_DynamicImageOptions = options.Value;
            m_DynamicImageUtility = dynamicImageUtility;
            m_UmbrellaHostingEnvironment = umbrellaHostingEnvironment;

            optionsBuilder?.Invoke(m_MiddlewareOptions);
        }
        #endregion

        #region Public Methods
        public async Task Invoke(HttpContext context)
        {
            try
            {
                string requestedPath = context.Request.Path.Value.ToLowerInvariant();

                if (requestedPath.StartsWith("/dynamicimage/"))
                {
                    //Break down the url into segments
                    string[] segments = requestedPath.Split('/');

                    //Check there are at least 7 segments
                    if (segments.Length < 7)
                    {
                        SetResponseStatusCode(context.Response, HttpStatusCode.NotFound);
                        return;
                    }
                    else
                    {
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
                        if (m_DynamicImageOptions.Enabled)
                        {
                            DynamicImageMapping mapping = new DynamicImageMapping
                            {
                                Width = width,
                                Height = height,
                                ResizeMode = mode,
                                Format = m_DynamicImageUtility.ParseImageFormat(originalExtension.ToLower())
                            };

                            //If the mapping is invalid, return a 404
                            if (!m_DynamicImageOptions.Mappings.Any(x => x == mapping))
                            {
                                SetResponseStatusCode(context.Response, HttpStatusCode.NotFound);
                                return;
                            }
                        }

                        if (m_MiddlewareOptions.MapFromWebRoot)
                            path = $"/wwwroot{path}";

                        Umbrella.WebUtilities.DynamicImage.DynamicImage image = m_DynamicImageUtility.GetImage(width, height, mode, originalExtension, path);

                        if (image == null)
                        {
                            SetResponseStatusCode(context.Response, HttpStatusCode.NotFound);
                            return;
                        }

                        if (!string.IsNullOrEmpty(image.CachedVirtualPath))
                        {
                            string fileName = m_UmbrellaHostingEnvironment.MapPath(image.CachedVirtualPath);

                            AppendResponseHeaders(context.Response, image);

                            await context.Response.SendFileAsync(fileName);
                            return;
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
                                    SetResponseStatusCode(context.Response, HttpStatusCode.NotModified);
                                    return;
                                }
                            }

                            AppendResponseHeaders(context.Response, image);

                            //Write the file to the output stream
                            await context.Response.Body.WriteAsync(image.Content, 0, image.Content.Length);
                            return;
                        }
                    }
                }
            }
            catch (Exception exc) when (m_Logger.WriteError(exc, message: "Error in DynamicImageModule for path: " + context.Request.Path, returnValue: false))
            {
                SetResponseStatusCode(context.Response, HttpStatusCode.NotFound);
                return;
            }

            await m_Next.Invoke(context);
        }
        #endregion

        #region Private Methods
        private void AppendResponseHeaders(HttpResponse response, Umbrella.WebUtilities.DynamicImage.DynamicImage dynamicImage)
        {
            response.Headers.Append("Content-Type", "image/" + dynamicImage.ImageOptions.Format.ToString().ToLower());
            response.Headers.Append("Last-Modified", dynamicImage.LastModified.ToUniversalTime().ToString("r"));
        }

        private void SetResponseStatusCode(HttpResponse response, HttpStatusCode statusCode)
        {
            response.Clear();
            response.StatusCode = (int)statusCode;
        }
        #endregion
    }
}