using System.Net;
using System.Text;
using System;

namespace LilyBBS.API
{
	public delegate void BaseHandler(object sender, BaseEventArgs e);

	public class BaseEventArgs : EventArgs
	{
		public object Result { get; private set; }
		public Exception Error { get; private set; }
		public BaseEventArgs(object result = null, Exception error = null)
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
		private BaseHandler innerCallback;

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
			// TODO see client.CancelAsync docs
			//if (e.Cancelled) return;
			if (innerCallback != null)
			{
				innerCallback(this, new BaseEventArgs(e.Result, e.Error));
			}
		}

		protected void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
		{
			//if (e.Cancelled) return;
			if (innerCallback != null)
			{
				innerCallback(this, new BaseEventArgs(e.Result, e.Error));
			}
		}

		protected void DoAction(BaseHandler innerCallback, string action, ParameterList qry = null, ParameterList data = null)
		{
			/*
			if (client.IsBusy)
			{
				client.CancelAsync();
			}
			*/
			this.innerCallback = innerCallback;
			
			StringBuilder sb = new StringBuilder(connection.BaseUrl);
			sb.Append(action);
			sb.Append("?");
			if (qry != null) sb.Append(qry.BuildQueryString());
			string url = sb.ToString();
			if (data != null)
			{
				client.UploadStringAsync(new Uri(url), data.BuildQueryString());
			}
			else
			{
				client.DownloadStringAsync(new Uri(url));
			}
		}
	}
}
