using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace LilyBBS.SDK
{
	public class FetchPageRequest : BaseRequest
	{
		private static readonly Regex PID_RE = new Regex(@"bbstcon\?board=(\w+)\&file=M\.(\d+)\.A", RegexOptions.Compiled);
		private static readonly Regex TITLE_RE = new Regex(@"bbstcon\?board=.+?>(.*?)</a>", RegexOptions.Compiled);
		private static readonly Regex AUTHOR_RE = new Regex(@"bbsqry\?userid=(\w+)", RegexOptions.Compiled);
		private static readonly Regex COUNT_RE = new Regex("<font color=\"(black|red)\">(\\d+)</font>/<font color=\"(black|red)\">(\\d+)</font>", RegexOptions.Compiled);
		private static readonly Regex PREV_START_RE = new Regex(@"bbstdoc\?board=(\w+)\&start=(\d+)>上一页", RegexOptions.Compiled);

		private string board;
		private int? start;

		public FetchPageRequest(Connection connection, BaseHandler callback)
			: base(connection, callback)
		{
		}

		public void FetchPage(string board, int? start=null)
		{
			this.board = board;
			this.start = start;
			ParameterList qry = new ParameterList();
			qry.Add("board", board);
			if (start != null)
				qry.Add("start", start.Value.ToString());
			DoAction(this.FetchPageCompleted, "bbstdoc", qry);
		}

		private void FetchPageCompleted(object sender, BaseEventArgs e)
		{
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(e.Result as string);
			string txt = doc.DocumentNode.SelectSingleNode("//table").InnerHtml;

			Page page = new Page(board);
			var r = PREV_START_RE.Match(e.Result as string);
			// TODO check PrevStart
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
