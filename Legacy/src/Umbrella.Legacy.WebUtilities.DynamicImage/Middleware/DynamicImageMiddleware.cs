using log4net;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbrella.WebUtilities.DynamicImage.Enumerations;
using Umbrella.Utilities.Extensions;
using Umbrella.WebUtilities.DynamicImage.Configuration;
using Umbrella.WebUtilities.DynamicImage;
using System.IO;
using Umbrella.Legacy.WebUtilities.Extensions;
using System.Net;
using System.Web.Configuration;

namespace Umbrella.Legacy.WebUtilities.DynamicImage.Middleware
{
	public class DynamicImageMiddleware : OwinMiddleware
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(DynamicImageMiddleware));

		public DynamicImageMiddleware(OwinMiddleware next)
			: base(next)
		{
			if (Log.IsDebugEnabled)
				Log.Debug("DynamicImageMiddleware registered successfully");
		}

		public override async Task Invoke(IOwinContext context)
		{
			bool exceptionOccurred = false;

            DynamicImageMappingsConfig mappingsConfig = new DynamicImageMappingsConfig(WebConfigurationManager.OpenWebConfiguration("~/web.config"));

			try
			{
				string requestedPath = VirtualPathUtility.ToAppRelative(context.Request.Path.Value).ToLower();

				if (requestedPath.StartsWith("~/dynamicimage/"))
				{
					//Break down the url into segments
					string[] segments = requestedPath.Split('/');

					//Check there are at least 7 segments
					if (segments.Length < 7)
					{
						await context.Response.SendStatusCode(HttpStatusCode.NotFound);
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
						if (mappingsConfig.Enabled)
						{
							DynamicImageMapping mapping = new DynamicImageMapping
							{
								Width = width,
								Height = height,
								ResizeMode = mode,
								Format = DynamicImageUtility.ParseImageFormat(originalExtension.ToLower())
							};

							//If the mapping is invalid, return a 404
							if (!mappingsConfig.Settings.Any(x => x == mapping))
							{
								await context.Response.SendStatusCode(HttpStatusCode.NotFound);
								return;
							}
						}

                        Umbrella.WebUtilities.DynamicImage.DynamicImage image = DynamicImageUtility.GetImage(width, height, mode, originalExtension, path);

						if (!string.IsNullOrEmpty(image.CachedVirtualPath))
						{
							await context.Response.SendFileAsync(image.CachedVirtualPath.TrimStart('~'));
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
									await context.Response.SendStatusCode(HttpStatusCode.NotModified);
									return;
								}
							}

							context.Response.Headers.Append("Content-Type", "image/" + image.ImageOptions.Format.ToString().ToLower());
							context.Response.Headers.Append("Last-Modified", image.LastModified.ToUniversalTime().ToString());

							//Write the file to the output stream
							await context.Response.WriteAsync(image.Content);
							return;
						}
					}
				}
			}
			catch (Exception exc) when (Log.LogError(exc, message: "Error in DynamicImageModule for path: " + context.Request.Path, returnValue: true))
			{
                await context.Response.SendStatusCode(HttpStatusCode.NotFound);
                return;
            }

			await Next.Invoke(context);
		}
	}
}