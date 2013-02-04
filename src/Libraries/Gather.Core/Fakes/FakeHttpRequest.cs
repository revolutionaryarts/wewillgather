﻿using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Gather.Core.Fakes
{
    public class FakeHttpRequest : HttpRequestBase
    {
        private readonly HttpCookieCollection _cookies;
        private readonly NameValueCollection _formParams;
        private readonly NameValueCollection _queryStringParams;
        private readonly NameValueCollection _serverVariables;
        private readonly RequestContext _requestContext;
        private readonly string _relativeUrl;
        private readonly Uri _url;
        private readonly Uri _urlReferrer;
        private readonly string _httpMethod;

        public FakeHttpRequest(string relativeUrl, string method,
            NameValueCollection formParams, NameValueCollection queryStringParams,
            HttpCookieCollection cookies, NameValueCollection serverVariables, RequestContext requestContext)
        {
            _httpMethod = method;
            _relativeUrl = relativeUrl;
            _formParams = formParams;
            _queryStringParams = queryStringParams;
            _cookies = cookies;
            _serverVariables = serverVariables;
            _requestContext = requestContext;
            if (_formParams == null)
                _formParams = new NameValueCollection();
            if (_queryStringParams == null)
                _queryStringParams = new NameValueCollection();
            if (_cookies == null)
                _cookies = new HttpCookieCollection();
            if (_serverVariables == null)
                _serverVariables = new NameValueCollection();
            if (_requestContext == null)
            {
                _requestContext = new RequestContext {HttpContext = new FakeHttpContext(""), RouteData = new RouteData()};
            }
        }

        public FakeHttpRequest(string relativeUrl, string method, Uri url, Uri urlReferrer,
            NameValueCollection formParams, NameValueCollection queryStringParams,
            HttpCookieCollection cookies, NameValueCollection serverVariables, RequestContext requestContext)
            : this(relativeUrl, method, formParams, queryStringParams, cookies, serverVariables, requestContext)
        {
            _url = url;
            _urlReferrer = urlReferrer;
        }

        public FakeHttpRequest(string relativeUrl, Uri url, Uri urlReferrer)
            : this(relativeUrl, HttpVerbs.Get.ToString("g"), url, urlReferrer, null, null, null, null, null)
        {
        }

        public override NameValueCollection ServerVariables
        {
            get
            {
                return _serverVariables;
            }
        }

        public override NameValueCollection Form
        {
            get { return _formParams; }
        }

        public override RequestContext RequestContext
        {
            get { return _requestContext; }
        }

        public override NameValueCollection QueryString
        {
            get { return _queryStringParams; }
        }

        public override HttpCookieCollection Cookies
        {
            get { return _cookies; }
        }

        public override string AppRelativeCurrentExecutionFilePath
        {
            get { return _relativeUrl; }
        }

        public override Uri Url
        {
            get
            {
                return _url;
            }
        }

        public override Uri UrlReferrer
        {
            get
            {
                return _urlReferrer;
            }
        }

        public override string PathInfo
        {
            get { return ""; }
        }

        public override string ApplicationPath
        {
            get
            {
                if (_relativeUrl != null && _relativeUrl.StartsWith("~/"))
                    return _relativeUrl.Remove(0, 1);
                return null;
            }
        }

        public override string HttpMethod
        {
            get
            {
                return _httpMethod;
            }
        }

        public override string UserHostAddress
        {
            get { return null; }
        }

        public override string RawUrl
        {
            get { return null; }
        }
        public override bool IsSecureConnection
        {
            get { return false; }
        }
    }
}