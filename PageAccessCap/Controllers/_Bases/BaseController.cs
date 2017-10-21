using PageAccessCap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PageAccessCap.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            if (!filterContext.ExceptionHandled)
            {
                HttpException httpError = filterContext.Exception as HttpException;

                if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                {
                    this.OnAjaxError(filterContext);
                }
                else
                {
                    RedirectToRouteResult routeResult;

                    if (httpError != null)
                    {
                        int statusCode = httpError.GetHttpCode();

                        switch ((HttpStatusCode)statusCode)
                        {
                            case HttpStatusCode.Unauthorized:
                                routeResult = RedirectToAction("Unauthorized", "Error");
                                break;

                            case HttpStatusCode.NotFound:
                                routeResult = RedirectToAction("NotFound", "Error");
                                break;

                            case HttpStatusCode.Forbidden:
                                routeResult = RedirectToAction("Forbidden", "Error");
                                break;

                            case HttpStatusCode.InternalServerError:
                            case HttpStatusCode.RequestTimeout:
                            default:
                                routeResult = RedirectToAction("InternalServerError", "Error");
                                break;
                        }
                    }
                    else
                    {
                        routeResult = RedirectToAction("NotFound", "Error");
                    }

                    TempData[ErrorController.ErrorModelKey] = filterContext.Exception.Message;

                    filterContext.Result = routeResult;
                }

                filterContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                filterContext.ExceptionHandled = true;
            }
        }

        protected virtual void OnAjaxError(ExceptionContext filterContext)
        {
            var hex = filterContext.Exception as HttpException;

            int errCode;

            if (hex != null)
                errCode = hex.GetHttpCode();
            else
                errCode = (int)HttpStatusCode.InternalServerError;

            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = errCode;
            filterContext.HttpContext.Response.Write(filterContext.Exception.Message);
        }

    }
}