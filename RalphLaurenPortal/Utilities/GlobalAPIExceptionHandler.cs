using Microsoft.AspNet.Identity;

using RalphLaurenPortal.Models;

using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace RalphLaurenPortal.Utilities
{
	public class GlobalAPIExceptionHandler : ExceptionHandler
	{
		public override void Handle(ExceptionHandlerContext context)
		{
			// 1. Get the principal from the Web API request context
			var user = context.RequestContext.Principal;

			int userId = 0;

			// 2. Check if the user is authenticated and is a ClaimsIdentity
			if (user?.Identity?.IsAuthenticated == true && user.Identity is ClaimsIdentity claimsIdentity)
			{
				// Use the standard AspNet.Identity extension method to find the ID string
				string userIdString = claimsIdentity.GetUserId();

				// Parse it to your required integer type
				int.TryParse(userIdString, out userId);
			}

			var exception = context.Exception;

			var db = new PortalDbContext();

			db.SaveToErrorLog(
				memberId: userId,
				exceptionType: exception.GetType().ToString(),
				exceptionMessage: exception.Message,
				exceptionSource: exception.Source,
				stackTrace: exception.StackTrace,
				innerExceptionType: exception.InnerException?.GetType().ToString(),
				innerExceptionMessage: exception.InnerException?.Message,
				innerExceptionSource: exception.InnerException?.Source,
				innerExceptionStackTrace: exception.InnerException?.StackTrace,
				context.Request.RequestUri.AbsoluteUri,
				context.Request?.Headers?.UserAgent?.ToString(),
				GetClientIpAddress(context.Request)
			);

			// 2. Create a generic, safe user response
			var response = context.Request.CreateResponse(
				HttpStatusCode.InternalServerError,
				new { Message = "An unexpected error occurred." }
			);

			context.Result = new ResponseMessageResult(response);
		}

		private string GetClientIpAddress(HttpRequestMessage request)
		{
			if (request == null) return "Unknown";

			// Scenario 1: standard IIS Hosting (Web Host)
			if (request.Properties.ContainsKey("MS_HttpContext"))
			{
				var ctx = request.Properties["MS_HttpContext"] as HttpContextBase;
				if (ctx?.Request != null)
				{
					// Check if behind a load balancer/proxy first
					string forwardedFor = ctx.Request.Headers["X-Forwarded-For"];
					if (!string.IsNullOrEmpty(forwardedFor))
					{
						// Can contain multiple IPs comma-separated; first one is the client
						return forwardedFor.Split(',')[0].Trim();
					}

					return ctx.Request.UserHostAddress;
				}
			}

			// Scenario 2: Self-Host or OWIN-driven hosting (Katana)
			if (request.Properties.ContainsKey("MS_OwinContext"))
			{
				var ctx = request.Properties["MS_OwinContext"] as dynamic; // or Microsoft.Owin.OwinContext
				string remoteIp = ctx?.Request?.RemoteIpAddress;
				if (!string.IsNullOrEmpty(remoteIp))
				{
					return remoteIp;
				}
			}

			// Scenario 3: Falling back to direct network connection properties
			if (request.Properties.ContainsKey("System.ServiceModel.Channels.RemoteEndpointMessageProperty"))
			{
				var prop = request.Properties["System.ServiceModel.Channels.RemoteEndpointMessageProperty"] as dynamic;
				if (prop != null)
				{
					return prop.Address;
				}
			}

			return "Unknown";
		}
	}
}