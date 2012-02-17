using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace LilyBBS.API
{
	public class FetchPostRequest : BaseRequest
	{
		private static readonly Regex GID_RE = new Regex(@"</a>\]\[<a href='bbstfind\?board=(\w+)\&gid=(\d+)' >同主题阅读</a>");
		private int Pid;
		private string Board;
		private int Num;

		public FetchPostRequest(Connection connection, BaseHandler callback)
			: base(connection, callback)
		{
		}

		public void FetchPost(string board, int pid, int num)
		{
			this.Pid = pid;
			this.Board = board;
			this.Num = num;

			ParameterList qry = new ParameterList();
			qry.Add("board", board);
			qry.Add("file", Utils.Pid2Str(pid));
			// Without `num`, the gid in the returned page is 0
			qry.Add("num", num.ToString());
			DoAction(FetchPostCompleted, "bbscon", qry);
		}

		private void FetchPostCompleted(object sender, BaseEventArgs e)
		{
			if (this.callback == null) return;
			Post post = new Post(Board, Pid, Num);
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(e.Result as string);
			string txt = doc.DocumentNode.SelectSingleNode("//textarea").InnerHtml;
			post.ParsePost(txt);
			post.Gid = int.Parse(GID_RE.Match(e.Result as string).Groups[2].Value);
			callback(this, new BaseEventArgs(post));
		}
	}
}
