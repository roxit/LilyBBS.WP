using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace LilyBBS.API
{
	public class FetchTopTenListRequest : BaseRequest
	{
		private static readonly Regex PID_RE = new Regex(@"bbstcon\?board=(\w+)\&file=M\.(\d+)\.A", RegexOptions.Compiled);
		private static readonly Regex TITLE_RE = new Regex(@"bbstcon\?board=.+?>(.*?)\n</a>", RegexOptions.Compiled);
		private static readonly Regex AUTHOR_RE = new Regex(@"bbsqry\?userid=(\w+)", RegexOptions.Compiled);

		public FetchTopTenListRequest(Connection connection, BaseHandler callback)
			: base(connection, callback)
		{
		}

		public void FetchTopTenList()
		{
			DoAction(FetchTopTenListCompleted, "bbstop10");
		}

		private void FetchTopTenListCompleted(object sender, BaseEventArgs e)
		{
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(e.Result as string);
			List<Header> headerList = new List<Header>();
			var items = doc.DocumentNode.SelectNodes("//tr");
			// TODO I'll try LINQ later
			bool flag = true;
			foreach (var i in items)
			{
				if (flag)
				{
					flag = false; continue;
				}
				Header h = new Header();
				Match m = PID_RE.Match(i.InnerHtml);
				h.Board = m.Groups[1].ToString().Trim();
				h.Pid = int.Parse(m.Groups[2].ToString().Trim());
				h.Title = TITLE_RE.Match(i.InnerHtml).Groups[1].ToString().Trim();
				h.Author = AUTHOR_RE.Match(i.InnerHtml).Groups[1].ToString().Trim();
				headerList.Add(h);
			}
			callback(this, new BaseEventArgs(headerList));
		}
	}
}
