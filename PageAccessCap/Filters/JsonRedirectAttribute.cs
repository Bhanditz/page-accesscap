using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PageAccessCap.Filters
{
    public class JsonRedirectResult : JsonResult
    {
        int statusCode = (int)System.Net.HttpStatusCode.RedirectMethod;
        string _redirectUrl;

        public override void ExecuteResult(ControllerContext context)
        {
            if (!string.IsNullOrEmpty(_redirectUrl))
            {
                var httpResponse = context.HttpContext.Response;

                httpResponse.Clear();
                httpResponse.ContentType = "application/json; charset=utf-8";
                httpResponse.StatusCode = statusCode;

                this.Data = new
                {
                    sc = statusCode,
                    desc = this._redirectUrl
                };
            }

            base.ExecuteResult(context);
        }

        public JsonRedirectResult(string url)
            : base()
        {
            this._redirectUrl = url;
            this.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        }
    }
}