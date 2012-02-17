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

		public void SendPost(string brd, string title, string text, int? pid=null, int? gid=null, int signature=0, string autocr="on")
		{
			ParameterList qry = new ParameterList();
			qry.Add("board", brd);
			ParameterList data = new ParameterList();
			data.Add("title", title);
			data.Add("text", text);
			if (pid != null)		// replying a post
			{
				data.Add("reid", pid.ToString());
				data.Add("pid", gid.ToString());
			}
			data.Add("signature", signature.ToString());
			data.Add("autocr", autocr);
			DoAction(SendPostCompleted, "bbssnd", qry, data);
		}

		private void SendPostCompleted(object sender, BaseEventArgs e)
		{
			if (callback == null) return;
			callback(this, new BaseEventArgs(e.Result, e.Error));
		}
	}
}
