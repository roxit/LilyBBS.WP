using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace LilyBBS.API
{
	public class SendPostRequest : BaseRequest
	{
		public SendPostRequest(Connection connection, BaseHandler callback)
			: base(connection, callback)
		{
		}

		public void SendPost(string brd, string title, string text, string autocr = "on", int signature = 1)
		{
			ParameterList qry = new ParameterList();
			qry.Add("board", brd);
			ParameterList data = new ParameterList();
			data.Add("title", title);
			data.Add("text", text);
			data.Add("autocr", autocr);
			data.Add("signature", signature.ToString());
			DoAction(SendPostCompleted, "bbssnd", qry, data);
		}

		private void SendPostCompleted(object sender, BaseEventArgs e)
		{
			if (callback == null) return;
			callback(this, new BaseEventArgs(e.Result, e.Error));
		}
	}
}
