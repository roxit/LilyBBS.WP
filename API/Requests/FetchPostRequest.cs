using HtmlAgilityPack;

namespace LilyBBS.API
{
	public class FetchPostRequest : BaseRequest
	{
		private int Pid;
		private string Board;
		private int Num;

		public FetchPostRequest(Connection connection, BaseHandler callback)
			: base(connection, callback)
		{
		}

		public void FetchPost(int pid, string board, int num)
		{
			this.Pid = pid;
			this.Board = board;
			this.Num = num;

			ParameterList qry = new ParameterList();
			qry.Add("board", board);
			qry.Add("file", Utils.Pid2Str(pid));
			qry.Add("num", num.ToString());
			DoAction(FetchPostCompleted, "bbscon", qry);
		}

		private void FetchPostCompleted(object sender, BaseEventArgs e)
		{
			if (this.callback == null) return;
			Post post = new Post(Pid, Board, Num);
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(e.Result as string);
			string txt = doc.DocumentNode.SelectNodes("//textarea")[0].InnerHtml;
			post.ParsePost(txt);
			callback(this, new BaseEventArgs(post));
		}
	}
}
