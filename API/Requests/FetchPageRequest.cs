using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace LilyBBS.API
{
	public class FetchPageRequest : BaseRequest
	{
		private static readonly Regex PID_RE = new Regex(@"bbstcon\?board=(\w+)\&file=M\.(\d+)\.A");
		private static readonly Regex TITLE_RE = new Regex(@"bbstcon\?board=.+?>(.*?)</a>");
		private static readonly Regex AUTHOR_RE = new Regex(@"bbsqry\?userid=(\w+)");

		private string Board;
		private int Start;

		public FetchPageRequest(Connection connection, BaseHandler callback)
			: base(connection, callback)
		{
		}

		public void FetchPage(string board, int start)
		{
			this.Board = board;
			this.Start = start;
			ParameterList qry = new ParameterList();
			qry.Add("board", board);
			if (start != -1)
				qry.Add("start", start.ToString());
			DoAction(this.FetchPageCompleted, "bbstdoc", qry);
		}

		private void FetchPageCompleted(object sender, BaseEventArgs e)
		{
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(e.Result as string);
			List<Header> headerList = new List<Header>();

			string txt = doc.DocumentNode.SelectSingleNode("//table").InnerHtml;
			Page page = new Page(Board);
			page.Start = Start;

			var authorList = AUTHOR_RE.Matches(txt);
			var pidList = PID_RE.Matches(txt);
			var titleList = TITLE_RE.Matches(txt);
			for (int i = 0; i < authorList.Count; i++)
			{
				Header h = new Header();
				h.Author = authorList[i].Groups[1].ToString().Trim();
				h.Board = page.Board;
				h.Pid = int.Parse(pidList[i].Groups[2].ToString().Trim());
				h.Title = titleList[i].Groups[1].ToString().Trim().Remove(0, 2);
				page.HeaderList.Add(h);
			}
			callback(this, new BaseEventArgs(page));
		}
	}
}
