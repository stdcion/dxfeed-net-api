﻿#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.IO;
using System.Net;
using System.Text;
using com.dxfeed.api;

namespace com.dxfeed.io
{
    /// <summary>
    /// Convenient class that opens specified URL for reading.
    /// </summary>
    public class URLInputStream
    {
        private const int ReadTimeout = 60000;

        static URLInputStream() {
            Tools.AddTls11PlusSupport();
        }
        
        /// <summary>
        /// Resolves a given URL in the context of the current file.
        /// </summary>
        /// <param name="url">Url, relative, or absolute file name.</param>
        /// <returns>Resolved url.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        public static Uri ResolveUrl(string url)
        {
            if (url.Length > 2 && url[1] == ':' && Path.PathSeparator == '\\')
                url = "/" + url; // special case for full file path with drive letter on windows
            return new Uri(url);
        }

        /// <summary>
        /// Opens WebRequest for a specified URL. This method resolves
        /// specified URL first, for a proper support of file name.
        /// Use CheckConnectionResponseCode(WebResponse) after establishing
        /// connection to ensure that it was Ok.
        /// This is a shortcut for OpenConnection(ResolveURL(URL), null, null).
        /// </summary>
        /// <param name="url">Url the URL.</param>
        /// <returns>A new WebRequest object.</returns>
        /// <exception cref="NotSupportedException">The request scheme specified in requestUri is not registered.</exception>
        /// <exception cref="ArgumentNullException">RequestUri is null.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have permission to connect
        /// to the requested URI or a URI that the request is redirected to.</exception>
        public static WebRequest OpenConnection(string url)
        {
            return OpenConnection(ResolveUrl(url), null, null);
        }

        /// <summary>
        /// Opens WebRequest for a specified URL with a specified basic user and password credentials.
        /// Use CheckConnectionResponseCode(WebResponse) after establishing
        /// connection to ensure that it was Ok.
        /// Credentials are used only when both user and password are non-null and non-empty.
        /// Specified credentials take precedence over authentication information that is supplied to this method
        /// as part of URL user info like "http://user:password@host:port/path/file".
        /// </summary>
        /// <param name="url">Url the URL string.</param>
        /// <param name="user">The user name (may be null).</param>
        /// <param name="password">The password (may be null).</param>
        /// <returns>A new WebRequest object.</returns>
        /// <exception cref="NotSupportedException">The request scheme specified in requestUri is not registered.</exception>
        /// <exception cref="ArgumentNullException">RequestUri is null.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have permission to connect to the requested URI or a URI that the request is redirected to.</exception>
        public static WebRequest OpenConnection(string url, string user, string password)
        {
            return OpenConnection(ResolveUrl(url), user, password);
        }

        /// <summary>
        /// Opens WebRequest for a specified URL with a specified basic user and password credentials.
        /// Use CheckConnectionResponseCode(WebResponse) after establishing
        /// connection to ensure that it was Ok.
        /// Credentials are used only when both user and password are non-null and non-empty.
        /// Specified credentials take precedence over authentication information that is supplied to this method
        /// as part of URL user info like "http://user:password@host:port/path/file".
        /// </summary>
        /// <param name="url">Url the URL.</param>
        /// <param name="user">The user name (may be null).</param>
        /// <param name="password">The password (may be null).</param>
        /// <returns>A new WebRequest object.</returns>
        /// <exception cref="NotSupportedException">The request scheme specified in requestUri is not registered.</exception>
        /// <exception cref="ArgumentNullException">RequestUri is null.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have permission to connect to the requested URI or a URI that the request is redirected to.</exception>
        public static WebRequest OpenConnection(Uri url, string user, string password)
        {
            var webRequest = WebRequest.Create(url);
            webRequest.Timeout = ReadTimeout;
            string auth;
            if (user != null && !string.IsNullOrEmpty(user) && password != null && !string.IsNullOrEmpty(password))
                auth = user + ":" + password;
            else
                auth = url.UserInfo;
            if (auth != null && !string.IsNullOrEmpty(auth))
                webRequest.Headers.Add("Authorization", "Basic " +
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(auth)));
            return webRequest;
        }

        /// <summary>
        /// Opens WebRequest for a specified URL with a specified bearer token.
        /// Use CheckConnectionResponseCode(WebResponse) after establishing connection to ensure that it was Ok.
        /// </summary>
        /// <param name="url">Url the URL string.</param>
        /// <param name="token">The bearer authorization's token</param>
        /// <returns>A new WebRequest object.</returns>
        /// <exception cref="NotSupportedException">The request scheme specified in requestUri is not registered.</exception>
        /// <exception cref="ArgumentNullException">RequestUri is null or token is null</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have permission to connect to the requested URI or a URI that the request is redirected to.</exception>
        public static WebRequest OpenConnection(string url, string token)
        {
            return OpenConnection(ResolveUrl(url), token);
        }

        /// <summary>
        /// Opens WebRequest for a specified URL with a specified bearer token.
        /// Use CheckConnectionResponseCode(WebResponse) after establishing connection to ensure that it was Ok.
        /// </summary>
        /// <param name="url">Url the URL.</param>
        /// <param name="token">The bearer authorization's token</param>
        /// <returns>A new WebRequest object.</returns>
        /// <exception cref="NotSupportedException">The request scheme specified in requestUri is not registered.</exception>
        /// <exception cref="ArgumentNullException">RequestUri is null or token is null</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have permission to connect to the requested URI or a URI that the request is redirected to.</exception>
        public static WebRequest OpenConnection(Uri url, string token)
        {
            var webRequest = WebRequest.Create(url);
            webRequest.Timeout = ReadTimeout;

            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }
            
            if (!string.IsNullOrEmpty(token))
                webRequest.Headers.Add("Authorization", $"Bearer {token}");
            return webRequest;
        }

        /// <summary>
        /// Checks connection response code and throws IOException if it is not Ok.
        /// </summary>
        /// <param name="webResponse">Response from the server.</param>
        /// <exception cref="IOException">If an I/O error occurs.</exception>
        public static void CheckConnectionResponseCode(WebResponse webResponse)
        {
            if (webResponse.GetType() == typeof(HttpWebResponse) && (((HttpWebResponse)webResponse).StatusCode != HttpStatusCode.OK))
                throw new IOException("Unexpected response: " + webResponse.Headers.Get(0));
        }
    }
}
