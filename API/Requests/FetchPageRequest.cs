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
		private static readonly Regex COUNT_RE = new Regex("<font color=\"(black|red)\">(\\d+)</font>/<font color=\"(black|red)\">(\\d+)</font>");
		private static readonly Regex PREV_START_RE = new Regex(@"bbstdoc\?board=(\w+)\&start=(\d+)>上一页");

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
			string txt = doc.DocumentNode.SelectSingleNode("//table").InnerHtml;

			Page page = new Page(Board);
			var r = PREV_START_RE.Match(e.Result as string);
			page.PrevStart = int.Parse(PREV_START_RE.Match(e.Result as string).Groups[2].ToString())-1;

			List<Header> headerList = new List<Header>();
			var authorList = AUTHOR_RE.Matches(txt);
			var pidList = PID_RE.Matches(txt);
			var titleList = TITLE_RE.Matches(txt);
			var countList = COUNT_RE.Matches(txt);
			int diff = authorList.Count - countList.Count;
			// skip fixed posts, and append headers in reverse order
			for (int i = authorList.Count-1; i >= diff; i--)
			{
				Header h = new Header();
				h.Author = authorList[i].Groups[1].ToString();
				h.Board = page.Board;
				h.Pid = int.Parse(pidList[i].Groups[2].ToString());
				h.Title = titleList[i].Groups[1].ToString().Remove(0, 2);
				h.ReplyCount = int.Parse(countList[i-diff].Groups[2].ToString());
				h.ViewCount = int.Parse(countList[i-diff].Groups[4].ToString());
				page.HeaderList.Add(h);
			}
			callback(this, new BaseEventArgs(page));
		}
	}
}
