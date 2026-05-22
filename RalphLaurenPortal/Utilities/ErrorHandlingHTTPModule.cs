using Microsoft.AspNet.Identity;

using RalphLaurenPortal.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RalphLaurenPortal.Utilities
{
	public class ErrorHandlingHTTPModule : IHttpModule
	{
		public void Init(HttpApplication context)
		{
			context.Error += OnError;
		}

		private void OnError(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication)sender;
			HttpContext context = app.Context;
			Exception exception = context.Server.GetLastError();

			// Log your exception here
			var db = new PortalDbContext();

			db.SaveToErrorLog(
				memberId: context.User.Identity.GetUserId<int>(),
				exceptionType: exception.GetType().ToString(),
				exceptionMessage: exception.Message,
				exceptionSource: exception.Source,
				stackTrace: exception.StackTrace,
				innerExceptionType: exception.InnerException?.GetType().ToString(),
				innerExceptionMessage: exception.InnerException?.Message,
				innerExceptionSource: exception.InnerException?.Source,
				innerExceptionStackTrace: exception.InnerException?.StackTrace,
				context.Request.RawUrl,
				context.Request.UserAgent,
				context.Request.UserHostAddress
			); 
			
			if (context.IsDebuggingEnabled)
			{
				return;
			}
			else
			{
				// Thanks, Google.

				// Clear the error to prevent the yellow screen of death
				context.Server.ClearError();

				// Clear the system error flag so the YSOD is suppressed
				context.Server.ClearError();
				context.Response.Clear();
				context.Response.StatusCode = 500;

				// Instantiate and run the MVC pipeline pointing to your Error Controller
				var routeData = new RouteData();
				routeData.Values.Add("controller", "Error");
				routeData.Values.Add("action", "Index");
				routeData.Values.Add("exception", exception); // Optional: Pass exception metadata to the view

				// Use standard MVC controller execution factories to render Error.cshtml
				IController errorController = ControllerBuilder.Current.GetControllerFactory()
					.CreateController(new RequestContext(new HttpContextWrapper(context), routeData), "Error");

				errorController.Execute(new RequestContext(new HttpContextWrapper(context), routeData));

				// Complete the pipeline immediately to prevent lower IIS handlers from writing data
				app.CompleteRequest();
			}
		}

		public void Dispose() { }
	}
}