using PageAccessCap.Utils;
using System;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Routing;

namespace PageAccessCap.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PageAccessAttribute : FilterAttribute, IActionFilter
    {
        static Cache _cache = HttpRuntime.Cache;
        string _uniqueID;
        string ddosWarningLink = "https://legalpiracy.wordpress.com/2011/01/10/ddos-attacks-and-the-law";

        int _threshold = 3;
        int _ddosThreshold = 10;
        double _sleepMinute = 15.0;

        const string CACHE_KEY = "CacheKey.PageAccessAttribute";

        #region Public Properties
        /// <summary>
        /// Number of subsequent page-hits allowed. Default is 3.
        /// </summary>
        public int AccessThreshold
        {
            get { return _threshold; }
            set { _threshold = value; }
        }
        /// <summary>
        /// Number of minute to keep target user's page-access capability suspended. Default is 15 minutes.
        /// </summary>
        public double SleepMinute
        {
            get { return _sleepMinute; }
            set { _sleepMinute = value; }
        }
        /// <summary>
        /// Arguments: {0} = access threshold, {1} = sleep minute
        /// </summary>
        public string MessageFormat { get; set; }
        public bool RedirectOnThresholdHit { get; set; } = true;
        #endregion

        int? TotalPageHits
        {
            get
            {
                return _cache[_uniqueID] as int?;
            }
            set
            {
                AddOrUpdateCache(value);
            }
        }


        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            var result = filterContext.Result;

            if (!ValidPageHits(filterContext.HttpContext, ref result))
                filterContext.Result = result;
        }

        void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        {
            //Supposedly derived class/controller already performed sign-in operation. 
            //If it still got here, user was not authenticated.

            if (!filterContext.HttpContext.Request.IsAuthenticated)
            {
                if (_uniqueID == null)
                    _uniqueID = GetClientUniqueID(filterContext.RequestContext.HttpContext.Request);

                if (TotalPageHits == null)
                {
                    AddOrUpdateCache(1, true);
                }
                else
                {
                    TotalPageHits++;
                }

                ActionResult result = filterContext.Result;

                if (!ValidPageHits(filterContext.HttpContext, ref result))
                    filterContext.Result = result;
            }
            else
            {
                if (_uniqueID != null)
                {
                    ClearUserAccessLimit();
                    _uniqueID = null;
                }
            }
        }

        bool ValidPageHits(HttpContextBase context, ref ActionResult result)
        {
            if (_uniqueID != null && !context.Request.IsAuthenticated)
            {
                int hits = TotalPageHits.GetValueOrDefault();

                if (hits >= AccessThreshold)
                {
                    //SleepMinute is not even over yet, and still trying? DDoS attempts?
                    if (hits >= _ddosThreshold)
                    {
                        if (RedirectOnThresholdHit)
                        {
                            if (context.Request.IsAjaxRequest())
                            {
                                result = new JsonRedirectResult(ddosWarningLink);
                            }
                            else
                            {
                                result = new RedirectResult(ddosWarningLink, true);
                            }
                        }

                        AddOrUpdateCache(hits, false);
                    }
                    else
                    {
                        TotalPageHits++;

                        string message;

                        if (string.IsNullOrEmpty(this.MessageFormat))
                            message = $"You have made at least {AccessThreshold} consecutive failed attempts to login within the past 24 hours, your access is thus suspended for the next {SleepMinute} minutes.";
                        else
                            message = string.Format(MessageFormat, AccessThreshold, SleepMinute);

                        throw new HttpException((int)HttpStatusCode.Forbidden, message);
                    }

                    return false;
                };
            }

            return true;
        }

        void AddOrUpdateCache(object value, bool useAbsoluteExpiration = true)
        {
            if (useAbsoluteExpiration)
                _cache.Insert(_uniqueID, value, null, DateTime.Now.AddMinutes(this.SleepMinute), Cache.NoSlidingExpiration, CacheItemPriority.High, new CacheItemRemovedCallback(OnCacheRemoved));
            else
                _cache.Insert(_uniqueID, value, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(this.SleepMinute), CacheItemPriority.High, new CacheItemRemovedCallback(OnCacheRemoved));
        }

        void OnCacheRemoved(string key, object value, CacheItemRemovedReason removedReason)
        {

        }

        static string GetClientUniqueID(HttpRequestBase requestContext)
        {
            //Apparently not the best combination yet
            return string.Join("::",
                CACHE_KEY,
                requestContext.Path,
                IPManager.GetClientIP(requestContext)
            );
        }

        bool ClearUserAccessLimit()
        {
            return _cache.Remove(_uniqueID) != null;
        }

        #region CONSTRUCTORS
        public PageAccessAttribute()
            : base()
        {

        }
        public PageAccessAttribute(int threshold)
            : this()
        {
            this._threshold = threshold;
        }
        #endregion
    }
}