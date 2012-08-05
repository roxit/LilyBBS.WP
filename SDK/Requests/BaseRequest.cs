using System;
using System.Net;
using System.Text;

namespace LilyBBS.SDK
{
	public delegate void BaseHandler(object sender, BaseEventArgs e);

	public class BaseEventArgs : EventArgs
	{
		public object Result { get; private set; }
		public LilyError Error { get; private set; }
		public BaseEventArgs(object result = null, LilyError error = null)
		{
			Result = result;
			Error = error;
		}
	}

	public class BaseRequest
	{
		protected Connection connection;
		protected WebClient client;
		protected BaseHandler callback;
		protected BaseHandler innerCallback;
		protected string url;

		public BaseRequest(Connection connection, BaseHandler callback)
		{
			this.connection = connection;
			this.callback = callback;
			this.innerCallback = null;

			client = new WebClient();
			client.Encoding = Utils.Enc;
			client.UploadStringCompleted += client_UploadStringCompleted;
			client.DownloadStringCompleted += client_DownloadStringCompleted;
			if (connection.Cookie != null)
			{
				client.Headers["Cookie"] = connection.Cookie;
			}
		}

		protected void client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
		{
			if (e.Cancelled) return;
			if (e.Error != null)
			{
				if (callback != null)
					callback(this, new BaseEventArgs(null, new NetworkError(url)));
				return;
			}
			if (innerCallback != null)
			{
				innerCallback(this, new BaseEventArgs(e.Result));
			}
		}

		protected void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
		{
			if (e.Cancelled) return;
			if (e.Error != null)
			{
				if (callback != null)
					callback(this, new BaseEventArgs(null, new NetworkError(url)));
				return;
			}
			if (innerCallback != null)
			{
				innerCallback(this, new BaseEventArgs(e.Result));
			}
		}

		protected void DoAction(BaseHandler innerCallback, string action, ParameterList qry = null, ParameterList data = null)
		{
			this.innerCallback = innerCallback;
			StringBuilder sb = new StringBuilder(connection.BaseUrl);
			sb.Append(action);
			sb.Append("?");
			if (qry != null) sb.Append(qry.BuildQueryString());
			string url = sb.ToString();
			this.url = url;
			if (data != null)
				client.UploadStringAsync(new Uri(url), data.BuildQueryString());
			else
				client.DownloadStringAsync(new Uri(url));
		}
	}
}
