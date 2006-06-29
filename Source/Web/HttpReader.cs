/*
 * File:    HttpReader.cs
 * Created: 01/17/2003
 * Author:  Igor Tkachev
 *          mailto:it@rsdn.ru
 */

using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace BLToolkit.Web
{
	/// <summary>
	/// Encapsulates WebReader functions.
	/// </summary>
	public class HttpReader
	{
		public HttpReader()
		{
		}

		public HttpReader(string baseUri)
		{
			BaseUri = baseUri;
		}

		private X509Certificate _certificate;
		public  X509Certificate  Certificate
		{
			get { return _certificate;  }
			set { _certificate = value; }
		}

		private string _baseUri;
		public  string  BaseUri
		{
			get { return _baseUri;  }
			set { _baseUri = value; }
		}

		private string _previousUri;
		public  string  PreviousUri
		{
			get { return _previousUri;  }
			set { _previousUri = value; }
		}

		private CookieContainer _cookieContainer = new CookieContainer();
		public  CookieContainer  CookieContainer
		{
			get { return _cookieContainer;  }
			set { _cookieContainer = value; }
		}

		private string _userAgent = @"HttpReader";
		public  string  UserAgent
		{
			get { return _userAgent;  }
			set { _userAgent = value; }
		}

		private string _accept =
			@"image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-excel, application/vnd.ms-powerpoint, */*";
		public  string  Accept
		{
			get { return _accept;  }
			set { _accept = value; }
		}

		private Uri _requestUri;
		public  Uri  RequestUri
		{
			get { return _requestUri;  }
			set { _requestUri = value; }
		}

		private string _contentType = string.Empty;
		public  string  ContentType
		{
			get { return _contentType;  }
			set { _contentType = value; }
		}

		private IWebProxy _proxy = new WebProxy();
		public  IWebProxy  Proxy
		{
			get { return _proxy;  }
			set { _proxy = value; }
		}

		private ICredentials _credentials = CredentialCache.DefaultCredentials;
		public  ICredentials  Credentials
		{
			get { return _credentials;  }
			set { _credentials = value; }
		}

		private string _html;
		public  string  Html
		{
			get { return _html; }
		}

		private Hashtable _headers = new Hashtable();
		public  Hashtable  Headers
		{
			get { return _headers; }
		}

		private string _location;
		public  string  Location
		{
			get { return _location; }
		}

		private bool _sendReferer = true;
		public  bool  SendReferer
		{
			get { return _sendReferer;  }
			set { _sendReferer = value; }
		}

		private HttpStatusCode _statusCode;
		public  HttpStatusCode  StatusCode
		{
			get { return _statusCode; }
		}

		private int _timeout;
		public  int  Timeout
		{
			get { return _timeout;  }
			set { _timeout = value; }
		}

		public HttpStatusCode Request(string requestUri, string method, string postData)
		{
			string uri = BaseUri + requestUri;

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

			if (Proxy       != null) request.Proxy       = Proxy;
			if (Credentials != null) request.Credentials = Credentials;

			request.CookieContainer = CookieContainer;
			request.UserAgent       = UserAgent;
			request.Accept          = Accept;
			request.Method          = method;
			request.KeepAlive       = true;

			if (SendReferer)
				request.Referer = PreviousUri != null? PreviousUri: uri;

			foreach (string key in Headers.Keys)
				request.Headers.Add(key, Headers[key].ToString());

			if (method == "POST")
			{
				request.ContentType       = "application/x-www-form-urlencoded";
				request.AllowAutoRedirect = false;
			}
			else
			{
				request.ContentType       = ContentType;
				request.AllowAutoRedirect = true;
			}

			PreviousUri = uri;

			if (Certificate != null)
				request.ClientCertificates.Add(Certificate);

			if (Timeout != 0)
				request.Timeout = Timeout;

			if (postData != null)
			{
				using (Stream st = request.GetRequestStream())
				{
					byte[] bytes = Encoding.ASCII.GetBytes(postData);
					st.Write(bytes,0,bytes.Length);
				}
			}

			_html = "";

			using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
			using (Stream          sm   = resp.GetResponseStream())
			using (StreamReader    sr   = new StreamReader(sm, Encoding.Default))
			{
				_statusCode = resp.StatusCode;
				_location   = resp.Headers["Location"];

				_html = sr.ReadToEnd();

				if (resp.ResponseUri.AbsoluteUri.StartsWith(BaseUri) == false)
					BaseUri = resp.ResponseUri.Scheme + "://" + resp.ResponseUri.Host;

				CookieCollection cc = request.CookieContainer.GetCookies(request.RequestUri);

				// This code fixes the situation when a server sets a cookie without the 'path'.
				// IE takes this as the root ('/') value,
				// the HttpWebRequest class as the RequestUri.AbsolutePath value.
				//
				foreach (Cookie c in cc)
				{
					if (c.Path == request.RequestUri.AbsolutePath)
					{
						CookieContainer.Add(new Cookie(c.Name, c.Value, "/", c.Domain));
					}

					string d = c.Domain;
					int    n = d.Length;
				}
			}

			RequestUri = request.RequestUri;

			return StatusCode;
		}

		public HttpStatusCode Get(string requestUri)
		{
			return Request(requestUri, "GET", null);
		}

		public HttpStatusCode Post(string requestUri, string postData)
		{
			Request(requestUri, "POST", postData);

			for (int i = 0; i < 10; i++)
			{
				bool post = false;

				switch (StatusCode)
				{
					case HttpStatusCode.MultipleChoices:   // 300
					case HttpStatusCode.MovedPermanently:  // 301
					case HttpStatusCode.Found:             // 302
					case HttpStatusCode.SeeOther:          // 303
						break;

					case HttpStatusCode.TemporaryRedirect: // 307
						post = true;
						break;

					default:
						return StatusCode;
				}

				if (Location == null)
					break;

				Uri uri = new Uri(new Uri(PreviousUri), Location);

				BaseUri    = uri.Scheme + "://" + uri.Host;
				requestUri = uri.AbsolutePath + uri.Query;

				Request(requestUri, post? "POST": "GET", post? postData: null);
			}

			return StatusCode;
		}

		public void LoadCertificate(string fileName)
		{
			Certificate = X509Certificate.CreateFromCertFile(fileName);
		}
	}
}