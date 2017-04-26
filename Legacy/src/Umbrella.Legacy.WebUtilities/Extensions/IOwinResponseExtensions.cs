using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Owin
{
	public static class IOwinResponseExtensions
	{
		public static Task SendStatusCode(this IOwinResponse response, HttpStatusCode statusCode, bool sendNullBody = true)
		{
			response.StatusCode = (int)statusCode;

			if(sendNullBody)
			{
				response.Body.Close();
				response.Body = Stream.Null;
			}

            return Task.CompletedTask;
		}
	}
}